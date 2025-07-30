using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Collections.Generic;
using KC.Framework.Extension;
using Xunit;
using KC.Framework.Base;
using KC.Common.FileHelper;
using Microsoft.Extensions.Logging;

namespace KC.Common.UnitTest
{
    /// <summary>
    /// EncryptTest 的摘要说明
    /// </summary>
    
    public class ImageHelper_Test : KC.UnitTest.Core.CommomTestBase
    {
        private ILogger _logger;
        public ImageHelper_Test(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(ImageHelper_Test));
        }

        [Xunit.Fact]
        public void Test_MakeThumbnail()
        {
            var rootdir = AppContext.BaseDirectory;
            var thumbnailPath = rootdir + "Image/1-thumbnail.jpg";
            ImageHelper.MakeThumbnail(rootdir + "Image/1.jpg", thumbnailPath, 128, 128);

            var isExist = File.Exists(thumbnailPath);
            Assert.True(isExist);

            if (isExist)
                File.Delete(thumbnailPath);
        }

        [Xunit.Fact]
        public void Test_MakeSmallImage()
        {
            var rootdir = AppContext.BaseDirectory;
            var smallPath = rootdir + "Image/1-small.jpg";
            using (FileStream stream = File.OpenRead(rootdir + "Image/1.jpg"))
            {
                Image image = new Bitmap(stream);

                var smallImage = ImageHelper.MakeSmallImage(image, 72, 72);
                smallImage.Save(smallPath);
            }

            var isExist = File.Exists(smallPath);
            Assert.True(isExist);

            if (isExist)
                File.Delete(smallPath);
        }

        [Xunit.Fact]
        public void Test_MergeQrImg()
        {
            var rootdir = AppContext.BaseDirectory;
            var smallPath = rootdir + "Image/1-merge.jpg";

            using (FileStream stream1 = File.OpenRead(rootdir + "Image/1.jpg"))
            using (FileStream stream2 = File.OpenRead(rootdir + "Image/2.png"))
            {
                var image1 = new Bitmap(stream1);
                var image2 = new Bitmap(stream2);

                var smallImage = ImageHelper.MergeQrImg(image1, image2);
                smallImage.Save(smallPath);
            }

            var isExist = File.Exists(smallPath);
            Assert.True(isExist);

            if (isExist)
                File.Delete(smallPath);
        }
    }
}
