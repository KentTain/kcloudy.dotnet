using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using KC.Framework.Tenant;
using KC.Web.Dict.Models;
using Microsoft.AspNetCore.Authorization;
using KC.Framework.Base;

namespace KC.Web.Dict.Controllers
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

        /// <summary>
        /// 一级菜单：字典管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("字典管理", "字典管理", "/Home/Dictionary",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-book", AuthorityId = "21E87C50-B014-40BD-ADD7-01C64513FD3A",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 25, IsExtPage = false, Level = 1)]
        public IActionResult Dictionary()
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
