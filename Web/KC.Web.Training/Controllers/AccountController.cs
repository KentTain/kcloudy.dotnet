using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

using KC.Framework.Tenant;

namespace KC.Web.Training.Controllers
{
    public class AccountController : TrainingBaseController
    {
        public AccountController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<AccountController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        [Authorize]
        public IActionResult Sigin()
        {
            return Redirect("/");
        }
    }
}
