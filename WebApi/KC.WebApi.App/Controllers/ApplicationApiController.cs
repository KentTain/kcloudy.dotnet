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

namespace KC.WebApi.Config.Controllers
{
    /// <summary>
    /// 应用信息
    /// </summary>
    [Route("api/[controller]")]
    public class ApplicationApiController : Web.Controllers.WebApiBaseController
    {
        protected IApplicationService _appService => ServiceProvider.GetService<IApplicationService>();

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

    }
}
