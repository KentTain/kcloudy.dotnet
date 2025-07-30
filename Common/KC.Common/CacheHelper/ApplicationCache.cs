using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Com.Framework.Util;
using Com.Framework.Util.Cache;

namespace Com.Common.CacheHelper
{
    /// <summary>
    /// 全局应用缓存: HttpContext.Current.Application
    /// </summary>
    public class ApplicationCache : ICache
    {
        #region ICache 成员

        public T GetCache<T>(string key)
        {
            return (T)HttpContext.Current.Application[key];
        }
        public object GetCache(Type type, string key)
        {
            var objString = HttpContext.Current.Application[key];
            if (type == typeof(string))
            {
                return objString.ToString();
            }

            return objString;
        }

        public void SetCache<T>(string key, T obj)
        {
            HttpContext.Current.Application.Add(key, obj);
        }

        public void SetCache<T>(string key, T obj, TimeSpan slidingExpiration)
        {
            throw new NotImplementedException();
        }

        public void SetCache<T>(string key, T obj, DateTime absoluteExpiration)
        {
            throw new NotImplementedException();
        }

        public void RemoveCache(string key)
        {
            HttpContext.Current.Application.Remove(key);
        }

        public void RemoveAllCache()
        {
            HttpContext.Current.Application.Clear();
        }
        #endregion

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
