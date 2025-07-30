using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace KC.UnitTest.Framework
{
    public abstract class FrameworkTestBase : CommonTestBase
    {
        private ILogger _logger;
        public FrameworkTestBase(CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(FrameworkTestBase));
        }

        protected override void SetUp()
        {
            base.SetUp();

            //AutoMapper.Mapper.Reset();
            //AutoMapper.Mapper.Initialize(cfg =>
            //{
            //    cfg.AddProfiles("KC.Service");
            //});

        }

    }
}
