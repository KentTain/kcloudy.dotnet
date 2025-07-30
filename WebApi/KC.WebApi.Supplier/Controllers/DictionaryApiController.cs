using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using KC.Service.Supplier;
using KC.Service;
using KC.Service.DTO.Supplier;
using KC.Service.DTO.Admin;

namespace KC.WebApi.Supplier.Controllers
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
        [HttpGet]
        [Route("GetAllProvinces")]
        public ServiceResult<List<ProvinceDTO>> GetAllProvinces()
        {
            return GetServiceResult(() =>
            {
                return _dictService.GetAllProvinces();
            });
        }

        [HttpGet]
        [Route("GetAllCities")]
        public ServiceResult<List<CityDTO>> GetAllCities()
        {
            return GetServiceResult(() =>
            {
                return _dictService.GetAllCities();
            });
        }

        [HttpGet]
        [Route("GetRootIndustryClassfications")]
        public ServiceResult<List<IndustryClassficationDTO>> GetRootIndustryClassfications()
        {
            return GetServiceResult(() =>
            {
                return _dictService.GetRootIndustryClassfications();
            });
        }

        [HttpGet]
        [Route("GetMobileLocation")]
        public ServiceResult<MobileLocationDTO> GetMobileLocation(string mobilePhone)
        {
            return GetServiceResult(() =>
            {
                return _dictService.GetMobileLocation(mobilePhone);
            });
        }

        [HttpGet]
        [Route("GetCitiesByProvinceId")]
        public ServiceResult<List<CityDTO>> GetCitiesByProvinceId(int provinceID)
        {
            return GetServiceResult(() =>
            {
                return _dictService.GetCitiesByProvinceId(provinceID);
            });
        }
    }
}
