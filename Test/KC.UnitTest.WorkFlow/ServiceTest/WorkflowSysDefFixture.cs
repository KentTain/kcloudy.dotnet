using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Service.DTO.Account;
using KC.Service.DTO.Workflow;
using KC.Service.Enums.Workflow;
using KC.Service.WebApiService.Business;
using KC.Service.Workflow;
using KC.Service.Workflow.DTO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KC.UnitTest.Workflow
{
    /// <summary>
    /// 测试前初始化的测试数据</br>
    ///     所有测试单元启动前只初始化一次数据，所有测试单元结束后进行删除
    /// </summary>
    public class WorkflowSysDefFixture : CommonFixture
    {
        private static Tenant currentTenant = TenantConstant.DbaTenantApiAccessInfo;
        #region 流程定义（WorkflowDefinition）

        /// <summary>
        /// 单人审批：Admin有三个任务节点，Test有两个任务节点 </br>
        /// 流程定义：Start:node1->Task:node2->End:node9
        /// </summary>
        public static Guid wfDefaultDefId_1 = new Guid("483EC2E2-309F-474C-A392-FAC3EFB0B44A");
        /// <summary>
        /// 流程编码：483EC2E2-309F-474C-A392-FAC3EFB0B44A </br>
        /// 单人审批：Admin有三个任务节点，Test有两个任务节点 </br>
        /// 流程定义：Start:node1->Task:node2->End:node9
        /// </summary>
        public static WorkflowDefinitionDTO WorkflowDefaultDef_1
        {
            get
            {
                var oauthAddress = GlobalConfig.SSOWebDomain + "connect/token";
                var oauthKey = TenantConstant.GetClientIdByTenantName(currentTenant.TenantName);
                var oauthSecret = TenantConstant.GetClientSecretByTenantNameAndKey(currentTenant.TenantName, currentTenant.PrivateEncryptKey);
                var wfDefinition = new WorkflowDefinitionDTO()
                {
                    Id = wfDefaultDefId_1,
                    Code = "wfd2021010100001",
                    Version = "wfv2021010100001",
                    Name = "员工入职审批",
                    Status = WorkflowBusStatus.Approved,
                    DefDeadlineInterval = 7,
                    DefMessageTemplateCode = "mgt2012020200001",
                    Description = "员工入职审批",
                    SecurityType = SecurityType.OAuth,
                    AuthAddress = oauthAddress,
                    AuthScope = ApplicationConstant.AccScope,
                    AuthKey = oauthKey,
                    AuthSecret = oauthSecret,
                    IsDeleted = false,
                    CreatedBy = RoleConstants.AdminUserId,
                    CreatedName = RoleConstants.AdminUserName,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = RoleConstants.AdminUserId,
                    ModifiedName = RoleConstants.AdminUserName,
                    ModifiedDate = DateTime.UtcNow,
                };

                var node1_start = new WorkflowDefNodeDTO()
                {
                    Id = Guid.NewGuid(),
                    Code = "wfn2021010100001",
                    Name = "开始",
                    NodeType = WorkflowNodeType.Start,
                    Type = WorkflowType.SingleLine,
                    LocTop = 120,
                    LocLeft = 240,
                    ExecutorSetting = ExecutorSetting.Executor,
                    OrgCodes = null,
                    OrgNames = null,
                    RoleIds = null,
                    RoleNames = null,
                    UserIds = null,
                    UserNames = null,
                    IsDeleted = false,
                    CreatedBy = RoleConstants.AdminUserId,
                    CreatedName = RoleConstants.AdminUserName,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = RoleConstants.AdminUserId,
                    ModifiedName = RoleConstants.AdminUserName,
                    ModifiedDate = DateTime.UtcNow,
                };
                var node2_task = new WorkflowDefNodeDTO()
                {
                    Id = Guid.NewGuid(),
                    Code = "wfn2012020200002",
                    Name = "人事审核",
                    Type = WorkflowType.SingleLine,
                    NodeType = WorkflowNodeType.Task,
                    LocTop = 240,
                    LocLeft = 210,
                    ExecutorSetting = ExecutorSetting.Executor,
                    OrgCodes = OrganizationConstants.人事部_Code,
                    OrgNames = "人事部",
                    RoleIds = RoleConstants.HrManagerRoleId + ", " + RoleConstants.HrRoleId,
                    RoleNames = "人事经理, 人事助理",
                    UserIds = null,
                    UserNames = null,
                    IsDeleted = false,
                    CreatedBy = RoleConstants.AdminUserId,
                    CreatedName = RoleConstants.AdminUserName,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = RoleConstants.AdminUserId,
                    ModifiedName = RoleConstants.AdminUserName,
                    ModifiedDate = DateTime.UtcNow,
                };
                var node9_end = new WorkflowDefNodeDTO()
                {
                    Id = Guid.NewGuid(),
                    Code = "wfn2012020200009",
                    Name = "结束",
                    Type = WorkflowType.SingleLine,
                    NodeType = WorkflowNodeType.End,
                    LocTop = 360,
                    LocLeft = 240,
                    ExecutorSetting = ExecutorSetting.Executor,
                    OrgCodes = null,
                    OrgNames = null,
                    RoleIds = null,
                    RoleNames = null,
                    UserIds = null,
                    UserNames = null,
                    IsDeleted = false,
                    CreatedBy = RoleConstants.AdminUserId,
                    CreatedName = RoleConstants.AdminUserName,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = RoleConstants.AdminUserId,
                    ModifiedName = RoleConstants.AdminUserName,
                    ModifiedDate = DateTime.UtcNow,
                };

                //流程图：Start:node1->Task:node2->End:node9
                node1_start.WorkflowDefId = wfDefinition.Id;
                node1_start.NextNodeCode = node2_task.Code;

                node2_task.WorkflowDefId = wfDefinition.Id;
                node2_task.PrevNodeCode = node1_start.Code;
                node2_task.NextNodeCode = node9_end.Code;

                node9_end.WorkflowDefId = wfDefinition.Id;
                node9_end.PrevNodeCode = node2_task.Code;

                var field1 = new WorkflowDefFieldDTO()
                {
                    Id = 0,
                    WorkflowDefId = wfDefinition.Id,
                    ParentId = null,
                    Text = "MemberId",
                    DataType = AttributeDataType.String,
                    Value = "【测试】员工编号",
                    DisplayName = "员工编号",
                    Description = "员工编号：int类型，USR2018120100001",
                    Leaf = true,
                    Level = 1,
                    CanEdit = false,
                    IsPrimaryKey = true,
                    IsCondition = false,
                    IsExecutor = false,
                    IsDeleted = false,
                    CreatedBy = RoleConstants.AdminUserId,
                    CreatedName = RoleConstants.AdminUserName,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = RoleConstants.AdminUserId,
                    ModifiedName = RoleConstants.AdminUserName,
                    ModifiedDate = DateTime.UtcNow,
                };
                var field2 = new WorkflowDefFieldDTO()
                {
                    Id = 0,
                    WorkflowDefId = wfDefinition.Id,
                    ParentId = null,
                    Text = "DisplayName",
                    DataType = AttributeDataType.String,
                    Value = "【测试】员工姓名",
                    DisplayName = "员工姓名",
                    Description = "员工姓名：string类型",
                    Leaf = true,
                    Level = 1,
                    CanEdit = false,
                    IsPrimaryKey = false,
                    IsCondition = false,
                    IsExecutor = true,
                    IsDeleted = false,
                    CreatedBy = RoleConstants.AdminUserId,
                    CreatedName = RoleConstants.AdminUserName,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = RoleConstants.AdminUserId,
                    ModifiedName = RoleConstants.AdminUserName,
                    ModifiedDate = DateTime.UtcNow,
                };
                var field3 = new WorkflowDefFieldDTO()
                {
                    Id = 0,
                    WorkflowDefId = wfDefinition.Id,
                    ParentId = null,
                    Text = "PhoneNumber",
                    DataType = AttributeDataType.String,
                    Value = "【测试】员工手机号",
                    DisplayName = "员工手机号",
                    Description = "员工手机号：string类型",
                    Leaf = true,
                    Level = 1,
                    CanEdit = false,
                    IsPrimaryKey = false,
                    IsCondition = false,
                    IsExecutor = true,
                    IsDeleted = false,
                    CreatedBy = RoleConstants.AdminUserId,
                    CreatedName = RoleConstants.AdminUserName,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = RoleConstants.AdminUserId,
                    ModifiedName = RoleConstants.AdminUserName,
                    ModifiedDate = DateTime.UtcNow,
                };
                var field4 = new WorkflowDefFieldDTO()
                {
                    Id = 0,
                    WorkflowDefId = wfDefinition.Id,
                    ParentId = null,
                    Text = "Email",
                    DataType = AttributeDataType.String,
                    Value = "【测试】员工邮箱",
                    DisplayName = "员工邮箱",
                    Description = "员工邮箱：string类型",
                    Leaf = true,
                    Level = 1,
                    CanEdit = false,
                    IsPrimaryKey = false,
                    IsCondition = false,
                    IsExecutor = true,
                    IsDeleted = false,
                    CreatedBy = RoleConstants.AdminUserId,
                    CreatedName = RoleConstants.AdminUserName,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = RoleConstants.AdminUserId,
                    ModifiedName = RoleConstants.AdminUserName,
                    ModifiedDate = DateTime.UtcNow,
                };

                wfDefinition.WorkflowNodes = new List<WorkflowDefNodeDTO>()
                {
                    node1_start, node2_task, node9_end
                };
                wfDefinition.WorkflowFields = new List<WorkflowDefFieldDTO>()
                {
                    field1, field2, field3, field4
                };
                return wfDefinition;
            }
        }

        #endregion

        /// <summary>
        /// 说明：单人审批测试， </br>
        ///     （小肖）启动后，而后由（肖经理）执行单人审核任务
        /// 使用流程定义1【483EC2E2-309F-474C-A392-FAC3EFB0B44A】生成流程实例，流程说明如下：
        ///     流程图：Start:node1->Task:node2->End:node9
        ///     流程类型：Task:node2：WorkFlowType.SingleLine
        ///     执行人设置：Task:node2【CreatorManager：发起人的主管--销售经理：齐副经理, 肖经理】 </br>
        ///     流程实例使用表单：wfProcessForm
        /// 执行流程
        ///     启动后的流程图：Start:node1->Task:node2(true)->End:node9 </br>
        ///     流转后的流程图：Start:node1->Task:node2->End:node9
        /// </summary>
        public static string wfDefaultInstance_1;

        /// <summary>
        /// 是否开启删除初始化数据
        /// </summary>
        private const bool IsDeleteInitData = false;
        public override void SetUpData()
        {
            //InjectTenant(TestTenant);
            InjectTenant(DbaTenant);

            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                var cache = scope.ServiceProvider.GetService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
                Service.CacheUtil.Cache = cache;
            }

            #region AutoMapper对象注入
            var profiles = KC.Service.AutoMapper.AutoMapperConfiguration.GetAllProfiles()
                .Union(KC.Service.Workflow.AutoMapper.AutoMapperConfiguration.GetAllProfiles());

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfiles(profiles);
            });
            Services.AddSingleton(config);
            var mapper = config.CreateMapper();
            Services.AddSingleton(mapper);

            #endregion

            KC.Service.Util.DependencyInjectUtil.InjectService(Services);
            KC.Service.Workflow.DependencyInjectUtil.InjectService(Services);

            var users = new List<UserSimpleDTO>() {
                new UserSimpleDTO()
                {
                    UserId = WorkflowFixture.wfItAdminUserId,
                    UserName = WorkflowFixture.wfItAdminUserName,
                    DisplayName = WorkflowFixture.wfItAdminUserName,
                    PhoneNumber = "17744949695",
                    Email = "kcloudy@163.com",
                    Status = WorkflowBusStatus.Approved
                },
                new UserSimpleDTO()
                {
                    UserId = WorkflowFixture.wfItTestUserId,
                    UserName = "tester",
                    DisplayName = "tester",
                    PhoneNumber = "17744949695",
                    Email = "kcloudy@163.com",
                    Status = WorkflowBusStatus.Disagree
                }
            };
            var orgs = new List<OrganizationSimpleDTO>() {
                new OrganizationSimpleDTO()
                {
                    Id = 1,
                    ParentId = null,
                    Text = "测试组织",
                    OrganizationCode = "TestOrgCode",
                    Users = new List<UserSimpleDTO>() {
                        new UserSimpleDTO()
                        {
                            UserId = WorkflowFixture.wfItAdminUserId,
                            UserName = WorkflowFixture.wfItAdminUserName,
                            DisplayName = WorkflowFixture.wfItAdminUserName,
                            PhoneNumber = "17744949695",
                            Email = "kcloudy@163.com",
                            Status = WorkflowBusStatus.Approved
                        },
                        new UserSimpleDTO()
                        {
                            UserId = WorkflowFixture.wfItTestUserId,
                            UserName = "tester",
                            DisplayName = "tester",
                            PhoneNumber = "17744949695",
                            Email = "kcloudy@163.com",
                            Status = WorkflowBusStatus.Disagree
                        }
                    }
                }
            };

            //var mockAccountApiService = new Moq.Mock<IAccountApiService>();
            //mockAccountApiService.Setup(m => m.LoadUsersByIdsAndRoleIdsAndOrgCodes(null).Result).Returns(users);
            //mockAccountApiService.Setup(m => m.LoadOrganizationsWithUsersByUserId(WorkflowFixture.wfItTestUserId).Result).Returns(orgs);
            //Services.AddScoped(typeof(IAccountApiService), serviceProvider => mockAccountApiService.Object);

            Services.AddScoped<IAccountApiService, MockAccountApiService>();

            _logger = LoggerFactory.CreateLogger(nameof(WorkflowFixture));
            _serviceProvider = Services.BuildServiceProvider();

            var isSuccess = true;
            //新增流程定义：483EC2E2-309F-474C-A392-FAC3EFB0B44A
            using (var scope = _serviceProvider.CreateScope())
            {
                var wfService = scope.ServiceProvider.GetService<IWorkflowDefinitionService>();
                isSuccess = wfService.SaveWorkflowDefinitionWithFieldsAndNodesAsync(WorkflowDefaultDef_1).Result;
            }

            

            _logger.LogInformation("----生成流程定义【" + wfDefaultDefId_1 + "】 ? " + isSuccess.ToString());
        }

        /* 删除脚本：流程定义数据
        SELECT * FROM [cTest].[wf_WorkflowDefinition]
        SELECT * FROM [cTest].[wf_WorkflowDefField]
        SELECT * FROM [cTest].[wf_WorkflowDefNode]
        SELECT * FROM [cTest].[wf_WorkflowDefNodeRule]
        GO

        delete [cTest].[wf_WorkflowDefField] where ISNULL(ParentId, 1)!=1
        GO
        delete [cTest].[wf_WorkflowDefField] where ISNULL(ParentId, 1)=1
        delete [cTest].[wf_WorkflowDefNodeRule]
        delete [cTest].[wf_WorkflowDefNode]
        delete [cTest].[wf_WorkflowDefinition]
        GO
        */
        /* 删除脚本：流程版本数据
        SELECT * FROM [cTest].[wf_WorkflowVerDefinition]
        SELECT * FROM [cTest].[wf_WorkflowVerDefField]
        SELECT * FROM [cTest].[wf_WorkflowVerDefNode]
        SELECT * FROM [cTest].[wf_WorkflowVerDefNodeRule]
        GO

        delete [cTest].[wf_WorkflowVerDefField] where ISNULL(ParentId, 1)!=1
        GO
        delete [cTest].[wf_WorkflowVerDefField] where ISNULL(ParentId, 1)=1
        delete [cTest].[wf_WorkflowVerDefNodeRule]
        delete [cTest].[wf_WorkflowVerDefNode]
        delete [cTest].[wf_WorkflowVerDefinition]
        GO
        */
        /* 删除脚本：流程实例数据
        SELECT * FROM [cTest].[wf_WorkflowProcess]
        SELECT * FROM [cTest].[wf_WorkflowProField]
        SELECT * FROM [cTest].[wf_WorkflowProTask] order by ProcessId,CreatedDate
        SELECT * FROM [cTest].[wf_WorkflowProTaskRule]
        GO

        delete [cTest].[wf_WorkflowProField] where ISNULL(ParentId, 1)!=1
        GO
        delete [cTest].[wf_WorkflowProField] where ISNULL(ParentId, 1)=1
        delete [cTest].[wf_WorkflowProTaskRule]
        delete [cTest].[wf_WorkflowProTaskExecute]
        delete [cTest].[wf_WorkflowProTask]
        delete [cTest].[wf_WorkflowProcess]
        GO
        */
        public override void TearDownData()
        {
            try
            {
                if (IsDeleteInitData)
                {
                    //删除生成的流程定义数据：
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var wfService = scope.ServiceProvider.GetService<IWorkflowDefinitionService>();
                        var success1 = wfService.RemoveWorkflowDefinitionWithFieldsAsync(wfDefaultDefId_1, RoleConstants.AdminUserId, RoleConstants.AdminUserName).Result;
                        
                        _logger.LogInformation("----删除流程实定义【" + wfDefaultDefId_1 + "】成功");
                    }

                    //删除生成的流程实例数据：
                    using (var scope = Services.BuildServiceProvider().CreateScope())
                    {
                        var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                        var success1 = wfService.RemoveWorkflowProcessWithTasksByCodeAsync(wfDefaultInstance_1).Result;
                        _logger.LogInformation("----删除流程实例【" + wfDefaultInstance_1 + "】成功。");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

    }
}
