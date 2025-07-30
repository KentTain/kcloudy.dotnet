using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using KC.Service.Training;
using KC.Service.DTO.Training;
using KC.Framework.Base;
using KC.Service;

namespace KC.WebApi.Training.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class TeacherApiController : Web.Controllers.WebApiBaseController
    {
        protected ICourseService ConfigService => ServiceProvider.GetService<ICourseService>();
        protected ITeacherService SeedService => ServiceProvider.GetService<ITeacherService>();

        public TeacherApiController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<TeacherApiController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }


        
    }
}
