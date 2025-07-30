using System;
using System.IO;
using System.Linq;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using KC.Service.Account;
using System.Collections.Generic;
using System.Threading.Tasks;
using KC.Service.DTO.Account;
using Microsoft.Extensions.Logging;

namespace KC.UnitTest.Account
{
    public class SysManageServiceTest : TestBase<UserFixture>
    {
        private ILogger _logger;
        protected IServiceProvider ServiceProvider;

        public SysManageServiceTest(UserFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(AccountServiceTest));
        }

        protected override void SetUp()
        {

        }

        [Xunit.Fact]
        public void Test_FindOrgTreesWithUsers()
        {
            InjectTenant(TestTenant);
            var testDbSysManangeService = Services.BuildServiceProvider().GetService<ISysManageService>();

            //获取“IT开发部”下所属部门下的所有员工
            var orgsWithUsers = testDbSysManangeService.FindOrgTreesWithUsers();
            _logger.LogInformation("获取所有的部门（包含所有上级及下级的部门树）及部门下所有员工");
            _logger.LogInformation("FindOrgTreesWithUsers 预期结果：一级组织：【企业, 注册企业】，人员：【齐总经理、小齐】；二级组织：【人事部, 采购部, 销售部, 仓储部, 生产部, 质检部, 财务部, 市场部, 运营部, 设计部, 行政部, IT部】、人员：【齐副经理、肖经理、小肖、蔡经理、小蔡、季经理、小季, 系统管理员】");
            _logger.LogInformation(string.Format("FindOrgTreesWithUsers 实际结果：一级组织：【{0}】，人员：【{1}】、二级组织：【{2}】，人员：【{3}】", 
                orgsWithUsers.ToCommaSeparatedStringByFilter(m => m.Text), 
                orgsWithUsers.SelectMany(m => m.Users).ToCommaSeparatedStringByFilter(m => m.DisplayName),
                orgsWithUsers.SelectMany(m => m.Children).ToCommaSeparatedStringByFilter(m => m.Text),
                orgsWithUsers.SelectMany(m => m.Children.SelectMany(o => o.Users)).ToCommaSeparatedStringByFilter(m => m.DisplayName)));
            Assert.True(orgsWithUsers.Any());
        }

        [Xunit.Fact]
        public void Test_FindOrgTreesWithUsersByOrgIds()
        {
            InjectTenant(TestTenant);
            var testDbSysManangeService = Services.BuildServiceProvider().GetService<ISysManageService>();

            //获取【部门：包含--销售部、采购部，排除--采购部】所属部门及部门下所有员工
            var depIds = new List<int>() { OrganizationConstants.销售部_Id, OrganizationConstants.采购部_Id };
            var exceptDepIds = new List<int>() { OrganizationConstants.采购部_Id };
            var orgsWithUsers = testDbSysManangeService.FindOrgTreesWithUsersByOrgIds(depIds, exceptDepIds);
            _logger.LogInformation("获取【部门：包含--销售部、采购部，排除--采购部】所属部门（包含所有上级及下级的部门树）及部门下所有员工");
            _logger.LogInformation("FindOrgTreesWithUsersByOrgIds 预期结果：一级组织：【企业】，人员：【齐总经理、小齐】；二级组织：【销售部】、人员：【齐副经理、肖经理、小肖】");
            _logger.LogInformation(string.Format("FindOrgTreesWithUsersByOrgIds 实际结果：一级组织：【{0}】，人员：【{1}】、二级组织：【{2}】，人员：【{3}】",
                orgsWithUsers.ToCommaSeparatedStringByFilter(m => m.Text),
                orgsWithUsers.SelectMany(m => m.Users).ToCommaSeparatedStringByFilter(m => m.DisplayName),
                orgsWithUsers.SelectMany(m => m.Children).ToCommaSeparatedStringByFilter(m => m.Text),
                orgsWithUsers.SelectMany(m => m.Children.SelectMany(o => o.Users)).ToCommaSeparatedStringByFilter(m => m.DisplayName)));
            Assert.True(orgsWithUsers.Any());
        }

        [Xunit.Fact]
        public void Test_FindOrganizationsWithUsersByUserId()
        {
            InjectTenant(TestTenant);
            var testDbSysManangeService = Services.BuildServiceProvider().GetService<ISysManageService>();

            //获取用户“小肖：部门：销售部；角色：销售助理”所属部门及部门下所有员工
            var orgsWithUsers = testDbSysManangeService.FindOrganizationsWithUsersByUserId(UserFixture.wfSaleTestUserId);
            _logger.LogInformation("获取用户【小肖：部门：销售部；角色：销售助理】所属部门及部门下所有员工");
            _logger.LogInformation("FindOrganizationsWithUsersByUserId 预期结果：销售部、齐副经理、肖经理、小肖");
            _logger.LogInformation(string.Format("FindOrganizationsWithUsersByUserId 预期结果：{0}、{1}", orgsWithUsers.ToCommaSeparatedStringByFilter(m => m.Text), orgsWithUsers.SelectMany(m => m.Users).ToCommaSeparatedStringByFilter(m => m.DisplayName)));
            Assert.True(orgsWithUsers.Any());
        }

