using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using KC.Common;
using KC.Framework.Util;
using KC.Framework.Tenant;
using KC.Service.Base;
using KC.Service.Constants;
using System.Collections.Concurrent;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Reflection;
using KC.Framework.Exceptions;
using System.Security.Cryptography;

namespace KC.Service.Util
{
    public class WeChatUtil
    {


        public static bool CheckSignature(string token, string signature, string timestamp, string nonce)
        {
            string strSignature = GetSignature(token, timestamp, nonce);

            return strSignature == signature;
        }
        public static string GetSignature(string token, string timestamp, string nonce)
        {
            string[] strs = new string[] { token, timestamp, nonce };

            Array.Sort(strs);

            //return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(String.Join("", strs), "SHA1").ToLower();
            return GetSwcSHA1(String.Join("", strs));
        }

        public static string GetSwcSHA1(string value)
        {
            //MD5 algorithm = MD5.Create();
            SHA1 algorithm = SHA1.Create();
            byte[] data = algorithm.ComputeHash(Encoding.UTF8.GetBytes(value));
            string sh1 = "";
            for (int i = 0; i < data.Length; i++)
            {
                sh1 += data[i].ToString("x2").ToUpperInvariant();
            }
            return sh1;
        }


        public static OAuthTokenResult GetWeiXinOpenId(WeixinConfig config, string code)
        {
            var appid = config.AppId;
            var secret = config.AppSecret;
            var url = OAuthConstants.WeixinServer + OAuthConstants.WeixinOAuth
                                .Replace("{appid}", appid)
                                .Replace("{secret}", secret)
                                .Replace("{code}", code);

            var key =  CacheKeyConstants.WeiXinCacheKey.OAuthToken(config.TenantName, appid, code);
            var result = CacheUtil.GetCache<OAuthTokenResult>(key);
            if (result != null && !result.IsTimeOut()) return result;

            try
            {
                string strToken = Get(url, Encoding.UTF8);
                LogUtil.LogDebug("----GetWeiXinOpenId return: " + strToken);

                var token = SerializeHelper.FromJson<OAuthTokenResult>(strToken);
                if (token == null || string.IsNullOrEmpty(token.AccessToken))
                {
                    return null;
                }

                CacheUtil.SetCache(key, token, TimeSpan.FromMinutes(TimeOutConstants.AccessTokenTimeOut));
                return token;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(string.Format("Action: WeiXinBLL.GetWeiXinOpenId,ErrorMessage:{0}", ex.Message));
                return null;
            }
        }

