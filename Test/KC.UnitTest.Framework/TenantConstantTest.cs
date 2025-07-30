using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using KC.Framework.SecurityHelper;
using Xunit;
using Microsoft.Extensions.Logging;
using KC.Framework.Tenant;

namespace KC.Framework.UnitTest
{
    /// <summary>
    /// StringHelperTest 的摘要说明
    /// </summary>

    public class TenantConstantTest : KC.UnitTest.Framework.FrameworkTestBase
    {
        private ILogger _logger;
        public TenantConstantTest(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(TenantConstantTest));
        }

        [Xunit.Fact]
        public void TenantClientSecret_Test()
        {
            var exceptClientId = "Y0RiYQ==";
            var exceptClientSecret = "MmJmNWIyM2Q5ZjY4OWU5YzFmYWVkZTUwNzY2ZWJkNTg=";
            var clientIdResult = TenantConstant.GetClientIdByTenant(TenantConstant.DbaTenantApiAccessInfo);
            var clientSecretResult = TenantConstant.GetClientSecretByTenant(TenantConstant.DbaTenantApiAccessInfo);
            var clientSecretSha256Result = TenantConstant.Sha256(clientSecretResult);

            var credential = Base64Provider.EncodeString(String.Format("{0}:{1}", exceptClientId, exceptClientSecret));
            _logger.LogInformation("\ncDba: credential: " + credential);

            _logger.LogInformation("\ncDba: ClientIdResult: " + clientIdResult);
            _logger.LogInformation("\ncDba: CientSecretResult: " + clientSecretResult);
            _logger.LogInformation("\ncDba: ClientSecretSha256Result: " + clientSecretSha256Result);
            Assert.Equal(exceptClientId, clientIdResult);
            Assert.Equal(exceptClientSecret, clientSecretResult);

            var clientIdResult1 = TenantConstant.GetClientIdByTenant(TenantConstant.TestTenantApiAccessInfo);
            var clientSecretResult1 = TenantConstant.GetClientSecretByTenant(TenantConstant.TestTenantApiAccessInfo);
            _logger.LogInformation("\ncTest: ClientIdResult1: " + clientIdResult1);
            _logger.LogInformation("\ncTest: CientSecretResult1: " + clientSecretResult1);
        }
    }
}
