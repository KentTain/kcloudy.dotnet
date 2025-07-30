using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using KC.Service.DTO.Config;
using KC.Framework.Base;
using KC.Service;
using KC.Service.App;
using KC.Service.DTO.App;
using KC.Service.Enums.App;

namespace KC.Web.Config.Controllers
{
    /// <summary>
    /// 应用信息
    /// </summary>
    [Route("api/[controller]")]
    public class ApplicationApiController : Web.Controllers.WebApiBaseController
    {
        protected IApplicationService _appService => ServiceProvider.GetService<IApplicationService>();
        protected IAppSettingService _appSettingService => ServiceProvider.GetService<IAppSettingService>();
        protected IAppDevelopService _appDevelopService => ServiceProvider.GetService<IAppDevelopService>();

        public ApplicationApiController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<ApplicationApiController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        /// <summary>
        /// 获取所有的应用信息
        /// </summary>
        /// <param name="name">应用名称</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("LoadAllApplications")]
        public async Task<ServiceResult<List<ApplicationDTO>>> LoadAllApplicationsAsync(string name)
        {
            return await GetServiceResultAsync(async () =>
            {
                return await _appService.FindAllApplications(name);
            });
        }

        /// <summary>
        /// 获取所有简单的应用信息
        /// </summary>
        /// <param name="name">应用名称</param>
        /// <returns></returns>
        [HttpGet]
        [Route("LoadAllSimpleApplications")]
        public async Task<ServiceResult<IEnumerable<ApplicationInfo>>> LoadAllSimpleApplicationsAsync(string name)
        {
            return await GetServiceResultAsync(async () =>
            {
                var result = await _appService.FindAllApplications(name);
                return result.Select(m => new ApplicationInfo(m.ApplicationId, m.ApplicationCode, m.ApplicationName, m.DomainName, m.WebSiteName, m.Index));
            });
        }

        /// <summary>
        /// 获取所有的工程模板
        /// </summary>
        /// <param name="type">工程模板类型</param>
        /// <param name="name">工程模板名称</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("LoadAllAppTemplates")]
        public async Task<ServiceResult<List<DevTemplateDTO>>> LoadAllAppTemplatesAsync(TemplateType? type, string name)
        {
            return await GetServiceResultAsync(async () =>
            {
                return await _appDevelopService.FindAllAppTemplates(type, name);
            });
        }

        /// <summary>
        /// 获取应用的所有应用设置
        /// </summary>
        /// <param name="appId">应用Id</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("LoadApplicationSettings")]
        public async Task<ServiceResult<List<AppSettingDTO>>> LoadApplicationSettingsAsync(Guid appId)
        {
            return await GetServiceResultAsync(async () =>
            {
                return await _appSettingService.FindApplicationSettings(appId);
            });
        }

        /// <summary>
        /// 获取应用的所有Git设置
        /// </summary>
        /// <param name="appId">应用Id</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("LoadApplicationGits")]
        public async Task<ServiceResult<List<AppGitDTO>>> LoadApplicationGitsAsync(Guid appId)
        {
            return await GetServiceResultAsync(async () =>
            {
                return await _appDevelopService.FindAllAppGits(appId, null);
            });
        }

        /// <summary>
        /// 获取应用的工程模板
        /// </summary>
        /// <param name="appId">应用Id</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("GetApplicationTemplate")]
        public async Task<ServiceResult<DevTemplateDTO>> LoadApplicationTemplateAsync(Guid appId)
        {
            return await GetServiceResultAsync(async () =>
            {
                return await _appDevelopService.GetApplicationTemplate(appId);
            });
        }
    }
}
