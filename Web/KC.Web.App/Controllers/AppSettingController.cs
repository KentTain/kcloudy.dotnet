using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class AppSettingController : AppBaseController
    {
        protected IAppSettingService appSettingService => ServiceProvider.GetService<IAppSettingService>();

        public AppSettingController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<ApplicationController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        #region 应用配置列表

        /// <summary>
        /// 三级菜单：应用管理/开发配置管理/应用配置列表
        /// </summary>
        [Web.Extension.MenuFilter("开发配置管理", "应用配置列表", "/AppSetting/Index",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-life-ring", AuthorityId = "4319F0B9-750B-4DA8-9690-50C502107563",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("应用配置列表", "应用配置列表", "/AppSetting/Index",
            "4319F0B9-750B-4DA8-9690-50C502107563", DefaultRoleId = RoleConstants.AdminRoleId,
            Order = 1, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> LoadAllAppSettingList(Guid? appId, string name = "")
        {
            var result = await appSettingService.FindAllAppSettings(appId, name);

            return Json(result);
        }

        public async Task<IActionResult> LoadAppSettingList(int page, int rows, Guid? appId = null, string name = "")
        {
            var result = await appSettingService.FindPaginatedAppSettings(page, rows, appId ?? Guid.Empty, name);

            return Json(result);
        }


        [Web.Extension.PermissionFilter("应用配置列表", "删除应用配置", "/AppSetting/RemoveAppSetting",
            "3DC34E69-432B-466D-9104-64039FF632C5", DefaultRoleId = RoleConstants.AdminRoleId,
            Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<IActionResult> RemoveAppSetting(int id)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                return await appSettingService.RemoveAppSettingById(id, CurrentUserId, CurrentUserDisplayName);
            });
        }

        /// <summary>
        /// 三级菜单：应用管理/开发配置管理/新增/编辑应用配置
        /// </summary>
        [Web.Extension.MenuFilter("开发配置管理", "新增/编辑应用配置", "/AppSetting/GetAppSetDefForm",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-life-ring", AuthorityId = "0E440482-514A-418C-8CA1-7EC09BE4179A",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsExtPage = true, Level = 3)]
        [Web.Extension.PermissionFilter("新增/编辑应用配置", "新增/编辑应用配置", "/AppSetting/GetAppSetDefForm",
            "0E440482-514A-418C-8CA1-7EC09BE4179A", DefaultRoleId = RoleConstants.AdminRoleId,
            Order = 2, IsPage = true, ResultType = ResultType.ActionResult)]
        [HttpGet]
        public async Task<IActionResult> GetAppSetDefForm(int id, Guid? appId)
        {
            var model = await appSettingService.GetAppSettingById(id, appId);
            return View("AppSetDefForm", model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExistAppSetCode(int id, Guid? appId, string name, string orginalName, bool isEditMode)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                if (isEditMode && name.Equals(orginalName, StringComparison.OrdinalIgnoreCase))
                    return false;

                return await appSettingService.ExistAppSetCode(id, appId, name);
            });
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExistAppSetName(int id, Guid? appId, string name, string orginalName, bool isEditMode)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                if (isEditMode && name.Equals(orginalName, StringComparison.OrdinalIgnoreCase))
                    return false;

                return await appSettingService.ExistAppSetName(id, appId, name);
            });
        }

        [Web.Extension.PermissionFilter("新增/编辑应用配置", "保存应用配置", "/AppSetting/SaveAppSetting", 
            "0ADC6488-A975-4E52-AEC4-85962B9D8988", DefaultRoleId = RoleConstants.AdminRoleId, 
            Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAppSetting(AppSettingDTO model)
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
                return await appSettingService.SaveAppSetting(model);
            });
        }

        #endregion

        #region 应用配置属性

        public async Task<IActionResult> LoadAppSettingPropertyList(int settingId)
        {
            var result = await appSettingService.FindAppSettingPropertysBySettingId(settingId);

            return Json(result);
        }

        [HttpGet]
        public async Task<PartialViewResult> GetAppSetPropDefForm(int id, int settingId)
        {
            var model = new AppSettingPropertyDTO();
            model.IsEditMode = false;
            model.AppSettingId = settingId;
            if (id != 0)
            {
                model = await appSettingService.GetAppSettingPropertyById(id);
                model.IsEditMode = true;
            }
            else
            {
                var setting = await appSettingService.GetAppSettingById(settingId, null);
                model.AppSettingCode = setting?.Code;
                model.AppSettingName = setting?.Name;
            }
            ViewBag.DataTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum(null, new List<AttributeDataType>()
            {
                AttributeDataType.Object
            });
            return PartialView("_appSetPropDefForm", model);
        }

        [AllowAnonymous]
        public async Task<JsonResult> ExistAppSetPropName(int id, int settingId, string name, string orginalName, bool isEditMode)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                if (isEditMode && name.Equals(orginalName, StringComparison.OrdinalIgnoreCase))
                    return false;

                return await appSettingService.ExistAppSetPropName(id, settingId, name);
            });
        }

        [Web.Extension.PermissionFilter("应用配置列表", "保存应用配置属性", "/AppSetting/SaveAppSettingProperty", 
            "25F11F2A-C036-4BD9-9D12-542DBCED501D", DefaultRoleId = RoleConstants.AdminRoleId, 
            Order = 6, IsPage = false, ResultType = ResultType.JsonResult)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAppSettingProperty(AppSettingPropertyDTO model)
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
                return await appSettingService.SaveAppSettingProperty(model);
            });
        }

        [Web.Extension.PermissionFilter("应用配置列表", "删除应用配置属性", "/AppSetting/RemoveAppSettingProperty", 
            "B78D5C65-AF93-492A-BF8E-B9014062B72C", DefaultRoleId = RoleConstants.AdminRoleId, 
            Order = 7, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<IActionResult> RemoveAppSettingProperty(int id)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                return await appSettingService.RemoveAppSettingPropertyById(id);
            });
        }
        #endregion
    }
}