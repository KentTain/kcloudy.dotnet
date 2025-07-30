using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using KC.Framework.Tenant;
using KC.Web.Workflow.Models;
using Microsoft.AspNetCore.Authorization;
using KC.Framework.Base;

namespace KC.Web.Workflow.Controllers
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

        [Microsoft.AspNetCore.Mvc.HttpGet]
        public PartialViewResult ExecutorSetting(int type = 0)
        {
            if (type == 1)
                return PartialView("ExecutorSetting1");
            if (type == 2)
                return PartialView("ExecutorSetting2");

            return PartialView();
        }

        #region 菜单配置

        /// <summary>
        /// 一级菜单：流程管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("流程管理", "流程管理", "/Home/WorkflowManager",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, 
            SmallIcon = "fa fa-object-group", AuthorityId = "F0CABB47-E065-4B91-A729-71ECD2AC1B73",
            DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 21, IsExtPage = false, Level = 1)]
        public IActionResult WorkflowManager()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }

        /// <summary>
        /// 二级菜单：流程管理/表单定义
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("流程管理", "表单定义", "/Home/ModelDefinition",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, 
            SmallIcon = "fa fa-list-alt", AuthorityId = "2A1B6BA6-2131-45A4-9D69-6ABE9A2E08CD",
            DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 1, IsExtPage = false, Level = 2)]
        public IActionResult ModelDefinition()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }

        /// <summary>
        /// 二级菜单：流程管理/流程定义
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("流程管理", "流程定义", "/Home/WorkflowDefinition",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, 
            SmallIcon = "fa fa-list-alt", AuthorityId = "F2D9C790-5D16-4084-995F-26FF6F86321E",
            DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 2, IsExtPage = false, Level = 2)]
        public IActionResult WorkflowDefinition()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }

        ///// <summary>
        ///// 二级菜单：流程管理/流程任务中心 （move to /WorkflowProcess/Index）
        ///// </summary>
        ///// <returns></returns>
        //[Web.Extension.MenuFilter("流程管理", "流程任务中心", "/Home/WorkflowProcess",
        //    Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, 
        //    SmallIcon = "fa fa-list-alt", AuthorityId = "76EDF6E8-168E-4831-B50A-6DAA6652AABD",
        //    DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 3, IsExtPage = false, Level = 2)]
        //public IActionResult WorkflowProcess()
        //{
        //    return base.ThrowErrorJsonMessage(true, string.Empty);
        //}

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
