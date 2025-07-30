using KC.Framework.Extension;
using KC.Service.DTO.Workflow;
using KC.Service.Enums.Workflow;
using KC.Service.Workflow;
using KC.Service.Workflow.DTO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
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
    public class WorkflowServiceTest : TestBase<WorkflowFixture>
    {
        private ILogger _logger;
        public WorkflowServiceTest(WorkflowFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(WorkflowServiceTest));
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
            WorkflowStartExecuteData wfStartForm;
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                //获取下一任务节点【node2-task】的审批人设置下的审批数据
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                wfStartForm = await wfService.GetStartWorkflowExecutorInfoAsync(WorkflowFixture.WorkflowDef_1.Code, WorkflowFixture.wfProcessForm, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName);
                Assert.NotNull(wfStartForm);
                Assert.Equal(WorkflowNodeType.Task, wfStartForm.TaskType);
                Assert.Equal(WorkflowFixture.node2_task.Name, wfStartForm.TaskName);
                // 预期的执行人：【CreatorManager：发起人(小肖)的主管--销售经理：齐副经理, 肖经理】
                Assert.Equal(WorkflowFixture.wfComSubAdminUserId + ", " + WorkflowFixture.wfSaleAdminUserId, wfStartForm.AllUserIds);

                _logger.LogInformation(string.Format("实例【WfInstance_1】--1. 预期的执行人：【CreatorManager：发起人的主管--销售经理：齐副经理, 肖经理】"));
                _logger.LogInformation(string.Format("实例【WfInstance_1】--1. 获取启动后下一{0}节点【{1}】的审批人设置下的审批数据：同意: 【{2}】不同意: 【{3}】未处理: 【{4}】", wfStartForm.TaskType, wfStartForm.TaskName, wfStartForm.AgreeUserNames, wfStartForm.DisagreeUserNames, wfStartForm.UnProcessUserNames));
            }

            // 2. 创建流程实例：wfInstance_1，流程图：Start:node1->Task:node2(current)->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 设置下一节点的审批人：齐总经理、肖经理
                var exceptExecuteUserIds = WorkflowFixture.wfComAdminUserId + ", " + WorkflowFixture.wfSaleAdminUserId;
                var exceptExecuteUserNames = WorkflowFixture.wfComAdminUserName + ", " + WorkflowFixture.wfSaleAdminUserName;
                wfStartForm.AllUserIds = exceptExecuteUserIds;
                wfStartForm.AllUserNames = exceptExecuteUserNames;
                wfStartForm.UnProcessUserIds = exceptExecuteUserIds;
                wfStartForm.UnProcessUserNames = exceptExecuteUserNames;

                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                WorkflowFixture.wfInstance_1 = wfService.StartWorkflowAsync(wfStartForm, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName).Result;
                Assert.NotNull(WorkflowFixture.wfInstance_1);
                _logger.LogInformation("实例【WfInstance_1】--2. 生成流程实例instant_1：" + WorkflowFixture.wfInstance_1);
            }

            // 3. 获取流程实例：wfInstance_1
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 启动后的流程图：Start:node1->Task:node2(current)->End:node9 
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();

                #region 根据任务Code获取开始任务（GetStartTaskByProcessCodeAsync）
                var task = await wfService.GetStartTaskByProcessCodeAsync(WorkflowFixture.wfInstance_1);
                var message = wfService.PrintProcessLinkedTask(task);
                _logger.LogInformation("实例【WfInstance_1】--3. 流程实例：wfInstance_1，开始任务：" + message);
                Assert.NotNull(task);
                Assert.Equal(WorkflowNodeType.Start, task.NodeType);
                Assert.Equal(WorkflowTaskStatus.Finished, task.TaskStatus);
                var nextLevel1Task = task.NextNode;
                Assert.NotNull(nextLevel1Task);
                Assert.Equal(WorkflowNodeType.Task, nextLevel1Task.NodeType);
                Assert.Equal(WorkflowFixture.node2_task.Name, nextLevel1Task.Name);
                Assert.Equal(WorkflowTaskStatus.Process, nextLevel1Task.TaskStatus);
                Assert.Equal(wfStartForm.AllUserNames, nextLevel1Task.AllUserNames);
                var nextLevel2Task = task.NextNode.NextNode;
                Assert.NotNull(nextLevel2Task);
                Assert.Equal(WorkflowNodeType.End, nextLevel2Task.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, nextLevel2Task.TaskStatus);
                #endregion

                #region 根据任务Code获取当前执行的任务【启动任务后，当前任务为：node2】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_1);
                Assert.NotNull(currentTask);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowFixture.node2_task.Name, currentTask.Name);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                Assert.Equal(wfStartForm.AllUserNames, currentTask.AllUserNames);
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
                wfSubmitForm = await wfService.GetSubmitWorkflowExecutorInfoAsync(WorkflowFixture.wfInstance_1, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName);
                Assert.NotNull(wfSubmitForm);
                Assert.Equal(WorkflowNodeType.Task, wfSubmitForm.TaskType);
                Assert.Equal(WorkflowFixture.node2_task.Name, wfSubmitForm.TaskName);
                Assert.Equal(wfStartForm.AllUserNames, wfSubmitForm.AllUserNames);

                //node2-task--org: null; role: null; user: 齐总经理,肖经理
                _logger.LogInformation(string.Format("实例【WfInstance_1】--4. 获取流转前的下一{0}节点【{1}】的审批人设置下的审批数据：同意: 【{2}】不同意: 【{3}】未处理: 【{4}】", wfSubmitForm.TaskType, wfSubmitForm.TaskName, wfSubmitForm.AgreeUserNames, wfSubmitForm.DisagreeUserNames, wfSubmitForm.UnProcessUserNames));
            }

            // 5. 第一次处理（肖经理-Submit：不同意-退回）流程实例（wfInstance_1）
            //流转后的流程图：(current)->Start:node1->Task:node2->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm.ExecuteUserId = WorkflowFixture.wfSaleAdminUserId;
                wfSubmitForm.ExecuteUserName = WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm.ExecuteStatus = WorkflowExecuteStatus.Return;
                wfSubmitForm.ExecuteRemark = "用户：【" + wfSubmitForm.ExecuteUserName + "】执行：不同意-退回";

                _logger.LogInformation("实例【WfInstance_1】--5. 流程流转：第一次处理（肖经理-Submit：不同意-退回）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_1, wfSubmitForm);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【退回任务后，将任务退回至发起人，当前任务为空】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_1);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【WfInstance_1】--5. 流程流转后，获取流程可执行任务流程图" + message);
                Assert.Null(currentTask);
                //Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                //Assert.Equal(WorkflowFixture.node2_task.Name, currentTask.Name);
                //Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                //Assert.Equal(wfSubmitForm.AllUserNames, currentTask.AllUserNames);
                //Assert.NotNull(currentTask.NextNode);
                //Assert.Equal(WorkflowNodeType.End, currentTask.NextNode.NodeType);
                //Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 6. 第二次处理（肖经理-Submit：同意）流程实例（wfInstance_1）
            //流转后的流程图：Start:node1->Task:node2->End:node9(current)
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm.ExecuteUserId = WorkflowFixture.wfSaleAdminUserId;
                wfSubmitForm.ExecuteUserName = WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm.ExecuteStatus = WorkflowExecuteStatus.Approve;
                wfSubmitForm.ExecuteRemark = "用户：【" + wfSubmitForm.ExecuteUserName + "】执行：同意提交";

                _logger.LogInformation("实例【WfInstance_1】--6. 流程流转：第二次处理（肖经理-Submit：同意）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_1, wfSubmitForm);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【流转任务后流程已结束，当前任务为空】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_1);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【WfInstance_1】--6. 流程流转后，获取流程可执行任务流程图：" + message);
                Assert.Null(currentTask);
                #endregion
            }
        }

        /// <summary>
        /// 多人审批(权重50%通过)测试：wfInstance_2 </br>
        ///     流程定义-1：生成1个多人审批流程实例，启动后为一个多人审批（权重50%通过）任务节点（Task:node3），而后进行三次流转处理
        ///     启动后的流程图：Start:node1->Task:node3(true)->End:node9
        ///     第一次流转(肖经理-不同意)后的流程图：Start:node1->Task:node3(true)->End:node9
        ///     第二次流转(齐副经理-同意)后的流程图：Start:node1->Task:node3(true)->End:node9
        ///     第三次流转(齐总经理-同意)后的流程图：Start:node1->Task:node3->End:node9
        /// </summary>
        [Xunit.Fact]
        public async Task Test_WfProcessService_Instance_2()
        {
            // 1. 启动流程前，获取下一任务节点【node3-task】的审批人设置下的审批数据
            WorkflowStartExecuteData wfStartForm;
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                //获取下一任务节点【node3-task】的审批人设置下的审批数据
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                wfStartForm = await wfService.GetStartWorkflowExecutorInfoAsync(WorkflowFixture.WorkflowDef_2.Code, WorkflowFixture.wfProcessForm, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName);
                Assert.NotNull(wfStartForm);
                Assert.Equal(WorkflowNodeType.Task, wfStartForm.TaskType);
                // 执行人设置：【组织/角色/用户--组织: 【企业, 销售部】角色: 【总经理, 销售经理】用户: 【肖经理】】
                // 预期的执行人【齐总经理,小齐,齐副经理,肖经理,小肖】  
                // Assert.Equal(WorkflowFixture.wfComSubAdminUserId + ", " + WorkflowFixture.wfSaleAdminUserId, wfStartForm.AllUserIds);

                _logger.LogInformation(string.Format("实例【WfInstance_2】--1. 预期的执行人：【齐总经理,小齐,齐副经理,肖经理,小肖】 "));
                //Task:node5--org: IT部; role: admin; user: admin、tester
                _logger.LogInformation(string.Format("实例【wfInstance_2】--1. 获取启动后下一{0}节点【{1}】的审批人设置下的审批数据：组织:【{2}】角色:【{3}】用户:【{4}】", wfStartForm.TaskType, wfStartForm.TaskName, wfStartForm.AgreeUserNames, wfStartForm.DisagreeUserNames, wfStartForm.UnProcessUserNames));
            }

            // 2. 创建流程实例：wfInstance_2，流程图：Start:node1->Task:node3(current)->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 设置下一节点的审批人：总经理：齐总经理、销售经理：齐副经理, 肖经理
                var exceptExecuteUserIds = WorkflowFixture.wfComAdminUserId + ", " + WorkflowFixture.wfComSubAdminUserId + ", " + WorkflowFixture.wfSaleAdminUserId;
                var exceptExecuteUserNames = WorkflowFixture.wfComAdminUserName + ", " + WorkflowFixture.wfComSubAdminUserName + ", " + WorkflowFixture.wfSaleAdminUserName;
                wfStartForm.AllUserIds = exceptExecuteUserIds;
                wfStartForm.AllUserNames = exceptExecuteUserNames;
                wfStartForm.UnProcessUserIds = exceptExecuteUserIds;
                wfStartForm.UnProcessUserNames = exceptExecuteUserNames;

                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                WorkflowFixture.wfInstance_2 = wfService.StartWorkflowAsync(wfStartForm, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName).Result;
                Assert.NotNull(WorkflowFixture.wfInstance_2);
                _logger.LogInformation("实例【wfInstance_2】--2. 生成流程实例instant_2：" + WorkflowFixture.wfInstance_2);
            }

            // 3. 获取流程实例：wfInstance_2
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 启动后的流程图：Start:node1->Task:node3(current)->End:node9 
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();

                #region 根据任务Code获取开始任务（GetStartTaskByProcessCodeAsync）
                var task = await wfService.GetStartTaskByProcessCodeAsync(WorkflowFixture.wfInstance_2);
                var message = wfService.PrintProcessLinkedTask(task);
                _logger.LogInformation("实例【wfInstance_2】--3. 流程实例：wfInstance_2，开始任务：" + message);
                Assert.NotNull(task);
                Assert.Equal(WorkflowNodeType.Start, task.NodeType);
                Assert.Equal(WorkflowTaskStatus.Finished, task.TaskStatus);
                var nextLevel1Task = task.NextNode;
                Assert.NotNull(nextLevel1Task);
                Assert.Equal(WorkflowNodeType.Task, nextLevel1Task.NodeType);
                Assert.Equal(WorkflowFixture.node3_task.Name, nextLevel1Task.Name);
                Assert.Equal(WorkflowTaskStatus.Process, nextLevel1Task.TaskStatus);
                Assert.Equal(wfStartForm.AllUserNames, nextLevel1Task.AllUserNames);
                var nextLevel2Task = task.NextNode.NextNode;
                Assert.NotNull(nextLevel2Task);
                Assert.Equal(WorkflowNodeType.End, nextLevel2Task.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, nextLevel2Task.TaskStatus);
                #endregion

                #region 根据任务Code获取当前执行的任务【启动任务后，当前任务为：node3】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_2);
                Assert.NotNull(currentTask);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                Assert.Equal(wfStartForm.AllUserNames, currentTask.AllUserNames);
                Assert.NotNull(currentTask.NextNode);
                Assert.Equal(WorkflowNodeType.End, currentTask.NextNode.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 4. 流程流转前，获取下一任务节点【node3-task】的审批设置下的审批数据
            WorkflowExecuteData wfSubmitForm;
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                //获取下一任务节点【node3-task】的审批人设置下的审批数据
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                wfSubmitForm = await wfService.GetSubmitWorkflowExecutorInfoAsync(WorkflowFixture.wfInstance_2, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName);
                Assert.NotNull(wfSubmitForm);
                Assert.Equal(WorkflowNodeType.Task, wfSubmitForm.TaskType);
                Assert.Equal(WorkflowFixture.node3_task.Name, wfSubmitForm.TaskName);
                Assert.Equal(wfStartForm.AllUserIds, wfSubmitForm.AllUserIds);

                //node3-task--org: 企业,销售部; role: 总经理,销售经理; user: 肖经理
                _logger.LogInformation(string.Format("实例【wfInstance_2】--4. 获取流转前的下一{0}节点【{1}】的审批人设置下的审批数据：同意: 【{2}】不同意: 【{3}】未处理: 【{4}】", wfSubmitForm.TaskType, wfSubmitForm.TaskName, wfSubmitForm.AgreeUserNames, wfSubmitForm.DisagreeUserNames, wfSubmitForm.UnProcessUserNames));
            }

            // 5. 第一次处理（肖经理-Submit：不同意-退回）流程实例（wfInstance_2）
            //流转后的流程图：Start:node1->Task:node3(current)->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm.ExecuteUserId = WorkflowFixture.wfSaleAdminUserId;
                wfSubmitForm.ExecuteUserName = WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm.ExecuteStatus = WorkflowExecuteStatus.Return;
                wfSubmitForm.ExecuteRemark = "用户：【" + wfSubmitForm.ExecuteUserName + "】执行：不同意-退回";

                _logger.LogInformation("实例【wfInstance_2】--5. 流程流转：第一次处理（肖经理-Submit：不同意-退回）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_2, wfSubmitForm);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【退回任务后，当前任务为node3-task】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_2);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_2】--5. 流程流转后，获取流程可执行任务流程图" + message);
                Assert.NotNull(currentTask);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowFixture.node3_task.Name, currentTask.Name);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                Assert.Equal(wfStartForm.UnProcessUserIds, currentTask.AllUserIds);
                Assert.Equal(WorkflowFixture.wfSaleAdminUserId, currentTask.DisagreeUserIds);
                Assert.Equal(WorkflowFixture.wfComAdminUserId + ", " + WorkflowFixture.wfComSubAdminUserId, currentTask.UnProcessUserIds);
                Assert.NotNull(currentTask.NextNode);
                Assert.Equal(WorkflowNodeType.End, currentTask.NextNode.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 6. 第二次处理（齐副经理-Submit：同意）流程实例（wfInstance_2）
            //流转后的流程图：Start:node1->Task:node3(current)->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm.ExecuteUserId = WorkflowFixture.wfComSubAdminUserId;
                wfSubmitForm.ExecuteUserName = WorkflowFixture.wfComSubAdminUserName;
                wfSubmitForm.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm.ExecuteStatus = WorkflowExecuteStatus.Approve;
                wfSubmitForm.ExecuteRemark = "用户：【" + wfSubmitForm.ExecuteUserName + "】执行：同意提交";

                _logger.LogInformation("实例【wfInstance_2】--6. 流程流转：第二次处理（齐副经理-Submit：同意）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_2, wfSubmitForm);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【流程流转后，当前任务为：node3-task】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_2);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_2】--6. 流程流转后，获取流程可执行任务流程图：" + message);
                Assert.NotNull(currentTask);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowFixture.node3_task.Name, currentTask.Name);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                Assert.Equal(wfStartForm.UnProcessUserIds, currentTask.AllUserIds);
                Assert.Equal(WorkflowFixture.wfComSubAdminUserId, currentTask.AgreeUserIds);
                Assert.Equal(WorkflowFixture.wfComAdminUserId, currentTask.UnProcessUserIds);
                Assert.NotNull(currentTask.NextNode);
                Assert.Equal(WorkflowNodeType.End, currentTask.NextNode.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 7. 第三次处理（齐总经理-Submit：同意）流程实例（wfInstance_2）
            //流转后的流程图：Start:node1->Task:node3->End:node9(current)
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm.ExecuteUserId = WorkflowFixture.wfComAdminUserId;
                wfSubmitForm.ExecuteUserName = WorkflowFixture.wfComAdminUserName;
                wfSubmitForm.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm.ExecuteStatus = WorkflowExecuteStatus.Approve;
                wfSubmitForm.ExecuteRemark = "用户：【" + wfSubmitForm.ExecuteUserName + "】执行：同意提交";

                _logger.LogInformation("实例【wfInstance_2】--7. 流程流转：第三次处理（齐总经理-Submit：同意）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_2, wfSubmitForm);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【流转任务后流程已结束，当前任务为空】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_2);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_2】--7. 流程流转后，获取流程可执行任务流程图：" + message);
                Assert.Null(currentTask);
                #endregion
            }
        }

        /// <summary>
        /// 串联审批测试：wfInstance_3 </br>
        ///     流程定义-3：生成1个多人审批流程实例，启动后为一个多人审批（权重50%通过）任务节点（Task:node3），而后为一个单人审批任务节点（Task:node2），流程进行四次流转处理
        ///     执行人设置：
        ///         Task:node2【CreatorManager：发起人的主管--销售经理：齐副经理, 肖经理】
        ///         Task:node3【Executor：组织/角色/用户--组织:【企业, 销售部】角色:【总经理, 销售经理】用户:【肖经理】】
        ///     启动后的流程图：Start:node1->Task:node3(true)->Task:node2->End:node9
        ///     第一次流转(肖经理-不同意)后的流程图：Start:node1->Task:node3(true)->Task:node2->End:node9
        ///     第二次流转(齐副经理-同意)后的流程图：Start:node1->Task:node3(true)->Task:node2->End:node9
        ///     第三次流转(齐总经理-同意)后的流程图：Start:node1->Task:node3->Task:node2(true)->End:node9
        ///     第四次流转(肖经理-同意)后的流程图：Start:node1->Task:node3->Task:node2->End:node9
        /// </summary>
        [Xunit.Fact]
        public async Task Test_WfProcessService_Instance_3()
        {
            // 1. 启动流程前，获取下一任务节点【node3-task】的审批人设置下的审批数据
            WorkflowStartExecuteData wfStartForm;
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                //获取下一任务节点【node3-task】的审批人设置下的审批数据
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                wfStartForm = await wfService.GetStartWorkflowExecutorInfoAsync(WorkflowFixture.WorkflowDef_3.Code, WorkflowFixture.wfProcessForm, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName);
                Assert.NotNull(wfStartForm);
                Assert.Equal(WorkflowNodeType.Task, wfStartForm.TaskType);
                // 执行人设置：【组织/角色/用户--组织: 【企业, 销售部】角色: 【总经理, 销售经理】用户: 【肖经理】】
                // 预期的执行人【齐总经理,小齐,齐副经理,肖经理,小肖】  
                // Assert.Equal(WorkflowFixture.wfComSubAdminUserId + ", " + WorkflowFixture.wfSaleAdminUserId, wfStartForm.AllUserIds);

                _logger.LogInformation(string.Format("实例【wfInstance_3】--1. 预期的执行人：【齐总经理,小齐,齐副经理,肖经理,小肖】 "));
                //Task:node5--org: IT部; role: admin; user: admin、tester
                _logger.LogInformation(string.Format("实例【wfInstance_3】--1. 获取启动后下一{0}节点【{1}】的审批人设置下的审批数据：组织:【{2}】角色:【{3}】用户:【{4}】", wfStartForm.TaskType, wfStartForm.TaskName, wfStartForm.AgreeUserNames, wfStartForm.DisagreeUserNames, wfStartForm.UnProcessUserNames));
            }

            // 2. 创建流程实例：wfInstance_3
            // 启动后的流程图：Start:node1->Task:node3(current)->Task:node2->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 设置下一节点的审批人：总经理：齐总经理、销售经理：齐副经理, 肖经理
                var exceptExecuteUserIds = WorkflowFixture.wfComAdminUserId + ", " + WorkflowFixture.wfComSubAdminUserId + ", " + WorkflowFixture.wfSaleAdminUserId;
                var exceptExecuteUserNames = WorkflowFixture.wfComAdminUserName + ", " + WorkflowFixture.wfComSubAdminUserName + ", " + WorkflowFixture.wfSaleAdminUserName;
                wfStartForm.AllUserIds = exceptExecuteUserIds;
                wfStartForm.AllUserNames = exceptExecuteUserNames;
                wfStartForm.UnProcessUserIds = exceptExecuteUserIds;
                wfStartForm.UnProcessUserNames = exceptExecuteUserNames;

                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                WorkflowFixture.wfInstance_3 = wfService.StartWorkflowAsync(wfStartForm, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName).Result;
                Assert.NotNull(WorkflowFixture.wfInstance_3);
                _logger.LogInformation("实例【wfInstance_3】--2. 生成流程实例instant_2：" + WorkflowFixture.wfInstance_3);
            }

            // 3. 获取流程实例：wfInstance_3
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 启动后的流程图：Start:node1->Task:node3(current)->End:node9 
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();

                #region 根据任务Code获取开始任务（GetStartTaskByProcessCodeAsync）
                var task = await wfService.GetStartTaskByProcessCodeAsync(WorkflowFixture.wfInstance_3);
                var message = wfService.PrintProcessLinkedTask(task);
                _logger.LogInformation("实例【wfInstance_3】--3. 流程实例：wfInstance_3，开始任务：" + message);
                Assert.NotNull(task);
                Assert.Equal(WorkflowNodeType.Start, task.NodeType);
                Assert.Equal(WorkflowTaskStatus.Finished, task.TaskStatus);
                var nextLevel1Task = task.NextNode;
                Assert.NotNull(nextLevel1Task);
                Assert.Equal(WorkflowNodeType.Task, nextLevel1Task.NodeType);
                Assert.Equal(WorkflowFixture.node3_task.Name, nextLevel1Task.Name);
                Assert.Equal(WorkflowTaskStatus.Process, nextLevel1Task.TaskStatus);
                Assert.Equal(wfStartForm.AllUserNames, nextLevel1Task.AllUserNames);
                var nextLevel2Task = task.NextNode.NextNode;
                Assert.NotNull(nextLevel2Task);
                Assert.Equal(WorkflowNodeType.Task, nextLevel2Task.NodeType);
                Assert.Equal(WorkflowFixture.node2_task.Name, nextLevel2Task.Name);
                Assert.Equal(WorkflowTaskStatus.UnProcess, nextLevel2Task.TaskStatus);
                #endregion

                #region 根据任务Code获取当前执行的任务【启动任务后，当前任务为：node3】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_3);
                Assert.NotNull(currentTask);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                Assert.Equal(wfStartForm.AllUserNames, currentTask.AllUserNames);
                Assert.NotNull(currentTask.NextNode);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NextNode.NodeType);
                Assert.Equal(WorkflowFixture.node2_task.Name, nextLevel2Task.Name);
                Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 4. 流程流转前，获取下一任务节点【node3-task】的审批设置下的审批数据
            WorkflowExecuteData wfSubmitForm_1;
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                //获取下一任务节点【node3-task】的审批人设置下的审批数据
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                wfSubmitForm_1 = await wfService.GetSubmitWorkflowExecutorInfoAsync(WorkflowFixture.wfInstance_3, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName);
                Assert.NotNull(wfSubmitForm_1);
                Assert.Equal(WorkflowNodeType.Task, wfSubmitForm_1.TaskType);
                Assert.Equal(WorkflowFixture.node3_task.Name, wfSubmitForm_1.TaskName);
                Assert.Equal(wfStartForm.AllUserNames, wfSubmitForm_1.AllUserNames);

                //node3-task--org: 企业,销售部; role: 总经理,销售经理; user: 肖经理
                _logger.LogInformation(string.Format("实例【wfInstance_3】--4. 获取流转前的下一{0}节点【{1}】的审批人设置下的审批数据：同意: 【{2}】不同意: 【{3}】未处理: 【{4}】", wfSubmitForm_1.TaskType, wfSubmitForm_1.TaskName, wfSubmitForm_1.AgreeUserNames, wfSubmitForm_1.DisagreeUserNames, wfSubmitForm_1.UnProcessUserNames));
            }

            // 5. 第一次处理（肖经理-Submit：不同意-退回）流程实例（wfInstance_3）
            // 流转后的流程图：Start:node1->Task:node3(current)->Task:node2->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm_1.ExecuteUserId = WorkflowFixture.wfSaleAdminUserId;
                wfSubmitForm_1.ExecuteUserName = WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm_1.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm_1.ExecuteStatus = WorkflowExecuteStatus.Return;
                wfSubmitForm_1.ExecuteRemark = "用户：【" + wfSubmitForm_1.ExecuteUserName + "】执行：不同意-退回";

                _logger.LogInformation("实例【wfInstance_3】--5. 流程流转：第一次处理（肖经理-Submit：不同意-退回）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_3, wfSubmitForm_1);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【退回任务后，当前任务为node3-task】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_3);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_3】--5. 流程流转后，获取流程可执行任务流程图" + message);
                Assert.NotNull(currentTask);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowFixture.node3_task.Name, currentTask.Name);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                Assert.Equal(wfStartForm.AllUserIds, currentTask.AllUserIds);
                Assert.Equal(WorkflowFixture.wfSaleAdminUserId, currentTask.DisagreeUserIds);
                Assert.Equal(WorkflowFixture.wfComAdminUserId + ", " + WorkflowFixture.wfComSubAdminUserId, currentTask.UnProcessUserIds);
                Assert.NotNull(currentTask.NextNode);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NextNode.NodeType);
                Assert.Equal(WorkflowFixture.node2_task.Name, currentTask.NextNode.Name);
                Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 6. 第二次处理（齐副经理-Submit：同意）流程实例（wfInstance_3）
            // 流转后的流程图：Start:node1->Task:node3(current)->Task:node2->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm_1.ExecuteUserId = WorkflowFixture.wfComSubAdminUserId;
                wfSubmitForm_1.ExecuteUserName = WorkflowFixture.wfComSubAdminUserName;
                wfSubmitForm_1.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm_1.ExecuteStatus = WorkflowExecuteStatus.Approve;
                wfSubmitForm_1.ExecuteRemark = "用户：【" + wfSubmitForm_1.ExecuteUserName + "】执行：同意提交";

                _logger.LogInformation("实例【wfInstance_3】--6. 流程流转：第二次处理（齐副经理-Submit：同意）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_3, wfSubmitForm_1);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【流程流转后，当前任务为：node3-task】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_3);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_3】--6. 流程流转后，获取流程可执行任务流程图：" + message);
                Assert.NotNull(currentTask);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowFixture.node3_task.Name, currentTask.Name);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                Assert.Equal(wfStartForm.AllUserIds, currentTask.AllUserIds);
                Assert.Equal(WorkflowFixture.wfComSubAdminUserId, currentTask.AgreeUserIds);
                Assert.Equal(WorkflowFixture.wfComAdminUserId, currentTask.UnProcessUserIds);
                Assert.NotNull(currentTask.NextNode);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NextNode.NodeType);
                Assert.Equal(WorkflowFixture.node2_task.Name, currentTask.NextNode.Name);
                Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 7. 第三次处理（齐总经理-Submit：同意）流程实例（wfInstance_3）
            // 流转后的流程图：Start:node1->Task:node3->Task:node2(current)->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm_1.ExecuteUserId = WorkflowFixture.wfComAdminUserId;
                wfSubmitForm_1.ExecuteUserName = WorkflowFixture.wfComAdminUserName;
                wfSubmitForm_1.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm_1.ExecuteStatus = WorkflowExecuteStatus.Approve;
                wfSubmitForm_1.ExecuteRemark = "用户：【" + wfSubmitForm_1.ExecuteUserName + "】执行：同意提交";

                _logger.LogInformation("实例【wfInstance_3】--7. 流程流转：第二次处理（齐总经理-Submit：同意）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_3, wfSubmitForm_1);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【流程流转后，当前任务为：node2-task】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_3);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_3】--7. 流程流转后，获取流程可执行任务流程图：" + message);
                Assert.NotNull(currentTask);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowFixture.node2_task.Name, currentTask.Name);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                Assert.NotNull(currentTask.NextNode);
                Assert.Equal(WorkflowNodeType.End, currentTask.NextNode.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 8. 第四次处理前，获取下一任务节点【node2-task】的审批设置下的审批数据
            WorkflowExecuteData wfSubmitForm_2;
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                //获取下一任务节点【node2-task】的审批人设置下的审批数据
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                wfSubmitForm_2 = await wfService.GetSubmitWorkflowExecutorInfoAsync(WorkflowFixture.wfInstance_3, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName);
                Assert.NotNull(wfSubmitForm_2);
                Assert.Equal(WorkflowNodeType.Task, wfSubmitForm_2.TaskType);
                Assert.Equal(WorkflowFixture.node2_task.Name, wfSubmitForm_2.TaskName);
                // 默认设置的执行人：【CreatorManager：发起人(小肖)的主管--销售经理：齐副经理, 肖经理】
                // Assert.Equal(wfStartForm.AllUserNames, wfSubmitForm_2.AllUserNames);

                //node2-task--org: null; role: null; user: 齐总经理,肖经理
                _logger.LogInformation(string.Format("实例【wfInstance_3】--8. 获取流转前的下一{0}节点【{1}】的审批人设置下的审批数据：同意: 【{2}】不同意: 【{3}】未处理: 【{4}】", wfSubmitForm_2.TaskType, wfSubmitForm_2.TaskName, wfSubmitForm_2.AgreeUserNames, wfSubmitForm_2.DisagreeUserNames, wfSubmitForm_2.UnProcessUserNames));
            }

            // 9. 第四次处理（肖经理-Submit：同意）流程实例（wfInstance_3）
            //流转后的流程图：Start:node1->Task:node3->Task:node2->End:node9(current)
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm_2.ExecuteUserId = WorkflowFixture.wfSaleAdminUserId;
                wfSubmitForm_2.ExecuteUserName = WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm_2.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm_2.ExecuteStatus = WorkflowExecuteStatus.Approve;
                wfSubmitForm_2.ExecuteRemark = "用户：【" + wfSubmitForm_2.ExecuteUserName + "】执行：同意提交";

                _logger.LogInformation("实例【wfInstance_3】--9. 流程流转：第四次处理（肖经理-Submit：同意）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_3, wfSubmitForm_2);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【流转任务后流程已结束，当前任务为空】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_3);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【WfInstance_3】--6. 流程流转后，获取流程可执行任务流程图：" + message);
                Assert.Null(currentTask);
                #endregion
            }
        }

        /// <summary>
        /// 条件前置审批测试：wfInstance_4 </br>
        ///     流程定义-4：生成1个条件前置流程实例，启动后为一个条件节点（Condition:node5【field3>=20】），而后进行流转处理（Submit：同意）
        ///     设置执行人：Task:node2【CreatorManager：发起人的主管--销售经理：齐副经理, 肖经理】
        ///     设置表单值：【设置表单字段【field3】的实际值为：23】
        ///     条件处理过程：node5-执行条件【field3>=20】
        ///                         条件为true，下一流程：Task:node2（满足）
        ///                         条件为false，流程结束：Task:node9
        ///     启动后的流程图：Start:node1->Condition:node5->Task:node2(true)->End:node9 
        ///     流转后的流程图：Start:node1->Condition:node5->Task:node2->End:node9
        /// </summary>
        [Xunit.Fact]
        public async Task Test_WfProcessService_Instance_4()
        {
            // 1. 启动流程前，获取下一任务节点【node2-task】的审批人设置下的审批数据
            //      设置表单字段【field3】的实际值为：23
            WorkflowStartExecuteData wfStartForm;
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 设置表单字段【field3】的实际值为：23，满足执行条件【field3>=20】为：true
                WorkflowFixture.field3_currency.Value = "23";

                //获取下一任务节点【node2-task】的审批人设置下的审批数据
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                wfStartForm = await wfService.GetStartWorkflowExecutorInfoAsync(WorkflowFixture.WorkflowDef_4.Code, WorkflowFixture.wfProcessForm, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName);
                Assert.NotNull(wfStartForm);
                Assert.Equal(WorkflowNodeType.Task, wfStartForm.TaskType);
                Assert.Equal(WorkflowFixture.node2_task.Name, wfStartForm.TaskName);
                // 默认预期的执行人：【CreatorManager：发起人(小肖)的主管--销售经理：齐副经理, 肖经理】
                Assert.Equal(WorkflowFixture.wfComSubAdminUserId + ", " + WorkflowFixture.wfSaleAdminUserId, wfStartForm.AllUserIds);

                _logger.LogInformation(string.Format("实例【wfInstance_4】--1. 预期的执行人：【CreatorManager：发起人(小肖)的主管--销售经理：齐副经理, 肖经理】"));
                _logger.LogInformation(string.Format("实例【wfInstance_4】--1. 获取启动后下一{0}节点【{1}】的审批人设置下的审批数据：同意: 【{2}】不同意: 【{3}】未处理: 【{4}】", wfStartForm.TaskType, wfStartForm.TaskName, wfStartForm.AgreeUserNames, wfStartForm.DisagreeUserNames, wfStartForm.UnProcessUserNames));
            }

            // 2. 创建流程实例：wfInstance_4：
            //条件处理过程：node5-执行条件【field3>=20】
            //                    条件为-true，下一流程：Condition:node2 （满足）
            //                    条件为-false，流程结束：End:node9
            //启动后的流程图：Start:node1->Condition:node5->Task:node2(current)->End:node9 
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 设置下一节点的审批人：齐总经理、肖经理
                var exceptExecuteUserIds = WorkflowFixture.wfComAdminUserId + ", " + WorkflowFixture.wfSaleAdminUserId;
                var exceptExecuteUserNames = WorkflowFixture.wfComAdminUserName + ", " + WorkflowFixture.wfSaleAdminUserName;
                wfStartForm.AllUserIds = exceptExecuteUserIds;
                wfStartForm.AllUserNames = exceptExecuteUserNames;
                wfStartForm.UnProcessUserIds = exceptExecuteUserIds;
                wfStartForm.UnProcessUserNames = exceptExecuteUserNames;
                // 设置表单字段【field3】的实际值为：23，满足执行条件【field3>=20】为：true
                wfStartForm.FormData.ForEach(f =>
                {
                    if (f.Text.Equals(WorkflowFixture.field3_currency.Text))
                    {
                        f.Value = "23";
                    }
                });

                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                WorkflowFixture.wfInstance_4 = wfService.StartWorkflowAsync(wfStartForm, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName).Result;
                Assert.NotNull(WorkflowFixture.wfInstance_4);
                _logger.LogInformation("实例【wfInstance_4】--2. 生成流程实例instant_1：" + WorkflowFixture.wfInstance_4);
            }

            // 3. 获取流程实例：wfInstance_4
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 启动后的流程图：Start:node1->Condition:node5->Task:node2(current)->End:node9 
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();

                #region 根据任务Code获取开始任务（GetStartTaskByProcessCodeAsync）
                var task = await wfService.GetStartTaskByProcessCodeAsync(WorkflowFixture.wfInstance_4);
                var message = wfService.PrintProcessLinkedTask(task);
                _logger.LogInformation("实例【wfInstance_4】--3. 流程实例：wfInstance_4，开始任务：" + message);
                Assert.NotNull(task);
                Assert.Equal(WorkflowNodeType.Start, task.NodeType);
                Assert.Equal(WorkflowTaskStatus.Finished, task.TaskStatus);
                var nextLevel0Task = task.NextNode;
                Assert.NotNull(nextLevel0Task);
                Assert.Equal(WorkflowNodeType.Condition, nextLevel0Task.NodeType);
                Assert.Equal(WorkflowTaskStatus.Finished, nextLevel0Task.TaskStatus);
                var nextLevel1Task = nextLevel0Task.NextNode;
                Assert.NotNull(nextLevel1Task);
                Assert.Equal(WorkflowNodeType.Task, nextLevel1Task.NodeType);
                Assert.Equal(WorkflowTaskStatus.Process, nextLevel1Task.TaskStatus);
                Assert.Equal(wfStartForm.AllUserNames, nextLevel1Task.AllUserNames);
                var nextLevel2Task = nextLevel1Task.NextNode;
                Assert.NotNull(nextLevel2Task);
                Assert.Equal(WorkflowNodeType.End, nextLevel2Task.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, nextLevel2Task.TaskStatus);
                #endregion

                #region 根据任务Code获取当前执行的任务【启动任务后，当前任务为：node2-task】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_4);
                Assert.NotNull(currentTask);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowFixture.node2_task.Name, currentTask.Name);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                Assert.Equal(wfStartForm.AllUserNames, currentTask.AllUserNames);
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
                wfSubmitForm = await wfService.GetSubmitWorkflowExecutorInfoAsync(WorkflowFixture.wfInstance_4, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName);
                Assert.NotNull(wfSubmitForm);
                Assert.Equal(WorkflowNodeType.Task, wfSubmitForm.TaskType);
                Assert.Equal(WorkflowFixture.node2_task.Name, wfSubmitForm.TaskName);
                Assert.Equal(wfStartForm.AllUserNames, wfSubmitForm.AllUserNames);

                //node2-task--org: null; role: null; user: 齐总经理,肖经理
                _logger.LogInformation(string.Format("实例【wfInstance_4】--4. 获取流转前的下一{0}节点【{1}】的审批人设置下的审批数据：同意: 【{2}】不同意: 【{3}】未处理: 【{4}】", wfSubmitForm.TaskType, wfSubmitForm.TaskName, wfSubmitForm.AgreeUserNames, wfSubmitForm.DisagreeUserNames, wfSubmitForm.UnProcessUserNames));
            }

            // 5. 第一次处理（肖经理-Submit：不同意-退回）流程实例（wfInstance_4）
            //流转后的流程图：(current)->Start:node1->Condition:node5->Task:node2->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm.ExecuteUserId = WorkflowFixture.wfSaleAdminUserId;
                wfSubmitForm.ExecuteUserName = WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm.ExecuteStatus = WorkflowExecuteStatus.Return;
                wfSubmitForm.ExecuteRemark = "用户：【" + wfSubmitForm.ExecuteUserName + "】执行：不同意-退回";

                _logger.LogInformation("实例【wfInstance_4】--5. 流程流转：第一次处理（肖经理-Submit：不同意-退回）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_4, wfSubmitForm);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【退回任务后，将任务退回至发起人，当前任务为空】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_4);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_4】--5. 流程流转后，获取流程可执行任务流程图" + message);
                Assert.Null(currentTask);
                //Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                //Assert.Equal(WorkflowFixture.node2_task.Name, currentTask.Name);
                //Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                //Assert.Equal(wfSubmitForm.AllUserNames, currentTask.AllUserNames);
                //Assert.NotNull(currentTask.NextNode);
                //Assert.Equal(WorkflowNodeType.End, currentTask.NextNode.NodeType);
                //Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 6. 第二次处理（肖经理-Submit：同意）流程实例（wfInstance_4）
            //流转后的流程图：Start:node1->Condition:node5->Task:node2->End:node9(current)
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm.ExecuteUserId = WorkflowFixture.wfSaleAdminUserId;
                wfSubmitForm.ExecuteUserName = WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm.ExecuteStatus = WorkflowExecuteStatus.Approve;
                wfSubmitForm.ExecuteRemark = "用户：【" + wfSubmitForm.ExecuteUserName + "】执行：同意提交";

                _logger.LogInformation("实例【wfInstance_4】--6. 流程流转：第二次处理（Submit：同意）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_4, wfSubmitForm);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【流转任务后流程已结束，当前任务为空】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_4);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_4】--6. 流程流转后，获取流程可执行任务流程图：" + message);
                Assert.Null(currentTask);
                #endregion
            }
        }

        /// <summary>
        /// 条件后置审批测试：wfInstance_5 </br>
        ///     流程定义-5：生成1个条件后置流程实例，启动后流转处理（Submit：同意），而后进行一个条件节点（Condition:node5【field3>=20】）
        ///     设置执行人：Task:node2【CreatorManager：发起人的主管--销售经理：齐副经理, 肖经理】
        ///     设置表单值：【设置表单字段【field3】的实际值为：23】
        ///     条件处理过程：node5-执行条件【field3>=20】
        ///                         条件为true，下一流程：End:node9（满足）
        ///                         条件为false，流程结束：Start:node1
        ///     启动后的流程图：Start:node1->Task:node2(true)->Condition:node5->End:node9 
        ///     流转后的流程图：Start:node1->Task:node2->Condition:node5->End:node9
        /// </summary>
        [Xunit.Fact]
        public async Task Test_WfProcessService_Instance_5()
        {
            // 1. 启动流程前，获取下一任务节点【node2-task】的审批人设置下的审批数据
            //      设置表单字段【field3】的实际值为：23
            WorkflowStartExecuteData wfStartForm;
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 设置表单字段【field3】的实际值为：23，满足执行条件【field3>=20】为：true
                WorkflowFixture.field3_currency.Value = "23";

                //获取下一任务节点【node2-task】的审批人设置下的审批数据
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                wfStartForm = await wfService.GetStartWorkflowExecutorInfoAsync(WorkflowFixture.WorkflowDef_5.Code, WorkflowFixture.wfProcessForm, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName);
                Assert.NotNull(wfStartForm);
                Assert.Equal(WorkflowNodeType.Task, wfStartForm.TaskType);
                Assert.Equal(WorkflowFixture.node2_task.Name, wfStartForm.TaskName);
                // 默认预期的执行人：【CreatorManager：发起人(小肖)的主管--销售经理：齐副经理, 肖经理】
                Assert.Equal(WorkflowFixture.wfComSubAdminUserId + ", " + WorkflowFixture.wfSaleAdminUserId, wfStartForm.AllUserIds);

                _logger.LogInformation(string.Format("实例【wfInstance_5】--1. 预期的执行人：【CreatorManager：发起人(小肖)的主管--销售经理：齐副经理, 肖经理】"));
                _logger.LogInformation(string.Format("实例【wfInstance_5】--1. 获取启动后下一{0}节点【{1}】的审批人设置下的审批数据：同意: 【{2}】不同意: 【{3}】未处理: 【{4}】", wfStartForm.TaskType, wfStartForm.TaskName, wfStartForm.AgreeUserNames, wfStartForm.DisagreeUserNames, wfStartForm.UnProcessUserNames));
            }

            // 2. 创建流程实例：wfInstance_5：
            //条件处理过程：node5-执行条件【field3>=20】
            //                    条件为-true，下一流程：Condition:node2 （满足）
            //                    条件为-false，流程结束：End:node9
            //启动后的流程图：Start:node1->Task:node2(current)->Condition:node5->End:node9 
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 设置下一节点的审批人：齐总经理、肖经理
                var exceptExecuteUserIds = WorkflowFixture.wfComAdminUserId + ", " + WorkflowFixture.wfSaleAdminUserId;
                var exceptExecuteUserNames = WorkflowFixture.wfComAdminUserName + ", " + WorkflowFixture.wfSaleAdminUserName;
                wfStartForm.AllUserIds = exceptExecuteUserIds;
                wfStartForm.AllUserNames = exceptExecuteUserNames;
                wfStartForm.UnProcessUserIds = exceptExecuteUserIds;
                wfStartForm.UnProcessUserNames = exceptExecuteUserNames;

                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                WorkflowFixture.wfInstance_5 = wfService.StartWorkflowAsync(wfStartForm, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName).Result;
                Assert.NotNull(WorkflowFixture.wfInstance_5);
                _logger.LogInformation("实例【wfInstance_5】--2. 生成流程实例instant_1：" + WorkflowFixture.wfInstance_5);
            }

            // 3. 获取流程实例：wfInstance_5
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 启动后的流程图：Start:node1->Task:node2(current)->Condition:node5->End:node9 
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();

                #region 根据任务Code获取开始任务（GetStartTaskByProcessCodeAsync）
                var task = await wfService.GetStartTaskByProcessCodeAsync(WorkflowFixture.wfInstance_5);
                var message = wfService.PrintProcessLinkedTask(task);
                _logger.LogInformation("实例【wfInstance_5】--3. 流程实例：wfInstance_5，开始任务：" + message);
                Assert.NotNull(task);
                Assert.Equal(WorkflowNodeType.Start, task.NodeType);
                Assert.Equal(WorkflowTaskStatus.Finished, task.TaskStatus);
                var nextLevel0Task = task.NextNode;
                Assert.NotNull(nextLevel0Task);
                Assert.Equal(WorkflowNodeType.Task, nextLevel0Task.NodeType);
                Assert.Equal(WorkflowFixture.node2_task.Name, nextLevel0Task.Name);
                Assert.Equal(WorkflowTaskStatus.Process, nextLevel0Task.TaskStatus);
                Assert.Equal(wfStartForm.AllUserNames, nextLevel0Task.AllUserNames);
                var nextLevel1Task = nextLevel0Task.NextNode;
                Assert.NotNull(nextLevel1Task);
                Assert.Equal(WorkflowNodeType.Condition, nextLevel1Task.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, nextLevel1Task.TaskStatus);
                var nextLevel2Task = nextLevel1Task.NextNode;
                Assert.NotNull(nextLevel2Task);
                Assert.Equal(WorkflowNodeType.End, nextLevel2Task.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, nextLevel2Task.TaskStatus);
                #endregion

                #region 根据任务Code获取当前执行的任务【启动任务后，当前任务为：node2-task】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_5);
                Assert.NotNull(currentTask);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowFixture.node2_task.Name, currentTask.Name);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                Assert.Equal(wfStartForm.AllUserNames, currentTask.AllUserNames);
                Assert.NotNull(currentTask.NextNode);
                Assert.Equal(WorkflowNodeType.Condition, currentTask.NextNode.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 4. 流程流转前，获取下一任务节点【node2-task】的审批设置下的审批数据
            WorkflowExecuteData wfSubmitForm;
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                //获取下一任务节点【node2-task】的审批人设置下的审批数据
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                wfSubmitForm = await wfService.GetSubmitWorkflowExecutorInfoAsync(WorkflowFixture.wfInstance_5, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName);
                Assert.NotNull(wfSubmitForm);
                Assert.Equal(WorkflowNodeType.Task, wfSubmitForm.TaskType);
                Assert.Equal(WorkflowFixture.node2_task.Name, wfSubmitForm.TaskName);
                Assert.Equal(wfStartForm.AllUserNames, wfSubmitForm.AllUserNames);

                //node2-task--org: null; role: null; user: 齐总经理,肖经理
                _logger.LogInformation(string.Format("实例【wfInstance_5】--4. 获取流转前的下一{0}节点【{1}】的审批人设置下的审批数据：同意: 【{2}】不同意: 【{3}】未处理: 【{4}】", wfSubmitForm.TaskType, wfSubmitForm.TaskName, wfSubmitForm.AgreeUserNames, wfSubmitForm.DisagreeUserNames, wfSubmitForm.UnProcessUserNames));
            }

            // 5. 第一次处理（肖经理-Submit：不同意-退回）流程实例（wfInstance_5）
            //流转后的流程图：(current)->Start:node1->Condition:node5->Task:node2->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm.ExecuteUserId = WorkflowFixture.wfSaleAdminUserId;
                wfSubmitForm.ExecuteUserName = WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm.ExecuteStatus = WorkflowExecuteStatus.Return;
                wfSubmitForm.ExecuteRemark = "用户：【" + wfSubmitForm.ExecuteUserName + "】执行：不同意-退回";

                _logger.LogInformation("实例【wfInstance_5】--5. 流程流转：第一次处理（肖经理-Submit：不同意-退回）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_5, wfSubmitForm);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【退回任务后，将任务退回至发起人，当前任务为空】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_5);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_5】--5. 流程流转后，获取流程可执行任务流程图" + message);
                Assert.Null(currentTask);
                //Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                //Assert.Equal(WorkflowFixture.node2_task.Name, currentTask.Name);
                //Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                //Assert.Equal(wfSubmitForm.AllUserNames, currentTask.AllUserNames);
                //Assert.NotNull(currentTask.NextNode);
                //Assert.Equal(WorkflowNodeType.Condition, currentTask.NextNode.NodeType);
                //Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 6. 第二次处理（肖经理-Submit：同意）流程实例（wfInstance_5）
            //流转后的流程图：Start:node1->Condition:node5->Task:node2->End:node9(current)
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm.ExecuteUserId = WorkflowFixture.wfSaleAdminUserId;
                wfSubmitForm.ExecuteUserName = WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm.ExecuteStatus = WorkflowExecuteStatus.Approve;
                wfSubmitForm.ExecuteRemark = "用户：【" + wfSubmitForm.ExecuteUserName + "】执行：同意提交";

                _logger.LogInformation("实例【wfInstance_5】--6. 流程流转：第二次处理（Submit：同意）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_5, wfSubmitForm);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【流转任务后流程已结束，当前任务为空】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_5);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_5】--6. 流程流转后，获取流程可执行任务流程图：" + message);
                Assert.Null(currentTask);
                #endregion
            }
        }

        /// <summary>
        /// 条件串联审批测试：wfInstance_6 </br>
        ///     流程定义-6：生成1个条件后置流程实例，启动后为两个条件节点（Condition:node5【field3>=20】、Condition:node6【field3>=40 && field5==true】），而后进行流转处理（Submit：同意）
        ///     执行人设置：
        ///         Task:node2【CreatorManager：发起人的主管--销售经理：齐副经理, 肖经理】
        ///         Task:node3【Executor：组织/角色/用户--组织:【企业, 销售部】角色:【总经理, 销售经理】用户:【肖经理】】
        ///     设置表单值：【设置表单字段【field3】的实际值为：43；【field5】的实际值为：true】
        ///     条件处理过程：node5-执行条件【field3>=20】
        ///                         条件为true，下一流程：Condition:node6（满足）
        ///                         条件为false，流程结束：End:node9
        ///                  node6-执行条件【field3>=40 && field5==true】
        ///                         条件为true，下一流程：Task:node2（满足）
        ///                         条件为false，流程结束：End:node9
        ///     启动后的流程图：Start:node1->Condition:node5->Condition:node6->Task:node2(true)->End:node9 
        ///     流转后的流程图：Start:node1->Condition:node5->Condition:node6->Task:node2->End:node9
        /// </summary>
        [Xunit.Fact]
        public async Task Test_WfProcessService_Instance_6()
        {
            // 1. 启动流程前，获取下一任务节点【node2-task】的审批人设置下的审批数据
            //      设置表单字段【field3】的实际值为：43；【field5】的实际值为：true
            WorkflowStartExecuteData wfStartForm;
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 设置表单字段【field3】的实际值为：43，【field5】的实际值为：true
                // 满足执行条件【field3>=20】【field3>=40 && field5=true】为：true
                WorkflowFixture.field3_currency.Value = "43";
                WorkflowFixture.field5_bool.Value = "true";

                //获取下一任务节点【node2-task】的审批人设置下的审批数据
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                wfStartForm = await wfService.GetStartWorkflowExecutorInfoAsync(WorkflowFixture.WorkflowDef_6.Code, WorkflowFixture.wfProcessForm, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName);
                Assert.NotNull(wfStartForm);
                Assert.Equal(WorkflowNodeType.Task, wfStartForm.TaskType);
                Assert.Equal(WorkflowFixture.node2_task.Name, wfStartForm.TaskName);
                // 默认预期的执行人：【CreatorManager：发起人(小肖)的主管--销售经理：齐副经理, 肖经理】
                Assert.Equal(WorkflowFixture.wfComSubAdminUserId + ", " + WorkflowFixture.wfSaleAdminUserId, wfStartForm.AllUserIds);

                _logger.LogInformation(string.Format("实例【wfInstance_6】--1. 预期的执行人：【CreatorManager：发起人(小肖)的主管--销售经理：齐副经理, 肖经理】"));
                _logger.LogInformation(string.Format("实例【wfInstance_6】--1. 获取启动后下一{0}节点【{1}】的审批人设置下的审批数据：同意: 【{2}】不同意: 【{3}】未处理: 【{4}】", wfStartForm.TaskType, wfStartForm.TaskName, wfStartForm.AgreeUserNames, wfStartForm.DisagreeUserNames, wfStartForm.UnProcessUserNames));
            }

            // 2. 创建流程实例：wfInstance_6：
            //条件处理过程：node5-执行条件【field3>=20】
            //                    条件为-true，下一流程：Condition:node6 （满足）
            //                    条件为-false，流程结束：End:node9
            ///            node6-执行条件【field3>=40 && field5==true】
            ///                   条件为true，下一流程：Task:node2（满足）
            ///                   条件为false，流程结束：End:node9
            //启动后的流程图：Start:node1->Condition:node5->Condition:node6->Task:node2(current)->End:node9 
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 设置下一节点的审批人：齐总经理、肖经理
                var exceptExecuteUserIds = WorkflowFixture.wfComAdminUserId + ", " + WorkflowFixture.wfSaleAdminUserId;
                var exceptExecuteUserNames = WorkflowFixture.wfComAdminUserName + ", " + WorkflowFixture.wfSaleAdminUserName;
                wfStartForm.AllUserIds = exceptExecuteUserIds;
                wfStartForm.AllUserNames = exceptExecuteUserNames;
                wfStartForm.UnProcessUserIds = exceptExecuteUserIds;
                wfStartForm.UnProcessUserNames = exceptExecuteUserNames;

                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                WorkflowFixture.wfInstance_6 = wfService.StartWorkflowAsync(wfStartForm, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName).Result;
                Assert.NotNull(WorkflowFixture.wfInstance_6);
                _logger.LogInformation("实例【wfInstance_6】--2. 生成流程实例instant_1：" + WorkflowFixture.wfInstance_6);
            }

            // 3. 获取流程实例：wfInstance_6
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 启动后的流程图：Start:node1->Task:node2(current)->Condition:node5->End:node9 
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();

                #region 根据任务Code获取开始任务（GetStartTaskByProcessCodeAsync）
                var task = await wfService.GetStartTaskByProcessCodeAsync(WorkflowFixture.wfInstance_6);
                var message = wfService.PrintProcessLinkedTask(task);
                _logger.LogInformation("实例【wfInstance_6】--3. 流程实例：wfInstance_6，开始任务：" + message);
                Assert.NotNull(task);
                Assert.Equal(WorkflowNodeType.Start, task.NodeType);
                Assert.Equal(WorkflowTaskStatus.Finished, task.TaskStatus);
                var nextLevel0Condition = task.NextNode;
                Assert.NotNull(nextLevel0Condition);
                Assert.Equal(WorkflowNodeType.Condition, nextLevel0Condition.NodeType);
                Assert.Equal(WorkflowTaskStatus.Finished, nextLevel0Condition.TaskStatus);
                var nextLevel1Condition = nextLevel0Condition.NextNode;
                Assert.NotNull(nextLevel1Condition);
                Assert.Equal(WorkflowNodeType.Condition, nextLevel1Condition.NodeType);
                Assert.Equal(WorkflowTaskStatus.Finished, nextLevel1Condition.TaskStatus);
                var nextLevel1Task = nextLevel1Condition.NextNode;
                Assert.NotNull(nextLevel1Task);
                Assert.Equal(WorkflowNodeType.Task, nextLevel1Task.NodeType);
                Assert.Equal(WorkflowFixture.node2_task.Name, nextLevel1Task.Name);
                Assert.Equal(WorkflowTaskStatus.Process, nextLevel1Task.TaskStatus);
                Assert.Equal(wfStartForm.AllUserNames, nextLevel1Task.AllUserNames);
                var nextLevel2Task = nextLevel1Task.NextNode;
                Assert.NotNull(nextLevel2Task);
                Assert.Equal(WorkflowNodeType.End, nextLevel2Task.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, nextLevel2Task.TaskStatus);
                #endregion

                #region 根据任务Code获取当前执行的任务【启动任务后，当前任务为：node2-task】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_6);
                Assert.NotNull(currentTask);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowFixture.node2_task.Name, currentTask.Name);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                Assert.Equal(wfStartForm.AllUserNames, currentTask.AllUserNames);
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
                wfSubmitForm = await wfService.GetSubmitWorkflowExecutorInfoAsync(WorkflowFixture.wfInstance_6, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName);
                Assert.NotNull(wfSubmitForm);
                Assert.Equal(WorkflowNodeType.Task, wfSubmitForm.TaskType);
                Assert.Equal(WorkflowFixture.node2_task.Name, wfSubmitForm.TaskName);
                Assert.Equal(wfStartForm.AllUserNames, wfSubmitForm.AllUserNames);

                //node2-task--org: null; role: null; user: 齐总经理,肖经理
                _logger.LogInformation(string.Format("实例【wfInstance_6】--4. 获取流转前的下一{0}节点【{1}】的审批人设置下的审批数据：同意: 【{2}】不同意: 【{3}】未处理: 【{4}】", wfSubmitForm.TaskType, wfSubmitForm.TaskName, wfSubmitForm.AgreeUserNames, wfSubmitForm.DisagreeUserNames, wfSubmitForm.UnProcessUserNames));
            }

            // 5. 第一次处理（肖经理-Submit：不同意-退回）流程实例（wfInstance_6）
            //流转后的流程图：(current)->Start:node1->Condition:node5->Condition:node6->Task:node2->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm.ExecuteUserId = WorkflowFixture.wfSaleAdminUserId;
                wfSubmitForm.ExecuteUserName = WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm.ExecuteStatus = WorkflowExecuteStatus.Return;
                wfSubmitForm.ExecuteRemark = "用户：【" + wfSubmitForm.ExecuteUserName + "】执行：不同意-退回";

                _logger.LogInformation("实例【wfInstance_6】--5. 流程流转：第一次处理（肖经理-Submit：不同意-退回）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_6, wfSubmitForm);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【退回任务后，当前任务为：node2-task】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_6);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_6】--5. 流程流转后，获取流程可执行任务流程图" + message);
                Assert.Null(currentTask);
                //Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                //Assert.Equal(WorkflowFixture.node2_task.Name, currentTask.Name);
                //Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                //Assert.Equal(wfSubmitForm.AllUserNames, currentTask.AllUserNames);
                //Assert.NotNull(currentTask.NextNode);
                //Assert.Equal(WorkflowNodeType.End, currentTask.NextNode.NodeType);
                //Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 6. 第二次处理（肖经理-Submit：同意）流程实例（wfInstance_6）
            //流转后的流程图：Start:node1->Condition:node5->Condition:node6->Task:node2->End:node9(current)
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm.ExecuteUserId = WorkflowFixture.wfSaleAdminUserId;
                wfSubmitForm.ExecuteUserName = WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm.ExecuteStatus = WorkflowExecuteStatus.Approve;
                wfSubmitForm.ExecuteRemark = "用户：【" + wfSubmitForm.ExecuteUserName + "】执行：同意提交";

                _logger.LogInformation("实例【wfInstance_6】--6. 流程流转：第二次处理（Submit：同意）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_6, wfSubmitForm);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【流转任务后流程已结束，当前任务为空】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_6);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_6】--6. 流程流转后，获取流程可执行任务流程图：" + message);
                Assert.Null(currentTask);
                #endregion
            }
        }

        /// <summary>
        /// 复杂条件前置审批测试：wfInstance_7_1 </br>
        ///     流程定义-7：生成1个条件前置的复杂流程实例，启动后由两个条件及任务节点（Condition:node5【field3>=20】、Task:node2）及（Condition:node6【field3>=40 && field5==true】、Task:node3）组成
        ///     执行人设置：
        ///         Task:node2【CreatorManager：发起人的主管--销售经理：齐副经理, 肖经理】
        ///         Task:node3【Executor：组织/角色/用户--组织:【企业, 销售部】角色:【总经理, 销售经理】用户:【肖经理】】
        ///     设置表单值：【设置表单字段【field3】的实际值为：23；【field5】的实际值为：true】
        ///     条件处理过程：node5-执行条件【field3>=20】
        ///                         条件为true，下一流程：Task:node2（满足）
        ///                         条件为false，流程结束：End:node9
        ///                  node6-执行条件【field3>=40 && field5==true】
        ///                         条件为true，下一流程：Task:node3
        ///                         条件为false，流程结束：End:node9（满足）
        ///     启动后的流程图：Start:node1->Condtion:node5->Task:node2(current)->Condtion:node6->Task:node3->End:node9 
        ///     流转后的流程图：Start:node1->Condtion:node5->Task:node2->Condtion:node6->Task:node3->End:node9
        /// </summary>
        [Xunit.Fact]
        public async Task Test_WfProcessService_Instance_7_1()
        {
            // 1. 启动流程前，获取下一任务节点【node2-task】的审批人设置下的审批数据
            //      设置表单字段【field3】的实际值为：23；【field5】的实际值为：true
            WorkflowStartExecuteData wfStartForm;
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 设置表单字段【field3】的实际值为：23，【field5】的实际值为：true
                // 满足执行条件【field3>=20】为：true，【field3>=40 && field5=true】为：false
                WorkflowFixture.field3_currency.Value = "23";
                WorkflowFixture.field5_bool.Value = "true";

                //获取下一任务节点【node2-task】的审批人设置下的审批数据
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                wfStartForm = await wfService.GetStartWorkflowExecutorInfoAsync(WorkflowFixture.WorkflowDef_7.Code, WorkflowFixture.wfProcessForm, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName);
                Assert.NotNull(wfStartForm);
                Assert.Equal(WorkflowNodeType.Task, wfStartForm.TaskType);
                Assert.Equal(WorkflowFixture.node2_task.Name, wfStartForm.TaskName);
                // 默认预期的执行人：【CreatorManager：发起人(小肖)的主管--销售经理：齐副经理, 肖经理】
                Assert.Equal(WorkflowFixture.wfComSubAdminUserId + ", " + WorkflowFixture.wfSaleAdminUserId, wfStartForm.AllUserIds);

                _logger.LogInformation(string.Format("实例【wfInstance_7_1】--1. 预期的执行人：【CreatorManager：发起人(小肖)的主管--销售经理：齐副经理, 肖经理】"));
                _logger.LogInformation(string.Format("实例【wfInstance_7_1】--1. 获取启动后下一{0}节点【{1}】的审批人设置下的审批数据：同意: 【{2}】不同意: 【{3}】未处理: 【{4}】", wfStartForm.TaskType, wfStartForm.TaskName, wfStartForm.AgreeUserNames, wfStartForm.DisagreeUserNames, wfStartForm.UnProcessUserNames));
            }

            // 2. 创建流程实例：wfInstance_7_1：
            // 条件处理过程：node5-执行条件【field3>=20】
            //                    条件为-true，下一流程：Task:node2 （满足）
            //                    条件为-false，流程结束：End:node9
            // 启动后的流程图：Start:node1->Condition:node5->Task:node2(current)->Condition:node6->Task:node3->End:node9 
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 设置下一节点的审批人：齐总经理、肖经理
                var exceptExecuteUserIds = WorkflowFixture.wfComAdminUserId + ", " + WorkflowFixture.wfSaleAdminUserId;
                var exceptExecuteUserNames = WorkflowFixture.wfComAdminUserName + ", " + WorkflowFixture.wfSaleAdminUserName;
                wfStartForm.AllUserIds = exceptExecuteUserIds;
                wfStartForm.AllUserNames = exceptExecuteUserNames;
                wfStartForm.UnProcessUserIds = exceptExecuteUserIds;
                wfStartForm.UnProcessUserNames = exceptExecuteUserNames;

                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                WorkflowFixture.wfInstance_7_1 = wfService.StartWorkflowAsync(wfStartForm, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName).Result;
                Assert.NotNull(WorkflowFixture.wfInstance_7_1);
                _logger.LogInformation("实例【wfInstance_7_1】--2. 生成流程实例wfInstance_7_1：" + WorkflowFixture.wfInstance_7_1);
            }

            // 3. 获取流程实例：wfInstance_7_1
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 启动后的流程图：Start:node1->Condition:node5->Task:node2(current)->Condition:node6->Task:node3->End:node9  
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();

                #region 根据任务Code获取开始任务（GetStartTaskByProcessCodeAsync）
                var task = await wfService.GetStartTaskByProcessCodeAsync(WorkflowFixture.wfInstance_7_1);
                var message = wfService.PrintProcessLinkedTask(task);
                _logger.LogInformation("实例【wfInstance_7_1】--3. 流程实例：wfInstance_7_1，开始任务：" + message);
                Assert.NotNull(task);
                Assert.Equal(WorkflowNodeType.Start, task.NodeType);
                Assert.Equal(WorkflowTaskStatus.Finished, task.TaskStatus);
                var nextLevel0Condition = task.NextNode;
                Assert.NotNull(nextLevel0Condition);
                Assert.Equal(WorkflowNodeType.Condition, nextLevel0Condition.NodeType);
                Assert.Equal(WorkflowTaskStatus.Finished, nextLevel0Condition.TaskStatus);
                var nextLevel0Task = nextLevel0Condition.NextNode;
                Assert.NotNull(nextLevel0Task);
                Assert.Equal(WorkflowNodeType.Task, nextLevel0Task.NodeType);
                Assert.Equal(WorkflowFixture.node2_task.Name, nextLevel0Task.Name);
                Assert.Equal(WorkflowTaskStatus.Process, nextLevel0Task.TaskStatus);
                Assert.Equal(wfStartForm.AllUserNames, nextLevel0Task.AllUserNames);
                var nextLevel1Condition = nextLevel0Task.NextNode;
                Assert.NotNull(nextLevel1Condition);
                Assert.Equal(WorkflowNodeType.Condition, nextLevel1Condition.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, nextLevel1Condition.TaskStatus);
                var nextLevel1Task = nextLevel1Condition.NextNode;
                Assert.NotNull(nextLevel1Task);
                Assert.Equal(WorkflowNodeType.Task, nextLevel1Task.NodeType);
                Assert.Equal(WorkflowFixture.node3_task.Name, nextLevel1Task.Name);
                Assert.Equal(WorkflowTaskStatus.UnProcess, nextLevel1Task.TaskStatus);
                var nextLevel2End = nextLevel1Task.NextNode;
                Assert.NotNull(nextLevel2End);
                Assert.Equal(WorkflowNodeType.End, nextLevel2End.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, nextLevel2End.TaskStatus);
                #endregion

                #region 根据任务Code获取当前执行的任务【启动任务后，当前任务为：node2-task】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_7_1);
                Assert.NotNull(currentTask);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowFixture.node2_task.Name, currentTask.Name);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                Assert.Equal(wfStartForm.AllUserNames, currentTask.AllUserNames);
                Assert.NotNull(currentTask.NextNode);
                Assert.Equal(WorkflowNodeType.Condition, currentTask.NextNode.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 4. 流程流转前，获取下一任务节点【node2-task】的审批设置下的审批数据
            WorkflowExecuteData wfSubmitForm;
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                //获取下一任务节点【node2-task】的审批人设置下的审批数据
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                wfSubmitForm = await wfService.GetSubmitWorkflowExecutorInfoAsync(WorkflowFixture.wfInstance_7_1, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName);
                Assert.NotNull(wfSubmitForm);
                Assert.Equal(WorkflowNodeType.Task, wfSubmitForm.TaskType);
                Assert.Equal(WorkflowFixture.node2_task.Name, wfSubmitForm.TaskName);
                // 执行人设置：【CreatorManager：发起人的主管--销售经理:齐副经理, 肖经理】
                // 预期的执行人【齐副经理,肖经理】
                Assert.Equal(wfStartForm.AllUserIds, wfSubmitForm.AllUserIds);

                _logger.LogInformation(string.Format("实例【wfInstance_7_1】--4. 预期的执行人：【CreatorManager：发起人的主管--销售经理：齐副经理, 肖经理】"));
                _logger.LogInformation(string.Format("实例【wfInstance_7_1】--4. 获取流转前的下一{0}节点【{1}】的审批人设置下的审批数据：同意: 【{2}】不同意: 【{3}】未处理: 【{4}】", wfSubmitForm.TaskType, wfSubmitForm.TaskName, wfSubmitForm.AgreeUserNames, wfSubmitForm.DisagreeUserNames, wfSubmitForm.UnProcessUserNames));
            }

            // 5. 第一次处理（肖经理-Submit：不同意-退回）流程实例（wfInstance_7_1）
            // 流转后的流程图：(current)->Start:node1->Condition:node5->Task:node2->Condition:node6->Task:node3->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm.ExecuteUserId = WorkflowFixture.wfSaleAdminUserId;
                wfSubmitForm.ExecuteUserName = WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm.ExecuteStatus = WorkflowExecuteStatus.Return;
                wfSubmitForm.ExecuteRemark = "用户：【" + wfSubmitForm.ExecuteUserName + "】执行：不同意-退回";

                _logger.LogInformation("实例【wfInstance_7_1】--5. 流程流转：第一次处理（肖经理-Submit：不同意-退回）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_7_1, wfSubmitForm);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【退回任务后，将任务退回至发起人，当前任务为空】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_7_1);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_7_1】--5. 流程流转后，获取流程可执行任务流程图" + message);
                Assert.Null(currentTask);
                //Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                //Assert.Equal(WorkflowFixture.node2_task.Name, currentTask.Name);
                //Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                //Assert.Equal(wfSubmitForm.AllUserNames, currentTask.AllUserNames);
                //Assert.NotNull(currentTask.NextNode);
                //Assert.Equal(WorkflowNodeType.Condition, currentTask.NextNode.NodeType);
                //Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 6. 第二次处理（肖经理-Submit：同意）流程实例（wfInstance_7_1）
            //            node6-执行条件【field3>=40 && field5==true】
            //                   条件为true，下一流程：Task:node2
            //                   条件为false，流程结束：End:node9（满足）
            // 流转后的流程图：Start:node1->Condition:node5->Task:node2->Condition:node6->Task:node3->End:node9(current)
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm.ExecuteUserId = WorkflowFixture.wfSaleAdminUserId;
                wfSubmitForm.ExecuteUserName = WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm.ExecuteStatus = WorkflowExecuteStatus.Approve;
                wfSubmitForm.ExecuteRemark = "用户：【" + wfSubmitForm.ExecuteUserName + "】执行：同意提交";

                _logger.LogInformation("实例【wfInstance_7_1】--6. 流程流转：第二次处理（肖经理-Submit：同意）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_7_1, wfSubmitForm);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【流转任务后流程已结束，当前任务为空】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_7_1);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_7_1】--6. 流程流转后，获取流程可执行任务流程图：" + message);
                Assert.Null(currentTask);
                #endregion
            }
        }

        /// <summary>
        /// 复杂条件前置审批测试：wfInstance_7_2 </br>
        ///     流程定义-7：生成1个条件前置的复杂流程实例，启动后由两个条件及任务节点（Condition:node5【field3>=20】、Task:node2）及（Condition:node6【field3>=40 && field5==true】、Task:node3）组成
        ///     执行人设置：
        ///         Task:node2【CreatorManager：发起人的主管--销售经理：齐副经理, 肖经理】
        ///         Task:node3【Executor：组织/角色/用户--组织:【企业, 销售部】角色:【总经理, 销售经理】用户:【肖经理】】
        ///     设置表单值：【设置表单字段【field3】的实际值为：43；【field5】的实际值为：true】
        ///     条件处理过程：node5-执行条件【field3>=20】
        ///                         条件为true，下一流程：Task:node2（满足）
        ///                         条件为false，流程结束：End:node9
        ///                  node6-执行条件【field3>=40 && field5==true】
        ///                         条件为true，下一流程：Task:node3（满足）
        ///                         条件为false，流程结束：End:node9
        ///     启动后的流程图：Start:node1->Condtion:node5->Task:node2(current)->Condtion:node6->Task:node3->End:node9 
        ///     流转后的流程图：Start:node1->Condtion:node5->Task:node2->Condtion:node6->Task:node3->End:node9
        /// </summary>
        [Xunit.Fact]
        public async Task Test_WfProcessService_Instance_7_2()
        {
            // 1. 启动流程前，获取下一任务节点【node2-task】的审批人设置下的审批数据
            //      设置表单字段【field3】的实际值为：43；【field5】的实际值为：true
            WorkflowStartExecuteData wfStartForm;
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 设置表单字段【field3】的实际值为：43，【field5】的实际值为：true
                // 满足执行条件【field3>=20】【field3>=40 && field5=true】为：true
                WorkflowFixture.field3_currency.Value = "43";
                WorkflowFixture.field5_bool.Value = "true";

                //获取下一任务节点【node2-task】的审批人设置下的审批数据
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                wfStartForm = await wfService.GetStartWorkflowExecutorInfoAsync(WorkflowFixture.WorkflowDef_7.Code, WorkflowFixture.wfProcessForm, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName);
                _logger.LogInformation(string.Format("实例【wfInstance_7_2】--1. 预期的执行人：【CreatorManager：发起人(小肖)的主管--销售经理：齐副经理, 肖经理】"));
                _logger.LogInformation(string.Format("实例【wfInstance_7_2】--1. 获取启动后下一{0}节点【{1}】的审批人设置下的审批数据：同意: 【{2}】不同意: 【{3}】未处理: 【{4}】", wfStartForm.TaskType, wfStartForm.TaskName, wfStartForm.AgreeUserNames, wfStartForm.DisagreeUserNames, wfStartForm.UnProcessUserNames));

                Assert.NotNull(wfStartForm);
                Assert.Equal(WorkflowNodeType.Task, wfStartForm.TaskType);
                Assert.Equal(WorkflowFixture.node2_task.Name, wfStartForm.TaskName);
                // 默认预期的执行人：【CreatorManager：发起人(小肖)的主管--销售经理：齐副经理, 肖经理】
                Assert.Equal(WorkflowFixture.wfComSubAdminUserId + ", " + WorkflowFixture.wfSaleAdminUserId, wfStartForm.AllUserIds);
            }

            // 2. 创建流程实例：wfInstance_7_2：
            // 条件处理过程：node5-执行条件【field3>=20】
            //                    条件为-true，下一流程：Task:node2 （满足）
            //                    条件为-false，流程结束：End:node9
            // 启动后的流程图：Start:node1->Condition:node5->Task:node2(current)->Condition:node6->Task:node3->End:node9 
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 设置下一节点的审批人：齐总经理、肖经理
                var exceptExecuteUserIds = WorkflowFixture.wfComAdminUserId + ", " + WorkflowFixture.wfSaleAdminUserId;
                var exceptExecuteUserNames = WorkflowFixture.wfComAdminUserName + ", " + WorkflowFixture.wfSaleAdminUserName;
                wfStartForm.AllUserIds = exceptExecuteUserIds;
                wfStartForm.AllUserNames = exceptExecuteUserNames;
                wfStartForm.UnProcessUserIds = exceptExecuteUserIds;
                wfStartForm.UnProcessUserNames = exceptExecuteUserNames;

                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                WorkflowFixture.wfInstance_7_2 = wfService.StartWorkflowAsync(wfStartForm, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName).Result;
                Assert.NotNull(WorkflowFixture.wfInstance_7_2);
                _logger.LogInformation("实例【wfInstance_7_2】--2. 生成流程实例wfInstance_7_2：" + WorkflowFixture.wfInstance_7_2);
            }

            // 3. 获取流程实例：wfInstance_7_2
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 启动后的流程图：Start:node1->Condition:node5->Task:node2(current)->Condition:node6->Task:node3->End:node9  
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();

                #region 根据任务Code获取开始任务（GetStartTaskByProcessCodeAsync）
                var task = await wfService.GetStartTaskByProcessCodeAsync(WorkflowFixture.wfInstance_7_2);
                var message = wfService.PrintProcessLinkedTask(task);
                _logger.LogInformation("实例【wfInstance_7_2】--3. 流程实例：wfInstance_7_2，开始任务：" + message);
                Assert.NotNull(task);
                Assert.Equal(WorkflowNodeType.Start, task.NodeType);
                Assert.Equal(WorkflowTaskStatus.Finished, task.TaskStatus);
                var nextLevel0Condition = task.NextNode;
                Assert.NotNull(nextLevel0Condition);
                Assert.Equal(WorkflowNodeType.Condition, nextLevel0Condition.NodeType);
                Assert.Equal(WorkflowTaskStatus.Finished, nextLevel0Condition.TaskStatus);
                var nextLevel0Task = nextLevel0Condition.NextNode;
                Assert.NotNull(nextLevel0Task);
                Assert.Equal(WorkflowNodeType.Task, nextLevel0Task.NodeType);
                Assert.Equal(WorkflowFixture.node2_task.Name, nextLevel0Task.Name);
                Assert.Equal(WorkflowTaskStatus.Process, nextLevel0Task.TaskStatus);
                Assert.Equal(wfStartForm.AllUserNames, nextLevel0Task.AllUserNames);
                var nextLevel1Condition = nextLevel0Task.NextNode;
                Assert.NotNull(nextLevel1Condition);
                Assert.Equal(WorkflowNodeType.Condition, nextLevel1Condition.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, nextLevel1Condition.TaskStatus);
                var nextLevel1Task = nextLevel1Condition.NextNode;
                Assert.NotNull(nextLevel1Task);
                Assert.Equal(WorkflowNodeType.Task, nextLevel1Task.NodeType);
                Assert.Equal(WorkflowFixture.node3_task.Name, nextLevel1Task.Name);
                Assert.Equal(WorkflowTaskStatus.UnProcess, nextLevel1Task.TaskStatus);
                var nextLevel2End = nextLevel1Task.NextNode;
                Assert.NotNull(nextLevel2End);
                Assert.Equal(WorkflowNodeType.End, nextLevel2End.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, nextLevel2End.TaskStatus);
                #endregion

                #region 根据任务Code获取当前执行的任务【启动任务后，当前任务为：node2-task】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_7_2);
                Assert.NotNull(currentTask);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowFixture.node2_task.Name, currentTask.Name);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                Assert.Equal(wfStartForm.AllUserNames, currentTask.AllUserNames);
                Assert.NotNull(currentTask.NextNode);
                Assert.Equal(WorkflowNodeType.Condition, currentTask.NextNode.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 4. 流程流转前，获取下一任务节点【node2-task】的审批设置下的审批数据
            WorkflowExecuteData wfSubmitForm;
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                //获取下一任务节点【node2-task】的审批人设置下的审批数据
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                wfSubmitForm = await wfService.GetSubmitWorkflowExecutorInfoAsync(WorkflowFixture.wfInstance_7_2, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName);
                _logger.LogInformation(string.Format("实例【wfInstance_7_2】--4. 预期的执行人：【CreatorManager：发起人的主管--销售经理：齐副经理, 肖经理】"));
                _logger.LogInformation(string.Format("实例【wfInstance_7_2】--4. 获取流转前的下一{0}节点【{1}】的审批人设置下的审批数据：同意: 【{2}】不同意: 【{3}】未处理: 【{4}】", wfSubmitForm.TaskType, wfSubmitForm.TaskName, wfSubmitForm.AgreeUserNames, wfSubmitForm.DisagreeUserNames, wfSubmitForm.UnProcessUserNames));

                Assert.NotNull(wfSubmitForm);
                Assert.Equal(WorkflowNodeType.Task, wfSubmitForm.TaskType);
                Assert.Equal(WorkflowFixture.node2_task.Name, wfSubmitForm.TaskName);
                // 执行人设置：【CreatorManager：发起人的主管--销售经理:齐副经理, 肖经理】
                // 预期的执行人【齐副经理,肖经理】
                Assert.Equal(wfStartForm.AllUserIds, wfSubmitForm.AllUserIds);
            }

            // 5. 第一次处理（肖经理-Submit：不同意-退回）流程实例（wfInstance_7_2）
            // 流转后的流程图：(current)->Start:node1->Condition:node5->Task:node2->Condition:node6->Task:node3->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm.ExecuteUserId = WorkflowFixture.wfSaleAdminUserId;
                wfSubmitForm.ExecuteUserName = WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm.ExecuteStatus = WorkflowExecuteStatus.Return;
                wfSubmitForm.ExecuteRemark = "用户：【" + wfSubmitForm.ExecuteUserName + "】执行：不同意-退回";

                _logger.LogInformation("实例【wfInstance_7_2】--5. 流程流转：第一次处理（肖经理-Submit：不同意-退回）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_7_2, wfSubmitForm);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【退回任务后，将任务退回至发起人，当前任务为空】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_7_2);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_7_2】--5. 流程流转后，获取流程可执行任务流程图" + message);
                Assert.Null(currentTask);
                //Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                //Assert.Equal(WorkflowFixture.node2_task.Name, currentTask.Name);
                //Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                //Assert.Equal(wfSubmitForm.AllUserNames, currentTask.AllUserNames);
                //Assert.NotNull(currentTask.NextNode);
                //Assert.Equal(WorkflowNodeType.Condition, currentTask.NextNode.NodeType);
                //Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 6. 第二次处理（肖经理-Submit：同意）流程实例（wfInstance_7_2）
            //            node6-执行条件【field3>=40 && field5==true】
            //                   条件为true，下一流程：Task:node2（满足）
            //                   条件为false，流程结束：End:node9
            // 流转后的流程图：Start:node1->Condition:node5->Task:node2->Condition:node6->Task:node3(current)->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm.ExecuteUserId = WorkflowFixture.wfSaleAdminUserId;
                wfSubmitForm.ExecuteUserName = WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm.ExecuteStatus = WorkflowExecuteStatus.Approve;
                wfSubmitForm.ExecuteRemark = "用户：【" + wfSubmitForm.ExecuteUserName + "】执行：同意提交";

                _logger.LogInformation("实例【wfInstance_7_2】--6. 流程流转：第二次处理（肖经理-Submit：同意）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_7_2, wfSubmitForm);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【流程流转后，当前任务为：node3-task】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_7_2);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_7_2】--6. 流程流转后，获取流程可执行任务流程图：" + message);
                Assert.NotNull(currentTask);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowFixture.node3_task.Name, currentTask.Name);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                #endregion
            }

            // 7. 流程流转前，获取下一任务节点【node3-task】的审批设置下的审批数据
            WorkflowExecuteData wfSubmitForm_1;
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                //获取下一任务节点【node3-task】的审批人设置下的审批数据
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                wfSubmitForm_1 = await wfService.GetSubmitWorkflowExecutorInfoAsync(WorkflowFixture.wfInstance_7_2, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName);
                Assert.NotNull(wfSubmitForm_1);
                Assert.Equal(WorkflowNodeType.Task, wfSubmitForm_1.TaskType);
                Assert.Equal(WorkflowFixture.node3_task.Name, wfSubmitForm_1.TaskName);
                // 执行人【Executor：组织/角色/用户--组织:【企业, 销售部】角色:【总经理, 销售经理】用户:【肖经理】】
                // 预期的执行人【齐总经理,小齐,齐副经理,肖经理,小肖】 
                // Assert.Equal(wfStartForm.AllUserNames, wfSubmitForm_1.AllUserNames);

                _logger.LogInformation(string.Format("实例【wfInstance_7_2】--7. 预期的执行人：【齐总经理,小齐,齐副经理,肖经理,小肖】 "));
                //node3-task--org: 企业,销售部; role: 总经理,销售经理; user: 肖经理
                _logger.LogInformation(string.Format("实例【wfInstance_7_2】--7. 获取流转前的下一{0}节点【{1}】的审批人设置下的审批数据：同意: 【{2}】不同意: 【{3}】未处理: 【{4}】", wfSubmitForm_1.TaskType, wfSubmitForm_1.TaskName, wfSubmitForm_1.AgreeUserNames, wfSubmitForm_1.DisagreeUserNames, wfSubmitForm_1.UnProcessUserNames));
            }

            // 8. 第三次处理（肖经理-Submit：不同意-退回）流程实例（wfInstance_7_2）
            // 流转后的流程图：Start:node1->Condition:node5->Task:node2->Condition:node6->Task:node3(current)->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 设置下一节点的审批人：总经理：齐总经理、销售经理：齐副经理, 肖经理
                var exceptExecuteUserIds = WorkflowFixture.wfComAdminUserId + ", " + WorkflowFixture.wfComSubAdminUserId + ", " + WorkflowFixture.wfSaleAdminUserId;
                var exceptExecuteUserNames = WorkflowFixture.wfComAdminUserName + ", " + WorkflowFixture.wfComSubAdminUserName + ", " + WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm_1.AllUserIds = exceptExecuteUserIds;
                wfSubmitForm_1.AllUserNames = exceptExecuteUserNames;
                wfSubmitForm_1.UnProcessUserIds = exceptExecuteUserIds;
                wfSubmitForm_1.UnProcessUserNames = exceptExecuteUserNames;
                wfSubmitForm_1.ExecuteUserId = WorkflowFixture.wfSaleAdminUserId;
                wfSubmitForm_1.ExecuteUserName = WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm_1.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm_1.ExecuteStatus = WorkflowExecuteStatus.Return;
                wfSubmitForm_1.ExecuteRemark = "用户：【" + wfSubmitForm_1.ExecuteUserName + "】执行：不同意-退回";

                _logger.LogInformation("实例【wfInstance_7_2】--8. 流程流转：第三次处理（肖经理-Submit：不同意-退回）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_7_2, wfSubmitForm_1);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【退回任务后，当前任务为：node3-task】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_7_2);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_7_2】--8. 流程流转后，获取流程可执行任务流程图" + message);
                Assert.NotNull(currentTask);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowFixture.node3_task.Name, currentTask.Name);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                Assert.Equal(wfSubmitForm_1.AllUserIds, currentTask.AllUserIds);
                Assert.Equal(WorkflowFixture.wfSaleAdminUserId, currentTask.DisagreeUserIds);
                Assert.Equal(WorkflowFixture.wfComAdminUserId + ", " + WorkflowFixture.wfComSubAdminUserId, currentTask.UnProcessUserIds);
                Assert.NotNull(currentTask.NextNode);
                Assert.Equal(WorkflowNodeType.End, currentTask.NextNode.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 9. 第四次处理（齐副经理-Submit：同意）流程实例（wfInstance_7_2）
            //流转后的流程图：Start:node1->Condition:node5->Task:node2->Condition:node6->Task:node3(current)->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm_1.ExecuteUserId = WorkflowFixture.wfComSubAdminUserId;
                wfSubmitForm_1.ExecuteUserName = WorkflowFixture.wfComSubAdminUserName;
                wfSubmitForm_1.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm_1.ExecuteStatus = WorkflowExecuteStatus.Approve;
                wfSubmitForm_1.ExecuteRemark = "用户：【" + wfSubmitForm_1.ExecuteUserName + "】执行：同意提交";

                _logger.LogInformation("实例【wfInstance_7_2】--9. 流程流转：第四次处理（齐副经理-Submit：同意）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_7_2, wfSubmitForm_1);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【流程流转后，当前任务为：node3-task】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_7_2);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_7_2】--9. 流程流转后，获取流程可执行任务流程图：" + message);
                Assert.NotNull(currentTask);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowFixture.node3_task.Name, currentTask.Name);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                Assert.Equal(wfSubmitForm_1.AllUserIds, currentTask.AllUserIds);
                Assert.Equal(WorkflowFixture.wfComSubAdminUserId, currentTask.AgreeUserIds);
                Assert.Equal(WorkflowFixture.wfComAdminUserId, currentTask.UnProcessUserIds);
                Assert.NotNull(currentTask.NextNode);
                Assert.Equal(WorkflowNodeType.End, currentTask.NextNode.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 10. 第五次处理（齐总经理-Submit：同意）流程实例（wfInstance_7_2）
            //流转后的流程图：Start:node1->Condition:node5->Task:node2->Condition:node6->Task:node3->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm_1.ExecuteUserId = WorkflowFixture.wfComAdminUserId;
                wfSubmitForm_1.ExecuteUserName = WorkflowFixture.wfComAdminUserName;
                wfSubmitForm_1.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm_1.ExecuteStatus = WorkflowExecuteStatus.Approve;
                wfSubmitForm_1.ExecuteRemark = "用户：【" + wfSubmitForm_1.ExecuteUserName + "】执行：同意提交";

                _logger.LogInformation("实例【wfInstance_7_2】--10. 流程流转：第五次处理（齐总经理-Submit：同意）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_7_2, wfSubmitForm_1);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【流转任务后流程已结束，当前任务为空】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_7_2);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_7_2】--10. 流程流转后，获取流程可执行任务流程图：" + message);
                Assert.Null(currentTask);
                #endregion
            }
        }

        /// <summary>
        /// 复杂条件后置审批测试：wfInstance_8_1 </br>
        ///     流程定义-7：生成1个条件前置的复杂流程实例，启动后由两个条件及任务节点（Task:node2、Condition:node5【field3>=20】）及（Task:node3、Condition:node6【field3>=40 && field5==true】）组成
        ///     执行人设置：
        ///         Task:node2【CreatorManager：发起人的主管--销售经理：齐副经理, 肖经理】
        ///         Task:node3【Executor：组织/角色/用户--组织:【企业, 销售部】角色:【总经理, 销售经理】用户:【肖经理】】
        ///     设置表单值：【设置表单字段【field3】的实际值为：43；【field5】的实际值为：true】
        ///     条件处理过程：node5-执行条件【field3>=20】
        ///                         条件为true，下一流程：Task:node2（满足）
        ///                         条件为false，流程结束：End:node9
        ///                  node6-执行条件【field3>=40 && field5==true】
        ///                         条件为true，下一流程：Task:node3（满足）
        ///                         条件为false，流程结束：Task:node2
        ///     启动后的流程图：Start:node1->Task:node2(current)->Condtion:node5->Task:node3->Condtion:node6->End:node9 
        ///     流转一次后的流程图：Start:node1->Task:node2(current)->Condition:node5->Task:node3->Condition:node6->End:node9
        ///     流转两次后的流程图：Start:node1->Task:node2->Condition:node5->Task:node3(current)->Condition:node6->End:node9
        ///     流转三次后的流程图：Start:node1->Task:node2->Condition:node5->Task:node3(current)->Condition:node6->End:node9
        ///     流转四次后的流程图：Start:node1->Task:node2->Condition:node5->Task:node3(current)->Condition:node6->End:node9
        ///     流转五次后的流程图：Start:node1->Task:node2->Condition:node5->Task:node3->Condition:node6->End:node9
        /// </summary>
        [Xunit.Fact]
        public async Task Test_WfProcessService_Instance_8_1()
        {
            // 1. 启动流程前，获取下一任务节点【node2-task】的审批人设置下的审批数据
            //      设置表单字段【field3】的实际值为：43；【field5】的实际值为：true
            WorkflowStartExecuteData wfStartForm;
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 设置表单字段【field3】的实际值为：43，【field5】的实际值为：true
                // 满足执行条件【field3>=20】【field3>=40 && field5=true】为：true
                WorkflowFixture.field3_currency.Value = "43";
                WorkflowFixture.field5_bool.Value = "true";

                //获取下一任务节点【node2-task】的审批人设置下的审批数据
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                wfStartForm = await wfService.GetStartWorkflowExecutorInfoAsync(WorkflowFixture.WorkflowDef_8.Code, WorkflowFixture.wfProcessForm, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName);
                _logger.LogInformation(string.Format("实例【wfInstance_8_1】--1. 预期的执行人：【CreatorManager：发起人(小肖)的主管--销售经理：齐副经理, 肖经理】"));
                _logger.LogInformation(string.Format("实例【wfInstance_8_1】--1. 获取启动后下一{0}节点【{1}】的审批人设置下的审批数据：同意: 【{2}】不同意: 【{3}】未处理: 【{4}】", wfStartForm.TaskType, wfStartForm.TaskName, wfStartForm.AgreeUserNames, wfStartForm.DisagreeUserNames, wfStartForm.UnProcessUserNames));

                Assert.NotNull(wfStartForm);
                Assert.Equal(WorkflowNodeType.Task, wfStartForm.TaskType);
                Assert.Equal(WorkflowFixture.node2_task.Name, wfStartForm.TaskName);
                // 默认预期的执行人：【CreatorManager：发起人(小肖)的主管--销售经理：齐副经理, 肖经理】
                Assert.Equal(WorkflowFixture.wfComSubAdminUserId + ", " + WorkflowFixture.wfSaleAdminUserId, wfStartForm.AllUserIds);
            }

            // 2. 创建流程实例：wfInstance_8_1：
            // 启动后的流程图：Start:node1->Task:node2(current)->Condtion:node5->Task:node3->Condtion:node6->End:node9 
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 设置下一节点的审批人：齐总经理、肖经理
                var exceptExecuteUserIds = WorkflowFixture.wfComAdminUserId + ", " + WorkflowFixture.wfSaleAdminUserId;
                var exceptExecuteUserNames = WorkflowFixture.wfComAdminUserName + ", " + WorkflowFixture.wfSaleAdminUserName;
                wfStartForm.AllUserIds = exceptExecuteUserIds;
                wfStartForm.AllUserNames = exceptExecuteUserNames;
                wfStartForm.UnProcessUserIds = exceptExecuteUserIds;
                wfStartForm.UnProcessUserNames = exceptExecuteUserNames;

                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                WorkflowFixture.wfInstance_8_1 = wfService.StartWorkflowAsync(wfStartForm, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName).Result;
                Assert.NotNull(WorkflowFixture.wfInstance_8_1);
                _logger.LogInformation("实例【wfInstance_8_1】--2. 生成流程实例wfInstance_8_1：" + WorkflowFixture.wfInstance_8_1);
            }

            // 3. 获取流程实例：wfInstance_8_1
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 启动后的流程图：Start:node1->Condition:node5->Task:node2(current)->Condition:node6->Task:node3->End:node9  
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();

                #region 根据任务Code获取开始任务（GetStartTaskByProcessCodeAsync）
                var task = await wfService.GetStartTaskByProcessCodeAsync(WorkflowFixture.wfInstance_8_1);
                var message = wfService.PrintProcessLinkedTask(task);
                _logger.LogInformation("实例【wfInstance_8_1】--3. 流程实例：wfInstance_8_1，开始任务：" + message);
                Assert.NotNull(task);
                Assert.Equal(WorkflowNodeType.Start, task.NodeType);
                Assert.Equal(WorkflowTaskStatus.Finished, task.TaskStatus);
                var nextLevel0Task = task.NextNode;
                Assert.NotNull(nextLevel0Task);
                Assert.Equal(WorkflowNodeType.Task, nextLevel0Task.NodeType);
                Assert.Equal(WorkflowFixture.node2_task.Name, nextLevel0Task.Name);
                Assert.Equal(WorkflowTaskStatus.Process, nextLevel0Task.TaskStatus);
                Assert.Equal(wfStartForm.AllUserNames, nextLevel0Task.AllUserNames);
                var nextLevel0Condition = nextLevel0Task.NextNode;
                Assert.NotNull(nextLevel0Condition);
                Assert.Equal(WorkflowNodeType.Condition, nextLevel0Condition.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, nextLevel0Condition.TaskStatus);
                var nextLevel1Task = nextLevel0Condition.NextNode;
                Assert.NotNull(nextLevel1Task);
                Assert.Equal(WorkflowNodeType.Task, nextLevel1Task.NodeType);
                Assert.Equal(WorkflowFixture.node3_task.Name, nextLevel1Task.Name);
                Assert.Equal(WorkflowTaskStatus.UnProcess, nextLevel1Task.TaskStatus);
                var nextLevel1Condition = nextLevel1Task.NextNode;
                Assert.NotNull(nextLevel1Condition);
                Assert.Equal(WorkflowNodeType.Condition, nextLevel1Condition.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, nextLevel1Condition.TaskStatus);
                var nextLevel2End = nextLevel1Condition.NextNode;
                Assert.NotNull(nextLevel2End);
                Assert.Equal(WorkflowNodeType.End, nextLevel2End.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, nextLevel2End.TaskStatus);
                #endregion

                #region 根据任务Code获取当前执行的任务【启动任务后，当前任务为：node2-task】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_8_1);
                Assert.NotNull(currentTask);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowFixture.node2_task.Name, currentTask.Name);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                Assert.Equal(wfStartForm.AllUserNames, currentTask.AllUserNames);
                Assert.NotNull(currentTask.NextNode);
                Assert.Equal(WorkflowNodeType.Condition, currentTask.NextNode.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 4. 流程流转前，获取下一任务节点【node2-task】的审批设置下的审批数据
            WorkflowExecuteData wfSubmitForm;
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                //获取下一任务节点【node2-task】的审批人设置下的审批数据
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                wfSubmitForm = await wfService.GetSubmitWorkflowExecutorInfoAsync(WorkflowFixture.wfInstance_8_1, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName);
                _logger.LogInformation(string.Format("实例【wfInstance_8_1】--4. 预期的执行人：【CreatorManager：发起人的主管--销售经理：齐副经理, 肖经理】"));
                _logger.LogInformation(string.Format("实例【wfInstance_8_1】--4. 获取流转前的下一{0}节点【{1}】的审批人设置下的审批数据：同意: 【{2}】不同意: 【{3}】未处理: 【{4}】", wfSubmitForm.TaskType, wfSubmitForm.TaskName, wfSubmitForm.AgreeUserNames, wfSubmitForm.DisagreeUserNames, wfSubmitForm.UnProcessUserNames));

                Assert.NotNull(wfSubmitForm);
                Assert.Equal(WorkflowNodeType.Task, wfSubmitForm.TaskType);
                Assert.Equal(WorkflowFixture.node2_task.Name, wfSubmitForm.TaskName);
                // 执行人设置：【CreatorManager：发起人的主管--销售经理:齐副经理, 肖经理】
                // 预期的执行人【齐副经理,肖经理】
                Assert.Equal(wfStartForm.AllUserIds, wfSubmitForm.AllUserIds);
            }

            // 5. 第一次处理（肖经理-Submit：不同意-退回）流程实例（wfInstance_8_1）
            // 流转后的流程图：(current)->Start:node1->Task:node2->Condition:node5->Task:node3->Condition:node6->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm.ExecuteUserId = WorkflowFixture.wfSaleAdminUserId;
                wfSubmitForm.ExecuteUserName = WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm.ExecuteStatus = WorkflowExecuteStatus.Return;
                wfSubmitForm.ExecuteRemark = "用户：【" + wfSubmitForm.ExecuteUserName + "】执行：不同意-退回";

                _logger.LogInformation("实例【wfInstance_8_1】--5. 流程流转：第一次处理（肖经理-Submit：不同意-退回）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_8_1, wfSubmitForm);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【退回任务后，将任务退回至发起人，当前任务为空】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_8_1);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_8_1】--5. 流程流转后，获取流程可执行任务流程图" + message);
                Assert.Null(currentTask);
                //Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                //Assert.Equal(WorkflowFixture.node2_task.Name, currentTask.Name);
                //Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                //Assert.Equal(wfSubmitForm.AllUserNames, currentTask.AllUserNames);
                //Assert.NotNull(currentTask.NextNode);
                //Assert.Equal(WorkflowNodeType.Condition, currentTask.NextNode.NodeType);
                //Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 6. 第二次处理（肖经理-Submit：同意）流程实例（wfInstance_8_1）
            // 条件处理过程：node5-执行条件【field3>=20】
            //                    条件为-true，下一流程：Task:node2 （满足）
            //                    条件为-false，流程结束：End:node9
            // 流转后的流程图：Start:node1->Task:node2->Condition:node5->Task:node3(current)->Condition:node6->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm.ExecuteUserId = WorkflowFixture.wfSaleAdminUserId;
                wfSubmitForm.ExecuteUserName = WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm.ExecuteStatus = WorkflowExecuteStatus.Approve;
                wfSubmitForm.ExecuteRemark = "用户：【" + wfSubmitForm.ExecuteUserName + "】执行：同意提交";

                _logger.LogInformation("实例【wfInstance_8_1】--6. 流程流转：第二次处理（肖经理-Submit：同意）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_8_1, wfSubmitForm);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【流程流转后，当前任务为：node3-task】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_8_1);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_8_1】--6. 流程流转后，获取流程可执行任务流程图：" + message);
                Assert.NotNull(currentTask);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowFixture.node3_task.Name, currentTask.Name);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                #endregion
            }

            // 7. 流程流转前，获取下一任务节点【node3-task】的审批设置下的审批数据
            WorkflowExecuteData wfSubmitForm_1;
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                //获取下一任务节点【node3-task】的审批人设置下的审批数据
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                wfSubmitForm_1 = await wfService.GetSubmitWorkflowExecutorInfoAsync(WorkflowFixture.wfInstance_8_1, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName);
                _logger.LogInformation(string.Format("实例【wfInstance_8_1】--7. 预期的执行人：【齐总经理,小齐,齐副经理,肖经理,小肖】 "));
                //node3-task--org: 企业,销售部; role: 总经理,销售经理; user: 肖经理
                _logger.LogInformation(string.Format("实例【wfInstance_8_1】--7. 获取流转前的下一{0}节点【{1}】的审批人设置下的审批数据：同意: 【{2}】不同意: 【{3}】未处理: 【{4}】", wfSubmitForm_1.TaskType, wfSubmitForm_1.TaskName, wfSubmitForm_1.AgreeUserNames, wfSubmitForm_1.DisagreeUserNames, wfSubmitForm_1.UnProcessUserNames));

                Assert.NotNull(wfSubmitForm_1);
                Assert.Equal(WorkflowNodeType.Task, wfSubmitForm_1.TaskType);
                Assert.Equal(WorkflowFixture.node3_task.Name, wfSubmitForm_1.TaskName);
                // 执行人【Executor：组织/角色/用户--组织:【企业, 销售部】角色:【总经理, 销售经理】用户:【肖经理】】
                // 预期的执行人【齐总经理,小齐,齐副经理,肖经理,小肖】 
                // Assert.Equal(wfStartForm.AllUserNames, wfSubmitForm_1.AllUserNames);
            }

            // 8. 第三次处理（肖经理-Submit：不同意-退回）流程实例（wfInstance_8_1）
            // 流转后的流程图：Start:node1->Task:node2->Condition:node5->Task:node3(current)->Condition:node6->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 设置下一节点的审批人：总经理：齐总经理、销售经理：齐副经理, 肖经理
                var exceptExecuteUserIds = WorkflowFixture.wfComAdminUserId + ", " + WorkflowFixture.wfComSubAdminUserId + ", " + WorkflowFixture.wfSaleAdminUserId;
                var exceptExecuteUserNames = WorkflowFixture.wfComAdminUserName + ", " + WorkflowFixture.wfComSubAdminUserName + ", " + WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm_1.AllUserIds = exceptExecuteUserIds;
                wfSubmitForm_1.AllUserNames = exceptExecuteUserNames;
                wfSubmitForm_1.UnProcessUserIds = exceptExecuteUserIds;
                wfSubmitForm_1.UnProcessUserNames = exceptExecuteUserNames;
                wfSubmitForm_1.ExecuteUserId = WorkflowFixture.wfSaleAdminUserId;
                wfSubmitForm_1.ExecuteUserName = WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm_1.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm_1.ExecuteStatus = WorkflowExecuteStatus.Return;
                wfSubmitForm_1.ExecuteRemark = "用户：【" + wfSubmitForm_1.ExecuteUserName + "】执行：不同意-退回";

                _logger.LogInformation("实例【wfInstance_8_1】--8. 流程流转：第三次处理（肖经理-Submit：不同意-退回）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_8_1, wfSubmitForm_1);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【退回任务后，当前任务为：node3-task】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_8_1);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_8_1】--8. 流程流转后，获取流程可执行任务流程图" + message);
                Assert.NotNull(currentTask);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowFixture.node3_task.Name, currentTask.Name);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                Assert.Equal(wfSubmitForm_1.AllUserIds, currentTask.AllUserIds);
                Assert.Equal(WorkflowFixture.wfSaleAdminUserId, currentTask.DisagreeUserIds);
                Assert.Equal(WorkflowFixture.wfComAdminUserId + ", " + WorkflowFixture.wfComSubAdminUserId, currentTask.UnProcessUserIds);
                Assert.NotNull(currentTask.NextNode);
                Assert.Equal(WorkflowNodeType.Condition, currentTask.NextNode.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 9. 第四次处理（齐副经理-Submit：同意）流程实例（wfInstance_8_1）
            //流转后的流程图：Start:node1->Task:node2->Condition:node5->Task:node3(current)->Condition:node6->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm_1.ExecuteUserId = WorkflowFixture.wfComSubAdminUserId;
                wfSubmitForm_1.ExecuteUserName = WorkflowFixture.wfComSubAdminUserName;
                wfSubmitForm_1.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm_1.ExecuteStatus = WorkflowExecuteStatus.Approve;
                wfSubmitForm_1.ExecuteRemark = "用户：【" + wfSubmitForm_1.ExecuteUserName + "】执行：同意提交";

                _logger.LogInformation("实例【wfInstance_8_1】--9. 流程流转：第四次处理（齐副经理-Submit：同意）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_8_1, wfSubmitForm_1);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【流程流转后，当前任务为：node3-task】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_8_1);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_8_1】--9. 流程流转后，获取流程可执行任务流程图：" + message);
                Assert.NotNull(currentTask);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowFixture.node3_task.Name, currentTask.Name);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                Assert.Equal(wfSubmitForm_1.AllUserIds, currentTask.AllUserIds);
                Assert.Equal(WorkflowFixture.wfComSubAdminUserId, currentTask.AgreeUserIds);
                Assert.Equal(WorkflowFixture.wfComAdminUserId, currentTask.UnProcessUserIds);
                Assert.NotNull(currentTask.NextNode);
                Assert.Equal(WorkflowNodeType.Condition, currentTask.NextNode.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 10. 第五次处理（齐总经理-Submit：同意）流程实例（wfInstance_8_1）
            //            node6-执行条件【field3>=40 && field5==true】
            //                   条件为true，下一流程：Task:node2（满足）
            //                   条件为false，流程结束：End:node9
            //流转后的流程图：Start:node1->Task:node2->Condition:node5->Task:node3->Condition:node6->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm_1.ExecuteUserId = WorkflowFixture.wfComAdminUserId;
                wfSubmitForm_1.ExecuteUserName = WorkflowFixture.wfComAdminUserName;
                wfSubmitForm_1.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm_1.ExecuteStatus = WorkflowExecuteStatus.Approve;
                wfSubmitForm_1.ExecuteRemark = "用户：【" + wfSubmitForm_1.ExecuteUserName + "】执行：同意提交";

                _logger.LogInformation("实例【wfInstance_8_1】--10. 流程流转：第五次处理（齐总经理-Submit：同意）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_8_1, wfSubmitForm_1);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【流转任务后流程已结束，当前任务为空】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_8_1);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_8_1】--10. 流程流转后，获取流程可执行任务流程图：" + message);
                Assert.Null(currentTask);
                #endregion
            }
        }

        /// <summary>
        /// 复杂条件后置审批测试：wfInstance_8_2 </br>
        ///     流程定义-7：生成1个条件前置的复杂流程实例，启动后由两个条件及任务节点（Task:node2、Condition:node5【field3>=20】）及（Task:node3、Condition:node6【field3>=40 && field5==true】）组成
        ///     执行人设置：
        ///         Task:node2【CreatorManager：发起人的主管--销售经理：齐副经理, 肖经理】
        ///         Task:node3【Executor：组织/角色/用户--组织:【企业, 销售部】角色:【总经理, 销售经理】用户:【肖经理】】
        ///     设置表单值：【设置表单字段【field3】的实际值为：23；【field5】的实际值为：true】
        ///     条件处理过程：node5-执行条件【field3>=20】
        ///                         条件为true，下一流程：Task:node2（满足）
        ///                         条件为false，流程结束：End:node9
        ///                  node6-执行条件【field3>=40 && field5==true】
        ///                         条件为true，下一流程：Task:node3（满足）
        ///                         条件为false，流程结束：Task:node2
        ///     
        ///     流转一次后的流程图：Start:node1->Task:node2(current)->Condition:node5->Task:node3->Condition:node6->End:node9
        ///     流转两次后的流程图：Start:node1->Task:node2->Condition:node5->Task:node3(current)->Condition:node6->End:node9
        ///     流转三次后的流程图：Start:node1->Task:node2->Condition:node5->Task:node3(current)->Condition:node6->End:node9
        ///     流转四次后的流程图：Start:node1->Task:node2->Condition:node5->Task:node3(current)->Condition:node6->End:node9
        ///     流转五次后的流程图：Start:node1->Task:node2->Condition:node5->Task:node3->Condition:node6->End:node9
        /// </summary>
        [Xunit.Fact]
        public async Task Test_WfProcessService_Instance_8_2()
        {
            // 1. 启动流程前，获取下一任务节点【node2-task】的审批人设置下的审批数据
            //      设置表单字段【field3】的实际值为：23；【field5】的实际值为：true
            WorkflowStartExecuteData wfStartForm;
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 设置表单字段【field3】的实际值为：23，【field5】的实际值为：true
                // 满足执行条件【field3>=20】为：false【field3>=40 && field5=true】为：true
                WorkflowFixture.field3_currency.Value = "23";
                WorkflowFixture.field5_bool.Value = "true";

                //获取下一任务节点【node2-task】的审批人设置下的审批数据
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                wfStartForm = await wfService.GetStartWorkflowExecutorInfoAsync(WorkflowFixture.WorkflowDef_8.Code, WorkflowFixture.wfProcessForm, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName);
                Assert.NotNull(wfStartForm);
                Assert.Equal(WorkflowNodeType.Task, wfStartForm.TaskType);
                Assert.Equal(WorkflowFixture.node2_task.Name, wfStartForm.TaskName);
                // 默认预期的执行人：【CreatorManager：发起人(小肖)的主管--销售经理：齐副经理, 肖经理】
                Assert.Equal(WorkflowFixture.wfComSubAdminUserId + ", " + WorkflowFixture.wfSaleAdminUserId, wfStartForm.AllUserIds);

                _logger.LogInformation(string.Format("实例【wfInstance_8_2】--1. 预期的执行人：【CreatorManager：发起人(小肖)的主管--销售经理：齐副经理, 肖经理】"));
                _logger.LogInformation(string.Format("实例【wfInstance_8_2】--1. 获取启动后下一{0}节点【{1}】的审批人设置下的审批数据：同意: 【{2}】不同意: 【{3}】未处理: 【{4}】", wfStartForm.TaskType, wfStartForm.TaskName, wfStartForm.AgreeUserNames, wfStartForm.DisagreeUserNames, wfStartForm.UnProcessUserNames));
            }

            // 2. 创建流程实例：wfInstance_8_2：
            // 启动后的流程图：Start:node1->Task:node2(current)->Condtion:node5->Task:node3->Condtion:node6->End:node9 
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 设置下一节点的审批人：齐总经理、肖经理
                var exceptExecuteUserIds = WorkflowFixture.wfComAdminUserId + ", " + WorkflowFixture.wfSaleAdminUserId;
                var exceptExecuteUserNames = WorkflowFixture.wfComAdminUserName + ", " + WorkflowFixture.wfSaleAdminUserName;
                wfStartForm.AllUserIds = exceptExecuteUserIds;
                wfStartForm.AllUserNames = exceptExecuteUserNames;
                wfStartForm.UnProcessUserIds = exceptExecuteUserIds;
                wfStartForm.UnProcessUserNames = exceptExecuteUserNames;

                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                WorkflowFixture.wfInstance_8_2 = wfService.StartWorkflowAsync(wfStartForm, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName).Result;
                Assert.NotNull(WorkflowFixture.wfInstance_8_2);
                _logger.LogInformation("实例【wfInstance_8_2】--2. 生成流程实例wfInstance_8_2：" + WorkflowFixture.wfInstance_8_2);
            }

            // 3. 获取流程实例：wfInstance_8_2
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 启动后的流程图：Start:node1->Condition:node5->Task:node2(current)->Condition:node6->Task:node3->End:node9  
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();

                #region 根据任务Code获取开始任务（GetStartTaskByProcessCodeAsync）
                var task = await wfService.GetStartTaskByProcessCodeAsync(WorkflowFixture.wfInstance_8_2);
                var message = wfService.PrintProcessLinkedTask(task);
                _logger.LogInformation("实例【wfInstance_8_2】--3. 流程实例：wfInstance_8_2，开始任务：" + message);
                Assert.NotNull(task);
                Assert.Equal(WorkflowNodeType.Start, task.NodeType);
                Assert.Equal(WorkflowTaskStatus.Finished, task.TaskStatus);
                var nextLevel0Task = task.NextNode;
                Assert.NotNull(nextLevel0Task);
                Assert.Equal(WorkflowNodeType.Task, nextLevel0Task.NodeType);
                Assert.Equal(WorkflowFixture.node2_task.Name, nextLevel0Task.Name);
                Assert.Equal(WorkflowTaskStatus.Process, nextLevel0Task.TaskStatus);
                Assert.Equal(wfStartForm.AllUserNames, nextLevel0Task.AllUserNames);
                var nextLevel0Condition = nextLevel0Task.NextNode;
                Assert.NotNull(nextLevel0Condition);
                Assert.Equal(WorkflowNodeType.Condition, nextLevel0Condition.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, nextLevel0Condition.TaskStatus);
                var nextLevel1Task = nextLevel0Condition.NextNode;
                Assert.NotNull(nextLevel1Task);
                Assert.Equal(WorkflowNodeType.Task, nextLevel1Task.NodeType);
                Assert.Equal(WorkflowFixture.node3_task.Name, nextLevel1Task.Name);
                Assert.Equal(WorkflowTaskStatus.UnProcess, nextLevel1Task.TaskStatus);
                var nextLevel1Condition = nextLevel1Task.NextNode;
                Assert.NotNull(nextLevel1Condition);
                Assert.Equal(WorkflowNodeType.Condition, nextLevel1Condition.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, nextLevel1Condition.TaskStatus);
                var nextLevel2End = nextLevel1Condition.NextNode;
                Assert.NotNull(nextLevel2End);
                Assert.Equal(WorkflowNodeType.End, nextLevel2End.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, nextLevel2End.TaskStatus);
                #endregion

                #region 根据任务Code获取当前执行的任务【启动任务后，当前任务为：node2-task】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_8_2);
                Assert.NotNull(currentTask);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowFixture.node2_task.Name, currentTask.Name);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                Assert.Equal(wfStartForm.AllUserNames, currentTask.AllUserNames);
                Assert.NotNull(currentTask.NextNode);
                Assert.Equal(WorkflowNodeType.Condition, currentTask.NextNode.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 4. 流程流转前，获取下一任务节点【node2-task】的审批设置下的审批数据
            WorkflowExecuteData wfSubmitForm;
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                //获取下一任务节点【node2-task】的审批人设置下的审批数据
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                wfSubmitForm = await wfService.GetSubmitWorkflowExecutorInfoAsync(WorkflowFixture.wfInstance_8_2, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName);
                Assert.NotNull(wfSubmitForm);
                Assert.Equal(WorkflowNodeType.Task, wfSubmitForm.TaskType);
                Assert.Equal(WorkflowFixture.node2_task.Name, wfSubmitForm.TaskName);
                // 执行人设置：【CreatorManager：发起人的主管--销售经理:齐副经理, 肖经理】
                // 预期的执行人【齐副经理,肖经理】
                Assert.Equal(wfStartForm.AllUserIds, wfSubmitForm.AllUserIds);

                _logger.LogInformation(string.Format("实例【wfInstance_8_2】--4. 预期的执行人：【CreatorManager：发起人的主管--销售经理：齐副经理, 肖经理】"));
                _logger.LogInformation(string.Format("实例【wfInstance_8_2】--4. 获取流转前的下一{0}节点【{1}】的审批人设置下的审批数据：同意: 【{2}】不同意: 【{3}】未处理: 【{4}】", wfSubmitForm.TaskType, wfSubmitForm.TaskName, wfSubmitForm.AgreeUserNames, wfSubmitForm.DisagreeUserNames, wfSubmitForm.UnProcessUserNames));
            }

            // 5. 第一次处理（肖经理-Submit：不同意-退回）流程实例（wfInstance_8_2）
            // 流转后的流程图：(current)->Start:node1->Task:node2->Condition:node5->Task:node3->Condition:node6->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm.ExecuteUserId = WorkflowFixture.wfSaleAdminUserId;
                wfSubmitForm.ExecuteUserName = WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm.ExecuteStatus = WorkflowExecuteStatus.Return;
                wfSubmitForm.ExecuteRemark = "用户：【" + wfSubmitForm.ExecuteUserName + "】执行：不同意-退回";

                _logger.LogInformation("实例【wfInstance_8_2】--5. 流程流转：第一次处理（肖经理-Submit：不同意-退回）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_8_2, wfSubmitForm);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【退回任务后，将任务退回至发起人，当前任务为空】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_8_2);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_8_2】--5. 流程流转后，获取流程可执行任务流程图" + message);
                Assert.Null(currentTask);
                //Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                //Assert.Equal(WorkflowFixture.node2_task.Name, currentTask.Name);
                //Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                //Assert.Equal(wfSubmitForm.AllUserNames, currentTask.AllUserNames);
                //Assert.NotNull(currentTask.NextNode);
                //Assert.Equal(WorkflowNodeType.Condition, currentTask.NextNode.NodeType);
                //Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 6. 第二次处理（肖经理-Submit：同意）流程实例（wfInstance_8_2）
            // 条件处理过程：node5-执行条件【field3>=20】
            //                    条件为-true，下一流程：Task:node2 （满足）
            //                    条件为-false，流程结束：End:node9
            // 流转后的流程图：Start:node1->Task:node2->Condition:node5->Task:node3(current)->Condition:node6->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm.ExecuteUserId = WorkflowFixture.wfSaleAdminUserId;
                wfSubmitForm.ExecuteUserName = WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm.ExecuteStatus = WorkflowExecuteStatus.Approve;
                wfSubmitForm.ExecuteRemark = "用户：【" + wfSubmitForm.ExecuteUserName + "】执行：同意提交";

                _logger.LogInformation("实例【wfInstance_8_2】--6. 流程流转：第二次处理（肖经理-Submit：同意）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_8_2, wfSubmitForm);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【流程流转后，当前任务为：node3-task】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_8_2);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_8_2】--6. 流程流转后，获取流程可执行任务流程图：" + message);
                Assert.NotNull(currentTask);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowFixture.node3_task.Name, currentTask.Name);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                #endregion
            }

            // 7. 流程流转前，获取下一任务节点【node3-task】的审批设置下的审批数据
            WorkflowExecuteData wfSubmitForm_1;
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                //获取下一任务节点【node3-task】的审批人设置下的审批数据
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                wfSubmitForm_1 = await wfService.GetSubmitWorkflowExecutorInfoAsync(WorkflowFixture.wfInstance_8_2, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName);
                Assert.NotNull(wfSubmitForm_1);
                Assert.Equal(WorkflowNodeType.Task, wfSubmitForm_1.TaskType);
                Assert.Equal(WorkflowFixture.node3_task.Name, wfSubmitForm_1.TaskName);
                // 执行人【Executor：组织/角色/用户--组织:【企业, 销售部】角色:【总经理, 销售经理】用户:【肖经理】】
                // 预期的执行人【齐总经理,小齐,齐副经理,肖经理,小肖】 
                // Assert.Equal(wfStartForm.AllUserNames, wfSubmitForm_1.AllUserNames);

                _logger.LogInformation(string.Format("实例【wfInstance_8_2】--7. 预期的执行人：【齐总经理,小齐,齐副经理,肖经理,小肖】 "));
                //node3-task--org: 企业,销售部; role: 总经理,销售经理; user: 肖经理
                _logger.LogInformation(string.Format("实例【wfInstance_8_2】--7. 获取流转前的下一{0}节点【{1}】的审批人设置下的审批数据：同意: 【{2}】不同意: 【{3}】未处理: 【{4}】", wfSubmitForm_1.TaskType, wfSubmitForm_1.TaskName, wfSubmitForm_1.AgreeUserNames, wfSubmitForm_1.DisagreeUserNames, wfSubmitForm_1.UnProcessUserNames));
            }

            // 8. 第三次处理（肖经理-Submit：不同意-退回）流程实例（wfInstance_8_2）
            // 流转后的流程图：Start:node1->Task:node2->Condition:node5->Task:node3(current)->Condition:node6->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 设置下一节点的审批人：总经理：齐总经理、销售经理：齐副经理, 肖经理
                var exceptExecuteUserIds = WorkflowFixture.wfComAdminUserId + ", " + WorkflowFixture.wfComSubAdminUserId + ", " + WorkflowFixture.wfSaleAdminUserId;
                var exceptExecuteUserNames = WorkflowFixture.wfComAdminUserName + ", " + WorkflowFixture.wfComSubAdminUserName + ", " + WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm_1.AllUserIds = exceptExecuteUserIds;
                wfSubmitForm_1.AllUserNames = exceptExecuteUserNames;
                wfSubmitForm_1.UnProcessUserIds = exceptExecuteUserIds;
                wfSubmitForm_1.UnProcessUserNames = exceptExecuteUserNames;
                wfSubmitForm_1.ExecuteUserId = WorkflowFixture.wfSaleAdminUserId;
                wfSubmitForm_1.ExecuteUserName = WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm_1.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm_1.ExecuteStatus = WorkflowExecuteStatus.Return;
                wfSubmitForm_1.ExecuteRemark = "用户：【" + wfSubmitForm_1.ExecuteUserName + "】执行：不同意-退回";

                _logger.LogInformation("实例【wfInstance_8_2】--8. 流程流转：第三次处理（肖经理-Submit：不同意-退回）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_8_2, wfSubmitForm_1);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【退回任务后，当前任务为：node3-task】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_8_2);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_8_2】--8. 流程流转后，获取流程可执行任务流程图" + message);
                Assert.NotNull(currentTask);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowFixture.node3_task.Name, currentTask.Name);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                Assert.Equal(wfSubmitForm_1.AllUserIds, currentTask.AllUserIds);
                Assert.Equal(WorkflowFixture.wfSaleAdminUserId, currentTask.DisagreeUserIds);
                Assert.Equal(WorkflowFixture.wfComAdminUserId + ", " + WorkflowFixture.wfComSubAdminUserId, currentTask.UnProcessUserIds);
                Assert.NotNull(currentTask.NextNode);
                Assert.Equal(WorkflowNodeType.Condition, currentTask.NextNode.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 9. 第四次处理（齐副经理-Submit：同意）流程实例（wfInstance_8_2）
            //流转后的流程图：Start:node1->Task:node2->Condition:node5->Task:node3(current)->Condition:node6->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm_1.ExecuteUserId = WorkflowFixture.wfComSubAdminUserId;
                wfSubmitForm_1.ExecuteUserName = WorkflowFixture.wfComSubAdminUserName;
                wfSubmitForm_1.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm_1.ExecuteStatus = WorkflowExecuteStatus.Approve;
                wfSubmitForm_1.ExecuteRemark = "用户：【" + wfSubmitForm_1.ExecuteUserName + "】执行：同意提交";

                _logger.LogInformation("实例【wfInstance_8_2】--9. 流程流转：第四次处理（齐副经理-Submit：同意）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_8_2, wfSubmitForm_1);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【流程流转后，当前任务为：node3-task】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_8_2);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_8_2】--9. 流程流转后，获取流程可执行任务流程图：" + message);
                Assert.NotNull(currentTask);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowFixture.node3_task.Name, currentTask.Name);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                Assert.Equal(wfSubmitForm_1.AllUserIds, currentTask.AllUserIds);
                Assert.Equal(WorkflowFixture.wfComSubAdminUserId, currentTask.AgreeUserIds);
                Assert.Equal(WorkflowFixture.wfComAdminUserId, currentTask.UnProcessUserIds);
                Assert.NotNull(currentTask.NextNode);
                Assert.Equal(WorkflowNodeType.Condition, currentTask.NextNode.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 10. 第五次处理（齐总经理-Submit：同意）流程实例（wfInstance_8_2）
            //            node6-执行条件【field3>=40 && field5==true】
            //                   条件为true，下一流程：Task:node2（满足）
            //                   条件为false，流程结束：End:node9
            //流转后的流程图：Start:node1->Task:node2(current)->Condition:node5->Task:node3->Condition:node6->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm_1.ExecuteUserId = WorkflowFixture.wfComAdminUserId;
                wfSubmitForm_1.ExecuteUserName = WorkflowFixture.wfComAdminUserName;
                wfSubmitForm_1.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm_1.ExecuteStatus = WorkflowExecuteStatus.Approve;
                wfSubmitForm_1.ExecuteRemark = "用户：【" + wfSubmitForm_1.ExecuteUserName + "】执行：同意提交";

                _logger.LogInformation("实例【wfInstance_8_2】--10. 流程流转：第五次处理（齐总经理-Submit：同意）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_8_2, wfSubmitForm_1);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【流转任务后流程已结束，当前任务为空】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_8_2);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_8_2】--10. 流程流转后，获取流程可执行任务流程图：" + message);
                Assert.NotNull(currentTask); 
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowFixture.node2_task.Name, currentTask.Name);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                Assert.Equal(wfStartForm.AllUserNames, currentTask.AllUserNames);
                Assert.NotNull(currentTask.NextNode);
                Assert.Equal(WorkflowNodeType.Condition, currentTask.NextNode.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }
        }

        /// <summary>
        /// 复杂条件前置回退串联审批：wfInstance_9 </br>
        ///     流程定义-9：生成1个条件前置的复杂流程实例，启动后由两个条件及任务节点（Condition:node5【field3>=20】、Task:node2）及（Condition:node6【field3>=40 && field5==true】、Task:node3）组成
        ///     执行人设置：
        ///         Task:node2【CreatorManager：发起人的主管--销售经理：齐副经理, 肖经理】
        ///         Task:node3【Executor：组织/角色/用户--组织:【企业, 销售部】角色:【总经理, 销售经理】用户:【肖经理】】
        ///     设置表单值：【设置表单字段【field3】的实际值为：23；【field5】的实际值为：true】
        ///     条件处理过程：node5-执行条件【field3>=20】
        ///                         条件为true，下一流程：Task:node2（满足）
        ///                         条件为false，流程结束：End:node9
        ///                  node6-执行条件【field3>=40 && field5==true】
        ///                         条件为true，下一流程：Task:node3
        ///                         条件为false，流程结束：Condtion:node5（满足）
        ///     启动后的流程图：Start:node1->Condtion:node5->Task:node2(current)->Condtion:node6->Task:node3->End:node9 
        ///     流转后的流程图：Start:node1->Condtion:node5->Task:node2->Condtion:node6->Task:node3->End:node9
        /// </summary>
        [Xunit.Fact]
        public async Task Test_WfProcessService_Instance_9()
        {
            // 1. 启动流程前，获取下一任务节点【node2-task】的审批人设置下的审批数据
            //      设置表单字段【field3】的实际值为：23；【field5】的实际值为：true
            WorkflowStartExecuteData wfStartForm;
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 设置表单字段【field3】的实际值为：23，【field5】的实际值为：true
                // 满足执行条件【field3>=20】为：true【field3>=40 && field5=true】为：false
                WorkflowFixture.field3_currency.Value = "23";
                WorkflowFixture.field5_bool.Value = "true";

                //获取下一任务节点【node2-task】的审批人设置下的审批数据
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                wfStartForm = await wfService.GetStartWorkflowExecutorInfoAsync(WorkflowFixture.WorkflowDef_9.Code, WorkflowFixture.wfProcessForm, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName);
                _logger.LogInformation(string.Format("实例【wfInstance_9】--1. 预期的执行人：【CreatorManager：发起人(小肖)的主管--销售经理：齐副经理, 肖经理】"));
                _logger.LogInformation(string.Format("实例【wfInstance_9】--1. 获取启动后下一{0}节点【{1}】的审批人设置下的审批数据：同意: 【{2}】不同意: 【{3}】未处理: 【{4}】", wfStartForm.TaskType, wfStartForm.TaskName, wfStartForm.AgreeUserNames, wfStartForm.DisagreeUserNames, wfStartForm.UnProcessUserNames));

                Assert.NotNull(wfStartForm);
                Assert.Equal(WorkflowNodeType.Task, wfStartForm.TaskType);
                Assert.Equal(WorkflowFixture.node2_task.Name, wfStartForm.TaskName);
                // 默认预期的执行人：【CreatorManager：发起人(小肖)的主管--销售经理：齐副经理, 肖经理】
                Assert.Equal(WorkflowFixture.wfComSubAdminUserId + ", " + WorkflowFixture.wfSaleAdminUserId, wfStartForm.AllUserIds);
            }

            // 2. 创建流程实例：wfInstance_9：
            // 条件处理过程：node5-执行条件【field3>=20】
            //                    条件为-true，下一流程：Task:node2 （满足）
            //                    条件为-false，流程结束：End:node9
            // 启动后的流程图：Start:node1->Condition:node5->Task:node2(current)->Condition:node6->Task:node3->End:node9 
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 设置下一节点的审批人：齐总经理、肖经理
                var exceptExecuteUserIds = WorkflowFixture.wfComAdminUserId + ", " + WorkflowFixture.wfSaleAdminUserId;
                var exceptExecuteUserNames = WorkflowFixture.wfComAdminUserName + ", " + WorkflowFixture.wfSaleAdminUserName;
                wfStartForm.AllUserIds = exceptExecuteUserIds;
                wfStartForm.AllUserNames = exceptExecuteUserNames;
                wfStartForm.UnProcessUserIds = exceptExecuteUserIds;
                wfStartForm.UnProcessUserNames = exceptExecuteUserNames;

                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                WorkflowFixture.wfInstance_9 = wfService.StartWorkflowAsync(wfStartForm, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName).Result;
                Assert.NotNull(WorkflowFixture.wfInstance_9);
                _logger.LogInformation("实例【wfInstance_9】--2. 生成流程实例wfInstance_9：" + WorkflowFixture.wfInstance_9);
            }

            // 3. 获取流程实例：wfInstance_9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 启动后的流程图：Start:node1->Condition:node5->Task:node2(current)->Condition:node6->Task:node3->End:node9  
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();

                #region 根据任务Code获取开始任务（GetStartTaskByProcessCodeAsync）
                var task = await wfService.GetStartTaskByProcessCodeAsync(WorkflowFixture.wfInstance_9);
                var message = wfService.PrintProcessLinkedTask(task);
                _logger.LogInformation("实例【wfInstance_9】--3. 流程实例：wfInstance_9，开始任务：" + message);
                Assert.NotNull(task);
                Assert.Equal(WorkflowNodeType.Start, task.NodeType);
                Assert.Equal(WorkflowTaskStatus.Finished, task.TaskStatus);
                var nextLevel0Condition = task.NextNode;
                Assert.NotNull(nextLevel0Condition);
                Assert.Equal(WorkflowNodeType.Condition, nextLevel0Condition.NodeType);
                Assert.Equal(WorkflowTaskStatus.Finished, nextLevel0Condition.TaskStatus);
                var nextLevel0Task = nextLevel0Condition.NextNode;
                Assert.NotNull(nextLevel0Task);
                Assert.Equal(WorkflowNodeType.Task, nextLevel0Task.NodeType);
                Assert.Equal(WorkflowFixture.node2_task.Name, nextLevel0Task.Name);
                Assert.Equal(WorkflowTaskStatus.Process, nextLevel0Task.TaskStatus);
                Assert.Equal(wfStartForm.AllUserNames, nextLevel0Task.AllUserNames);
                var nextLevel1Condition = nextLevel0Task.NextNode;
                Assert.NotNull(nextLevel1Condition);
                Assert.Equal(WorkflowNodeType.Condition, nextLevel1Condition.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, nextLevel1Condition.TaskStatus);
                var nextLevel1Task = nextLevel1Condition.NextNode;
                Assert.NotNull(nextLevel1Task);
                Assert.Equal(WorkflowNodeType.Task, nextLevel1Task.NodeType);
                Assert.Equal(WorkflowFixture.node3_task.Name, nextLevel1Task.Name);
                Assert.Equal(WorkflowTaskStatus.UnProcess, nextLevel1Task.TaskStatus);
                var nextLevel2End = nextLevel1Task.NextNode;
                Assert.NotNull(nextLevel2End);
                Assert.Equal(WorkflowNodeType.End, nextLevel2End.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, nextLevel2End.TaskStatus);
                #endregion

                #region 根据任务Code获取当前执行的任务【启动任务后，当前任务为：node2-task】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_9);
                Assert.NotNull(currentTask);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowFixture.node2_task.Name, currentTask.Name);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                Assert.Equal(wfStartForm.AllUserNames, currentTask.AllUserNames);
                Assert.NotNull(currentTask.NextNode);
                Assert.Equal(WorkflowNodeType.Condition, currentTask.NextNode.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 4. 流程流转前，获取下一任务节点【node2-task】的审批设置下的审批数据
            WorkflowExecuteData wfSubmitForm;
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                //获取下一任务节点【node2-task】的审批人设置下的审批数据
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                wfSubmitForm = await wfService.GetSubmitWorkflowExecutorInfoAsync(WorkflowFixture.wfInstance_9, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName);
                _logger.LogInformation(string.Format("实例【wfInstance_9】--4. 预期的执行人：【CreatorManager：发起人的主管--销售经理：齐副经理, 肖经理】"));
                _logger.LogInformation(string.Format("实例【wfInstance_9】--4. 获取流转前的下一{0}节点【{1}】的审批人设置下的审批数据：同意: 【{2}】不同意: 【{3}】未处理: 【{4}】", wfSubmitForm.TaskType, wfSubmitForm.TaskName, wfSubmitForm.AgreeUserNames, wfSubmitForm.DisagreeUserNames, wfSubmitForm.UnProcessUserNames));

                Assert.NotNull(wfSubmitForm);
                Assert.Equal(WorkflowNodeType.Task, wfSubmitForm.TaskType);
                Assert.Equal(WorkflowFixture.node2_task.Name, wfSubmitForm.TaskName);
                // 执行人设置：【CreatorManager：发起人的主管--销售经理:齐副经理, 肖经理】
                // 预期的执行人【齐副经理,肖经理】
                Assert.Equal(wfStartForm.AllUserIds, wfSubmitForm.AllUserIds);
            }

            // 5. 第一次处理（肖经理-Submit：不同意-退回）流程实例（wfInstance_9）
            // 流转后的流程图：Start:node1->Condition:node5->Task:node2(current)->Condition:node6->Task:node3->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm.ExecuteUserId = WorkflowFixture.wfSaleAdminUserId;
                wfSubmitForm.ExecuteUserName = WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm.ExecuteStatus = WorkflowExecuteStatus.Return;
                wfSubmitForm.ExecuteRemark = "用户：【" + wfSubmitForm.ExecuteUserName + "】执行：不同意-退回";

                _logger.LogInformation("实例【wfInstance_9】--5. 流程流转：第一次处理（肖经理-Submit：不同意-退回）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_9, wfSubmitForm);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【退回任务后，将任务退回至发起人，当前任务为空】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_9);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_9】--5. 流程流转后，获取流程可执行任务流程图" + message);
                Assert.Null(currentTask);
                //Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                //Assert.Equal(WorkflowFixture.node2_task.Name, currentTask.Name);
                //Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                //Assert.Equal(wfSubmitForm.AllUserNames, currentTask.AllUserNames);
                //Assert.NotNull(currentTask.NextNode);
                //Assert.Equal(WorkflowNodeType.Condition, currentTask.NextNode.NodeType);
                //Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 6. 第二次处理（肖经理-Submit：同意）流程实例（wfInstance_9）
            //            node6-执行条件【field3>=40 && field5==true】
            //                   条件为true，下一流程：Task:node2
            //                   条件为false，流程结束：Condtion:node5（满足）
            // 流转后的流程图：Start:node1->Condition:node5->Task:node2(current)->Condition:node6->Task:node3->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm.ExecuteUserId = WorkflowFixture.wfSaleAdminUserId;
                wfSubmitForm.ExecuteUserName = WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm.ExecuteStatus = WorkflowExecuteStatus.Approve;
                wfSubmitForm.ExecuteRemark = "用户：【" + wfSubmitForm.ExecuteUserName + "】执行：同意提交";

                _logger.LogInformation("实例【wfInstance_9】--6. 流程流转：第二次处理（肖经理-Submit：同意）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_9, wfSubmitForm);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【流程流转后，当前任务为：node2-task】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_9);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_9】--6. 流程流转后，获取流程可执行任务流程图：" + message);
                Assert.NotNull(currentTask);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowFixture.node2_task.Name, currentTask.Name);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                Assert.NotNull(currentTask.NextNode);
                Assert.Equal(WorkflowNodeType.Condition, currentTask.NextNode.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 7. 流程流转前，获取下一任务节点【node2-task】的审批设置下的审批数据
            WorkflowExecuteData wfSubmitForm_1;
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                //获取下一任务节点【node2-task】的审批人设置下的审批数据
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                wfSubmitForm_1 = await wfService.GetSubmitWorkflowExecutorInfoAsync(WorkflowFixture.wfInstance_9, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName);
                _logger.LogInformation(string.Format("实例【wfInstance_9】--4. 预期的执行人：【CreatorManager：发起人的主管--销售经理：齐副经理, 肖经理】"));
                _logger.LogInformation(string.Format("实例【wfInstance_9】--4. 获取流转前的下一{0}节点【{1}】的审批人设置下的审批数据：同意: 【{2}】不同意: 【{3}】未处理: 【{4}】", wfSubmitForm_1.TaskType, wfSubmitForm_1.TaskName, wfSubmitForm_1.AgreeUserNames, wfSubmitForm_1.DisagreeUserNames, wfSubmitForm_1.UnProcessUserNames));

                Assert.NotNull(wfSubmitForm_1);
                Assert.Equal(WorkflowNodeType.Task, wfSubmitForm_1.TaskType);
                Assert.Equal(WorkflowFixture.node2_task.Name, wfSubmitForm_1.TaskName);
                // 默认预期的执行人：【CreatorManager：发起人(小肖)的主管--销售经理：齐副经理, 肖经理】
                Assert.Equal(WorkflowFixture.wfComSubAdminUserId + ", " + WorkflowFixture.wfSaleAdminUserId, wfSubmitForm_1.AllUserIds);

            }

            // 8. 第三次处理（肖经理-Submit：不同意-退回）流程实例（wfInstance_9），更改字段：field3的值为：43
            // 流转后的流程图：(current)->Start:node1->Condition:node5->Task:node2->Condition:node6->Task:node3->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm_1.ExecuteUserId = WorkflowFixture.wfSaleAdminUserId;
                wfSubmitForm_1.ExecuteUserName = WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm_1.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm_1.ExecuteStatus = WorkflowExecuteStatus.Return;
                wfSubmitForm_1.ExecuteRemark = "用户：【" + wfSubmitForm_1.ExecuteUserName + "】执行：不同意-退回";

                // 更新表单字段：field3_currency的字段值，以便流程正常进行
                if (wfSubmitForm_1.FormData != null)
                    wfSubmitForm_1.FormData.ForEach(m => {
                        if (m.Text.Equals(WorkflowFixture.field3_currency.Text))
                        {
                            m.Value = "43";
                        }
                    });

                _logger.LogInformation("实例【wfInstance_9】--8. 流程流转：第三次处理（肖经理-Submit：不同意-退回）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_9, wfSubmitForm_1);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【退回任务后，将任务退回至发起人，当前任务为空】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_9);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_9】--8. 流程流转后，获取流程可执行任务流程图" + message);
                Assert.Null(currentTask);
                //Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                //Assert.Equal(WorkflowFixture.node2_task.Name, currentTask.Name);
                //Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                //Assert.Equal(wfSubmitForm_1.AllUserNames, currentTask.AllUserNames);
                //Assert.NotNull(currentTask.NextNode);
                //Assert.Equal(WorkflowNodeType.Condition, currentTask.NextNode.NodeType);
                //Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 9. 第四次处理（肖经理-Submit：同意）流程实例（wfInstance_9）
            //            node6-执行条件【field3>=40 && field5==true】
            //                   条件为true，下一流程：Task:node3（满足）
            //                   条件为false，流程结束：Condtion:node5
            // 流转后的流程图：Start:node1->Condition:node5->Task:node2->Condition:node6->Task:node3(current)->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm_1.ExecuteUserId = WorkflowFixture.wfSaleAdminUserId;
                wfSubmitForm_1.ExecuteUserName = WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm_1.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm_1.ExecuteStatus = WorkflowExecuteStatus.Approve;
                wfSubmitForm_1.ExecuteRemark = "用户：【" + wfSubmitForm_1.ExecuteUserName + "】执行：同意提交";

                _logger.LogInformation("实例【wfInstance_9】--9. 流程流转：第四次处理（肖经理-Submit：同意）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_9, wfSubmitForm_1);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【流程流转后，当前任务为：node2-task】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_9);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_9】--9. 流程流转后，获取流程可执行任务流程图：" + message);
                Assert.NotNull(currentTask);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowFixture.node3_task.Name, currentTask.Name);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                #endregion
            }

            // 10. 流程流转前，获取下一任务节点【node3-task】的审批设置下的审批数据
            WorkflowExecuteData wfSubmitForm_2;
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                //获取下一任务节点【node3-task】的审批人设置下的审批数据
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                wfSubmitForm_2 = await wfService.GetSubmitWorkflowExecutorInfoAsync(WorkflowFixture.wfInstance_9, WorkflowFixture.wfSaleTestUserId, WorkflowFixture.wfSaleTestUserName);
                _logger.LogInformation(string.Format("实例【wfInstance_9】--10. 预期的执行人：【齐总经理,小齐,齐副经理,肖经理,小肖】 "));
                //node3-task--org: 企业,销售部; role: 总经理,销售经理; user: 肖经理
                _logger.LogInformation(string.Format("实例【wfInstance_9】--10. 获取流转前的下一{0}节点【{1}】的审批人设置下的审批数据：同意: 【{2}】不同意: 【{3}】未处理: 【{4}】", wfSubmitForm_2.TaskType, wfSubmitForm_2.TaskName, wfSubmitForm_2.AgreeUserNames, wfSubmitForm_2.DisagreeUserNames, wfSubmitForm_2.UnProcessUserNames));

                Assert.NotNull(wfSubmitForm_2);
                Assert.Equal(WorkflowNodeType.Task, wfSubmitForm_2.TaskType);
                Assert.Equal(WorkflowFixture.node3_task.Name, wfSubmitForm_2.TaskName);
                // 执行人【Executor：组织/角色/用户--组织:【企业, 销售部】角色:【总经理, 销售经理】用户:【肖经理】】
                // 预期的执行人【齐总经理,小齐,齐副经理,肖经理,小肖】 
                // Assert.Equal(wfStartForm.AllUserNames, wfSubmitForm_2.AllUserNames);
            }

            // 11. 第五次处理（肖经理-Submit：不同意-退回）流程实例（wfInstance_9）
            // 流转后的流程图：Start:node1->Condition:node5->Task:node2->Condition:node6->Task:node3(current)->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                // 设置下一节点的审批人：总经理：齐总经理、销售经理：齐副经理, 肖经理
                var exceptExecuteUserIds = WorkflowFixture.wfComAdminUserId + ", " + WorkflowFixture.wfComSubAdminUserId + ", " + WorkflowFixture.wfSaleAdminUserId;
                var exceptExecuteUserNames = WorkflowFixture.wfComAdminUserName + ", " + WorkflowFixture.wfComSubAdminUserName + ", " + WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm_2.AllUserIds = exceptExecuteUserIds;
                wfSubmitForm_2.AllUserNames = exceptExecuteUserNames;
                wfSubmitForm_2.UnProcessUserIds = exceptExecuteUserIds;
                wfSubmitForm_2.UnProcessUserNames = exceptExecuteUserNames;
                wfSubmitForm_2.ExecuteUserId = WorkflowFixture.wfSaleAdminUserId;
                wfSubmitForm_2.ExecuteUserName = WorkflowFixture.wfSaleAdminUserName;
                wfSubmitForm_2.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm_2.ExecuteStatus = WorkflowExecuteStatus.Return;
                wfSubmitForm_2.ExecuteRemark = "用户：【" + wfSubmitForm_2.ExecuteUserName + "】执行：不同意-退回";

                _logger.LogInformation("实例【wfInstance_9】--11. 流程流转：第五次处理（肖经理-Submit：不同意-退回）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_9, wfSubmitForm_2);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【退回任务后，当前任务为：node3-task】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_9);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_9】--11. 流程流转后，获取流程可执行任务流程图" + message);
                Assert.NotNull(currentTask);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowFixture.node3_task.Name, currentTask.Name);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                Assert.Equal(wfSubmitForm_2.AllUserIds, currentTask.AllUserIds);
                Assert.Equal(WorkflowFixture.wfSaleAdminUserId, currentTask.DisagreeUserIds);
                Assert.Equal(WorkflowFixture.wfComAdminUserId + ", " + WorkflowFixture.wfComSubAdminUserId, currentTask.UnProcessUserIds);
                Assert.NotNull(currentTask.NextNode);
                Assert.Equal(WorkflowNodeType.End, currentTask.NextNode.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 12. 第六次处理（齐副经理-Submit：同意）流程实例（wfInstance_9）
            //流转后的流程图：Start:node1->Condition:node5->Task:node2->Condition:node6->Task:node3(current)->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm_2.ExecuteUserId = WorkflowFixture.wfComSubAdminUserId;
                wfSubmitForm_2.ExecuteUserName = WorkflowFixture.wfComSubAdminUserName;
                wfSubmitForm_2.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm_2.ExecuteStatus = WorkflowExecuteStatus.Approve;
                wfSubmitForm_2.ExecuteRemark = "用户：【" + wfSubmitForm_2.ExecuteUserName + "】执行：同意提交";

                _logger.LogInformation("实例【wfInstance_9】--12. 流程流转：第六次处理（齐副经理-Submit：同意）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_9, wfSubmitForm_2);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【流程流转后，当前任务为：node3-task】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_9);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_9】--12. 流程流转后，获取流程可执行任务流程图：" + message);
                Assert.NotNull(currentTask);
                Assert.Equal(WorkflowNodeType.Task, currentTask.NodeType);
                Assert.Equal(WorkflowFixture.node3_task.Name, currentTask.Name);
                Assert.Equal(WorkflowTaskStatus.Process, currentTask.TaskStatus);
                Assert.Equal(wfSubmitForm_2.AllUserIds, currentTask.AllUserIds);
                Assert.Equal(WorkflowFixture.wfComSubAdminUserId, currentTask.AgreeUserIds);
                Assert.Equal(WorkflowFixture.wfComAdminUserId, currentTask.UnProcessUserIds);
                Assert.NotNull(currentTask.NextNode);
                Assert.Equal(WorkflowNodeType.End, currentTask.NextNode.NodeType);
                Assert.Equal(WorkflowTaskStatus.UnProcess, currentTask.NextNode.TaskStatus);
                #endregion
            }

            // 13. 第七次处理（齐总经理-Submit：同意）流程实例（wfInstance_9）
            //流转后的流程图：Start:node1->Condition:node5->Task:node2->Condition:node6->Task:node3->End:node9
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                wfSubmitForm_2.ExecuteUserId = WorkflowFixture.wfComAdminUserId;
                wfSubmitForm_2.ExecuteUserName = WorkflowFixture.wfComAdminUserName;
                wfSubmitForm_2.ExecuteDateTime = DateTime.UtcNow;
                wfSubmitForm_2.ExecuteStatus = WorkflowExecuteStatus.Approve;
                wfSubmitForm_2.ExecuteRemark = "用户：【" + wfSubmitForm_2.ExecuteUserName + "】执行：同意提交";

                _logger.LogInformation("实例【wfInstance_9】--13. 流程流转：第七次处理（齐总经理-Submit：同意）流程实例");
                var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                var success = await wfService.SubmitWorkflowAsync(WorkflowFixture.wfInstance_9, wfSubmitForm_2);
                Assert.True(success);

                #region 根据任务Code获取当前执行的任务【流转任务后流程已结束，当前任务为空】
                var currentTask = await wfService.GetCurrentTaskByProcessCodeAsync(WorkflowFixture.wfInstance_9);
                var message = wfService.PrintProcessLinkedTask(currentTask);
                _logger.LogInformation("实例【wfInstance_9】--13. 流程流转后，获取流程可执行任务流程图：" + message);
                Assert.Null(currentTask);
                #endregion
            }
        }

        protected override void TearDown()
        {

        }
    }
}
