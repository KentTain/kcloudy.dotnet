using System;
using Microsoft.Extensions.Logging;

namespace KC.UnitTest.Contract
{
    public class DocumentInfoServiceTest : ContractTestBase
    {
        private ILogger _logger;
        public DocumentInfoServiceTest(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(DocumentInfoServiceTest));
        }


    }
}
