using AutoMapper;
using KC.Common;
using KC.DataAccess.Workflow.Repository;
using KC.Database.EFRepository;
using KC.Database.IRepository;
using KC.Enums.Workflow;
using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Model.Workflow;
using KC.Service.DTO;
using KC.Service.DTO.Search;
using KC.Service.Workflow.DTO;
using KC.Service.EFService;
using KC.Service.Util;
using KC.Service.WebApiService.Business;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KC.Service.DTO.Workflow;
using KC.Service.DTO.Account;
using KC.Service.WebApiService;

namespace KC.Service.Workflow
{
    public interface IWorkflowProcessService : IEFService
    {
        #region 启动及执行流程
        /// <summary>
        /// 获取流程开始后的任务审批相关数据（审批人）
        /// </summary>
        /// <param name="wfDefCode">流程定义编码</param>
        /// <param name="formData">流程定义中的表单数据</param>
        /// <param name="currentUserId">表单提交人</param>
        /// <param name="currentUserName">表单提交人</param>
        /// <returns></returns>
        Task<WorkflowStartExecuteData> GetStartWorkflowExecutorInfoAsync(string wfDefCode, List<WorkflowProFieldDTO> formData, string currentUserId, string currentUserName);

        /// <summary>
        /// 启动一个流程实例（流程过程）
        /// </summary>
        /// <param name="wfDefCode">流程定义编码</param>
        /// <param name="formData">流程定义中的表单数据</param>
        /// <returns>流程实例编码（Code）</returns>
        Task<string> StartWorkflowAsync(WorkflowStartExecuteData formData, string currentUserId, string currentUserName, bool isTest = false);

        /// <summary>
        /// 获取流程下一个任务的审批相关数据（审批人）
        /// </summary>
        /// <param name="processCode">流程实例编码</param>
        /// <param name="executeUserId">流程执行人</param>
        /// <param name="executeUserName">流程执行人</param>
        /// <returns></returns>
        Task<WorkflowExecuteData> GetSubmitWorkflowExecutorInfoAsync(string processCode, string executeUserId, string executeUserName);

        /// <summary>
        /// 处理流程实例（流程过程）：Submit流程，进入下一个流程处理节点
        /// </summary>
        /// <param name="code">流程实例编码</param>
        /// <param name="executeData">流程实例传入的表单数据</param>
        /// <returns>是否处理成功</returns>
        Task<bool> SubmitWorkflowAsync(string processCode, WorkflowExecuteData executeData);
        #endregion

        #region 可执行任务列表
        /// <summary>
        /// 获取所有的可执行任务
        /// </summary>
        /// <param name="pageIndex">分页页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="nodeName">节点名称</param>
        /// <param name="userName">执行人姓名</param>
        /// <param name="categoryId">所属流程实例的分类</param>
        /// <returns></returns>
        PaginatedBaseDTO<WorkflowProTaskDTO> FindPagenatedWorkflowTasksByFilter(int pageIndex, int pageSize, int? categoryId, string nodeName, string userName, WorkflowTaskStatus? status);
        /// <summary>
        /// 获取当前用户的可执行任务
        /// </summary>
        /// <param name="pageIndex">分页页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="userId">当前用户的UserId</param>
        /// <param name="taskName">任务名称</param>
        /// <param name="status">任务状态</param>
        /// <returns></returns>
        PaginatedBaseDTO<WorkflowProTaskDTO> FindPagenatedMyTasksByFilter(int pageIndex, int pageSize, string userId, string taskName, WorkflowTaskStatus? status);
        /// <summary>
        /// 获取用户的可执行任务列表
        /// </summary>
        /// <param name="currentUserId">当前用户Id</param>
        /// <param name="currentUserRoleIds">当前用户所属角色Id列表</param>
        /// <param name="currentUserOrgIds">当前用户所属部门Id列表</param>
        /// <returns></returns>
        Task<List<WorkflowProTaskDTO>> FindUserWorkflowTasksAsync(string currentUserId, List<string> currentUserRoleIds, List<string> currentUserOrgCodes);

        #endregion

        #region 获取流程实例及相关的任务
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
        /// <returns>返回下一任务的双向链表结果数据</returns>
        Task<WorkflowProTaskDTO> GetNextTaskByProcessCodeAsync(string code);
        /// <summary>
        /// 根据传入以流程链数据结构的任务对象，获取其后的所有节点信息数据</br>
        ///     结合获取流程链数据的方法使用：GetStartTaskByProcessCodeAsync、GetCurrentTaskByProcessCodeAsync</br>
        ///     结果实例：任务节点【名称：node4-task-2，状态：处理中，类型：单人审批】-->任务节点【名称：node5-task-2，状态：未处理，类型：单人审批】-->结束节点【名称：node7-end-2，状态：未处理，类型：单人审批】
        /// </summary>
        /// <param name="startTask">
        /// 传入的任务数据(流程链数据结构)</br>
        /// 结合获取流程链数据的方法使用：GetStartTaskByProcessCodeAsync、GetCurrentTaskByProcessCodeAsync
        /// </param>
        /// <returns>返回实例：任务节点【名称：node4-task-2，状态：处理中，类型：单人审批】-->任务节点【名称：node5-task-2，状态：未处理，类型：单人审批】-->结束节点【名称：node7-end-2，状态：未处理，类型：单人审批】 </returns>
        string PrintProcessLinkedTask(WorkflowProTaskDTO startTask);
        #endregion 

        #region 获取流程实例相关表单及审批记录
        /// <summary>
        /// 根据流程实例Id，获取所有的表单数据
        /// </summary>
        /// <param name="processId">流程实例Id</param>
        /// <returns></returns>
        List<WorkflowProFieldDTO> FindAllWfProcessFieldsByProcessId(Guid processId);

        /// <summary>
        /// 获取流程实例的审批记录
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="processId">流程实例Id</param>
        /// <param name="taskId">任务编码</param>
        /// <param name="userName">执行人姓名</param>
        /// <param name="status">任务状态</param>
        /// <returns></returns>
        PaginatedBaseDTO<WorkflowProTaskExecuteDTO> FindPagenatedWorkflowTaskExecutesByFilter(int pageIndex, int pageSize, Guid? processId, Guid? taskId, string userName, WorkflowTaskStatus? status);
        #endregion

        #region 删除流程任务
        Task<bool> RemoveWorkflowProcessWithTasksByIdAsync(Guid wfDefId);

        Task<bool> RemoveWorkflowProcessWithTasksByCodeAsync(string code);
        #endregion

    }

    public class WorkflowProcessService : EFServiceBase, IWorkflowProcessService
    {
        private readonly IMapper _mapper;
        private readonly IOAuth2ClientCommonService _oAuth2Client;

        #region Repository
        /// <summary>
        /// ComAccountUnitOfWorkContext
        ///     use AccountUnitOfWorkContext.Commit() to save the context change(transaction).
        /// </summary>
        protected EFUnitOfWorkContextBase _unitOfWorkContext { get; private set; }

        private readonly IConfigApiService _configApiService;
        private readonly IAccountApiService _accountApiService;

        private readonly IWorkflowDefinitionRepository _wfDefinitionRepository;
        private readonly IDbRepository<WorkflowDefLog> _wfDefLogRepository;

        private readonly IWorkflowProcessRepository _wfProcessRepository;
        private readonly IWorkflowProTaskRepository _wfProTaskRepository;
        private readonly IDbTreeRepository<WorkflowProField> _wfProFieldRepository;
        private readonly IDbRepository<WorkflowProTaskRule> _wfProTaskRuleRepository;
        private readonly IWorkflowProTaskExecuteRepository _wfProTaskExecuteRepository;
        private readonly IDbRepository<WorkflowProRequestLog> _wfProRequestLogRepository;

        public WorkflowProcessService(
            Tenant tenant,
            IMapper mapper,
            IOAuth2ClientCommonService oAuth2Client,
            IConfigApiService configApiService,
            IAccountApiService accountApiService,
            EFUnitOfWorkContextBase unitOfWorkContext,

            IWorkflowDefinitionRepository wfDefinitionRepository,
            IDbRepository<WorkflowDefLog> wfDefLogRepository,

            IWorkflowProcessRepository wfProcessRepository,
            IWorkflowProTaskRepository wfProTaskRepository,
            IDbTreeRepository<WorkflowProField> wfProFieldRepository,
            IDbRepository<WorkflowProTaskRule> wfProTaskRuleRepository,
            IWorkflowProTaskExecuteRepository wfProTaskExecuteRepository,
            IDbRepository<WorkflowProRequestLog> wfProRequestLogRepository,

            System.Net.Http.IHttpClientFactory clientFactory,
            ILogger<WorkflowProcessService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _oAuth2Client = oAuth2Client;
            _unitOfWorkContext = unitOfWorkContext;
            _configApiService = configApiService;
            _accountApiService = accountApiService;

            _wfDefinitionRepository = wfDefinitionRepository;
            _wfDefLogRepository = wfDefLogRepository;

            _wfProcessRepository = wfProcessRepository;
            _wfProFieldRepository = wfProFieldRepository;
            _wfProTaskRepository = wfProTaskRepository;
            _wfProTaskRuleRepository = wfProTaskRuleRepository;
            _wfProTaskExecuteRepository = wfProTaskExecuteRepository;
            _wfProRequestLogRepository = wfProRequestLogRepository;
        }
        #endregion

        #region 创建一个流程实例
        /// <summary>
        /// 获取流程开始后的任务审批相关数据
        /// </summary>
        /// <param name="wfDefCode">流程定义编码</param>
        /// <param name="formData">流程定义中的表单数据</param>
        /// <param name="currentUserId">表单提交人</param>
        /// <param name="currentUserName">表单提交人</param>
        /// <returns></returns>
        public async Task<WorkflowStartExecuteData> GetStartWorkflowExecutorInfoAsync(string wfDefCode, List<WorkflowProFieldDTO> formData, string currentUserId, string currentUserName)
        {
            // 需要添加记得流程实例数据
            var processData = await InitialProcessDataByFormAsync(wfDefCode, formData, currentUserId, currentUserName, null, null, null, false);

            // 获取开始流程节点
            var startTask = processData.Tasks.FirstOrDefault(m => m.NodeType == WorkflowNodeType.Start);

            // 根据开始任务及提交的表单数据获取下一任务数据，并设置下一任务节点的所有执行人及未执行人数据
            var nextTask = GetNextTask(startTask, processData.Context, processData.Tasks);

            // 根据流程设置，获取所有的可执行数据并更新字段：AllUserIds、UnProcessUserNames
            await SetExecutorInfoByExecutorSetting(nextTask, processData.SubmitUserId, processData.SubmitUserName, currentUserId, currentUserName);

            return new WorkflowStartExecuteData()
            {
                Id = Guid.NewGuid(),
                //表单定义数据
                WorkflowDefId = processData.WorkflowDefId,
                WorkflowDefCode = processData.WorkflowDefCode,
                WorkflowDefVersion = processData.Version,
                WorkflowDefName = processData.WorkflowDefName,
                //设置表单详情的Api地址
                WorkflowFormType = processData.WorkflowFormType,
                AppFormDetailApiUrl = processData.AppFormDetailApiUrl,
                AppFormDetailQueryString = processData.AppFormDetailQueryString,
                //设置应用回调Api地址
                AppAuditSuccessApiUrl = processData.AppAuditSuccessApiUrl,
                AppAuditReturnApiUrl = processData.AppAuditReturnApiUrl,
                AppAuditQueryString = processData.AppAuditQueryString,
                //审核人设置数据
                AgreeUserIds = nextTask.AgreeUserIds,
                AgreeUserNames = nextTask.AgreeUserNames,
                DisagreeUserIds = nextTask.DisagreeUserIds,
                DisagreeUserNames = nextTask.DisagreeUserNames,
                UnProcessUserIds = nextTask.UnProcessUserIds,
                UnProcessUserNames = nextTask.UnProcessUserNames,
                AllUserIds = nextTask.AllUserIds,
                AllUserNames = nextTask.AllUserNames,
                //任务数据
                TaskId = nextTask.Id,
                TaskCode = nextTask.Code,
                TaskName = nextTask.Name,
                TaskType = Enum.Parse<Enums.Workflow.WorkflowNodeType>(nextTask.NodeType.ToString()),
                NotifyUserIds = nextTask.NotifyUserIds,
                NotifyUserNames = nextTask.NotifyUserNames,
                ExecuteUserId = currentUserId,
                ExecuteUserName = currentUserName,
                //传入的表单数据
                FormData = formData,
            };
        }

