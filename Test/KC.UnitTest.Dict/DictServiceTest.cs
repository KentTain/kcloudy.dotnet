using System;
using Microsoft.Extensions.Logging;

namespace KC.UnitTest.Dict
{
    public class DictServiceTest : DictTestBase
    {
        private ILogger _logger;
        public DictServiceTest(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(DictServiceTest));
        }


    }
}
