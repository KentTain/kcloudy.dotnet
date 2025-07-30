using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using KC.Common.ParserManager;
using KC.UnitTest;
using Xunit;
using Microsoft.Extensions.Logging;

namespace KC.Common.UnitTest
{
    /// <summary>
    /// EncryptTest 的摘要说明
    /// </summary>
    
    public class ParserMananger_Test : KC.UnitTest.Core.CommomTestBase
    {
        private ILogger _logger;
        public ParserMananger_Test(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(ParserMananger_Test));
        }

        [Xunit.Fact]
        public void Test_ParserMananger()
        {
            var func = @"(12+32*2)/2 + 102";
            decimal except = 140m;
            var calc = (new Parser().Evaluate(func)).ToString();
            decimal result = 0.0m;
            if (decimal.TryParse(calc, out result))
            {
                _logger.LogInformation("calc result is decimal!");
                Assert.Equal(except, result);
            }
        }
    }
}
