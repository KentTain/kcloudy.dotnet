using KC.Framework.Base;
using KC.Framework.Tenant;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.WebApiService
{
    public class OAuth2ClientCommonService : OAuth2ClientRequestBase, IOAuth2ClientCommonService
    {
        public const string OAuth2_Authorize_Action = "connect/authorize";
        public const string OAuth2_Token_Action = "connect/token";
        public const string OAuth2_UserInfo_Action = "connect/userinfo";
        public const string OAuth2_JwkSet_Action = ".well-known/openid-configuration/jwks";
        private const string serviceName = "KC.Service.WebApiService.OAuth2ClientCommonService";

        private Tenant _tenant; 
        private OAuth2ClientInfo _oAuth2Client;
        public OAuth2ClientCommonService(
            Tenant tenant,
            IHttpClientFactory clientFactory,
            ILogger<OAuth2ClientCommonService> logger)
            : base(clientFactory, logger)
        {
            _tenant = tenant;
        }

        public void SetOAuth2ClientInfo(OAuth2ClientInfo oAuth2Client)
        {
            _oAuth2Client = oAuth2Client;
        }

        public void SetTenantOAuth2ClientInfo()
        {
            if (_tenant != null)
            {
                var tenantName = _tenant.TenantName;
                var clientId = TenantConstant.GetClientIdByTenant(_tenant);
                var clientSecret = TenantConstant.GetClientSecretByTenant(_tenant);
                var credential = TenantConstant.Sha256(clientSecret);

                var tokenEndpoint = GlobalConfig.SSOWebDomain + OAuth2_Token_Action;
                //string tokenEndpoint = SSOServerUrl() + OpenIdConnectConstants.OAuth2_Token_Action;
                var grantType = "client_credentials";

                _oAuth2Client = new OAuth2ClientInfo(tenantName, clientId, clientSecret, tokenEndpoint, grantType);
            }
        }

        protected override OAuth2ClientInfo GetOAuth2ClientInfo()
        {
            if (_oAuth2Client == null)
                throw new ArgumentNullException("_oAuth2Client", "未设置OAuth2相关的配置信息");

            return _oAuth2Client;
        }

        #region WebClient Get

        public void WebSendGet<T>(string url, string scope, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            WebSendGet(serviceName, url, scope, DefaultContentType, callback, failCallback, needOAuthAuthenticated);
        }
        public new void WebSendGet<T>(string url, string scope, string contentType, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            WebSendGet(serviceName, url, scope, contentType, null, callback, failCallback, needOAuthAuthenticated);
        }
        public void WebSendGet<T>(string url, string scope, string contentType, Dictionary<string, string> headers, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            WebSendGet(serviceName, url, scope, contentType, headers, callback, failCallback, needOAuthAuthenticated);
        }

        public async Task WebSendGetAsync<T>(string url, string scope, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            await WebSendGetAsync(serviceName, url, scope, DefaultContentType, callback, failCallback, needOAuthAuthenticated);
        }
        public new async Task WebSendGetAsync<T>(string url, string scope, string contentType, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            await WebSendGetAsync(serviceName, url, scope, contentType, null, callback, failCallback, needOAuthAuthenticated);
        }
        public async Task WebSendGetAsync<T>(string url, string scope, string contentType, Dictionary<string, string> headers, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            await WebSendGetAsync(serviceName, url, scope, contentType, headers, callback, failCallback, needOAuthAuthenticated);
        }

        #endregion

        #region WebClient Delete

        public async Task WebSendDeleteAsync<T>(string url, string scope, Action<T> callback,
            Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            await WebSendDeleteAsync(serviceName, url, scope, DefaultContentType, callback, failCallback, needOAuthAuthenticated);
        }
        public new async Task WebSendDeleteAsync<T>(string url, string scope, string contentType, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            await WebSendDeleteAsync(serviceName, url, scope, contentType, null, callback, failCallback, needOAuthAuthenticated);
        }
        public async Task WebSendDeleteAsync<T>(string url, string scope, string contentType, Dictionary<string, string> headers, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            await WebSendDeleteAsync(serviceName, url, scope, contentType, headers, callback, failCallback, needOAuthAuthenticated);
        }
        #endregion

        #region WebClient Post
        public void WebSendPost<T>(string url, string scope, string postJsonData, Action<T> callback,
            Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            WebSendPost(serviceName, url, scope, postJsonData, null, callback, failCallback, needOAuthAuthenticated);
        }
        public void WebSendPost<T>(string url, string scope, string postJsonData, Dictionary<string, string> headers, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            WebSendPost(serviceName, url, scope, postJsonData, DefaultContentType, headers, callback,
                    failCallback, needOAuthAuthenticated);
        }
        public new void WebSendPost<T>(string url, string scope, string postJsonData, string contentType, Dictionary<string, string> headers, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            WebSendPost<T>(url, scope, postJsonData, contentType, headers, callback, failCallback, needOAuthAuthenticated);
        }

        public async Task WebSendPostAsync<T>(string url, string scope, string postJsonData, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            await WebSendPostAsync(serviceName, url, scope, postJsonData, null, callback, failCallback, needOAuthAuthenticated);
        }
        public async Task WebSendPostAsync<T>(string url, string scope, string postJsonData, Dictionary<string, string> headers, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            await WebSendPostAsync(serviceName, url, scope, postJsonData, DefaultContentType, headers, callback, failCallback, needOAuthAuthenticated);
        }
        public new async Task WebSendPostAsync<T>(string url, string scope, string postJsonData, string contentType, Dictionary<string, string> headers, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            await WebSendPostAsync(serviceName, url, scope, postJsonData, contentType, headers, callback, failCallback, needOAuthAuthenticated);
        }

        #endregion

        #region WebClient Put

        public async Task WebSendPutAsync<T>(string url, string scope, string postJsonData, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            await WebSendPutAsync(serviceName, url, scope, postJsonData, null, callback, failCallback, needOAuthAuthenticated);
        }
        public async Task WebSendPutAsync<T>(string url, string scope, string postJsonData, Dictionary<string, string> headers, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            await WebSendPutAsync(serviceName, url, scope, postJsonData, DefaultContentType, headers, callback, failCallback, needOAuthAuthenticated);
        }
        public new async Task WebSendPutAsync<T>(string url, string scope, string postJsonData, string contentType, Dictionary<string, string> headers, Action<T> callback, Action<HttpStatusCode, string> failCallback, bool needOAuthAuthenticated = false)
        {
            await WebSendPutAsync(serviceName, url, scope, postJsonData, contentType, headers, callback, failCallback, needOAuthAuthenticated);
        }

        #endregion

        #region Download File

        public async Task DownLoadFile(string url, Action<byte[]> callback, Action<string> failCallback)
        {
            await base.DownLoadFile(serviceName, url, callback, failCallback);
        }

        public async Task<byte[]> GetDownloadFileBytes(string url)
        {
            return await base.GetDownloadFileBytes(serviceName, url);
        }
        #endregion
    }
}
