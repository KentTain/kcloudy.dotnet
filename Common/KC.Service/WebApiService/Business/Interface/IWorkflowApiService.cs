using KC.Service.DTO.Search;
using KC.Service.DTO.Workflow;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KC.Service.WebApiService.Business
{
    public interface IWorkflowApiService
    {
        /// <summary>
        /// 启动流程前，根据流程定义编码及提交的表单数据，获取流程启动执行数据 </br>
        ///     包括：启动后的下一审批环节的任务数据、执行人数据、预定义的回调函数
        /// </summary>
        /// <param name="userId">启动人用户Id</param>
        /// <param name="userDisplayName">启动人姓名</param>
        /// <param name="wfDefCode">启动的流程定义编码</param>
        /// <param name="formData">流程定义中的表单数据</param>
        /// <returns>流程启动执行数据（下一审批环节的任务数据、执行人数据、预定义的回调函数）</returns>
        Task<WorkflowStartExecuteData> GetWorkflowStartExecuteDataAsync(string userId, string userDisplayName, string wfDefCode, List<WorkflowProFieldDTO> formData);
        /// <summary>
        /// 启动一个流程实例
        /// </summary>
        /// <param name="userId">启动人用户Id</param>
        /// <param name="userDisplayName">启动人姓名</param>
        /// <param name="wfDefCode">流程定义编码</param>
        /// <param name="model">流程定义中的表单数据</param>
        /// <returns>流程实例的编码（可以根据此编码进行后续的查询及操作）</returns>
        Task<string> StartWorkflowAsync(string userId, string userDisplayName, WorkflowExecuteData model);

        /// <summary>
        /// 审核流程前，根据流程实例编码，获取流程实例的可执行数据 </br>
        ///     包括：提交的表单数据（获取后可使用接口：SubmitWorkflow，进行表单数据更新操作）
        ///         启动后的下一审批环节的任务数据、执行人数据、预定义的回调函数
        /// </summary>
        /// <param name="userId">审核人用户Id</param>
        /// <param name="userDisplayName">审核人姓名</param>
        /// <param name="processCode">流程实例编码</param>
        /// <returns>流程审核执行数据（下一审批环节的任务数据、执行人数据、预定义的回调函数）</returns>
        Task<WorkflowExecuteData> GetWorkflowSubmitExecuteDataAsync(string userId, string userDisplayName, string processCode);
        /// <summary>
        /// 处理流程实例（流程过程）：Submit流程，进入下一个流程处理节点
        /// </summary>
        /// <param name="userId">审核人用户Id</param>
        /// <param name="userDisplayName">审核人姓名</param>
        /// <param name="processCode">流程实例编码（启动流程所返回的流程实例编码）</param>
        /// <param name="model">审核流程的数据对象</param>
        /// <returns>是否成功</returns>
        Task<bool> SubmitWorkflowAsync(string userId, string userDisplayName, string processCode, WorkflowExecuteData model);

        /// <summary>
        /// 获取用户的可执行任务列表
        /// </summary>
        /// <param name="model">查询条件，包括：用户Id、用户所属角色Id列表、当前用户所属部门Id列表</param>
        /// <returns></returns>
        Task<List<WorkflowProTaskDTO>> LoadUserWorkflowTasksAsync(WorkflowTaskSearchDTO model);

        /// <summary>
        /// 根据流程实例编码，获取流程实例
        /// </summary>
        /// <param name="code">流程实例编码</param>
        /// <returns>流程实例</returns>
        Task<WorkflowProcessDTO> GetWorkflowProcessByCodeAsync(string code);
        /// <summary>
        /// 获取流程实例的开始任务
        /// </summary>
        /// <param name="code">流程实例的编码</param>
        /// <returns>返回开始任务的双向链表结果数据</returns>
        Task<WorkflowProTaskDTO> GetStartTaskByProcessCodeAsync(string code);
        /// <summary>
        /// 获取流程实例的当前任务
        /// </summary>
        /// <param name="code">流程实例的编码</param>
        /// <returns>返回当前任务的双向链表结果数据</returns>
        Task<WorkflowProTaskDTO> GetCurrentTaskByProcessCodeAsync(string code);
        /// <summary>
        /// 获取流程实例的下一任务
        /// </summary>
        /// <param name="code">流程实例的编码</param>
        /// <returns>返回当前任务的双向链表结果数据</returns>
        Task<WorkflowProTaskDTO> GetNextTaskByProcessCodeAsync(string code);
    }
}