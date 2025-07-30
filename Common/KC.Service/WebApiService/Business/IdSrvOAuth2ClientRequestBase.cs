using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using KC.Framework.Base;
using KC.Framework.Tenant;

namespace KC.Service.WebApiService.Business
{
    public abstract class IdSrvOAuth2ClientRequestBase : OAuth2ClientRequestBase, EFService.IEFService
    {
        private const string OAuth2_Authorize_Action = "connect/authorize";
        private const string OAuth2_Token_Action = "connect/token";
        private const string OAuth2_UserInfo_Action = "connect/userinfo";
        private const string OAuth2_JwkSet_Action = ".well-known/openid-configuration/jwks";

        public Tenant Tenant { get; set; }

        #region Api Url

        /// <summary>
        /// 获取租户信息接口地址：http://adminapi.kcloudy.com/
        ///     本地测试接口地址：http://localhost:1004/api/
        /// </summary>
        protected string AdminApiUrl
        {
            get
            {
                if (string.IsNullOrEmpty(GlobalConfig.AdminWebDomain))
                    return null;

                return GlobalConfig.AdminWebDomain.Replace("admin.", "adminapi.")
                        .Replace(":1003", ":1004").Replace(":1013", ":1014") + "api/";
            }
        }

        /// <summary>
        /// 获取租户信息接口地址：http://blogapi.kcloudy.com/
        ///     本地测试接口地址：http://localhost:1006/api/
        /// </summary>
        protected string BlogApiUrl
        {
            get
            {
                if (string.IsNullOrEmpty(GlobalConfig.BlogWebDomain))
                    return null;

                return GlobalConfig.BlogWebDomain.Replace("blog.", "blogapi.")
                        .Replace(":1005", ":1006").Replace(":1015", ":1016") + "api/";
            }
        }


        /// <summary>
        /// SSO登录接口：http://ssoapi.kcloudy.com/
        ///     本地测试接口地址：http://localhost:1002/api/
        /// </summary>
        /// <param name="tenantName"></param>
        /// <returns></returns>
        protected string GetSsoApiUrl(string tenantName)
        {
            if (string.IsNullOrEmpty(GlobalConfig.SSOWebDomain))
                return null;

            return GlobalConfig.SSOWebDomain.Replace(TenantConstant.SubDomain, tenantName)
                .Replace("sso.", "ssoapi.").Replace(":1001", ":1002").Replace(":1011", ":1012") + "api/";
        }
        /// <summary>
        /// 租户SSO接口地址：http://cTest.ssoapi.kcloudy.com/api/
        ///     本地测试接口地址：http://cTest.localhost:1009/api/
        /// </summary>
        protected string SsoApiServerUrl
        {
            get
            {
                return GetSsoApiUrl(Tenant.TenantName);
            }
        }
        /// <summary>
        /// 租户subdomin的SSO接口地址：http://subdomin.ssoapi.kcloudy.com/api/
        ///     本地测试接口地址：http://subdomin.localhost:1002/api/
        /// </summary>
        protected string SsoSubdomainApiServerUrl
        {
            get
            {
                return GetSsoApiUrl(TenantConstant.SubDomain);
            }
        }

        /// <summary>
        /// 获取配置信息接口地址：http://[tenantName].cfgapi.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:1102/api/
        /// </summary>
        /// <param name="tenantName">租户代码</param>
        /// <returns></returns>
        protected string GetConfigApiUrl(string tenantName)
        {
            if (string.IsNullOrEmpty(GlobalConfig.CfgWebDomain))
                return null;

            return GlobalConfig.CfgWebDomain.Replace(TenantConstant.SubDomain, tenantName)
                .Replace("cfg.", "cfgapi.").Replace(":1101", ":1102").Replace(":1111", ":1112") + "api/";
        }

        /// <summary>
        /// 获取配置信息接口地址：http://cdba.cfgapi.kcloudy.com/api/
        ///     本地测试接口地址：http://localhost:1102/api/
        /// </summary>
        protected string ConfigApiUrl
        {
            get
            {
                return GetConfigApiUrl(Tenant.TenantName);
            }
        }

