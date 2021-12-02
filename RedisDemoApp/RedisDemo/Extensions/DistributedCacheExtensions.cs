using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RedisDemo.Extensions
{
    public static class DistributedCacheExtensions
    {
        public static async Task SetRecordAsync<T>(this IDistributedCache cache,
            string recordId,
            T data,
            TimeSpan? absoluteExpirTime = null,
            TimeSpan? unusedExpireTime = null)
        {
            var options = new DistributedCacheEntryOptions();

            //this is an options:
            //once you put the item in the cache, it will live for a total of one minute
            options.AbsoluteExpirationRelativeToNow = absoluteExpirTime ?? TimeSpan.FromSeconds(60);


            //this is an options: (not often to be used)
            // if you're not accessing the cache data for amount of time,
            // it will expire
            options.SlidingExpiration = unusedExpireTime;


            // transform incoming data into Json format
            var jsonData = JsonSerializer.Serialize(data);


            // parse the key id, data format, any options into the cache
            await cache.SetStringAsync(recordId, jsonData, options);
        }


        public static async Task<T> GetRecrodAsync<T>(this IDistributedCache cache, string recordId)
        {
            // get the string based upon the key value, store into variable
            var jsonData = await cache.GetStringAsync(recordId);


            if(jsonData is null)
            {
                // default means the specific data type
                //for example int default value is 0, not null.
                return default(T);
            }

            // deserialize the value into the model you were given and return
            return JsonSerializer.Deserialize<T>(jsonData);

        }



    }
}
