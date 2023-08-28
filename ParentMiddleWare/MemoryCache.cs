using ParentMiddleWare.ApiModels;
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
            try
            {
                if (_object == null) return;
                var cacheEntryOptions = new CacheItemPolicy();
                cacheEntryOptions.SlidingExpiration = TimeSpan.FromMinutes(120);
                cacheEntryOptions.Priority = System.Runtime.Caching.CacheItemPriority.NotRemovable;


                CacheItem cacheitem = new CacheItem(key, _object);
                _memoryCache.Set(cacheitem, cacheEntryOptions);
            }
            catch { }
        }

        public static object Get(string key)
        {
            return _memoryCache.Get(key);
        }

        public static void Remove(string key)
        {
            _memoryCache.Remove(key);
        }

        public static async Task<List<UserSetHistory>> GetUserHistory()
        {
            try
            {
                if (_memoryCache.Get("SetHistory" + MiddleWare.FkFederatedUser) == null)
                {
                    var stats = await MiddleWare.GetSetHistory();
                    Set("SetHistory" + MiddleWare.FkFederatedUser, stats);
                }

                return _memoryCache.Get("SetHistory" + MiddleWare.FkFederatedUser) as List<UserSetHistory>;
            }
            catch
            {
                return new List<UserSetHistory>();
            }
        }


        public static async Task RefreshUserHistory()
        {
            try
            {
                var stats = await MiddleWare.GetSetHistory();
                Set("SetHistory" + MiddleWare.FkFederatedUser, stats);
            }
            catch
            {
            }
        }

    }


    public class UserSetHistory
    {
        public string FkFederatedUser { get; set; }
        public long ExerciseTypeId { get; set; }
        public short SetNumber { get; set; }
        public string SetString { get; set; }
    }


    public class MemoryCacheItem
    {
        public string Id { get; set; }
        public object _object { get; set; }
    }





}



