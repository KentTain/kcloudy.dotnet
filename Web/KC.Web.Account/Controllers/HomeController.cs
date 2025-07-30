using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using KC.Framework.Tenant;
using KC.Web.Account.Models;
using Microsoft.AspNetCore.Authorization;
using KC.Framework.Base;
using KC.Service.DTO.Account;
using KC.Framework.Extension;
using KC.Service.Enums.Message;
using KC.Service.DTO.Search;
using AutoMapper;
using KC.Service.Util;

namespace KC.Web.Account.Controllers
{
    public class HomeController : AccountBaseController
    {
        private readonly IMapper Mapper;
        protected Service.Account.WebApiService.IMessageApiService messageApiService
        {
            get
            {
                return ServiceProvider.GetService<Service.Account.WebApiService.IMessageApiService>();
            }
        }
        protected Service.WebApiService.Business.IWorkflowApiService workflowApiService
        {
            get
            {
                return ServiceProvider.GetService<Service.WebApiService.Business.IWorkflowApiService>();
            }
        }

        public HomeController(
            Tenant tenant,
            IMapper mapper,
            IServiceProvider serviceProvider,
            ILogger<HomeController> logger)
            : base(tenant, serviceProvider, logger)
        {
            Mapper = mapper;
        }

        [Web.Extension.PermissionFilter("系统管理首页", "首页页面", "Home/Index", ApplicationConstant.DefaultAuthorityId,
            DefaultRoleId = RoleConstants.DefaultRoleId, Order = 0, IsPage = true, ResultType = ResultType.ActionResult, IsDeleted = true)]
        public IActionResult Index()
        {
            ViewBag.UserName = base.CurrentUserName;
            ViewBag.UserEmail = base.CurrentUserEmail;
            ViewBag.UserPhone = base.CurrentUserPhone;
            ViewBag.UserDisplayName = base.CurrentUserDisplayName;
            ViewBag.CanOrgManager = base.IsSystemAdmin || CurrentUserPosition == PositionLevel.Mananger;
            ViewBag.TenantDisplayName = base.CurrentUserTenantDisplayName;
            return View();
        }

        #region 菜单配置
        /// <summary>
        /// 一级菜单：系统管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("系统管理", "系统管理", "/Home/Enterprise",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-cogs", AuthorityId = "8D4AB971-7EB6-42DC-B40E-6CF7590381F1",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 27, IsExtPage = false, Level = 1)]
        public IActionResult Enterprise()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }

        /// <summary>
        /// 二级菜单：系统管理/组织管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("系统管理", "组织管理", "/Home/Organization",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-sitemap", AuthorityId = "6A773F5C-8BE0-4381-8036-1B6F948015CB",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsExtPage = false, Level = 2)]
        public IActionResult Organization()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }

        /// <summary>
        /// 二级菜单：系统管理/用户管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("系统管理", "用户管理", "/Home/User",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-user", AuthorityId = "495C2EB0-8772-4430-9952-5B528F41605F",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsExtPage = false, Level = 2)]
        public IActionResult User()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }

        /// <summary>
        /// 二级菜单：系统管理/菜单权限管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("系统管理", "菜单权限管理", "/Home/MenuPermission",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-bars", AuthorityId = "986CA9CE-7AB2-411D-83C5-CFB3C65D2FFB",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 4, IsExtPage = false, Level = 2)]
        public IActionResult MenuPermission()
        {
            return base.ThrowErrorJsonMessage(true, string.Empty);
        }
        #endregion

        #region 获取菜单/权限数据
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

        #region 获取消息数据
        /// <summary>
        /// 主页获取内部通知
        /// </summary>
        /// <returns></returns>
        public JsonResult LoadNewsBulletins()
        {
            var result = messageApiService.LoadLatestNewsBulletins(NewsBulletinType.Internal);
            return Json(result);
        }

        /// <summary>
        /// 主页获取用户消息
        /// </summary>
        /// <returns></returns>
        public JsonResult LoadMessages()
        {
            var result = messageApiService.LoadTop10UserMessages(base.CurrentUserId, null);
            return Json(result);
        }

        /// <summary>
        /// 消息已读
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult ReadMessage(int id)
        {
            var result = messageApiService.ReadRemindMessage(id);
            return Json(result);
        }

        #endregion

        #region 获取工作流待办任务
        /// <summary>
        /// 获取工作流待办任务
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> LoadWorkflowTasks()
        {
            var search = new WorkflowTaskSearchDTO() {
                UserId = CurrentUserId,
                RoleIds = CurrentUserRoleIds,
                OrgCodes = CurrentUserOrgCodes,
            };
            var result = await workflowApiService.LoadUserWorkflowTasksAsync(search);
            return Json(result);
        }
        #endregion

        #region 获取图表数据

        public async Task<JsonResult> GetChartData(DateTime? startDate, DateTime? endDate)
        {
            var settings = Common.SerializeHelper.GetJsonSerializerSettings();
            var option = await SysManageService.GetUserLoginReportData(startDate, endDate);
            return Json(option, settings);
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
