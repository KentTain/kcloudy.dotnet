using KC.Common;
using KC.Service.DTO.Contract;
using KC.Service.DTO;
using KC.Framework.Tenant;
using KC.Service.Contract.WebApiService.ThridParty;
using KC.Service;
using KC.Service.WebApiService.Business;
using System;

namespace KC.Service.Contract.WebApiService.Business
{
    public interface IContractApiService
    {
        ServiceResult<bool> UpdateContractGroupTo(ContractGroupDTO contractGroup, string customerCode);
        ServiceResult<bool> EditContractStatuTo(ContractGroupDTO contractGroup, string customerCode);
        ServiceResult<bool> DelContractGroupTo(Guid id, string customerCode);
        ServiceResult<PaginatedBaseDTO<SearchUser>> SearchEnterpriseEmployee(int page, int size, string tName, string search);
        ServiceResult<bool> SendContractMessage(string customerCode, Guid id, string email, string type = "Contract_Comfirm", string remark = "");
        ServiceResult<bool> IsSeal(string personalId, string tenantName);
        ServiceResult<bool> GetCode(string userId, string tenantName, bool? isPersonal);
    }

    public class ContractApiService : IdSrvOAuth2ClientRequestBase, IContractApiService
    {
        private const string ServiceName = "KC.Service.WebApiService.ContractApiService";

        public ContractApiService(
            Tenant tenant, 
            System.Net.Http.IHttpClientFactory httpClient,
            Microsoft.Extensions.Logging.ILogger<ElectronicSignApiService> logger)
            : base(tenant ?? TenantConstant.DbaTenantApiAccessInfo, httpClient, logger)
        {
        }

        public ServiceResult<bool> UpdateContractGroupTo(ContractGroupDTO contractGroup, string customerCode)
        {
            var postData = SerializeHelper.ToJson(contractGroup);
            ServiceResult<bool> result = null;
            WebSendPost<ServiceResult<bool>>(
                ServiceName + ".UpdateContractGroupTo",
                GetTenantDomain(PortalSubdomainApiServerUrl, customerCode) + "CurrencySign/UpdateContractGroupTo",
                ApplicationConstant.SsoScope,
                postData,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<bool>(ServiceResultType.Error, errorMessage);
                },
                true);

            return result;
        }


        public ServiceResult<bool> EditContractStatuTo(ContractGroupDTO contractGroup, string customerCode)
        {
            var postData = SerializeHelper.ToJson(contractGroup);
            ServiceResult<bool> result = null;
            WebSendPost<ServiceResult<bool>>(
                ServiceName + ".EditContractStatuTo",
                 GetTenantDomain(PortalSubdomainApiServerUrl, customerCode) + "CurrencySign/EditContractStatuTo",
                ApplicationConstant.SsoScope,
                postData,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<bool>(ServiceResultType.Error, errorMessage);
                },
                true);

            return result;
        }

        public ServiceResult<bool> DelContractGroupTo(Guid id, string customerCode)
        {
            ServiceResult<bool> result = null;
            WebSendGet<ServiceResult<bool>>(
                ServiceName + ".DelContractGroupTo",
                  GetTenantDomain(PortalSubdomainApiServerUrl, customerCode) + "CurrencySign/DelContractGroupTo?id=" + id,
                ApplicationConstant.SsoScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<bool>(ServiceResultType.Error, errorMessage);
                },
                true);

            return result;
        }

        public ServiceResult<PaginatedBaseDTO<SearchUser>> SearchEnterpriseEmployee(int page, int size, string tName, string search)
        {
            ServiceResult<PaginatedBaseDTO<SearchUser>> result = null;
            WebSendGet<ServiceResult<PaginatedBaseDTO<SearchUser>>>(
                ServiceName + ".SearchEnterpriseEmployee",
                GetTenantDomain(PortalSubdomainApiServerUrl, Tenant.TenantName) + "MemberService/SearchEnterpriseEmployee?page=" + page + "&size=" + size + "&tName=" + tName + "&search=" + search,
                ApplicationConstant.SsoScope,
                callback =>
                {
                    result = callback;
                },
                 (httpStatusCode, errorMessage) =>
                 {
                     result = new ServiceResult<PaginatedBaseDTO<SearchUser>>(ServiceResultType.Error, errorMessage);
                 },
                true);

            return result;
        }

        public ServiceResult<bool> SendContractMessage(string customerCode, Guid id, string email, string type = "Contract_Comfirm", string remark = "")
        {
            ServiceResult<bool> result = null;
            WebSendGet<ServiceResult<bool>>(
                ServiceName + ".SendContractMessage",
                 GetTenantDomain(PortalSubdomainApiServerUrl, customerCode) + "CurrencySign/SendContractMessage?id=" + id + "&email=" + email + "&type=" + type + "&userId=" + Tenant.TenantDisplayName + "&remark=" + remark,
                 ApplicationConstant.SsoScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<bool>(ServiceResultType.Error, errorMessage);
                },
                true);

            return result;
        }

        public ServiceResult<bool> IsSeal(string personalId, string tenantName)
        {
            ServiceResult<bool> result = null;
            WebSendGet<ServiceResult<bool>>(
                ServiceName + ".IsSeal",
                GetTenantDomain(PortalSubdomainApiServerUrl, tenantName) + "CurrencySign/IsSeal?token=" + DateTime.Now.Ticks + "&personalId=" + personalId + "&memberId=" + tenantName,
                ApplicationConstant.SsoScope,
                callback =>
                {
                    result = callback;
                },
                 (httpStatusCode, errorMessage) =>
                 {
                     result = new ServiceResult<bool>(ServiceResultType.Error, errorMessage);
                 },
                true);
            return result;
        }

        public ServiceResult<bool> GetCode(string userId, string tenantName, bool? isPersonal)
        {
            ServiceResult<bool> result = null;
            WebSendGet<ServiceResult<bool>>(
                ServiceName + ".GetCode",
                GetTenantDomain(PortalSubdomainApiServerUrl, tenantName) + "CurrencySign/GetCode?userId=" + userId + "&tenantName=" + tenantName + "&isPersonal=" + isPersonal,
                ApplicationConstant.SsoScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<bool>(ServiceResultType.Error, errorMessage);
                },
                true);
            return result;
        }
    }
}
