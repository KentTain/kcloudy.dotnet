using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace KC.UnitTest.Core
{
    public abstract class CommomTestBase : CommonTestBase
    {
        private ILogger _logger;
        public CommomTestBase(CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(CommomTestBase));
        }

        protected override void SetUp()
        {
            base.SetUp();
            
            //Services.AddEntityFrameworkSqlServer()
            //    .AddDistributedMemoryCache();

            //AutoMapper.Mapper.Initialize(cfg =>
            //{
            //    cfg.AddProfiles("KC.Service");
            //});

        }

    }
}