        /// <summary>
        /// 获取字典信息接口地址：http://[tenantName].dicapi.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:1104/api/
        /// </summary>
        /// <param name="tenantName">租户代码</param>
        /// <returns></returns>
        protected string GetDictionaryApiUrl(string tenantName)
        {
            if (string.IsNullOrEmpty(GlobalConfig.DicWebDomain))
                return null;

            return GlobalConfig.DicWebDomain.Replace(TenantConstant.SubDomain, tenantName)
                .Replace("dic.", "dicapi.").Replace(":1103", ":1104").Replace(":1113", ":1114") + "api/";
        }

        /// <summary>
        /// 获取字典信息接口地址：http://[tenantName].dicapi.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:1104/api/
        /// </summary>
        protected string DictionaryApiUrl
        {
            get
            {
                return GetDictionaryApiUrl(Tenant.TenantName);
            }
        }


        /// <summary>
        /// 获取应用信息接口地址：http://[tenantName].appapi.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:1106/api/
        /// </summary>
        /// <param name="tenantName">租户代码</param>
        /// <returns></returns>
        protected string GetApplicationApiUrl(string tenantName)
        {
            if (string.IsNullOrEmpty(GlobalConfig.AppWebDomain))
                return null;

            return GlobalConfig.AppWebDomain.Replace(TenantConstant.SubDomain, tenantName)
                .Replace("app.", "appapi.").Replace(":1105", ":1106").Replace(":1115", ":1116") + "api/";
        }

        /// <summary>
        /// 租户信息Api接口地址：http://cdba.appapi.kcloudy.com/api/
        ///     本地测试接口地址：http://localhost:1106/api/
        /// </summary>
        protected string ApplicationApiUrl
        {
            get
            {
                return GetApplicationApiUrl(Tenant.TenantName);
            }
        }

        /// <summary>
        /// 获取文档接口地址：http://[tenantName].msgapi.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:2000/api/
        /// </summary>
        /// <param name="tenantName">租户代码</param>
        /// <returns></returns>
        protected string GetMessageApiUrl(string tenantName)
        {
            if (string.IsNullOrEmpty(GlobalConfig.MsgWebDomain))
                return null;

            return GlobalConfig.MsgWebDomain.Replace(TenantConstant.SubDomain, tenantName)
                .Replace("msg.", "msgapi.").Replace(":1107", ":1108").Replace(":1117", ":1118") + "api/";
        }

        /// <summary>
        /// 获取文档接口地址：http://[tenantName].msgapi.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:2000/api/
        /// </summary>
        /// <param name="tenantName">租户代码</param>
        /// <returns></returns>
        protected string MessageApiUrl
        {
            get
            {
                return GetMessageApiUrl(Tenant.TenantName);
            }
        }

        /// <summary>
        /// 获取账户信息接口地址：http://[tenantName].accapi.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:2002/api/
        /// </summary>
        /// <param name="tenantName">租户代码</param>
        /// <returns></returns>
        protected string GetAccountApiUrl(string tenantName)
        {
            if (string.IsNullOrEmpty(GlobalConfig.AccWebDomain))
                return null;

            return GlobalConfig.AccWebDomain.Replace(TenantConstant.SubDomain, tenantName)
                .Replace("acc.", "accapi.").Replace(":2001", ":2002").Replace(":2011", ":2012") + "api/";
        }

        /// <summary>
        /// 租户用户接口地址：http://cTest.accapi.kcloudy.com/api/
        ///     本地测试接口地址：http://localhost:2002/api/
        /// </summary>
        protected string AccoutApiServerUrl
        {
            get
            {
                return GetAccountApiUrl(Tenant.TenantName);
            }
        }

