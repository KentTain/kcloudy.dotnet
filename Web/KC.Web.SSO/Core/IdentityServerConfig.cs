using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Extension;
using KC.IdentityServer4;
using KC.IdentityServer4.Models;
using KC.Framework.Tenant;
using KC.Web.Constants;

namespace KC.Web.SSO
{
    /// <summary>
    /// 移至：KC.Web.SSO.Services.CustomClientStore，读取Tenant数据获取IdentityResource及ApiResource
    /// </summary>
    public class IdentityServerConfig
    {
        public static IEnumerable<Tenant> TenantsForTest = new List<Tenant>(new[]
        {
            TenantConstant.DbaTenantApiAccessInfo,
            TenantConstant.TestTenantApiAccessInfo,
            TenantConstant.BuyTenantApiAccessInfo,
            TenantConstant.SaleTenantApiAccessInfo
        });

        // scopes define the resources in your system
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource(ApplicationConstant.SsoScope, ApplicationConstant.SsoAppName),
                new ApiResource(ApplicationConstant.AdminScope, ApplicationConstant.AdminAppName),
                new ApiResource(ApplicationConstant.BlogScope, ApplicationConstant.BlogAppName),

                new ApiResource(ApplicationConstant.AppScope, ApplicationConstant.AppAppName),
                new ApiResource(ApplicationConstant.CfgScope, ApplicationConstant.ConfigAppName),
                new ApiResource(ApplicationConstant.DicScope, ApplicationConstant.DictAppName),
                new ApiResource(ApplicationConstant.MsgScope, ApplicationConstant.MsgAppName),

                new ApiResource(ApplicationConstant.AccScope, ApplicationConstant.AccAppName),
                new ApiResource(ApplicationConstant.EconScope, ApplicationConstant.EconAppName),
                new ApiResource(ApplicationConstant.DocScope, ApplicationConstant.DocAppName),
                new ApiResource(ApplicationConstant.HrScope, ApplicationConstant.HrAppName),

                new ApiResource(ApplicationConstant.CrmScope, ApplicationConstant.CrmAppName),
                new ApiResource(ApplicationConstant.SrmScope, ApplicationConstant.SrmAppName),
                new ApiResource(ApplicationConstant.PrdScope, ApplicationConstant.PrdAppName),
                new ApiResource(ApplicationConstant.PmcScope, ApplicationConstant.PmcAppName),

                new ApiResource(ApplicationConstant.PortalScope, ApplicationConstant.PortalAppName),
                new ApiResource(ApplicationConstant.SomScope, ApplicationConstant.SomAppName),
                new ApiResource(ApplicationConstant.PomScope, ApplicationConstant.PomAppName),
                new ApiResource(ApplicationConstant.WmsScope, ApplicationConstant.WmsAppName),

                new ApiResource(ApplicationConstant.JrScope, ApplicationConstant.JrAppName),
                new ApiResource(ApplicationConstant.TrainScope, ApplicationConstant.TrainAppName),

