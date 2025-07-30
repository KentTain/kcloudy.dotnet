using System.Collections.Generic;
using System.Threading.Tasks;
using KC.Service.DTO.App;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Service.Base;

namespace KC.Service.WebApiService.Business
{
    public class ApplicationApiService : IdSrvOAuth2ClientRequestBase, IApplicationApiService
    {
        private const string _serviceName = "KC.Service.WebApiService.Business.ApplicationApiService";

        public ApplicationApiService(
            System.Net.Http.IHttpClientFactory httpClient,
            Microsoft.Extensions.Logging.ILogger<ApplicationApiService> logger)
            : base(TenantConstant.DbaTenantApiAccessInfo, httpClient, logger)
        {
        }

        /// <summary>
        /// 租户应用接口地址：http://[tenantName].app.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:1105/api/
        /// </summary>
        private string _applicationServiceUrl
        {
            get
            {
                if (string.IsNullOrEmpty(GlobalConfig.AppWebDomain))
                    return null;

                return GlobalConfig.AppWebDomain.Replace(TenantConstant.SubDomain, Tenant.TenantName) + "api/";
            }
        }

        /// <summary>
        /// 获取所有的简单的应用信息
        /// </summary>
        /// <returns>ServiceResult<List<ApplicationInfo>></returns>
        public async Task<ServiceResult<List<ApplicationInfo>>> LoadAllSimpleApplicationsAsync()
        {
            ServiceResult<List<ApplicationInfo>> result = null;
            await WebSendGetAsync<ServiceResult<List<ApplicationInfo>>>(
                _serviceName + ".LoadAllSimpleApplications",
                _applicationServiceUrl + "LoadAllSimpleApplications",
                ApplicationConstant.AppScope,
                DefaultContentType,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<ApplicationInfo>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                false);

            return result;
        }

        /// <summary>
        /// 获取所有的应用信息
        /// </summary>
        /// <returns>ServiceResult<List<ApplicationDTO>></returns>
        public async Task<ServiceResult<List<ApplicationDTO>>> LoadAllApplicationsAsync()
        {
            ServiceResult<List<ApplicationDTO>> result = null;
            await WebSendGetAsync<ServiceResult<List<ApplicationDTO>>>(
                _serviceName + ".LoadAllApplications",
                _applicationServiceUrl + "LoadAllApplications",
                ApplicationConstant.AppScope,
                DefaultContentType, 
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<ApplicationDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            return result;
        }

    }
}
