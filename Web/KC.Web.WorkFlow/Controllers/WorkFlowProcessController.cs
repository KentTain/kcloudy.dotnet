using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Common;
using KC.Enums.Workflow;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Service.DTO;
using KC.Service.DTO.Workflow;
using KC.Service.Workflow;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KC.Web.Workflow.Controllers
{
    /// <summary>
    /// 二级菜单：流程管理/流程任务中心
    /// </summary>
    /// <returns></returns>
    
    public class WorkflowProcessController : WorkflowBaseController
    {
        private IWorkflowProcessService _wfProcessService;

        public WorkflowProcessController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            IWorkflowProcessService wfProcessService,
            ILogger<WorkflowProcessController> logger)
            : base(tenant, serviceProvider, logger)
        {
            _wfProcessService = wfProcessService;
        }

        #region 二级级菜单：流程任务中心
        /// <summary>
        /// 二级级菜单：流程管理/流程任务中心
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("流程管理", "流程任务中心", "/WorkflowProcess/Index", Version = TenantConstant.DefaultVersion, 
            TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-list-alt", 
            AuthorityId = "1FC606EC-5C83-4375-B805-B6FEC8C5D02E", DefaultRoleId = RoleConstants.ExecutorManagerRoleId, 
            Order = 3, IsExtPage = false, Level = 2)]
        [Web.Extension.PermissionFilter("流程任务中心", "流程任务中心", "/WorkflowProcess/Index", 
            "1FC606EC-5C83-4375-B805-B6FEC8C5D02E", DefaultRoleId = RoleConstants.ExecutorManagerRoleId, 
            Order = 11, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult Index()
        {
            ViewBag.StatusList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum(null, new List<WorkflowTaskStatus>() { WorkflowTaskStatus.UnProcess });
            return View();
        }

        public JsonResult LoadWfProcessTaskList(int page, int rows, int? categoryId = 0, string nodeName = null, string userName = null, WorkflowTaskStatus? status = null)
        {
            var result = _wfProcessService.FindPagenatedWorkflowTasksByFilter(page, rows, categoryId, nodeName, userName, status);
            return Json(result);
        }

        #endregion

        #region 二级级菜单：我的待办列表、流程任务详情
        /// <summary>
        /// 二级菜单：流程管理/我的待办列表
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("流程管理", "我的待办列表", "/WorkflowProcess/MyTaskList",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-list-alt", AuthorityId = "1F763C7E-FC0B-42DE-8623-9F4A883D1A35",
            DefaultRoleId = RoleConstants.DefaultRoleId, Order = 4, IsExtPage = true, Level = 2)]
        [Web.Extension.PermissionFilter("我的待办列表", "我的待办列表", "/WorkflowProcess/MyTaskList",
            "1F763C7E-FC0B-42DE-8623-9F4A883D1A35", DefaultRoleId = RoleConstants.DefaultRoleId,
            Order = 12, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult MyTaskList(WorkflowTaskStatus? status)
        {
            ViewBag.selectStatus = status.HasValue ? ((int)status.Value).ToString() : string.Empty;
            ViewBag.StatusList = status.HasValue
                ? KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum(new List<int>() { (int)status.Value }, new List<WorkflowTaskStatus>() { WorkflowTaskStatus.UnProcess })
                : KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum(null, new List<WorkflowTaskStatus>() { WorkflowTaskStatus.UnProcess });

            return View();
        }

        public JsonResult LoadMyTasks(int page, int rows, string title, WorkflowTaskStatus? status)
        {
            var result = _wfProcessService.FindPagenatedMyTasksByFilter(page, rows, CurrentUserId, title, status);
            return Json(result);
        }

        /// <summary>
        /// 二级级菜单：流程管理/流程任务详情（系统默认角色)
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("流程管理", "流程任务详情", "/WorkflowProcess/WorkflowTaskInfo",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-list-alt", 
            AuthorityId = "2F79D503-50B1-414C-B689-A79F5613F425", DefaultRoleId = RoleConstants.DefaultRoleId, 
            Order = 5, IsExtPage = true, Level = 2)]
        [Web.Extension.PermissionFilter("流程任务详情", "流程任务详情", "/WorkflowProcess/WorkflowTaskInfo", 
            "2F79D503-50B1-414C-B689-A79F5613F425", DefaultRoleId = RoleConstants.DefaultRoleId, 
            Order = 13, IsPage = true, ResultType = ResultType.ActionResult)]
        public async Task<IActionResult> WorkflowTaskInfo(string code)
        {
            var model = await _wfProcessService.GetWorkflowProcessByCodeAsync(code);
            var currentTask = model.Tasks.FirstOrDefault(m => m.TaskStatus == Service.Enums.Workflow.WorkflowTaskStatus.Process);
            var allTaskExecutes = model.Tasks.SelectMany(m => m.TaskExecutes).ToList();
            allTaskExecutes.ForEach(m => { m.Task = null;});
            ViewBag.CurrentTaskId = currentTask?.Id;
            ViewBag.CurrentTaskCode = currentTask?.Code;
            ViewBag.CurrentTaskName = currentTask?.Name;
            ViewBag.TaskExecuteData = SerializeHelper.ToJson(allTaskExecutes, true);
            return View(model);
        }

        //流程实例的表单数据
        public JsonResult LoadWfProcessFieldList(Guid id)
        {
            var result = id != Guid.Empty
                ? _wfProcessService.FindAllWfProcessFieldsByProcessId(id)
                : new List<WorkflowProFieldDTO>();
            return Json(result);
        }

        #region 流程审批
        public async Task<PartialViewResult> GetWorkflowTaskAuditForm(string wfDefCode, Guid taskId, string taskCode, string taskName)
        {
            var executeData = await _wfProcessService.GetSubmitWorkflowExecutorInfoAsync(wfDefCode, CurrentUserId, CurrentUserDisplayName);
            return PartialView("_workflowAuditForm", executeData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Web.Extension.PermissionFilter("流程任务详情", "流程任务审批", "/WorkflowProcess/AuditWorkflowTask",
            "4BA0B9E6-C5D2-49FA-A009-2438C189AEAA", DefaultRoleId = RoleConstants.DefaultRoleId,
            Order = 14, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<IActionResult> AuditWorkflowTask(string code, WorkflowExecuteData executeData)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                executeData.ExecuteUserId = CurrentUserId;
                executeData.ExecuteUserName = CurrentUserDisplayName;
                return await _wfProcessService.SubmitWorkflowAsync(code, executeData);
            });
        }

        #endregion

        //流程图所需的开始任务的流程双向链表数据
        public async Task<IActionResult> GetWorkflowTaskStartNode(string code)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                return await _wfProcessService.GetStartTaskByProcessCodeAsync(code);
            });
        }
        //流程审批记录数据
        public JsonResult LoadWfProcessTaskExecuteList(int page, int rows, Guid? processId = null, Guid? taskId = null, string name = null, Enums.Workflow.WorkflowTaskStatus? status = null)
        {
            var result = _wfProcessService.FindPagenatedWorkflowTaskExecutesByFilter(page, rows, processId, taskId, name, status);
            return Json(result);
        }
        #endregion
    }
}