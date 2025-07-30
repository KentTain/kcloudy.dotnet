using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;

namespace KC.UnitTest.Core
{
    public abstract class ThirdPartyTestBase : CommonTestBase
    {
        private ILogger _logger;
        protected readonly ITestOutputHelper Output;
        public ThirdPartyTestBase(CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(ThirdPartyTestBase));
        }

        public ThirdPartyTestBase(CommonFixture data, ITestOutputHelper tempOutput)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(ThirdPartyTestBase));
            Output = tempOutput;
        }



        protected override void SetUp()
        {
            base.SetUp();

            //AutoMapper.Mapper.Reset();
            //Services.AddEntityFrameworkSqlServer()
            //    .AddDistributedMemoryCache();

            //AutoMapper.Mapper.Initialize(cfg =>
            //{
            //    cfg.AddProfiles("KC.Service");
            //});

        }

    }
}
