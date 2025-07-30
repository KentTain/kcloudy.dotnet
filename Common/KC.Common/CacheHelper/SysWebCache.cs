using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections;
using System.Web.Caching;
using Com.Framework.Util.Cache;

namespace Com.Common.CacheHelper
{
    /// <summary>
    /// 缓存辅助类: System.Web.Caching.Cache
    /// </summary>
    public class SysWebCache : ICache
    {
        private static HashSet<string> CacheKeyList = new HashSet<string>();

        /// <summary>
        /// 获取数据缓存
        /// </summary>
        /// <param name="key">键</param>
        public T GetCache<T>(string key)
        {
            //var objCache = HttpRuntime.Cache;
            //return (T)objCache[key];

            var objString = HttpRuntime.Cache[key];
            if (typeof(T) == typeof(string))
            {
                return (T)objString;
            }

            //var result = SerializeHelper.FromBinary<T>(objString);
            var result = SerializeHelper.FromProtobufBinary<T>(objString as byte[]);
            return result;
        }
        public object GetCache(Type type, string key)
        {
            //var objCache = HttpRuntime.Cache;
            //var objString = objCache[key];
            //if (type == typeof(string))
            //{
            //    return objString.ToString();
            //}

            //return objString;

            var objString = HttpRuntime.Cache[key];
            if (type == typeof(string))
            {
                return objString.ToString();
            }

            //var result = SerializeHelper.FromBinary<T>(objString);
            var result = SerializeHelper.FromProtobufBinary(type, objString as byte[]);
            return result;
        }

        /// <summary>
        /// 设置数据缓存
        /// </summary>
        public void SetCache<T>(string key, T obj)
        {
            //if (CacheKeyList.Contains(key))
            //    throw new ArgumentException("System Web缓存中已存在重复的键值: " + key + "，请更改缓存键值再重试！", "key");

            //var objCache = HttpRuntime.Cache;
            //objCache.Insert(key, obj);
            //if (!CacheKeyList.Contains(key))
            //    CacheKeyList.Add(key);

            var objCache = HttpRuntime.Cache;
            if (typeof(T) == typeof(string))
            {
                objCache.Insert(key, obj);
                return;
            }

            var objString = SerializeHelper.ToProtobufBinary<T>(obj);
            objCache.Insert(key, objString);
            if (!CacheKeyList.Contains(key))
                CacheKeyList.Add(key);
        }
        /// <summary>
        /// 设置数据缓存
        /// </summary>
        public void SetCache<T>(string key, T obj, TimeSpan slidingExpiration)
        {
            //if (CacheKeyList.Contains(key))
            //    throw new ArgumentException("System Web缓存中已存在重复的键值: " + key + "，请更改缓存键值再重试！", "key");

            //var objCache = HttpRuntime.Cache;
            //objCache.Insert(key, obj, null, System.Web.Caching.Cache.NoAbsoluteExpiration, slidingExpiration, System.Web.Caching.CacheItemPriority.NotRemovable, new CacheItemRemovedCallback(ReportRemovedCallback));
            //if (!CacheKeyList.Contains(key))
            //    CacheKeyList.Add(key);

            var objCache = HttpRuntime.Cache;
            if (typeof(T) == typeof(string))
            {
                objCache.Insert(key, obj.ToString(), null, Cache.NoAbsoluteExpiration, slidingExpiration, CacheItemPriority.NotRemovable, ReportRemovedCallback);
                return;
            }

            var objString = SerializeHelper.ToProtobufBinary<T>(obj);
            objCache.Insert(key, objString, null, Cache.NoAbsoluteExpiration, slidingExpiration, CacheItemPriority.NotRemovable, ReportRemovedCallback);
            if (!CacheKeyList.Contains(key))
                CacheKeyList.Add(key);
        }
        /// <summary>
        /// 设置数据缓存
        /// </summary>
        public void SetCache<T>(string key, T obj, DateTime absoluteExpiration)
        {
            //if (CacheKeyList.Contains(key))
            //    throw new ArgumentException("System Web缓存中已存在重复的键值: " + key + "，请更改缓存键值再重试！", "key");

            //var objCache = HttpRuntime.Cache;
            //objCache.Insert(key, obj, null, absoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.NotRemovable, new CacheItemRemovedCallback(ReportRemovedCallback));
            //if (!CacheKeyList.Contains(key))
            //    CacheKeyList.Add(key);

            var objCache = HttpRuntime.Cache;
            if (typeof(T) == typeof(string))
            {
                objCache.Insert(key, obj.ToString(), null, absoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, ReportRemovedCallback);
                return;
            }

            var objString = SerializeHelper.ToProtobufBinary<T>(obj);
            objCache.Insert(key, objString, null, absoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, ReportRemovedCallback);
            if (!CacheKeyList.Contains(key))
                CacheKeyList.Add(key);
        }

        public static void ReportRemovedCallback(String key, object value, CacheItemRemovedReason removedReason)
        {
            if (CacheKeyList.Contains(key))
                CacheKeyList.Remove(key);
        }

        /// <summary>
        /// 移除指定数据缓存
        /// </summary>
        public void RemoveCache(string key)
        {
            if (CacheKeyList.Contains(key))
                CacheKeyList.Remove(key);

            var _cache = HttpRuntime.Cache;
            _cache.Remove(key);
        }
        /// <summary>
        /// 移除全部缓存
        /// </summary>
        public void RemoveAllCache()
        {
            var _cache = HttpRuntime.Cache;
            var CacheEnum = _cache.GetEnumerator();
            while (CacheEnum.MoveNext())
            {
                _cache.Remove(CacheEnum.Key.ToString());
            }

            CacheKeyList.Clear();
        }

        public int DatabaseId { get; set; }

        public List<string> GetCacheKeyList(string key)
        {
            return string.IsNullOrWhiteSpace(key) ? CacheKeyList.ToList() : CacheKeyList.Where(m => m.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        }

        public bool ExistKey(string key)
        {
            return CacheKeyList.Contains(key);
        }
    }
}