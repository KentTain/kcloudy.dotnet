namespace KC.Service.Constants
{
    public sealed class CacheKeyConstants
    {
        private const string ComPrx = "com-";

        public class Prefix
        {
            public const string CurrentUserId = ComPrx + "currentUserId-";

            public const string TenantAuthTokenEndpoint = ComPrx + "OAuth-TokenEndpoint-";
            public const string TenantAccessToken = ComPrx + "OAuth-AccessToken-";
            public const string TenantRefreashToken = ComPrx + "OAuth-RefreashToken-";

            public const string TenantName = ComPrx + "tenant-";

            public const string ConfigName = ComPrx + "config-";
        }

        public class WeiXinCacheKey
        {
            public static string OAuthToken(string tenantName, string appid, string code)
            {
                return ComPrx + string.Format("Wx-OAuthToken-appid[{0}]-{1}-code-{2}", tenantName, appid, code);
            }
            public static string AccessTokenKey(string tenantName)
            {
                return ComPrx + "WeiXinAccessToken-" + tenantName.ToLower();
            }
            public static string JsTicketKey(string tenantName)
            {
                return ComPrx + "WeixinJsTicket-" + tenantName.ToLower();
            }
            public static string ConfigKey(string tenantName)
            {
                return ComPrx + "WeixinConfig-" + tenantName.ToLower();
            }
        }
    }

    public static class TimeOutConstants
    {
        /// <summary>
        /// Cookie：20分钟过期
        /// </summary>
        public const int CookieTimeOut = 20; //Cookie：20分钟过期
        /// <summary>
        /// Cache：12小时过期
        /// </summary>
        public const int CacheTimeOut = 12 * 60; //Cache：12小时过期
        /// <summary>
        /// Cache：5分钟过期
        /// </summary>
        public const int CacheShortTimeOut = 5; //Cache：5分钟过期
        /// <summary>
        /// AccessToken：60分钟过期
        /// </summary>
        public const int AccessTokenTimeOut = 60; //AccessToken：60分钟过期
        /// <summary>
        /// 短信验证码有效期：5分钟
        /// </summary>
        public const int PhoneCodeTimeout = 5;//短信验证码有效期
        /// <summary>
        /// 默认缓存有效期：30天
        /// </summary>
        public const int DefaultCacheTimeOut = 30 * 24 * 60; //Cache：30天
        /// <summary>
        /// 共用缓存有效期：15分钟
        /// </summary>
        public const int SharedCacheTimeOut = 15; //Cache：15分钟

        /// <summary>
        /// 微信Toke过期时间：120分钟过期
        /// </summary>
        public const int WeixTokenTimeOut = 120;
    }
}
