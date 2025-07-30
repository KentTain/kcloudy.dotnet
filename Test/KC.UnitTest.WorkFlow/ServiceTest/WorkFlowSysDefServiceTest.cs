using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Service.DTO.Workflow;
using KC.Service.Enums.Workflow;
using KC.Service.Workflow;
using KC.Service.Workflow.DTO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace KC.UnitTest.Workflow
{
    /// <summary>
    /// 测试以下流程方法：</br>
    /// 1. 保存流程（SaveWorkflowDefinitionWithFieldsAndNodes）
    /// 2. 发起流程（StartWorkflow）
    /// 3. 获取流程开始任务的双向流程链数据（GetStartTaskByProcessCodeAsync）
    /// 4. 获取流程当前可执行任务的双向流程链数据（GetCurrentTaskByProcessCodeAsync）
    /// 5. 执行流程（SubmitWorkflow）
    /// 6. 获取用户可执行流程（FindUserWorkflowTasksAsync）
    /// </summary>
    public class WorkFlowSysDefServiceTest : TestBase<WorkflowSysDefFixture>
    {
        private ILogger _logger;
        public WorkFlowSysDefServiceTest(WorkflowSysDefFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(WorkFlowSysDefServiceTest));
        }

        protected override void SetUp()
        {

        }

        /// <summary>
        /// 单人审批测试：wfInstance_1 </br>
        ///     流程定义-1：生成1个单人审批流程实例，启动后为一个任务节点（Task:node2） </br>
        ///     执行人：【CreatorManager：发起人的主管--销售经理：齐副经理, 肖经理】 </br>
        ///     启动后的流程图：Start:node1->Task:node2(true)->End:node9 </br>
        ///     流转后的流程图：Start:node1->Task:node2->End:node9
        /// </summary>
        [Xunit.Fact]
        public async Task Test_WfProcessService_Instance_1()
        {
            // 1. 启动流程前，获取下一任务节点【node2-task】的审批人设置下的审批数据
            var accountApiUrl = GlobalConfig.GetTenantWebApiDomain(GlobalConfig.AccWebDomain, TestTenant.TenantName);
            var wfStartForm = new WorkflowStartExecuteData()
            {
                WorkflowDefCode = "wfd2021010100001",
                // 设置应用回调Api地址
                WorkflowFormType = WorkflowFormType.ModelDefinition,
                AppAuditSuccessApiUrl = accountApiUrl + "ApproveUser?userId=" + WorkflowFixture.wfHrTestUserId,
                AppAuditReturnApiUrl = accountApiUrl + "DisagreeUserAsync?userId=" + WorkflowFixture.wfHrTestUserId,
                ExecuteRemark = "自主注册员工提交的审批流程",
                FormData = new List<WorkflowProFieldDTO>()
                {
                    new WorkflowProFieldDTO()
                    {
                        ParentId = null,
                        Text = "MemberId",
                        DataType = AttributeDataType.String,
                        Value = WorkflowFixture.wfHrTestUserId,
                        DisplayName = "员工编号",
                    },
                    new WorkflowProFieldDTO()
                    {
                        ParentId = null,
                        Text = "DisplayName",
                        DataType = AttributeDataType.String,
                        Value = "系统管理员",
                        DisplayName = "员工姓名",
                    },
                    new WorkflowProFieldDTO()
                    {
                        ParentId = null,
                        Text = "PhoneNumber",
                        DataType = AttributeDataType.String,
                        Value = "17744949695",
                        DisplayName = "员工手机号",
                    },
                    new WorkflowProFieldDTO()
                    {
                        ParentId = null,
                        Text = "Email",
                        DataType = AttributeDataType.String,
                        Value = "17744949695@139.com",
                        DisplayName = "员工邮箱",
                    }
                }
            };

            // 2. 创建流程实例：wfInstance_1，流程图：Start:node1->Task:node2(current)->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                WorkflowSysDefFixture.wfDefaultInstance_1 = wfService.StartWorkflowAsync(wfStartForm, WorkflowFixture.wfHrTestUserId, WorkflowFixture.wfHrTestUserName).Result;
                Assert.NotNull(WorkflowSysDefFixture.wfDefaultInstance_1);
                _logger.LogInformation("实例【WfInstance_1】--2. 生成流程实例instant_1：" + WorkflowSysDefFixture.wfDefaultInstance_1);
            }

            // 3. 获取流程实例：wfInstance_1
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 启动后的流程图：Start:node1->Task:node2(current)->End:node9 
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();

                #region 根据任务Code获取开始任务（GetStartTaskByProcessCodeAsync）
                var task = await wfService.GetStartTaskByProcessCodeAsync(WorkflowSysDefFixture.wfDefaultInstance_1);
                var message = wfService.PrintProcessLinkedTask(task);
                _logger.LogInformation("实例【WfInstance_1】--3. 流程实例：wfInstance_1，开始任务：" + message);
                Assert.NotNull(task);
                Assert.Equal(WorkflowNodeType.Start, task.NodeType);
                Assert.Equal(WorkflowTaskStatus.Finished, task.TaskStatus);
                var nextLevel1Task = task.NextNode;
                Assert.NotNull(nextLevel1Task);
                Assert.Equal(WorkflowNodeType.Task, nextLevel1Task.NodeType);
                Assert.Equal(WorkflowTaskStatus.Process, nextLevel1Task.TaskStatus);
                //Assert.Equal(WorkflowSysDefFixture.node2_task.Name, nextLevel1Task.Name);
                //Assert.Equal(wfStartForm.AllUserNames, nextLevel1Task.AllUserNames);
                var nextLevel2Task = task.NextNode.NextNode;
                Assert.NotNull(nextLevel2Task);
                Assert.Equal(WorkflowNodeType.End, nextLevel2Task.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, nextLevel2Task.TaskStatus);
                #endregion

                #region 根据任务Code获取当前执行的任务【启动任务后，当前任务为：node2】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowSysDefFixture.wfDefaultInstance_1);
                Assert.NotNull(currentTask);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                //Assert.Equal(WorkflowSysDefFixture.node2_task.Name, currentTask.Name);
                //Assert.Equal(wfStartForm.AllUserNames, currentTask.AllUserNames);
                Assert.NotNull(currentTask.NextNode);
                Assert.Equal(WorkflowNodeType.End, currentTask.NextNode.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 4. 流程流转前，获取下一任务节点【node2-task】的审批设置下的审批数据
            WorkflowExecuteData wfSubmitForm;
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                //获取下一任务节点【node2-task】的审批人设置下的审批数据
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                wfSubmitForm = await wfService.GetSubmitWorkflowExecutorInfoAsync(WorkflowSysDefFixture.wfDefaultInstance_1, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName);
                Assert.NotNull(wfSubmitForm);
                Assert.Equal(WorkflowNodeType.Task, wfSubmitForm.TaskType);
                //Assert.Equal(WorkflowSysDefFixture.node2_task.Name, wfSubmitForm.TaskName);
                //Assert.Equal(wfStartForm.AllUserNames, wfSubmitForm.AllUserNames);

                //node2-task--org: null; role: null; user: 齐总经理,肖经理
                _logger.LogInformation(string.Format("实例【WfInstance_1】--4. 获取流转前的下一{0}节点【{1}】的审批人设置下的审批数据：同意: 【{2}】不同意: 【{3}】未处理: 【{4}】", wfSubmitForm.TaskType, wfSubmitForm.TaskName, wfSubmitForm.AgreeUserNames, wfSubmitForm.DisagreeUserNames, wfSubmitForm.UnProcessUserNames));
            }

            // 5. 第一次处理（任经理-Submit：不同意-退回）流程实例（wfInstance_1）
            //流转后的流程图：(current)->Start:node1->Task:node2->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm.ExecuteUserId = WorkflowFixture.wfHrAdminUserId;
                wfSubmitForm.ExecuteUserName = WorkflowFixture.wfHrAdminUserName;
                wfSubmitForm.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm.ExecuteStatus = WorkflowExecuteStatus.Return;
                wfSubmitForm.ExecuteRemark = "用户：【" + wfSubmitForm.ExecuteUserName + "】执行：不同意-退回";

                _logger.LogInformation("实例【WfInstance_1】--5. 流程流转：第一次处理（肖经理-Submit：不同意-退回）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowSysDefFixture.wfDefaultInstance_1, wfSubmitForm);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【退回任务后，将任务退回至发起人，当前任务为空】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowSysDefFixture.wfDefaultInstance_1);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【WfInstance_1】--5. 流程流转后，获取流程可执行任务流程图" + message);
                Assert.Null(currentTask);
                //Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                //Assert.Equal(WorkflowSysDefFixture.node2_task.Name, currentTask.Name);
                //Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                //Assert.Equal(wfSubmitForm.AllUserNames, currentTask.AllUserNames);
                //Assert.NotNull(currentTask.NextNode);
                //Assert.Equal(WorkflowNodeType.End, currentTask.NextNode.NodeType);
                //Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 6. 第二次处理（任经理-Submit：同意）流程实例（wfInstance_1）
            //流转后的流程图：Start:node1->Task:node2->End:node9(current)
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm.ExecuteUserId = WorkflowFixture.wfHrAdminUserId;
                wfSubmitForm.ExecuteUserName = WorkflowFixture.wfHrAdminUserName;
                wfSubmitForm.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm.ExecuteStatus = WorkflowExecuteStatus.Approve;
                wfSubmitForm.ExecuteRemark = "用户：【" + wfSubmitForm.ExecuteUserName + "】执行：同意提交";

                _logger.LogInformation("实例【WfInstance_1】--6. 流程流转：第二次处理（肖经理-Submit：同意）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowSysDefFixture.wfDefaultInstance_1, wfSubmitForm);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【流转任务后流程已结束，当前任务为空】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowSysDefFixture.wfDefaultInstance_1);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【WfInstance_1】--6. 流程流转后，获取流程可执行任务流程图：" + message);
                Assert.Null(currentTask);
                #endregion
            }
        }


        protected override void TearDown()
        {

        }
    }
}
