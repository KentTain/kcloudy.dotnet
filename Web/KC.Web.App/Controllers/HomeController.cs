using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

using KC.Framework.Tenant;
using KC.Web.App.Models;
using KC.Framework.Base;
using Microsoft.Extensions.DependencyInjection;
using KC.Service.DTO.Account;

namespace KC.Web.App.Controllers
{
    public class HomeController : Web.Controllers.TenantWebBaseController
    {
        protected Service.WebApiService.Business.IMessageApiService messageApiService
        {
            get
            {
                return ServiceProvider.GetService<Service.WebApiService.Business.IMessageApiService>();
            }
        }

        public HomeController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<HomeController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        [Authorize]
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
        /// 一级菜单：应用管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("应用管理", "应用管理", "/Home/Application",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, 
            SmallIcon = "fa fa-list-alt", AuthorityId = "87402527-9ED1-4AEC-8F42-CCF31E12A4AB",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 26, IsExtPage = false, Level = 1)]
        public IActionResult Application()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }

        /// <summary>
        /// 二级菜单：应用管理/应用管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("应用管理", "应用管理", "/Home/AppManager",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, 
            SmallIcon = "fa fa-list-alt", AuthorityId = "CF0B3945-E2D7-4F51-ADB2-740DC85C29AA",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsExtPage = false, Level = 2)]
        public IActionResult AppManager()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }


        /// <summary>
        /// 二级菜单：应用管理/开发配置管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("应用管理", "开发配置管理", "/Home/AppDevSetting",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-list-alt", AuthorityId = "6493AD88-ACFD-43F5-B134-6DD467403E45",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsExtPage = false, Level = 2)]
        public IActionResult AppDevSetting()
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
