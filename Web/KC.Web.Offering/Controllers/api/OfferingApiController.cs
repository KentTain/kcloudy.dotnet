using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using KC.Service.Offering;

namespace KC.Web.Offering.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class OfferingApiController : Web.Controllers.WebApiBaseController
    {
        protected IOfferingService _dictService => ServiceProvider.GetService<IOfferingService>();

        public OfferingApiController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<OfferingApiController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }
        
    }
}
