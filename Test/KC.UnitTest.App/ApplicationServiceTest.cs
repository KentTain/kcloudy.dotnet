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
    public class ApplicationServiceTest : AppTestBase
    {
        private static ILogger _logger;
        public ApplicationServiceTest(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(ApplicationServiceTest));
        }

        [Xunit.Fact]
        public async Task Test_SavePermissionsAsync()
        {
            InjectTenant(TestTenant);

            var appService = ServiceProvider.GetService<IApplicationService>();
            var result = await appService.CreateAppGitProject("test");
            Assert.True(result);

            result = await appService.CreateAppGitProject("test");
            Assert.True(result);
        }
    }
}
