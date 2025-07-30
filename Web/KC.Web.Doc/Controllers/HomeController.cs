using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using KC.Framework.Tenant;
using KC.Web.Doc.Models;
using Microsoft.AspNetCore.Authorization;
using KC.Framework.Base;

namespace KC.Web.Doc.Controllers
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

        /// <summary>
        /// 一级菜单：文件管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("文件管理", "文件管理", "/Home/DocManager",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, 
            SmallIcon = "fa fa-folder-open", AuthorityId = "CAA81BFE-9CDB-4AA4-8617-3A127856F9D9",
            DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 20, IsExtPage = false, Level = 1)]
        public IActionResult DocManager()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }

        /// <summary>
        /// 二级菜单：文件管理/文件模板管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("文件管理", "文件模板管理", "/Home/DocTemplate",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, 
            SmallIcon = "fa fa-file-text", AuthorityId = "875540DC-9C97-4385-BF81-B9C6F8F1B91C",
            DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 1, IsExtPage = false, Level = 2)]
        public IActionResult DocTemplate()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }

        /// <summary>
        /// 二级菜单：文件管理/文件管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("文件管理", "文件管理", "/Home/Document",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
        SmallIcon = "fa fa-folder", AuthorityId = "2C838EC1-AFD2-4ABC-8A41-9A74A3597D5A",
        DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 2, IsExtPage = false, Level = 2)]
        public IActionResult Document()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }

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
