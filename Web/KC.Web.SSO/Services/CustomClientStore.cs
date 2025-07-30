using KC.IdentityServer4;
using KC.IdentityServer4.Models;
using KC.IdentityServer4.Stores;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Web.Constants;
using KC.Service.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KC.Web.SSO.Services
{
    /// <summary>
    /// 读取Tenant中的开通Application数据，获取授权的Client</br>
    /// https://damienbod.com/2017/12/30/using-an-ef-core-database-for-the-KC.IdentityServer4-configuration-data/
    /// </summary>
    public class CustomClientStore : IClientStore
    {
        private readonly ITenantUserService _tenantUserService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;

        public CustomClientStore(
            ITenantUserService tenantService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CustomClientStore> logger)
        {
            _tenantUserService = tenantService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        async Task<Client> IClientStore.FindClientByIdAsync(string clientId)
        {
            if (clientId.Equals(OpenIdConnectConstants.TestClient_ClientId))
            {
                return await Task.FromResult(new Client()
                {
                    ClientId = OpenIdConnectConstants.TestClient_ClientId,
                    ClientName = OpenIdConnectConstants.TestClient_ClientId,
                    ClientSecrets =
                    {
                        new Secret(OpenIdConnectConstants.TestClient_ClientSecret.Sha256())
                    },
                    Enabled = true,
                    RequireConsent = false,
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowAccessTokensViaBrowser = true,

                    //AllowedCorsOrigins = new List<string>() { "*" },

                    AccessTokenLifetime = 60 * 60,//1 hours
                    AccessTokenType = AccessTokenType.Jwt,

                    AlwaysIncludeUserClaimsInIdToken = true,
                    IdentityTokenLifetime = Service.Constants.TimeOutConstants.CookieTimeOut * 60,// 15 minutes

                    AllowedScopes = { ApplicationConstant.AdminScope, ApplicationConstant.AppScope, ApplicationConstant.AccScope, ApplicationConstant.EconScope, ApplicationConstant.DocScope, ApplicationConstant.CfgScope, ApplicationConstant.DicScope, ApplicationConstant.MsgScope, ApplicationConstant.CrmScope, ApplicationConstant.SrmScope, ApplicationConstant.PrdScope, ApplicationConstant.PmcScope, ApplicationConstant.PortalScope, ApplicationConstant.SomScope, ApplicationConstant.PomScope, ApplicationConstant.WmsScope, ApplicationConstant.WorkflowScope, ApplicationConstant.PayScope, ApplicationConstant.WXScope, ApplicationConstant.JrScope, ApplicationConstant.BlogScope }
                });
            }

            if (clientId.Equals(OpenIdConnectConstants.WebApiTestClient_ClientId))
            {
                return await Task.FromResult(new Client
                {
                    ClientId = OpenIdConnectConstants.WebApiTestClient_ClientId,
                    ClientName = "api's swagger test",
                    ClientSecrets =
                    {
                        new Secret(OpenIdConnectConstants.TestClient_ClientSecret.Sha256())
                    },
                    Enabled = true,
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,

                    AccessTokenLifetime = Service.Constants.TimeOutConstants.AccessTokenTimeOut * 60,//1 hours
                    AccessTokenType = AccessTokenType.Jwt,

                    AlwaysIncludeUserClaimsInIdToken = true,
                    IdentityTokenLifetime = Service.Constants.TimeOutConstants.CookieTimeOut * 60,// 15 minutes

                    RedirectUris = {
                    "http://localhost:1004/oauth2-redirect.html",//com.webapi.admin
                    "http://localhost:1006/oauth2-redirect.html",//com.webapi.app
                    "http://cdba.localhost:1004/oauth2-redirect.html",//com.webapi.admin
                    "http://cdba.localhost:1006/oauth2-redirect.html",//com.webapi.app
                    "http://cdba.localhost:1102/oauth2-redirect.html",//com.webapi.config
                    "http://cdba.localhost:1104/oauth2-redirect.html",//com.webapi.dict
                    "http://cdba.localhost:2002/oauth2-redirect.html",//com.webapi.account

                    "http://ctest.localhost:1004/oauth2-redirect.html",//com.webapi.admin
                    "http://ctest.localhost:1006/oauth2-redirect.html",//com.webapi.app
                    "http://ctest.localhost:1102/oauth2-redirect.html",//com.webapi.config
                    "http://ctest.localhost:1104/oauth2-redirect.html",//com.webapi.dict
                    "http://ctest.localhost:2002/oauth2-redirect.html",//com.webapi.account

                    "http://cbuy.localhost:1004/oauth2-redirect.html",//com.webapi.admin
                    "http://cbuy.localhost:1006/oauth2-redirect.html",//com.webapi.app
                    "http://cbuy.localhost:1102/oauth2-redirect.html",//com.webapi.config
                    "http://cbuy.localhost:1104/oauth2-redirect.html",//com.webapi.dict
                    "http://cbuy.localhost:2002/oauth2-redirect.html",//com.webapi.account

                    "http://testadminapi.kcloudy.com/oauth2-redirect.html",//com.webapi.admin
                    "http://testappapi.kcloudy.com/oauth2-redirect.html",//com.webapi.app
                    "http://cdba.testcfgapi.kcloudy.com/oauth2-redirect.html",//com.webapi.config
                    "http://cdba.testdicapi.kcloudy.com/oauth2-redirect.html",//com.webapi.dict
                    "http://cdba.testaccapi.kcloudy.com/oauth2-redirect.html",//com.webapi.account

                    "http://ctest.testcfgapi.kcloudy.com/oauth2-redirect.html",//com.webapi.config
                    "http://ctest.testdicapi.kcloudy.com/oauth2-redirect.html",//com.webapi.dict
                    "http://ctest.testaccapi.kcloudy.com/oauth2-redirect.html",//com.webapi.account

                    "http://betaadminapi.kcloudy.com/oauth2-redirect.html",//com.webapi.admin
                    "http://betaappapi.kcloudy.com/oauth2-redirect.html",//com.webapi.app
                    "http://cdba.betacfgapi.kcloudy.com/oauth2-redirect.html",//com.webapi.config
                    "http://cdba.betadicapi.kcloudy.com/oauth2-redirect.html",//com.webapi.dict
                    "http://cdba.betaaccapi.kcloudy.com/oauth2-redirect.html",//com.webapi.account

                    "http://ctest.betacfgapi.kcloudy.com/oauth2-redirect.html",//com.webapi.config
                    "http://ctest.betadicapi.kcloudy.com/oauth2-redirect.html",//com.webapi.dict
                    "http://ctest.betaaccapi.kcloudy.com/oauth2-redirect.html",//com.webapi.account

                    "http://adminapi.kcloudy.com/oauth2-redirect.html",//com.webapi.admin
                    "http://appapi.kcloudy.com/oauth2-redirect.html",//com.webapi.app
                    "http://cdba.cfgapi.kcloudy.com/oauth2-redirect.html",//com.webapi.config
                    "http://cdba.dicapi.kcloudy.com/oauth2-redirect.html",//com.webapi.dict
                    "http://cdba.accapi.kcloudy.com/oauth2-redirect.html",//com.webapi.account

                    "http://ctest.cfgapi.kcloudy.com/oauth2-redirect.html",//com.webapi.config
                    "http://ctest.dicapi.kcloudy.com/oauth2-redirect.html",//com.webapi.dict
                    "http://ctest.accapi.kcloudy.com/oauth2-redirect.html",//com.webapi.account
                     },
                    AllowedScopes = { ApplicationConstant.AdminScope, ApplicationConstant.AppScope, ApplicationConstant.AccScope, ApplicationConstant.EconScope, ApplicationConstant.DocScope, ApplicationConstant.CfgScope, ApplicationConstant.DicScope, ApplicationConstant.MsgScope, ApplicationConstant.CrmScope, ApplicationConstant.SrmScope, ApplicationConstant.PrdScope, ApplicationConstant.PmcScope, ApplicationConstant.PortalScope, ApplicationConstant.SomScope, ApplicationConstant.PomScope, ApplicationConstant.WmsScope, ApplicationConstant.WorkflowScope, ApplicationConstant.PayScope, ApplicationConstant.WXScope, ApplicationConstant.JrScope, ApplicationConstant.BlogScope }
                });
            }

            var tenantName = TenantConstant.GetDecodeClientId(clientId);
            var tenant = await _tenantUserService.GetTenantByNameOrNickNameAsync(tenantName);
            _logger.LogDebug(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|CustomClientStore FindClientByIdAsync with clientId--{0}", tenantName);

            var postLogoutUrls = tenant.Hostnames.ToList();
            var redirectUrls = tenant.Hostnames.Select(m => m.TrimEndSlash() + Web.Constants.OpenIdConnectConstants.CallbackPath).ToList();
            var frontLogoutCallbackUrl = tenant.Hostnames.FirstOrDefault().TrimEndSlash() + Web.Constants.OpenIdConnectConstants.SignOutPath;

            //for java spring security
            redirectUrls.AddRange(tenant.Hostnames.Select(m => m.TrimEndSlash() + Web.Constants.OpenIdConnectConstants.JavaCallbackPath).ToList());
            //for java swagger security
            redirectUrls.AddRange(tenant.Hostnames.Select(m => m.TrimEndSlash() + Web.Constants.OpenIdConnectConstants.JavaSwaggerCallbackPath).ToList());

            var scopes = new List<string>(){
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess };
            scopes.AddRange(tenant.Scopes.Select(m => m.Key));

            return await Task.FromResult(new Client
            {
                ClientName = tenant.TenantName,
                ClientId = TenantConstant.GetClientIdByTenant(tenant),
                ClientSecrets =
                    {
                        new Secret(TenantConstant.GetClientSecretByTenant(tenant).Sha256())
                    },
                Enabled = true,

                // RequireClientSecret might as well be true if you are giving this client a secret
                //RequireClientSecret = true,

                RequireConsent = false,
                AllowOfflineAccess = true,

                FrontChannelLogoutSessionRequired = true,
                FrontChannelLogoutUri = frontLogoutCallbackUrl,

                //对应客户端设置：
                //Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectOptions.CallbackPath
                RedirectUris = redirectUrls,
                //对应客户端设置：
                //Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectOptions.RemoteSignOutPath
                PostLogoutRedirectUris = postLogoutUrls,

                AllowAccessTokensViaBrowser = true,
                AccessTokenLifetime = 60 * 60,//1 hours
                AccessTokenType = AccessTokenType.Jwt,

                AlwaysIncludeUserClaimsInIdToken = true,
                IdentityTokenLifetime = Service.Constants.TimeOutConstants.CookieTimeOut * 60,// 20 minutes

                //AllowedCorsOrigins = new List<string>() { "*" },

                RefreshTokenUsage = TokenUsage.OneTimeOnly,
                RefreshTokenExpiration = TokenExpiration.Sliding,

                AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                AllowedScopes = scopes
            });
        }
    }
}
