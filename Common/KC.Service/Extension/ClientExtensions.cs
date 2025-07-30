using KC.Framework.Base;
using KC.Framework.Util;
using KC.Service.Constants;
using KC.IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using System.Collections;

namespace KC.Service.Extension
{
    public static class ClientExtensions
    {
        private const string DefaultUserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0) AppleWebKit/534.10 (KHTML, like Gecko) Chrome/8.0.552.224 Safari/534.10";
        private const string DefaultAccept = "application/json";
        private const string DefaultAcceptEncoding = "utf-8";
        private const string DefaultAcceptLanguage = "en-US,en;q=0.8";

        private const string FormContentType = "application/x-www-form-urlencoded";

        public static HttpClient GetClient(IHttpClientFactory httpClientFactory, string baseAddress,
            Dictionary<string, string> headers = null)
        {
            HttpClient client = httpClientFactory != null
                ? httpClientFactory.CreateClient()
                : new HttpClient();
            // for fiddler proxy setting
            //var httpClientHandler = new HttpClientHandler
            //{
            //    // Does not work 
            //    Proxy = new WebProxy("http://Kent-pc:8888", false),
            //    UseProxy = true
            //};
            //client = new HttpClient(httpClientHandler);

            client.BaseAddress = new Uri(baseAddress);
            client.Timeout = TimeSpan.FromMinutes(TimeOutConstants.CacheShortTimeOut);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(DefaultAccept));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue(DefaultAcceptEncoding));
            //client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(DefaultUserAgent));
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
            return client;
        }

        private static string tokenEndpoint { get; set; }
        private static Dictionary<string, string> dictToken = new Dictionary<string, string>();

        /// <summary>
        /// 获取OAuth服务器地址
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <returns></returns>
        public static async Task<string> GetOAuthTokenServerUrlAsync(IHttpClientFactory httpClientFactory)
        {
            HttpClient client = httpClientFactory != null
                ? httpClientFactory.CreateClient()
                : new HttpClient();
            // for fiddler proxy setting
            //var httpClientHandler = new HttpClientHandler
            //{
            //    // Does not work 
            //    Proxy = new WebProxy("http://Kent-pc:8888", false),
            //    UseProxy = true
            //};
            //client = new HttpClient(httpClientHandler);

            if (string.IsNullOrEmpty(tokenEndpoint))
            {
                var disco = await client.GetDiscoveryDocumentAsync(GlobalConfig.SSOWebDomain);
                if (disco.IsError) throw new Exception(disco.Error);
                tokenEndpoint = disco.TokenEndpoint;
            }

            return tokenEndpoint;
        }
    }
}
