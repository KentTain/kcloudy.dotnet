using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Service.WebApiService;
using Microsoft.Extensions.Logging;

namespace KC.Service.EFService
{
    public interface IEFService
    {
        Tenant Tenant { get; set; }
    }

    public abstract class EFServiceBase : IEFService
    {
        private const string OAuth2_Authorize_Action = "connect/authorize";
        private const string OAuth2_Token_Action = "connect/token";
        private const string OAuth2_UserInfo_Action = "connect/userinfo";
        private const string OAuth2_JwkSet_Action = ".well-known/openid-configuration/jwks";

        protected readonly IHttpClientFactory HttpClientFactory;
        protected readonly ILogger Logger;

        protected EFServiceBase(
            IHttpClientFactory clientFactory,
            ILogger logger)
        {
            HttpClientFactory = clientFactory;
            Logger = logger;
        }

        //protected EFServiceBase(
        //    Tenant tenant, 
        //    IHttpClientFactory clientFactory)
        //{
        //    Tenant = tenant;
        //    httpClientFactory = clientFactory;
        //}

        protected EFServiceBase(
            Tenant tenant,
            IHttpClientFactory clientFactory,
            ILogger logger)
        {
            Tenant = tenant;

            HttpClientFactory = clientFactory;
            Logger = logger;
        }

        public Tenant Tenant { get; set; }

        /// <summary>
        /// 获取OAuth2身份认证服务器相关信息
        /// </summary>
        /// <returns></returns>
        protected OAuth2ClientInfo GetTenantOAuth2ClientInfo()
        {
            string tenantName = Tenant.TenantName;
            string clientId = TenantConstant.GetClientIdByTenant(Tenant);
            string clientSecret = TenantConstant.GetClientSecretByTenant(Tenant);
            string credential = TenantConstant.Sha256(clientSecret);

            string tokenEndpoint = GlobalConfig.SSOWebDomain + OAuth2_Token_Action;
            //string tokenEndpoint = SSOServerUrl() + OpenIdConnectConstants.OAuth2_Token_Action;
            string grantType = "client_credentials";

            return new OAuth2ClientInfo(tenantName, clientId, clientSecret, tokenEndpoint, grantType);
        }

        /// <summary>
        /// 将subdomain（如：subdomain.acc.kcloudy.com）的域名替换为tenantName的Web域名 </br>
        ///     例如：[tenantName].acc.kcloudy.com
        /// 备注：WebApi方法上必须添加特性[Com.WebApi.Core.Attributes.WebApiAllowOtherTenant]，以便其他租户可以访问该接口
        /// </summary>
        /// <param name="subdomain">Web的配置地址：subdomain.acc.kcloudy.com</param>
        /// <param name="tenantName">接口所有者的TenantName</param>
        /// <returns></returns>
        protected string GetTenantWebDomain(string subdomain)
        {
            string tenantName = Tenant.TenantName;
            return GlobalConfig.GetTenantWebDomain(subdomain, tenantName);
        }

        /// <summary>
        /// 将subdomain（如：subdomain.acc.kcloudy.com）的域名替换为tenantName的WebApi域名 </br>
        ///     例如：[tenantName].accapi.kcloudy.com
        /// 备注：WebApi方法上必须添加特性[Com.WebApi.Core.Attributes.WebApiAllowOtherTenant]，以便其他租户可以访问该接口
        /// </summary>
        /// <param name="subdomain">Web的配置地址：subdomain.acc.kcloudy.com</param>
        /// <param name="tenantName">接口所有者的TenantName</param>
        /// <returns></returns>
        protected string GetTenantWebApiDomain(string subdomain)
        {
            string tenantName = Tenant.TenantName;
            return GlobalConfig.GetTenantWebApiDomain(subdomain, tenantName);
        }
    }

    public abstract class WebApiServiceBase : IEFService
    {
        protected readonly IHttpClientFactory httpClientFactory;
        protected readonly ILogger Logger;

        protected WebApiServiceBase(
            Tenant tenant)
        {
            Tenant = tenant ?? TenantConstant.DbaTenantApiAccessInfo;
        }

        protected WebApiServiceBase(
            Tenant tenant,
            IHttpClientFactory clientFactory,
            ILogger logger)
        {
            Tenant = tenant;
            httpClientFactory = clientFactory;
            Logger = logger;
        }

        public Tenant Tenant { get; set; }
    }
}
