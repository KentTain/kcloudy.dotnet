using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using KC.Service.Config;
using KC.Service.DTO.Config;
using KC.Framework.Base;
using KC.Service;

namespace KC.Web.Config.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ConfigApiController : Web.Controllers.WebApiBaseController
    {
        protected IConfigService ConfigService => ServiceProvider.GetService<IConfigService>();
        protected ISeedService SeedService => ServiceProvider.GetService<ISeedService>();

        public ConfigApiController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<ConfigApiController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        /// <summary>
        /// 根据业务配置名称，获取系统生成的相关业务Id对象
        /// </summary>
        /// <param name="name">业务配置名称</param>
        /// <param name="step">步长</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSeedEntityByName")]
        public ServiceResult<SeedEntity> GetSeedEntityByName(string name, int step = 1)
        {
            return GetServiceResult(() =>
            {
                return SeedService.GetSeedEntityByName(name, step);
            });
        }

        /// <summary>
        /// 根据业务配置名称，获取系统生成的相关业务Id对象
        /// </summary>
        /// <param name="name">业务配置名称</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSeedCodeByName")]
        public ServiceResult<string> GetSeedCodeByName(string name)
        {
            return GetServiceResult(() =>
            {
                return SeedService.GetSeedCodeByName(name);
            });
        }

        /// <summary>
        /// 获取所有的配置信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllConfigs")]
        public ServiceResult<List<ConfigEntityDTO>> GetAllConfigs()
        {
            return GetServiceResult(() =>
            {
                return ConfigService.FindAllConfigsWithProperties();
            });
        }
        /// <summary>
        /// 获取所有的配置信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllConfigsByType")]
        public ServiceResult<List<ConfigEntityDTO>> GetAllConfigsByType(ConfigType type)
        {
            return GetServiceResult(() =>
            {
                return ConfigService.FindAllConfigsWithPropertiesByType(type);
            });
        }

        /// <summary>
        /// 根据Id，获取配置对象
        /// </summary>
        /// <param name="id">配置Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetConfigById")]
        public ServiceResult<ConfigEntityDTO> GetConfigById(int id)
        {
            return GetServiceResult(() =>
            {
                return ConfigService.GetConfigById(id);
            });
        }

        /// <summary>
        /// 根据配置类型，获取配置对象
        /// </summary>
        /// <param name="type">配置类型</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetConfigByType")]
        public ServiceResult<ConfigEntityDTO> GetConfigByType(ConfigType type)
        {
            return GetServiceResult(() =>
            {
                return ConfigService.GetConfigByType(type);
            });
        }
    }
}
