using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using KC.Framework.Util;

namespace KC.Service.Util
{
    public class LocalWebCacheUtil
    {
        private static string ServiceName = "KC.Service.Util.LocalWebCacheUtil";
        private static HashSet<string> CacheKeyList = new HashSet<string>();
        private static readonly object _cacheLock = new object();
        //private static readonly int DefaultCacheTimeOut = 30 * 24 * 60;

        public static object GetCache(string key)
        {
            try
            {
                var objString = System.Runtime.Caching.MemoryCache.Default.Get(key);
                return objString;
            }
            catch (Exception ex)
            {
                var title = string.Format(ServiceName + "：key({0})--方法(GetCache<T>)缓存对象时抛出异常：", key);
                var msg = string.Format("错误消息：{0}, " + Environment.NewLine + "堆栈消息：{1}",
                    ex.Message, ex.StackTrace);
                LogUtil.LogError(title, msg);

                return null;
            }
        }

        public static T GetCache<T>(string key)
        {
            try
            {
                var objString = System.Runtime.Caching.MemoryCache.Default.Get(key);
                return (T)objString;
            }
            catch (Exception ex)
            {
                var title = string.Format(ServiceName + "：key({0})--方法(GetCache<T>)缓存对象({1})时抛出异常：", key, typeof(T).FullName);
                var msg = string.Format("错误消息：{0}, " + Environment.NewLine + "堆栈消息：{1}",
                    ex.Message, ex.StackTrace);
                LogUtil.LogError(title, msg);

                return default(T);
            }
        }

        public static void SetCache<T>(string key, T obj)
        {
            try
            {
                var monitor = System.Runtime.Caching.MemoryCache.Default.CreateCacheEntryChangeMonitor(new string[] { key });

                //Insert data cache item with sliding timeout using changeMonitors
                var itemPolicy = new CacheItemPolicy();
                itemPolicy.ChangeMonitors.Add(monitor);
                itemPolicy.Priority = CacheItemPriority.NotRemovable;
                System.Runtime.Caching.MemoryCache.Default.Add(key, obj, itemPolicy);
                if (!CacheKeyList.Contains(key))
                    CacheKeyList.Add(key);
            }
            catch (Exception ex)
            {
                var title = string.Format(ServiceName + "：key({0})--方法(SetCache<T>)缓存对象({1})时抛出异常：", key, typeof(T).FullName);
                var msg = string.Format("错误消息：{0}, " + Environment.NewLine + "堆栈消息：{1}",
                    ex.Message, ex.StackTrace);
                LogUtil.LogError(title, msg);
            }
        }

        /// <summary>
        /// Only KC.Common.CacheHelper.SysWebCache 
        ///     & CFW.Azure.Common.Util.Cache.AzureCacheService 
        ///     & CFW.Azure.Common.Util.Cache.RedisCacheService 
        ///     implement the method of the ICache interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="slidingExpiration"></param>
        public static void SetCache<T>(string key, T obj, TimeSpan slidingExpiration)
        {
            try
            {
                var monitor = System.Runtime.Caching.MemoryCache.Default.CreateCacheEntryChangeMonitor(new string[] { key });

                //Insert data cache item with sliding timeout using changeMonitors
                var itemPolicy = new CacheItemPolicy();
                itemPolicy.ChangeMonitors.Add(monitor);
                itemPolicy.SlidingExpiration = slidingExpiration;
                System.Runtime.Caching.MemoryCache.Default.Add(key, obj, itemPolicy);
                if (!CacheKeyList.Contains(key))
                    CacheKeyList.Add(key);
            }
            catch (Exception ex)
            {
                var title = string.Format(ServiceName + "：key({0})--方法(SetCache<T>)缓存对象({1})时抛出异常：", key, typeof(T).FullName);
                var msg = string.Format("错误消息：{0}, " + Environment.NewLine + "堆栈消息：{1}",
                    ex.Message, ex.StackTrace);
                LogUtil.LogError(title, msg);
            }
        }

        /// <summary>
        /// Only KC.Common.CacheHelper.SysWebCache implement the method of the ICache interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="absoluteExpiration"></param>
        public static void SetCache<T>(string key, T obj, DateTime absoluteExpiration)
        {
            try
            {
                var offset = new DateTimeOffset(DateTime.UtcNow, absoluteExpiration.Subtract(DateTime.UtcNow));
                var monitor = System.Runtime.Caching.MemoryCache.Default.CreateCacheEntryChangeMonitor(new string[] { key });

                //Insert data cache item with sliding timeout using changeMonitors
                var itemPolicy = new CacheItemPolicy();
                itemPolicy.ChangeMonitors.Add(monitor);
                itemPolicy.AbsoluteExpiration = offset;
                System.Runtime.Caching.MemoryCache.Default.Add(key, obj, itemPolicy);
                if (!CacheKeyList.Contains(key))
                    CacheKeyList.Add(key);
            }
            catch (Exception ex)
            {
                var title = string.Format(ServiceName + "：key({0})--方法(SetCache<T>)缓存对象({1})时抛出异常：", key, typeof(T).FullName);
                var msg = string.Format("错误消息：{0}, " + Environment.NewLine + "堆栈消息：{1}",
                    ex.Message, ex.StackTrace);
                LogUtil.LogError(title, msg);
            }
        }

        public static void RemoveCache(string key)
        {
            try
            {
                if (CacheKeyList.Contains(key))
                    CacheKeyList.Remove(key);

                System.Runtime.Caching.MemoryCache.Default.Remove(key);
            }
            catch (Exception ex)
            {
                var title = string.Format(ServiceName + "：key({0})--方法(RemoveCache)缓存对象时抛出异常：", key);
                var msg = string.Format("错误消息：{0}, " + Environment.NewLine + "堆栈消息：{1}",
                    ex.Message, ex.StackTrace);
                LogUtil.LogError(title, msg);
            }
        }

        public static void RemoveAllCache()
        {
            try
            {
                System.Runtime.Caching.MemoryCache.Default.Dispose();
            }
            catch (Exception ex)
            {
                var title = string.Format(ServiceName + "：key--方法(RemoveAllCache)缓存对象时抛出异常：");
                var msg = string.Format("错误消息：{0}, " + Environment.NewLine + "堆栈消息：{1}",
                    ex.Message, ex.StackTrace);
                LogUtil.LogError(title, msg);
            }
        }

        public static List<string> GetAllCacheKey()
        {
            return CacheKeyList.ToList();
        }

        public static bool ExistKey(string key)
        {
            return CacheKeyList.Contains(key);
        }

        public static void RemoveGroupKey(string groupName)
        {
            groupName = "group-" + groupName.ToLower();
            var groupList = CacheKeyList.Where(m => m.StartsWith(groupName)).ToList();
            if (groupList != null && groupList.Any())
            {
                var _cache = System.Runtime.Caching.MemoryCache.Default;
                foreach (var item in groupList)
                {
                    _cache.Remove(item);
                    CacheKeyList.Remove(item);
                }
            }
        }
    }
}
