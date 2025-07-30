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
using KC.Service.Customer;
using KC.Service.DTO.Customer;
using KC.Service;

namespace KC.WebApi.Customer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class CustomerApiController : Web.Controllers.WebApiBaseController
    {
        protected ICustomerService CustomerService => ServiceProvider.GetService<ICustomerService>();

        public CustomerApiController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<CustomerApiController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        /// <summary>
        /// 第三方Api导入客户信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ServiceResult<bool> ImportCustomersFromOtherTenant(List<CustomerInfoDTO> data)
        {
            return GetServiceResult(() =>
            {
                return CustomerService.ImportCustomersFromOtherTenant(data);
            });
        }

        /// <summary>
        /// 测试接口是否存在
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ServiceResult<bool> ExistTenantService()
        {
            return GetServiceResult(() =>
            {
                return CustomerService != null;
            });
        }

    }
}
