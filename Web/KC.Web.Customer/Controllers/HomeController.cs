using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using KC.Framework.Tenant;
using KC.Web.Customer.Models;
using Microsoft.AspNetCore.Authorization;
using KC.Framework.Base;

namespace KC.Web.Customer.Controllers
{
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

        #region 菜单配置
        [Web.Extension.MenuFilter("客户管理", "客户管理", "/Home/Customer",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-users", AuthorityId = "E78ED2C1-CA26-4837-B938-9592B07108A7",
            DefaultRoleId = RoleConstants.PurchaseManagerRoleId, Order = 7, IsExtPage = false, Level = 1)]
        public IActionResult Customer()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }

        [Web.Extension.MenuFilter("客户管理", "我的客户", "/Home/CustomerInfo",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-user-circle", AuthorityId = "F44C4007-A301-483A-99ED-B5E04D478D1D",
            DefaultRoleId = RoleConstants.PurchaseManagerRoleId, Order = 1, IsExtPage = false, Level = 2)]
        public IActionResult CustomerInfo()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }

        [Web.Extension.MenuFilter("客户管理", "客户公海", "/Home/CustomerSea",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-ship", AuthorityId = "7D78D062-9E7D-4859-BB72-B6E01785814B",
            DefaultRoleId = RoleConstants.PurchaseManagerRoleId, Order = 2, IsExtPage = false, Level = 2)]
        public IActionResult CustomerSea()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }

        [Web.Extension.MenuFilter("客户管理", "客户营销", "/Home/CustomerNotification",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-shopping-bag", AuthorityId = "53491CFC-8479-4807-881A-635D64056830",
            DefaultRoleId = RoleConstants.PurchaseManagerRoleId, Order = 3, IsExtPage = false, Level = 2)]
        public IActionResult CustomerNotification()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }

        [Web.Extension.MenuFilter("客户管理", "推送日志", "/Home/SendCustomerLog",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-file-text", AuthorityId = "71512395-BBE0-4242-8B5F-CE997DD350F9",
            DefaultRoleId = RoleConstants.PurchaseManagerRoleId, Order = 4, IsExtPage = false, Level = 2)]
        public IActionResult SendCustomerLog()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }
        #endregion

        #region 辅助方法
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult AccessDenied(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult DownLoadLogFile(string name)
        {
            var url = ServerPath + "/logs/" + name + ".log";
            return File(url, "text/plain");
        }
        #endregion
    }
}
