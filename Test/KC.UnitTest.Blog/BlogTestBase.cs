using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KC.UnitTest.Blog
{
    public abstract class BlogTestBase : CommonTestBase
    {
        protected IServiceProvider ServiceProvider;
        private static ILogger _logger;

        public BlogTestBase(CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(BlogTestBase));
            ServiceProvider = Services.BuildServiceProvider();
        }

        protected override void InjectTenant(Framework.Tenant.Tenant tenant)
        {
            base.InjectTenant(tenant);

            KC.Service.Util.DependencyInjectUtil.InjectService(Services);
            KC.Service.Blog.DependencyInjectUtil.InjectService(Services);
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
            
        }
    }
}
