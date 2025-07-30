using System;
using Microsoft.Extensions.Logging;

namespace KC.UnitTest.Offering
{
    public class OfferingServiceTest : OfferingTestBase
    {
        private ILogger _logger;
        public OfferingServiceTest(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(OfferingServiceTest));
        }


    }
}
