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
    /// 保存流程（SaveWorkflowDefinitionWithFieldsAndNodes）
    /// </summary>
    public class WorkFlowDefServiceTest : TestBase<WorkflowFixture>
    {
        private ILogger _logger;
        public WorkFlowDefServiceTest(WorkflowFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(WorkflowServiceTest));
        }

        protected override void SetUp()
        {

        }

        /// <summary>
        /// 测试修改流程信息后，是否可用保存流程信息及生成版本信息（实际使用：分两步保存流程数据）
        /// </summary>
        [Xunit.Fact]
        public async Task Test_WorkflowDefinitionService_SaveWorkflowDefinition()
        {
            //测试修改流程基础数据、表单数据后，保存流程
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                var wfDefinition = WorkflowFixture.WorkflowDef_1;
                //编辑数据，名称添加标识（版本二），移除表单中第三个Field字段
                wfDefinition.IsEditMode = true;
                wfDefinition.Version = "wfv2012020200002-1";
                wfDefinition.Name = "流程定义-1-测试（版本二）";
                wfDefinition.Description = "流程定义-1-测试（版本二）";
                //流程表单
                foreach (var field in wfDefinition.WorkflowFields)
                {
                    field.Text = field.Text + "（版本二）";
                    field.DisplayName = field.DisplayName + "（版本二）";
                    field.Description = field.Description + "（版本二）";
                    if (field.Children.Any())
                    {
                        foreach (var cField in field.Children)
                        {
                            cField.Text = cField.Text + "（版本二）";
                            cField.DisplayName = cField.DisplayName + "（版本二）";
                            cField.Description = cField.Description + "（版本二）";
                        }

                        field.Children.RemoveAt(2);
                    }
                }
                wfDefinition.WorkflowFields.RemoveAt(2);
                wfDefinition.WorkflowNodes.Clear();//只提供流程表单数据，流程节点数据无需提供；

                var wfService = scope.ServiceProvider.GetService<IWorkflowDefinitionService>();
                var result = await wfService.SaveWorkflowDefinitionWithFieldsAsync(wfDefinition);
                Assert.True(result);
                await JudgeDefinitionFieldsAndNodesAsync(wfDefinition.Id, wfDefinition);
            }

            //测试修改流程节点数据后，保存流程
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                var wfDefinition = WorkflowFixture.WorkflowDef_1;
                //流程节点
                foreach (var node in wfDefinition.WorkflowNodes)
                {
                    node.Name = node.Name + "（版本二）";
                }
                var wfService = scope.ServiceProvider.GetService<IWorkflowDefinitionService>();

                var result = await wfService.SaveWorkflowNodesAsync(wfDefinition.Id, wfDefinition.Name, wfDefinition.WorkflowNodes, WorkflowFixture.wfItAdminUserId, WorkflowFixture.wfItAdminUserName);
                Assert.True(result);
                await JudgeDefinitionNodeAsync(wfDefinition.Id, wfDefinition);
            }
        }

        /// <summary>
        /// 测试修改流程信息后，是否可用保存流程信息及生成版本信息（测试使用：一次保存所有流程数据）
        /// </summary>
        [Xunit.Fact]
        public async Task Test_WorkflowDefinitionService_SaveWorkflowDefinitionWithFieldsAndNodes()
        {
            var wfDefinition = WorkflowFixture.WorkflowDef_1;
            //流程定义-1
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                //编辑数据，名称添加标识（版本二），移除表单中第三个Field字段
                wfDefinition.IsEditMode = true;
                wfDefinition.Version = "wfv2012020200002-1";
                wfDefinition.Name = "流程定义-1-测试（版本二）";
                wfDefinition.Description = "流程定义-1-测试（版本二）";
                //流程表单
                foreach (var field in wfDefinition.WorkflowFields)
                {
                    field.Text = field.Text + "（版本二）";
                    field.DisplayName = field.DisplayName + "（版本二）";
                    field.Description = field.Description + "（版本二）";
                    if (field.Children.Any())
                    {
                        foreach (var cField in field.Children)
                        {
                            cField.Text = cField.Text + "（版本二）";
                            cField.DisplayName = cField.DisplayName + "（版本二）";
                            cField.Description = cField.Description + "（版本二）";
                        }

                        field.Children.RemoveAt(2);
                    }
                }
                wfDefinition.WorkflowFields.RemoveAt(2);
                //流程节点
                foreach (var node in wfDefinition.WorkflowNodes)
                {
                    node.Name = node.Name + "（版本二）";
                }
                var wfService = scope.ServiceProvider.GetService<IWorkflowDefinitionService>();

                var result = await wfService.SaveWorkflowDefinitionWithFieldsAndNodesAsync(wfDefinition);
                Assert.True(result);
                await JudgeDefinitionFieldsAndNodesAsync(wfDefinition.Id, wfDefinition);
            }

        }

        #region 流程数据判断
        /// <summary>
        /// 判断流程数据保存后，数据库中的数据和传入数据为一致的
        /// </summary>
        /// <param name="dbId">数据库流程定义Id</param>
        /// <param name="wfDefinition">需要保存的流程定义数据</param>
        private async Task JudgeDefinitionFieldsAndNodesAsync(Guid dbId, WorkflowDefinitionDTO wfDefinition)
        {
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                var wfService = scope.ServiceProvider.GetService<IWorkflowDefinitionService>();
                var dbWfDefinition = await wfService.GetWorkflowDefinitionDetailAsync(dbId);
                Assert.Equal(dbWfDefinition.Name, wfDefinition.Name);
                Assert.Equal(dbWfDefinition.Description, wfDefinition.Description);
                //流程表单
                foreach (var field in dbWfDefinition.WorkflowFields)
                {
                    var dbField = wfDefinition.WorkflowFields.FirstOrDefault(m => m.Text.Equals(field.Text) && m.DisplayName.Equals(field.DisplayName));
                    Assert.NotNull(dbField);
                    if (field.Children.Any())
                    {
                        var dbChildFields = dbField.Children;
                        foreach (var cField in field.Children)
                        {
                            var dbChildField = dbChildFields.FirstOrDefault(m => m.Text.Equals(cField.Text) && m.DisplayName.Equals(cField.DisplayName));
                            Assert.NotNull(dbChildField);
                        }
                    }
                }
                //流程节点
                foreach (var node in wfDefinition.WorkflowNodes)
                {
                    var dbNode = wfDefinition.WorkflowNodes.FirstOrDefault(m => m.Name.Equals(node.Name));
                    Assert.NotNull(dbNode);
                }
            }
        }
        /// <summary>
        /// 判断流程数据保存后，数据库中的数据和传入数据为一致的
        /// </summary>
        /// <param name="dbId">数据库流程定义Id</param>
        /// <param name="wfDefinition">需要保存的流程定义数据</param>
        private async Task JudgeDefinitionNodeAsync(Guid dbId, WorkflowDefinitionDTO wfDefinition)
        {
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                var wfService = scope.ServiceProvider.GetService<IWorkflowDefinitionService>();
                var dbWfDefinition = await wfService.GetWorkflowDefinitionDetailAsync(dbId);
                //流程节点
                foreach (var node in wfDefinition.WorkflowNodes)
                {
                    var dbNode = wfDefinition.WorkflowNodes.FirstOrDefault(m => m.Name.Equals(node.Name));
                    Assert.NotNull(dbNode);
                }
            }
        }
        #endregion

        protected override void TearDown()
        {

        }
    }
}
