using System;
using System.Collections.Generic;
using System.Linq;
using KC.Service.WebApiService.Business;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KC.UnitTest.Customer
{
    public abstract class CustomerTestBase : CommonTestBase
    {
        protected IServiceProvider ServiceProvider;
        private ILogger _logger;

        public CustomerTestBase(CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(CustomerTestBase));
            ServiceProvider = Services.BuildServiceProvider();
        }

        protected override void InjectTenant(Framework.Tenant.Tenant tenant)
        {
            base.InjectTenant(tenant);

            KC.Service.Util.DependencyInjectUtil.InjectService(Services);
            KC.Service.Customer.DependencyInjectUtil.InjectService(Services);

            ServiceProvider = Services.BuildServiceProvider();
        }

        protected override void SetUp()
        {
            base.SetUp();

            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                var cache = scope.ServiceProvider.GetService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
                Service.CacheUtil.Cache = cache;
            }

            Services.AddSingleton<ITenantUserApiService, TenantUserApiService>();
        }
    }
}
