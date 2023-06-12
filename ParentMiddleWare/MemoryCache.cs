using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace ParentMiddleWare 
{
    public static class AirMemoryCache
    {

        public static MemoryCache _memoryCache = new MemoryCache("AirMemory");


        public static void Set(string key, object _object)
        {
            var cacheEntryOptions = new CacheItemPolicy();
            cacheEntryOptions.SlidingExpiration = TimeSpan.FromMinutes(60);
            cacheEntryOptions.Priority = System.Runtime.Caching.CacheItemPriority.NotRemovable;


            CacheItem cacheitem = new CacheItem(key, _object);
            _memoryCache.Set(cacheitem, cacheEntryOptions);
        }

        public static object Get(string key)
        {
            return _memoryCache.Get(key);
        }

        public static void Remove(string key)
        {
            _memoryCache.Remove(key);
        }

    }



    public class MemoryCacheItem
    {
        public string Id { get; set; }
        public object _object { get; set; }
    }





}



