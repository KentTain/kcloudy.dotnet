using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using KC.Service.Admin;
using KC.Service;
using KC.Service.DTO.Admin;
using KC.Service.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KC.WebApi.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[EnableCors(Web.Util.StaticFactoryUtil.MyAllowSpecificOrigins)]
    public class TenantApiController : ControllerBase
    {
        private const string ServiceName = "KC.WebApi.Admin.Controllers.TenantController";

        private ITenantUserService TenantStore => ServiceProvider.GetService<ITenantUserService>();
        private readonly IServiceProvider ServiceProvider;
        private readonly ILogger Logger;
        public TenantApiController(IServiceProvider serviceProvider,
            ILogger<TenantApiController> logger)
        {
            Logger = logger;
            ServiceProvider = serviceProvider;
        }

        #region 获取单个租户的数据
        /// <summary>
        /// 租户名称是否存在
        /// </summary>
        /// <param name="tenantName">租户代码</param>
        /// <returns>租户信息</returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("ExistTenantName")]
        public ServiceResult<bool> ExistTenantName(string tenantName)
        {
            return ServiceWrapper.Invoke(
                ServiceName,
                MethodBase.GetCurrentMethod().Name,
                () =>
                {
                    return TenantStore.ExistTenantName(tenantName);
                },
                Logger);
        }

        /// <summary>
        /// 根据租户代码或是租户昵称获取租户信息
        /// </summary>
        /// <param name="tenantName">租户代码或是租户昵称</param>
        /// <returns>租户信息</returns>
        [HttpGet]
        [Authorize]
        [Route("GetTenantByTenantName")]
        public async Task<ServiceResult<Tenant>> GetTenantByTenantName(string tenantName)
        {
            return await ServiceWrapper.InvokeAsync(
                ServiceName,
                MethodBase.GetCurrentMethod().Name,
                async () =>
                {
                    return await TenantStore.GetTenantByNameOrNickNameAsync(tenantName);
                },
                Logger);
        }

        /// <summary>
        /// 根据租户独立域名获取租户信息
        /// </summary>
        /// <param name="domainName">租户独立域名</param>
        /// <returns>租户信息</returns>
        [HttpGet]
        [Authorize]
        [Route("GetTenantEndWithDomainName")]
        public async Task<ServiceResult<Tenant>> GetTenantEndWithDomainName(string domainName)
        {
            return await ServiceWrapper.InvokeAsync(
                ServiceName,
                MethodBase.GetCurrentMethod().Name,
                async  () =>
                {
                    return await TenantStore.GetTenantEndWithDomainNameAsync(domainName);
                },
                Logger);
        }
        #endregion

        #region 多个租户的数据
        /// <summary>
        /// 根据租户代码列表获取租户信息列表
        /// </summary>
        /// <param name="tenantNames">租户代码列表</param>
        /// <returns>租户信息列表</returns>
        [HttpPost]
        [Authorize]
        [Route("GetTenantByTenantNames")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ServiceResult<List<TenantSimpleDTO>> GetTenantByTenantNames([FromBody]List<string> tenantNames)
        {
            return ServiceWrapper.Invoke(
                ServiceName,
                MethodBase.GetCurrentMethod().Name,
                () =>
                {
                    return TenantStore.FindTenantsByNames(tenantNames).ToList();
                },
                Logger);
        }
        /// <summary>
        /// 根据开通应用Id及租户代码列表获取租户信息列表
        /// </summary>
        /// <param name="appId">开通的应用Id</param>
        /// <param name="tenantNames">租户代码列表</param>
        /// <returns>租户信息列表</returns>
        [HttpPost]
        [Authorize]
        [Route("GetOpenAppTenantsByNames")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ServiceResult<List<TenantSimpleDTO>> GetOpenAppTenantsByNames(Guid appId, [FromBody]List<string> tenantNames)
        {
            return ServiceWrapper.Invoke(
                ServiceName,
                MethodBase.GetCurrentMethod().Name,
                () =>
                {
                    return TenantStore.FindOpenAppTenantsByNames(appId, tenantNames);
                },
                Logger);
        }

        /// <summary>
        /// 根据条件，获取租户分页数据
        /// </summary>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">页面记录数</param>
        /// <param name="applicationId">开通应用Id</param>
        /// <param name="tenantDisplayName">租户中文名</param>
        /// <param name="exceptTenant">所需排除的租户代码</param>
        /// <returns>租户信息列表</returns>
        [HttpGet]
        [Authorize]
        [Route("GetTenantUsersByOpenAppId")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ServiceResult<PaginatedBaseDTO<TenantSimpleDTO>> GetTenantUsersByOpenAppId(
            int pageIndex, int pageSize,
            Guid applicationId, string tenantDisplayName, string exceptTenant)
        {
            return ServiceWrapper.Invoke(
                ServiceName,
                MethodBase.GetCurrentMethod().Name,
                () =>
                {
                    var exceptTenants = !string.IsNullOrWhiteSpace(exceptTenant)
                        ? new List<string>() { exceptTenant, TenantConstant.DbaTenantName }
                        : new List<string>() { TenantConstant.DbaTenantName };
                    return TenantStore.FindTenantUsersByOpenAppId(pageIndex, pageSize, applicationId, tenantDisplayName,
                        exceptTenants, false);
                },
                Logger);
        }

        #endregion

        #region 更新租户信息
        /// <summary>
        /// 根据租户代码TenantName，更新租户基本信息 </br>
        ///     包括：企业名称、联系人、联系电话、联系邮箱、经营范围、经营类型
        /// </summary>
        /// <param name="tenant">租户基本信息</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("UpdateTenantUserBasicInfo")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ServiceResult<bool> UpdateTenantUserBasicInfo([FromBody] TenantSimpleDTO tenant)
        {
            return ServiceWrapper.Invoke(
                ServiceName,
                MethodBase.GetCurrentMethod().Name,
                () =>
                {
                    return TenantStore.UpdateTenantUserBasicInfo(tenant);
                },
                Logger);
        }

        /// <summary>
        /// 根据租户代码（CompanyCode），添加或是保存租户认证数据
        /// </summary>
        /// <param name="tenant">租户代码认证数据</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("SaveTenantUserAuthInfo")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ServiceResult<bool> SaveTenantUserAuthInfo([FromBody] TenantUserAuthenticationDTO tenant)
        {
            return ServiceWrapper.Invoke(
                ServiceName,
                MethodBase.GetCurrentMethod().Name,
                () =>
                {
                    return TenantStore.SaveTenantUserAuthInfo(tenant);
                },
                Logger);
        }
        #endregion
    }
}
