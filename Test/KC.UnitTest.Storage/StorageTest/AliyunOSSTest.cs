using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using KC.Framework.Tenant;
using Aliyun.OSS;
using System.IO;
using Aliyun.OSS.Common;
using KC.Common.FileHelper;

namespace KC.UnitTest.Storage.Aliyun
{
    public class AliyunOSSTest : KC.UnitTest.Storage.StorageTestBase
    {
        private static string bucket = "k-storage";

        private static string fileType = FileType.Image.ToString();
        private static string fileFormat = ImageFormat.Png.ToString();

        private static string userId = RoleConstants.AdminUserId.ToLower();
        private static ILogger _logger;
        public AliyunOSSTest(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(AliyunOSSTest));
        }


        [Xunit.Fact]
        public void Test_File_Txt()
        {
            var blobId = "file-blobId-txt";
            var filePath = @"File\UploadTxt.txt";
            var downloadFileName = @"File\downloadTxt.txt";
            UploadToOSS(blobId, filePath);

            DownloadFromOSS2(blobId, downloadFileName);
        }

        [Xunit.Fact]
        public void Test_File_Image()
        {
            var blobId = "file-blobId-png";
            var filePath = @"File\UploadImage.png";
            var downloadFileName = @"File\downloadImage.png";
            //UploadToOSS(blobId, filePath);

            DownloadFromOSS1(blobId, downloadFileName);
            //DownloadFromOSS2(blobId, downloadFileName);
        }

        [Xunit.Fact]
        public void Test_File_Word()
        {
            var blobId = "file-blobId-word";
            var filePath = @"File\UploadWord.docx";
            var downloadFileName = @"File\downloadWord.docx";

            UploadToOSS(blobId, filePath);

            DownloadFromOSS2(blobId, downloadFileName);
        }

        [Xunit.Fact]
        public void Test_File_Pdf()
        {
            var blobId = "file-blobId-pdf";
            var filePath = @"File\UploadPdf.pdf";
            var downloadFileName = @"File\downloadPdf.pdf";

            UploadToOSS(blobId, filePath);

            DownloadFromOSS2(blobId, downloadFileName);
        }

        // 创建OssClient实例。
        protected OssClient GetAliyunOSSClient()
        {
            var config = new ClientConfiguration();
            //config.ConnectionLimit = 512;
            config.MaxErrorRetry = 3;
            config.ConnectionTimeout = 300;
            // 设置是否支持CNAME。CNAME是指将自定义域名绑定到存储空间上。
            config.IsCname = true;
            var client = new OssClient("http://k-storage.oss-cn-shenzhen.aliyuncs.com",
                "LTAI4G3LCfpphDx3pLBgbHmY", "Ov8acS2cQACMYZSJ9sDWcwVgTvvQP6", config);
            return client;
        }

        private void UploadToOSS(string blobId, string filePath)
        {
            var client = GetAliyunOSSClient();
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    // 上传文件并设置文件元信息。
                    client.PutObject(bucket, blobId, stream);
                    _logger.LogInformation(filePath + "：Put object succeeded. len: " + stream.Length);
                }
            }
            catch (OssException ex)
            {
                _logger.LogError(string.Format(filePath + "：WriteBlob failed with error info: {0}; Error info: {1}. " + Environment.NewLine + "RequestID:{2}" + Environment.NewLine + "HostID:{3}",
                                  ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(filePath + "：WriteBlob failed with error info: {0}; Error info: {1}. " + Environment.NewLine + "RequestID:{2}" + Environment.NewLine + "HostID:{3}",
                                  ex.Message, ex.StackTrace, ex.InnerException?.Message, ex.InnerException?.StackTrace));
            }
        }

        //获取流后，先获取bytes[]，而后写入本地文件
        private void DownloadFromOSS1(string blobId, string downloadFileName)
        {
            var client = GetAliyunOSSClient();
            try
            {
                var response = client.GetObject(bucket, blobId);
                byte[] data;
                using (var stream = response.Content)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        data = memoryStream.ToArray();
                    }
                }

                using (var m = new MemoryStream(data))
                {
                    using (var fs = File.Open(downloadFileName, FileMode.OpenOrCreate))
                    {
                        m.WriteTo(fs);
                    }
                }
            }
            catch (OssException ex)
            {
                _logger.LogError(string.Format(downloadFileName + "：GetBlob failed with error info: {0}; Error info: {1}. " + Environment.NewLine + "RequestID:{2}" + Environment.NewLine + "HostID:{3}",
                                  ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(downloadFileName + "：GetBlob failed with error info: {0}; Error info: {1}. " + Environment.NewLine + "RequestID:{2}" + Environment.NewLine + "HostID:{3}",
                                  ex.Message, ex.StackTrace, ex.InnerException?.Message, ex.InnerException?.StackTrace));
            }
        }

        //获取流后，直接写入本地文件
        private void DownloadFromOSS2(string blobId, string downloadFileName)
        {
            var client = GetAliyunOSSClient();
            try
            {
                var response = client.GetObject(bucket, blobId);

                //直接将流写入文件没问题
                byte[] buf;
                using (var stream = response.Content)
                {
                    buf = new byte[1024];
                    var fs = File.Open(downloadFileName, FileMode.OpenOrCreate);
                    var len = 0;
                    // 通过输入流将文件的内容读取到文件或者内存中。
                    while ((len = stream.Read(buf, 0, buf.Length)) != 0)
                    {
                        fs.Write(buf, 0, len);
                    }
                    fs.Close();

                    _logger.LogInformation(downloadFileName + "：download object succeeded. buf len: " + buf.Length);
                }

                byte[] data;
                using (var stream = new FileStream(downloadFileName, FileMode.Open, FileAccess.Read))
                {
                    data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                }

                using (var m = new MemoryStream(data))
                {
                    using (var fs = File.Open(downloadFileName, FileMode.OpenOrCreate))
                    {
                        m.WriteTo(fs);
                    }
                }

                _logger.LogInformation(downloadFileName + "：Get object succeeded. data len: " + data.Length);
            }
            catch (OssException ex)
            {
                _logger.LogError(string.Format(downloadFileName + "：GetBlob failed with error info: {0}; Error info: {1}. " + Environment.NewLine + "RequestID:{2}" + Environment.NewLine + "HostID:{3}",
                                  ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(downloadFileName + "：GetBlob failed with error info: {0}; Error info: {1}. " + Environment.NewLine + "RequestID:{2}" + Environment.NewLine + "HostID:{3}",
                                  ex.Message, ex.StackTrace, ex.InnerException?.Message, ex.InnerException?.StackTrace));
            }
        }
    }
}
