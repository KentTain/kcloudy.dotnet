using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Web.SSO.Models;
using KC.IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

namespace KC.Web.SSO.Controllers
{
    [KC.Web.Extension.SecurityHeaders]
    [EnableCors(Startup.MyAllowSpecificOrigins)]
    public class ErrorController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;

        public ErrorController(IIdentityServerInteractionService interaction)
        {
            _interaction = interaction;
        }
        public async Task<IActionResult> Index(string errorId)
        {
            var vm = new ErrorViewModel();
            var message = await _interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                vm.Error = message;
            }

            return View("Index", vm);
        }
    }
}