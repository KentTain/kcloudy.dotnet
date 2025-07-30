using KC.Framework.Tenant;
using KC.Web.Portal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace KC.Web.Portal.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Portal.Controllers.PortalBaseController
    {
        public HomeController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<HomeController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        #region 菜单配置
        /// <summary>
        /// 一级菜单：门户管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("门户管理", "门户管理", "/Admin/Home/PortalManager",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-home", AuthorityId = "79DEB1A7-491A-468F-807C-0D675A026820",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 1, IsExtPage = false, Level = 1)]
        public IActionResult PortalManager()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }

        /// <summary>
        /// 二级菜单：门户管理/企业信息管理
        /// </summary>
        [Web.Extension.MenuFilter("门户管理", "企业信息管理", "/Admin/Home/CompanyInfo",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-list-alt", AuthorityId = "2EDE67A3-ADF8-4989-8721-9A40F9811C54",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 1, IsExtPage = false, Level = 2)]
        public IActionResult CompanyInfo()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }

        /// <summary>
        /// 二级菜单：门户管理/门户信息管理
        /// </summary>
        [Web.Extension.MenuFilter("门户管理", "门户信息管理", "/Admin/Home/PortalInfo",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-list-alt", AuthorityId = "BB85AAD0-B25B-4B96-877B-709C095EC377",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 2, IsExtPage = false, Level = 2)]
        public IActionResult PortalInfo()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }

        /// <summary>
        /// 二级菜单：门户管理/门户推荐管理
        /// </summary>
        [Web.Extension.MenuFilter("门户管理", "门户推荐管理", "/Admin/Home/RecommendInfo",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-list-alt", AuthorityId = "95DE1AFD-7EFA-4513-9BDE-CEFA93731F97",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 3, IsExtPage = false, Level = 2)]
        public IActionResult RecommendInfo()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }

        /// <summary>
        /// 二级菜单：门户管理/门户装饰管理
        /// </summary>
        [Web.Extension.MenuFilter("门户管理", "门户装饰管理", "/Admin/Home/Decorator",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-list-alt", AuthorityId = "F19BE76D-9640-4164-B385-4535DB5FDBB4",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 3, IsExtPage = false, Level = 2)]
        public IActionResult Decorator()
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
