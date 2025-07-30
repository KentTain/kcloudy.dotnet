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

namespace KC.UnitTest.CodeGenerate
{
    public class ModelDefinitionServiceTest : CodeGenerateTestBase
    {
        private static ILogger _logger;
        public ModelDefinitionServiceTest(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(ModelDefinitionServiceTest));
        }

        protected override void InjectTenant(Framework.Tenant.Tenant tenant)
        {
            base.InjectTenant(tenant);

            KC.Service.Util.DependencyInjectUtil.InjectService(Services);
            KC.Service.CodeGenerate.DependencyInjectUtil.InjectService(Services);

            ServiceProvider = Services.BuildServiceProvider();
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                var cache = scope.ServiceProvider.GetService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
                Service.CacheUtil.Cache = cache;
            }
        }

        protected override void SetUp()
        {
            base.SetUp();

            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                var cache = scope.ServiceProvider.GetService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
                Service.CacheUtil.Cache = cache;
            }
        }

    }
}
