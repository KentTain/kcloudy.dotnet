using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using KC.Service.Portal;
using KC.Service;
using KC.Service.DTO.Portal;

namespace KC.WebApi.Portal.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class CompanyInfoApiController : Web.Controllers.WebApiBaseController
    {
        protected ICompanyInfoService _companyInfoService => ServiceProvider.GetService<ICompanyInfoService>();

        public CompanyInfoApiController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<CompanyInfoApiController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        [HttpGet]
        [Route("GetCompanyInfo")]
        public async Task<ServiceResult<CompanyInfoDTO>> GetCompanyInfo()
        {
            return await GetServiceResultAsync(async () =>
            {
                return await _companyInfoService.GetCompanyInfoAsync();
            });
        }

        
    }
}
