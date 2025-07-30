using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KC.Web.Admin.Models;
using KC.Framework.Tenant;
using Microsoft.AspNetCore.Authorization;
using KC.Framework.Base;
using Microsoft.Extensions.DependencyInjection;
using KC.Service.DTO.Account;

namespace KC.Web.Admin.Controllers
{
    public class HomeController : AdminBaseController
    {
        protected Service.WebApiService.Business.IMessageApiService messageApiService
        {
            get
            {
                return ServiceProvider.GetService<Service.WebApiService.Business.IMessageApiService>();
            }
        }

        public HomeController(IServiceProvider serviceProvider, ILogger<HomeController> logger)
            :base(serviceProvider, logger)
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
        /// 一级菜单：后台管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("后台管理", "后台管理", "/Home/System",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-cog", AuthorityId = "87402527-9ED1-4AEC-8F42-CCF31E12A4AB",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 28, IsExtPage = false, Level = 1)]
        public IActionResult System()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }

        /// <summary>
        /// 二级菜单：后台管理/资源管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("后台管理", "资源管理", "/Home/Resource",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-database", AuthorityId = "98577725-58CF-448D-AC6B-4E33F390BF25",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsExtPage = false, Level = 2)]
        public IActionResult Resource()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }

        /// <summary>
        /// 二级菜单：后台管理/租户管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("后台管理", "租户管理", "/Home/Tenant",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-users", AuthorityId = "5B38914F-6847-4DC4-B0A5-D51FFA3F3817",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsExtPage = false, Level = 2)]
        public IActionResult Tenant()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }

        /// <summary>
        /// 二级菜单：后台管理/日志管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("后台管理", "日志管理", "/Home/Log",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-file", AuthorityId = "F2994BE5-B1D7-4832-83C1-820BB0D04E84",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 4, IsExtPage = false, Level = 2)]
        public IActionResult Log()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }
        #endregion

        #region 获取菜单数据
        /// <summary>
        /// 主页面载入菜单
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> LoadMenus()
        {
            if (!base.User.Identity.IsAuthenticated)
                return new ChallengeResult();

            Func<MenuNodeSimpleDTO, bool> predicate = m => true;
            var menuTrees = await GetCurrentUserMenuTree(predicate);
            return Json(menuTrees);
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
