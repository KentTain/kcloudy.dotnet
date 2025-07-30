using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;

namespace KC.UnitTest.Admin
{
    public class GitlabServiceTest : AdminTestBase
    {
        private static ILogger _logger;
        public GitlabServiceTest(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(GitlabServiceTest));
        }

        [Xunit.Fact]
        public async Task Test_CreateTenantGitlabGroupAndUser()
        {
            //InjectTenant(TestTenant);
            InjectTenant(DbaTenant);

            var result = await TenantUserService.CreateTenantGitlabGroupAndUser(TestTenant.TenantName, "P@ssw0rd");
            System.Console.WriteLine(result);
            Assert.True(string.IsNullOrEmpty(result));
        }
    }
}
