using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using Com.Framework.Util;
using Com.Framework.Util.Cache;

namespace Com.Common.CacheHelper
{
    public class InMemoryCache : ICache
    {
        public T GetCache<T>(string key)
        {
            return (T)System.Runtime.Caching.MemoryCache.Default.Get(key);
        }
        public object GetCache(Type type, string key)
        {
            var objString = System.Runtime.Caching.MemoryCache.Default.Get(key);
            if (type == typeof(string))
            {
                return objString.ToString();
            }

            return objString;
        }

        public void SetCache<T>(string key, T obj) 
        {
            var monitor = System.Runtime.Caching.MemoryCache.Default.CreateCacheEntryChangeMonitor(new string[] { key });

            //Insert data cache item with sliding timeout using changeMonitors
            var itemPolicy = new CacheItemPolicy();
            itemPolicy.ChangeMonitors.Add(monitor);
            itemPolicy.Priority = CacheItemPriority.NotRemovable;
            System.Runtime.Caching.MemoryCache.Default.Add(key, obj, itemPolicy);
        }

        public void SetCache<T>(string key, T obj, TimeSpan slidingExpiration)
        {
            var monitor = System.Runtime.Caching.MemoryCache.Default.CreateCacheEntryChangeMonitor(new string[] {key});

            //Insert data cache item with sliding timeout using changeMonitors
            var itemPolicy = new CacheItemPolicy();
            itemPolicy.ChangeMonitors.Add(monitor);
            itemPolicy.SlidingExpiration = slidingExpiration;
            System.Runtime.Caching.MemoryCache.Default.Add(key, obj, itemPolicy);
        }

        public void SetCache<T>(string key, T obj, DateTime absoluteExpiration)
        {
            var offset = new DateTimeOffset(DateTime.UtcNow, absoluteExpiration.Subtract(DateTime.UtcNow));
            var monitor = System.Runtime.Caching.MemoryCache.Default.CreateCacheEntryChangeMonitor(new string[] { key });

            //Insert data cache item with sliding timeout using changeMonitors
            var itemPolicy = new CacheItemPolicy();
            itemPolicy.ChangeMonitors.Add(monitor);
            itemPolicy.AbsoluteExpiration = offset;
            System.Runtime.Caching.MemoryCache.Default.Add(key, obj, itemPolicy);
        }

        public void RemoveCache(string key)
        {
            System.Runtime.Caching.MemoryCache.Default.Remove(key);
        }

        public void RemoveAllCache()
        {
            System.Runtime.Caching.MemoryCache.Default.Dispose();
        }

        public int DatabaseId { get; set; }

        public List<string> GetCacheKeyList(string key)
        {
            throw new NotImplementedException();
        }

        public bool ExistKey(string key)
        {
            throw new NotImplementedException();
        }
    }
}
