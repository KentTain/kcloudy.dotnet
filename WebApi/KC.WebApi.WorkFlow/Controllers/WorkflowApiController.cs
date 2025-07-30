using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using KC.Service;
using KC.Service.DTO.Search;
using KC.Service.DTO.Workflow;
using KC.Service.Workflow;
using KC.Service.Workflow.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KC.WebApi.Workflow.Controllers
{
    [Authorize]
    public class WorkflowApiController : Web.Controllers.WebApiBaseController
    {
        private new const string ServiceName = "KC.WebApi.Workflow.Controllers.WorkflowApiController";
        private IWorkflowProcessService _wfProcessService => ServiceProvider.GetService<IWorkflowProcessService>();
        public WorkflowApiController(
            Tenant tenant, IServiceProvider serviceProvider,
            ILogger<WorkflowApiController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        /// <summary>
        /// 启动流程前，根据流程定义编码及提交的表单数据，获取流程启动执行数据 </br>
        ///     包括：启动后的下一审批环节的任务数据、执行人数据、预定义的回调函数
        /// </summary>
        /// <param name="userId">启动人用户Id</param>
        /// <param name="userDisplayName">启动人姓名</param>
        /// <param name="wfDefCode">启动的流程定义编码</param>
        /// <param name="formData">流程定义中的表单数据</param>
        /// <returns>流程启动执行数据（下一审批环节的任务数据、执行人数据、预定义的回调函数）</returns>
        [HttpPost]
        [Route("GetWorkflowStartExecuteData")]
        public async Task<ServiceResult<WorkflowStartExecuteData>> GetWorkflowStartExecuteData(string userId, string userDisplayName, string wfDefCode, [FromBody] List<WorkflowProFieldDTO> formData)
        {
            return await GetServiceResultAsync(async () =>
            {
                var result = await _wfProcessService.GetStartWorkflowExecutorInfoAsync(wfDefCode, formData, userId, userDisplayName);
                return result;
            });
        }

        /// <summary>
        /// 启动一个流程实例
        /// </summary>
        /// <param name="userId">启动人用户Id</param>
        /// <param name="userDisplayName">启动人姓名</param>
        /// <param name="model">流程定义中的表单数据</param>
        /// <returns>流程实例的编码（可以根据此编码进行后续的查询及操作）</returns>
        [HttpPost]
        [Route("StartWorkflow")]
        public async Task<ServiceResult<string>> StartWorkflow(string userId, string userDisplayName, [FromBody] WorkflowStartExecuteData model)
        {
            return await GetServiceResultAsync(async () =>
            {
                var success = await _wfProcessService.StartWorkflowAsync(model, userId, userDisplayName);
                return success;
            });
        }

        /// <summary>
        /// 审核流程前，根据流程实例编码，获取流程实例的可执行数据 </br>
        ///     包括：提交的表单数据（获取后可使用接口：SubmitWorkflow，进行表单数据更新操作）
        ///         启动后的下一审批环节的任务数据、执行人数据、预定义的回调函数
        /// </summary>
        /// <param name="userId">审核人用户Id</param>
        /// <param name="userDisplayName">审核人姓名</param>
        /// <param name="processCode">流程实例编码</param>
        /// <returns>流程审核执行数据（下一审批环节的任务数据、执行人数据、预定义的回调函数）</returns>
        [HttpGet]
        [Route("GetWorkflowSubmitExecuteData")]
        public async Task<ServiceResult<WorkflowExecuteData>> GetWorkflowSubmitExecuteData(string userId, string userDisplayName, string processCode)
        {
            return await GetServiceResultAsync(async () =>
            {
                var result = await _wfProcessService.GetSubmitWorkflowExecutorInfoAsync(processCode, userId, userDisplayName);
                return result;
            });
        }

        /// <summary>
        /// 处理流程实例（流程过程）：Submit流程，进入下一个流程处理节点
        /// </summary>
        /// <param name="userId">审核人用户Id</param>
        /// <param name="userDisplayName">审核人姓名</param>
        /// <param name="processCode">流程实例编码（启动流程所返回的流程实例编码）</param>
        /// <param name="executeData">
        ///     包括：表单数据（可进行表单数据更新操作）
        ///         审核流程的数据对象
        /// </param>
        /// <returns>是否成功</returns>
        [HttpPost]
        [Route("SubmitWorkflow")]
        public async Task<ServiceResult<bool>> SubmitWorkflow(string userId, string userDisplayName, string processCode, [FromBody] WorkflowExecuteData executeData)
        {
            return await GetServiceResultAsync(async () =>
            {
                if (string.IsNullOrEmpty(executeData.ExecuteUserId))
                    executeData.ExecuteUserId = userId;
                if (string.IsNullOrEmpty(executeData.ExecuteUserName))
                    executeData.ExecuteUserName = userDisplayName;

                var success = await _wfProcessService.SubmitWorkflowAsync(processCode, executeData);
                return success;
            });
        }

        /// <summary>
        /// 获取用户的可执行任务列表
        /// </summary>
        /// <param name="searchDTO">查询条件，包括：用户Id、用户所属角色Id列表、当前用户所属部门Id列表</param>
        /// <returns></returns>
        [HttpPost]
        [Route("LoadUserWorkflowTasks")]
        public async Task<ServiceResult<List<WorkflowProTaskDTO>>> LoadUserWorkflowTasks([FromBody] WorkflowTaskSearchDTO searchDTO)
        {
            return await GetServiceResultAsync(async () =>
            {
                var success = await _wfProcessService.FindUserWorkflowTasksAsync(searchDTO.UserId, searchDTO.RoleIds, searchDTO.OrgCodes);
                return success;
            });
        }

        /// <summary>
        /// 根据流程实例编码，获取流程实例
        /// </summary>
        /// <param name="code">流程实例编码</param>
        /// <returns>流程实例</returns>
        [HttpGet]
        [Route("GetWorkflowProcessByCode")]
        public async Task<ServiceResult<WorkflowProcessDTO>> GetWorkflowProcessByCode(string code)
        {
            return await GetServiceResultAsync(async () =>
            {
                var result = await _wfProcessService.GetWorkflowProcessByCodeAsync(code);
                return result;
            });
        }

        /// <summary>
        /// 获取流程实例的开始任务
        /// </summary>
        /// <param name="code">流程实例的编码</param>
        /// <returns>返回开始任务的双向链表结果数据</returns>
        [HttpGet]
        [Route("GetStartTaskByProcessCode")]
        public async Task<ServiceResult<WorkflowProTaskDTO>> GetStartTaskByProcessCode(string code)
        {
            return await GetServiceResultAsync(async () =>
            {
                var result = await _wfProcessService.GetStartTaskByProcessCodeAsync(code);
                return result;
            });
        }

        /// <summary>
        /// 获取流程实例的当前任务
        /// </summary>
        /// <param name="code">流程实例的编码</param>
        /// <returns>返回当前任务的双向链表结果数据</returns>
        [HttpGet]
        [Route("GetCurrentTaskByProcessCode")]
        public async Task<ServiceResult<WorkflowProTaskDTO>> GetCurrentTaskByProcessCode(string code)
        {
            return await GetServiceResultAsync(async () =>
            {
                var result = await _wfProcessService.GetCurrentTaskByProcessCodeAsync(code);
                return result;
            });
        }

        /// <summary>
        /// 获取流程实例的下一任务
        /// </summary>
        /// <param name="code">流程实例的编码</param>
        /// <returns>返回当前任务的双向链表结果数据</returns>
        [HttpGet]
        [Route("GetNextTaskByProcessCode")]
        public async Task<ServiceResult<WorkflowProTaskDTO>> GetNextTaskByProcessCode(string code)
        {
            return await GetServiceResultAsync(async () =>
            {
                var result = await _wfProcessService.GetNextTaskByProcessCodeAsync(code);
                return result;
            });
        }
    }
}
