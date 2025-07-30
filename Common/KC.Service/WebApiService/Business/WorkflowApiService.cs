using System.Collections.Generic;
using System.Threading.Tasks;
using KC.Common;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Service.Base;
using KC.Service.DTO.Search;
using KC.Service.DTO.Workflow;

namespace KC.Service.WebApiService.Business
{
    public class WorkflowApiService : IdSrvOAuth2ClientRequestBase, IWorkflowApiService
    {
        private const string _serviceName = "KC.Service.WebApiService.Business.WorkflowApiService";

        public WorkflowApiService(
            Tenant tenant,
            System.Net.Http.IHttpClientFactory httpClient,
            Microsoft.Extensions.Logging.ILogger<WorkflowApiService> logger)
            : base(tenant ?? TenantConstant.DbaTenantApiAccessInfo, httpClient, logger)
        {
        }

        /// <summary>
        /// 租户WorkFlow接口地址：http://[tenantName].flow.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:7001/api/
        /// </summary>
        private string _workflowServerUrl
        {
            get
            {
                if (string.IsNullOrEmpty(GlobalConfig.WorkflowWebDomain))
                    return null;

                return GlobalConfig.WorkflowWebDomain.Replace(TenantConstant.SubDomain, Tenant.TenantName) + "api/";
            }
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
        public async Task<WorkflowStartExecuteData> GetWorkflowStartExecuteDataAsync(string userId, string userDisplayName, string wfDefCode, List<WorkflowProFieldDTO> formData)
        {
            var jsonData = SerializeHelper.ToJson(formData);
            ServiceResult<WorkflowStartExecuteData> result = null;
            await WebSendPostAsync<ServiceResult<WorkflowStartExecuteData>>(
                _serviceName + ".GetWorkflowStartExecuteData",
                _workflowServerUrl + "WorkflowApi/GetWorkflowStartExecuteData?userId=" + userId + "&userDisplayName=" + userDisplayName + "&wfDefCode=" + wfDefCode,
                ApplicationConstant.WorkflowScope,
                jsonData,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<WorkflowStartExecuteData>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success)
            {
                return result.Result;
            }

            return null;
        }

        /// <summary>
        /// 启动一个流程实例
        /// </summary>
        /// <param name="userId">启动人用户Id</param>
        /// <param name="userDisplayName">启动人姓名</param>
        /// <param name="wfDefCode">流程定义编码</param>
        /// <param name="model">流程定义中的表单数据</param>
        /// <returns>流程实例的编码（可以根据此编码进行后续的查询及操作）</returns>
        public async Task<string> StartWorkflowAsync(string userId, string userDisplayName, WorkflowExecuteData model)
        {
            var jsonData = SerializeHelper.ToJson(model);
            ServiceResult<string> result = null;
            await WebSendPostAsync<ServiceResult<string>>(
                _serviceName + ".StartWorkflow",
                _workflowServerUrl + "WorkflowApi/StartWorkflow?userId=" + userId + "&userDisplayName=" + userDisplayName,
                ApplicationConstant.WorkflowScope,
                jsonData,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<string>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success)
            {
                return result.Result;
            }

            return null;
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
        public async Task<WorkflowExecuteData> GetWorkflowSubmitExecuteDataAsync(string userId, string userDisplayName, string processCode)
        {
            ServiceResult<WorkflowExecuteData> result = null;
            await WebSendGetAsync<ServiceResult<WorkflowExecuteData>>(
                _serviceName + ".GetWorkflowSubmitExecuteData",
                _workflowServerUrl + "WorkflowApi/GetWorkflowSubmitExecuteData?userId=" + userId + "&userDisplayName=" + userDisplayName + "&processCode=" + processCode,
                ApplicationConstant.WorkflowScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<WorkflowExecuteData>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success)
            {
                return result.Result;
            }

            return null;
        }

        /// <summary>
        /// 处理流程实例（流程过程）：Submit流程，进入下一个流程处理节点
        /// </summary>
        /// <param name="userId">审核人用户Id</param>
        /// <param name="userDisplayName">审核人姓名</param>
        /// <param name="processCode">流程实例编码（启动流程所返回的流程实例编码）</param>
        /// <param name="model">审核流程的数据对象</param>
        /// <returns>是否成功</returns>
        public async Task<bool> SubmitWorkflowAsync(string userId, string userDisplayName, string processCode, WorkflowExecuteData model)
        {
            var jsonData = SerializeHelper.ToJson(model);
            ServiceResult<bool> result = null;
            await WebSendPostAsync<ServiceResult<bool>>(
                _serviceName + ".SubmitWorkflow",
                _workflowServerUrl + "WorkflowApi/SubmitWorkflow?userId=" + userId + "&userDisplayName=" + userDisplayName + "&processCode=" + processCode,
                ApplicationConstant.WorkflowScope,
                jsonData,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<bool>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success)
            {
                return result.Result;
            }

            return false;
        }

        /// <summary>
        /// 获取用户的可执行任务列表
        /// </summary>
        /// <param name="model">查询条件，包括：用户Id、用户所属角色Id列表、当前用户所属部门Id列表</param>
        /// <returns></returns>
        public async Task<List<WorkflowProTaskDTO>> LoadUserWorkflowTasksAsync(WorkflowTaskSearchDTO model)
        {
            var jsonData = SerializeHelper.ToJson(model);
            ServiceResult<List<WorkflowProTaskDTO>> result = null;
            await WebSendPostAsync<ServiceResult<List<WorkflowProTaskDTO>>>(
                _serviceName + ".LoadUserWorkflowTasks",
                _workflowServerUrl + "WorkflowApi/LoadUserWorkflowTasks",
                ApplicationConstant.WorkflowScope,
                jsonData,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<WorkflowProTaskDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success)
            {
                return result.Result;
            }

            return null;
        }

        /// <summary>
        /// 根据流程实例编码，获取流程实例
        /// </summary>
        /// <param name="code">流程实例编码</param>
        /// <returns>流程实例</returns>
        public async Task<WorkflowProcessDTO> GetWorkflowProcessByCodeAsync(string code)
        {
            ServiceResult<WorkflowProcessDTO> result = null;
            await WebSendGetAsync<ServiceResult<WorkflowProcessDTO>>(
                _serviceName + ".GetWorkflowProcessByCode",
                _workflowServerUrl + "WorkflowApi/GetWorkflowProcessByCode?code=" + code,
                ApplicationConstant.WorkflowScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<WorkflowProcessDTO>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success)
            {
                return result.Result;
            }

            return null;
        }

        /// <summary>
        /// 获取流程实例的开始任务
        /// </summary>
        /// <param name="code">流程实例的编码</param>
        /// <returns>返回开始任务的双向链表结果数据</returns>
        public async Task<WorkflowProTaskDTO> GetStartTaskByProcessCodeAsync(string code)
        {
            ServiceResult<WorkflowProTaskDTO> result = null;
            await WebSendGetAsync<ServiceResult<WorkflowProTaskDTO>>(
                _serviceName + ".GetStartTaskByProcessCode",
                _workflowServerUrl + "WorkflowApi/GetStartTaskByProcessCode?code=" + code,
                ApplicationConstant.WorkflowScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<WorkflowProTaskDTO>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success)
            {
                return result.Result;
            }

            return null;
        }

        /// <summary>
        /// 获取流程实例的当前任务
        /// </summary>
        /// <param name="code">流程实例的编码</param>
        /// <returns>返回当前任务的双向链表结果数据</returns>
        public async Task<WorkflowProTaskDTO> GetCurrentTaskByProcessCodeAsync(string code)
        {
            ServiceResult<WorkflowProTaskDTO> result = null;
            await WebSendGetAsync<ServiceResult<WorkflowProTaskDTO>>(
                _serviceName + ".GetCurrentTaskByProcessCode",
                _workflowServerUrl + "WorkflowApi/GetCurrentTaskByProcessCode?code=" + code,
                ApplicationConstant.WorkflowScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<WorkflowProTaskDTO>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success)
            {
                return result.Result;
            }

            return null;
        }

        /// <summary>
        /// 获取流程实例的下一任务
        /// </summary>
        /// <param name="code">流程实例的编码</param>
        /// <returns>返回当前任务的双向链表结果数据</returns>
        public async Task<WorkflowProTaskDTO> GetNextTaskByProcessCodeAsync(string code)
        {
            ServiceResult<WorkflowProTaskDTO> result = null;
            await WebSendGetAsync<ServiceResult<WorkflowProTaskDTO>>(
                _serviceName + ".GetNextTaskByProcessCode",
                _workflowServerUrl + "WorkflowApi/GetNextTaskByProcessCode?code=" + code,
                ApplicationConstant.WorkflowScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<WorkflowProTaskDTO>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success)
            {
                return result.Result;
            }

            return null;
        }
    }
}