        /// <summary>
        /// 创建一个流程实例（流程过程）
        /// </summary>
        /// <param name="wfDefCode">流程定义编码</param>
        /// <param name="executeData">流程定义中的表单数据</param>
        /// <returns>流程实例编码（Code）</returns>
        public async Task<string> StartWorkflowAsync(WorkflowStartExecuteData executeData, string currentUserId, string currentUserName, bool isTest = false)
        {
            if (executeData.WorkflowFormType == WorkflowFormType.FormAddress
                && string.IsNullOrEmpty(executeData.AppFormDetailApiUrl))
                throw new ArgumentNullException("AppFormDetailApiUrl", "当表单类型为：" + WorkflowFormType.FormAddress.ToDescription() + "时，需要设置表单详情的接入地址");
            
            if (executeData.WorkflowFormType == WorkflowFormType.ModelDefinition
                && executeData.FormData == null && !executeData.FormData.Any())
                throw new ArgumentNullException("FormData", "当表单类型为：" + WorkflowFormType.ModelDefinition.ToDescription() + "时，需要设置表单数据");

            Logger.LogDebug(string.Format("----开始-StartWorkflowAsync：使用流程定义【{0}】启动流程实例，提交人【{1}】",
                executeData.WorkflowDefCode + "：" + executeData.WorkflowDefVersion + "：" + executeData.WorkflowDefName,
                currentUserName));

            // 需要添加记得流程实例数据
            var addFields = new List<WorkflowProField>();
            var addNodes = new List<WorkflowProTask>();
            var addRules = new List<WorkflowProTaskRule>();
            var processData = await InitialProcessDataByFormAsync(executeData.WorkflowDefCode, executeData.FormData, currentUserId, currentUserName, addFields, addNodes, addRules, isTest);

            var wfCode = _configApiService.GetSeedCodeByName("WorkflowID");
            processData.Code = wfCode;
            // 设置表单详情的Api地址
            processData.WorkflowFormType = processData.WorkflowFormType;
            processData.AppFormDetailApiUrl = !string.IsNullOrEmpty(executeData.AppFormDetailApiUrl) ? executeData.AppFormDetailApiUrl : processData.AppFormDetailApiUrl;
            processData.AppFormDetailQueryString = !string.IsNullOrEmpty(executeData.AppFormDetailQueryString) ? executeData.AppFormDetailQueryString : processData.AppFormDetailQueryString;
            // 设置应用回调Api地址
            processData.AppAuditSuccessApiUrl = !string.IsNullOrEmpty(executeData.AppAuditSuccessApiUrl) ? executeData.AppAuditSuccessApiUrl : processData.AppAuditSuccessApiUrl;
            processData.AppAuditReturnApiUrl = !string.IsNullOrEmpty(executeData.AppAuditReturnApiUrl) ? executeData.AppAuditReturnApiUrl : processData.AppAuditReturnApiUrl;
            processData.AppAuditQueryString = !string.IsNullOrEmpty(executeData.AppAuditQueryString) ? executeData.AppAuditQueryString : processData.AppAuditQueryString;

            // 启动流程处理过程，设置各个任务的执行状态，获取所有修改后的任务列表
            var modifiedTasks = new List<WorkflowProTask>();
            var addTaskExecutes = new List<WorkflowProTaskExecute>();
            
            // 获取开始流程节点
            var startTask = processData.Tasks.FirstOrDefault(m => m.NodeType == WorkflowNodeType.Start);
            // 根据开始节点及表单数据获取下一任务节点，并设置下一任务的执行人数据
            var nextTask = GetNextTask(startTask, processData.Context, processData.Tasks);
            if (executeData.AllUserIds != null && executeData.AllUserIds.Any())
            {
                // 设置下一任务的执行人数据
                if (!string.IsNullOrEmpty(executeData.AllUserIds))
                {
                    nextTask.AllUserIds = executeData.AllUserIds;
                    nextTask.AllUserNames = executeData.AllUserNames;
                }
                // 设置下一任务的未处理人数据
                if (!string.IsNullOrEmpty(executeData.UnProcessUserIds))
                {
                    nextTask.UnProcessUserIds = executeData.UnProcessUserIds;
                    nextTask.UnProcessUserNames = executeData.UnProcessUserNames;
                }
            }
            else
            {
                // 根据流程设置，获取所有的可执行数据并更新字段：AllUserIds、UnProcessUserNames
                await SetExecutorInfoByExecutorSetting(nextTask, processData.SubmitUserId, processData.SubmitUserName, currentUserId, currentUserName);
            }

            // 处理各个流程节点的状态，以及获取所有的执行数据(WorkflowProTaskExecute:addTaskExecutes)
            await ProcessNextTask(processData, startTask, processData.Tasks, executeData, modifiedTasks, addTaskExecutes);

            Logger.LogDebug(string.Format("----结束-StartWorkflowAsync：使用流程定义【{0}】启动流程实例【{1}】，提交人【{2}】",
                executeData.WorkflowDefCode + "：" + executeData.WorkflowDefVersion + "：" + executeData.WorkflowDefName,
                processData.Id + "：" + processData.Code + "：" + processData.Name,
                currentUserName));
            try
            {
                //保存流程实例中相关的：表单、任务及任务规则、已执行数据
                if (addFields.Any())
                    await _wfProFieldRepository.AddAsync(addFields, false);
                if (addNodes.Any())
                    await _wfProTaskRepository.AddAsync(addNodes, false);
                if (addRules.Any())
                    await _wfProTaskRuleRepository.AddAsync(addRules, false);
                if (addTaskExecutes.Any())
                    await _wfProTaskExecuteRepository.AddAsync(addTaskExecutes, false);
                //保存流程实例
                await _wfProcessRepository.AddAsync(processData, false);

                //保存日志
                var log = new WorkflowDefLog()
                {
                    WorkflowDefId = processData.WorkflowDefId,
                    Code = processData.WorkflowDefCode,
                    Version = processData.Version,
                    Name = executeData.WorkflowDefName,
                    OperatorId = currentUserId,
                    Operator = currentUserName,
                    Remark = "启动了一个新的流程。"
                };
                await _wfDefLogRepository.AddAsync(log, false);

                var success = await _unitOfWorkContext.CommitAsync() > 0;
                //成功后更新树结构（TreeNode）中的扩展字段（TreeCode、Level、Leaf）
                if (success)
                {
                    await _wfProFieldRepository.UpdateExtendFieldsByFilterAsync(m => m.ProcessId.Equals(processData.Id));
                }
                return success ? wfCode : null;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
                if (ex.InnerException != null)
                    Logger.LogError(ex.InnerException.Message + Environment.NewLine + ex.InnerException.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// 将流程定义数据映射为流程实例数据，见：KC.Service.Workflow.AutoMapper.Profile.WorkflowMapperProfile
        /// </summary>
        /// <param name="wfDefCode"></param>
        /// <param name="formData"></param>
        /// <param name="currentUserId"></param>
        /// <param name="currentUserName"></param>
        /// <param name="isTest"></param>
        /// <returns></returns>
        private async Task<WorkflowProcess> InitialProcessDataByFormAsync(string wfDefCode, List<WorkflowProFieldDTO> formData, string currentUserId, string currentUserName, List<WorkflowProField> addFields, List<WorkflowProTask> addNodes, List<WorkflowProTaskRule> addRules, bool isTest = false)
        {
            var data = await _wfDefinitionRepository.GetWfDefinitionDetailByCodeAsync(wfDefCode);
            if (data == null)
                throw new ArgumentNullException("wfDefCode", string.Format("未找到流程定义【Code={0}】的记录", wfDefCode));

            var rootData = data.WorkflowFields.Where(m => m.ParentId == null).ToList();
            foreach (var parent in rootData)
            {
                TreeNodeUtil.NestTreeNode(parent, data.WorkflowFields);
            }
            data.WorkflowFields = rootData;

            //将流程定义数据映射为流程实例数据，见：KC.Service.Workflow.AutoMapper.Profile.WorkflowMapperProfile
            var processData = _mapper.Map<WorkflowProcess>(data);
            processData.StartDateTime = DateTime.UtcNow;
            if (processData.DefDeadlineInterval.HasValue)
                processData.DeadlineDate = processData.StartDateTime.Value.AddDays(processData.DefDeadlineInterval.Value);
            processData.SubmitUserId = currentUserId;
            processData.SubmitUserName = currentUserName;
            processData.CreatedBy = currentUserId;
            processData.CreatedName = currentUserName;
            processData.CreatedDate = DateTime.UtcNow;
            processData.ModifiedBy = currentUserId;
            processData.ModifiedName = currentUserName;
            processData.ModifiedDate = DateTime.UtcNow;

            //测试验证时，设置流程实例及流程相关任务的title
            var testPreHeader = "【验证测试】";
            if (isTest)
            {
                processData.Name = testPreHeader + processData.Name;
                processData.Tasks.ToList().ForEach(m =>
                {
                    m.Name = testPreHeader + m.Name;
                    //将当前用户设置为所有任务的审批人
                    m.UserIds = currentUserId;
                    m.UserNames = currentUserName;
                    m.RoleIds = null;
                    m.RoleNames = null;
                    m.OrgCodes = null;
                    m.OrgNames = null;
                });
            }

            //保存流程表单，只保存两级的Field，超出不再保存
            if (processData.Context != null && formData != null)
            {
                var i = 1;
                foreach (var field in processData.Context)
                {
                    field.Id = 0;
                    field.ProcessId = processData.Id;
                    field.Process = processData;
                    field.ParentId = null;
                    field.ParentNode = null;
                    field.Level = 1;
                    field.Index = i;
                    i++;
                    if (field.DataType != AttributeDataType.List)
                    {
                        field.Value = GetNestFieldValue(field.Name, formData);
                        if (addFields != null)
                            addFields.Add(field);
                    }
                    //对List列表中的数据进行处理
                    else
                    {
                        var fieldValue = GetNestFieldValue(field.Name, formData);
                        //获取key（属性名）和value（属性值）的对象列表
                        var listDict = SerializeHelper.GetDictListByArrayJson(fieldValue);
                        if (null == listDict) continue;

                        var j = 1;
                        foreach (var cField in field.ChildNodes)
                        {
                            var keyValues = listDict.FirstOrDefault(m => m.ContainsKey(cField.Name));
                            if (keyValues != null)
                            {
                                var cFieldValue = keyValues[cField.Name];
                                cField.Id = 0;
                                cField.ProcessId = processData.Id;
                                cField.Process = processData;
                                cField.ParentNode = field;
                                cField.Value = cFieldValue;
                                cField.Index = j;
                                if (addFields != null)
                                    addFields.Add(cField);
                                j++;
                            }
                        }
                    }
                }
            }

            //保存流程节点
            var startTask = processData.Tasks.FirstOrDefault(m => m.NodeType == WorkflowNodeType.Start);
            //开始节点默认为完成阶段
            if (startTask != null)
                startTask.TaskStatus = WorkflowTaskStatus.Finished;
            var index = 1;
            foreach (var node in processData.Tasks)
            {
                var wfNodeCode = _configApiService.GetSeedCodeByName("WorkflowTaskId");
                //将下一个流程任务节点为本节点的Code重新设置
                processData.Tasks.Where(m => m.PrevNodeCode != null && m.PrevNodeCode.Equals(node.Code)).ToList()
                    .ForEach(n => n.PrevNodeCode = wfNodeCode);
                //将上一个流程任务节点为本节点的Code重新设置
                processData.Tasks.Where(m => m.NextNodeCode != null && m.NextNodeCode.Equals(node.Code)).ToList()
                    .ForEach(n => n.NextNodeCode = wfNodeCode);
                //将返回流程任务节点为本节点的Code重新设置
                processData.Tasks.Where(m => m.ReturnNodeCode != null && m.ReturnNodeCode.Equals(node.Code)).ToList()
                    .ForEach(n => n.ReturnNodeCode = wfNodeCode);

                //设置本节点相关数据
                node.Id = Guid.NewGuid();
                node.Code = wfNodeCode;
                node.ProcessId = processData.Id;
                node.Process = processData;
                //开始节点后的下一个节点，状态设置为：处理中
                if (node.Code.Equals(startTask.NextNodeCode, StringComparison.OrdinalIgnoreCase))
                {
                    node.TaskStatus = WorkflowTaskStatus.Process;

                    processData.CurrentTaskId = node.Id;
                }
                // 除去开始节点及其下一个节点，状态设置为：未处理
                else if (!node.Code.Equals(startTask.Code, StringComparison.OrdinalIgnoreCase))
                {
                    node.TaskStatus = WorkflowTaskStatus.UnProcess;
                }

                if (addNodes != null)
                    addNodes.Add(node);
                foreach (var rule in node.Rules)
                {
                    rule.Id = 0;
                    rule.Task = node;
                    rule.TaskId = node.Id;
                    rule.TaskCode = node.Code;
                    if (addRules != null)
                        addRules.Add(rule);
                }
                index++;
            }

            return processData;
        }
        private string GetNestFieldValue(string fieldName, IEnumerable<WorkflowProFieldDTO> childFields)
        {
            var matchField = childFields.FirstOrDefault(m => m.Text.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
            if (matchField != null) return matchField.Value;

            foreach (var field in childFields)
            {
                return GetNestFieldValue(fieldName, field.Children);
            }

            return null;
        }

        /// <summary>
        /// 判断提交提交的表单是否符合任务中设置的规则条件
        /// </summary>
        /// <param name="context">提交的表单</param>
        /// <param name="currentTask">当前任务（为Condition的任务时，并且设置了任务规则：Rules）</param>
        /// <returns></returns>
        private bool JudgeFormIsFitWfTaskRules(IEnumerable<WorkflowProField> context, WorkflowProTask currentTask)
        {
            if (context == null) return true;

            var conditionResult = true;
            foreach (var rule in currentTask.Rules)
            {
                var ruleType = rule.RuleType;
                var name = rule.FieldName;
                var oper = rule.OperatorType;
                var ruleValue = rule.FieldValue;
                var field = context.FirstOrDefault(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                var ruleResult = true;
                Logger.LogDebug(string.Format("JudgeFormIsFitWfTaskRules--3.条件处理过程：针对条件任务【{0}】的判断规则【{1}】", currentTask.Code + "：" + currentTask.Name, rule.ToString()));
                if (field != null)
                {
                    var dataType = field.DataType;
                    var fieldValue = field.Value;
                    #region 处理传入表单的值是否符合对应规则
                    if (dataType == Framework.Base.AttributeDataType.String)
                    {
                        switch (oper)
                        {
                            case Framework.Base.RuleOperatorType.Equal:
                                ruleResult = ruleValue.Equals(fieldValue, StringComparison.OrdinalIgnoreCase);
                                break;
                            case Framework.Base.RuleOperatorType.NotEqual:
                                ruleResult = !ruleValue.Equals(fieldValue, StringComparison.OrdinalIgnoreCase);
                                break;
                            case Framework.Base.RuleOperatorType.Contains:
                                ruleResult = ruleValue.Contains(fieldValue);
                                break;
                        }
                    }
                    else if (dataType == Framework.Base.AttributeDataType.Bool)
                    {
                        var intRuleValue = Boolean.Parse(ruleValue);
                        var intFieldValue = Boolean.Parse(fieldValue);
                        switch (oper)
                        {
                            case RuleOperatorType.Equal:
                                ruleResult = intRuleValue == intFieldValue;
                                break;
                            case RuleOperatorType.NotEqual:
                                ruleResult = intRuleValue != intFieldValue;
                                break;
                        }
                    }
                    else if (dataType == Framework.Base.AttributeDataType.Int)
                    {
                        var intRuleValue = int.Parse(ruleValue);
                        var intFieldValue = int.Parse(fieldValue);
                        switch (oper)
                        {
                            case RuleOperatorType.Equal:
                                ruleResult = intRuleValue == intFieldValue;
                                break;
                            case RuleOperatorType.NotEqual:
                                ruleResult = intRuleValue != intFieldValue;
                                break;
                            case RuleOperatorType.Greater:
                                ruleResult = intRuleValue < intFieldValue;
                                break;
                            case RuleOperatorType.GreaterThanAndEqual:
                                ruleResult = intRuleValue <= intFieldValue;
                                break;
                            case RuleOperatorType.Less:
                                ruleResult = intRuleValue > intFieldValue;
                                break;
                            case RuleOperatorType.LessThanAndEqual:
                                ruleResult = intRuleValue >= intFieldValue;
                                break;
                        }
                    }
                    else if (dataType == Framework.Base.AttributeDataType.Decimal)
                    {
                        var decRuleValue = decimal.Parse(ruleValue);
                        var decFieldValue = decimal.Parse(fieldValue);
                        switch (oper)
                        {
                            case RuleOperatorType.Equal:
                                ruleResult = decRuleValue == decFieldValue;
                                break;
                            case RuleOperatorType.NotEqual:
                                ruleResult = decRuleValue != decFieldValue;
                                break;
                            case RuleOperatorType.Greater:
                                ruleResult = decRuleValue < decFieldValue;
                                break;
                            case RuleOperatorType.GreaterThanAndEqual:
                                ruleResult = decRuleValue <= decFieldValue;
                                break;
                            case RuleOperatorType.Less:
                                ruleResult = decRuleValue > decFieldValue;
                                break;
                            case RuleOperatorType.LessThanAndEqual:
                                ruleResult = decRuleValue >= decFieldValue;
                                break;
                        }
                    }
                    else if (dataType == Framework.Base.AttributeDataType.DateTime)
                    {
                        var dateRuleValue = DateTime.Parse(ruleValue);
                        var dateFieldValue = DateTime.Parse(fieldValue);
                        switch (oper)
                        {
                            case RuleOperatorType.Equal:
                                ruleResult = dateRuleValue == dateFieldValue;
                                break;
                            case RuleOperatorType.NotEqual:
                                ruleResult = dateRuleValue != dateFieldValue;
                                break;
                            case RuleOperatorType.Greater:
                                ruleResult = dateRuleValue < dateFieldValue;
                                break;
                            case RuleOperatorType.GreaterThanAndEqual:
                                ruleResult = dateRuleValue <= dateFieldValue;
                                break;
                            case RuleOperatorType.Less:
                                ruleResult = dateRuleValue > dateFieldValue;
                                break;
                            case RuleOperatorType.LessThanAndEqual:
                                ruleResult = dateRuleValue >= dateFieldValue;
                                break;
                        }
                    }
                    #endregion

                    Logger.LogDebug(string.Format("JudgeFormIsFitWfTaskRules--3.条件判断过程：【{0}】实际值【{1}】，该判断规则的结果：{2}", dataType.ToDescription(), fieldValue, ruleResult ? "真" : "假"));
                }

                if (ruleType == RuleType.And)
                {
                    conditionResult = conditionResult && ruleResult;
                }
                else if (ruleType == RuleType.Or)
                {
                    conditionResult = conditionResult || ruleResult;
                }
                else
                {
                    conditionResult = ruleResult;
                }

                if (!conditionResult) break;
            }

            return conditionResult;
        }

        /// <summary>
        /// 判断提交的表单是否符合任务中设置的规则条件，并按任务设置进行处理： </br>
        ///     为真时，调用任务中设置的NextNodeCode的下一任务 </br>
        ///     为假时，调用任务中设置的ReturnNodeCode的下一任务 </br>
        /// </summary>
        /// <param name="process">当前流程实例</param>
        /// <param name="currentTask">需要处理的任务（NodeType=Task)</param>
        /// <param name="allTasks">任务实例下的所有任务</param>
        /// <param name="executeUserId">执行人Id</param>
        /// <param name="executeUserName">执行人姓名</param>
        /// <param name="modifiedTasks">修改状态后的所有任务</param>
        private void JudgeAndHandleFormIsFitWfTaskRules(WorkflowProcess process, WorkflowProTask currentTask, IEnumerable<WorkflowProTask> allTasks, string executeUserId, string executeUserName)
        {
            var conditionResult = JudgeFormIsFitWfTaskRules(process.Context, currentTask);
            Logger.LogDebug(string.Format("3.针对条件任务【{0}】的最终结果【{1}】",
        currentTask.Code + "：" + currentTask.Name, conditionResult ? "真" : "假"));
            //根据流程节点的规则为true（当前任务设置的：NextNodeCode）或false（当前任务设置的：ReturnNodeCode）时，获取所需执行的下一个任务节点
            var conditionNext = conditionResult
                ? allTasks.FirstOrDefault(t => !currentTask.NextNodeCode.IsNullOrEmpty() && t.Code.Equals(currentTask.NextNodeCode, StringComparison.OrdinalIgnoreCase))
                : allTasks.FirstOrDefault(t => !currentTask.ReturnNodeCode.IsNullOrEmpty() && t.Code.Equals(currentTask.ReturnNodeCode, StringComparison.OrdinalIgnoreCase));
            if (conditionNext != null)
            {
                Logger.LogDebug(string.Format("4.针对条件任务【{0}】的进入下一环节【{1}】",
currentTask.Code + "：" + currentTask.Name, conditionNext.NodeType.ToDescription() + "：" + conditionNext.Code + "：" + conditionNext.Name));
                //如果下一个任务为Task时，设置状态未处理中，并设置该任务Id未流程实例的当前任务Id
                if (conditionNext.NodeType == WorkflowNodeType.Task)
                {
                    currentTask.ModifiedBy = executeUserId;
                    currentTask.ModifiedName = executeUserName;
                    currentTask.ModifiedDate = DateTime.UtcNow;
                    currentTask.TaskStatus = WorkflowTaskStatus.Finished;

                    conditionNext.ModifiedBy = executeUserId;
                    conditionNext.ModifiedName = executeUserName;
                    conditionNext.ModifiedDate = DateTime.UtcNow;
                    conditionNext.TaskStatus = WorkflowTaskStatus.Process;
                    
                    process.CurrentTaskId = conditionNext.Id;
                }
                //如果下一个任务为Condition时，继续进行迭代处理
                else if (conditionNext.NodeType == WorkflowNodeType.Condition)
                {
                    JudgeAndHandleFormIsFitWfTaskRules(process, conditionNext, allTasks, executeUserId, executeUserName);
                }
            }
        }
        #endregion

        #region 处理流程实例（流程过程）
        /// <summary>
        /// 获取流程下一个任务的审批相关数据（审批人）
        /// </summary>
        /// <param name="processCode">流程实例编码</param>
        /// <param name="executeUserId">流程执行人</param>
        /// <param name="executeUserName">流程执行人</param>
        /// <returns></returns>
        public async Task<WorkflowExecuteData> GetSubmitWorkflowExecutorInfoAsync(string processCode, string executeUserId, string executeUserName)
        {
            var processData = await _wfProcessRepository.GetWorkflowProcessDetailByCodeAsync(processCode);
            if (processData == null)
                throw new ArgumentNullException("code", string.Format("未找到流程实例【Code={0}】的记录", processCode));

            //保存流程节点
            var currentTask = processData.Tasks.FirstOrDefault(m => m.TaskStatus == WorkflowTaskStatus.Process);
            if (currentTask == null)
                throw new ArgumentNullException("currentTask", "当前流程任务为空，无法获取下一流程节点");

            if (currentTask.NodeType != WorkflowNodeType.Task)
            {
                currentTask = GetNextTask(currentTask, processData.Context, processData.Tasks);
            }

            // 根据流程设置，获取所有的可执行数据并更新字段：AllUserIds、UnProcessUserNames
            await SetExecutorInfoByExecutorSetting(currentTask, processData.SubmitUserId, processData.SubmitUserName, executeUserId, executeUserName);

            return new WorkflowExecuteData()
            {
                Id = Guid.NewGuid(),
                //表单定义数据
                WorkflowDefId = processData.WorkflowDefId,
                WorkflowDefCode = processData.WorkflowDefCode,
                WorkflowDefVersion = processData.WorkflowDefName,
                //审核人设置数据
                AgreeUserIds = currentTask.AgreeUserIds,
                AgreeUserNames = currentTask.AgreeUserNames,
                DisagreeUserIds = currentTask.DisagreeUserIds,
                DisagreeUserNames = currentTask.DisagreeUserNames,
                UnProcessUserIds = currentTask.UnProcessUserIds,
                UnProcessUserNames = currentTask.UnProcessUserNames,
                AllUserIds = currentTask.AllUserIds,
                AllUserNames = currentTask.AllUserNames,
                //任务数据
                TaskId = currentTask.Id,
                TaskCode = currentTask.Code,
                TaskName = currentTask.Name,
                TaskType = Enum.Parse<Enums.Workflow.WorkflowNodeType>(currentTask.NodeType.ToString()),
                NotifyUserIds = currentTask.NotifyUserIds,
                NotifyUserNames = currentTask.NotifyUserNames,
                ExecuteUserId = executeUserId,
                ExecuteUserName = executeUserName,

                FormData = _mapper.Map<List<WorkflowProFieldDTO>>(processData.Context),
            };
        }

        /// <summary>
        /// 处理流程实例（流程过程）：Submit流程，进入下一个流程处理节点
        /// </summary>
        /// <param name="processCode">流程实例编码</param>
        /// <param name="executeData">流程实例传入的表单数据</param>
        /// <returns>是否处理成功</returns>
        public async Task<bool> SubmitWorkflowAsync(string processCode, WorkflowExecuteData executeData)
        {
            var process = await _wfProcessRepository.GetWorkflowProcessDetailByCodeAsync(processCode);
            if (process == null)
                throw new ArgumentNullException("code", string.Format("未找到流程实例【Code={0}】的记录", processCode));

            var currentIsAgree = executeData.ExecuteStatus == Enums.Workflow.WorkflowExecuteStatus.Approve;
            var currentUserId = executeData.ExecuteUserId;
            var currentUserName = executeData.ExecuteUserName;

            process.ModifiedBy = currentUserId;
            process.ModifiedName = currentUserName;
            process.ModifiedDate = DateTime.UtcNow;

            var modifiedFields = new List<WorkflowProField>();
            if (executeData.FormData != null)
            {
                foreach (var field in executeData.FormData)
                {
                    var dbField = process.Context.FirstOrDefault(m => m.Id == field.Id);
                    if (dbField != null)
                    {
                        dbField.Value = field.Value;
                        dbField.Value1 = field.Value1;
                        dbField.Value2 = field.Value2;
                        dbField.ModifiedBy = currentUserId;
                        dbField.ModifiedName = currentUserName;
                        dbField.ModifiedDate = DateTime.UtcNow;
                        modifiedFields.Add(dbField);
                    }
                }
            }

            var modifiedTasks = new List<WorkflowProTask>();
            var addTaskExecutes = new List<WorkflowProTaskExecute>();

            var tasks = process.Tasks;
            var startTask = tasks.FirstOrDefault(m => m.NodeType == WorkflowNodeType.Start);
            var currentTask = tasks.FirstOrDefault(m => m.TaskStatus == WorkflowTaskStatus.Process);
            // 如果为回退后的流程，需要重新发起并执行
            if (currentTask == null && process.ProcessStatus == WorkflowProcessStatus.Regress) {
                await ProcessNextTask(process, startTask, tasks, executeData, modifiedTasks, addTaskExecutes);
                currentTask = tasks.FirstOrDefault(m => m.TaskStatus == WorkflowTaskStatus.Process);
            }
            
            if (currentTask == null) return true;

            var currentType = currentTask.Type;
            var currentNodeType = currentTask.NodeType;
            var currentTaskExecutes = currentTask.TaskExecutes;

            currentTask.StartDateTime = DateTime.UtcNow;
            if (currentTask.DeadlineInterval.HasValue)
                currentTask.DeadlineDate = currentTask.StartDateTime.Value.AddDays(currentTask.DeadlineInterval.Value);
            currentTask.ModifiedBy = currentUserId;
            currentTask.ModifiedName = currentUserName;
            currentTask.ModifiedDate = DateTime.UtcNow;

            Logger.LogDebug(string.Format("----开始-SubmitWorkflowAsync：流程实例【{0}】处理当前任务【{1}】，已同意人【{2}】，未同意人【{3}】，未审核人【{4}】，当前处理人【{5}】处理结果【{6}】",
                process.Code + "：" + process.Name,
                currentTask.Code + "：" + currentTask.Name + "：" + currentTask.Type.ToDescription() + "：" + currentTask.TaskStatus.ToDescription(),
                currentTask.AgreeUserNames,
                currentTask.DisagreeUserNames,
                currentTask.UnProcessUserNames,
                currentUserName,
                executeData.ExecuteStatus.ToDescription()));

            // 所有审批人员
            var allAuditUserIds = !string.IsNullOrEmpty(currentTask.AllUserIds)
                    ? currentTask.AllUserIds.ArrayFromCommaDelimitedStrings().ToList()
                    : executeData.AllUserIds.ArrayFromCommaDelimitedStrings().ToList();
            var allAuditUserNames = !string.IsNullOrEmpty(currentTask.AllUserNames)
                    ? currentTask.AllUserNames.ArrayFromCommaDelimitedStrings().ToList()
                    : executeData.AllUserNames.ArrayFromCommaDelimitedStrings().ToList();
            // 已同意的审批人员
            var currentAgreeUserIds = !string.IsNullOrEmpty(currentTask.AgreeUserIds)
                    ? currentTask.AgreeUserIds.ArrayFromCommaDelimitedStrings().ToList()
                    : new List<string>();
            var currentAgreeUserNames = !string.IsNullOrEmpty(currentTask.AgreeUserNames)
                    ? currentTask.AgreeUserNames.ArrayFromCommaDelimitedStrings().ToList()
                    : new List<string>();
            #region 设置当前任务的审批人记录

            // 设置当前任务的执行人数据
            if (executeData.AllUserIds != null && executeData.AllUserIds.Any())
            {
                // 设置当前任务的执行人数据
                if (string.IsNullOrEmpty(currentTask.AllUserIds))
                {
                    currentTask.AllUserIds = executeData.AllUserIds;
                    currentTask.AllUserNames = executeData.AllUserNames;
                }
                // 设置当前任务的未处理人数据
                if (string.IsNullOrEmpty(currentTask.UnProcessUserIds))
                {
                    currentTask.UnProcessUserIds = executeData.UnProcessUserIds;
                    currentTask.UnProcessUserNames = executeData.UnProcessUserNames;
                }
            }
            // 设置当前任务同意、不同意处理人
            if (currentIsAgree)
            {
                currentAgreeUserIds.Add(executeData.ExecuteUserId);
                currentAgreeUserNames.Add(executeData.ExecuteUserName);

                currentTask.AgreeUserIds = currentTask.AgreeUserIds != null
                    ? currentTask.AgreeUserIds + ", " + executeData.ExecuteUserId
                    : executeData.ExecuteUserId;
                currentTask.AgreeUserNames = currentTask.AgreeUserNames != null
                    ? currentTask.AgreeUserNames + ", " + executeData.ExecuteUserName
                    : executeData.ExecuteUserName;
            }
            else
            {
                currentTask.DisagreeUserIds = currentTask.DisagreeUserIds != null
                    ? currentTask.DisagreeUserIds + ", " + executeData.ExecuteUserId
                    : executeData.ExecuteUserId;
                currentTask.DisagreeUserNames = currentTask.DisagreeUserNames != null
                    ? currentTask.DisagreeUserNames + ", " + executeData.ExecuteUserName
                    : executeData.ExecuteUserName;
            }

            // 将已执行的人从未执行列表中删除
            if (!string.IsNullOrEmpty(currentTask.UnProcessUserIds))
            {
                var unUsedIds = currentTask.UnProcessUserIds.ArrayFromCommaDelimitedStrings();
                var unUsedNames = currentTask.UnProcessUserNames.ArrayFromCommaDelimitedStrings();
                var i = 0;
                var unUsedUsers = new Dictionary<string, string>();
                foreach (var unUsedId in unUsedIds)
                {
                    if (!unUsedId.Equals(executeData.ExecuteUserId, StringComparison.OrdinalIgnoreCase))
                    {
                        unUsedUsers.Add(unUsedIds[i], unUsedNames[i]);
                    }
                    i++;
                }
                if (unUsedUsers.Any())
                {
                    currentTask.UnProcessUserIds = unUsedUsers.Keys.ToCommaSeparatedString();
                    currentTask.UnProcessUserNames = unUsedUsers.Values.ToCommaSeparatedString();
                }
            }
            #endregion

            var canProcessNext = false;
            #region 根据任务类型（SingleLine、MultiLine、WeightLine）判断任务是否进入下一个流程环节
            if (currentType == WorkflowType.SingleLine)
            {
                canProcessNext = true;
            }
            else if (currentType == WorkflowType.MultiLine
                || currentType == WorkflowType.WeightLine)
            {
                Logger.LogDebug(string.Format("SubmitWorkflowAsync--1.判断【多人审批：{0}】是否进入下一流程节点：流程实例所有的审批人【{1}】，已同意审批人【{2}】",
                    currentType.ToDescription(),
                    allAuditUserNames?.ToCommaSeparatedString(),
                    currentAgreeUserNames.ToCommaSeparatedString()));
                if (currentType == WorkflowType.MultiLine)
                {
                    canProcessNext = allAuditUserIds.ListEquals(currentAgreeUserIds);
                }
                else if (currentType == WorkflowType.WeightLine)
                {
                    var weight = currentTask.WeightValue;
                    var actualWeight = (decimal)currentAgreeUserIds.Count / allAuditUserIds.Count;
                    canProcessNext = actualWeight > weight;

                    Logger.LogDebug(string.Format("SubmitWorkflowAsync--1.判断是否进入下一流程节点：流程实例【{0}】处理当前任务【{1}】，当前权重【{2}】，通过权重【{3}】",
                        process.Code + "：" + process.Name,
                        currentTask.Code + "：" + currentTask.Name,
                        actualWeight,
                        weight));
                }
            }
            Logger.LogDebug(string.Format("SubmitWorkflowAsync--1.判断是否进入下一流程节点：流程实例【{0}】处理当前任务【{1}】，是否进入下一个环节？【{2}】",
                process.Code + "：" + process.Name,
                currentTask.Code + "：" + currentTask.Name,
                canProcessNext ? "是" : "否"));
            #endregion

            if (canProcessNext)
            {
                //单人审批（或多人审批）完成后，根据审批是否同意，处理流程环节
                if (executeData.ExecuteStatus == Enums.Workflow.WorkflowExecuteStatus.Approve)
                {
                    await ProcessNextTask(process, currentTask, tasks, executeData, modifiedTasks, addTaskExecutes);
                }
                else
                {
                    await ProcessReturnTask(process, currentTask, tasks, executeData, modifiedTasks, addTaskExecutes);
                }
            }
            else
            {
                //多人审批，修改当前任务的审批人记录
                currentTask.TaskStatus = WorkflowTaskStatus.Process;

                //任务完成后，设置当前任务节点的执行数据
                var execute = new WorkflowProTaskExecute()
                {
                    TaskId = currentTask.Id,
                    ExecuteStatus = Enum.Parse<WorkflowExecuteStatus>(executeData.ExecuteStatus.ToString()),
                    ExecuteUserId = currentUserId,
                    ExecuteUserName = currentUserName,
                    ExecuteDateTime = DateTime.UtcNow,
                    ExecuteRemark = executeData.ExecuteRemark,
                    ExecuteFileBlob = executeData.ExecuteFileBlob,
                };
                if (!addTaskExecutes.Any(m => m.TaskId.Equals(currentTask.Id) && m.ExecuteUserId.Equals(currentUserId)))
                    addTaskExecutes.Add(execute);
            }

            Logger.LogDebug(string.Format("----结束-SubmitWorkflowAsync：流程实例【{0}】处理当前任务【{1}】，已同意人【{2}】，未同意人【{3}】，未审核人【{4}】，当前处理人【{5}】处理结果【{6}】",
                process.Code + "：" + process.Name,
                currentTask.Code + "：" + currentTask.Name + "：" + currentTask.Type.ToDescription() + "：" + currentTask.TaskStatus.ToDescription(),
                currentTask.AgreeUserNames,
                currentTask.DisagreeUserNames,
                currentTask.UnProcessUserNames,
                currentUserName,
                executeData.ExecuteStatus.ToDescription()));
            try
            {
                //更改流程实例状态
                await _wfProcessRepository.ModifyAsync(process, new string[] { "CurrentTaskId", "ProcessStatus",
                    "StartDateTime", "EndDateTime", "DeadlineDate",
                    "ModifiedBy", "ModifiedName", "ModifiedDate" }, false);

                //更新流程任务表单数据
                if (modifiedFields.Any())
                    await _wfProFieldRepository.ModifyAsync(modifiedFields, new string[] {
                    "Value", "Value1", "Value2",
                    "ModifiedBy", "ModifiedName", "ModifiedDate" }, false);

                //新增流程任务处理数据
                if (addTaskExecutes.Any())
                    await _wfProTaskExecuteRepository.AddAsync(addTaskExecutes, false);

                //更改流程任务状态
                if (!modifiedTasks.Any(m => m.Id == startTask.Id))
                    modifiedTasks.Add(startTask);
                if (!modifiedTasks.Any(m => m.Id == currentTask.Id))
                    modifiedTasks.Add(currentTask);
                var success = await _wfProTaskRepository.ModifyAsync(modifiedTasks,
                    new string[] { "Name", "TaskStatus", "AllUserIds", "AllUserNames",
                        "AgreeUserIds", "AgreeUserNames", "DisagreeUserIds",
                        "DisagreeUserNames","UnProcessUserIds","UnProcessUserNames",
                        "StartDateTime", "EndDateTime", "DeadlineDate",
                        "ModifiedBy", "ModifiedName", "ModifiedDate" }) > 0;

                //var success = _unitOfWorkContext.Commit() > 0;
                return success;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
                if (ex.InnerException != null)
                    Logger.LogError(ex.InnerException.Message + Environment.NewLine + ex.InnerException.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// 根据当前任务，逐级向下提交，处理过程：</br>
        ///     1. 如果下一任务为（NodeType=Task），设置当前任务为（Finished），设置下一任务为（Process）；</br>
        ///     2. 如果下一任务为（NodeType=Condition），根据条件任务下的规则（Rules）及表单传入值进行判断；</br>
        ///     2. 判断条件为True时：设置该条件任务为（Finished）并找到下一任务重复该处理过程；</br>
        ///     3. 判断条件为False时：设置该条件任务为（UnProcess）并找到退回任务进行退回处理；</br>
        /// </summary>
        /// <param name="process">当前流程实例</param>
        /// <param name="currentTask">需要处理的任务（NodeType=Task)</param>
        /// <param name="allTasks">任务实例下的所有任务</param>
        /// <param name="executeUserId">执行人Id</param>
        /// <param name="executeUserName">执行人姓名</param>
        /// <param name="executeRemark">执行人审批说明</param>
        /// <param name="executeFileBlob">执行人上传附件</param>
        /// <param name="modifiedTasks">修改状态后的所有任务</param>
        /// <param name="addTaskExecutes">任务节点的执行数据</param>
        private async Task ProcessNextTask(WorkflowProcess process, WorkflowProTask currentTask, IEnumerable<WorkflowProTask> allTasks, WorkflowExecuteData executeData, List<WorkflowProTask> modifiedTasks, List<WorkflowProTaskExecute> addTaskExecutes)
        {
            WorkflowProTask nextTask;
            if (currentTask.NodeType == WorkflowNodeType.Condition)
            {
                //判断传入表单是否符合条件任务
                var conditionResult = JudgeFormIsFitWfTaskRules(process.Context, currentTask);

                //根据流程节点的规则为true（当前任务设置的：NextNodeCode）或false（当前任务设置的：ReturnNodeCode）时，获取所需执行的下一个任务节点
                nextTask = conditionResult
                        ? allTasks.FirstOrDefault(t => !currentTask.NextNodeCode.IsNullOrEmpty() && t.Code.Equals(currentTask.NextNodeCode, StringComparison.OrdinalIgnoreCase))
                        : allTasks.FirstOrDefault(t => !currentTask.ReturnNodeCode.IsNullOrEmpty() && t.Code.Equals(currentTask.ReturnNodeCode, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                nextTask = allTasks.FirstOrDefault(t => t.Code.Equals(currentTask.NextNodeCode, StringComparison.OrdinalIgnoreCase));
            }
            if (nextTask == null) return;

            var nextTaskType = nextTask.NodeType;
            nextTask.ModifiedBy = executeData.ExecuteUserId;
            nextTask.ModifiedName = executeData.ExecuteUserName;
            nextTask.ModifiedDate = DateTime.UtcNow;

            Logger.LogDebug(string.Format("ProcessNextTask--2.流程处理过程：开始处理下一个【{0}节点】：流程实例【{1}】开始处理下一个任务【{2}】，{3}",
                nextTaskType.ToDescription(),
                process.Code + "：" + process.Name,
                nextTask.Code + "：" + nextTask.Name,
                nextTaskType == WorkflowNodeType.Condition && nextTask.Rules.Any()
                    ? "判断条件【" + nextTask.Rules.ToCommaSeparatedStringByFilter(m => m.ToString()) + "】"
                    : string.Empty));
            //根据下一环节的任务类型，处理流程
            switch (nextTaskType)
            {
                case WorkflowNodeType.Start:
                    // 设置流程实例状态及当前正在执行的任务Id
                    process.ProcessStatus = WorkflowProcessStatus.Regress;
                    currentTask.TaskStatus = WorkflowTaskStatus.UnProcess;
                    // 退回表单时，调用相关应用的API
                    await HandlerReturnCallBackAync(process);

                    // 下一个节点为开始节点，需要继续执行下下一个节点
                    //await ProcessNextTask(process, nextTask, allTasks, executeData, modifiedTasks, addTaskExecutes);
                    break;
                case WorkflowNodeType.Task:
                    process.CurrentTaskId = nextTask.Id;
                    process.ProcessStatus = WorkflowProcessStatus.Auditing;
                    // 设置当前任务信息的相关用户信息
                    currentTask.EndDateTime = DateTime.UtcNow;
                    currentTask.TaskStatus = WorkflowTaskStatus.Finished;
                    // 任务完成后，设置当前任务节点的执行数据
                    var taskExcuteData = new WorkflowProTaskExecute()
                    {
                        TaskId = currentTask.Id,
                        ExecuteStatus = WorkflowExecuteStatus.Approve,
                        ExecuteUserId = executeData.ExecuteUserId,
                        ExecuteUserName = executeData.ExecuteUserName,
                        ExecuteRemark = executeData.ExecuteRemark,
                        ExecuteFileBlob = executeData.ExecuteFileBlob,
                        ExecuteDateTime = DateTime.UtcNow,
                    };
                    if (addTaskExecutes != null && !addTaskExecutes.Any(m => m.TaskId.Equals(currentTask.Id) && m.ExecuteUserId.Equals(executeData.ExecuteUserId)))
                        addTaskExecutes.Add(taskExcuteData);
                    // 设置下一任务的执行状态
                    nextTask.TaskStatus = WorkflowTaskStatus.Process;
                    
                    Logger.LogDebug(string.Format("ProcessNextTask--3.根据当前任务执行人【{0}】及下一任务设置类型【{1}--{2}】设置下一任务的状态为【{3}】所有执行人【{4}】，未处理人【{5}】",
                        executeData.ExecuteUserName,
                        nextTask.Name,
                        nextTask.ExecutorSetting.ToDescription(),
                        nextTask.TaskStatus.ToDescription(),
                        nextTask.AllUserNames,
                        nextTask.UnProcessUserNames));
                    break;
                case WorkflowNodeType.Condition:
                    process.CurrentTaskId = nextTask.Id;
                    process.ProcessStatus = WorkflowProcessStatus.Auditing;
                    currentTask.TaskStatus = WorkflowTaskStatus.Finished;
                    // 判断传入表单是否符合条件任务
                    var conditionResult = JudgeFormIsFitWfTaskRules(process.Context, nextTask);

                    // 根据流程节点的规则为true（当前任务设置的：NextNodeCode）或false（当前任务设置的：ReturnNodeCode）时，获取所需执行的下一个任务节点
                    var conditionNextTask = conditionResult
                            ? allTasks.FirstOrDefault(t => !nextTask.NextNodeCode.IsNullOrEmpty() && t.Code.Equals(nextTask.NextNodeCode, StringComparison.OrdinalIgnoreCase))
                            : allTasks.FirstOrDefault(t => !nextTask.ReturnNodeCode.IsNullOrEmpty() && t.Code.Equals(nextTask.ReturnNodeCode, StringComparison.OrdinalIgnoreCase));

                    Logger.LogDebug(string.Format("ProcessNextTask--3.条件处理过程：针对条件任务【{0}】的最终结果为【{1}】，是否找到下一任务？【{2}】",
                        nextTask.Code + "：" + nextTask.Name,
                        conditionResult ? "真" : "假",
                        conditionNextTask != null));
                    if (conditionNextTask != null)
                    {
                        Logger.LogDebug(string.Format("ProcessNextTask--4.条件处理过程：针对条件任务【{0}】的进入下一环节【{1}】",
                            nextTask.Code + "：" + nextTask.Name,
                            conditionNextTask.NodeType.ToDescription() + "：" + conditionNextTask.Code + "：" + conditionNextTask.Name));

                        // 判断并处理中间环节的所有任务，判断依据及处理过程如下：
                        //      1. 从当前条件任务（nextTask）到下一个处理任务（conditionNextTask）有中间环节的话，为朝下流转处理，设置所有的中间节点状态为：已完成；
                        var betweenTasks = FindAllTasksBetweenCodes(allTasks, nextTask.Code, conditionNextTask.Code);
                        foreach (var t in betweenTasks)
                        {
                            Logger.LogDebug(string.Format("ProcessNextTask--4.1 条件处理过程：针对条件任务【{0}】需处理的中间环节【{1}】状态设置为【{2}】",
                                    nextTask.Code + "：" + nextTask.Name,
                                    t.Code + "：" + t.Name,
                                    WorkflowTaskStatus.Finished.ToDescription()));

                            t.TaskStatus = WorkflowTaskStatus.Finished;
                            t.ModifiedBy = executeData.ExecuteUserId;
                            t.ModifiedName = executeData.ExecuteUserName;
                            t.ModifiedDate = DateTime.UtcNow;
                            if (modifiedTasks != null)
                                modifiedTasks.Add(t);
                        }

                        // 判断并处理中间环节的所有任务，判断依据及处理过程如下：
                        //      2. 从下一个处理任务（conditionNextTask）到当前条件任务（nextTask）有中间环节的话，为：朝上退回处理，设置所有的中间节点状态为：未处理；
                        betweenTasks = FindAllTasksBetweenCodes(allTasks, conditionNextTask.Code, nextTask.Code);
                        var isReturnProcess = betweenTasks.Any();
                        foreach (var t in betweenTasks)
                        {
                            Logger.LogDebug(string.Format("ProcessNextTask--4.2 条件处理过程：针对条件任务【{0}】需处理的中间环节【{1}】状态设置为【{2}】，并设置所有的执行人数据为空",
                                    nextTask.Code + "：" + nextTask.Name,
                                    t.Code + "：" + t.Name,
                                    WorkflowTaskStatus.UnProcess.ToDescription()));

                            t.AllUserIds = null;
                            t.AllUserNames = null;
                            t.AgreeUserIds = null;
                            t.AgreeUserNames = null;
                            t.DisagreeUserIds = null;
                            t.DisagreeUserNames = null;
                            t.UnProcessUserIds = null;
                            t.UnProcessUserNames = null;
                            t.TaskStatus = WorkflowTaskStatus.UnProcess;
                            t.ModifiedBy = executeData.ExecuteUserId;
                            t.ModifiedName = executeData.ExecuteUserName;
                            t.ModifiedDate = DateTime.UtcNow;
                            if (modifiedTasks != null)
                                modifiedTasks.Add(t);
                        }

                        // 条件判断完毕后，根据条件的下一任务，进行处理
                        var condNextTaskType = conditionNextTask.NodeType;
                        switch (condNextTaskType)
                        {
                            case WorkflowNodeType.Start:
                                // 退回表单时，调用相关应用的API
                                await HandlerReturnCallBackAync(process);
                                break;
                            case WorkflowNodeType.Condition:
                            case WorkflowNodeType.Task:
                                await ProcessNextTask(process, nextTask, allTasks, executeData, modifiedTasks, addTaskExecutes);
                                // 当当前任务节点及其后的条件节点需要回退时，设置两者状态为：未处理
                                if (isReturnProcess)
                                {
                                    Logger.LogDebug(string.Format("ProcessNextTask--4.3 条件处理过程：当前任务节点【{0}】及其后的条件节点【{1}】进行回退处理后，设置两者状态设置为【{2}】",
                                    currentTask.Code + "：" + currentTask.Name + "：" + currentTask.TaskStatus.ToDescription(),
                                    nextTask.Code + "：" + nextTask.Name + "：" + nextTask.TaskStatus.ToDescription(),
                                    WorkflowTaskStatus.UnProcess.ToDescription()));

                                    if (currentTask.TaskStatus != WorkflowTaskStatus.Process)
                                        currentTask.TaskStatus =  WorkflowTaskStatus.UnProcess;
                                    if (nextTask.TaskStatus != WorkflowTaskStatus.Process)
                                        nextTask.TaskStatus = WorkflowTaskStatus.UnProcess;
                                }

                                Logger.LogDebug(string.Format("ProcessNextTask--4.4 条件处理过程：当前任务节点【{0}】状态为：【{1}】及其后的条件节点【{2}】状态为：【{3}】",
                                    currentTask.Code + "：" + currentTask.Name,
                                    currentTask.TaskStatus.ToDescription(),
                                    nextTask.Code + "：" + nextTask.Name,
                                    nextTask.TaskStatus.ToDescription(),
                                    WorkflowTaskStatus.UnProcess.ToDescription()));
                                break;
                            case WorkflowNodeType.SubFlow:
                                // TODO: 子流程的处理过程未实现
                                throw new NotSupportedException("子流程的处理过程未实现");
                            case WorkflowNodeType.End:
                                process.CurrentTaskId = nextTask.Id;
                                process.EndDateTime = DateTime.UtcNow;
                                process.ProcessStatus = WorkflowProcessStatus.Finished;

                                currentTask.TaskStatus = WorkflowTaskStatus.Finished;
                                nextTask.TaskStatus = WorkflowTaskStatus.Finished;
                                conditionNextTask.TaskStatus = WorkflowTaskStatus.Finished;

                                // 流程执行完成后，调用相关应用的API
                                await HandlerCompleteCallBackAync(process);
                                break;
                        }
                    }

                    break;
                case WorkflowNodeType.SubFlow:
                    //TODO: 子流程的处理过程未实现
                    throw new NotSupportedException("子流程的处理过程未实现");
                case WorkflowNodeType.End:
                    process.CurrentTaskId = nextTask.Id;
                    process.EndDateTime = DateTime.UtcNow;
                    process.ProcessStatus = WorkflowProcessStatus.Finished;

                    currentTask.TaskStatus = WorkflowTaskStatus.Finished;
                    nextTask.TaskStatus = WorkflowTaskStatus.Finished;

                    // 流程执行完成后，调用相关应用的API
                    await HandlerCompleteCallBackAync(process);
                    break;
            }

            Logger.LogDebug(string.Format("ProcessNextTask--2.流程处理过程：结束下一个【{0}节点】：流程实例【{1}】处理完成下一个任务【{2}】",
                nextTaskType.ToDescription(),
                process.Code + "：" + process.Name,
                nextTask.Code + "：" + nextTask.Name));

            if (modifiedTasks != null && !modifiedTasks.Any(m => m.Id.Equals(nextTask.Id)))
                modifiedTasks.Add(nextTask);
        }
        /// <summary>
        /// 根据当前任务，逐级向上退回，处理过程：</br>
        ///     1. 如果退回任务为（NodeType=Condition），再朝上退回直至找到任务为（NodeType=Task）的节点；</br>
        ///     2. 设置每个退回任务(Condition)的任务状态为：未处理（WorkflowTaskStatus.UnProcess);</br>
        ///     3. 设置最终退回任务(Task)的任务状态为：处理中（WorkflowTaskStatus.Process);</br>
        /// </summary>
        /// <param name="process">当前流程实例</param>
        /// <param name="currentTask">需要处理的任务（NodeType=Task)</param>
        /// <param name="allTasks">任务实例下的所有任务</param>
        /// <param name="executeUserId">执行人Id</param>
        /// <param name="executeUserName">执行人姓名</param>
        /// <param name="executeRemark">执行人审批说明</param>
        /// <param name="executeFileBlob">执行人上传附件</param>
        /// <param name="modifiedTasks">修改状态后的所有任务</param>
        /// <param name="addTaskExecutes">任务节点的执行数据</param>
        private async Task ProcessReturnTask(WorkflowProcess process, WorkflowProTask currentTask, IEnumerable<WorkflowProTask> allTasks, WorkflowExecuteData executeData, List<WorkflowProTask> modifiedTasks, List<WorkflowProTaskExecute> addTaskExecutes)
        {
            var returnTask = allTasks.FirstOrDefault(t => t.Code.Equals(currentTask.PrevNodeCode, StringComparison.OrdinalIgnoreCase));
            if (returnTask == null) return;

            var returnTaskType = returnTask.NodeType;
            returnTask.ModifiedBy = executeData.ExecuteUserId;
            returnTask.ModifiedName = executeData.ExecuteUserName;
            returnTask.ModifiedDate = DateTime.UtcNow;

            Logger.LogDebug(string.Format("ProcessReturnTask--2.流程实例退回：流程实例【{0}】退回处理任务【{1}】，退回任务类型【{2}】，任务状态：【{3}】",
                process.Code + "：" + process.Name,
                returnTask.Code + "：" + returnTask.Name,
                returnTaskType.ToDescription(),
                returnTaskType == WorkflowNodeType.Condition
                    ? WorkflowTaskStatus.UnProcess.ToDescription()
                    : WorkflowTaskStatus.Process.ToDescription()));

            // 根据下一环节的任务类型，处理流程
            switch (returnTaskType)
            {
                case WorkflowNodeType.Start:
                    // 设置流程实例状态及当前正在执行的任务Id
                    process.ProcessStatus = WorkflowProcessStatus.Regress;
                    currentTask.TaskStatus = WorkflowTaskStatus.UnProcess;
                    // 退回表单时，调用相关应用的API
                    await HandlerReturnCallBackAync(process);

                    // 下一个节点为开始节点，需要继续执行下下一个节点
                    //await ProcessNextTask(process, returnTask, allTasks, executeData, modifiedTasks, addTaskExecutes);
                    break;
                case WorkflowNodeType.Task:
                    process.CurrentTaskId = returnTask.Id;
                    process.ProcessStatus = WorkflowProcessStatus.Auditing;
                    // 设置当前任务信息的相关用户信息
                    currentTask.ModifiedBy = executeData.ExecuteUserId;
                    currentTask.ModifiedName = executeData.ExecuteUserName;
                    currentTask.ModifiedDate = DateTime.UtcNow;
                    currentTask.TaskStatus = WorkflowTaskStatus.Back;

                    // 如果为多人审批任务，还需要重置相关审核记录
                    if (currentTask.Type == WorkflowType.MultiLine
                        || currentTask.Type == WorkflowType.WeightLine)
                    {
                        currentTask.AgreeUserIds = null;
                        currentTask.AgreeUserNames = null;
                        currentTask.DisagreeUserIds = null;
                        currentTask.DisagreeUserNames = null;
                        currentTask.UnProcessUserIds = null;
                        currentTask.UnProcessUserNames = null;
                    }

                    // 根据流程设置，获取所有的可执行数据并更新字段：OrgCodes、RoleIds、UserIds
                    await SetExecutorInfoByExecutorSetting(currentTask, process.SubmitUserId, process.SubmitUserName, executeData.ExecuteUserId, executeData.ExecuteUserName);

                    // 任务退回后，设置当前任务节点的执行数据
                    var taskExcuteData = new WorkflowProTaskExecute()
                    {
                        TaskId = currentTask.Id,
                        ExecuteStatus = WorkflowExecuteStatus.Return,
                        ExecuteUserId = executeData.ExecuteUserId,
                        ExecuteUserName = executeData.ExecuteUserName,
                        ExecuteDateTime = DateTime.UtcNow,
                        ExecuteRemark = executeData.ExecuteRemark,
                        ExecuteFileBlob = executeData.ExecuteFileBlob,
                    };
                    if (addTaskExecutes != null && !addTaskExecutes.Any(m => m.TaskId.Equals(currentTask.Id) && m.ExecuteUserId.Equals(executeData.ExecuteUserId)))
                        addTaskExecutes.Add(taskExcuteData);

                    returnTask.TaskStatus = WorkflowTaskStatus.Process;

                    Logger.LogDebug(string.Format("ProcessReturnTask--3.根据当前任务执行人【{0}】及下一任务设置类型【{1}--{2}】设置回退任务的状态为【{3}】所有执行人【{4}】，未处理人【{5}】",
                        executeData.ExecuteUserName,
                        returnTask.Name,
                        returnTask.ExecutorSetting.ToDescription(),
                        returnTask.TaskStatus.ToDescription(),
                        returnTask.AllUserNames,
                        returnTask.UnProcessUserNames));
                    break;
                case WorkflowNodeType.Condition:
                    // 设置当前任务信息执行状态
                    currentTask.TaskStatus = WorkflowTaskStatus.UnProcess;
                    returnTask.TaskStatus = WorkflowTaskStatus.UnProcess;

                    await ProcessReturnTask(process, returnTask, allTasks, executeData, modifiedTasks, addTaskExecutes);
                    break;
                case WorkflowNodeType.SubFlow:
                    //TODO: 子流程的处理过程未实现
                    throw new NotSupportedException("子流程的处理过程未实现");
                case WorkflowNodeType.End:
                    // 设置流程实例状态及当前正在执行的任务Id
                    process.ProcessStatus = WorkflowProcessStatus.Finished;
                    process.CurrentTaskId = returnTask.Id;

                    currentTask.TaskStatus = WorkflowTaskStatus.Finished;
                    returnTask.TaskStatus = WorkflowTaskStatus.Finished;

                    // 流程执行完成后，调用相关应用的API
                    await HandlerCompleteCallBackAync(process);
                    break;
            }

            Logger.LogDebug(string.Format("ProcessReturnTask--2.结束下一个【{2}】任务：流程实例【{0}】处理完成下一个任务【{1}】",
                process.Code + "：" + process.Name,
                returnTask.Code + "：" + returnTask.Name,
                returnTaskType.ToDescription()));

            if (modifiedTasks != null && !modifiedTasks.Any(m => m.Id.Equals(returnTask.Id)))
                modifiedTasks.Add(returnTask);
        }

        /// <summary>
        /// 根据开始任务编码及结束任务编码，获取开始至结束任务之间的所有任务
        /// </summary>
        /// <param name="allTasks">所有任务</param>
        /// <param name="startTaskCode">开始任务编码</param>
        /// <param name="endTaskCode">结束任务编码</param>
        /// <returns>开始至结束任务之间的所有任务</returns>
        private List<WorkflowProTask> FindAllTasksBetweenCodes(IEnumerable<WorkflowProTask> allTasks, string startTaskCode, string endTaskCode)
        {
            var startTask = allTasks.FirstOrDefault(m => m.NodeType == WorkflowNodeType.Start);
            var sequenceDictTasks = new Dictionary<string, WorkflowProTask>();
            GetSequenctTaskCodes(startTask, allTasks, ref sequenceDictTasks);

            var result = new List<WorkflowProTask>();
            var index = 0;
            var startIndex = 0;
            var endIndex = allTasks.Count();
            foreach (var dictTask in sequenceDictTasks)
            {
                if (dictTask.Key.Equals(startTaskCode, StringComparison.OrdinalIgnoreCase))
                    startIndex = index;
                if (dictTask.Key.Equals(endTaskCode, StringComparison.OrdinalIgnoreCase))
                    endIndex = index;
                index++;
            }
            var i = 0;
            foreach (var dictTask in sequenceDictTasks)
            {
                if (i > startIndex && i < endIndex)
                    result.Add(dictTask.Value);
                i++;
            }
            return result;
        }
        /// <summary>
        /// 获取任务列表（按流转顺序进行排列）的任务编号
        /// </summary>
        /// <param name="startTask">开始任务</param>
        /// <param name="allTasks">所有任务</param>
        /// <param name="result">排序后的任务编号列表</param>
        private void GetSequenctTaskCodes(WorkflowProTask startTask, IEnumerable<WorkflowProTask> allTasks, ref Dictionary<string, WorkflowProTask> result)
        {
            result.Add(startTask.Code, startTask);
            var nextTask = allTasks.FirstOrDefault(m => m.Code.Equals(startTask.NextNodeCode));
            if (nextTask != null)
                GetSequenctTaskCodes(nextTask, allTasks, ref result);
        }

        #endregion

        #region 根据流程定义下节点的执行人设置，设置当前任务的执行人数据
        /// <summary>
        /// 根据当前任务及表单数据，获取下一流程节点任务
        /// </summary>
        /// <param name="currentTask">当前任务</param>
        /// <param name="context">表单数据</param>
        /// <param name="allTasks">所有任务</param>
        /// <returns>下一流程节点任务</returns>
        private WorkflowProTask GetNextTask(WorkflowProTask currentTask, IEnumerable<WorkflowProField> context, IEnumerable<WorkflowProTask> allTasks)
        {
            if (currentTask == null)
                throw new ArgumentNullException("currentTask", "当前流程任务为空，无法获取下一流程节点");

            if (currentTask.Code.Equals(currentTask.NextNodeCode, StringComparison.OrdinalIgnoreCase))
                throw new Exception(string.Format("流程定义出错，此为：循环任务，当前任务【{0}】的下一个流程任务为自己：【编码：{1}】", currentTask.Name, currentTask.NextNodeCode));

            var nextTask = allTasks.FirstOrDefault(t => t.Code.Equals(currentTask.NextNodeCode, StringComparison.OrdinalIgnoreCase));
            if (nextTask == null)
                throw new ArgumentNullException("currentTask", string.Format("未找到当前任务【{0}】的下一个编码为：{1} 的对应流程任务", currentTask.Name, currentTask.NextNodeCode));

            var nextTaskType = nextTask.NodeType;
            Logger.LogDebug(string.Format("---GetNextTask--1.获取下一流程过程：开始处理下一个【{0}节点】：【{1}】{2}",
                nextTaskType.ToDescription(),
                nextTask.Code + "：" + nextTask.Name,
                nextTaskType == WorkflowNodeType.Condition && nextTask.Rules.Any()
                    ? "判断条件【" + nextTask.Rules.ToCommaSeparatedStringByFilter(m => m.ToString()) + "】"
                    : string.Empty));

            // 根据下一环节的任务类型，处理流程
            WorkflowProTask result = null;
            switch (nextTaskType)
            {
                case WorkflowNodeType.Start:
                    return GetNextTask(nextTask, context, allTasks);
                case WorkflowNodeType.Condition:
                    // 判断
                    var conditionResult = JudgeFormIsFitWfTaskRules(context, nextTask);

                    // 根据流程节点的规则为true（当前任务设置的：NextNodeCode）或false（当前任务设置的：ReturnNodeCode）时，获取所需执行的下一个任务节点
                    var conditionNextTask = conditionResult
                        ? allTasks.FirstOrDefault(t => !nextTask.NextNodeCode.IsNullOrEmpty() && t.Code.Equals(nextTask.NextNodeCode, StringComparison.OrdinalIgnoreCase))
                        : allTasks.FirstOrDefault(t => !nextTask.ReturnNodeCode.IsNullOrEmpty() && t.Code.Equals(nextTask.ReturnNodeCode, StringComparison.OrdinalIgnoreCase));
                    if (conditionNextTask == null)
                        throw new ArgumentException(string.Format("---GetNextTask--条件任务【{0}】判断为：{1}，未找到的下一个编码为：{2} 的流程节点",
                            nextTask.Name, conditionResult, conditionResult ? currentTask.NextNodeCode : nextTask.ReturnNodeCode));

                    Logger.LogDebug(string.Format("---GetNextTask--2.条件处理过程：针对条件任务【{0}】的进入下一环节【{1}】",
                            nextTask.Code + "：" + nextTask.Name,
                            conditionNextTask.NodeType.ToDescription() + "：" + conditionNextTask.Code + "：" + conditionNextTask.Name));

                    // 条件判断完毕后，根据条件的下一任务，进行处理
                    var condNextTaskType = conditionNextTask.NodeType;
                    switch (condNextTaskType)
                    {
                        case WorkflowNodeType.Start:
                        case WorkflowNodeType.Condition:
                            return GetNextTask(conditionNextTask, context, allTasks);
                        case WorkflowNodeType.Task:
                            result = conditionNextTask;
                            break;
                        case WorkflowNodeType.SubFlow:
                            // TODO: 子流程的处理过程未实现
                            throw new NotSupportedException("子流程的处理过程未实现");
                        case WorkflowNodeType.End:
                            return nextTask;
                    }
                    break;
                case WorkflowNodeType.SubFlow:
                    // TODO: 子流程的处理过程未实现
                    throw new NotSupportedException("子流程的处理过程未实现");
                case WorkflowNodeType.End:
                    return nextTask;
                default:
                    result = nextTask;
                    break;
            }

            return result;
        }

        /// <summary>
        /// 获取流程实例中当前任务下的下一任务执行者数据（组织Ids、角色Ids、用户Ids）
        /// </summary>
        /// <param name="processCode">流程实例编码</param>
        /// <param name="createByUserId">流程发起人UserId</param>
        /// <param name="createByUserName">流程发起人姓名</param>
        /// <param name="executeUserId">当前任务执行人UserId</param>
        /// <param name="executeUserName">当前任务执行人姓名</param>
        /// <returns>组织Ids、角色Ids、用户Ids</returns>
        private async Task SetExecutorInfoByExecutorSetting(WorkflowProTask currentTask, string createByUserId, string createByUserName, string executeUserId, string executeUserName)
        {
            var orgs = new Dictionary<string, string>();
            var roles = new Dictionary<string, string>();
            var users = new Dictionary<string, string>();

            var currentUserName = string.Empty;
            var executors = new List<UserSimpleDTO>();
            var exeOrgs = new List<OrganizationSimpleDTO>();
            var exeRoles = new List<RoleSimpleDTO>();
            var setting = currentTask.ExecutorSetting;

            Logger.LogDebug(string.Format("SetExecutorInfoByExecutorSetting--根据设置类型【{0}】开始获取任务【{1}】的执行人信息",
                setting.ToDescription(),
                currentTask.Code + "：" + currentTask.Name));
            switch (setting)
            {
                // 设置组织/角色/用户
                case ExecutorSetting.Executor:
                    var searchModel = new DataPermissionSearchDTO();
                    searchModel.OrgCodes.AddRange(currentTask.OrgCodes.ArrayFromCommaDelimitedStrings());
                    searchModel.RoleIds.AddRange(currentTask.RoleIds.ArrayFromCommaDelimitedStrings());
                    searchModel.UserIds.AddRange(currentTask.UserIds.ArrayFromCommaDelimitedStrings());
                    var allAuditUsers = await _accountApiService.LoadUsersByIdsAndRoleIdsAndOrgCodes(searchModel);
                    if (allAuditUsers != null && allAuditUsers.Any())
                        executors.AddRange(allAuditUsers);
                    break;
                // 流程发起人的主管
                case ExecutorSetting.CreatorManager:
                    currentUserName = createByUserName;
                    //获取流程发起人所属组织及其成员
                    var createdOrgs = await _accountApiService.LoadOrganizationsWithUsersByUserId(createByUserId);
                    if (createdOrgs != null && createdOrgs.Any())
                    {
                        //获取流程发起人归属的内部组织及组织的主管
                        var cManagerUsers = createdOrgs
                            .Where(org => org.OrganizationType == OrganizationType.Internal)
                            .SelectMany(org => org.Users.Where(u => u.PositionLevel == PositionLevel.Mananger && u.Status == WorkflowBusStatus.Approved).Distinct(m => m.UserId))
                            .ToList();
                        executors.AddRange(cManagerUsers);
                    }
                    break;
                //流程发起人所属组织
                case ExecutorSetting.CreatorOrganization:
                    currentUserName = createByUserName;
                    //获取流程发起人所属组织及角色
                    var createdUserOrgs = await _accountApiService.LoadOrganizationsWithUsersByUserId(createByUserId);
                    if (createdUserOrgs != null && createdUserOrgs.Any())
                    {
                        exeOrgs.AddRange(createdUserOrgs);
                        executors.AddRange(createdUserOrgs.SelectMany(m => m.Users).Distinct(m => m.UserId));
                    }
                    break;
                //流程发起人的上级主管
                case ExecutorSetting.CreatorSuperior:
                    currentUserName = createByUserName;
                    //获取流程发起人所属组织及其成员
                    var createdHigherOrgs = await _accountApiService.LoadHigherOrganizationsWithUsersByUserId(createByUserId);
                    if (createdHigherOrgs != null && createdHigherOrgs.Any())
                    {
                        //获取流程发起人归属的内部组织及组织的主管
                        var cHigherManagerUsers = createdHigherOrgs
                            .Where(org => org.OrganizationType == OrganizationType.Internal)
                            .SelectMany(org => org.Users.Where(u => u.PositionLevel == PositionLevel.Mananger && u.Status == WorkflowBusStatus.Approved))
                            .ToList();
                        executors.AddRange(cHigherManagerUsers);
                    }
                    break;
                //流程发起人所属角色
                case ExecutorSetting.CreatorRole:
                    currentUserName = createByUserName;
                    //获取流程发起人所属组织及角色
                    var createdUserRoles = await _accountApiService.LoadRolesWithUsersByUserId(createByUserId);
                    if (createdUserRoles != null && createdUserRoles.Any())
                    {
                        exeRoles.AddRange(createdUserRoles);
                        executors.AddRange(createdUserRoles.SelectMany(m => m.Users).Distinct(m => m.UserId));
                    }
                    break;
                //上一流程提交审核人的主管
                case ExecutorSetting.SubmitterManager:
                    currentUserName = executeUserName;
                    //获取上一流程提交人所属组织及其成员
                    var submitOrgs = await _accountApiService.LoadOrganizationsWithUsersByUserId(executeUserId);
                    if (submitOrgs != null && submitOrgs.Any())
                    {
                        //获取上一流程提交人归属的内部组织及组织的主管
                        var sManagerUsers = submitOrgs
                            .Where(org => org.OrganizationType == OrganizationType.Internal)
                            .SelectMany(org => org.Users.Where(u => u.PositionLevel == PositionLevel.Mananger && u.Status == WorkflowBusStatus.Approved).Distinct(m => m.UserId))
                            .ToList();
                        executors.AddRange(sManagerUsers);
                    }
                    break;
                //上一流程提交审核人所属组织
                case ExecutorSetting.SubmitterOrganization:
                    currentUserName = executeUserName;
                    //获取流程发起人所属组织及角色
                    var submitUserOrgs = await _accountApiService.LoadOrganizationsWithUsersByUserId(executeUserId);
                    if (submitUserOrgs != null && submitUserOrgs.Any())
                    {
                        exeOrgs.AddRange(submitUserOrgs);
                        executors.AddRange(submitUserOrgs.SelectMany(m => m.Users).Distinct(m => m.UserId));
                    }
                    break;
                //上一流程提交审核人所属角色
                case ExecutorSetting.SubmitterRole:
                    currentUserName = executeUserName;
                    //获取流程发起人所属组织及角色
                    var submitUserRoles = await _accountApiService.LoadRolesWithUsersByUserId(executeUserId);
                    if (submitUserRoles != null && submitUserRoles.Any())
                    {
                        exeRoles.AddRange(submitUserRoles);
                        executors.AddRange(submitUserRoles.SelectMany(m => m.Users).Distinct(m => m.UserId));
                    }
                    break;
                //上一流程提交审核人的上级主管
                case ExecutorSetting.SubmitterSuperior:
                    currentUserName = executeUserName;
                    //获取上一流程提交人所属组织及其成员
                    var submitHigherOrgs = await _accountApiService.LoadHigherOrganizationsWithUsersByUserId(executeUserId);
                    if (submitHigherOrgs != null && submitHigherOrgs.Any())
                    {
                        //获取上一流程提交人归属的内部组织及组织的主管
                        var sHigherManagerUsers = submitHigherOrgs
                            .Where(org => org.OrganizationType == OrganizationType.Internal)
                            .SelectMany(org => org.Users.Where(u => u.PositionLevel == PositionLevel.Mananger && u.Status == WorkflowBusStatus.Approved).Distinct(m => m.UserId))
                            .ToList();
                        executors.AddRange(sHigherManagerUsers);
                    }
                    break;
            }

            if (exeOrgs.Any())
            {
                exeOrgs.ForEach(m =>
                {
                    if (!orgs.ContainsKey(m.OrganizationCode)) { }
                        orgs.Add(m.OrganizationCode, m.Text);
                });
            }
            if (exeRoles.Any())
            {
                exeRoles.ForEach(m =>
                {
                    if (!roles.ContainsKey(m.RoleId))
                        roles.Add(m.RoleId, m.DisplayName);
                });
            }
            if (executors.Any())
            {
                executors.ForEach(m =>
                {
                    if (!users.ContainsKey(m.UserId))
                        users.Add(m.UserId, m.DisplayName);
                });
            }

            // 设置下一流程的执行人数据：用户
            if (users.Any())
            {
                if (string.IsNullOrEmpty(currentTask.AllUserIds))
                {
                    currentTask.AllUserIds = users.Keys.ToCommaSeparatedString();
                    currentTask.AllUserNames = users.Values.ToCommaSeparatedString();
                }

                if (string.IsNullOrEmpty(currentTask.UnProcessUserIds))
                {
                    currentTask.UnProcessUserIds = users.Keys.ToCommaSeparatedString();
                    currentTask.UnProcessUserNames = users.Values.ToCommaSeparatedString();
                }

            }

            Logger.LogDebug(string.Format("SetExecutorInfoByExecutorSetting--根据设置类型【{0}】获取任务【{1}】的执行人信息：所有执行人【{2}】；未处理人【{3}】；设置执行人【{4}】",
                setting.ToDescription(),
                currentTask.Code + "：" + currentTask.Name,
                currentTask.AllUserNames,
                currentTask.UnProcessUserNames,
                users.Values.ToCommaSeparatedString()));
            //return new Tuple<IDictionary<string, string>, IDictionary<string, string>, IDictionary<string, string>>(orgs, roles, users);
        }
        #endregion

        #region 流程执行中，回调函数的处理

        private async Task HandlerCompleteCallBackAync(WorkflowProcess process)
        {
            if (string.IsNullOrEmpty(process.AppAuditSuccessApiUrl))
                return;

            var authType = process.SecurityType;
            var authScope = process.AuthScope;
            var authKey = process.AuthKey;
            var authSecret = process.AuthSecret;
            var authAddress = getFullRequestUrl(process.AuthAddress, process.AuthAddressParams);
            var successUrl = getFullRequestUrl(process.AppAuditSuccessApiUrl, process.AppAuditQueryString);
            var returnUrl = getFullRequestUrl(process.AppAuditReturnApiUrl, process.AppAuditQueryString);
            var log = new WorkflowProRequestLog()
            {
                ProcessId = process.Id,
                ProcessCode = process.Code,
                ProcessName = process.Name,
                RequestType = 0,
                RequestUrl = successUrl,
            };
            switch (authType)
            {
                case SecurityType.SecurityKey:
                    var skQueryString = authKey + "=" + authSecret;
                    successUrl = getFullRequestUrl(successUrl, skQueryString);
                    log.RequestUrl = successUrl;
                    await _oAuth2Client.WebSendGetAsync<string>(successUrl, authScope, "application/json", 
                        callback =>
                        {
                            log.Type = ProcessLogType.Success;
                            log.RequestResultData = callback;
                        },
                        (httpStatusCode, errorMessage) =>
                        {
                            log.Type = ProcessLogType.Failure;
                            log.RequestErrorData = "StatusCode: " + httpStatusCode + "; error:" + errorMessage;
                        });
                    break;
                case SecurityType.HeaderKey:
                    var headers = new Dictionary<string, string>();
                    headers.Add(authKey, authSecret);
                    await _oAuth2Client.WebSendGetAsync<string>(successUrl, authScope, "application/json", headers, 
                        callback =>
                        {
                            log.Type = ProcessLogType.Success;
                            log.RequestResultData = callback;
                        },
                        (httpStatusCode, errorMessage) =>
                        {
                            log.Type = ProcessLogType.Failure;
                            log.RequestErrorData = "StatusCode: " + httpStatusCode + "; error:" + errorMessage;
                        });
                    break;
                case SecurityType.OAuth:
                    var oauthClientInfo = new OAuth2ClientInfo(Tenant.TenantName, authKey, authSecret, authAddress);
                    _oAuth2Client.SetOAuth2ClientInfo(oauthClientInfo);
                    await _oAuth2Client.WebSendGetAsync<string>(successUrl, authScope, "application/json",
                        callback =>
                        {
                            log.Type = ProcessLogType.Success;
                            log.RequestResultData = callback;
                        },
                        (httpStatusCode, errorMessage) =>
                        {
                            log.Type = ProcessLogType.Failure;
                            log.RequestErrorData = "StatusCode: " + httpStatusCode + "; error:" + errorMessage;
                        }, true);
                    break;
                default:
                    await _oAuth2Client.WebSendGetAsync<string>(successUrl, authScope, "application/json",
                        callback =>
                        {
                            log.Type = ProcessLogType.Success;
                            log.RequestResultData = callback;
                        },
                        (httpStatusCode, errorMessage) =>
                        {
                            log.Type = ProcessLogType.Failure;
                            log.RequestErrorData = "StatusCode: " + httpStatusCode + "; error:" + errorMessage;
                        });
                    break;
            }
            Logger.LogDebug(string.Format("HandlerCompleteCallBackAync：流程实例【{0}】处理当前成功回调函数地址【{1}】",
                process.Code + "：" + process.Name, successUrl));

            await _wfProRequestLogRepository.AddAsync(log, false);
        }

        private async Task HandlerReturnCallBackAync(WorkflowProcess process)
        {
            if (string.IsNullOrEmpty(process.AppAuditSuccessApiUrl))
                return;

            var authType = process.SecurityType;
            var authScope = process.AuthScope;
            var authKey = process.AuthKey;
            var authSecret = process.AuthSecret;
            var authAddress = getFullRequestUrl(process.AuthAddress, process.AuthAddressParams);
            var successUrl = getFullRequestUrl(process.AppAuditSuccessApiUrl, process.AppAuditQueryString);
            var returnUrl = getFullRequestUrl(process.AppAuditReturnApiUrl, process.AppAuditQueryString);
            var log = new WorkflowProRequestLog()
            {
                ProcessId = process.Id,
                ProcessCode = process.Code,
                ProcessName = process.Name,
                RequestType = 0,
                RequestUrl = returnUrl,
            };
            switch (authType)
            {
                case SecurityType.SecurityKey:
                    var skQueryString = authKey + "=" + authSecret;
                    returnUrl = getFullRequestUrl(returnUrl, skQueryString);
                    log.RequestUrl = returnUrl;
                    await _oAuth2Client.WebSendGetAsync<string>(returnUrl, authScope, "application/json",
                        callback =>
                        {
                            log.Type = ProcessLogType.Success;
                            log.RequestResultData = callback;
                        },
                        (httpStatusCode, errorMessage) =>
                        {
                            log.Type = ProcessLogType.Failure;
                            log.RequestErrorData = "StatusCode: " + httpStatusCode + "; error:" + errorMessage;
                        });
                    break;
                case SecurityType.HeaderKey:
                    var headers = new Dictionary<string, string>();
                    headers.Add(authKey, authSecret);
                    await _oAuth2Client.WebSendGetAsync<string>(returnUrl, authScope, "application/json", headers,
                        callback =>
                        {
                            log.Type = ProcessLogType.Success;
                            log.RequestResultData = callback;
                        },
                        (httpStatusCode, errorMessage) =>
                        {
                            log.Type = ProcessLogType.Failure;
                            log.RequestErrorData = "StatusCode: " + httpStatusCode + "; error:" + errorMessage;
                        });
                    break;
                case SecurityType.OAuth:
                    var oauthClientInfo = new OAuth2ClientInfo(Tenant.TenantName, authKey, authSecret, authAddress);
                    _oAuth2Client.SetOAuth2ClientInfo(oauthClientInfo);
                    await _oAuth2Client.WebSendGetAsync<string>(returnUrl, authScope, "application/json",
                        callback =>
                        {
                            log.Type = ProcessLogType.Success;
                            log.RequestResultData = callback;
                        },
                        (httpStatusCode, errorMessage) =>
                        {
                            log.Type = ProcessLogType.Failure;
                            log.RequestErrorData = "StatusCode: " + httpStatusCode + "; error:" + errorMessage;
                        }, true);
                    break;
                default:
                    await _oAuth2Client.WebSendGetAsync<string>(returnUrl, authScope, "application/json",
                        callback =>
                        {
                            log.Type = ProcessLogType.Success;
                            log.RequestResultData = callback;
                        },
                        (httpStatusCode, errorMessage) =>
                        {
                            log.Type = ProcessLogType.Failure;
                            log.RequestErrorData = "StatusCode: " + httpStatusCode  + "; error:" + errorMessage;
                        });
                    break;
            }

            Logger.LogDebug(string.Format("HandlerReturnCallBackAync：流程实例【{0}】处理当前回退回调函数地址【{1}】", 
                process.Code + "：" + process.Name, returnUrl));
            await _wfProRequestLogRepository.AddAsync(log, false);
        }

        private string getFullRequestUrl(string baseUrl, string queryString)
        {
            var result = baseUrl;
            if (baseUrl.Contains("?")
                && !string.IsNullOrEmpty(queryString))
            {
                result = baseUrl + "&" + queryString;
            }
            else if (!string.IsNullOrEmpty(queryString))
            {
                result = baseUrl + "?" + queryString;
            }

            return result;
        }

        #endregion

        #region 获取可执行任务数据
        /// <summary>
        /// 获取所有的可执行任务
        /// </summary>
        /// <param name="pageIndex">分页页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="nodeName">节点名称</param>
        /// <param name="userName">执行人姓名</param>
        /// <param name="categoryId">所属流程实例的分类</param>
        /// <returns></returns>
        public PaginatedBaseDTO<WorkflowProTaskDTO> FindPagenatedWorkflowTasksByFilter(int pageIndex, int pageSize, int? categoryId, string nodeName, string userName, WorkflowTaskStatus? status)
        {
            //获取节点类型为任务，并且为未处理的任务状态
            Expression<Func<WorkflowProTask, bool>> predicate = m => m.NodeType == WorkflowNodeType.Task && m.TaskStatus == WorkflowTaskStatus.Process;
            //未分类
            if (categoryId.HasValue && categoryId.Value < 0)
            {
                predicate = predicate.And(m => m.Process.CategoryId == null);
            }
            //已分类
            else if (categoryId.HasValue && categoryId.Value > 0)
            {
                predicate = predicate.And(m => m.Process.CategoryId.Equals(categoryId.Value));
            }

            if (!string.IsNullOrWhiteSpace(nodeName))
            {
                predicate = predicate.And(m => m.Name.Contains(nodeName));
            }
            if (!string.IsNullOrWhiteSpace(userName))
            {
                predicate = predicate.And(m => m.UserNames.Contains(userName));
            }

            if (status.HasValue)
            {
                predicate = predicate.And(m => m.TaskStatus == status.Value);
            }

            var data = _wfProTaskRepository.FindPagenatedListWithCount(pageIndex, pageSize, predicate, m => m.CreatedDate, false);
            var total = data.Item1;
            var rows = data.Item2;
            var model = new PaginatedBaseDTO<WorkflowProTaskDTO>(pageIndex, pageSize, total, _mapper.Map<List<WorkflowProTaskDTO>>(rows).ToList());
            return model;
        }
        /// <summary>
        /// 获取当前用户的可执行任务
        /// </summary>
        /// <param name="pageIndex">分页页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="userId">当前用户的UserId</param>
        /// <param name="taskName">任务名称</param>
        /// <param name="status">任务状态</param>
        /// <returns></returns>
        public PaginatedBaseDTO<WorkflowProTaskDTO> FindPagenatedMyTasksByFilter(int pageIndex, int pageSize, string userId, string taskName, WorkflowTaskStatus? status)
        {
            //获取节点类型为任务，并且为未处理的任务状态
            Expression<Func<WorkflowProTask, bool>> predicate = m => m.NodeType == WorkflowNodeType.Task && m.TaskStatus != WorkflowTaskStatus.UnProcess;
            if (!string.IsNullOrWhiteSpace(userId))
            {
                predicate = predicate.And(m => m.UserIds.Contains(userId));
            }
            if (!string.IsNullOrWhiteSpace(taskName))
            {
                predicate = predicate.And(m => m.Name.Contains(taskName));
            }
            if (status.HasValue)
            {
                predicate = predicate.And(m => m.TaskStatus == status.Value);
            }

            var data = _wfProTaskRepository.FindPagenatedListWithCount(pageIndex, pageSize, predicate, m => m.CreatedDate, false);
            var total = data.Item1;
            var rows = data.Item2;
            var model = new PaginatedBaseDTO<WorkflowProTaskDTO>(pageIndex, pageSize, total, _mapper.Map<List<WorkflowProTaskDTO>>(rows).ToList());
            return model;
        }

        /// <summary>
        /// 获取用户的可执行任务列表
        /// </summary>
        /// <param name="currentUserId">当前用户Id</param>
        /// <param name="currentUserRoleIds">当前用户所属角色Id列表</param>
        /// <param name="currentUserOrgCodes">当前用户所属部门Id列表</param>
        /// <returns></returns>
        public async Task<List<WorkflowProTaskDTO>> FindUserWorkflowTasksAsync(string currentUserId, List<string> currentUserRoleIds, List<string> currentUserOrgCodes)
        {
            if (currentUserId.IsNullOrEmpty())
                throw new ArgumentNullException("currentUserId", "未传入查询任务的用户Id。");

            Expression<Func<WorkflowProTask, bool>> predicate = m => !m.IsDeleted && m.TaskStatus == WorkflowTaskStatus.Process;
            //当前用户为未处理该任务的用户
            predicate = predicate.And(m => m.UnProcessUserIds.Contains(currentUserId));
            //数据权限判断
            Expression<Func<WorkflowProTask, bool>> dataPredicate = Util.DataPermitUtil.GetDatePermitPredicate<WorkflowProTask>(currentUserId, currentUserRoleIds, currentUserOrgCodes);

            predicate = predicate.And(dataPredicate);

            var model = await _wfProTaskRepository.FindAllAsync(predicate);
            return _mapper.Map<List<WorkflowProTaskDTO>>(model);
        }
        #endregion

        #region 获取流程实例
        /// <summary>
        /// 根据流程实例编码，获取流程实例
        /// </summary>
        /// <param name="code">流程实例编码</param>
        /// <returns>流程实例</returns>
        public async Task<WorkflowProcessDTO> GetWorkflowProcessByCodeAsync(string code)
        {
            var data = await _wfProcessRepository.GetWorkflowProcessDetailByCodeAsync(code);
            return _mapper.Map<WorkflowProcessDTO>(data);
        }

        /// <summary>
        /// 获取流程实例的开始任务
        /// </summary>
        /// <param name="code">流程实例的编码</param>
        /// <returns>返回开始任务的双向链表结果数据</returns>
        public async Task<WorkflowProTaskDTO> GetStartTaskByProcessCodeAsync(string code)
        {
            var model = await _wfProTaskRepository.FindAllWfTaskDetailsByProcessCodeAsync(code);
            var data = _mapper.Map<List<WorkflowProTaskDTO>>(model);
            foreach (var task in data)
            {
                ProcessLinkNodeTaskDTOs(task, data);
            }
            return data.FirstOrDefault(m => m.NodeType == Enums.Workflow.WorkflowNodeType.Start); ;
        }
        /// <summary>
        /// 获取流程实例的当前任务
        /// </summary>
        /// <param name="code">流程实例的编码</param>
        /// <returns>返回当前任务的双向链表结果数据</returns>
        public async Task<WorkflowProTaskDTO> GetCurrentTaskByProcessCodeAsync(string code)
        {
            var model = await _wfProTaskRepository.FindAllAsync(m => m.Process.Code.Equals(code));
            var data = _mapper.Map<List<WorkflowProTaskDTO>>(model);
            foreach (var task in data)
            {
                ProcessLinkNodeTaskDTOs(task, data);
            }
            return data.FirstOrDefault(m => m.TaskStatus == Enums.Workflow.WorkflowTaskStatus.Process);
        }
        /// <summary>
        /// 获取流程实例的下一任务
        /// </summary>
        /// <param name="code">流程实例的编码</param>
        /// <returns>返回下一任务的双向链表结果数据</returns>
        public async Task<WorkflowProTaskDTO> GetNextTaskByProcessCodeAsync(string processCode)
        {
            var processData = await _wfProcessRepository.GetWorkflowProcessDetailByCodeAsync(processCode);
            if (processData == null)
                throw new ArgumentNullException("code", string.Format("未找到流程实例【Code={0}】的记录", processCode));

            var model = await _wfProTaskRepository.FindAllAsync(m => m.Process.Code.Equals(processCode));
            var currentTask = model.FirstOrDefault(m => m.TaskStatus == WorkflowTaskStatus.Process);
            var nextTask = GetNextTask(currentTask, processData.Context, processData.Tasks);

            var data = _mapper.Map<List<WorkflowProTaskDTO>>(model);
            foreach (var task in data)
            {
                ProcessLinkNodeTaskDTOs(task, data);
            }
            return data.FirstOrDefault(m => m.Id == nextTask.Id);
        }

        /// <summary>
        /// 根据任务列表数据，生成任务的链表任务数据
        /// </summary>
        /// <param name="task">当前任务（ParentId==null）</param>
        /// <param name="allTasks">所有的任务数据</param>
        private void ProcessLinkNodeTaskDTOs(WorkflowProTaskDTO task, List<WorkflowProTaskDTO> allTasks)
        {
            if (!string.IsNullOrEmpty(task.PrevNodeCode))
            {
                var prevTask = allTasks.FirstOrDefault(m => m.Code.Equals(task.PrevNodeCode, StringComparison.OrdinalIgnoreCase));
                if (prevTask != null)
                {
                    task.PrevNode = prevTask;
                    task.PrevNodeId = prevTask.Id;
                    task.PrevNodeCode = prevTask.Code;
                }
            }

            if (!string.IsNullOrEmpty(task.NextNodeCode))
            {
                var nextTask = allTasks.FirstOrDefault(m => m.Code.Equals(task.NextNodeCode, StringComparison.OrdinalIgnoreCase));
                if (nextTask != null)
                {
                    task.NextNode = nextTask;
                    task.NextNodeId = nextTask.Id;
                    task.NextNodeCode = nextTask.Code;
                }
            }

            if (!string.IsNullOrEmpty(task.ReturnNodeCode))
            {
                var returnTask = allTasks.FirstOrDefault(m => m.Code.Equals(task.ReturnNodeCode, StringComparison.OrdinalIgnoreCase));
                if (returnTask != null)
                {
                    task.ReturnNode = returnTask;
                    task.ReturnNodeId = returnTask.Id;
                    task.ReturnNodeCode = returnTask.Code;
                }
            }
        }

        /// <summary>
        /// 根据传入以流程链数据结构的任务对象，获取其后的所有节点信息数据</br>
        ///     结合获取流程链数据的方法使用：GetStartTaskByProcessCodeAsync、GetCurrentTaskByProcessCodeAsync</br>
        ///     结果实例：任务节点【名称：node4-task-2，状态：处理中，类型：单人审批】-->任务节点【名称：node5-task-2，状态：未处理，类型：单人审批】-->结束节点【名称：node7-end-2，状态：未处理，类型：单人审批】
        /// </summary>
        /// <param name="startTask">
        /// 传入的任务数据(流程链数据结构)</br>
        /// 结合获取流程链数据的方法使用：GetStartTaskByProcessCodeAsync、GetCurrentTaskByProcessCodeAsync
        /// </param>
        /// <returns>返回实例：任务节点【名称：node4-task-2，状态：处理中，类型：单人审批】-->任务节点【名称：node5-task-2，状态：未处理，类型：单人审批】-->结束节点【名称：node7-end-2，状态：未处理，类型：单人审批】 </returns>
        public string PrintProcessLinkedTask(WorkflowProTaskDTO startTask)
        {
            if (startTask == null) return "";

            var message = startTask.NodeType == Enums.Workflow.WorkflowNodeType.Task 
                ? string.Format("{0}节点【名称：{1}，状态：{2}，类型：{3}，执行人设置：{4}，【已同意：{5}，未同意：{6}，未处理：{7}】】", 
                    startTask.NodeType.ToDescription(), 
                    startTask.Name, 
                    startTask.TaskStatus.ToDescription(), 
                    startTask.Type.ToDescription(), 
                    startTask.ExecutorSetting.ToDescription(), 
                    startTask.AgreeUserNames, 
                    startTask.DisagreeUserNames, 
                    startTask.UnProcessUserNames)
                : string.Format("{0}节点【名称：{1}，状态：{2}，类型：{3}】", 
                    startTask.NodeType.ToDescription(), 
                    startTask.Name, 
                    startTask.TaskStatus.ToDescription(), 
                    startTask.Type.ToDescription());
            if (startTask.NodeType != Enums.Workflow.WorkflowNodeType.End)
            {
                message += "-->" + PrintProcessLinkedTask(startTask.NextNode);
            }
            else
            {
                message += PrintProcessLinkedTask(startTask.NextNode);
            }
            return message;
        }
        #endregion

        #region 获取流程实例相关表单及审批记录
        /// <summary>
        /// 根据流程实例Id，获取所有的表单数据
        /// </summary>
        /// <param name="processId">流程实例Id</param>
        /// <returns></returns>
        public List<WorkflowProFieldDTO> FindAllWfProcessFieldsByProcessId(Guid processId)
        {
            Expression<Func<WorkflowProField, bool>> predicate = m => !m.IsDeleted && m.ProcessId == processId;
            var data = _wfProFieldRepository.FindTreeNodesWithNestParentAndChildByFilter(predicate);
            return _mapper.Map<List<WorkflowProFieldDTO>>(data);
        }

        /// <summary>
        /// 获取流程实例的审批记录
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="processId">流程实例Id</param>
        /// <param name="taskId">任务编码</param>
        /// <param name="userName">执行人姓名</param>
        /// <param name="status">任务状态</param>
        /// <returns></returns>
        public PaginatedBaseDTO<WorkflowProTaskExecuteDTO> FindPagenatedWorkflowTaskExecutesByFilter(int pageIndex, int pageSize, Guid? processId, Guid? taskId, string userName, WorkflowTaskStatus? status)
        {
            //获取节点类型为任务，并且为未处理的任务状态
            Expression<Func<WorkflowProTaskExecute, bool>> predicate = m => true;
            if (processId.HasValue)
            {
                predicate = predicate.And(m => m.Task.Process.Id.Equals(processId.Value));
            }
            if (taskId.HasValue)
            {
                predicate = predicate.And(m => m.TaskId.Equals(taskId.Value));
            }
            if (!string.IsNullOrWhiteSpace(userName))
            {
                predicate = predicate.And(m => m.ExecuteUserName.Contains(userName));
            }
            if (status.HasValue)
            {
                predicate = predicate.And(m => m.ExecuteStatus.Equals(status.Value));
            }

            var data = _wfProTaskExecuteRepository.FindPagenatedListWithCount(pageIndex, pageSize, predicate, m => m.ExecuteDateTime, false);
            var total = data.Item1;
            var rows = data.Item2;
            var model = new PaginatedBaseDTO<WorkflowProTaskExecuteDTO>(pageIndex, pageSize, total, _mapper.Map<List<WorkflowProTaskExecuteDTO>>(rows).ToList());
            return model;
        }

        #endregion

        #region 删除流程实例
        public async Task<bool> RemoveWorkflowProcessWithTasksByIdAsync(Guid wfDefId)
        {
            return await _wfProcessRepository.RemoveByIdAsync(wfDefId);
        }

        public async Task<bool> RemoveWorkflowProcessWithTasksByCodeAsync(string code)
        {
            return await _wfProcessRepository.RemoveAsync(m => m.Code.Equals(code)) >= 0;
        }
        #endregion
    }
}
