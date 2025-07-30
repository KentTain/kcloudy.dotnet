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
using KC.Service.Enums.App;
using Microsoft.AspNetCore.Authorization;

namespace KC.Web.App.Controllers
{
    public class DevSettingController : AppBaseController
    {
        protected IAppDevelopService appDevlopService => ServiceProvider.GetService<IAppDevelopService>();
        public DevSettingController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<DevSettingController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        #region Git仓库管理
        /// <summary>
        /// 三级菜单：应用管理/开发配置管理/Git仓库管理
        /// </summary>
        [Web.Extension.MenuFilter("开发配置管理", "Git仓库管理", "/DevSetting/AppGit",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-dropbox", AuthorityId = "09577DE6-8A12-4989-9A25-70C55CF74A69",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("Git仓库管理", "Git仓库管理", "/DevSetting/AppGit",
            "09577DE6-8A12-4989-9A25-70C55CF74A69", DefaultRoleId = RoleConstants.AdminRoleId,
            Order = 2, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult AppGit(string appid)
        {
            ViewBag.appid = CurrentApplicationId;
            return View();
        }


        public async Task<IActionResult> LoadAllAppGitList(Guid? appId, string address = "")
        {
            var result = await appDevlopService.FindAllAppGits(appId, address);

            return Json(result);
        }


        public async Task<IActionResult> LoadAppGitList(int page, int rows, Guid? appId, string address = "")
        {
            var result = await appDevlopService.FindPaginatedAppGits(page, rows, appId, address);

            return Json(result);
        }

        public async Task<IActionResult> LoadAllAppGitUserList(Guid? gitId, string name)
        {
            var result = await appDevlopService.FindAllAppGitUsers(gitId, name);

            return Json(result);
        }

        [Web.Extension.PermissionFilter("Git仓库管理", "保存Git仓库", "/DevSetting/SaveAppGit",
            "9A5A6421-CC76-47DA-89F5-C61E8D8F0DCC", DefaultRoleId = RoleConstants.AdminRoleId,
            Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveAppGit(AppGitDTO model)
        {
            return GetServiceJsonResult(() =>
            {
                return appDevlopService.SaveAppGit(model);
            });
        }

        [Web.Extension.PermissionFilter("Git仓库管理", "启用/禁用Git仓库", "/DevSetting/EnableAppGit",
            "A0CCDE54-0095-4270-91F3-91E43B134A29", DefaultRoleId = RoleConstants.AdminRoleId,
            Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public IActionResult EnableAppGit(Guid id, bool enabled)
        {
            return GetServiceJsonResult(() =>
            {
                return appDevlopService.EnableAppGit(id, enabled, CurrentUserId, CurrentUserDisplayName);
            });
        }


        [HttpGet]
        public async Task<IActionResult> AppGitDetail(Guid? id, Guid appId)
        {
            var model = new AppGitDTO();
            if (id.HasValue && !id.Equals(Guid.Empty))
            {
                model = await appDevlopService.GetAppGitById(id.Value);
                model.IsEditMode = true;
            }

            return View("AppGitDetail", model);
        }

        #endregion

        #region 开发模板管理
        /// <summary>
        /// 三级菜单：应用管理/开发配置管理/开发模板管理
        /// </summary>
        [Web.Extension.MenuFilter("开发配置管理", "开发模板管理", "/DevSetting/AppTemplate",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-dropbox", AuthorityId = "8A506A12-109E-4CC8-915C-B39015FE8742",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("开发模板管理", "开发模板管理", "/DevSetting/AppTemplate",
            "8A506A12-109E-4CC8-915C-B39015FE8742", DefaultRoleId = RoleConstants.AdminRoleId,
            Order = 3, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult AppTemplate(string appId)
        {
            ViewBag.appId = CurrentApplicationId;
            return View();
        }

        public async Task<IActionResult> LoadAllTemplateList(TemplateType? type, string name = "")
        {
            var result = await appDevlopService.FindAllAppTemplates(type, name);

            return Json(result);
        }

        public async Task<IActionResult> LoadAppTemplateList(int page, int rows, TemplateType? type, string name = "")
        {
            var result = await appDevlopService.FindPaginatedAppTemplates(page, rows, type, name);

            return Json(result);
        }

        public async Task<IActionResult> GetAppTemplateForm(Guid id)
        {
            var model = new DevTemplateDTO();
            model.Id = Guid.NewGuid();
            if (id != Guid.Empty)
            {
                model = await appDevlopService.GetAppTemplateById(id);
                model.IsEditMode = true;
            }
            ViewBag.TemplateTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum(null, new List<TemplateType>()
            {
                TemplateType.H5, TemplateType.Mobile
            });
            return PartialView("_devTemplateForm", model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExistTemplateName(Guid? id, string name, string orginalName, bool isEditMode)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                if (isEditMode && name.Equals(orginalName, StringComparison.OrdinalIgnoreCase))
                    return false;

                return await appDevlopService.ExistTemplateName(id, name);
            });
        }

        [Web.Extension.PermissionFilter("开发模板管理", "保存开发模板", "/DevSetting/SaveAppTemplate",
            "52732D56-F6FA-4F29-A35D-E3D7443582CD", DefaultRoleId = RoleConstants.AdminRoleId,
            Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAppTemplate(DevTemplateDTO model)
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
                return await appDevlopService.SaveAppTemplate(model);
            });
        }

        [Web.Extension.PermissionFilter("开发模板管理", "删除开发模板", "/DevSetting/RemoveAppTemplate",
            "E825AC32-CCE3-4110-B542-66B33D87AFA1", DefaultRoleId = RoleConstants.AdminRoleId,
            Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<IActionResult> RemoveAppTemplate(Guid id)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                return await appDevlopService.RemoveAppTemplate(id);
            });
        }
        #endregion

    }
}