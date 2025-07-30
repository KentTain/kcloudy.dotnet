using System.Collections.Generic;
using KC.Common;
using KC.Framework.Tenant;
using KC.Service;
using KC.Service.WebApiService;
using KC.Service.WebApiService.Business;
using KC.Service.DTO.Customer;

namespace KC.Service.Customer.WebApiService.Business
{
    public interface ICustomerApiService
    {
        ServiceResult<bool> ExistTenantService();
        ServiceResult<bool> SendCustomersToTenant(List<CustomerInfoDTO> data);
    }

    public class CustomerApiService : IdSrvOAuth2ClientRequestBase, ICustomerApiService
    {
        private const string ServiceName = "KC.Service.Customer.WebApiService.CustomerApiService";

        public CustomerApiService(
            Tenant tenant,
            System.Net.Http.IHttpClientFactory httpClient,
            Microsoft.Extensions.Logging.ILogger<CustomerApiService> logger)
            : base(tenant ?? TenantConstant.DbaTenantApiAccessInfo, httpClient, logger)
        {
        }

        public ServiceResult<bool> SendCustomersToTenant(List<CustomerInfoDTO> data)
        {
            var postData = SerializeHelper.ToJson(data);
            ServiceResult<bool> result = null;
            WebSendPost<ServiceResult<bool>>(
                ServiceName + ".SendCustomersToTenant",
                CrmApiServerUrl + "CustomerApi/ImportCustomersFromOtherTenant",
                ApplicationConstant.CrmScope,
                postData,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<bool>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            return result;
        }

        public ServiceResult<bool> ExistTenantService()
        {
            ServiceResult<bool> result = null;
            WebSendGet<ServiceResult<bool>>(
                ServiceName + ".ExistTenantService",
                CrmApiServerUrl + "CustomerApi/ExistTenantService",
                ApplicationConstant.CrmScope,
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
    }
}
