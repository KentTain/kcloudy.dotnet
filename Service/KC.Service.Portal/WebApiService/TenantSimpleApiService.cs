using System;
using System.Collections.Generic;
using KC.Common;
using KC.Framework.Tenant;
using KC.Service.Util;
using KC.Service.Constants;
using KC.Service.DTO.Admin;
using KC.Service.DTO;
using System.Reflection;
using System.Threading.Tasks;
using System.Net.Http;
using KC.Service.WebApiService.Business;
using KC.Service.DTO.Portal;

namespace KC.Service.Portal.WebApiService
{
    public interface ITenantSimpleApiService
    {
        Task<List<TenantSimpleDTO>> GetTenantByTenantNames(List<string> tenantNames);

        Task<PaginatedBaseDTO<TenantSimpleDTO>> GetTenantUsersByOpenAppId(int pageIndex, int pageSize, Guid applicationId,
            string tenantDisplayName, string exceptTenant);
        /// <summary>
        /// 根据租户代码TenantName，更新租户的企业名称、联系人、联系电话、联系邮箱、经营范围、经营类型
        /// </summary>
        /// <param name="tenant">租户基本信息</param>
        /// <returns></returns>
        Task<bool> UpdateTenantUserBasicInfo(TenantSimpleDTO tenant);
        /// <summary>
        /// 根据租户代码CompanyCode（即租户代码TenantName），添加或是保存租户认证数据
        /// </summary>
        /// <param name="tenant">租户代码认证数据</param>
        /// <returns></returns>
        Task<bool> SaveTenantUserAuthInfo(CompanyAuthenticationDTO tenantAuth);
    }

    /// <summary>
    /// 注意：TenantUserApiService，默认使用的Dba的身份去访问接口，故基类中的Tenant永远为Dba
    /// </summary>
    public class TenantSimpleApiService : IdSrvOAuth2ClientRequestBase, ITenantSimpleApiService
    {
        private const string ServiceName = "KC.Service.Admin.WebApiService.TenantSimpleApiService";

        public TenantSimpleApiService(
            IHttpClientFactory httpClient,
            Microsoft.Extensions.Logging.ILogger<TenantSimpleApiService> logger)
            : base(TenantConstant.DbaTenantApiAccessInfo, httpClient, logger)
        {
        }


        public async Task<List<TenantSimpleDTO>> GetTenantByTenantNames(List<string> tenantNames)
        {
            var jsonData = SerializeHelper.ToJson(tenantNames);
            ServiceResult<List<TenantSimpleDTO>> result = null;
            await WebSendPostAsync<ServiceResult<List<TenantSimpleDTO>>>(
                ServiceName + ".GetTenantByTenantNames",
                AdminApiUrl + "TenantApi/GetTenantByTenantNames",
                ApplicationConstant.AdminScope,
                jsonData,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<TenantSimpleDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                return result.Result;
            }

            return null;
        }

        public async Task<PaginatedBaseDTO<TenantSimpleDTO>> GetTenantUsersByOpenAppId(int pageIndex, int pageSize, Guid applicationId,
            string tenantDisplayName, string exceptTenant)
        {
            ServiceResult<PaginatedBaseDTO<TenantSimpleDTO>> result = null;
            await WebSendGetAsync<ServiceResult<PaginatedBaseDTO<TenantSimpleDTO>>>(
                ServiceName + ".GetTenantUsersByOpenAppId",
                AdminApiUrl + "TenantApi/GetTenantUsersByOpenAppId?pageIndex=" + pageIndex + "&pageSize=" +
                    pageSize + "&applicationId=" + applicationId + "&tenantDisplayName=" + tenantDisplayName +
                    "&exceptTenant=" + exceptTenant,
                ApplicationConstant.AdminScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<PaginatedBaseDTO<TenantSimpleDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
                return result.Result;

            return null;
        }
        
        /// <summary>
        /// 根据租户代码TenantName，更新租户的企业名称、联系人、联系电话、联系邮箱、经营范围、经营类型
        /// </summary>
        /// <param name="tenant">租户基本信息</param>
        /// <returns></returns>
        public async Task<bool> UpdateTenantUserBasicInfo(TenantSimpleDTO tenant)
        {
            var jsonData = SerializeHelper.ToJson(tenant);
            ServiceResult<bool> result = null;
            await WebSendPostAsync<ServiceResult<bool>>(
                ServiceName + ".UpdateTenantUserBasicInfo",
                AdminApiUrl + "TenantApi/UpdateTenantUserBasicInfo",
                ApplicationConstant.AdminScope,
                jsonData,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<bool>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success)
            {
                return result.Result;
            }

            return false;
        }

        /// <summary>
        /// 根据租户代码CompanyCode（即租户代码TenantName），添加或是保存租户认证数据
        /// </summary>
        /// <param name="tenant">租户代码认证数据</param>
        /// <returns></returns>
        public async Task<bool> SaveTenantUserAuthInfo(CompanyAuthenticationDTO tenantAuth)
        {
            var jsonData = SerializeHelper.ToJson(tenantAuth);
            ServiceResult<bool> result = null;
            await WebSendPostAsync<ServiceResult<bool>>(
                ServiceName + ".SaveTenantUserAuthInfo",
                AdminApiUrl + "TenantApi/SaveTenantUserAuthInfo",
                ApplicationConstant.AdminScope,
                jsonData,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<bool>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success)
            {
                return result.Result;
            }

            return false;
        }
    }
}
