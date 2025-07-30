using System;
using Microsoft.Extensions.Logging;

namespace KC.UnitTest.Portal
{
    public class PortalServiceTest : PortalTestBase
    {
        private ILogger _logger;
        public PortalServiceTest(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(PortalServiceTest));
        }


    }
}
