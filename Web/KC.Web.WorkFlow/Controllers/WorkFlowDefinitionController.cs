using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Common;
using KC.Framework.Base;
using KC.Framework.Exceptions;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Service.DTO;
using KC.Service.DTO.Workflow;
using KC.Service.Util;
using KC.Service.WebApiService.Business;
using KC.Service.Workflow;
using KC.Service.Workflow.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KC.Web.Workflow.Controllers
{
    /// <summary>
    /// 二级菜单：流程管理/流程定义
    /// </summary>
    public class WorkflowDefinitionController : WorkflowBaseController
    {
        private readonly IWorkflowDefinitionService _wfDefinitionService;
        private readonly IWorkflowProcessService _wfProcessService;
        private readonly IConfigApiService _configApiService;

        public WorkflowDefinitionController(
            Tenant tenant,
            IConfigApiService configApiService,
            IServiceProvider serviceProvider,
            IWorkflowDefinitionService wfDefinitionService,
            IWorkflowProcessService wfProcessService,
            ILogger<WorkflowDefinitionController> logger)
            : base(tenant, serviceProvider, logger)
        {
            _configApiService = configApiService;
            _wfDefinitionService = wfDefinitionService;
            _wfProcessService = wfProcessService;
        }

        /// <summary>
        /// 三级菜单：流程管理/流程定义/流程定义列表
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("流程定义", "流程定义列表", "/WorkflowDefinition/Index",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, 
            SmallIcon = "fa fa-list-alt", AuthorityId = "95C0ABE1-7005-4458-9204-9937FD5DDAC2",
            DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 1, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("流程定义列表", "流程定义列表", "/WorkflowDefinition/Index", 
            "95C0ABE1-7005-4458-9204-9937FD5DDAC2", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 4, IsPage = true, ResultType = ResultType.ActionResult)]
        public ActionResult Index()
        {
            return View();
        }

        #region 流程分类
        public IActionResult LoadCategoryList(string searchKey = "", string searchValue = "")
        {
            var result = new List<WorkflowCategoryDTO>(){
                new WorkflowCategoryDTO()
                {
                    Id = 0,
                    Text = "所有流程定义",
                    Description="所有流程定义",
                    Children = null,
                    Level = 1
                },
                new WorkflowCategoryDTO()
                {
                    Id = -1,
                    Text = "未分类流程定义",
                    Description = "未分配类型的流程定义",
                    Children = null,
                    Level = 1
                }
            };

            string name = string.Empty;
            if (!string.IsNullOrWhiteSpace(searchKey)
                && !string.IsNullOrWhiteSpace(searchValue))
            {
                switch (searchKey.ToLower())
                {
                    case "name":
                        name = searchValue;
                        break;
                }
            }
            var data = _wfDefinitionService.FindCategoryTreesByName(name);
            if (data != null && data.Count() > 0)
                result.AddRange(data);

            return Json(result);
        }

        [AllowAnonymous]
        public IActionResult LoadCategoryTree(int selectedId, bool hasRoot = true, int maxLevel = 3)
        {
            var tree = _wfDefinitionService.FindAllCategoryTrees();
            if (hasRoot)
            {
                var rootMenu = new WorkflowCategoryDTO() { Text = "顶级流程分类", Children = tree, Level = 0 };
                var org = TreeNodeUtil.GetNeedLevelTreeNodeDTO(rootMenu, maxLevel, null, new List<int>() { selectedId });
                return Json(new List<WorkflowCategoryDTO> { org });
            }

            var orgList = TreeNodeUtil.LoadNeedLevelTreeNodeDTO(tree, maxLevel, null, new List<int>() { selectedId });
            return Json(orgList);
        }
        [AllowAnonymous]
        public PartialViewResult GetCategoryForm(int id = 0, int? parentId = null, string parentName = "顶级流程分类")
        {
            var model = new WorkflowCategoryDTO();
            model.IsEditMode = false;
            model.ParentId = parentId;
            model.ParentName = parentName;
            if (id != 0)
            {
                model = _wfDefinitionService.GetCategoryById(id);
                model.IsEditMode = true;
                model.ParentName = model.ParentId == null ? parentName : model.ParentName;
            }

            return PartialView("_categoryForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Web.Extension.PermissionFilter("流程定义列表", "保存流程分类", "/WorkflowDefinition/SaveCategoryForm", 
            "09C4B0CF-2A5A-4C97-8248-0756C9243BD3", DefaultRoleId = RoleConstants.ExecutorManagerRoleId, 
            Order = 1, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult SaveCategoryForm(WorkflowCategoryDTO model)
        {
            if (model.ParentId == 0)
                model.ParentId = null;
            ModelState.Remove("Leaf");
            ModelState.Remove("IsDeleted");
            ModelState.Remove("CreatedDate");
            ModelState.Remove("ModifiedDate");
            return GetServiceJsonResult(() =>
            {
                model.Level = 1;
                if (model.ParentId.HasValue && model.ParentId > 0)
                {
                    var parentCategory = _wfDefinitionService.GetCategoryById(model.ParentId.Value);
                    if (null == parentCategory)
                    {
                        throw new ArgumentException(string.Format("父级ID:{0} 未找到！", model.ParentId));
                    }
                    if (parentCategory.Level >= 4)
                    {
                        throw new ArgumentException(string.Format("父级:{0} 不能作为父级！", model.Text));
                    }

                    model.Level = parentCategory.Level + 1;
                }
                if (model.Id > 0)
                {
                    var Categorychild = _wfDefinitionService.FindCategoryTreesByIds(new List<int> { model.Id });
                    foreach (var item in Categorychild)
                    {
                        if (item.Level.Equals(4))
                        {
                            throw new ArgumentException("此节点为父节点且有等级为4的子级，不可移动到其他父节点下");
                        }
                    }
                }
                else
                {
                    model.CreatedBy = CurrentUserId;
                    model.CreatedName = CurrentUserDisplayName;
                    model.CreatedDate = DateTime.UtcNow;
                }
                
                model.ModifiedBy = CurrentUserId;
                model.ModifiedName = CurrentUserDisplayName;
                model.ModifiedDate = DateTime.UtcNow;
                return _wfDefinitionService.SaveCategory(model, CurrentUserId);
            });

        }

        [Web.Extension.PermissionFilter("流程定义列表", "删除流程分类", "/WorkflowDefinition/RemoveCategory", 
            "363D6D5D-69AC-45E4-83D3-0197C5DB8C99", DefaultRoleId = RoleConstants.ExecutorManagerRoleId, 
            Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult RemoveCategory(int id)
        {
            return GetServiceJsonResult(() =>
            {
                var Category = _wfDefinitionService.GetCategoryById(id);
                if (null == Category)
                {
                    throw new ArgumentException(string.Format("未找到菜单ID:{0}", id));
                }
                if (Category.Children.Any())
                {
                    throw new ArgumentException(string.Format("菜单ID:{0}存在子菜单，无法删除，请先删除子菜单！", id));
                }
                //var Category2 = _wfDefinitionService.GetCategorysWithUsersByOrgId(id);
                //if (Category2.Users.Count > 0)
                //{
                //    throw new BusinessPromptException(string.Format("存在下属员工，无法删除，请先移除下属员工！"));
                //}
                return _wfDefinitionService.RemoveCategory(id);
            });
        }

        [AllowAnonymous]
        public JsonResult ExistCategoryName(int id, string name, string orginalName, bool isEditMode)
        {
            if (isEditMode && name.Equals(orginalName, StringComparison.OrdinalIgnoreCase))
            {
                return Json(false);
            }

            var tname = _wfDefinitionService.ExistCategoryName(id, name);
            return Json(tname);
        }

        #endregion

        #region 流程定义列表
        public JsonResult LoadWorkflowDefinitionList(int page = 1, int rows = 10, int? categoryId = 0, string name = "", WorkflowBusStatus? status = null)
        {
            var result = _wfDefinitionService.FindPagenatedDefinitionsByFilter(page, rows, categoryId, name, status);
            return Json(result);
        }
        public JsonResult LoadWorkflowFieldList(Guid id)
        {
            var result = id != null && id != Guid.Empty 
                ? _wfDefinitionService.FindAllWorkflowFieldsByDefId(id) 
                : new List<WorkflowDefFieldDTO>();
            return Json(result);
        }

        [Web.Extension.PermissionFilter("流程定义列表", "删除流程定义", "/WorkflowDefinition/RemoveWorkflowDefinition",
            "2BC23260-AA28-4EC4-BD3A-F927601E7860", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<IActionResult> RemoveWorkflowDefinition(Guid id)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                return await _wfDefinitionService.RemoveWorkflowDefinitionWithFieldsAsync(id, CurrentUserId, CurrentUserDisplayName);
            });
        }

        #endregion

        #region 流程版本列表
        /// <summary>
        /// 三级菜单：流程管理/流程定义/流程版本列表
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("流程定义", "流程版本列表", "/WorkflowDefinition/WorkflowVerDefinition",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-list-alt", AuthorityId = "8A4DE229-DD41-42F3-8566-83E80E19A680",
            DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 6, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("流程版本列表", "流程版本列表", "/WorkflowDefinition/WorkflowVerDefinition",
            "8A4DE229-DD41-42F3-8566-83E80E19A680", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 5, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult WorkflowVerDefinition()
        {
            return View();
        }

        public JsonResult LoadWorkflowVerDefinitionList(int page = 1, int rows = 20, int? categoryId = 0, string name = null, WorkflowBusStatus? status = null)
        {
            var result = _wfDefinitionService.FindPagenatedWorkflowVerDefsByFilter(page, rows, categoryId, name, status);
            return Json(result);
        }
        public JsonResult LoadWorkflowVerFieldList(Guid id)
        {
            var result = id != null && id != Guid.Empty
                ? _wfDefinitionService.FindAllWorkflowVerFieldsByDefId(id)
                : new List<WorkflowVerDefFieldDTO>();
            return Json(result);
        }

        [Web.Extension.PermissionFilter("流程版本列表", "删除流程版本", "/WorkflowDefinition/RemoveWorkflowVerDefinition",
            "3FC8EBFB-12E1-4CF7-9C1D-FB0C5431B4B4", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 1, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<IActionResult> RemoveWorkflowVerDefinition(Guid id)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                return await _wfDefinitionService.RemoveWorkflowVerDefAsync(id);
            });
        }

        /// <summary>
        /// 三级菜单：流程管理/流程定义/流程版本设计器
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Web.Extension.MenuFilter("流程定义", "流程版本设计器", "/WorkflowDefinition/WorkflowVerDesigner",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-sitemap",
            AuthorityId = "3CCF1D7F-E5C2-4014-A275-BD1FBB8F75C5", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 3, IsExtPage = true, Level = 3)]
        [Web.Extension.PermissionFilter("流程版本设计器", "流程版本设计器", "/WorkflowDefinition/WorkflowVerDesigner",
            "3CCF1D7F-E5C2-4014-A275-BD1FBB8F75C5", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 6, IsPage = true, ResultType = ResultType.ActionResult)]
        public async Task<IActionResult> WorkflowVerDesigner(Guid id)
        {
            var model = await _wfDefinitionService.GetWorkflowVerDefinitionDetailAsync(id);
            return View("WorkflowVerDesigner", model);
        }
        #endregion

        #region 流程基本信息

        /// <summary>
        /// 三级菜单：流程管理/流程定义/流程基本信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Web.Extension.MenuFilter("流程定义", "流程基本信息", "/WorkflowDefinition/GetWorkflowDefinitionForm",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-sitemap", AuthorityId = "231F8FE6-1B8A-415B-B902-DD4EB6D9FF64",
            DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 2, IsExtPage = true, Level = 3)]
        [Web.Extension.PermissionFilter("流程基本信息", "流程基本信息", "/WorkflowDefinition/GetWorkflowDefinitionForm",
            "231F8FE6-1B8A-415B-B902-DD4EB6D9FF64", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 7, IsPage = true, ResultType = ResultType.ActionResult)]
        public async Task<IActionResult> GetWorkflowDefinitionForm(Guid id, int? categoryId = null)
        {
            var model = new WorkflowDefinitionDTO();
            if (id != Guid.Empty)
            {
                model = await _wfDefinitionService.GetWorkflowDefinitionDetailAsync(id);
                model.IsEditMode = true;
                ViewBag.PositionLevels = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<WorkflowBusStatus>((int)model.Status);
            }
            else
            {
                model.IsEditMode = false;
                model.Id = Guid.NewGuid();
                model.CategoryId = categoryId;
                model.Code = _configApiService.GetSeedCodeByName("WorkflowCode") ?? StringExtensions.GenerateStringID();
                model.Version = _configApiService.GetSeedCodeByName("WorkflowVersion") ?? StringExtensions.GenerateStringID();
                ViewBag.PositionLevels = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<WorkflowBusStatus>();
            }

            ViewBag.SelectCategoryId = categoryId;
            ViewBag.BusinessTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<BusinessType>();
            ViewBag.DataTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<AttributeDataType>();

            return View("WorkflowDefinitionForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Web.Extension.PermissionFilter("流程基本信息", "保存流程基本信息", "/WorkflowDefinition/SaveWorkflowBasicInfo",
            "4A5ADE5F-19F8-4DC7-8731-850605BFCD39", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 1, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<IActionResult> SaveWorkflowBasicInfo(WorkflowDefinitionDTO model)
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

                return await _wfDefinitionService.SaveWorkflowDefinitionWithFieldsAsync(model);
            });
        }

        [Web.Extension.PermissionFilter("流程基本信息", "删除流程表单字段", "/WorkflowDefinition/RemoveWorkflowField",
            "6BDD72EA-9B03-4FC1-832F-24F8AD351A21", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult RemoveWorkflowField(int id)
        {
            return GetServiceJsonResult(() =>
            {
                return _wfDefinitionService.RemoveWorkflowFieldAsync(id);
            });
        }


        #endregion

        #region 流程设计器
        /// <summary>
        /// 三级菜单：流程管理/流程定义/流程设计器
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Web.Extension.MenuFilter("流程定义", "流程设计器", "/WorkflowDefinition/WorkflowDesigner",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-sitemap",
            AuthorityId = "C3E245E2-50A4-48AB-A88B-F64BFCF03FC9", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 3, IsExtPage = true, Level = 3)]
        [Web.Extension.PermissionFilter("流程设计器", "流程设计器", "/WorkflowDefinition/WorkflowDesigner",
            "C3E245E2-50A4-48AB-A88B-F64BFCF03FC9", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 8, IsPage = true, ResultType = ResultType.ActionResult)]
        public async Task<IActionResult> WorkflowDesigner(Guid id)
        {
            var model = await _wfDefinitionService.GetWorkflowDefinitionDetailAsync(id);
            return View("WorkflowDesigner", model);
        }

        public async Task<IActionResult> GetWorkflowStartNode(Guid id)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                return await _wfDefinitionService.GetStartNodeByWfDefIdAsync(id, CurrentUserId, CurrentUserDisplayName);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Web.Extension.PermissionFilter("流程设计器", "保存流程设计数据", "/WorkflowDefinition/SaveWorkflowDefinition",
            "776F9C78-ADAF-44AB-9B21-A3295645E68A", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 1, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<IActionResult> SaveWorkflowDefinition(Guid wfDefId, string wfDefName, [FromBody] List<WorkflowDefNodeDTO> model)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                var success = await _wfDefinitionService.SaveWorkflowNodesAsync(wfDefId, wfDefName, model, CurrentUserId, CurrentUserDisplayName);
                return success;
            });
        }
        #endregion

        #region 流程验证

        /// <summary>
        /// 三级菜单：流程管理/流程定义/流程验证
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Web.Extension.MenuFilter("流程定义", "流程验证", "/WorkflowDefinition/WorkflowValidator",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-sitemap",
            AuthorityId = "35963A10-DD4D-4C04-974D-FC7E6E1438A4", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 4, IsExtPage = true, Level = 3)]
        [Web.Extension.PermissionFilter("流程验证", "流程验证", "/WorkflowDefinition/WorkflowValidator",
            "35963A10-DD4D-4C04-974D-FC7E6E1438A4", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 9, IsPage = true, ResultType = ResultType.ActionResult)]
        public async Task<IActionResult> WorkflowValidator(Guid id)
        {
            var model = await _wfDefinitionService.GetWorkflowDefinitionDetailAsync(id);
            model.ExecuteData = new WorkflowStartExecuteData();
            return View("WorkflowValidator", model);
        }

        [HttpPost]
        public async Task<IActionResult> GetWorkflowTaskStartData(string wfDefCode, [FromBody] List<WorkflowProFieldDTO> formData)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                var executeData = await _wfProcessService.GetStartWorkflowExecutorInfoAsync(wfDefCode, formData, CurrentUserId, CurrentUserDisplayName);
                return executeData;
            });
        }

        [HttpPost]
        public async Task<IActionResult> WorkflowTaskStartForm(string wfDefCode, [FromForm] List<WorkflowProFieldDTO> formData)
        {
            var executeData = await _wfProcessService.GetStartWorkflowExecutorInfoAsync(wfDefCode, formData, CurrentUserId, CurrentUserDisplayName);
            return View("WorkflowStartForm", executeData);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Web.Extension.PermissionFilter("流程验证", "发起流程验证", "/WorkflowDefinition/StartWorkflowValidator",
            "DB541D4E-67BF-4DDF-94C3-F76089EC2971", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 1, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<IActionResult> StartWorkflowValidator([FromBody] WorkflowStartExecuteData model)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                var success = await _wfProcessService.StartWorkflowAsync(model, CurrentUserId, CurrentUserDisplayName, true);
                return success;
            });
        }
        #endregion

        #region 流程日志
        /// <summary>
        /// 三级菜单：流程管理/流程定义/流程定义日志
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("流程定义", "流程定义日志", "/WorkflowDefinition/WorkflowDefLog",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, 
            SmallIcon = "fa fa-list-alt", AuthorityId = "AD775CE1-BDB7-4111-8AE8-DF98C0EA398D",
            DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 7, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("流程定义日志", "流程定义日志", "/WorkflowDefinition/WorkflowDefLog", 
            "AD775CE1-BDB7-4111-8AE8-DF98C0EA398D", DefaultRoleId = RoleConstants.ExecutorManagerRoleId, 
            Order = 10, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult WorkflowDefLog()
        {
            return View();
        }

        public JsonResult LoadWorkflowDefLogList(int page = 1, int rows = 10, string selectname = null)
        {
            string name = string.Empty;
            if (!string.IsNullOrWhiteSpace(selectname))
            {
                name = selectname;
            }

            var result = _wfDefinitionService.FindPaginatedWorkflowDefLogs(page, rows, name);

            return Json(result);
        }
        #endregion
    }
}