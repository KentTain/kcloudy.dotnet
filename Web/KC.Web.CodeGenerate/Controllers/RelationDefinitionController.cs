using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using KC.Framework.Tenant;
using KC.Service.CodeGenerate;
using KC.Service.DTO.CodeGenerate;
using System.Threading.Tasks;
using KC.Framework.Base;

namespace KC.Web.App.Controllers
{
    /// <summary>
    /// 三级菜单：代码生成管理/模型管理/关系模型
    /// </summary>
    public class RelationDefinitionController : CodeGenerateBaseController
    {
        protected IRelationDefinitionService AppService => ServiceProvider.GetService<IRelationDefinitionService>();

        public RelationDefinitionController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<ModelDefinitionController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        [Web.Extension.MenuFilter("模型管理", "关系模型", "/RelationDefinition/Index",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-life-ring", AuthorityId = "9CF893D5-31E7-43C2-8037-201990C16BDB",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("关系模型", "关系模型", "/RelationDefinition/Index",
            "9CF893D5-31E7-43C2-8037-201990C16BDB", DefaultRoleId = RoleConstants.AdminRoleId, 
            Order = 2, IsPage = true, ResultType = ResultType.ActionResult)]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        #region RelationDefinition

        public async Task<IActionResult> LoadRelationDefinitionList(int page, int rows, string categoryId = null, string name = "")
        {
            var result = await AppService.FindPaginatedRelationDefsByFilterAsync(page, rows, categoryId, name);

            return Json(result);
        }

        public async Task<IActionResult> LoadRelationDefDetailList(int id)
        {
            var result = await AppService.FindAllRelationDefDetailListAsync(id);
            return Json(result);
        }


        [Web.Extension.PermissionFilter("关系模型", "删除关系模型属性", "/RelationDefinition/RemoveRelationDefinition",
            "7E7E0CB7-49DB-4C2B-8219-8F95996B12C1", DefaultRoleId = RoleConstants.AdminRoleId, 
            Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public IActionResult RemoveRelationDefinition(int id)
        {
            return GetServiceJsonResult(() =>
            {
                return AppService.RemoveRelationDefById(id);
            });
        }

        /// <summary>
        /// 三级菜单：代码生成管理/模型管理/新增/编辑关系模型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Web.Extension.MenuFilter("模型管理", "新增/编辑关系模型", "/RelationDefinition/GetModelDefinitionForm",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-dropbox", AuthorityId = "3D20BC51-4A68-458A-A078-9561BCD77500",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("模型管理", "新增/编辑关系模型", "/RelationDefinition/GetModelDefinitionForm",
            "3D20BC51-4A68-458A-A078-9561BCD77500", DefaultRoleId = RoleConstants.AdminRoleId,
            Order = 1, IsPage = true, ResultType = ResultType.ActionResult)]
        [HttpGet]
        public async Task<IActionResult> GetRelationDefDetailForm(int id)
        {
            var model = new RelationDefinitionDTO();
            if (id != 0)
            {
                model = await AppService.GetRelationDefByIdAsync(id);
            }

            return PartialView("RelationDefinitionForm", model);
        }

        [Web.Extension.PermissionFilter("新增/编辑关系模型", "保存关系模型", "/RelationDefinition/SaveRelationDefinition",
            "0A941AE9-300F-481D-A3DC-6DEE67BD4B30", DefaultRoleId = RoleConstants.AdminRoleId,
            Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveRelationDefinition(RelationDefinitionDTO model)
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

                return AppService.SaveRelationDef(model);
            });
        }
        #endregion

        #region RelationDefinition's Detail

        [HttpGet]
        public async Task<IActionResult> GetRelationDefDetailForm(int id, string defId)
        {
            var model = new RelationDefDetailDTO();
            if (id != 0)
            {
                model = await AppService.GetRelationDefDetailByIdAsync(id);
            }

            return PartialView("_relationDefDetailForm", model);
        }

        [Web.Extension.PermissionFilter("关系模型", "保存关系模型属性", "/RelationDefinition/SaveRelationDefDetail",
            "9E25E12D-D8F4-480A-9472-7E217725F74C", DefaultRoleId = RoleConstants.AdminRoleId,
            Order = 6, IsPage = false, ResultType = ResultType.JsonResult)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveRelationDefDetail(RelationDefDetailDTO model)
        {
            return GetServiceJsonResult(() =>
            {
                return AppService.SaveRelationDefDetail(model);
            });
        }

        [Web.Extension.PermissionFilter("关系模型", "删除关系模型属性", "/RelationDefinition/RemoveRelationDefDetail",
            "7E7E0CB7-49DB-4C2B-8219-8F95996B12C1", DefaultRoleId = RoleConstants.AdminRoleId,
            Order = 7, IsPage = false, ResultType = ResultType.JsonResult)]
        public IActionResult RemoveRelationDefDetail(int id)
        {
            return GetServiceJsonResult(() =>
            {
                return AppService.RemoveRelationDefDetailById(id);
            });
        }
        #endregion
    }
}