        [Xunit.Fact]
        public void Test_FindHigherOrganizationsWithUsersByUserId()
        {
            InjectTenant(TestTenant);
            var testDbSysManangeService = Services.BuildServiceProvider().GetService<ISysManageService>();

            //获取用户“小肖：部门：销售部；角色：销售助理”所属上级部门及部门下所有员工
            var orgsWithUsers = testDbSysManangeService.FindHigherOrganizationsWithUsersByUserId(UserFixture.wfSaleTestUserId);
            _logger.LogInformation("获取用户【小肖：部门：销售部；角色：销售助理】所属上级部门及部门下所有员工");
            _logger.LogInformation("FindHigherOrganizationsWithUsersByUserId 预期结果：企业、齐总经理、小齐");
            _logger.LogInformation(string.Format("FindHigherOrganizationsWithUsersByUserId 实际结果：{0}、{1}", orgsWithUsers.ToCommaSeparatedStringByFilter(m => m.Text), orgsWithUsers.SelectMany(m => m.Users).ToCommaSeparatedStringByFilter(m => m.DisplayName)));
            Assert.True(orgsWithUsers.Any());
        }
        
        #region 测试TreeNode基类
        [Xunit.Fact]
        public void Test_GetAllTreeNodeWithNestChild()
        {
            InjectTenant(TestTenant);
            var dbaSysManageService = Services.BuildServiceProvider().GetService<ISysManageService>();

            var result = dbaSysManageService.Test_GetAllTreeNodeWithNestChild();
            _logger.LogInformation("GetAllTreeNodeWithNestChild：获取所有组织: " + result.ToCommaSeparatedStringByFilter(m => m.Text));
            Assert.True(result.Any());

            var child = result.SelectMany(m => m.Children);
            _logger.LogInformation("GetAllTreeNodeWithNestChild：获取所有组织下的子部门: " + child.ToCommaSeparatedStringByFilter(m => m.Text));
            Assert.True(child.Any());
        }

        [Xunit.Fact]
        public void Test_GetTreeNodeWithNestChildById()
        {
            InjectTenant(TestTenant);
            var dbaSysManageService = Services.BuildServiceProvider().GetService<ISysManageService>();

            var result = dbaSysManageService.Test_GetTreeNodeWithNestChildById(OrganizationConstants.企业_Id);
            _logger.LogInformation("GetTreeNodeWithNestChildById：获取企业: " + result.Text);
            _logger.LogInformation("GetTreeNodeWithNestChildById：获取企业下所有组织: " + result.Children.ToCommaSeparatedStringByFilter(m => m.Text));
            Assert.True(result != null);
            Assert.True(result.Children.Any());
        }

        [Xunit.Fact]
        public void Test_GetTreeNodesWithNestParentAndChildById()
        {
            InjectTenant(TestTenant);
            var dbaSysManageService = Services.BuildServiceProvider().GetService<ISysManageService>();

            var result = dbaSysManageService.Test_GetTreeNodesWithNestParentAndChildById(OrganizationConstants.销售部_Id);
            _logger.LogInformation("GetTreeNodesWithNestParentAndChildById：获取销售部上级组织: " + result.ToCommaSeparatedStringByFilter(m => m.Text));
            Assert.True(result.Any());

            var child = result.SelectMany(m => m.Children);
            _logger.LogInformation("GetTreeNodesWithNestParentAndChildById：获取销售部上级组织下的子部门: " + child.ToCommaSeparatedStringByFilter(m => m.Text));
            Assert.True(child.Any());
        }

        [Xunit.Fact]
        public void Test_GetTreeNodesWithNestParentAndChildByFilter()
        {
            InjectTenant(TestTenant);
            var dbaSysManageService = Services.BuildServiceProvider().GetService<ISysManageService>();

            var result = dbaSysManageService.Test_GetTreeNodesWithNestParentAndChildByFilter("务");
            _logger.LogInformation("GetTreeNodesWithNestParentAndChildByFilter：获取带【务】字的组织: " + result.ToCommaSeparatedStringByFilter(m => m.Text));
            Assert.True(result.Any());

            var child = result.SelectMany(m => m.Children);
            _logger.LogInformation("GetTreeNodesWithNestParentAndChildByFilter：获取带【务】字的组织的子部门: " + child.ToCommaSeparatedStringByFilter(m => m.Text));
            Assert.True(child.Any());
        }

        [Xunit.Fact(Skip = "true")]
        public void Test_Test_UpdateExtendFields()
        {
            InjectTenant(TestTenant);
            var dbaSysManageService = Services.BuildServiceProvider().GetService<ISysManageService>();

            var result = dbaSysManageService.Test_UpdateExtendFields();
            Assert.True(result);
        }
        #endregion

        protected override void TearDown()
        {

        }
    }
}
