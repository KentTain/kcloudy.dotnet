using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using KC.Framework.SecurityHelper;
using Xunit;
using Microsoft.Extensions.Logging;

namespace KC.Framework.UnitTest
{
    /// <summary>
    /// StringHelperTest 的摘要说明
    /// </summary>

    public class Base64ProviderTest : KC.UnitTest.Framework.FrameworkTestBase
    {
        private ILogger _logger;
        public Base64ProviderTest(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(Base64ProviderTest));
        }

        [Xunit.Fact]
        public void testBase64Provider()
        {
            string except = "L3c132f119l";

            string encryptString = Base64Provider.EncodeString(except);
            string decryptString = Base64Provider.DecodeString(encryptString);

            _logger.LogInformation("\ntestBase64Provider EncryptString: " + encryptString);
            _logger.LogInformation("\ntestBase64Provider DecryptString: " + decryptString);
            Assert.Equal(except, decryptString);
        }
    }
}
