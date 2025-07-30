using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Framework.Tenant;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KC.Web.Blog.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Web.Controllers.TenantWebBaseController
    {
        public HomeController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<HomeController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        public IActionResult Index()
        {
            ViewBag.UserName = base.CurrentUserName;
            ViewBag.UserEmail = base.CurrentUserEmail;
            ViewBag.UserPhone = base.CurrentUserPhone;
            ViewBag.UserDisplayName = base.CurrentUserDisplayName;
            ViewBag.TenantDisplayName = base.CurrentUserTenantDisplayName;
            return View();
        }

        [Web.Extension.MenuFilter("博客管理", "博客管理", "/Home/Blog",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-rss-square", AuthorityId = "A015AC93-7222-4918-8CD8-E73E6720E087",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 13, IsExtPage = false, Level = 1)]
        public IActionResult Blog()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }
    }
}