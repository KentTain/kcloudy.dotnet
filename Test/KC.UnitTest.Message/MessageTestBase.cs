using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Framework.Util;
using KC.Service.Message;
using KC.Service.Constants;
using KC.Service.WebApiService.Business;
using KC.IdentityModel.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KC.UnitTest.Message
{
    public abstract class MessageTestBase : CommonTestBase
    {
        protected IServiceProvider ServiceProvider;
        private static ILogger _logger;

        public MessageTestBase(CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(MessageTestBase));
            ServiceProvider = Services.BuildServiceProvider();
        }

        protected override void InjectTenant(Framework.Tenant.Tenant tenant)
        {
            base.InjectTenant(tenant);

            KC.Service.Util.DependencyInjectUtil.InjectService(Services);
            KC.Service.Message.DependencyInjectUtil.InjectService(Services);

            ServiceProvider = Services.BuildServiceProvider();
        }

        protected override void SetUp()
        {
            base.SetUp();

            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                var cache = scope.ServiceProvider.GetService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
                Service.CacheUtil.Cache = cache;
            }

            Services.AddSingleton<ITenantUserApiService, TenantUserApiService>();
        }

        #region HttpClient
        /// <summary>
        /// 获取accesstoken
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="baseAddress"></param>
        /// <param name="clientScope"></param>
        /// <returns></returns>
        protected static async Task<string> GetAccessTokenAsync(string clientId, string clientSecret, string clientScope)
        {
            var client = new HttpClient();

            var tokenEndpointCacheKey = CacheKeyConstants.Prefix.TenantAuthTokenEndpoint + clientId;
            var tokenEndpoint = Service.CacheUtil.GetCache<string>(tokenEndpointCacheKey);
            if (string.IsNullOrEmpty(tokenEndpoint))
            {
                if (GlobalConfig.SSOWebDomain.StartsWith("https"))
                {
                    var disco = await client.GetDiscoveryDocumentAsync(GlobalConfig.SSOWebDomain);
                    if (disco.IsError) throw new Exception(disco.Error);
                    tokenEndpoint = disco.TokenEndpoint;
                    
                }
                else
                {
                    var disco = await client.GetDiscoveryDocumentAsync(GlobalConfig.SSOWebDomain);
                    if (disco.IsError) throw new Exception(disco.Error);
                    tokenEndpoint = disco.TokenEndpoint;
                }

                Service.CacheUtil.SetCache(tokenEndpointCacheKey, tokenEndpoint, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));

                LogUtil.LogDebug("--KC.Service.Extension.ClientExtensions tokenEndpoint: " + tokenEndpoint);
            }

            var accessTokenCacheKey = CacheKeyConstants.Prefix.TenantAccessToken + clientId;
            var accessToken = Service.CacheUtil.GetCache<string>(accessTokenCacheKey);
            if (string.IsNullOrEmpty(accessToken))
            {
                var response = await client.RequestClientCredentialsTokenAsync(
                    new ClientCredentialsTokenRequest
                    {
                        Address = tokenEndpoint,
                        ClientId = clientId,
                        ClientSecret = clientSecret,
                        Scope = clientScope
                    });
                if (response.IsError) throw new Exception(response.Error);

                accessToken = response.AccessToken;
                var expiredTimeSpan = response.ExpiresIn - 100;
                Service.CacheUtil.SetCache(accessTokenCacheKey, accessToken, TimeSpan.FromSeconds(expiredTimeSpan));

                LogUtil.LogDebug("--KC.Service.Extension.ClientExtensions accessToken: " + accessToken);
            }

            return accessToken;
        }

        #endregion
    }
}
