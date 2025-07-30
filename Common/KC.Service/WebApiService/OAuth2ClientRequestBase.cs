using KC.Common;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Service.Constants;
using KC.Service.Extension;
using KC.IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Runtime.Serialization;

namespace KC.Service.WebApiService
{
    public abstract class OAuth2ClientRequestBase
    {
        protected const string DefaultContentType = "application/json";
        protected const string FormContentType = "application/x-www-form-urlencoded";

        protected readonly IHttpClientFactory httpClientFactory;
        protected readonly ILogger Logger;

        /// <summary>
        /// HttpClient访问外部接口的基类
        /// </summary>
        /// <param name="tenant">注入tenant对象</param>
        /// <param name="client">通过注入方式设置HttpClient：
        ///     例如：services.AddHttpClient<ITenantUserApiService, TenantUserApiService>()
        /// </param>
        protected OAuth2ClientRequestBase(
            IHttpClientFactory clientFactory,
            ILogger logger)
        {
            httpClientFactory = clientFactory;
            Logger = logger;
        }

        protected abstract OAuth2ClientInfo GetOAuth2ClientInfo();

        #region WebClient Get

        protected void WebSendGet<T>(string serviceName, string url, string scope, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            WebSendGet(serviceName, url, scope, DefaultContentType, callback, failCallback, needOAuthAuthenticated);
        }
        protected void WebSendGet<T>(string serviceName, string url, string scope, string contentType, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            WebSendGet(serviceName, url, scope, contentType, null, callback, failCallback, needOAuthAuthenticated);
        }
        protected void WebSendGet<T>(string serviceName, string url, string scope, string contentType, Dictionary<string, string> headers, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            if (string.IsNullOrEmpty(url))
            {
                failCallback?.Invoke(HttpStatusCode.BadRequest,
                    string.Format("Service's ({0}) url is null or empty. ",
                        serviceName));
                return;
            }
            if (needOAuthAuthenticated && string.IsNullOrEmpty(scope))
            {
                failCallback?.Invoke(HttpStatusCode.BadRequest,
                    string.Format("Service's ({0}) scope is null or empty. ",
                        serviceName));
                return;
            }

            try
            {
                Logger?.LogDebug(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|开始调用服务{0}，参数如下：URL={1}", serviceName, url));

                var uri = new Uri(url);
                var baseUri = uri.Scheme + "://" + uri.Authority;
                var apiUri = uri.PathAndQuery;

                var client = GetClient(baseUri, scope, headers, needOAuthAuthenticated).Result;
                var response = client.GetAsync(apiUri).Result;
                if (!response.IsSuccessStatusCode)
                {
                    var errorString = response.Content.ReadAsStringAsync().Result;
                    failCallback?.Invoke(HttpStatusCode.BadRequest, errorString);
                    return;
                }

                //内容
                var responseString = response.Content.ReadAsStringAsync().Result;
                if (callback != null && !string.IsNullOrWhiteSpace(responseString))
                {
                    if (typeof(T) == typeof(string))
                    {
                        var result = (T)Convert.ChangeType(responseString, typeof(T));
                        callback(result);
                    }
                    else if (typeof(T) == typeof(bool))
                    {
                        var result = SerializeHelper.FromJson<T>(responseString);
                        callback(result);
                    }
                    else
                    {
                        var result = SerializeHelper.FromJson<T>(responseString);
                        callback(result);
                    }
                }
                
                Logger?.LogDebug(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|结束调用服务{0}，参数如下：URL={1}", serviceName, url));

            }
            catch (Exception ex)
            {
                string error =
                    string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|调用服务{0}出现异常，参数如下：URL={1}，错误消息{2}，详细信息{3}",
                        serviceName,
                        url,
                        ex.Message,
                        ex.StackTrace);
                Logger?.LogError(error);
                failCallback?.Invoke(HttpStatusCode.BadRequest, error);
            }
        }

        protected async Task WebSendGetAsync<T>(string serviceName, string url, string scope, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            await WebSendGetAsync(serviceName, url, scope, DefaultContentType, callback, failCallback, needOAuthAuthenticated);
        }
        protected async Task WebSendGetAsync<T>(string serviceName, string url, string scope, string contentType, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            await WebSendGetAsync(serviceName, url, scope, contentType, null, callback, failCallback, needOAuthAuthenticated);
        }
        protected async Task WebSendGetAsync<T>(string serviceName, string url, string scope, string contentType, Dictionary<string, string> headers, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            if (string.IsNullOrEmpty(url))
            {
                failCallback?.Invoke(HttpStatusCode.BadRequest,
                    string.Format("Service's ({0}) url is null or empty. ",
                        serviceName));
                return;
            }
            if (needOAuthAuthenticated && string.IsNullOrEmpty(scope))
            {
                failCallback?.Invoke(HttpStatusCode.BadRequest,
                    string.Format("Service's ({0}) scope is null or empty. ",
                        serviceName));
                return;
            }

            try
            {
                Logger?.LogDebug(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|开始调用服务{0}，参数如下：URL={1}", serviceName, url));

                var uri = new Uri(url);
                var baseUri = uri.Scheme + "://" + uri.Authority;
                var apiUri = uri.PathAndQuery;
                var client = await GetClient(baseUri, scope, headers, needOAuthAuthenticated);
                var response = await client.GetAsync(apiUri);
                if (!response.IsSuccessStatusCode)
                {
                    var errorString = await response.Content.ReadAsStringAsync();
                    failCallback?.Invoke(HttpStatusCode.BadRequest, errorString);
                    return;
                }
                //内容
                var responseString = await response.Content.ReadAsStringAsync();
                if (callback != null && !string.IsNullOrWhiteSpace(responseString))
                {
                    if (typeof(T) == typeof(string))
                    {
                        var result = (T)Convert.ChangeType(responseString, typeof(T));
                        callback(result);
                    }
                    else if (typeof(T) == typeof(bool))
                    {
                        var result = SerializeHelper.FromJson<T>(responseString);
                        callback(result);
                    }
                    else
                    {
                        var result = SerializeHelper.FromJson<T>(responseString);
                        callback(result);
                    }
                }

                Logger?.LogDebug(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|结束调用服务{0}，参数如下：URL={1}", serviceName, url));
            }
            catch (Exception ex)
            {
                string error =
                    string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|调用服务{0}出现异常，参数如下：URL={1}，错误消息{2}，详细信息{3}",
                        serviceName,
                        url,
                        ex.Message,
                        ex.StackTrace);
                Logger?.LogError(error);
                failCallback?.Invoke(HttpStatusCode.BadRequest, error);
            }
        }

        #endregion

        #region WebClient Delete

        protected async Task WebSendDeleteAsync<T>(string serviceName, string url, string scope, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            await WebSendDeleteAsync(serviceName, url, scope, DefaultContentType, callback, failCallback, needOAuthAuthenticated);
        }
        protected async Task WebSendDeleteAsync<T>(string serviceName, string url, string scope, string contentType, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            await WebSendDeleteAsync(serviceName, url, scope, contentType, null, callback, failCallback, needOAuthAuthenticated);
        }
        protected async Task WebSendDeleteAsync<T>(string serviceName, string url, string scope, string contentType, Dictionary<string, string> headers, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            //Logger?.LogDebug(string.Format("开始调用服务{0}，参数如下：URL={1}", serviceName, url));

            if (string.IsNullOrEmpty(url))
            {
                failCallback?.Invoke(HttpStatusCode.BadRequest,
                    string.Format("Service's ({0}) url is null or empty. ",
                        serviceName));
                return;
            }
            if (needOAuthAuthenticated && string.IsNullOrEmpty(scope))
            {
                failCallback?.Invoke(HttpStatusCode.BadRequest,
                    string.Format("Service's ({0}) scope is null or empty. ",
                        serviceName));
                return;
            }

            try
            {
                var uri = new Uri(url);
                var baseUri = uri.Scheme + "://" + uri.Authority;
                var apiUri = uri.PathAndQuery;
                var client = await GetClient(baseUri, scope, headers, needOAuthAuthenticated);
                var response = await client.DeleteAsync(apiUri);
                if (!response.IsSuccessStatusCode)
                {
                    var errorString = await response.Content.ReadAsStringAsync();
                    failCallback?.Invoke(HttpStatusCode.BadRequest, errorString);
                    return;
                }
                //内容
                var responseString = await response.Content.ReadAsStringAsync();
                if (callback != null && !string.IsNullOrWhiteSpace(responseString))
                {
                    if (typeof(T) == typeof(string))
                    {
                        var result = (T)Convert.ChangeType(responseString, typeof(T));
                        callback(result);
                    }
                    else if (typeof(T) == typeof(bool))
                    {
                        var result = SerializeHelper.FromJson<T>(responseString);
                        callback(result);
                    }
                    else
                    {
                        var result = SerializeHelper.FromJson<T>(responseString);
                        callback(result);
                    }
                }
            }
            catch (Exception ex)
            {
                string error =
                    string.Format("调用服务{0}出现异常，参数如下：URL={1}，错误消息{2}，详细信息{3}",
                        serviceName,
                        url,
                        ex.Message,
                        ex.StackTrace);
                Logger?.LogError(error);
                failCallback?.Invoke(HttpStatusCode.BadRequest, error);
            }
        }
        #endregion

        #region WebClient Post
        protected void WebSendPost<T>(string serviceName, string url, string scope, string postJsonData, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            WebSendPost(serviceName, url, scope, postJsonData, null, callback, failCallback, needOAuthAuthenticated);
        }
        protected void WebSendPost<T>(string serviceName, string url, string scope, string postJsonData, Dictionary<string, string> headers, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            WebSendPost(serviceName, url, scope, postJsonData, DefaultContentType, headers, callback,
                    failCallback, needOAuthAuthenticated);
        }
        protected void WebSendPost<T>(string serviceName, string url, string scope, string postJsonData, string contentType, Dictionary<string, string> headers, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            if (string.IsNullOrEmpty(url))
            {
                failCallback?.Invoke(HttpStatusCode.BadRequest,
                    string.Format("Service's ({0}) url is null or empty in {1}. ",
                        serviceName, Framework.Base.GlobalConfig.CurrentApplication?.AppName));
                return;
            }
            if (needOAuthAuthenticated && string.IsNullOrEmpty(scope))
            {
                failCallback?.Invoke(HttpStatusCode.BadRequest,
                    string.Format("Service's ({0}) scope is null or empty in {1}. ",
                        serviceName, Framework.Base.GlobalConfig.CurrentApplication?.AppName));
                return;
            }

            try
            {
                Logger?.LogDebug(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|开始调用服务{0}，参数如下：URL={1}、postJsonData={2}", serviceName, url, postJsonData));

                var uri = new Uri(url);
                var baseUri = uri.Scheme + "://" + uri.Authority;
                var apiUri = uri.PathAndQuery;

                var client = GetClient(baseUri, scope, headers, needOAuthAuthenticated).Result;

                var request = new HttpRequestMessage(HttpMethod.Post, apiUri);
                request.Content = new StringContent(postJsonData, Encoding.UTF8, contentType);
                var response = client.SendAsync(request).Result;
                if (!response.IsSuccessStatusCode)
                {
                    var errorString = response.Content.ReadAsStringAsync().Result;
                    failCallback?.Invoke(HttpStatusCode.BadRequest, errorString);
                    return;
                }
                //内容
                var responseString = response.Content.ReadAsStringAsync().Result;
                if (callback != null && !string.IsNullOrWhiteSpace(responseString))
                {
                    if (typeof(T) == typeof(bool))
                    {
                        var result = SerializeHelper.FromJson<T>(responseString);
                        callback(result);
                    }
                    else if (typeof(T) == typeof(string))
                    {
                        var result = (T)(Object)responseString;
                        callback(result);
                    }
                    else
                    {
                        var result = SerializeHelper.FromJson<T>(responseString);
                        callback(result);
                    }
                }

                Logger?.LogDebug(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|结束调用服务{0}，参数如下：URL={1}、postJsonData={2}", serviceName, url, postJsonData));
            }
            catch (Exception ex)
            {
                string error =
                    string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|调用服务{0}出现异常，参数如下：URL={1}，错误消息{2}，详细信息{3}",
                        serviceName,
                        url,
                        ex.Message,
                        ex.StackTrace);
                Logger?.LogError(error);
                failCallback?.Invoke(HttpStatusCode.BadRequest, error);
            }
        }

        protected async Task WebSendPostAsync<T>(string serviceName, string url, string scope, string postJsonData, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            await
                WebSendPostAsync(serviceName, url, scope, postJsonData, null, callback,
                    failCallback, needOAuthAuthenticated);
        }
        protected async Task WebSendPostAsync<T>(string serviceName, string url, string scope, string postJsonData, Dictionary<string, string> headers, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            await
                WebSendPostAsync(serviceName, url, scope, postJsonData, DefaultContentType, headers, callback,
                    failCallback, needOAuthAuthenticated);
        }
        protected async Task WebSendPostAsync<T>(string serviceName, string url, string scope, string postJsonData, string contentType, Dictionary<string, string> headers, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            if (string.IsNullOrEmpty(url))
            {
                var errorString = string.Format("Service's ({0}) url is null or empty in {1}. ", serviceName, Framework.Base.GlobalConfig.CurrentApplication?.AppName);
                Logger?.LogError(errorString);
                failCallback?.Invoke(HttpStatusCode.BadRequest, errorString);
                return;
            }
            if (needOAuthAuthenticated && string.IsNullOrEmpty(scope))
            {
                var errorString = string.Format("Service's ({0}) scope is null or empty in {1}. ", serviceName,  Framework.Base.GlobalConfig.CurrentApplication?.AppName);
                Logger?.LogError(errorString);
                failCallback?.Invoke(HttpStatusCode.BadRequest, errorString);
                return;
            }

            try
            {
                Logger?.LogDebug(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|开始调用服务{0}，参数如下：URL={1}、postJsonData={2}", serviceName, url, postJsonData));

                var uri = new Uri(url);
                var baseUri = uri.Scheme + "://" + uri.Authority;
                var apiUri = uri.PathAndQuery;

                var client = await GetClient(baseUri, scope, headers, needOAuthAuthenticated);

                var request = new HttpRequestMessage(HttpMethod.Post, apiUri);
                var content = new StringContent(postJsonData, Encoding.UTF8, contentType);
                var response = await client.PostAsync(apiUri, content);
                if (!response.IsSuccessStatusCode)
                {
                    var errorString = await response.Content.ReadAsStringAsync();
                    Logger?.LogError(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|开始调用服务{0}，参数如下：URL={1}、出错状态={2}，出错：{3}", serviceName, url, response.StatusCode, errorString));
                    failCallback?.Invoke(HttpStatusCode.BadRequest, errorString);
                    return;
                }

                //response.EnsureSuccessStatusCode();
                //内容
                var responseString = await response.Content.ReadAsStringAsync();
                if (callback != null && !string.IsNullOrWhiteSpace(responseString))
                {
                    if (typeof(T) == typeof(bool))
                    {
                        var result = SerializeHelper.FromJson<T>(responseString);
                        callback(result);
                    }
                    else if (typeof(T) == typeof(string))
                    {
                        var result = (T)(Object)responseString;
                        callback(result);
                    }
                    else
                    {
                        var result = SerializeHelper.FromJson<T>(responseString);
                        callback(result);
                    }
                }

                Logger?.LogDebug(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|结束调用服务{0}，参数如下：URL={1}、postJsonData={2}", serviceName, url, postJsonData));
            }
            catch (Exception ex)
            {
                string error =
                    string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|调用服务{0}出现异常，参数如下：URL={1}，错误消息{2}，详细信息{3}",
                        serviceName,
                        url,
                        ex.Message,
                        ex.StackTrace);
                Logger?.LogError(error);
                failCallback?.Invoke(HttpStatusCode.BadRequest, error);
            }
        }

        #endregion

        #region WebClient Put

        protected async Task WebSendPutAsync<T>(string serviceName, string url, string scope, string postJsonData, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            await
                WebSendPutAsync(serviceName, url, scope, postJsonData, null, callback,
                    failCallback, needOAuthAuthenticated);
        }
        protected async Task WebSendPutAsync<T>(string serviceName, string url, string scope, string postJsonData, Dictionary<string, string> headers, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            await
                WebSendPutAsync(serviceName, url, scope, postJsonData, DefaultContentType, headers, callback,
                    failCallback, needOAuthAuthenticated);
        }
        protected async Task WebSendPutAsync<T>(string serviceName, string url, string scope, string postJsonData, string contentType, Dictionary<string, string> headers, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            if (string.IsNullOrEmpty(url))
            {
                failCallback?.Invoke(HttpStatusCode.BadRequest,
                    string.Format("Service's ({0}) url is null or empty in {1}. ",
                        serviceName, Framework.Base.GlobalConfig.CurrentApplication?.AppName));
                return;
            }
            if (needOAuthAuthenticated && string.IsNullOrEmpty(scope))
            {
                failCallback?.Invoke(HttpStatusCode.BadRequest,
                    string.Format("Service's ({0}) scope is null or empty in {1}. ",
                        serviceName, Framework.Base.GlobalConfig.CurrentApplication?.AppName));
                return;
            }

            try
            {
                Logger?.LogDebug(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|开始调用服务{0}，参数如下：URL={1}、postJsonData={2}", serviceName, url, postJsonData));

                var uri = new Uri(url);
                var baseUri = uri.Scheme + "://" + uri.Authority;
                var apiUri = uri.PathAndQuery;

                var client = await GetClient(baseUri, scope, headers, needOAuthAuthenticated);

                var request = new HttpRequestMessage(HttpMethod.Post, apiUri);
                var content = new StringContent(postJsonData, Encoding.UTF8, contentType);
                var response = await client.PutAsync(apiUri, content);
                if (!response.IsSuccessStatusCode)
                {
                    var errorString = await response.Content.ReadAsStringAsync();
                    failCallback?.Invoke(HttpStatusCode.BadRequest, errorString);
                    return;
                }

                //response.EnsureSuccessStatusCode();
                //内容
                var responseString = await response.Content.ReadAsStringAsync();
                if (callback != null && !string.IsNullOrWhiteSpace(responseString))
                {
                    if (typeof(T) == typeof(bool))
                    {
                        var result = SerializeHelper.FromJson<T>(responseString);
                        callback(result);
                    }
                    else if (typeof(T) == typeof(string))
                    {
                        var result = (T)(Object)responseString;
                        callback(result);
                    }
                    else
                    {
                        var result = SerializeHelper.FromJson<T>(responseString);
                        callback(result);
                    }
                }

                Logger?.LogDebug(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|结束调用服务{0}，参数如下：URL={1}、postJsonData={2}", serviceName, url, postJsonData));
            }
            catch (Exception ex)
            {
                string error =
                    string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|调用服务{0}出现异常，参数如下：URL={1}，错误消息{2}，详细信息{3}",
                        serviceName,
                        url,
                        ex.Message,
                        ex.StackTrace);
                Logger?.LogError(error);
                failCallback?.Invoke(HttpStatusCode.BadRequest, error);
            }
        }

        #endregion

        #region Download File

        public async Task DownLoadFile(string serviceName, string url, Action<byte[]> callback, Action<string> failCallback)
        {
            try
            {
                Logger?.LogDebug(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|开始调用服务{0}，参数如下：URL={1}", serviceName, url));

                var uri = new Uri(url);
                var client = new WebClient();
                var data = await client.DownloadDataTaskAsync(uri);
                if (callback != null)
                    callback(data);

                Logger?.LogDebug(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|结束调用服务{0}，参数如下：URL={1}", serviceName, url));
            }
            catch (Exception ex)
            {
                var msg = string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|调用服务{0}出错，参数如下：URL={1}，错误消息：{2}，详细消息：{3}。",
                    serviceName, url, ex.Message, ex.StackTrace);
                Logger?.LogError(msg);
                if (failCallback != null)
                    failCallback(msg);
            }

            //client.DownloadDataCompleted += (s, e) =>
            //{
            //    if (e.Error != null)
            //    {
            //        var msg = string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|开始调用服务{0}出错，参数如下：URL={1}，错误消息：{2}，详细信息：{3}。",
            //            serviceName, url, e.Error.Message, e.Error.StackTrace);
            //        Logger?.LogError(msg);
            //        if (failCallback != null)
            //            failCallback(msg);
            //    }
            //    else
            //    {
            //        Logger?.LogDebug(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|结束调用服务{0}，参数如下：URL={1}", serviceName, url));
            //        if (callback != null)
            //            callback(e.Result);
            //    }
            //};
            //client.DownloadDataAsync(uri);
        }

        public async Task<byte[]> GetDownloadFileBytes(string serviceName, string url)
        {
            try
            {
                Logger?.LogDebug(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|开始调用服务{0}，参数如下：URL={1}", serviceName, url));

                var uri = new Uri(url);
                var client = new WebClient();
                var data = await client.DownloadDataTaskAsync(uri);

                Logger?.LogDebug(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|结束调用服务{0}，参数如下：URL={1}", serviceName, url));
                return data;
            }
            catch (Exception ex)
            {
                var msg = string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|调用服务{0}出错，参数如下：URL={1}，错误消息：{2}，详细消息：{3}。",
                    serviceName, url, ex.Message, ex.StackTrace);
                Logger?.LogError(msg);
                return null;
            }
        }
        #endregion

        private async Task<HttpClient> GetClient(
            string url, string scope, Dictionary<string, string> headers, bool needOAuthAuthenticated)
        {
            if (needOAuthAuthenticated)
            {
                var accessToken = await GetAccessTokenAsync(scope);

                var client = ClientExtensions.GetClient(httpClientFactory, url, headers);
                client.SetBearerToken(accessToken);

                return client;
            }

            return ClientExtensions.GetClient(httpClientFactory, url, headers);
        }

        /// <summary>
        /// 获取accesstoken
        /// </summary>
        /// <param name="clientScope"></param>
        /// <returns></returns>
        protected virtual async Task<string> GetAccessTokenAsync(string clientScope)
        {
            var clientInfo = GetOAuth2ClientInfo();
            var tenantName = clientInfo.TenantName;
            var tokenEndpoint = clientInfo.TokenEndpoint.Replace("http://", "https://");
            var clientId = clientInfo.ClientId;
            var clientSecret = clientInfo.ClientSecret;

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

            var accessTokenCacheKey = CacheKeyConstants.Prefix.TenantAccessToken + clientId;
            var accessToken = Service.CacheUtil.GetCache<string>(accessTokenCacheKey);
            //if (string.IsNullOrEmpty(accessToken))
            {
                Logger?.LogDebug(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|开始调用服务【{0}】获取AccessToken，参数如下：tenant={1}、url={2}、clientId={3}、clientId={4}、Scope={5}", "RequestClientCredentialsTokenAsync", tenantName, tokenEndpoint, clientId, clientSecret, clientScope));
                var response = await client.RequestClientCredentialsTokenAsync(
                    new ClientCredentialsTokenRequest
                    {
                        Address = tokenEndpoint,
                        ClientId = clientId,
                        ClientSecret = clientSecret,
                        Scope = clientScope,
                        Parameters = !string.IsNullOrEmpty(tenantName)
                            ? new Dictionary<string, string>() { { TenantConstant.ClaimTypes_TenantName, tenantName } }
                            : null
                    });
                if (response.IsError) {
                    var errMsg = String.Format("调用服务【{0}】，参数如下：tenant={1}、url={2}、clientId={3}、clientId={4}、Scope={5}，获取AccessToken出错：{5}",
                      "GetAccessTokenAsync", tenantName, tokenEndpoint, clientId, clientSecret, clientScope, response.Error + ": " + response.ErrorDescription);
                    Logger?.LogError(errMsg);
                    throw new Exception(errMsg);
                }

                accessToken = response.AccessToken;
                var expiredTimeSpan = response.ExpiresIn > 100 ? response.ExpiresIn - 100 : TimeOutConstants.AccessTokenTimeOut * 60;
                Service.CacheUtil.SetCache(accessTokenCacheKey, accessToken, TimeSpan.FromSeconds(expiredTimeSpan));

                Logger?.LogDebug(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|KC.Service.Extension.ClientExtensions accessToken: " + accessToken);
            }

            return accessToken;
        }

        /// <summary>
        /// 使用RefreshToken，获取更新后的AccessToken
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <param name="clientScope"></param>
        /// <returns></returns>
        protected virtual async Task<string> GetRefreshToken(string refreshToken, string clientScope)
        {
            var clientInfo = GetOAuth2ClientInfo();
            var tenantName = clientInfo.TenantName;
            var tokenEndpoint = clientInfo.TokenEndpoint;
            var clientId = clientInfo.ClientId;
            var clientSecret = clientInfo.ClientSecret;

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

            var response = await client.RequestRefreshTokenAsync(
                    new RefreshTokenRequest
                    {
                        Address = tokenEndpoint,
                        ClientId = clientId,
                        ClientSecret = clientSecret,
                        RefreshToken = refreshToken,
                        Scope = clientScope,
                        Parameters = !string.IsNullOrEmpty(tenantName)
                            ? new Dictionary<string, string>() { { TenantConstant.ClaimTypes_TenantName, tenantName } }
                            : null
                    });
            if (response.IsError) {
                var errMsg = String.Format("GetRefreshToken tenantName：{0}，tokenEndpoint：{1}，clientId：{2}，clientId：{3}，Scope{4}，获取AccessToken出错：{5}", 
                        tenantName, tokenEndpoint, clientId, clientSecret, clientScope, response.Error + ": " + response.ErrorDescription);
                    Logger?.LogError(errMsg);
                    throw new Exception(errMsg);
            }

            var accessToken = response.AccessToken;
            var expiredTimeSpan = response.ExpiresIn - 100;
            var accessTokenCacheKey = CacheKeyConstants.Prefix.TenantAccessToken + clientId;
            Service.CacheUtil.SetCache(accessTokenCacheKey, accessToken, TimeSpan.FromSeconds(expiredTimeSpan));

            return accessToken;
        }
    }

    [Serializable]
    [DataContract(IsReference = true)]
    public class OAuth2ClientInfo
    {
        public OAuth2ClientInfo(string tenantName, string clientId, string clientSecret, string tokenEndpoint)
            : this(tenantName, clientId, clientSecret, tokenEndpoint, "client_credentials")
        {
            this.TenantName = tenantName;
            this.ClientId = clientId;
            this.ClientSecret = clientSecret;

            this.TokenEndpoint = tokenEndpoint;
        }
        public OAuth2ClientInfo(string tenantName, string clientId, string clientSecret, string tokenEndpoint, string grantType)
        {
            this.TenantName = tenantName;
            this.ClientId = clientId;
            this.ClientSecret = clientSecret;

            this.TokenEndpoint = tokenEndpoint;
            this.GrantType = grantType;
        }
        [DataMember]
        public string TenantName { get; set; }
        [DataMember]
        public string ClientId { get; set; }
        [DataMember]
        public string ClientSecret { get; set; }
        [DataMember]
        public string ClientCredential { get; set; }
        [DataMember]
        public string TokenEndpoint { get; set; }
        [DataMember]
        public string GrantType { get; set; }
    }
}
