using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.UnitTest;
using Microsoft.Extensions.Logging;
using Xunit;

namespace KC.ThirdParty.UnitTest
{
    
    public class QRCodeHelper_Test : KC.UnitTest.Core.ThirdPartyTestBase
    {
        private ILogger _logger;
        public QRCodeHelper_Test(CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(QRCodeHelper_Test));
        }

        [Xunit.Fact]
        public void Test_Qrcode()
        {
            var rootdir = AppContext.BaseDirectory;
            string testCode = "testsetsesetset中国";
            var image = QRCodeHelper.QREncoder(testCode, 400);

            SaveImg(rootdir + "/Image", image);

            //var result = QRCodeHelper.QRDecoder(image);

            //Assert.Equal(testCode, result);
        }

        private void SaveImg(string strPath, Bitmap img)
        {
            //保存图片到目录  
            if (Directory.Exists(strPath))
            {
                using (img)
                {
                    //文件名称  
                    string guid = Guid.NewGuid().ToString().Replace("-", "") + ".png";
                    img.Save(strPath + "/" + guid, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
            else
            {
                //当前目录不存在，则创建  
                Directory.CreateDirectory(strPath);
            }
        }
    }
}
