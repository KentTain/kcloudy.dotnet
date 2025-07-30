using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Framework.Util;
using KC.Service.Constants;
using KC.Service.WebApiService.Business;
using KC.IdentityModel.Client;
using Microsoft.Extensions.DependencyInjection;
using KC.Service.App;
using Microsoft.Extensions.Logging;

namespace KC.UnitTest.App
{
    public abstract class AppTestBase : CommonTestBase
    { 
        protected IServiceProvider ServiceProvider;

        protected ITenantUserApiService TenantUserApiService;

        private static ILogger _logger;
        public AppTestBase(CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(AppTestBase));
            ServiceProvider = Services.BuildServiceProvider();
        }


        protected override void InjectTenant(Framework.Tenant.Tenant tenant)
        {
            base.InjectTenant(tenant);

            KC.Service.Util.DependencyInjectUtil.InjectService(Services);
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

            #region AutoMapper对象注入
            var profiles = KC.Service.AutoMapper.AutoMapperConfiguration.GetAllProfiles()
                .Union(KC.Service.App.AutoMapper.AutoMapperConfiguration.GetAllProfiles());

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfiles(profiles);
            });
            Services.AddSingleton(config);
            var mapper = config.CreateMapper();
            Services.AddSingleton(mapper);

            #endregion

            Services.AddSingleton<ITenantUserApiService, TenantUserApiService>();

            KC.Service.App.DependencyInjectUtil.InjectService(Services);

            ServiceProvider = Services.BuildServiceProvider();
            TenantUserApiService = ServiceProvider.GetService<ITenantUserApiService>();
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
                var disco = await client.GetDiscoveryDocumentAsync(GlobalConfig.SSOWebDomain);
                disco.Policy.RequireHttps = GlobalConfig.SSOWebDomain.StartsWith("https");

                if (disco.IsError) throw new Exception(disco.Error);

                tokenEndpoint = disco.TokenEndpoint;
                if (!string.IsNullOrEmpty(tokenEndpoint))
                    Service.CacheUtil.SetCache(tokenEndpointCacheKey, tokenEndpoint, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));

                _logger.LogDebug("--KC.Service.Extension.ClientExtensions tokenEndpoint: " + tokenEndpoint);
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

                _logger.LogDebug("--KC.Service.Extension.ClientExtensions accessToken: " + accessToken);
            }

            return accessToken;
        }

        #endregion
    }
}
