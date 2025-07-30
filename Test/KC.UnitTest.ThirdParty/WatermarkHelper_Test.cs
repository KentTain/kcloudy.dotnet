using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.UnitTest;
using Microsoft.Extensions.Logging;
using Xunit;

namespace KC.ThirdParty.UnitTest
{
    
    public class WatermarkHelper_Test : KC.UnitTest.Core.ThirdPartyTestBase
    {
        private ILogger _logger;
        public WatermarkHelper_Test(CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(WatermarkHelper_Test));
        }

        [Xunit.Fact]
        public void Test_Watermark()
        {
            var rootdir = AppContext.BaseDirectory;
            WatermarkHelper.AddWatermarkToImage(rootdir + @"\Image\1.jpg", rootdir + @"\Image\result.jpg", rootdir + @"\Image\watermark.png", "鑫亚科技：http://www.kcloudy.com");

            //WatermarkHelper.AddWatermarkToWord(@"Image\1.docx", @"Image\result.docx", @"Cfwin.Com");
            //WatermarkHelper.AddWatermarkToPdf1(@"Image\1.pdf", @"Image\result.pdf", @"Cfwin.Com");

            WatermarkHelper.AddWatermarkPictureToPdf(rootdir + @"\Image\1.pdf", rootdir + @"\Image\resultInPic.pdf", rootdir + @"\Image\2.png");
            WatermarkHelper.AddWatermarkToPdf(rootdir + @"\Image\1.pdf", rootdir + @"\Image\result2.pdf", "Cfwin.Com");
        }
    }
}