        #region HttpRequest
        public static string Get(string url, Encoding responseEncoding = null, int? timeout = 300, string userAgent = "", CookieCollection cookies = null
            , string Referer = "", Dictionary<string, string> headers = null, string contentType = "application/x-www-form-urlencoded")
        {
            HttpWebResponse response = CreateGetHttpResponse(url, timeout, userAgent, cookies, Referer, headers, contentType);
            try
            {
                if (responseEncoding == null)
                    responseEncoding = Encoding.UTF8;
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), responseEncoding))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string Post(string url, string parameters, Encoding requestEncoding = null, Encoding responseEncoding = null, int? timeout = 300, string userAgent = "", CookieCollection cookies = null, string Referer = "", Dictionary<string, string> headers = null, string contentType = "application/x-www-form-urlencoded")
        {
            HttpWebResponse response = CreatePostHttpResponse(url, parameters, requestEncoding, timeout, userAgent, cookies, Referer, headers, contentType);
            try
            {
                responseEncoding = responseEncoding ?? Encoding.UTF8;
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), responseEncoding))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogError("调用微信接口" + url + "，发生错误，错误信息:" + ex.Message);
                throw new BusinessPromptException("调用接口失败，请稍后重试");
            }
        }

        public static JObject PostToJson(string url, string parameters, Encoding requestEncoding = null, Encoding responseEncoding = null, int? timeout = 300, string userAgent = "", CookieCollection cookies = null, string Referer = "", Dictionary<string, string> headers = null, string contentType = "application/x-www-form-urlencoded")
        {
            var responseStr = Post(url, parameters, requestEncoding, requestEncoding, timeout, userAgent, cookies, Referer, headers, contentType);
            return JObject.Parse(responseStr);
        }
        public static JObject GetToJson(string url, Encoding responseEncoding = null, int? timeout = 300, string userAgent = "", CookieCollection cookies = null
    , string Referer = "", Dictionary<string, string> headers = null, string contentType = "application/x-www-form-urlencoded")
        {
            var responseStr = Get(url, responseEncoding, timeout, userAgent, cookies, Referer, headers, contentType);
            return JObject.Parse(responseStr);
        }

        /// <summary>  
        /// 创建POST方式的HTTP请求  
        /// </summary>  
        /// <param name="url">请求的URL</param>  
        /// <param name="parameters">随同请求POST的参数名称及参数值字典</param>  
        /// <param name="timeout">请求的超时时间</param>  
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>  
        /// <param name="requestEncoding">发送HTTP请求时所用的编码</param>  
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
        /// <returns></returns>  
        public static HttpWebResponse CreatePostHttpResponse(string url, string parameters, Encoding requestEncoding = null, int? timeout = 300, string userAgent = "", CookieCollection cookies = null, string Referer = "", Dictionary<string, string> headers = null, string contentType = "application/x-www-form-urlencoded")
        {
            if (Debug)
            {
                Console.Write("Start Post Url:{0} ,parameters:{1}  ", url, parameters);
            }

            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            HttpWebRequest request = null;
            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                //https://open.weixin.qq.com/cgi-bin/announce?action=getannouncement&key=1414994120&version=4&lang=zh_CN&token=
                ServicePointManager.SecurityProtocol =
                    SecurityProtocolType.Tls
                    | SecurityProtocolType.Tls11
                    | SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }

            if (Proxy != null)
            {
                request.Proxy = Proxy;
            }

            request.Method = "POST";
            request.Headers.Add("Accept-Language", "zh-CN,en-GB;q=0.5");
            request.Method = "POST";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.Referer = Referer;
            request.Headers["Accept-Language"] = "en-US,en;q=0.5";
            request.UserAgent = !string.IsNullOrEmpty(userAgent) ? userAgent : DefaultUserAgent;
            request.ContentType = contentType;
            request.Headers["Pragma"] = "no-cache";

            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value * 1000;
            }
            request.Expect = string.Empty;
            //如果需要POST数据  
            if (!string.IsNullOrEmpty(parameters))
            {
                requestEncoding = requestEncoding ?? Encoding.UTF8;
                byte[] data = requestEncoding.GetBytes(parameters);
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }

            var v = request.GetResponse() as HttpWebResponse;

            //Cookies.Add(request.CookieContainer.GetCookies(new Uri("http://" + new Uri(url).Host)));
            //Cookies.Add(request.CookieContainer.GetCookies(new Uri("https://" + new Uri(url).Host)));
            //Cookies.Add(v.Cookies);

            if (Debug)
            {
                Console.WriteLine("OK");
            }


            return v;
        }

        /// <summary>  
        /// 创建GET方式的HTTP请求  
        /// </summary>  
        /// <param name="url">请求的URL</param>  
        /// <param name="timeout">请求的超时时间</param>  
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>  
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
        /// <returns></returns>  
        public static HttpWebResponse CreateGetHttpResponse(string url, int? timeout = 300, string userAgent = "", CookieCollection cookies = null
            , string Referer = "", Dictionary<string, string> headers = null, string contentType = "application/x-www-form-urlencoded")
        {
            if (Debug)
            {
                Console.Write("Start Get Url:{0}    ", url);
            }

            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebRequest request;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                //https://open.weixin.qq.com/cgi-bin/announce?action=getannouncement&key=1414994120&version=4&lang=zh_CN&token=
                ServicePointManager.SecurityProtocol =
                    SecurityProtocolType.Tls
                    | SecurityProtocolType.Tls11
                    | SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }

            if (Proxy != null)
            {
                request.Proxy = Proxy;
            }
            request.Method = "GET";
            request.Headers["Pragma"] = "no-cache";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.Headers["Accept-Language"] = "en-US,en;q=0.5";
            request.ContentType = contentType;
            request.UserAgent = !string.IsNullOrEmpty(userAgent) ? userAgent : DefaultUserAgent;
            request.Referer = Referer;
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value * 1000;
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            //else
            //{
            //    request.CookieContainer = new CookieContainer();
            //    request.CookieContainer.Add(Cookies);
            //}
            var v = request.GetResponse() as HttpWebResponse;

            //Cookies.Add(request.CookieContainer.GetCookies(new Uri("http://" + new Uri(url).Host)));
            //Cookies.Add(request.CookieContainer.GetCookies(new Uri("https://" + new Uri(url).Host)));
            //Cookies.Add(v.Cookies);

            if (Debug)
            {
                Console.WriteLine("OK");
            }

            return v;
        }

        private static System.Net.WebProxy Proxy { get; set; }
        private static bool Debug { get; set; }

        private static readonly string DefaultUserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";


        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }

        #endregion

        public static void RemoveDefaultWeixinConfigCache()
        {
            var cacheKey = CacheKeyConstants.Prefix.ConfigName + "DefaultWeixinConfig";
            CacheUtil.RemoveCache(cacheKey);
        }


        private static ConcurrentDictionary<string, Lazy<object>> accessTokenLockDic = new ConcurrentDictionary<string, Lazy<object>>();
        private static ConcurrentDictionary<string, Lazy<object>> ticketLockDic = new ConcurrentDictionary<string, Lazy<object>>();
        public static WeixinAccessToken GetAccessToken(Tenant tenant, string appId, string appSecret)
        {
            if (string.IsNullOrWhiteSpace(appId) || string.IsNullOrWhiteSpace(appSecret))
                throw new BusinessPromptException("您未设置微信公众号的配置");
            WeixinAccessToken accessToken;

            var key = CacheKeyConstants.WeiXinCacheKey.AccessTokenKey(tenant.TenantName);
            accessToken = CacheUtil.GetCache<WeixinAccessToken>(key);
            if (accessToken == null || !accessToken.IsValid)
            {
                var lazy = accessTokenLockDic.GetOrAdd(tenant.TenantName.ToLower(), new Lazy<object>(() => new object(), LazyThreadSafetyMode.ExecutionAndPublication));
                lock (lazy.Value)
                {
                    accessToken = CacheUtil.GetCache<WeixinAccessToken>(key);//从缓存获取多一遍，判断上一个进入锁的人是否已更新
                    if (accessToken == null || !accessToken.IsValid)
                    {
                        var accessTokenUrl = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + appId + "&secret=" + appSecret;
                        var responseStr = Get(accessTokenUrl);
                        var result = SerializeHelper.FromJson<WeixinAccessToken>(responseStr);
                        if (result != null && !string.IsNullOrWhiteSpace(result.AccessToken))
                        {
                            CacheUtil.SetCache(key, result);
                            return result;
                        }
                        var jObject = JObject.Parse(responseStr);
                        var errcode = (int)jObject["errcode"];
                        if (errcode == -1)
                            throw new BusinessPromptException("微信公众平台繁忙，请稍后重试");
                        else if (errcode == 40001)
                            throw new BusinessPromptException("AppId或AppSecret有误");
                        else if (errcode == 40164)
                            throw new BusinessPromptException("IP不在白名单中");
                        throw new BusinessPromptException("错误代码为：" + errcode + "，错误信息为:" + jObject["errmsg"].ToString() + "，请联系管理员解决");
                    }
                    return accessToken;
                }
            }
            return accessToken;
        }

        public static bool CheckAppValid(string appId, string appSecret)
        {
            var accessTokenUrl = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + appId + "&secret=" + appSecret;
            var responseStr = Get(accessTokenUrl);
            var jObject = JObject.Parse(responseStr);

            var access_token = jObject.GetValue("access_token");
            if (access_token != null)
                return true;
            var errcode = (int)jObject["errcode"];
            if (errcode == -1)
                throw new BusinessPromptException("微信公众平台繁忙，请稍后重试");
            else if (errcode == 40001)
                throw new BusinessPromptException("AppId或AppSecret有误，请确保与微信公众平台保持一致");
            else if (errcode == 40013)
                throw new BusinessPromptException("AppId有误，请确保与微信公众平台保持一致");
            else if (errcode == 40125)
                throw new BusinessPromptException("AppSecret有误，请确保与微信公众平台保持一致");
            else if (errcode == 40164)
                throw new BusinessPromptException("IP不在白名单中，请前往微信公众平台设置");
            throw new BusinessPromptException("错误代码为：" + errcode + "，错误信息为:" + jObject["errmsg"].ToString() + "，请联系管理员解决");
        }

        public static WeixinJsTicket GetWeixinJsTicket(Tenant tenant, string appId, string appSecret)
        {
            var key = CacheKeyConstants.WeiXinCacheKey.JsTicketKey(tenant.TenantName);
            var ticket = CacheUtil.GetCache<WeixinJsTicket>(key);
            if (ticket == null || !ticket.IsValid)
            {
                var lazy = ticketLockDic.GetOrAdd(tenant.TenantName.ToLower(), new Lazy<object>(() => new object(), LazyThreadSafetyMode.ExecutionAndPublication));
                lock (lazy.Value)
                {
                    ticket = CacheUtil.GetCache<WeixinJsTicket>(key);//从缓存获取多一遍，判断上一个进入锁的人是否已更新
                    if (ticket == null || !ticket.IsValid)
                    {
                        var accessToken = GetAccessToken(tenant, appId, appSecret);
                        var ticketUrl = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + accessToken.AccessToken + "&type=jsapi";
                        var responseStr = Get(ticketUrl);
                        var result = SerializeHelper.FromJson<WeixinJsTicket>(responseStr);
                        if (result != null && !string.IsNullOrWhiteSpace(result.Ticket))
                        {
                            result.AppId = appId;
                            CacheUtil.SetCache(key, result);
                            return result;
                        }
                        throw new BusinessPromptException(responseStr);
                    }
                    return ticket;
                }
            }
            return ticket;
        }
    }
}
