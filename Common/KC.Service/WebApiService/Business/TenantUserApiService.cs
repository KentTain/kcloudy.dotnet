using System;
using System.Collections.Generic;
using KC.Framework.Tenant;
using System.Threading.Tasks;
using System.Net.Http;

namespace KC.Service.WebApiService.Business
{
    public interface ITenantUserApiService
    {
        Task<ServiceResult<bool>> ExistTenantName(string tenantName);

        /// <summary>
        /// 根据租户代码或是租户昵称获取租户信息
        /// </summary>
        /// <param name="tenantName">租户代码或是租户昵称</param>
        /// <returns>租户信息</returns>
        [Extension.CachingCallHandler()]
        Task<Tenant> GetTenantByName(string tenantName);

        /// <summary>
        /// 根据租户独立域名获取租户信息
        /// </summary>
        /// <param name="domainName">租户独立域名</param>
        /// <returns>租户信息</returns>
        [Extension.CachingCallHandler()]
        Task<Tenant> GetTenantEndWithDomainName(string domainName);

    }

    /// <summary>
    /// 注意：TenantUserApiService，默认使用的Dba的身份去访问接口，故基类中的Tenant永远为Dba
    /// </summary>
    public class TenantUserApiService : IdSrvOAuth2ClientRequestBase, ITenantUserApiService
    {
        private const string _serviceName = "KC.Service.WebApiService.Business.TenantUserApiService";

        public TenantUserApiService(
            IHttpClientFactory httpClient,
            Microsoft.Extensions.Logging.ILogger<TenantUserApiService> logger)
            : base(TenantConstant.DbaTenantApiAccessInfo, httpClient, logger)
        {
        }

        /// <summary>
        /// 租户名称是否存在
        /// </summary>
        /// <param name="tenantName">租户代码</param>
        /// <returns>租户信息</returns>
        public async Task<ServiceResult<bool>> ExistTenantName(string tenantName)
        {
            ServiceResult<bool> result = null;
            await WebSendGetAsync<ServiceResult<bool>>(
                _serviceName + ".ExistTenantName",
                AdminApiUrl + "TenantApi/ExistTenantName?tenantName=" + tenantName,
                ApplicationConstant.AdminScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<bool>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                false);

            return result;
        }
        /// <summary>
        /// 根据租户代码或是租户昵称获取租户信息
        /// </summary>
        /// <param name="tenantName">租户代码或是租户昵称</param>
        /// <returns>租户信息</returns>
        public async Task<Tenant> GetTenantByName(string tenantName)
        {
            //使用Redis缓存
            //var cacheKey = "WebApiService.TenantUserApiService-GetTenantByName-" + tenantName;
            //var tenant = CacheUtil.GetCache<Tenant>(cacheKey);
            //if (tenant != null) 
            //    return tenant;

            ServiceResult<Tenant> result = null;
            await WebSendGetAsync<ServiceResult<Tenant>>(
                _serviceName + ".GetTenantUserByName",
                AdminApiUrl + "TenantApi/GetTenantByTenantName?tenantName=" + tenantName,
                ApplicationConstant.AdminScope, 
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<Tenant>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                //使用Redis缓存
                //CacheUtil.SetCache(cacheKey, result.Result, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));
                return result.Result;
            }
            return null;
        }

        /// <summary>
        /// 根据租户独立域名获取租户信息
        /// </summary>
        /// <param name="domainName">租户独立域名</param>
        /// <returns>租户信息</returns>
        public async Task<Tenant> GetTenantEndWithDomainName(string domainName)
        {
            //使用IIS缓存
            //var cacheKey = "WebApiService.TenantUserApiService-GetTenantEndWithDomainName-" + domainName;
            //var tenant = CacheUtil.GetCache<Tenant>(cacheKey);
            //if (tenant != null) 
            //    return tenant;

            ServiceResult<Tenant> result = null;
            await WebSendGetAsync<ServiceResult<Tenant>>(
                _serviceName + ".GetTenantEndWithDomainName",
                AdminApiUrl + "TenantApi/GetTenantEndWithDomainName?domainName=" + domainName,
                ApplicationConstant.AdminScope, 
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<Tenant>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);
            if (result.success && result.Result != null)
            {
                //使用Redis缓存
                //CacheUtil.SetCache(cacheKey, result.Result, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));
                return result.Result;
            }

            return null;
        }


    }
}
