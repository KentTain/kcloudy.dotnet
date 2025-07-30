using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace KC.Service.WebApiService
{
    public interface IOAuth2ClientCommonService
    {
        void SetTenantOAuth2ClientInfo();
        void SetOAuth2ClientInfo(OAuth2ClientInfo oAuth2Client);

        #region WebClient Get

        void WebSendGet<T>(string url, string scope, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false);
        void WebSendGet<T>(string url, string scope, string contentType, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false);
        void WebSendGet<T>(string url, string scope, string contentType, Dictionary<string, string> headers, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false);

        Task WebSendGetAsync<T>(string url, string scope, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false);
        Task WebSendGetAsync<T>(string url, string scope, string contentType, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false);
        Task WebSendGetAsync<T>(string url, string scope, string contentType, Dictionary<string, string> headers, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false);

        #endregion

        #region WebClient Delete

        Task WebSendDeleteAsync<T>(string url, string scope, Action<T> callback,
            Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false);
        Task WebSendDeleteAsync<T>(string url, string scope, string contentType, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false);
        Task WebSendDeleteAsync<T>(string url, string scope, string contentType, Dictionary<string, string> headers, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false);
        #endregion

        #region WebClient Post
        void WebSendPost<T>(string url, string scope, string postJsonData, Action<T> callback,
            Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false);
        void WebSendPost<T>(string url, string scope, string postJsonData, Dictionary<string, string> headers, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false);
        void WebSendPost<T>(string url, string scope, string postJsonData, string contentType, Dictionary<string, string> headers, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false);

        Task WebSendPostAsync<T>(string url, string scope, string postJsonData, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false);
        Task WebSendPostAsync<T>(string url, string scope, string postJsonData, Dictionary<string, string> headers, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false);
        Task WebSendPostAsync<T>(string url, string scope, string postJsonData, string contentType, Dictionary<string, string> headers, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false);

        #endregion

        #region WebClient Put

        Task WebSendPutAsync<T>(string url, string scope, string postJsonData, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false);
        Task WebSendPutAsync<T>(string url, string scope, string postJsonData, Dictionary<string, string> headers, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false);
        Task WebSendPutAsync<T>(string url, string scope, string postJsonData, string contentType, Dictionary<string, string> headers, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false);

        #endregion

        #region Download File

        Task DownLoadFile(string url, Action<byte[]> callback, Action<string> failCallback);

        Task<byte[]> GetDownloadFileBytes(string url);
        #endregion
    }
}