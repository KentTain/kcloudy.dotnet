using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using KC.Framework.Base;
using KC.Service;
using KC.Service.CodeGenerate;
using KC.Service.DTO.CodeGenerate;
using KC.Service.DTO.App;

namespace KC.Web.App.Controllers
{
    /// <summary>
    /// 应用信息
    /// </summary>
    [Route("api/[controller]")]
    public class ModelDefinitionApiController : Web.Controllers.WebApiBaseController
    {
        protected IModelDefinitionService _appService => ServiceProvider.GetService<IModelDefinitionService>();

        public ModelDefinitionApiController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<ModelDefinitionApiController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        /// <summary>
        /// 获取所有的应用信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("LoadAllModelDefinitions")]
        public async Task<ServiceResult<List<ModelDefinitionDTO>>> LoadAllModelDefinitionsAsync()
        {
            return await GetServiceResultAsync(async () =>
            {
               return await _appService.FindAllModelDefinitionsAsync();
            });
        }


    }
}
