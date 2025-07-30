using System;
using System.Net.Http;
using System.Threading.Tasks;
using KC.IdentityModel.Client;
using KC.Common;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Service.Constants;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using KC.Service.App;

namespace KC.UnitTest.App
{
    public class GitlabServiceTest : AppTestBase
    {
        private static ILogger _logger;
        public GitlabServiceTest(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(GitlabServiceTest));
        }

        [Xunit.Fact]
        public async Task Test_CreateAppGitProject()
        {
            DbaTenant.CodeEndpoint = "http://gitlab.kcloudy.com";
            DbaTenant.CodeAccessName = "cDba-token";
            DbaTenant.CodeAccessKeyPasswordHash = "7+uan74bsXZlzfabgPQvdotWd+Ls1/F/";
            //InjectTenant(TestTenant);
            InjectTenant(DbaTenant);

            var appService = ServiceProvider.GetService<IApplicationService>();
            var result = await appService.CreateAppGitProject("test");
            Assert.True(result);

            result = await appService.DeleteAppGitProject("test");
            Assert.True(result);
        }
    }
}
