using System;
using Microsoft.Extensions.Logging;

namespace KC.UnitTest.Supplier
{
    public class BakServiceTest : BakTestBase
    {
        private ILogger _logger;
        public BakServiceTest(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(BakServiceTest));
        }


    }
}
