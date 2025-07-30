using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using KC.Framework.Tenant;
using KC.Service.App;
using KC.Service.DTO.App;
using System.Threading.Tasks;
using KC.Framework.Base;
using Microsoft.AspNetCore.Authorization;

namespace KC.Web.App.Controllers
{
    public class ApplicationController : AppBaseController
    {
        protected IApplicationService appService => ServiceProvider.GetService<IApplicationService>();
        protected IAppSettingService appSettingService => ServiceProvider.GetService<IAppSettingService>();
        public ApplicationController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<ApplicationController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        /// <summary>
        /// 三级菜单：应用管理/应用管理/应用列表
        /// </summary>
        [Web.Extension.MenuFilter("应用管理", "应用列表", "/Application/Index",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, 
            SmallIcon = "fa fa-dropbox", AuthorityId = "42911D0D-8CF4-421C-9CE3-21E5DFCC78B3",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("应用列表", "应用列表", "/Application/Index", 
            "42911D0D-8CF4-421C-9CE3-21E5DFCC78B3", DefaultRoleId = RoleConstants.AdminRoleId, 
            Order = 1, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult Index(string appId)
        {
            ViewBag.appId = CurrentApplicationId;
            return View();
        }

        #region 应用管理

        public async Task<IActionResult> LoadAllApplications(bool hasAll, string name)
        {
            var result = await appService.FindAllApplications(name);
            if (hasAll)
                result.Insert(0, new ApplicationDTO()
                {
                    ApplicationId = Guid.Empty,
                    ApplicationName = "所有应用"
                });
            return Json(result);
        }

        public async Task<IActionResult> LoadApplicationList(int page, int rows, string name = "")
        {
            var result = await appService.FindPaginatedApplications(page, rows, name, false);
            return Json(result);
        }


        [Web.Extension.PermissionFilter("应用列表", "开通流程", "/Application/EnableWorkFlow",
            "F32C3127-0267-48C6-8BB9-AA9D1245A604", DefaultRoleId = RoleConstants.AdminRoleId,
            Order = 4, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult EnableWorkFlow(Guid id, bool type)
        {
            return GetServiceJsonResult(() =>
            {
                return appService.EnableWorkFlow(id, type);
            });
        }

        [Web.Extension.PermissionFilter("应用列表", "删除应用", "/Application/RemoveApplication",
            "0E86F5D6-F93B-47FD-BBC1-39CBBAF17178", DefaultRoleId = RoleConstants.AdminRoleId,
            Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<JsonResult> RemoveApplication(Guid id)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                return await appService.RemoveApplication(id);
            });
        }

        public async Task<PartialViewResult> GetApplicationForm(Guid id)
        {
            var model = new ApplicationDTO();
            if (id != Guid.Empty)
            {
                model = await appService.GetApplicationById(id);
                model.IsEditMode = true;
            }

            return PartialView("_applicationForm", model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExistAppCode(Guid? id, string name, string orginalName, bool isEditMode)
        {
            if (isEditMode && name.Equals(orginalName, StringComparison.OrdinalIgnoreCase))
                return Json(false);

            return Json(await appService.ExistAppCode(id, name));
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExistAppName(Guid? id, string name, string orginalName, bool isEditMode)
        {
            if (isEditMode && name.Equals(orginalName, StringComparison.OrdinalIgnoreCase))
                return Json(false);

            return Json(await appService.ExistAppName(id, name));
        }

        [Web.Extension.PermissionFilter("应用列表", "保存应用", "/Application/SaveApplication", 
            "9C65146A-A0BE-4E15-A6C7-C3FDCF897121", DefaultRoleId = RoleConstants.AdminRoleId, 
            Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveApplication(ApplicationDTO model)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                if (!model.IsEditMode)
                {
                    model.CreatedName = CurrentUserDisplayName;
                    model.CreatedBy = CurrentUserId;
                    model.CreatedDate = DateTime.UtcNow;
                }
                model.ModifiedName = CurrentUserDisplayName;
                model.ModifiedBy = CurrentUserId;
                model.ModifiedDate = DateTime.UtcNow;
                return await appService.SaveApplication(model);
            });
        }

        public PartialViewResult GetApplicationSelectList(int tenantId)
        {
            ViewBag.TenantId = tenantId;
            return PartialView("_applicationSelectList");
        }

        #endregion

        #region 应用设置

        /// <summary>
        /// 三级菜单：应用管理/应用管理/应用设置
        /// </summary>
        [Web.Extension.MenuFilter("应用管理", "应用设置", "/Application/AppSettingForm",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-dropbox", AuthorityId = "C8EEF01F-8EE6-4BDB-8750-3D87BB66BDC7",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsExtPage = true, Level = 3)]
        [Web.Extension.PermissionFilter("应用设置", "应用设置", "/Application/AppSettingForm",
            "C8EEF01F-8EE6-4BDB-8750-3D87BB66BDC7", DefaultRoleId = RoleConstants.AdminRoleId,
            Order = 1, IsPage = true, ResultType = ResultType.ActionResult)]
        public async Task<IActionResult> AppSettingForm(Guid appId)
        {
            var model = await appService.GetApplicationWithSettingsById(appId);
            return View("AppSettingForm", model);
        }

        [Web.Extension.PermissionFilter("应用设置", "保存应用设置", "/Application/SaveAppSetting",
            "AC66039E-42F2-4566-9D62-F445D17D90CE", DefaultRoleId = RoleConstants.AdminRoleId,
            Order = 1, IsPage = false, ResultType = ResultType.JsonResult)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAppSetting(ApplicationDTO model)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                if (!model.IsEditMode)
                {
                    model.CreatedName = CurrentUserDisplayName;
                    model.CreatedBy = CurrentUserId;
                    model.CreatedDate = DateTime.UtcNow;
                }
                model.ModifiedName = CurrentUserDisplayName;
                model.ModifiedBy = CurrentUserId;
                model.ModifiedDate = DateTime.UtcNow;

                return await appSettingService.SaveAppSettingAttrs(model.AppSettings); ;
            });
        }

        #endregion

        #region 回收站

        /// <summary>
        /// 三级菜单：应用管理/应用管理/回收站
        /// </summary>
        [Web.Extension.MenuFilter("应用管理", "回收站", "/Application/AppRecycle",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-dropbox", AuthorityId = "475007EF-7297-498B-9BB6-01C788252B86",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("回收站", "回收站", "/Application/AppRecycle",
            "475007EF-7297-498B-9BB6-01C788252B86", DefaultRoleId = RoleConstants.AdminRoleId,
            Order = 1, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult AppRecycle()
        {
            return View("AppRecycle");
        }

        public async Task<IActionResult> LoadAppRecycleList(int page, int rows, string name)
        {
            var result = await appService.FindPaginatedApplications(page, rows, name, true);
            return Json(result);
        }

        [Web.Extension.PermissionFilter("回收站", "恢复应用", "/Application/RecoverApplication",
            "D3D874E9-7757-40AE-9D11-69A2B315E54A", DefaultRoleId = RoleConstants.AdminRoleId,
            Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<IActionResult> RecoverApplication(Guid id)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                return await appService.RecoverApplication(id);
            });
        }
        #endregion

        #region 应用日志
        /// <summary>
        /// 二级菜单：应用管理/应用日志
        /// </summary>
        [Web.Extension.MenuFilter("应用管理", "应用日志", "/Application/AppLog",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-dropbox", AuthorityId = "8A30C5D5-D0C2-4CAF-B3BF-088825E6E3A4",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 9, IsExtPage = false, Level = 2)]
        [Web.Extension.PermissionFilter("应用日志", "应用日志", "/Application/AppLog",
            "8A30C5D5-D0C2-4CAF-B3BF-088825E6E3A4", DefaultRoleId = RoleConstants.AdminRoleId,
            Order = 9, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult AppLog(Guid? appId)
        {
            ViewBag.appid = appId.HasValue ? appId.Value : CurrentApplicationId;
            return View();
        }


        public IActionResult LoadApplicationLogList(int page = 1, int rows = 20, string name = null)
        {
            var result = appService.FindPaginatedApplicationLogs(page, rows, name);

            return Json(result);
        }
        #endregion

    }
}