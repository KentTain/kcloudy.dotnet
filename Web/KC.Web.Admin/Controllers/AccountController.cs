using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using KC.Web.Base;
using KC.Service.WebApiService.Business;
using Microsoft.Extensions.DependencyInjection;

namespace KC.Web.Admin.Controllers
{
    public class AccountController : AdminBaseController
    {
        protected IAccountApiService AccountApiService
        {
            get
            {
                return ServiceProvider.GetService<IAccountApiService>();
                //return KC.Service.Util.UnityContainerUtil.TenantResolve<IAccountApiService>();
            }
        }

        public AccountController(IServiceProvider serviceProvider, ILogger<AccountController> logger)
            : base(serviceProvider, logger)
        {
        }

        [Authorize]
        public IActionResult Sigin()
        {
            return Redirect("/");
        }
    }
}
