using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using KC.Service.Contract;
using KC.Service;
using KC.Service.DTO.Contract;
using KC.Enums.Contract;
using KC.Framework.Base;

namespace KC.WebApi.Contract.Controllers
{
    /// <summary>
    /// 印章管理
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    public class ElectronicSignApiController : Web.Controllers.WebApiBaseController
    {
        private IContractService _contractService => ServiceProvider.GetService<IContractService>();

        public ElectronicSignApiController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<ElectronicSignApiController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        /// <summary>
        /// 获取合同模板
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetContractTempletById")]
        public async Task<ServiceResult<ContractTemplateValueDTO>> GetContractTempletById(int id)
        {
            return await GetServiceResultAsync(async () =>
            {
                return await _contractService.GetMyContractTemplateById(id);
            });
        }

        [HttpPost]
        //[Com.WebApi.Core.Attributes.WebApiAllowOtherTenant]
        public ServiceResult<bool> UpdateContractGroupTo(ContractGroupDTO contractGroup)
        {
            return GetServiceResult(() =>
            {
                return _contractService.UpdateContractGroupTo(contractGroup);
            });
        }

        [HttpPost]
        //[Com.WebApi.Core.Attributes.WebApiAllowOtherTenant]
        public ServiceResult<bool> EditContractStatuTo(ContractGroupDTO contractGroup)
        {
            return GetServiceResult(() =>
            {
                return _contractService.EditContractStatu(contractGroup);
            });
        }


        [HttpGet]
        //[Com.WebApi.Core.Attributes.WebApiAllowOtherTenant]
        public ServiceResult<bool> DelContractGroupTo(Guid id)
        {
            return GetServiceResult(() =>
            {
                return _contractService.RemoveSingContractServices(id);
            });
        }

        //添加合同
        public ServiceResult<string> ConfirmCurrencySign(ContractGroupDTO model)
        {
            return GetServiceResult(() =>
            {
                return _contractService.AddContractGroup(model);
            });
        }
        [HttpGet]
        public ServiceResult<string> SureUserContract(string blobId, string tenantName, UserContractStatus userContractStatu)
        {
            return GetServiceResult(() =>
            {
                return _contractService.UpdateUserContract(blobId, tenantName, userContractStatu);
            });
        }
        [HttpGet]
        public ServiceResult<string> SureContractAndUserContract(string userContractId, ContractStatus contractGroupStatu, WorkflowBusStatus workFlowStatus, UserContractStatus userContractStatu, string remark)
        {
            return GetServiceResult(() =>
            {
                return _contractService.UpdateContractAndUserContract(userContractId, contractGroupStatu, workFlowStatus, userContractStatu, remark);
            });
        }

        [HttpGet]
        public ServiceResult<string> DeleteContractGroup(string contractGroupId)
        {
            return GetServiceResult(() =>
            {
                return _contractService.DeleteContractGroup(contractGroupId);
            });
        }
    }
}
