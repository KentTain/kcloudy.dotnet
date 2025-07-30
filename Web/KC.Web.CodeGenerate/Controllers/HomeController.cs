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
        /// 一级菜单：代码生成管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("代码生成管理", "代码生成管理", "/Home/CodeGenerate",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, 
            SmallIcon = "fa fa-code", AuthorityId = "E25602AB-97D4-4E7E-9B94-D05F1BD3E9B0",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 26, IsExtPage = false, Level = 1)]
        public IActionResult CodeGenerate()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }

        /// <summary>
        /// 二级菜单：代码生成管理/模型管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("代码生成管理", "模型管理", "/Home/ModelDefinition",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, 
            SmallIcon = "fa fa-code", AuthorityId = "FFA02598-FE23-4CF9-993A-478379EDC462",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsExtPage = false, Level = 2)]
        public IActionResult ModelDefinition()
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
