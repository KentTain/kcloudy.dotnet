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

namespace KC.UnitTest.Training
{
    
    public class ConfigServiceTest : ConfigTestBase
    {
        private static ILogger _logger;
        public ConfigServiceTest(CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(ConfigServiceTest));
        }

    }
}
