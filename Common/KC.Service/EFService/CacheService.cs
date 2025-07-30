using KC.Common;
using KC.Framework.Util;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service
{
    public class CacheUtil
    {
        protected static int DatabaseId = 5;

        private static readonly object _cacheLock = new object();
        private static readonly int DefaultCacheTimeOut = 30 * 24 * 60;//30天
        private static ConcurrentBag<string> keys = new ConcurrentBag<string>();

        private static IDistributedCache _cache;
        public static IDistributedCache Cache
        {
            get { return _cache; }
            set
            {
                if (_cache == value)
                    return;
                if (_cache == null)
                {
                    _cache = value;
                    return;
                }
                //throw new Exception("Cache service must only be set once.");
            }
        }

        #region 设置缓存
        public static void SetCache<T>(string key, T obj)
        {
            if (_cache == null)
                throw new Exception("Cache service not initialized");

            try
            {
                var option = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(DefaultCacheTimeOut)
                };
                var stringObj = SerializeHelper.ToJson(obj);
                _cache.SetString(key, stringObj, option);
                keys.Add(key);
            }
            catch (Exception ex)
            {
                string title = string.Format("key({0})--方法(SetCache<T>)缓存对象({1})时抛出异常: ", key, typeof(T).FullName);
                var msg = string.Format("错误消息: {0}, " + Environment.NewLine + "Stack Trace: {1}",
                    ex.Message, ex.StackTrace);
                LogUtil.LogError(title, msg);
            }
        }
        public static async Task SetCacheAsync<T>(string key, T obj)
        {
            if (_cache == null)
                throw new Exception("Cache service not initialized");

            try
            {
                var option = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(DefaultCacheTimeOut)
                };
                var stringObj = SerializeHelper.ToJson(obj);
                await _cache.SetStringAsync(key, stringObj, option);
                keys.Add(key);
            }
            catch (Exception ex)
            {
                string title = string.Format("key({0})--方法(SetCache<T>)缓存对象({1})时抛出异常: ", key, typeof(T).FullName);
                var msg = string.Format("错误消息: {0}, " + Environment.NewLine + "Stack Trace: {1}",
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
            if (_cache == null)
                throw new Exception("Cache service not initialized");

            try
            {
                var option = new DistributedCacheEntryOptions()
                {
                    SlidingExpiration = slidingExpiration
                };
                var stringObj = SerializeHelper.ToJson(obj);
                _cache.SetString(key, stringObj, option);
                keys.Add(key);
            }
            catch (Exception ex)
            {
                string title = string.Format("key({0})--方法(SetCache<T>)缓存对象({1})时抛出异常: ", key, typeof(T).FullName);
                var msg = string.Format("错误消息: {0}, " + Environment.NewLine + "Stack Trace: {1}",
                    ex.Message, ex.StackTrace);
                LogUtil.LogError(title, msg);
            }
        }
        public static async Task SetCacheAsync<T>(string key, T obj, TimeSpan slidingExpiration)
        {
            if (_cache == null)
                throw new Exception("Cache service not initialized");

            try
            {
                var option = new DistributedCacheEntryOptions()
                {
                    SlidingExpiration = slidingExpiration
                };
                var stringObj = SerializeHelper.ToJson(obj);
                await _cache.SetStringAsync(key, stringObj, option);
                keys.Add(key);
            }
            catch (Exception ex)
            {
                string title = string.Format("key({0})--方法(SetCache<T>)缓存对象({1})时抛出异常: ", key, typeof(T).FullName);
                var msg = string.Format("错误消息: {0}, " + Environment.NewLine + "Stack Trace: {1}",
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
            if (_cache == null)
                throw new Exception("Cache service not initialized");

            try
            {
                var option = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = absoluteExpiration - DateTime.UtcNow
                };
                var stringObj = SerializeHelper.ToJson(obj);
                _cache.SetString(key, stringObj, option);
                keys.Add(key);
            }
            catch (Exception ex)
            {
                string title = string.Format("key({0})--方法(SetCache<T>)缓存对象({1})时抛出异常: ", key, typeof(T).FullName);
                var msg = string.Format("错误消息: {0}, " + Environment.NewLine + "Stack Trace: {1}",
                    ex.Message, ex.StackTrace);
                LogUtil.LogError(title, msg);
            }
        }
        public static async Task SetCacheAsync<T>(string key, T obj, DateTime absoluteExpiration)
        {
            if (_cache == null)
                throw new Exception("Cache service not initialized");

            try
            {
                var option = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = absoluteExpiration - DateTime.UtcNow
                };
                var stringObj = SerializeHelper.ToJson(obj);
                await _cache.SetStringAsync(key, stringObj, option);
                keys.Add(key);
            }
            catch (Exception ex)
            {
                string title = string.Format("key({0})--方法(SetCache<T>)缓存对象({1})时抛出异常: ", key, typeof(T).FullName);
                var msg = string.Format("错误消息: {0}, " + Environment.NewLine + "Stack Trace: {1}",
                    ex.Message, ex.StackTrace);
                LogUtil.LogError(title, msg);
            }
        }
        #endregion

        #region 获取缓存
        public static T GetCache<T>(string key)
        {
            if (_cache == null)
                throw new Exception("Cache service not initialized");

            try
            {
                var stringObj = _cache.GetString(key);
                return SerializeHelper.FromJson<T>(stringObj);
            }
            catch (Exception ex)
            {
                string title = string.Format("key({0})--方法(GetCache<T>)缓存对象({1})时抛出异常: ", key, typeof(T).FullName);
                var msg = string.Format("错误消息: {0}, " + Environment.NewLine + "Stack Trace: {1}",
                    ex.Message, ex.StackTrace);
                LogUtil.LogError(title, msg);

                return default(T);
            }
        }
        public static async Task<T> GetCacheAsync<T>(string key)
        {
            if (_cache == null)
                throw new Exception("Cache service not initialized");

            try
            {
                var stringObj = await _cache.GetStringAsync(key);
                return SerializeHelper.FromJson<T>(stringObj);
            }
            catch (Exception ex)
            {
                string title = string.Format("key({0})--方法(GetCache<T>)缓存对象({1})时抛出异常: ", key, typeof(T).FullName);
                var msg = string.Format("错误消息: {0}, " + Environment.NewLine + "Stack Trace: {1}",
                    ex.Message, ex.StackTrace);
                LogUtil.LogError(title, msg);

                return default(T);
            }
        }

        public static T GetCache<T>(Type type, string key)
        {
            try
            {
                var stringObj = _cache.GetString(key);
                var value = SerializeHelper.FromJson(type, stringObj);
                return (T)value;
            }
            catch (Exception ex)
            {
                string title = string.Format("key({0})--方法(GetCache<T>)缓存对象({1})时抛出异常: ", key, typeof(T).FullName);
                var msg = string.Format("Message: {0}, " + Environment.NewLine + "Stack Trace: {1}",
                    ex.Message, ex.StackTrace);
                LogUtil.LogError(title, msg);

                return default(T);
            }

            //throw new Exception("Cache service not initialized");
        }

        public static async Task<T> GetCacheAsync<T>(Type type, string key)
        {
            try
            {
                var stringObj = await _cache.GetStringAsync(key);
                var value = SerializeHelper.FromJson(type, stringObj);
                return (T)value;
            }
            catch (Exception ex)
            {
                string title = string.Format("key({0})--方法(GetCache<T>)缓存对象({1})时抛出异常: ", key, typeof(T).FullName);
                var msg = string.Format("Message: {0}, " + Environment.NewLine + "Stack Trace: {1}",
                    ex.Message, ex.StackTrace);
                LogUtil.LogError(title, msg);

                return default(T);
            }

            //throw new Exception("Cache service not initialized");
        }
        #endregion

        #region 移除缓存
        public static void RemoveCache(string key)
        {
            if (_cache == null)
                throw new Exception("Cache service not initialized");

            try
            {
                _cache.Remove(key);
            }
            catch (Exception ex)
            {
                string title = string.Format("key({0})--方法(RemoveCache)缓存对象时抛出异常: ", key);
                var msg = string.Format("错误消息: {0}, " + Environment.NewLine + "Stack Trace: {1}",
                    ex.Message, ex.StackTrace);
                LogUtil.LogError(title, msg);
            }
        }
        public static async Task RemoveCacheAsync(string key)
        {
            if (_cache == null)
                throw new Exception("Cache service not initialized");

            try
            {
                await _cache.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                string title = string.Format("key({0})--方法(RemoveCache)缓存对象时抛出异常: ", key);
                var msg = string.Format("错误消息: {0}, " + Environment.NewLine + "Stack Trace: {1}",
                    ex.Message, ex.StackTrace);
                LogUtil.LogError(title, msg);
            }
        }

        public static void RemoveAllCache()
        {
            if (_cache == null)
                throw new Exception("Cache service not initialized");

            try
            {
                foreach (var key in keys)
                RemoveCache(key);
            }
            catch (Exception ex)
            {
                var title = "移除所有缓存对象出错：";
                var msg = string.Format("错误消息: {0}, " + Environment.NewLine + "Stack Trace: {1}",
                    ex.Message, ex.StackTrace);
                LogUtil.LogError(title, msg);
            }
        }
        public static async Task RemoveAllCacheAsync()
        {
            if (_cache == null)
                throw new Exception("Cache service not initialized");

            try
            {
                foreach (var key in keys)
                    await _cache.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                var title = "移除所有缓存对象出错：";
                var msg = string.Format("错误消息: {0}, " + Environment.NewLine + "Stack Trace: {1}",
                    ex.Message, ex.StackTrace);
                LogUtil.LogError(title, msg);
            }
        }
        #endregion

        public static List<string> GetAllCacheKey()
        {
            return keys.ToList();
        }
    }
}