        /// <summary>
        /// 获取电子合同接口地址：http://[tenantName].econapi.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:2004/api/
        /// </summary>
        /// <param name="tenantName">租户代码</param>
        /// <returns></returns>
        protected string GetEContractApiUrl(string tenantName)
        {
            if (string.IsNullOrEmpty(GlobalConfig.EconWebDomain))
                return null;

            return GlobalConfig.EconWebDomain.Replace(TenantConstant.SubDomain, tenantName)
                .Replace("econ.", "econapi.").Replace(":2003", ":2004").Replace(":2013", ":2014") + "api/";
        }
        /// <summary>
        /// 获取文档接口地址：http://[tenantName].docapi.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:2006/api/
        /// </summary>
        /// <param name="tenantName">租户代码</param>
        /// <returns></returns>
        protected string GetDocumentApiUrl(string tenantName)
        {
            if (string.IsNullOrEmpty(GlobalConfig.DocWebDomain))
                return null;

            return GlobalConfig.DocWebDomain.Replace(TenantConstant.SubDomain, tenantName)
                .Replace("doc.", "docapi.").Replace(":2005", ":2006").Replace(":2015", ":2016") + "api/";
        }
        /// <summary>
        /// 获取人事接口地址：http://[tenantName].hrapi.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:2008/api/
        /// </summary>
        /// <param name="tenantName">租户代码</param>
        /// <returns></returns>
        protected string GetHumanResourceApiUrl(string tenantName)
        {
            if (string.IsNullOrEmpty(GlobalConfig.HrWebDomain))
                return null;

            return GlobalConfig.HrWebDomain.Replace(TenantConstant.SubDomain, tenantName)
                .Replace("hr.", "hrapi.").Replace(":2007", ":2008").Replace(":2017", ":2018") + "api/";
        }

        /// <summary>
        /// 获取客户信息接口地址：http://[tenantName].crmapi.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:3002/api/
        /// </summary>
        /// <param name="tenantName">租户代码</param>
        /// <returns></returns>
        protected string GetCrmApiUrl(string tenantName)
        {
            if (string.IsNullOrEmpty(GlobalConfig.CrmWebDomain))
                return null;

            return GlobalConfig.CrmWebDomain.Replace(TenantConstant.SubDomain, tenantName)
                .Replace("crm.", "crmapi.").Replace(":3001", ":3002").Replace(":3011", ":3012") + "api/";
        }

        /// <summary>
        /// 租户CRM接口地址：http://cTest.crmapi.kcloudy.com/api/
        ///     本地测试接口地址：http://cTest.localhost:3002/api/
        /// </summary>
        protected string CrmApiServerUrl
        {
            get
            {
                return GetCrmApiUrl(Tenant.TenantName);
            }
        }

        /// <summary>
        /// 获取供应商信息接口地址：http://[tenantName].srmapi.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:3004/api/
        /// </summary>
        /// <param name="tenantName">租户代码</param>
        /// <returns></returns>
        protected string GetSrmApiUrl(string tenantName)
        {
            if (string.IsNullOrEmpty(GlobalConfig.SrmWebDomain))
                return null;

            return GlobalConfig.SrmWebDomain.Replace(TenantConstant.SubDomain, tenantName)
                .Replace("srm.", "srmapi.").Replace(":3003", ":3004").Replace(":3013", ":3014") + "api/";
        }
        /// <summary>
        /// 获取产品信息接口地址：http://[tenantName].prdapi.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:3006/api/
        /// </summary>
        /// <param name="tenantName">租户代码</param>
        /// <returns></returns>
        protected string GetPrdApiUrl(string tenantName)
        {
            if (string.IsNullOrEmpty(GlobalConfig.PrdWebDomain))
                return null;

            return GlobalConfig.PrdWebDomain.Replace(TenantConstant.SubDomain, tenantName)
                .Replace("prd.", "prdapi.").Replace(":3005", ":3006").Replace(":3015", ":3016") + "api/";
        }
        /// <summary>
        /// 获取物料信息接口地址：http://[tenantName].Pmcapi.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:3008/api/
        /// </summary>
        /// <param name="tenantName">租户代码</param>
        /// <returns></returns>
        protected string GetPmcApiUrl(string tenantName)
        {
            if (string.IsNullOrEmpty(GlobalConfig.PmcWebDomain))
                return null;

            return GlobalConfig.PmcWebDomain.Replace(TenantConstant.SubDomain, tenantName)
                .Replace("pmc.", "pmcapi.").Replace(":3007", ":3008").Replace(":3017", ":3018") + "api/";
        }

