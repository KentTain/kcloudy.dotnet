using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using KC.Service.Dict;
using KC.Service;
using KC.Service.DTO.Dict;
using KC.Service.DTO.Admin;

namespace KC.WebApi.Dict.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class DictionaryApiController : Web.Controllers.WebApiBaseController
    {
        protected IDictionaryService _dictService => ServiceProvider.GetService<IDictionaryService>();

        public DictionaryApiController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<DictionaryApiController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        /// <summary>
        /// 获取所有的省份列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("LoadAllProvinces")]
        public ServiceResult<List<ProvinceDTO>> LoadAllProvinces()
        {
            return GetServiceResult(() =>
            {
                return _dictService.FindAllProvinces();
            });
        }
        /// <summary>
        /// 获取所有的城市列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("LoadAllCities")]
        public ServiceResult<List<CityDTO>> LoadAllCities()
        {
            return GetServiceResult(() =>
            {
                return _dictService.FindAllCities();
            });
        }
        /// <summary>
        /// 根据省份Id，获取所有的城市列表
        /// </summary>
        /// <param name="provinceID">省份Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("LoadCitiesByProvinceId")]
        public ServiceResult<List<CityDTO>> LoadCitiesByProvinceId(int provinceID)
        {
            return GetServiceResult(() =>
            {
                return _dictService.FindCitiesByProvinceId(provinceID);
            });
        }
        /// <summary>
        /// 获取所有的国民经济分类树结构数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("LoadRootIndustryClassfications")]
        public ServiceResult<List<IndustryClassficationDTO>> LoadRootIndustryClassfications()
        {
            return GetServiceResult(() =>
            {
                return _dictService.FindRootIndustryClassfications();
            });
        }

        /// <summary>
        /// 根据手机号获取归属地信息
        /// </summary>
        /// <param name="mobilePhone">手机号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetMobileLocation")]
        public ServiceResult<MobileLocationDTO> GetMobileLocation(string mobilePhone)
        {
            return GetServiceResult(() =>
            {
                return _dictService.GetMobileLocation(mobilePhone);
            });
        }

        /// <summary>
        /// 根据字典类型编码，获取其下的所有字典列表数据
        /// </summary>
        /// <param name="code">字典类型编码，例如：DCT2020010100001</param>
        /// <returns></returns>
        [HttpGet]
        [Route("LoadAllDictValuesByTypeCode")]
        public ServiceResult<List<DictValueDTO>> LoadAllDictValuesByTypeCode(string code)
        {
            return GetServiceResult(() =>
            {
                return _dictService.FindAllDictValuesByDictTypeCode(code);
            });
        }
    }
}
