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
    
    public class DESProvider_Test : KC.UnitTest.Framework.FrameworkTestBase
    {
        private ILogger _logger;
        public DESProvider_Test(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(DESProvider_Test));
        }

        [Xunit.Fact]
        public void Test_EncryptString()
        {
            String key = "12345678";
            String except = "L3c132f119l";

            String encryptString = DESProvider.EncryptString(except, key);
            String decryptString = DESProvider.DecryptString(encryptString, key);

            _logger.LogInformation("\ntestDesProvider EncryptString: " + encryptString);
            _logger.LogInformation("\ntestDesProvider DecryptString: " + decryptString);
            Assert.Equal(except, decryptString);
        }

        [Xunit.Fact]
        public void Test_DESProvider_EncryptString()
        {
            //var key = "dev-cfwin-EncryptKey";
            var key = "KCloudy-Microsoft-EncryptKey";
            var except = "P@ssw0rd";

            var encryptString = DESProvider.EncryptString(except, key);
            var decryptString = DESProvider.DecryptString(encryptString, key);

            _logger.LogInformation("\ntestTenantDesProvider EncryptString: " + encryptString);
            _logger.LogInformation("\ntestTenantDesProvider DecryptString: " + decryptString);
            Assert.Equal(except, decryptString);


        }

        [Xunit.Fact]
        public void Test_DESProvider()
        {
            var key = KC.Framework.Base.GlobalConfig.EncryptKey;
            var encryptString = "cF0V6oCzMrat9RYDOyRfuVdUKI64x5mOfFipfUzVklho/Y2EEVyZ21Ip3zxYoHrw7U9nuF3wzNt/QvFSH1NIZQ==";
            var decryptString = DESProvider.EncryptString(encryptString, "K7ef0139cbk");
            _logger.LogInformation(decryptString);

            var result = DESProvider.DecryptString(decryptString, "K7ef0139cbk");
            Assert.Equal(encryptString, result);
            _logger.LogInformation(result);
        }
        
    }
}