        /// <summary>
        /// 获取门户站点接口地址：http://[tenantName].portalapi.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:4002/api/
        /// </summary>
        /// <param name="tenantName">租户代码</param>
        /// <returns></returns>
        protected string GetPortalApiUrl(string tenantName)
        {
            if (string.IsNullOrEmpty(GlobalConfig.PortalWebDomain))
                return null;

            return GlobalConfig.PortalWebDomain.Replace(TenantConstant.SubDomain, tenantName)
                .Replace("portal.", "portalapi.").Replace(":4001", ":4002").Replace(":4011", ":4012") + "api/";
        }

        /// <summary>
        /// 租户电商Subdomin接口地址：http://subdomin.portalapi.kcloudy.com/api/
        ///     本地测试接口地址：http://subdomin.localhost:4002/api/
        /// </summary>
        protected string PortalSubdomainApiServerUrl
        {
            get
            {
                return GetPortalApiUrl(TenantConstant.SubDomain);
            }
        }

        /// <summary>
        /// 租户电商接口地址：http://cTest.shopapi.kcloudy.com/api/
        ///     本地测试接口地址：http://cTest.localhost:4001/api/
        /// </summary>
        protected string PortalApiServerUrl
        {
            get
            {
                return GetPortalApiUrl(Tenant.TenantName);
            }
        }
        /// <summary>
        /// 获取采购订单信息接口地址：http://[tenantName].somapi.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:4004/api/
        /// </summary>
        /// <param name="tenantName">租户代码</param>
        /// <returns></returns>
        protected string GetSomApiUrl(string tenantName)
        {
            if (string.IsNullOrEmpty(GlobalConfig.SomWebDomain))
                return null;

            return GlobalConfig.SomWebDomain.Replace(TenantConstant.SubDomain, tenantName)
                .Replace("som.", "somapi.").Replace(":4003", ":4004").Replace(":4013", ":4014") + "api/";
        }

        /// <summary>
        /// 获取销售订单信息接口地址：http://[tenantName].pomapi.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:4006/api/
        /// </summary>
        /// <param name="tenantName">租户代码</param>
        /// <returns></returns>
        protected string GetPomApiUrl(string tenantName)
        {
            if (string.IsNullOrEmpty(GlobalConfig.PomWebDomain))
                return null;

            return GlobalConfig.PomWebDomain.Replace(TenantConstant.SubDomain, tenantName)
                .Replace("pom.", "pomapi.").Replace(":4005", ":4006").Replace(":4015", ":4016") + "api/";
        }

        /// <summary>
        /// 获取物料信息接口地址：http://[tenantName].wmsapi.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:4008/api/
        /// </summary>
        /// <param name="tenantName">租户代码</param>
        /// <returns></returns>
        protected string GetWmsApiUrl(string tenantName)
        {
            if (string.IsNullOrEmpty(GlobalConfig.WmsWebDomain))
                return null;

            return GlobalConfig.WmsWebDomain.Replace(TenantConstant.SubDomain, tenantName)
                .Replace("wms.", "wmsapi.").Replace(":4007", ":4008").Replace(":4017", ":4018") + "api/";
        }

        /// <summary>
        /// 获取登录用户接口地址：http://[tenantName].jrapi.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:5002/api/
        /// </summary>
        /// <param name="tenantName">租户代码</param>
        /// <returns></returns>
        protected string GetMarketApiUrl(string tenantName)
        {
            if (string.IsNullOrEmpty(GlobalConfig.JRWebDomain))
                return null;

            return GlobalConfig.JRWebDomain.Replace(TenantConstant.SubDomain, tenantName)
                .Replace("jr.", "jrapi.").Replace(":5001", ":5002").Replace(":5011", ":5012") + "api/";
        }
        /// <summary>
        /// 租户融资接口地址：http://cTest.marketapi.kcloudy.com/api/
        ///     本地测试接口地址：http://localhost:6002/api/
        /// </summary>
        protected string MarketApiServerUrl
        {
            get
            {
                return GetMarketApiUrl(Tenant.TenantName);
            }
        }

