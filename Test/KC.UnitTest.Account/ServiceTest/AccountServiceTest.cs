using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Model.Account;
using KC.Service.Account;
using KC.Service.DTO.Search;
using KC.Service.WebApiService.Business;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace KC.UnitTest.Account
{
    public class AccountServiceTest : TestBase<UserFixture>
    {
        private ILogger _logger;
        protected IServiceProvider ServiceProvider;

        public AccountServiceTest(UserFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(AccountServiceTest));
        }

        protected override void SetUp()
        {
            
        }

        [Xunit.Fact]
        public void Test_FindUserManagersByUserId()
        {
            InjectTenant(DbaTenant);
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                var testAccountService = scope.ServiceProvider.GetService<IAccountService>();
                //获取用户“小肖：部门：销售部；角色：销售助理”的主管信息
                var users = testAccountService.FindUserManagersByUserId(UserFixture.wfSaleTestUserId);
                _logger.LogInformation("获取用户【Test-3：小肖：部门：销售部；角色：销售助理】的主管信息");
                _logger.LogInformation("FindUserManagersByUserId 预期结果：齐副经理、肖经理");
                _logger.LogInformation("FindUserManagersByUserId 实际结果：" + users.ToCommaSeparatedStringByFilter(m => m.DisplayName));
                Assert.True(users.Any());
            }
        }

        [Xunit.Fact]
        public void Test_FindUserSupervisorsByUserId()
        {
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                var testAccountService = scope.ServiceProvider.GetService<IAccountService>();
                //获取用户“小肖：部门：销售部；角色：销售助理”的上级主管信息
                var users = testAccountService.FindUserSupervisorsByUserId(UserFixture.wfSaleTestUserId);
                _logger.LogInformation("获取用户【小肖：部门：销售部；角色：销售助理】的上级主管信息");
                _logger.LogInformation("FindUserSupervisorsByUserId 预期结果：齐总经理");
                _logger.LogInformation("FindUserSupervisorsByUserId 实际结果：" + users.ToCommaSeparatedStringByFilter(m => m.DisplayName));
                Assert.True(users.Any());
            }
        }

        [Xunit.Fact]
        public void Test_FindUsersByUserIds()
        {
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                var testAccountService = scope.ServiceProvider.GetService<IAccountService>();
                //获取用户（小肖、小蔡）下的所有员工：小肖、小蔡
                var searchIds = new List<string>() { UserFixture.wfSaleTestUserId, UserFixture.wfBuyTestUserId };
                var users = testAccountService.FindUsersByUserIds(searchIds);
                _logger.LogInformation("获取用户（小肖、小蔡）下的所有员工");
                _logger.LogInformation("FindUsersByUserIds 预期结果：小肖、小蔡");
                _logger.LogInformation("FindUsersByUserIds 实际结果：" + users.ToCommaSeparatedStringByFilter(m => m.DisplayName));
                Assert.True(users.Any());
            }
        }

        [Xunit.Fact]
        public void Test_FindUsersByOrgIds()
        {
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                var testAccountService = scope.ServiceProvider.GetService<IAccountService>();
                //获取“销售部、IT部”下所属部门下的所有员工
                var searchIds = new List<int>() { UserFixture.wfSaleDeptId, UserFixture.wfBuyDeptId };
                var users = testAccountService.FindUsersByOrgIds(searchIds);
                _logger.LogInformation("获取部门（销售部、采购部）下的所有员工");
                _logger.LogInformation("FindUsersByOrgIds 预期结果：齐副经理、肖经理、小肖、蔡经理、小蔡");
                _logger.LogInformation("FindUsersByOrgIds 实际结果：" + users.ToCommaSeparatedStringByFilter(m => m.DisplayName));
                Assert.True(users.Any());
            }
        }


        [Xunit.Fact]
        public async Task Test_FindUsersByDataPermissionFilterAsync()
        {
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                var testAccountService = scope.ServiceProvider.GetService<IAccountService>();
                var searchIds = new List<string>() { UserFixture.wfSaleTestUserId, UserFixture.wfBuyTestUserId };
                DataPermissionSearchDTO searchModel = new DataPermissionSearchDTO();
                searchModel.UserIds = searchIds;//获取用户（小肖、小蔡）下的所有员工
                searchModel.RoleIds = UserFixture.wfItAdminUser_RoleIds;//获取角色（系统管理员）下的所有员工
                searchModel.OrgCodes = UserFixture.wfBuyAdminUser_OrgCodes;//获取部门（采购部）下的所有员工
                var users = await testAccountService.FindUsersByDataPermissionFilterAsync(searchModel);
                _logger.LogInformation("部门（采购部）角色（系统管理员）用户（小肖、小蔡）下的所有员工"); 
                _logger.LogInformation("FindUsersByDataPermissionFilterAsync 预期结果：齐副经理、蔡经理、小蔡, 季经理、小季、系统管理员, 小肖");
                _logger.LogInformation("FindUsersByDataPermissionFilterAsync 实际结果：" + users.ToCommaSeparatedStringByFilter(m => m.DisplayName));
                Assert.True(users.Any());
            }
        }

        [Xunit.Fact]
        //[Xunit.Fact(Skip = "true")]
        public async Task Test_GetSimpleUserWithOrgsAndRolesByUserIdAsync()
        {
            InjectTenant(TestTenant);
            var testAccountService = Services.BuildServiceProvider().GetService<IAccountService>();

            var simpleUsers = await testAccountService.GetSimpleUserWithOrgsAndRolesByUserIdAsync(UserFixture.wfComSubAdminUserId);
            _logger.LogInformation("获取【齐副经理】所属部门和所属角色");
            _logger.LogInformation("GetSimpleUserWithOrgsAndRolesByUserIdAsync 预期结果：部门【企业, 销售部, 采购部】；角色【销售经理、采购经理】");
            _logger.LogInformation("GetSimpleUserWithOrgsAndRolesByUserIdAsync 实际结果：部门【" + simpleUsers.UserOrgNames.ToCommaSeparatedString() + "】；角色【" + simpleUsers.UserRoleNames.ToCommaSeparatedString() + "】");
            Assert.True(simpleUsers.UserOrgIds.Any());
            Assert.True(simpleUsers.UserRoleIds.Any());

            var detailUsers = await testAccountService.GetSimpleUserWithOrgsAndRolesByUserIdAsync(UserFixture.wfBuyTestUserId);
            _logger.LogInformation("获取【小蔡】所属部门和所属角色");
            _logger.LogInformation("GetSimpleUserWithOrgsAndRolesByUserIdAsync 预期结果：部门【采购部】；角色【采购助理】");
            _logger.LogInformation("GetSimpleUserWithOrgsAndRolesByUserIdAsync 实际结果：部门【" + simpleUsers.UserOrgNames.ToCommaSeparatedString() + "】；角色【" + simpleUsers.UserRoleNames.ToCommaSeparatedString() + "】");
            Assert.True(detailUsers.UserOrgIds.Any());
            Assert.True(detailUsers.UserRoleIds.Any());
        }

        #region DataPermitUtil
        [Xunit.Fact]
        //[Xunit.Fact(Skip = "true")]
        public async Task Test_LoadSubordinateUsersByUserId()
        {
            InjectTenant(TestTenant);
            var testAccountService = Services.BuildServiceProvider().GetService<IAccountApiService>();

            //最终调用AccountService下的方法：GetSimpleUserWithOrgsAndRolesByUserIdAsync
            var users = await Service.Util.DataPermitUtil.LoadSubordinateUsersByUserId(testAccountService, UserFixture.wfSaleAdminUserId);
            _logger.LogInformation("获取【肖经理】下所属部门下的所有员工");
            _logger.LogInformation("LoadSubordinateUsersByUserId 预期结果：用户【齐副经理、肖经理、小肖】");
            _logger.LogInformation("LoadSubordinateUsersByUserId 实际结果：用户【" + users.ToCommaSeparatedStringByFilter(m => m.DisplayName) + "】");

            var users2 = await Service.Util.DataPermitUtil.LoadSubordinateUsersByUserId(testAccountService, UserFixture.wfBuyTestUserId);
            _logger.LogInformation("获取【小蔡】下所属部门下的所有员工");
            _logger.LogInformation("LoadSubordinateUsersByUserId 预期结果：齐副经理、蔡经理、小蔡");
            _logger.LogInformation("LoadSubordinateUsersByUserId 实际结果：" + users2.ToCommaSeparatedStringByFilter(m => m.DisplayName));
        }
        #endregion

        [Xunit.Fact]
        public void UserLogMapperTest()
        {
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                var mapper = scope.ServiceProvider.GetService<AutoMapper.IMapper>();
                var model = new Service.DTO.Account.UserLoginLogDTO()
                {
                    OperatorId = Guid.NewGuid().ToString(),
                    Operator = "test",
                    OperateDate = DateTime.Now,
                    Type = ProcessLogType.Success,
                    IPAddress = "test",
                    BrowserInfo = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.82 Safari/537.36",
                    Remark = string.Format("用户【IP：{0}】使用浏览器【{1}】登录系统", "test", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.82 Safari/537.36"),
                };
                var result = mapper.Map<UserLoginLog>(model);
                Assert.True(result != null);
                Assert.Equal(result.OperatorId, model.OperatorId);
                Assert.Equal(result.Operator, model.Operator);
            }
        }

        protected override void TearDown()
        {

        }

    }
}
