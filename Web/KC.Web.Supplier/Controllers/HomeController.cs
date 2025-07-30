using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using KC.Framework.Tenant;
using KC.Web.Supplier.Models;
using Microsoft.AspNetCore.Authorization;
using KC.Framework.Base;

namespace KC.Web.Supplier.Controllers
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
        [Web.Extension.MenuFilter("应用管理", "应用管理", "/Home/Application",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-list-alt", AuthorityId = "87402527-9ED1-4AEC-8F42-CCF31E12A4AB",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsExtPage = false, Level = 1)]
        public IActionResult Application()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }

        [Web.Extension.MenuFilter("应用管理", "应用业务管理", "/Home/AppBusiness",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-list-alt", AuthorityId = "CF0B3945-E2D7-4F51-ADB2-740DC85C29AA",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsExtPage = false, Level = 2)]
        public IActionResult AppBusiness()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }

        [Web.Extension.MenuFilter("应用管理", "应用推送管理", "/Home/AppPushSetting",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-list-alt", AuthorityId = "E458756C-E384-4734-99C8-7FB8257E2120",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsExtPage = false, Level = 2)]
        public IActionResult AppPushSetting()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }
        #endregion

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

        #region Cache
        [HttpGet]
        [AllowAnonymous]
        public JsonResult ClearAllCache()
        {
            Service.CacheUtil.RemoveAllCache();

            return ThrowErrorJsonMessage(true, "清除缓存成功");
        }
        [HttpGet]
        [AllowAnonymous]
        public JsonResult RemoveCacheByKey(string key)
        {
            Service.CacheUtil.RemoveCache(key);

            return ThrowErrorJsonMessage(true, "清除缓存成功");
        }

        #endregion

        [HttpGet]
        [AllowAnonymous]
        public ActionResult DownLoadLogFile(string name)
        {
            var url = ServerPath + "/logs/" + name + ".log";
            return File(url, "text/plain");
        }
    }
}