                new ApiResource(ApplicationConstant.WorkflowScope, ApplicationConstant.WorkflowAppName),
                new ApiResource(ApplicationConstant.PayScope, ApplicationConstant.PayAppName),
                new ApiResource(ApplicationConstant.WXScope, ApplicationConstant.WXAppName),

            };
        }

        //private static DateTime _defaultExpiredTime = DateTime.Now.AddMonths(1);
        private static DateTime _defaultExpiredTime = DateTime.Now.AddMinutes(15);
        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            var result = new List<Client>();
            // 配置测试登录的client
            result.Add(new Client
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
            // 配置测试Api的client
            result.Add(new Client
            {
                ClientId = OpenIdConnectConstants.WebApiTestClient_ClientId,
                ClientName = "api's swagger test",
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
            // 所有租户的client配置
            foreach (var tenant in TenantsForTest)
            {
                //tenant.Hostnames = TenantConstant.GetTenantHosts(tenant.TenantName);
                var postLogoutUrls = tenant.Hostnames.ToList();
                var redirectUrls = tenant.Hostnames.Select(m => m.TrimEndSlash() + Web.Constants.OpenIdConnectConstants.CallbackPath).ToList();
                var frontLogoutCallbackUrl = tenant.Hostnames.FirstOrDefault().TrimEndSlash() + Web.Constants.OpenIdConnectConstants.SignOutPath;

                //for java spring security
                redirectUrls.AddRange(tenant.Hostnames.Select(m => m.TrimEndSlash() + Web.Constants.OpenIdConnectConstants.JavaCallbackPath).ToList());
                //for java swagger security
                redirectUrls.AddRange(tenant.Hostnames.Select(m => m.TrimEndSlash() + Web.Constants.OpenIdConnectConstants.JavaSwaggerCallbackPath).ToList());

                result.Add(new Client
                {
                    ClientName = tenant.TenantName,
                    ClientId = TenantConstant.GetClientIdByTenant(tenant),
                    ClientSecrets =
                    {
                        new Secret(TenantConstant.GetClientSecretByTenant(tenant).Sha256())
                        //new Secret(TenantConstant.GetClientSecretByTenant(tenant))
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
                    //AllowedGrantTypes = GrantTypes.Hybrid,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        ApplicationConstant.AdminScope, ApplicationConstant.AppScope, ApplicationConstant.AccScope, ApplicationConstant.EconScope, ApplicationConstant.DocScope, ApplicationConstant.CfgScope, ApplicationConstant.DicScope, ApplicationConstant.MsgScope, ApplicationConstant.CrmScope, ApplicationConstant.SrmScope, ApplicationConstant.PrdScope, ApplicationConstant.PmcScope, ApplicationConstant.PortalScope, ApplicationConstant.SomScope, ApplicationConstant.PomScope, ApplicationConstant.WmsScope, ApplicationConstant.WorkflowScope, ApplicationConstant.PayScope, ApplicationConstant.WXScope, ApplicationConstant.JrScope, ApplicationConstant.BlogScope
                    }
                });
            }
            return result;
        }

        #region 获取租户客户端的认知Host列表
        public static string[] GetTenantHosts(string tenantName)
        {
            tenantName = tenantName.ToLower();

            switch (Framework.Base.GlobalConfig.SystemType)
            {
                case Framework.Base.SystemType.Dev:
                    #region 本地开发环境
                    return new[] {
                    //--------------本地开发环境：调试--------------
                    "http://" + tenantName + ".localhost:1003/",//com.web.admin
                    "https://" + tenantName + ".localhost:1013/",//com.web.admin
                    "http://" + tenantName + ".localhost:1004/",//com.webapi.admin
                    "https://" + tenantName + ".localhost:1014/",//com.webapi.admin
                    //博客管理
                    "http://" + tenantName + ".localhost:1005/",//com.web.blog
                    "https://" + tenantName + ".localhost:1015/",//com.web.blog
                    "http://" + tenantName + ".localhost:1006/",//com.webapi.blog
                    "https://" + tenantName + ".localhost:1016/",//com.webapi.blog
                    //配置管理
                    "http://" + tenantName + ".localhost:1101/",//com.web.config
                    "https://" + tenantName + ".localhost:1111/",//com.web.config
                    "http://" + tenantName + ".localhost:1102/",//com.webapi.config
                    "https://" + tenantName + ".localhost:1112/",//com.webapi.config
                    //字典管理
                    "http://" + tenantName + ".localhost:1103/",//com.web.dict
                    "https://" + tenantName + ".localhost:1113/",//com.web.dict
                    "http://" + tenantName + ".localhost:1104/",//com.webapi.dict
                    "https://" + tenantName + ".localhost:1114/",//com.webapi.dict
                    //应用管理
                    "http://" + tenantName + ".localhost:1105/",//com.web.app
                    "https://" + tenantName + ".localhost:1115/",//com.web.app
                    "http://" + tenantName + ".localhost:1106/",//com.webapi.app
                    "https://" + tenantName + ".localhost:1116/",//com.webapi.app
                    //消息管理
                    "http://" + tenantName + ".localhost:1107/",//com.web.message
                    "https://" + tenantName + ".localhost:1117/",//com.web.message
                    "http://" + tenantName + ".localhost:1108/",//com.webapi.message
                    "https://" + tenantName + ".localhost:1118/",//com.webapi.message
                    
                    //账户管理
                    "http://" + tenantName + ".localhost:2001/",//com.web.account
                    "https://" + tenantName + ".localhost:2011/",//com.web.account
                    "http://" + tenantName + ".localhost:2002/",//com.webapi.account
                    "https://" + tenantName + ".localhost:2012/",//com.webapi.account
                    //合同管理
                    "http://" + tenantName + ".localhost:2003/",//com.web.contract
                    "https://" + tenantName + ".localhost:2013/",//com.web.contract
                    "http://" + tenantName + ".localhost:2004/",//com.webapi.contract
                    "https://" + tenantName + ".localhost:2014/",//com.webapi.contract
                    //文档管理
                    "http://" + tenantName + ".localhost:2005/",//com.web.doc
                    "https://" + tenantName + ".localhost:2015/",//com.web.doc
                    "http://" + tenantName + ".localhost:2006/",//com.webapi.doc
                    "https://" + tenantName + ".localhost:2016/",//com.webapi.doc
                    //人事管理
                    "http://" + tenantName + ".localhost:2007/",//com.web.hr
                    "https://" + tenantName + ".localhost:2017/",//com.web.hr
                    "http://" + tenantName + ".localhost:2008/",//com.webapi.hr
                    "https://" + tenantName + ".localhost:2018/",//com.webapi.hr
                    
                    //客户管理
                    "http://" + tenantName + ".localhost:3001/",//com.web.customer
                    "https://" + tenantName + ".localhost:3011/",//com.web.customer
                    "http://" + tenantName + ".localhost:3002/",//com.webapi.customer
                    "https://" + tenantName + ".localhost:3012/",//com.webapi.customer
                    //供应商管理
                    "http://" + tenantName + ".localhost:3003/",//com.web.supply
                    "https://" + tenantName + ".localhost:3013/",//com.web.supply
                    "http://" + tenantName + ".localhost:3004/",//com.webapi.supply
                    "https://" + tenantName + ".localhost:3014/",//com.webapi.supply
                    //商品管理
                    "http://" + tenantName + ".localhost:3005/",//com.web.product
                    "https://" + tenantName + ".localhost:3015/",//com.web.product
                    "http://" + tenantName + ".localhost:3006/",//com.webapi.product
                    "https://" + tenantName + ".localhost:3016/",//com.webapi.product
                    //物料管理
                    "http://" + tenantName + ".localhost:3007/",//com.web.material
                    "https://" + tenantName + ".localhost:3017/",//com.web.material
                    "http://" + tenantName + ".localhost:3008/",//com.webapi.material
                    "https://" + tenantName + ".localhost:3018/",//com.webapi.material

                    //门户管理
                    "http://" + tenantName + ".localhost:4001/",//com.web.portal
                    "https://" + tenantName + ".localhost:4011/",//com.web.portal
                    "http://" + tenantName + ".localhost:4002/",//com.webapi.portal
                    "https://" + tenantName + ".localhost:4012/",//com.webapi.portal

                    //培训管理
                    "http://" + tenantName + ".localhost:6001/",//com.web.training
                    "https://" + tenantName + ".localhost:6011/",//com.web.training
                    "http://" + tenantName + ".localhost:6002/",//com.webapi.training
                    "https://" + tenantName + ".localhost:6012/",//com.webapi.training

                    //流程管理
                    "http://" + tenantName + ".localhost:7001/",//com.web.workflow
                    "https://" + tenantName + ".localhost:7011/",//com.web.workflow
                    "http://" + tenantName + ".localhost:7002/",//com.webapi.workflow
                    "https://" + tenantName + ".localhost:7012/",//com.webapi.workflow
                    //支付管理
                    "http://" + tenantName + ".localhost:8001/",//com.web.payment
                    "https://" + tenantName + ".localhost:8011/",//com.web.payment
                    "http://" + tenantName + ".localhost:8002/",//com.webapi.payment
                    "https://" + tenantName + ".localhost:8012/",//com.webapi.payment

                    //微信管理
                    "http://" + tenantName + ".localhost:9001/",//com.web.wechat
                    "https://" + tenantName + ".localhost:9011/",//com.web.wechat
                    "http://" + tenantName + ".localhost:9002/",//com.webapi.wechat
                    "https://" + tenantName + ".localhost:9012/",//com.webapi.wechat

                    //--------------本地开发发布环境：linux/IIS--------------
                    //配置管理
                    "http://" + tenantName + ".localcfg.kcloudy.com/",
                    "http://" + tenantName + ".localcfgapi.kcloudy.com/",
                    "https://" + tenantName + ".localcfg.kcloudy.com/",
                    "https://" + tenantName + ".localcfgapi.kcloudy.com/",
                    //字典管理
                    "http://" + tenantName + ".localdic.kcloudy.com/",
                    "http://" + tenantName + ".localdicapi.kcloudy.com/",
                    "https://" + tenantName + ".localdic.kcloudy.com/",
                    "https://" + tenantName + ".localdicapi.kcloudy.com/",
                    //应用管理
                    "http://" + tenantName + ".localapp.kcloudy.com/",
                    "http://" + tenantName + ".localappapi.kcloudy.com/",
                    "https://" + tenantName + ".localapp.kcloudy.com/",
                    "https://" + tenantName + ".localappapi.kcloudy.com/",
                    //消息管理
                    "http://" + tenantName + ".localmsg.kcloudy.com/",
                    "http://" + tenantName + ".localmsgapi.kcloudy.com/",
                    "https://" + tenantName + ".localmsg.kcloudy.com/",
                    "https://" + tenantName + ".localmsgapi.kcloudy.com/",

                    //账户管理
                    "http://" + tenantName + ".localacc.kcloudy.com/",
                    "http://" + tenantName + ".localaccapi.kcloudy.com/",
                    "https://" + tenantName + ".localacc.kcloudy.com/",
                    "https://" + tenantName + ".localaccapi.kcloudy.com/",
                    //合同管理
                    "http://" + tenantName + ".localecon.kcloudy.com/",
                    "https://" + tenantName + ".localeconapi.kcloudy.com/",
                    "http://" + tenantName + ".localecon.kcloudy.com/",
                    "https://" + tenantName + ".localeconapi.kcloudy.com/",
                    //文档管理
                    "http://" + tenantName + ".localdoc.kcloudy.com/",
                    "http://" + tenantName + ".localdocapi.kcloudy.com/",
                    "https://" + tenantName + ".localdoc.kcloudy.com/",
                    "https://" + tenantName + ".localdocapi.kcloudy.com/",
                    //人事管理
                    "http://" + tenantName + ".localhr.kcloudy.com/",
                    "http://" + tenantName + ".localhrapi.kcloudy.com/",
                    "https://" + tenantName + ".localhr.kcloudy.com/",
                    "https://" + tenantName + ".localhrapi.kcloudy.com/",
                    
                    //客户管理
                    "http://" + tenantName + ".localcrm.kcloudy.com/",
                    "http://" + tenantName + ".localcrmapi.kcloudy.com/",
                    "https://" + tenantName + ".localcrm.kcloudy.com/",
                    "https://" + tenantName + ".localcrmapi.kcloudy.com/",
                    //供应商管理
                    "http://" + tenantName + ".localsrm.kcloudy.com/",
                    "http://" + tenantName + ".localsrmapi.kcloudy.com/",
                    "https://" + tenantName + ".localsrm.kcloudy.com/",
                    "https://" + tenantName + ".localsrmapi.kcloudy.com/",
                    //商品管理
                    "http://" + tenantName + ".localprd.kcloudy.com/",
                    "http://" + tenantName + ".localprdapi.kcloudy.com/",
                    "https://" + tenantName + ".localprd.kcloudy.com/",
                    "https://" + tenantName + ".localprdapi.kcloudy.com/",
                    //物料管理
                    "http://" + tenantName + ".localpmc.kcloudy.com/",
                    "http://" + tenantName + ".localpmcapi.kcloudy.com/",
                    "https://" + tenantName + ".localpmc.kcloudy.com/",
                    "https://" + tenantName + ".localpmcapi.kcloudy.com/",
                    
                    //门户管理
                    "http://" + tenantName + ".local.kcloudy.com/",//com.web.portal
                    "https://" + tenantName + ".local.kcloudy.com/",//com.web.portal
                    "http://" + tenantName + ".localapi.kcloudy.com/",//com.webapi.portal
                    "https://" + tenantName + ".localapi.kcloudy.com/",//com.webapi.portal

                    //培训管理
                    "http://" + tenantName + ".localtrn.kcloudy.com/",//com.web.training
                    "https://" + tenantName + ".localtrn.kcloudy.com/",//com.web.training
                    "http://" + tenantName + ".localtrnapi.kcloudy.com/",//com.webapi.training
                    "https://" + tenantName + ".localtrnapi.kcloudy.com/",//com.webapi.training

                    //流程管理
                    "http://" + tenantName + ".localflow.kcloudy.com/",//com.web.workflow
                    "https://" + tenantName + ".localflow.kcloudy.com/",//com.web.workflow
                    "http://" + tenantName + ".localflowapi.kcloudy.com/",//com.webapi.workflow
                    "https://" + tenantName + ".localflowapi.kcloudy.com/",//com.webapi.workflow
                    //支付管理
                    "http://" + tenantName + ".localpay.kcloudy.com/",//com.web.payment
                    "https://" + tenantName + ".localpay.kcloudy.com/",//com.web.payment
                    "http://" + tenantName + ".localpayapi.kcloudy.com/",//com.webapi.payment
                    "https://" + tenantName + ".localpayapi.kcloudy.com/",//com.webapi.payment

                    //微信管理
                    "http://" + tenantName + ".localwx.kcloudy.com/",//com.web.wechat
                    "https://" + tenantName + ".localwx.kcloudy.com/",//com.web.wechat
                    "http://" + tenantName + ".localwxapi.kcloudy.com/",//com.webapi.wechat
                    "https://" + tenantName + ".localwxapi.kcloudy.com/",//com.webapi.wechat
                };
                #endregion

                case Framework.Base.SystemType.Test:
                    #region 测试环境
                    return new[] {
                    //--------------测试环境--------------
                    //配置管理
                    "http://" + tenantName + ".testcfg.kcloudy.com/",
                    "http://" + tenantName + ".testcfgapi.kcloudy.com/",
                    "https://" + tenantName + ".testcfg.kcloudy.com/",
                    "https://" + tenantName + ".testcfgapi.kcloudy.com/",
                    //字典管理
                    "http://" + tenantName + ".testdic.kcloudy.com/",
                    "http://" + tenantName + ".testdicapi.kcloudy.com/",
                    "https://" + tenantName + ".testdic.kcloudy.com/",
                    "https://" + tenantName + ".testdicapi.kcloudy.com/",
                    //应用管理
                    "http://" + tenantName + ".testapp.kcloudy.com/",
                    "http://" + tenantName + ".testappapi.kcloudy.com/",
                    "https://" + tenantName + ".testapp.kcloudy.com/",
                    "https://" + tenantName + ".testappapi.kcloudy.com/",
                    //消息管理
                    "http://" + tenantName + ".testmsg.kcloudy.com/",
                    "http://" + tenantName + ".testmsgapi.kcloudy.com/",
                    "https://" + tenantName + ".testmsg.kcloudy.com/",
                    "https://" + tenantName + ".testmsgapi.kcloudy.com/",
                    
                    //账户管理
                    "http://" + tenantName + ".testacc.kcloudy.com/",
                    "http://" + tenantName + ".testaccapi.kcloudy.com/",
                    "https://" + tenantName + ".testacc.kcloudy.com/",
                    "https://" + tenantName + ".testaccapi.kcloudy.com/",
                    //合同管理
                    "http://" + tenantName + ".testecon.kcloudy.com/",//com.web.contract
                    "https://" + tenantName + ".testeconapi.kcloudy.com/",//com.web.contract
                    "http://" + tenantName + ".testecon.kcloudy.com/",//com.webapi.contract
                    "https://" + tenantName + ".testeconapi.kcloudy.com/",//com.webapi.contract
                    //文档管理
                    "http://" + tenantName + ".testdoc.kcloudy.com/",
                    "http://" + tenantName + ".testdocapi.kcloudy.com/",
                    "https://" + tenantName + ".testdoc.kcloudy.com/",
                    "https://" + tenantName + ".testdocapi.kcloudy.com/",
                    //人事管理
                    "http://" + tenantName + ".testhr.kcloudy.com/",
                    "http://" + tenantName + ".testhrapi.kcloudy.com/",
                    "https://" + tenantName + ".testhr.kcloudy.com/",
                    "https://" + tenantName + ".testhrapi.kcloudy.com/",
                    
                    //客户管理
                    "http://" + tenantName + ".testcrm.kcloudy.com/",
                    "http://" + tenantName + ".testcrmapi.kcloudy.com/",
                    "https://" + tenantName + ".testcrm.kcloudy.com/",
                    "https://" + tenantName + ".testcrmapi.kcloudy.com/",
                    //供应商管理
                    "http://" + tenantName + ".testsrm.kcloudy.com/",
                    "http://" + tenantName + ".testsrmapi.kcloudy.com/",
                    "https://" + tenantName + ".testsrm.kcloudy.com/",
                    "https://" + tenantName + ".testsrmapi.kcloudy.com/",
                    //商品管理
                    "http://" + tenantName + ".testprd.kcloudy.com/",
                    "http://" + tenantName + ".testprdapi.kcloudy.com/",
                    "https://" + tenantName + ".testprd.kcloudy.com/",
                    "https://" + tenantName + ".testprdapi.kcloudy.com/",
                    //物料管理
                    "http://" + tenantName + ".testpmc.kcloudy.com/",
                    "http://" + tenantName + ".testpmcapi.kcloudy.com/",
                    "https://" + tenantName + ".testpmc.kcloudy.com/",
                    "https://" + tenantName + ".testpmcapi.kcloudy.com/",

                    //门户管理
                    "http://" + tenantName + ".test.kcloudy.com/",//com.web.portal
                    "https://" + tenantName + ".test.kcloudy.com/",//com.web.portal
                    "http://" + tenantName + ".test.kcloudy.com/",//com.webapi.portal
                    "https://" + tenantName + ".test.kcloudy.com/",//com.webapi.portal
                    //培训管理
                    "http://" + tenantName + ".testtrn.kcloudy.com/",//com.web.training
                    "https://" + tenantName + ".testtrn.kcloudy.com/",//com.web.training
                    "http://" + tenantName + ".testtrnapi.kcloudy.com/",//com.webapi.training
                    "https://" + tenantName + ".testtrnapi.kcloudy.com/",//com.webapi.training

                    //流程管理
                    "http://" + tenantName + ".testflow.kcloudy.com/",//com.web.workflow
                    "https://" + tenantName + ".testflow.kcloudy.com/",//com.web.workflow
                    "http://" + tenantName + ".testflowapi.kcloudy.com/",//com.webapi.workflow
                    "https://" + tenantName + ".testflowapi.kcloudy.com/",//com.webapi.workflow
                    //支付管理
                    "http://" + tenantName + ".testpay.kcloudy.com/",//com.web.payment
                    "https://" + tenantName + ".testpay.kcloudy.com/",//com.web.payment
                    "http://" + tenantName + ".testpayapi.kcloudy.com/",//com.webapi.payment
                    "https://" + tenantName + ".testpayapi.kcloudy.com/",//com.webapi.payment

                    //微信管理
                    "http://" + tenantName + ".testwx.kcloudy.com/",//com.web.wechat
                    "https://" + tenantName + ".testwx.kcloudy.com/",//com.web.wechat
                    "http://" + tenantName + ".testwxapi.kcloudy.com/",//com.webapi.wechat
                    "https://" + tenantName + ".testwxapi.kcloudy.com/",//com.webapi.wechat
                };
                #endregion

                case Framework.Base.SystemType.Beta:
                    #region UAT环境
                    return new[] {
                    //--------------灰度发布环境--------------
                    //配置管理
                    "http://" + tenantName + ".betacfg.kcloudy.com/",
                    "http://" + tenantName + ".betacfgapi.kcloudy.com/",
                    "https://" + tenantName + ".betacfg.kcloudy.com/",
                    "https://" + tenantName + ".betacfgapi.kcloudy.com/",
                    //字典管理
                    "http://" + tenantName + ".betadic.kcloudy.com/",
                    "http://" + tenantName + ".betadicapi.kcloudy.com/",
                    "https://" + tenantName + ".betadic.kcloudy.com/",
                    "https://" + tenantName + ".betadicapi.kcloudy.com/",
                    //应用管理
                    "http://" + tenantName + ".betaapp.kcloudy.com/",
                    "http://" + tenantName + ".betaappapi.kcloudy.com/",
                    "https://" + tenantName + ".betaapp.kcloudy.com/",
                    "https://" + tenantName + ".betaappapi.kcloudy.com/",
                    //消息管理
                    "http://" + tenantName + ".betamsg.kcloudy.com/",
                    "http://" + tenantName + ".betamsgapi.kcloudy.com/",
                    "https://" + tenantName + ".betamsg.kcloudy.com/",
                    "https://" + tenantName + ".betamsgapi.kcloudy.com/",

                    //账户管理
                    "http://" + tenantName + ".betaacc.kcloudy.com/",
                    "http://" + tenantName + ".betaaccapi.kcloudy.com/",
                    "https://" + tenantName + ".betaacc.kcloudy.com/",
                    "https://" + tenantName + ".betaaccapi.kcloudy.com/",
                    //合同管理
                    "http://" + tenantName + ".betaecon.kcloudy.com/",
                    "https://" + tenantName + ".betaeconapi.kcloudy.com/",
                    "http://" + tenantName + ".betaecon.kcloudy.com/",
                    "https://" + tenantName + ".betaeconapi.kcloudy.com/",
                    //文档管理
                    "http://" + tenantName + ".betadoc.kcloudy.com/",
                    "http://" + tenantName + ".betadocapi.kcloudy.com/",
                    "https://" + tenantName + ".betadoc.kcloudy.com/",
                    "https://" + tenantName + ".betadocapi.kcloudy.com/",
                    //人事管理
                    "http://" + tenantName + ".betahr.kcloudy.com/",
                    "http://" + tenantName + ".betahrapi.kcloudy.com/",
                    "https://" + tenantName + ".betahr.kcloudy.com/",
                    "https://" + tenantName + ".betahrapi.kcloudy.com/",
                    
                    //客户管理
                    "http://" + tenantName + ".betacrm.kcloudy.com/",
                    "http://" + tenantName + ".betacrmapi.kcloudy.com/",
                    "https://" + tenantName + ".betacrm.kcloudy.com/",
                    "https://" + tenantName + ".betacrmapi.kcloudy.com/",
                    //供应商管理
                    "http://" + tenantName + ".betasrm.kcloudy.com/",
                    "http://" + tenantName + ".betasrmapi.kcloudy.com/",
                    "https://" + tenantName + ".betasrm.kcloudy.com/",
                    "https://" + tenantName + ".betasrmapi.kcloudy.com/",
                    //商品管理
                    "http://" + tenantName + ".betaprd.kcloudy.com/",
                    "http://" + tenantName + ".betaprdapi.kcloudy.com/",
                    "https://" + tenantName + ".betaprd.kcloudy.com/",
                    "https://" + tenantName + ".betaprdapi.kcloudy.com/",
                    //物料管理
                    "http://" + tenantName + ".betapmc.kcloudy.com/",
                    "http://" + tenantName + ".betapmcapi.kcloudy.com/",
                    "https://" + tenantName + ".betapmc.kcloudy.com/",
                    "https://" + tenantName + ".betapmcapi.kcloudy.com/",
                    
                    //门户管理
                    "http://" + tenantName + ".beta.kcloudy.com/",//com.web.portal
                    "https://" + tenantName + ".beta.kcloudy.com/",//com.web.portal
                    "http://" + tenantName + ".beta.kcloudy.com/",//com.webapi.portal
                    "https://" + tenantName + ".beta.kcloudy.com/",//com.webapi.portal
                    //培训管理
                    "http://" + tenantName + ".betatrn.kcloudy.com/",//com.web.training
                    "https://" + tenantName + ".betatrn.kcloudy.com/",//com.web.training
                    "http://" + tenantName + ".betatrnapi.kcloudy.com/",//com.webapi.training
                    "https://" + tenantName + ".betatrnapi.kcloudy.com/",//com.webapi.training

                    //流程管理
                    "http://" + tenantName + ".betaflow.kcloudy.com/",//com.web.workflow
                    "https://" + tenantName + ".betaflow.kcloudy.com/",//com.web.workflow
                    "http://" + tenantName + ".betaflowapi.kcloudy.com/",//com.webapi.workflow
                    "https://" + tenantName + ".betaflowapi.kcloudy.com/",//com.webapi.workflow
                    //支付管理
                    "http://" + tenantName + ".betapay.kcloudy.com/",//com.web.payment
                    "https://" + tenantName + ".betapay.kcloudy.com/",//com.web.payment
                    "http://" + tenantName + ".betapayapi.kcloudy.com/",//com.webapi.payment
                    "https://" + tenantName + ".betapayapi.kcloudy.com/",//com.webapi.payment

                    //微信管理
                    "http://" + tenantName + ".betawx.kcloudy.com/",//com.web.wechat
                    "https://" + tenantName + ".betawx.kcloudy.com/",//com.web.wechat
                    "http://" + tenantName + ".betawxapi.kcloudy.com/",//com.webapi.wechat
                    "https://" + tenantName + ".betawxapi.kcloudy.com/",//com.webapi.wechat
                    
                };
                #endregion

                default:
                    #region 正式发布环境
                    return new[] {
                    //--------------正式发布环境--------------
                    //配置管理
                    "http://" + tenantName + ".cfg.kcloudy.com/",
                    "http://" + tenantName + ".cfgapi.kcloudy.com/",
                    "https://" + tenantName + ".cfg.kcloudy.com/",
                    "https://" + tenantName + ".cfgapi.kcloudy.com/",
                    //字典管理
                    "http://" + tenantName + ".dic.kcloudy.com/",
                    "http://" + tenantName + ".dicapi.kcloudy.com/",
                    "https://" + tenantName + ".dic.kcloudy.com/",
                    "https://" + tenantName + ".dicapi.kcloudy.com/",
                    //应用管理
                    "http://" + tenantName + ".app.kcloudy.com/",
                    "http://" + tenantName + ".appapi.kcloudy.com/",
                    "https://" + tenantName + ".app.kcloudy.com/",
                    "https://" + tenantName + ".appapi.kcloudy.com/",
                    //消息管理
                    "http://" + tenantName + ".msg.kcloudy.com/",
                    "http://" + tenantName + ".msgapi.kcloudy.com/",
                    "https://" + tenantName + ".msg.kcloudy.com/",
                    "https://" + tenantName + ".msgapi.kcloudy.com/",

                    //账户管理
                    "http://" + tenantName + ".acc.kcloudy.com/",
                    "http://" + tenantName + ".accapi.kcloudy.com/",
                    "https://" + tenantName + ".acc.kcloudy.com/",
                    "https://" + tenantName + ".accapi.kcloudy.com/",
                    //合同管理
                    "http://" + tenantName + ".econ.kcloudy.com/",
                    "https://" + tenantName + ".econapi.kcloudy.com/",
                    "http://" + tenantName + ".econ.kcloudy.com/",
                    "https://" + tenantName + ".econapi.kcloudy.com/",
                    //文档管理
                    "http://" + tenantName + ".doc.kcloudy.com/",
                    "http://" + tenantName + ".docapi.kcloudy.com/",
                    "https://" + tenantName + ".doc.kcloudy.com/",
                    "https://" + tenantName + ".docapi.kcloudy.com/",
                    //人事管理
                    "http://" + tenantName + ".hr.kcloudy.com/",
                    "http://" + tenantName + ".hrapi.kcloudy.com/",
                    "https://" + tenantName + ".hr.kcloudy.com/",
                    "https://" + tenantName + ".hrapi.kcloudy.com/",
                    
                    //客户管理
                    "http://" + tenantName + ".crm.kcloudy.com/",
                    "http://" + tenantName + ".crmapi.kcloudy.com/",
                    "https://" + tenantName + ".crm.kcloudy.com/",
                    "https://" + tenantName + ".crmapi.kcloudy.com/",
                    //供应商管理
                    "http://" + tenantName + ".srm.kcloudy.com/",
                    "http://" + tenantName + ".srmapi.kcloudy.com/",
                    "https://" + tenantName + ".srm.kcloudy.com/",
                    "https://" + tenantName + ".srmapi.kcloudy.com/",
                    //商品管理
                    "http://" + tenantName + ".prd.kcloudy.com/",
                    "http://" + tenantName + ".prdapi.kcloudy.com/",
                    "https://" + tenantName + ".prd.kcloudy.com/",
                    "https://" + tenantName + ".prdapi.kcloudy.com/",
                    //物料管理
                    "http://" + tenantName + ".pmc.kcloudy.com/",
                    "http://" + tenantName + ".pmcapi.kcloudy.com/",
                    "https://" + tenantName + ".pmc.kcloudy.com/",
                    "https://" + tenantName + ".pmcapi.kcloudy.com/",

                    //门户管理
                    "http://" + tenantName + ".kcloudy.com/",//com.web.portal
                    "https://" + tenantName + ".kcloudy.com/",//com.web.portal
                    "http://" + tenantName + ".kcloudy.com/",//com.webapi.portal
                    "https://" + tenantName + ".kcloudy.com/",//com.webapi.portal
                    //培训管理
                    "http://" + tenantName + ".trn.kcloudy.com/",//com.web.training
                    "https://" + tenantName + ".trn.kcloudy.com/",//com.web.training
                    "http://" + tenantName + ".trnapi.kcloudy.com/",//com.webapi.training
                    "https://" + tenantName + ".trnapi.kcloudy.com/",//com.webapi.training

                    //流程管理
                    "http://" + tenantName + ".flow.kcloudy.com/",//com.web.workflow
                    "https://" + tenantName + ".flow.kcloudy.com/",//com.web.workflow
                    "http://" + tenantName + ".flowapi.kcloudy.com/",//com.webapi.workflow
                    "https://" + tenantName + ".flowapi.kcloudy.com/",//com.webapi.workflow
                    //支付管理
                    "http://" + tenantName + ".pay.kcloudy.com/",//com.web.payment
                    "https://" + tenantName + ".pay.kcloudy.com/",//com.web.payment
                    "http://" + tenantName + ".payapi.kcloudy.com/",//com.webapi.payment
                    "https://" + tenantName + ".payapi.kcloudy.com/",//com.webapi.payment

                    //微信管理
                    "http://" + tenantName + ".wx.kcloudy.com/",//com.web.wechat
                    "https://" + tenantName + ".wx.kcloudy.com/",//com.web.wechat
                    "http://" + tenantName + ".wxapi.kcloudy.com/",//com.webapi.wechat
                    "https://" + tenantName + ".wxapi.kcloudy.com/",//com.webapi.wechat
                };
                    #endregion
            }
        }
        #endregion
    }
}
