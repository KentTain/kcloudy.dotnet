using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using KC.Service.Workflow;
using KC.Framework.Tenant;
using KC.Framework.Base;
using KC.Service.DTO.Workflow;
using KC.Service.Workflow.DTO;

namespace KC.Web.Workflow.Controllers
{
    public class ModelDefinitionController : WorkflowBaseController
    {
        private readonly IModelDefinitionService _modelDefinitionService;

        public ModelDefinitionController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            IModelDefinitionService modelDefinitionService,
            ILogger<WorkflowDefinitionController> logger)
            : base(tenant, serviceProvider, logger)
        {
            _modelDefinitionService = modelDefinitionService;
        }

        /// <summary>
        /// 三级菜单：流程管理/表单定义/表单定义列表
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("表单定义", "表单定义列表", "/ModelDefinition/Index",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, 
            SmallIcon = "fa fa-list-alt", AuthorityId = "48DEBEC5-8B68-4E58-A75C-857556A3CBE5",
            DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 1, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("表单定义列表", "表单定义列表", "/ModelDefinition/Index", 
            "48DEBEC5-8B68-4E58-A75C-857556A3CBE5", DefaultRoleId = RoleConstants.ExecutorManagerRoleId, 
            Order = 1, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult Index()
        {
            ViewBag.BusinessTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<BusinessType>();
            return View();
        }

        #region 表单定义
        public JsonResult LoadModelDefinitionList(int page = 1, int rows = 10, string name = "", BusinessType? type = null)
        {
            var result = _modelDefinitionService.FindPaginatedModelDefinitionsByName(page, rows, name, type);
            return Json(result);
        }

        public JsonResult LoadModelDefFieldList(int id)
        {
            var result = id != 0
                ? _modelDefinitionService.FindAllModelDefFieldsByDefId(id)
                : new List<ModelDefFieldDTO>();
            return Json(result);
        }

        /// <summary>
        /// 移除表单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Web.Extension.PermissionFilter("表单定义列表", "删除表单", "/ModelDefinition/RemoveModelDefinition",
            "2A7717B7-C71E-458D-980D-C4CB8FDB210B", DefaultRoleId = RoleConstants.ExecutorManagerRoleId, 
            Order = 1, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<IActionResult> RemoveModelDefinition(int id)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                return await _modelDefinitionService.RemoveModelDefinitionByIdAsync(id, CurrentUserId, CurrentUserDisplayName);
            });
        }

        /// <summary>
        /// 三级菜单：流程管理/表单定义/新增/编辑表单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Web.Extension.MenuFilter("表单定义", "新增/编辑表单", "/ModelDefinition/GetModelDefinitionForm",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-sitemap", AuthorityId = "D0150A82-DB4C-4956-81E0-161AF23469A2",
            DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 3, IsExtPage = true, Level = 3)]
        [Web.Extension.PermissionFilter("新增/编辑表单", "新增/编辑表单", "/ModelDefinition/GetModelDefinitionForm",
            "D0150A82-DB4C-4956-81E0-161AF23469A2", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 2, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult GetModelDefinitionForm(int id)
        {
            var model = new ModelDefinitionDTO();
            if (id != 0)
            {
                model = _modelDefinitionService.GetModelDefinitionById(id);
                model.IsEditMode = true;
            }

            ViewBag.DataTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<AttributeDataType>();
            ViewBag.BusinessTypeList = id != 0
                ? KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<BusinessType>((int)model.BusinessType)
                : KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<BusinessType>();
            return View("ModelDefinitionForm", model);
        }

        /// <summary>
        /// 添加/修改表单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Web.Extension.PermissionFilter("新增/编辑表单", "保存表单", "/ModelDefinition/SaveModelDefinition",
            "A1CD7E76-F748-4632-8C5A-A7EEB30F45AB", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 1, IsPage = false, ResultType = ResultType.JsonResult)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveModelDefinition(ModelDefinitionDTO model)
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

                return await _modelDefinitionService.SaveModelDefinitionWithFieldsAsync(model);
            });
        }

        [Web.Extension.PermissionFilter("新增/编辑表单", "删除表单属性", "/ModelDefinition/RemoveModelDefField",
            "93A3A179-6EF4-421F-B9E7-8165B40D59FB", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<IActionResult> RemoveModelDefField(int id)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                return await _modelDefinitionService.RemoveModelDefFieldByIdAsync(id, CurrentUserId, CurrentUserDisplayName);
            });
        }
        #endregion

        #region 表单定义日志
        /// <summary>
        /// 三级菜单：流程管理/表单定义/表单定义日志
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("表单定义", "表单定义日志", "/ModelDefinition/ModelDefLog",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, 
            SmallIcon = "fa fa-list-alt", AuthorityId = "A24758B6-7782-4C00-B93D-3F00175D6C6F",
            DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 2, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("表单定义日志", "表单定义日志", "/ModelDefinition/ModelDefLog", 
            "A24758B6-7782-4C00-B93D-3F00175D6C6F", DefaultRoleId = RoleConstants.ExecutorManagerRoleId, 
            Order = 3, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult ModelDefLog()
        {
            return View();
        }

        public JsonResult LoadModelDefLogList(int page = 1, int rows = 10, string selectname = null)
        {
            string name = string.Empty;
            if (!string.IsNullOrWhiteSpace(selectname))
            {
                name = selectname;
            }

            var result = _modelDefinitionService.FindPaginatedModelDefLogs(page, rows, name);

            return Json(result);
        }
        #endregion
    }
}
