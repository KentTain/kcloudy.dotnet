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
using Microsoft.Extensions.Logging;
using KC.Service.WebApiService.Business;

namespace KC.UnitTest.Account
{
    
    public class RoleServiceTest : AccountTestBase
    {
        private ILogger _logger;
        public RoleServiceTest(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(RoleServiceTest));
        }

        [Xunit.Fact]
        public async Task Test_GetSubordinateUsersByUserId()
        {
            InjectTenant(TestTenant);
            var testDbAccountService = Services.BuildServiceProvider().GetService<IAccountApiService>();

            //获取“IT开发部：管理岗”--“系统管理员”下所属部门下的所有员工
            var users = await Service.Util.DataPermitUtil.LoadSubordinateUsersByUserId(testDbAccountService, RoleConstants.AdminUserId);
            _logger.LogInformation(users.ToCommaSeparatedStringByFilter(m => m.DisplayName));
            Assert.True(users.Count > 1);
            ////获取“开发部：管理岗”--“开发部经理”下所属部门下的所有员工
            //var users1 = testDbAccountService.GetSubordinateUsersByUserId("3ac66f49-3269-4c4f-bd35-e0b5ff06e41f");
            //Console.WriteLine(users.ToCommaSeparatedStringByFilter(m => m.DisplayName));
            //Assert.True(users1.Count > 1);
        }

        [Xunit.Fact]
        public async Task Test_GetSubordinateUsersByOrgId()
        {
            InjectTenant(TestTenant);
            var testDbAccountService = Services.BuildServiceProvider().GetService<IAccountApiService>();

            //获取“IT开发部”下所属部门下的所有员工
            var users = await Service.Util.DataPermitUtil.LoadSubordinateUsersByOrgId(testDbAccountService, 1);
            _logger.LogInformation(users.ToCommaSeparatedStringByFilter(m => m.DisplayName));
            Assert.True(users.Count > 1);
            ////获取“开发部”-下所属部门下的所有员工
            //var users1 = testDbAccountService.GetSubordinateUsersByOrgId(6);
            //Console.WriteLine(users.ToCommaSeparatedStringByFilter(m => m.DisplayName));
            //Assert.True(users1.Count > 1);
        }

        [Xunit.Fact]
        public async Task Test_FindRolesWithUsersByRoleIds()
        {
            InjectTenant(TestTenant);
            var testRoleService = Services.BuildServiceProvider().GetService<IRoleService>();

            //获取【角色：包含--系统管理员、销售经理，排除--销售经理】所属角色及角色下所有员工
            var depIds = new List<string>() { RoleConstants.AdminRoleId, RoleConstants.SaleManagerRoleId };
            var exceptDepIds = new List<string>() { RoleConstants.SaleManagerRoleId };
            var orgsWithUsers = await testRoleService.FindRolesWithUsersByRoleIds(depIds, exceptDepIds);
            _logger.LogInformation("获取【角色：包含--系统管理员、销售经理，排除--销售经理】所属角色及角色下所有员工");
            _logger.LogInformation("FindRolesWithUsersByRoleIds 预期结果：角色：【系统管理员】，人员：【admin】");
            _logger.LogInformation(string.Format("FindRolesWithUsersByRoleIds 实际结果：角色：【{0}】，人员：【{1}】",
                orgsWithUsers.ToCommaSeparatedStringByFilter(m => m.DisplayName),
                orgsWithUsers.SelectMany(m => m.Users).ToCommaSeparatedStringByFilter(m => m.UserName)));
            Assert.True(orgsWithUsers.Any());
        }

        [Xunit.Fact]
        public async Task Test_LoadUserMenusByRoleIdsAsync()
        {
            InjectTenant(DbaTenant);
            Services.AddScoped<IRoleService, RoleService>();
            var dbaAccountService = Services.BuildServiceProvider().GetService<IRoleService>();

            var roleIds = new List<string>() { RoleConstants.AdminRoleId };
            var result = await dbaAccountService.FindUserMenusByRoleIdsAsync(roleIds);
            Assert.True(result.Any());
        }

        [Xunit.Fact]
        public async Task Test_LoadUserPermissionsByRoleIds()
        {
            InjectTenant(DbaTenant);
            Services.AddScoped<IRoleService, RoleService>();
            var dbaAccountService = Services.BuildServiceProvider().GetService<IRoleService>();

            var roleIds = new List<string>() { RoleConstants.AdminRoleId };
            var result = await dbaAccountService.FindUserPermissionsByRoleIdsAsync(roleIds);
            Assert.True(result.Any());
        }

    }
}
