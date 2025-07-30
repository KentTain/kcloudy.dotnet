using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using KC.Framework.Tenant;
using KC.Web.Message.Models;
using Microsoft.AspNetCore.Authorization;
using KC.Framework.Base;

namespace KC.Web.Message.Controllers
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
        /// 一级菜单：消息管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("消息管理", "消息管理", "/Home/Message",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, 
            SmallIcon = "fa fa-commenting", AuthorityId = "D0EDDC75-D25C-42A9-8EDE-2C7917FC0D1B",
            DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 23, IsExtPage = false, Level = 1)]
        public IActionResult Message()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }

        /// <summary>
        /// 二级菜单：消息管理/消息模板管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("消息管理", "消息模板管理", "/Home/MessageTemplate",
         Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-commenting", AuthorityId = "729602C8-AA70-47D1-A512-450A58FFA89E",
         DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 1, IsExtPage = false, Level = 2)]
        public IActionResult MessageTemplate()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }

        /// <summary>
        /// 二级菜单：消息管理/新闻公告管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("消息管理", "新闻公告管理", "/Home/NewsBulletin",
         Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-commenting", AuthorityId = "2E487989-25B5-44E6-A4C2-C58F790C03D7",
         DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 2, IsExtPage = false, Level = 2)]
        public IActionResult NewsBulletin()
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
