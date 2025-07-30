using AutoMapper;
using KC.DataAccess.Workflow.Repository;
using KC.Database.EFRepository;
using KC.Database.IRepository;
using KC.Framework.Base;
using KC.Framework.Exceptions;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Model.Workflow;
using KC.Service.DTO;
using KC.Service.Workflow.DTO;
using KC.Service.EFService;
using KC.Service.Enums.Workflow;
using KC.Service.Util;
using KC.Service.WebApiService.Business;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace KC.Service.Workflow
{
    public interface IWorkflowDefinitionService : IEFService
    {
        #region 流程分类
        List<WorkflowCategoryDTO> FindAllCategoryTrees();
        List<WorkflowCategoryDTO> FindCategoryTreesByName(string name);
        List<WorkflowCategoryDTO> FindCategoryTreesByIds(List<int> orgIds);

        WorkflowCategoryDTO GetCategoryById(int id);

        bool SaveCategory(WorkflowCategoryDTO data, string currentUserId);
        bool RemoveCategory(int id);
        bool ExistCategoryName(int pid, string name);
        #endregion

        #region 流程定义
        PaginatedBaseDTO<WorkflowDefinitionDTO> FindPagenatedDefinitionsByFilter(int pageIndex, int pageSize, int? categoryId, string name, WorkflowBusStatus? status);

        Task<WorkflowDefinitionDTO> GetWorkflowDefinitionDetailAsync(Guid id);
        Task<WorkflowDefNodeDTO> GetStartNodeByWfDefIdAsync(Guid wfDefId, string currentUserId, string currentUserName);

        /// <summary>
        /// 保存流程基础数据：流程定义数据、流程表单数据
        /// </summary>
        /// <param name="model">流程定义基本数据及表单数据</param>
        /// <returns>是否成功</returns>
        Task<bool> SaveWorkflowDefinitionWithFieldsAsync(WorkflowDefinitionDTO model);
        /// <summary>
        /// 保存流程设计数据（流程节点数据）
        /// </summary>
        /// <param name="wfDefId">流程定义Id</param>
        /// <param name="wfDefName">流程定义名称</param>
        /// <param name="wfNodes">流程定义的流程节点数据</param>
        /// <param name="currentUserId">提交人UserId</param>
        /// <param name="currentUserName">提交人姓名</param>
        /// <returns>是否成功</returns>
        Task<bool> SaveWorkflowNodesAsync(Guid wfDefId, string wfDefName, List<WorkflowDefNodeDTO> WorkflowNodes, string currentUserId, string currentUserName);

        /// <summary>
        /// 保存流程基础数据：流程定义数据、流程表单数据、流程设计数据（流程节点数据）
        /// </summary>
        /// <param name="model">包括：流程定义数据、流程表单数据、流程设计数据（流程节点数据）</param>
        /// <returns>是否成功</returns>
        Task<bool> SaveWorkflowDefinitionWithFieldsAndNodesAsync(WorkflowDefinitionDTO model);
        Task<bool> RemoveWorkflowDefinitionWithFieldsAsync(Guid wfDefId, string currentUserId, string currentUserName);
        #endregion

        #region 流程表单

        List<WorkflowDefFieldDTO> FindAllWorkflowFieldsByDefId(Guid defId);
        Task<bool> RemoveWorkflowFieldAsync(int wfFieldId);
        #endregion

        #region 流程版本

        PaginatedBaseDTO<WorkflowVerDefinitionDTO> FindPagenatedWorkflowVerDefsByFilter(int pageIndex, int pageSize, int? categoryId, string name, WorkflowBusStatus? status);

        List<WorkflowVerDefFieldDTO> FindAllWorkflowVerFieldsByDefId(Guid defId);

        Task<WorkflowVerDefinitionDTO> GetWorkflowVerDefinitionDetailAsync(Guid id);

        Task<bool> RemoveWorkflowVerDefAsync(Guid wfDefId);
        #endregion

        #region 流程日志
        PaginatedBaseDTO<WorkflowDefLogDTO> FindPaginatedWorkflowDefLogs(int pageIndex, int pageSize, string name);
        #endregion
    }

    public class WorkflowDefinitionService : EFServiceBase, IWorkflowDefinitionService
    {
        private readonly IMapper _mapper;

        #region Repository
        /// <summary>
        /// ComAccountUnitOfWorkContext
        ///     use AccountUnitOfWorkContext.Commit() to save the context change(transaction).
        /// </summary>
        protected EFUnitOfWorkContextBase _unitOfWorkContext { get; private set; }

        private readonly IConfigApiService _configApiService;
        private readonly IAccountApiService _accountApiService;

        private readonly IWorkflowCategoryRepository _wfCategoryRepository;

        private readonly IWorkflowDefinitionRepository _wfDefinitionRepository;
        private readonly IDbTreeRepository<WorkflowDefField> _wfDefFieldRepository;
        private readonly IWorkflowDefNodeRepository _wfDefNodeRepository;
        private readonly IDbRepository<WorkflowDefNodeRule> _wfDefNodeRuleRepository;
        private readonly IDbRepository<WorkflowDefLog> _wfDefLogRepository;

        private readonly IWorkflowVerDefinitionRepository _wfVerDefinitionRepository;
        private readonly IDbTreeRepository<WorkflowVerDefField> _wfVerDefFieldRepository;
        private readonly IDbRepository<WorkflowVerDefNode> _wfVerDefNodeRepository;
        private readonly IDbRepository<WorkflowVerDefNodeRule> _wfVerDefNodeRuleRepository;
        public WorkflowDefinitionService(
            Tenant tenant,
            IMapper mapper,
            IConfigApiService configApiService,
            IAccountApiService accountApiService,
            EFUnitOfWorkContextBase unitOfWorkContext,

            IWorkflowCategoryRepository wfCategoryRepository,
            IWorkflowDefinitionRepository wfDefinitionRepository,
            IDbTreeRepository<WorkflowDefField> wfDefFieldRepository,
            IWorkflowDefNodeRepository wfDefNodeRepositor,
            IDbRepository<WorkflowDefNodeRule> wfDefNodeRuleRepository,
            IDbRepository<WorkflowDefLog> wfDefLogRepository,

            IWorkflowVerDefinitionRepository wfVerDefinitionRepository,
            IDbTreeRepository<WorkflowVerDefField> wfVerDefFieldRepository,
            IDbRepository<WorkflowVerDefNode> wfVerDefNodeRepository,
            IDbRepository<WorkflowVerDefNodeRule> wfVerDefNodeRuleRepository,

            System.Net.Http.IHttpClientFactory clientFactory,
            ILogger<WorkflowDefinitionService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfWorkContext = unitOfWorkContext;
            _configApiService = configApiService;
            _accountApiService = accountApiService;

            _wfCategoryRepository = wfCategoryRepository;
            _wfDefinitionRepository = wfDefinitionRepository;
            _wfDefFieldRepository = wfDefFieldRepository;
            _wfDefNodeRepository = wfDefNodeRepositor;
            _wfDefNodeRuleRepository = wfDefNodeRuleRepository;
            _wfDefLogRepository = wfDefLogRepository;

            _wfVerDefinitionRepository = wfVerDefinitionRepository;
            _wfVerDefFieldRepository = wfVerDefFieldRepository;
            _wfVerDefNodeRepository = wfVerDefNodeRepository;
            _wfVerDefNodeRuleRepository = wfVerDefNodeRuleRepository;

        }
        #endregion

        #region 流程分类
        public List<WorkflowCategoryDTO> FindAllCategoryTrees()
        {
            var data = _wfCategoryRepository.FindAllTreeNodeWithNestChild();

            return _mapper.Map<List<WorkflowCategoryDTO>>(data);
        }

        public List<WorkflowCategoryDTO> FindCategoryTreesByIds(List<int> orgIds)
        {
            Expression<Func<WorkflowCategory, bool>> predicate = m => !m.IsDeleted;
            if (orgIds.Any())
            {
                predicate = predicate.And(m => orgIds.Contains(m.Id));
            }
            var data = _wfCategoryRepository.FindTreeNodesWithNestParentAndChildByFilter(predicate);

            return _mapper.Map<List<WorkflowCategoryDTO>>(data);
        }

        public List<WorkflowCategoryDTO> FindCategoryTreesByName(string name)
        {
            Expression<Func<WorkflowCategory, bool>> predicate = m => !m.IsDeleted;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Name.Contains(name));
            }

            var data = _wfCategoryRepository.FindTreeNodesWithNestParentAndChildByFilter(predicate);
            return _mapper.Map<List<WorkflowCategoryDTO>>(data);
        }

        public WorkflowCategoryDTO GetCategoryById(int id)
        {
            var data = _wfCategoryRepository.GetById(id);
            return _mapper.Map<WorkflowCategoryDTO>(data);
        }

        public bool SaveCategory(WorkflowCategoryDTO data, string currentUserId)
        {
            Expression<Func<WorkflowCategory, bool>> predicate = c => !c.IsDeleted && c.Name == data.Text;
            if (string.IsNullOrEmpty(currentUserId))
            {
                predicate = predicate.And(c => c.CreatedBy == null);
            }
            else
            {
                predicate = predicate.And(c => c.CreatedBy == currentUserId);
            }
            if (data.Id > 0)
            {
                predicate = predicate.And(c => c.Id != data.Id);
            }
            var repeatTreeNameTemp = _wfCategoryRepository.ExistByFilter(predicate);
            if (repeatTreeNameTemp)
            {
                throw new BusinessPromptException("名称【" + data.Text + "】已存在,请重新输入！");
            }
            var model = _mapper.Map<WorkflowCategory>(data);
            model.Name = model.Name.Trim();
            WorkflowCategory parent = null;
            if (!data.IsDeleted)
            {
                if (model.ParentId.HasValue)
                {
                    parent = _wfCategoryRepository.GetById(model.ParentId.Value);
                    if (parent != null && parent.Level >= 4)
                    {
                        throw new ArgumentException(string.Format("父级:{0} 不能作为父级！", model.Name));
                    }
                    model.ParentNode = parent;
                }
            }
            if (model.Id == 0)
            {
                _wfCategoryRepository.Add(model, false);
            }
            else
            {
                _wfCategoryRepository.Modify(model, new[] { "ParentId", "Name", "Description" }, false);
            }

            var success = _unitOfWorkContext.Commit() > 0;
            //成功后更新树结构（TreeNode）中的扩展字段（TreeCode、Level、Leaf）
            if (success)
            {
                _wfCategoryRepository.UpdateExtendFields();
            }
            return success;
        }
        public bool RemoveCategory(int id)
        {
            var item = _wfCategoryRepository.GetById(id);
            item.IsDeleted = true;
            return _wfCategoryRepository.Modify(item, new[] { "IsDeleted" });
        }
        public bool ExistCategoryName(int pid, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name", "name is null or empty.");

            Expression<Func<WorkflowCategory, bool>> predicate = m => !m.IsDeleted && m.Name.Equals(name);
            if (pid == 0)
                predicate = predicate.And(c => c.ParentId == null);
            else
                predicate = predicate.And(c => c.ParentId == pid);

            return _wfCategoryRepository.ExistByFilter(predicate);
        }
        #endregion

        #region 流程定义
        public PaginatedBaseDTO<WorkflowDefinitionDTO> FindPagenatedDefinitionsByFilter(int pageIndex, int pageSize, int? categoryId, string name, WorkflowBusStatus? status)
        {
            Expression<Func<WorkflowDefinition, bool>> predicate = m => true;
            //未分类
            if (categoryId.HasValue && categoryId.Value < 0)
            {
                predicate = predicate.And(m => m.CategoryId == null);
            }
            //已分类
            else if (categoryId.HasValue && categoryId.Value > 0)
            {
                predicate = predicate.And(m => m.CategoryId.Equals(categoryId.Value));
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Name.Contains(name));
            }
            if (status.HasValue)
            {
                predicate = predicate.And(m => m.Status.Equals(status.Value));
            }

            var data = _wfDefinitionRepository.FindPagenatedDefinitionsByFilter(pageIndex, pageSize, predicate, "CreatedDate", false);
            var total = data.Item1;
            var rows = data.Item2;
            var model = new PaginatedBaseDTO<WorkflowDefinitionDTO>(pageIndex, pageSize, total, _mapper.Map<List<WorkflowDefinitionDTO>>(rows).ToList());
            return model;
        }

        public async Task<WorkflowDefinitionDTO> GetWorkflowDefinitionDetailAsync(Guid id)
        {
            var data = await _wfDefinitionRepository.GetWfDefinitionDetailByIdAsync(id);
            var allFields = data.WorkflowFields.Where(m => m.ParentId == null).ToList();
            foreach (var level1 in allFields)
            {
                TreeNodeUtil.NestTreeNode(level1, data.WorkflowFields);
            }
            data.WorkflowFields = allFields;
            return _mapper.Map<WorkflowDefinitionDTO>(data);
        }

        /// <summary>
        /// 获取流程实例的开始任务
        /// </summary>
        /// <param name="code">流程实例的编码</param>
        /// <returns></returns>
        public async Task<WorkflowDefNodeDTO> GetStartNodeByWfDefIdAsync(Guid wfDefId, string currentUserId, string currentUserName)
        {
            var model = await _wfDefNodeRepository.FindAllDetailNodesAsync(m => m.WorkflowDefId.Equals(wfDefId));
            if (model == null || !model.Any())
            {
                #region 设置默认的流程
                var startNode = new WorkflowDefNodeDTO()
                {
                    Id = Guid.NewGuid(),
                    Code = "E00001",
                    Type = WorkflowType.SingleLine,
                    NodeType = WorkflowNodeType.Start,
                    Name = "开始",
                    LocTop = 120,
                    LocLeft = 470,
                    WorkflowDefId = wfDefId,
                    IsDeleted = false,
                    CreatedBy = currentUserId,
                    CreatedName = currentUserName,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = currentUserId,
                    ModifiedName = currentUserName,
                    ModifiedDate = DateTime.UtcNow
                };
                var cNodeId = Guid.NewGuid();
                var cNodeCode = "G00001";
                var conditionNode = new WorkflowDefNodeDTO()
                {
                    Id = cNodeId,
                    Code = cNodeCode,
                    Type = WorkflowType.SingleLine,
                    NodeType = WorkflowNodeType.Condition,
                    Name = "条件",
                    LocTop = 240,
                    LocLeft = 453,
                    ExecutorSetting = ExecutorSetting.CreatorManager,
                    WorkflowDefId = wfDefId,
                    Rules = new List<WorkflowDefNodeRuleDTO>()
                    {
                        new WorkflowDefNodeRuleDTO()
                        {
                            Id = 1,
                            WorkflowNodeId = cNodeId,
                            WorkflowNodeCode = cNodeCode,
                            RuleType = RuleType.None,
                            FieldName = "Amount",
                            FieldDisplayName = "金额",
                            OperatorType = RuleOperatorType.GreaterThanAndEqual,
                            FieldValue = "10.2"
                        },
                        new WorkflowDefNodeRuleDTO()
                        {
                            Id = 2,
                            WorkflowNodeId = cNodeId,
                            WorkflowNodeCode = "G00001",
                            RuleType = RuleType.And,
                            FieldName = "Auditor",
                            FieldDisplayName = "审批人",
                            OperatorType = RuleOperatorType.Equal,
                            FieldValue = "admin"
                        },
                    },
                    IsDeleted = false,
                    CreatedBy = currentUserId,
                    CreatedName = currentUserName,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = currentUserId,
                    ModifiedName = currentUserName,
                    ModifiedDate = DateTime.UtcNow
                };
                var taskNode = new WorkflowDefNodeDTO()
                {
                    Id = Guid.NewGuid(),
                    Code = "T00001",
                    Type = WorkflowType.SingleLine,
                    NodeType = WorkflowNodeType.Task,
                    Name = "主管审批",
                    LocTop = 240,
                    LocLeft = 440,
                    ExecutorSetting = ExecutorSetting.CreatorManager,
                    WorkflowDefId = wfDefId,
                    IsDeleted = false,
                    CreatedBy = currentUserId,
                    CreatedName = currentUserName,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = currentUserId,
                    ModifiedName = currentUserName,
                    ModifiedDate = DateTime.UtcNow
                };
                var endNode = new WorkflowDefNodeDTO()
                {
                    Id = Guid.NewGuid(),
                    Code = "E00002",
                    Type = WorkflowType.SingleLine,
                    NodeType = WorkflowNodeType.End,
                    Name = "结束",
                    LocTop = 360,
                    LocLeft = 470,
                    WorkflowDefId = wfDefId,
                    IsDeleted = false,
                    CreatedBy = currentUserId,
                    CreatedName = currentUserName,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = currentUserId,
                    ModifiedName = currentUserName,
                    ModifiedDate = DateTime.UtcNow
                };

                startNode.NextNode = taskNode;
                startNode.NextNodeId = taskNode.Id;
                startNode.NextNodeCode = taskNode.Code;

                //conditionNode.PrevNode = startNode;
                //conditionNode.PrevNodeId = startNode.Id;
                //conditionNode.PrevNodeCode = startNode.Code;
                //conditionNode.NextNode = taskNode;
                //conditionNode.NextNodeId = taskNode.Id;
                //conditionNode.NextNodeCode = taskNode.Code;
                //conditionNode.ReturnNode = endNode;
                //conditionNode.ReturnNodeId = endNode.Id;
                //conditionNode.ReturnNodeCode = endNode.Code;

                taskNode.PrevNode = startNode;
                taskNode.PrevNodeId = startNode.Id;
                taskNode.PrevNodeCode = startNode.Code;
                taskNode.NextNode = endNode;
                taskNode.NextNodeId = endNode.Id;
                taskNode.NextNodeCode = endNode.Code;

                endNode.PrevNode = taskNode;
                endNode.PrevNodeId = taskNode.Id;
                endNode.PrevNodeCode = taskNode.Code;
                #endregion

                return startNode;
            }
            var data = _mapper.Map<List<WorkflowDefNodeDTO>>(model);
            foreach (var task in data)
            {
                ProcessLinkNodeTasks(task, data);
            }
            return data.FirstOrDefault(m => m.NodeType == WorkflowNodeType.Start);
        }
        /// <summary>
        /// 根据任务列表数据，生成任务的链表任务数据
        /// </summary>
        /// <param name="task">当前任务（ParentId==null）</param>
        /// <param name="allTasks">所有的任务数据</param>
        private void ProcessLinkNodeTasks(WorkflowDefNodeDTO task, List<WorkflowDefNodeDTO> allTasks)
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

        #region 分两步保存流程基础数据、流程表单数据、流程设计数据
        /// <summary>
        /// 保存流程基础数据：流程定义数据、流程表单数据
        /// </summary>
        /// <param name="model">流程定义基本数据及表单数据</param>
        /// <returns>是否成功</returns>
        public async Task<bool> SaveWorkflowDefinitionWithFieldsAsync(WorkflowDefinitionDTO model)
        {
            if (model.SecurityType == SecurityType.OAuth
                && (string.IsNullOrEmpty(model.AuthAddress) || string.IsNullOrEmpty(model.AuthScope)))
                throw new ArgumentNullException("AuthAddress、AuthScope", "当调用对外接口为：" + model.SecurityType.ToDescription() + "时，需要设置OAuth的认证地址及Scope");

            if ((model.SecurityType == SecurityType.SecurityKey || model.SecurityType == SecurityType.HeaderKey || model.SecurityType == SecurityType.OAuth)
                && (string.IsNullOrEmpty(model.AuthKey) || string.IsNullOrEmpty(model.AuthSecret)))
                throw new ArgumentNullException("AuthKey、AuthSecret", "当调用对外接口为：" + model.SecurityType.ToDescription() + "时，需要设置OAuth的认证Key及Secret");

            var data = _mapper.Map<WorkflowDefinition>(model);
            var wfId = data.Id.Equals(Guid.Empty)
                ? Guid.NewGuid()
                : data.Id;
            var wfCode = data.Code.IsNullOrEmpty()
                ? _configApiService.GetSeedCodeByName("WorkflowCode")
                : data.Code;
            var wfVersion = data.Version.IsNullOrEmpty()
                ? _configApiService.GetSeedCodeByName("WorkflowVersion")
                : data.Version;
            data.Id = wfId;
            data.Code = wfCode;
            data.Version = wfVersion;
            Logger.LogDebug(string.Format("-------开始-SaveWorkflowDefinitionWithFieldsAsync：保存流程定义【{0}】的流程表单及设计数据，提交人【{1}】",
                data.Code + "：" + data.Version + "：" + data.Name, model.ModifiedName));
            //新增流程定义（新增流程定义及相关表单数据，同时添加至流程版本数据）
            var addFields = new List<WorkflowDefField>();
            if (!model.IsEditMode)
            {
                data.Status = Framework.Base.WorkflowBusStatus.Draft;
                //保存流程表单，只保存两级的Field，超出不再保存
                var i = 1;
                foreach (var field in data.WorkflowFields)
                {
                    field.Id = 0;
                    field.WorkflowDefId = data.Id;
                    field.WorkflowDefinition = data;
                    field.ParentId = null;
                    field.ParentNode = null;
                    field.Level = 1;
                    field.Index = i;
                    addFields.Add(field);
                    i++;
                    var j = 1;
                    foreach (var cField in field.ChildNodes)
                    {
                        cField.Id = 0;
                        cField.WorkflowDefId = data.Id;
                        cField.WorkflowDefinition = data;
                        cField.ParentNode = field;
                        cField.Index = j;
                        addFields.Add(cField);
                        j++;
                    }
                }

                await _wfDefFieldRepository.AddAsync(addFields, false);
                await _wfDefinitionRepository.AddAsync(data, false);

                var log = new WorkflowDefLog()
                {
                    WorkflowDefId = wfId,
                    Code = wfCode,
                    Version = wfVersion,
                    Name = data.Name,
                    OperatorId = data.CreatedBy,
                    Operator = data.CreatedName,
                    Remark = "新增流程定义基本信息及相关表单信息。"
                };
                await _wfDefLogRepository.AddAsync(log, false);
            }
            //修改流程定义（保存新的流程版本数据，同时更新流程定义数据）
            else
            {
                data.Status = Framework.Base.WorkflowBusStatus.Draft;
                //删除老的流程表单数据，插入新的流程表单数据
                if (data.WorkflowFields.Any())
                {
                    await _wfDefFieldRepository.RemoveAsync(m => m.WorkflowDefId.Equals(data.Id), false);
                    //保存流程表单，只保存两级的Field，超出不再保存
                    var i = 1;
                    foreach (var field in data.WorkflowFields)
                    {
                        field.Id = 0;
                        field.WorkflowDefId = data.Id;
                        field.WorkflowDefinition = data;
                        field.ParentId = null;
                        field.ParentNode = null;
                        field.Level = 1;
                        field.Index = i;
                        addFields.Add(field);
                        i++;
                        var j = 1;
                        foreach (var cField in field.ChildNodes)
                        {
                            cField.Id = 0;
                            cField.WorkflowDefId = data.Id;
                            cField.WorkflowDefinition = data;
                            cField.ParentNode = field;
                            cField.Index = j;
                            addFields.Add(cField);
                            j++;
                        }
                    }
                }

                await _wfDefFieldRepository.AddAsync(addFields, false);
                await _wfDefinitionRepository.ModifyAsync(data, new string[]
                     { "CategoryId", "Name", "Status", "Description", 
                         "DefMessageTemplateCode", "DefDeadlineInterval", "SecurityType",
                         "AuthKey", "AuthSecret", "AuthScope", "AuthAddress", "AuthAddressParams",
                         "AppAuditSuccessApiUrl", "AppAuditReturnApiUrl", "AppAuditQueryString",
                         "WorkflowFormType","AppFormDetailApiUrl", "AppFormDetailQueryString",
                         "ModifiedBy", "ModifiedName", "ModifiedDate" }, false);

                var log = new WorkflowDefLog()
                {
                    WorkflowDefId = wfId,
                    Code = wfCode,
                    Version = wfVersion,
                    Name = data.Name,
                    OperatorId = data.ModifiedBy,
                    Operator = data.ModifiedName,
                    Remark = "修改流程定义基本信息及相关表单信息。"
                };
                await _wfDefLogRepository.AddAsync(log, false);
            }

            try
            {
                var success = await _unitOfWorkContext.CommitAsync() > 0;
                Logger.LogDebug(string.Format("-------结束-SaveWorkflowDefinitionWithFieldsAsync：保存流程定义【{0}】的流程表单及设计数据，提交人【{1}】，是否成功【{2}】",
                    data.Code + "：" + data.Version + "：" + data.Name, model.ModifiedName, success));
                //成功后更新树结构（TreeNode）中的扩展字段（TreeCode、Level、Leaf）
                if (success)
                {
                    await _wfDefFieldRepository.UpdateExtendFieldsByFilterAsync(m => m.WorkflowDefId.Equals(data.Id));
                }
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
        /// 保存流程设计数据（流程节点数据）
        /// </summary>
        /// <param name="wfDefId">流程定义Id</param>
        /// <param name="wfDefName">流程定义名称</param>
        /// <param name="wfNodes">流程定义的流程节点数据</param>
        /// <param name="currentUserId">提交人UserId</param>
        /// <param name="currentUserName">提交人姓名</param>
        /// <returns>是否成功</returns>
        public async Task<bool> SaveWorkflowNodesAsync(Guid wfDefId, string wfDefName, List<WorkflowDefNodeDTO> wfNodes, string currentUserId, string currentUserName)
        {
            var dbData = await _wfDefinitionRepository.GetWfDefinitionDetailByIdAsync(wfDefId);
            if (dbData == null)
                throw new ArgumentNullException("Id", "未找到该条[" + wfDefId + "]流程记录。");

            var wfNewVersion = dbData.Version.IsNullOrEmpty()
                            ? _configApiService.GetSeedCodeByName("WorkflowCode")
                            : dbData.Version;

            dbData.Name = wfDefName;
            dbData.Version = wfNewVersion;
            dbData.ModifiedName = currentUserName;
            dbData.ModifiedBy = currentUserId;
            dbData.ModifiedDate = DateTime.UtcNow;

            Logger.LogDebug(string.Format("-------开始-SaveWorkflowNodesAsync：保存流程定义【{0}】的流程节点数据，提交人【{1}】",
                dbData.Code + "：" + dbData.Version + "：" + dbData.Name, currentUserName));

            //新增流程定义（新增流程定义及相关表单数据，同时添加至流程版本数据）
            var addNodes = new List<WorkflowDefNode>();
            var addRules = new List<WorkflowDefNodeRule>();
            var addVerFields = new List<WorkflowVerDefField>();
            var addVerNodes = new List<WorkflowVerDefNode>();
            var addVerRules = new List<WorkflowVerDefNodeRule>();

            //删除老的流程节点数据，插入新的流程节点数据
            var nodes = _mapper.Map<List<WorkflowDefNode>>(wfNodes);
            if (nodes.Any())
            {
                _wfDefNodeRepository.Remove(m => m.WorkflowDefId.Equals(dbData.Id));

                //保存流程表单，只保存两级的Field，超出不再保存
                foreach (var node in nodes)
                {
                    var wfNodeCod = node.Code.IsNullOrEmpty()
                        ? _configApiService.GetSeedCodeByName("WorkflowNode")
                        : node.Code;
                    node.Id = Guid.NewGuid();
                    node.Code = wfNodeCod;
                    node.WorkflowDefId = dbData.Id;
                    //node.WorkflowDefinition = data;
                    addNodes.Add(node);
                    foreach (var rule in node.Rules)
                    {
                        rule.Id = 0;
                        rule.WorkflowNodeCode = node.Code;
                        rule.WorkflowNodeId = node.Id;
                        rule.WorkflowNode = node;
                        addRules.Add(rule);
                    }
                }
            }

            //保存流程版本数据
            var verData = _mapper.Map<WorkflowVerDefinition>(dbData);
            verData.Id = Guid.NewGuid();
            verData.Code = dbData.Code;
            verData.Version = wfNewVersion;
            verData.WorkflowVerDefId = dbData.Id;
            //保存流程表单版本数据，只保存两级的Field，超出不再保存
            var i = 1;
            foreach (var field in verData.WorkflowVerFields)
            {
                field.Id = 0;
                field.WorkflowVerDefId = verData.Id;
                field.WorkflowVerDefinition = verData;
                field.ParentId = null;
                field.ParentNode = null;
                field.Level = 1;
                field.Index = i;
                addVerFields.Add(field);
                i++;
                var j = 1;
                foreach (var cField in field.ChildNodes)
                {
                    cField.Id = 0;
                    cField.WorkflowVerDefId = verData.Id;
                    cField.WorkflowVerDefinition = verData;
                    cField.ParentNode = field;
                    cField.Index = j;
                    addVerFields.Add(cField);
                    j++;
                }
            }
            //保存流程节点版本数据
            foreach (var node in verData.WorkflowVerNodes)
            {
                node.Id = Guid.NewGuid();
                node.Code = node.Code;
                node.WorkflowVerDefId = verData.Id;
                node.WorkflowVerDefinition = verData;
                addVerNodes.Add(node);
                foreach (var rule in node.Rules)
                {
                    rule.Id = 0;
                    rule.WorkflowVerNodeId = node.Id;
                    rule.WorkflowVerNode = node;
                    rule.WorkflowVerNodeCode = node.Code;
                    addVerRules.Add(rule);
                }
            }

            try
            {
                await _wfDefNodeRepository.AddAsync(addNodes, false);
                await _wfDefinitionRepository.ModifyAsync(dbData, new string[] { "Name", "Version", "ModifiedBy", "ModifiedName", "ModifiedDate" }, false);

                await _wfVerDefinitionRepository.AddAsync(verData, false);
                await _wfVerDefFieldRepository.AddAsync(addVerFields, false);
                await _wfVerDefNodeRepository.AddAsync(addVerNodes, false);

                var log = new WorkflowDefLog()
                {
                    WorkflowDefId = wfDefId,
                    Code = dbData.Code,
                    Version = dbData.Version,
                    Name = wfDefName,
                    OperatorId = currentUserId,
                    Operator = currentUserName,
                    Remark = "保存流程设计及相关流程节点信息。"
                };
                await _wfDefLogRepository.AddAsync(log, false);

                var success = await _unitOfWorkContext.CommitAsync() > 0;
                Logger.LogDebug(string.Format("-------结束-SaveWorkflowNodesAsync：保存流程定义【{0}】的流程节点数据，提交人【{1}】，是否成功【{2}】",
                    dbData.Code + "：" + dbData.Version + "：" + dbData.Name, currentUserName, success));
                //成功后更新树结构（TreeNode）中的扩展字段（TreeCode、Level、Leaf）
                if (success)
                {
                    await _wfVerDefFieldRepository.UpdateExtendFieldsByFilterAsync(m => m.WorkflowVerDefId.Equals(verData.Id));
                }
                return success;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }
        #endregion

        #region 单元测试用：一次性保存流程数据（流程基础数据、流程表单数据、流程设计数据）
        /// <summary>
        /// 保存流程基础数据：流程定义数据、流程表单数据、流程设计数据（流程节点数据）
        /// </summary>
        /// <param name="model">包括：流程定义数据、流程表单数据、流程设计数据（流程节点数据）</param>
        /// <returns>是否成功</returns>
        public async Task<bool> SaveWorkflowDefinitionWithFieldsAndNodesAsync(WorkflowDefinitionDTO model)
        {
            if (model.SecurityType == SecurityType.OAuth
                && (string.IsNullOrEmpty(model.AuthAddress) || string.IsNullOrEmpty(model.AuthScope)))
                throw new ArgumentNullException("AuthAddress、AuthScope", "当调用对外接口为：" + model.SecurityType.ToDescription() + "时，需要设置OAuth的认证地址及Scope");

            if ((model.SecurityType == SecurityType.SecurityKey || model.SecurityType == SecurityType.HeaderKey || model.SecurityType == SecurityType.OAuth)
                && (string.IsNullOrEmpty(model.AuthKey) || string.IsNullOrEmpty(model.AuthSecret)))
                throw new ArgumentNullException("AuthKey、AuthSecret", "当调用对外接口为：" + model.SecurityType.ToDescription() + "时，需要设置OAuth的认证Key及Secret");

            var data = _mapper.Map<WorkflowDefinition>(model);
            var wfId = data.Id.Equals(Guid.Empty)
                ? Guid.NewGuid()
                : data.Id;
            var wfCode = data.Code.IsNullOrEmpty()
                ? _configApiService.GetSeedCodeByName("WorkflowCode")
                : data.Code;
            var wfVersion = data.Version.IsNullOrEmpty()
                ? _configApiService.GetSeedCodeByName("WorkflowVersion")
                : data.Version;
            data.Id = wfId;
            data.Code = wfCode;
            data.Version = wfVersion;

            Logger.LogDebug(string.Format("-------开始-SaveWorkflowDefinitionWithFieldsAndNodesAsync：保存流程定义【{0}】的流程基础、表单及设计数据，提交人【{1}】",
                data.Code + "：" + data.Version + "：" + data.Name, model.ModifiedName));

            //新增流程定义（新增流程定义及相关表单数据，同时添加至流程版本数据）
            var addFields = new List<WorkflowDefField>();
            var addNodes = new List<WorkflowDefNode>();
            var addRules = new List<WorkflowDefNodeRule>();
            var addVerFields = new List<WorkflowVerDefField>();
            var addVerNodes = new List<WorkflowVerDefNode>();
            var addVerRules = new List<WorkflowVerDefNodeRule>();
            if (!model.IsEditMode)
            {
                data.Status = Framework.Base.WorkflowBusStatus.Draft;
                //保存流程表单，只保存两级的Field，超出不再保存
                var i = 1;
                foreach (var field in data.WorkflowFields)
                {
                    field.Id = 0;
                    field.ParentId = null;
                    field.Level = 1;
                    field.WorkflowDefId = data.Id;
                    field.WorkflowDefinition = data;
                    field.Index = i;
                    addFields.Add(field);
                    i++;
                    var j = 1;
                    foreach (var cField in field.ChildNodes)
                    {
                        cField.Id = 0;
                        cField.WorkflowDefId = data.Id;
                        cField.WorkflowDefinition = data;
                        cField.ParentNode = field;
                        cField.Index = j;
                        addFields.Add(cField);
                        j++;
                    }
                }

                //保存流程节点
                foreach (var node in data.WorkflowNodes)
                {
                    var wfNodeCod = node.Code.IsNullOrEmpty()
                        ? _configApiService.GetSeedCodeByName("WorkflowNode")
                        : node.Code;
                    node.Id = Guid.NewGuid();
                    node.Code = wfNodeCod;
                    node.WorkflowDefId = data.Id;
                    node.WorkflowDefinition = data;
                    addNodes.Add(node);
                    foreach (var rule in node.Rules)
                    {
                        rule.Id = 0;
                        rule.WorkflowNodeId = node.Id;
                        rule.WorkflowNode = node;
                        rule.WorkflowNodeCode = node.Code;
                        addRules.Add(rule);
                    }
                }

                //保存流程版本数据
                var verData = _mapper.Map<WorkflowVerDefinition>(data);
                verData.Id = Guid.NewGuid();
                verData.Version = wfVersion;
                verData.WorkflowVerDefId = wfId;
                //保存流程表单版本数据，只保存两级的Field，超出不再保存
                foreach (var field in verData.WorkflowVerFields)
                {
                    field.WorkflowVerDefId = verData.Id;
                    field.WorkflowVerDefinition = verData;
                    addVerFields.Add(field);
                    foreach (var cField in field.ChildNodes)
                    {
                        cField.WorkflowVerDefId = verData.Id;
                        cField.WorkflowVerDefinition = verData;
                        cField.ParentNode = field;
                        addVerFields.Add(cField);
                    }
                }
                //保存流程节点版本数据
                foreach (var node in verData.WorkflowVerNodes)
                {
                    node.Id = Guid.NewGuid();
                    node.Code = node.Code;
                    node.WorkflowVerDefId = verData.Id;
                    node.WorkflowVerDefinition = verData;
                    addVerNodes.Add(node);
                    foreach (var rule in node.Rules)
                    {
                        rule.Id = 0;
                        rule.WorkflowVerNodeId = node.Id;
                        rule.WorkflowVerNode = node;
                        rule.WorkflowVerNodeCode = node.Code;
                        addVerRules.Add(rule);
                    }
                }

                await _wfDefFieldRepository.AddAsync(addFields, false);
                await _wfDefNodeRepository.AddAsync(addNodes, false);
                await _wfDefNodeRuleRepository.AddAsync(addRules, false);
                await _wfDefinitionRepository.AddAsync(data, false);

                await _wfVerDefFieldRepository.AddAsync(addVerFields, false);
                await _wfVerDefNodeRepository.AddAsync(addVerNodes, false);
                await _wfVerDefNodeRuleRepository.AddAsync(addVerRules, false);
                await _wfVerDefinitionRepository.AddAsync(verData, false);
            }
            //修改流程定义（保存新的流程版本数据，同时更新流程定义数据）
            else
            {
                var wfNewVersion = data.Version.IsNullOrEmpty()
                    ? _configApiService.GetSeedCodeByName("WorkflowCode")
                    : data.Version;

                data.Version = wfNewVersion;
                data.Status = Framework.Base.WorkflowBusStatus.Draft;
                //删除老的流程表单数据，插入新的流程表单数据
                if (data.WorkflowFields.Any())
                {
                    await _wfDefFieldRepository.RemoveAsync(m => m.WorkflowDefId.Equals(data.Id), false);
                    //保存流程表单，只保存两级的Field，超出不再保存
                    var i = 1;
                    foreach (var field in data.WorkflowFields)
                    {
                        field.Id = 0;
                        field.WorkflowDefId = data.Id;
                        field.WorkflowDefinition = data;
                        field.ParentId = null;
                        field.ParentNode = null;
                        field.Index = i;
                        addFields.Add(field);
                        i++;
                        var j = 1;
                        foreach (var cField in field.ChildNodes)
                        {
                            cField.Id = 0;
                            cField.WorkflowDefId = data.Id;
                            cField.WorkflowDefinition = data;
                            cField.ParentNode = field;
                            cField.Index = j;
                            addFields.Add(cField);
                            j++;
                        }
                    }
                }
                //删除老的流程节点数据，插入新的流程节点数据
                if (data.WorkflowNodes.Any())
                {
                    await _wfDefNodeRepository.RemoveAsync(m => m.WorkflowDefId.Equals(data.Id), false);
                    //保存流程表单，只保存两级的Field，超出不再保存
                    foreach (var node in data.WorkflowNodes)
                    {
                        var wfNodeCod = node.Code.IsNullOrEmpty()
                            ? _configApiService.GetSeedCodeByName("WorkflowNode")
                            : node.Code;
                        node.Id = Guid.NewGuid();
                        node.Code = wfNodeCod;
                        node.WorkflowDefId = data.Id;
                        node.WorkflowDefinition = data;
                        addNodes.Add(node);
                        foreach (var rule in node.Rules)
                        {
                            rule.Id = 0;
                            rule.WorkflowNodeCode = node.Code;
                            rule.WorkflowNodeId = node.Id;
                            rule.WorkflowNode = node;
                            addRules.Add(rule);
                        }
                    }
                }

                //保存流程版本数据
                var verData = _mapper.Map<WorkflowVerDefinition>(data);
                verData.Id = Guid.NewGuid();
                verData.Version = wfNewVersion;
                verData.WorkflowVerDefId = wfId;
                //保存流程表单版本数据，只保存两级的Field，超出不再保存
                foreach (var field in verData.WorkflowVerFields)
                {
                    field.Id = 0;
                    field.WorkflowVerDefId = verData.Id;
                    field.WorkflowVerDefinition = verData;
                    addVerFields.Add(field);
                    foreach (var cField in field.ChildNodes)
                    {
                        cField.Id = 0;
                        cField.WorkflowVerDefId = verData.Id;
                        cField.WorkflowVerDefinition = verData;
                        cField.ParentNode = field;
                        addVerFields.Add(cField);
                    }
                }
                //保存流程节点版本数据
                foreach (var node in verData.WorkflowVerNodes)
                {
                    node.Id = Guid.NewGuid();
                    node.Code = node.Code;
                    node.WorkflowVerDefId = verData.Id;
                    node.WorkflowVerDefinition = verData;
                    addVerNodes.Add(node);
                    foreach (var rule in node.Rules)
                    {
                        rule.Id = 0;
                        rule.WorkflowVerNodeId = node.Id;
                        rule.WorkflowVerNode = node;
                        rule.WorkflowVerNodeCode = node.Code;
                        addVerRules.Add(rule);
                    }
                }

                await _wfDefinitionRepository.ModifyAsync(data, new string[]
                     { "CategoryId", "Name", "Status", "Description",
                         "DefMessageTemplateCode", "DefDeadlineInterval", "SecurityType",
                         "AuthKey", "AuthSecret", "AuthScope", "AuthAddress", "AuthAddressParams",
                         "AppAuditSuccessApiUrl", "AppAuditReturnApiUrl", "AppAuditQueryString",
                         "WorkflowFormType","AppFormDetailApiUrl", "AppFormDetailQueryString",
                         "ModifiedBy", "ModifiedName", "ModifiedDate"  }, false);
                await _wfDefFieldRepository.AddAsync(addFields, false);
                await _wfDefNodeRepository.AddAsync(addNodes, false);
                //await _wfDefNodeRuleRepository.AddAsync(addRules, false);

                await _wfVerDefinitionRepository.AddAsync(verData, false);
                await _wfVerDefFieldRepository.AddAsync(addVerFields, false);
                await _wfVerDefNodeRepository.AddAsync(addVerNodes, false);
                //await _wfVerDefNodeRuleRepository.AddAsync(addVerRules, false);
            }
            try
            {
                var success = await _unitOfWorkContext.CommitAsync() > 0;
                Logger.LogDebug(string.Format("-------结束-SaveWorkflowDefinitionWithFieldsAndNodesAsync：保存流程定义【{0}】的流程基础、表单及设计数据，提交人【{1}】，是否成功【{2}】",
                    data.Code + "：" + data.Version + "：" + data.Name, model.ModifiedName, success));
                //成功后更新树结构（TreeNode）中的扩展字段（TreeCode、Level、Leaf）
                if (success)
                {
                    await _wfDefFieldRepository.UpdateExtendFieldsByFilterAsync(m => m.WorkflowDefId.Equals(data.Id));
                }
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
        #endregion

        public async Task<bool> RemoveWorkflowDefinitionWithFieldsAsync(Guid wfDefId, string currentUserId, string currentUserName)
        {
            var data = await _wfDefinitionRepository.GetByIdAsync(wfDefId);
            if (data == null)
                throw new ArgumentNullException("未找到该流程定义信息。");

            var success = await _wfDefinitionRepository.RemoveByIdAsync(wfDefId);
            if (success)
            {
                var log = new WorkflowDefLog()
                {
                    WorkflowDefId = data.Id,
                    Code = data.Code,
                    Version = data.Version,
                    Name = data.Name,
                    OperatorId = currentUserId,
                    Operator = currentUserName,
                    Remark = "删除了流程定义基本信息及相关表单信息。"
                };
                await _wfDefLogRepository.AddAsync(log);
            }

            return success;
        }

        #endregion

        #region 流程表单

        public List<WorkflowDefFieldDTO> FindAllWorkflowFieldsByDefId(Guid defId)
        {
            Expression<Func<WorkflowDefField, bool>> predicate = m => !m.IsDeleted && m.WorkflowDefId == defId;
            var data = _wfDefFieldRepository.FindTreeNodesWithNestParentAndChildByFilter(predicate);
            return _mapper.Map<List<WorkflowDefFieldDTO>>(data);
        }
        public async Task<bool> RemoveWorkflowFieldAsync(int wfFieldId)
        {
            return await _wfDefFieldRepository.RemoveByIdAsync(wfFieldId);
        }

        #endregion

        #region 流程版本

        public PaginatedBaseDTO<WorkflowVerDefinitionDTO> FindPagenatedWorkflowVerDefsByFilter(int pageIndex, int pageSize, int? categoryId, string name, WorkflowBusStatus? status)
        {
            Expression<Func<WorkflowVerDefinition, bool>> predicate = m => true;
            //未分类
            if (categoryId.HasValue && categoryId.Value < 0)
            {
                predicate = predicate.And(m => m.CategoryId == null);
            }
            //已分类
            else if (categoryId.HasValue && categoryId.Value > 0)
            {
                predicate = predicate.And(m => m.CategoryId.Equals(categoryId.Value));
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Name.Contains(name));
            }
            if (status.HasValue)
            {
                predicate = predicate.And(m => m.Status.Equals(status.Value));
            }

            var data = _wfVerDefinitionRepository.FindPagenatedListWithCount(pageIndex, pageSize, predicate, m => m.CreatedDate, false);
            var total = data.Item1;
            var rows = data.Item2;
            var model = new PaginatedBaseDTO<WorkflowVerDefinitionDTO>(pageIndex, pageSize, total, _mapper.Map<List<WorkflowVerDefinitionDTO>>(rows).ToList());
            return model;
        }

        public List<WorkflowVerDefFieldDTO> FindAllWorkflowVerFieldsByDefId(Guid defId)
        {
            Expression<Func<WorkflowVerDefField, bool>> predicate = m => !m.IsDeleted && m.WorkflowVerDefId == defId;
            var data = _wfVerDefFieldRepository.FindAll(predicate);
            return _mapper.Map<List<WorkflowVerDefFieldDTO>>(data);
        }

        public async Task<WorkflowVerDefinitionDTO> GetWorkflowVerDefinitionDetailAsync(Guid id)
        {
            var data = await _wfVerDefinitionRepository.GetWfVerDefinitionDetailByIdAsync(id);
            if (data == null) return null;

            var allFields = data.WorkflowVerFields.Where(m => m.ParentId == null).ToList();
            foreach (var level1 in allFields)
            {
                TreeNodeUtil.NestTreeNode(level1, data.WorkflowVerFields);
            }
            data.WorkflowVerFields = allFields;
            return _mapper.Map<WorkflowVerDefinitionDTO>(data);
        }

        public async Task<bool> RemoveWorkflowVerDefAsync(Guid wfDefId)
        {
            return await _wfVerDefinitionRepository.RemoveByIdAsync(wfDefId);
        }
        #endregion

        #region 流程日志
        public PaginatedBaseDTO<WorkflowDefLogDTO> FindPaginatedWorkflowDefLogs(int pageIndex, int pageSize, string name)
        {
            Expression<Func<WorkflowDefLog, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Operator.Contains(name));
            }

            var data = _wfDefLogRepository.FindPagenatedListWithCount(
                pageIndex, pageSize, predicate, m => m.OperateDate, false);

            var total = data.Item1;
            var rows = _mapper.Map<List<WorkflowDefLogDTO>>(data.Item2);
            return new PaginatedBaseDTO<WorkflowDefLogDTO>(pageIndex, pageSize, total, rows);

        }
        #endregion
    }
}