        /// <summary>
        /// 获取登录培训管理接口地址：http://[tenantName].trnapi.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:6002/api/
        /// </summary>
        /// <param name="tenantName">租户代码</param>
        /// <returns></returns>
        protected string GetTrainApiUrl(string tenantName)
        {
            if (string.IsNullOrEmpty(GlobalConfig.TrainWebDomain))
                return null;

            return GlobalConfig.TrainWebDomain.Replace(TenantConstant.SubDomain, tenantName)
                .Replace("trn.", "trnapi.").Replace(":6001", ":6002").Replace(":6011", ":6012") + "api/";
        }
        /// <summary>
        /// 获取登录用户接口地址：http://[tenantName].flowapi.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:7002/api/
        /// </summary>
        /// <param name="tenantName">租户代码</param>
        /// <returns></returns>
        protected string GetWorkflowApiUrl(string tenantName)
        {
            if (string.IsNullOrEmpty(GlobalConfig.WorkflowWebDomain))
                return null;

            return GlobalConfig.WorkflowWebDomain.Replace(TenantConstant.SubDomain, tenantName)
                .Replace("flow.", "flowapi.").Replace(":7001", ":7002").Replace(":7011", ":7012") + "api/";
        }
        /// <summary>
        /// 租户WorkFlow接口地址：http://cTest.flowapi.kcloudy.com/api/
        ///     本地测试接口地址：http://cTest.localhost:7002/api/
        /// </summary>
        protected string WorkflowApiServerUrl
        {
            get
            {
                return GetWorkflowApiUrl(Tenant.TenantName);
            }
        }

        /// <summary>
        /// 获取登录用户接口地址：http://[tenantName].payapi.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:8002/api/
        /// </summary>
        /// <param name="tenantName">租户代码</param>
        /// <returns></returns>
        protected string GetPaymentApiUrl(string tenantName)
        {
            if (string.IsNullOrEmpty(GlobalConfig.PayWebDomain))
                return null;

            return GlobalConfig.PayWebDomain.Replace(TenantConstant.SubDomain, tenantName)
                .Replace("pay.", "payapi.").Replace(":8001", ":8002").Replace(":8011", ":8012") + "api/";
        }
        /// <summary>
        /// 租户Payment接口地址：http://cTest.Paymentapi.kcloudy.com/api/
        ///     本地测试接口地址：http://cTest.localhost:8002/api/
        /// </summary>
        protected string PaymentApiServerUrl
        {
            get
            {
                return GetPaymentApiUrl(Tenant.TenantName);
            }
        }

        /// <summary>
        /// 获取登录用户接口地址：http://[tenantName].wxapi.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:9002/api/
        /// </summary>
        /// <param name="tenantName">租户代码</param>
        /// <returns></returns>
        protected string GetWeixinApiUrl(string tenantName)
        {
            if (string.IsNullOrEmpty(GlobalConfig.WXWebDomain))
                return null;

            return GlobalConfig.WXWebDomain.Replace(TenantConstant.SubDomain, tenantName)
                .Replace("wx.", "wxapi.").Replace(":9001", ":9002").Replace(":9011", ":9012") + "api/";
        }

        /// <summary>
        /// 租户Weixin接口地址：http://cTest.wxapi.kcloudy.com/api/
        ///     本地测试接口地址：http://cTest.localhost:9002/api/
        /// </summary>
        protected string WeixinApiServerUrl
        {
            get
            {
                return GetWeixinApiUrl(Tenant.TenantName);
            }
        }
        #endregion

        /// <summary>
        /// 将subdomain（如：subdomain.shopapi.kcloudy.com）的域名替换为tenantName的域名（如：[tenantName].shopapi.kcloudy.com）
        /// 备注：WebApi方法上必须添加特性[Com.WebApi.Core.Attributes.WebApiAllowOtherTenant]，以便其他租户可以访问该接口
        /// </summary>
        /// <param name="subdomain">WebApi的配置地址：subdomain.api.kcloudy.com</param>
        /// <param name="tenantName">接口所有者的TenantName</param>
        /// <returns></returns>
        protected string GetTenantDomain(string subdomain, string tenantName)
        {
            return subdomain.Replace(TenantConstant.SubDomain, tenantName);
        }


        protected IdSrvOAuth2ClientRequestBase(
            Tenant tenant,
            IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger logger)
            : base(clientFactory, logger)
        {
            Tenant = tenant;
        }

        protected override OAuth2ClientInfo GetOAuth2ClientInfo()
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
    }
}
