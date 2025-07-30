using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using KC.Framework.Tenant;
using KC.Service.CodeGenerate;
using KC.Service.DTO.CodeGenerate;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using KC.Framework.Base;
using KC.Service.DTO.App;

namespace KC.Web.App.Controllers
{
    /// <summary>
    /// 三级菜单：代码生成管理/模型管理/数据模型
    /// </summary>
    public class ModelDefinitionController : CodeGenerateBaseController
    {
        protected IModelDefinitionService AppService => ServiceProvider.GetService<IModelDefinitionService>();
        public ModelDefinitionController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<ModelDefinitionController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }
 
        [Web.Extension.MenuFilter("模型管理", "数据模型", "/ModelDefinition/Index",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, 
            SmallIcon = "fa fa-dropbox", AuthorityId = "C1396AE0-5CD8-4008-B862-FDA1B34A7CCA",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("模型管理", "数据模型", "/ModelDefinition/Index",
            "C1396AE0-5CD8-4008-B862-FDA1B34A7CCA", DefaultRoleId = RoleConstants.AdminRoleId, 
            Order = 1, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult Index(string appid)
        {
            return View();
        }

        #region ModelDefinition

        public async Task<IActionResult> LoadModelDefinitionList(int page, int rows, string searchValue = "")
        {
            var result = await AppService.FindPaginatedModelDefinitionsByFilterAsync(page, rows, searchValue);
            return Json(result);
        }

        public async Task<IActionResult> LoadModelDefFieldList(int id)
        {
            var result = await AppService.FindModulesByAppIdAsync(id);
            return Json(result);
        }

        [Web.Extension.PermissionFilter("模型管理", "删除数据模型", "/ModelDefinition/RemoveModelDefinition",
            "7D996D8E-EF5D-4151-A5DF-9C2D2938F447", DefaultRoleId = RoleConstants.AdminRoleId,
            Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public IActionResult RemoveModelDefinition(int id)
        {
            return GetServiceJsonResult(() =>
            {
                return AppService.RemoveModelDefinition(id);
            });
        }

        /// <summary>
        /// 三级菜单：代码生成管理/模型管理/新增/编辑数据模型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Web.Extension.MenuFilter("模型管理", "新增/编辑数据模型", "/ModelDefinition/GetModelDefinitionForm",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-dropbox", AuthorityId = "C1396AE0-5CD8-4008-B862-FDA1B34A7CCA",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("模型管理", "新增/编辑数据模型", "/ModelDefinition/GetModelDefinitionForm",
            "C1396AE0-5CD8-4008-B862-FDA1B34A7CCA", DefaultRoleId = RoleConstants.AdminRoleId,
            Order = 1, IsPage = true, ResultType = ResultType.ActionResult)]
        public async Task<IActionResult> GetModelDefinitionForm(int id, int? categoryId)
        {
            var model = new ModelDefinitionDTO();
            if (id != 0)
            {
                model = await AppService.GetModelDefinitionByIdAsync(id);
            }

            return PartialView("ModelDefinitionForm", model);
        }

        [Web.Extension.PermissionFilter("新增/编辑数据模型", "保存数据模型", "/ModelDefinition/SaveModelDefinition",
            "624AD3A5-A7AD-4AB3-8B8E-D6916564D927", DefaultRoleId = RoleConstants.AdminRoleId, 
            Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveModelDefinition(ModelDefinitionDTO model)
        {
            return GetServiceJsonResult(() =>
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

                var result = AppService.SaveModelDefinition(model);
                return result;
            });
        }

        
        public IActionResult GetModelDefinitionSelectList(int tenantId)
        {
            ViewBag.TenantId = tenantId;
            return PartialView("_applicationSelectList");
        }

        #endregion

        #region ModelDefinition's Fields

        [HttpGet]
        public async Task<IActionResult> GetModelDefFieldForm(int id, string defId)
        {
            var model = new ModelDefFieldDTO();
            if (id != 0)
            {
                model = await AppService.GetModuleByIdAsync(id);
            }

            return PartialView("_moduleForm", model);
        }

        [Web.Extension.PermissionFilter("数据模型", "保存数据模型属性", "/ModelDefinition/SaveModelDefField",
            "AA313516-4C42-4D13-A5A5-8FB30E08EFE5", DefaultRoleId = RoleConstants.AdminRoleId, 
            Order = 6, IsPage = false, ResultType = ResultType.JsonResult)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveModelDefField(ModelDefFieldDTO model)
        {
            return GetServiceJsonResult(() =>
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

                return AppService.SaveModule(model);
            });
        }

        [Web.Extension.PermissionFilter("数据模型", "删除数据模型属性", "/ModelDefinition/RemoveModelDefField",
            "54099A1B-B97E-45EB-9DF6-379D5EE464C0", DefaultRoleId = RoleConstants.AdminRoleId, 
            Order = 7, IsPage = false, ResultType = ResultType.JsonResult)]
        public IActionResult RemoveModelDefField(int id)
        {
            return GetServiceJsonResult(() =>
            {
                return AppService.RemoveModuleById(id);
            });
        }
        #endregion

    }
}