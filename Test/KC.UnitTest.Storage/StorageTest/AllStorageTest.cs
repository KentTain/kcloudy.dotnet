using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Extension;
using KC.Common.FileHelper;
using KC.Framework.SecurityHelper;
using KC.Framework.Util;
using KC.Framework.Tenant;
using KC.Storage.Util;
using KC.UnitTest;
using Xunit;
using Microsoft.Extensions.Logging;
using KC.Framework.Base;

namespace KC.UnitTest.Storage
{
    /// <summary>
    /// 通过设置Web.config中的：BlobStorage节点，控制文件存储的方式</br>
    ///     FileSystem：本地存储
    ///     azure：微软云Azure存储
    ///     cmb：招行云/AWS中的存储
    ///     Aliyun：阿里云OSS存储
    /// </summary>
    public class AllStorageTest : KC.UnitTest.Storage.StorageTestBase
    {
        private static string fileName = "TestImage";
        private static string fileType = FileType.Image.ToString();
        private static string fileFormat = ImageFormat.Png.ToString();

        private static string userId = RoleConstants.AdminUserId.ToLower();
        //private static string blobId = Guid.NewGuid().ToString().ToLower();
        private static string blobId = "ef35812e-a2aa-4e51-9186-624e9d1f9a0b";
        private BlobInfo blobInfo = new BlobInfo(blobId, fileType, fileFormat, fileName, "png");


        private ILogger _logger;
        public AllStorageTest(CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(AllStorageTest));
        }

        private string GetStorageConnectionString(Tenant tenant)
        {
            try
            {
                return
                string.Format(
                    @"BlobEndpoint={0};DefaultEndpointsProtocol=https;AccountName={1};AccountKey={2}",
                    tenant.StorageEndpoint, tenant.StorageAccessName,
                    EncryptPasswordUtil.DecryptPassword(tenant.StorageAccessKeyPasswordHash, tenant.PrivateEncryptKey));
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        //cTest：Local File; Copy to cBuy: Azure blob
        [Xunit.Fact]
        public void AllBlobOperator_LocalFile_Test()
        {
            var filePath = @"File\";
            var uploadfile = filePath + "UploadImage.png";
            var heByte = GetFileBytes(uploadfile);
            blobInfo.Data = heByte;

            var success = BlobUtil.SaveBlob(TestTenant, userId, blobInfo);
            _logger.LogInformation("cTest use storage connection: " + GetStorageConnectionString(TestTenant));
            Assert.True(success);

            var existResult = BlobUtil.ExistBlob(TestTenant, blobId);
            Assert.True(existResult);

            var blob = BlobUtil.GetBlobById(TestTenant, userId, blobId);
            Assert.True(blob.Data.Any());
            using (var m = new MemoryStream(blob.Data))
            {
                var downfile = filePath + "downloadImage-" + blobId + "-file.png";
                using (var fs = new FileStream(downfile, FileMode.OpenOrCreate))
                {
                    m.WriteTo(fs);
                }
            }

            var copyResult = BlobUtil.CopyBlobsToOtherClient(TestTenant, BuyTenant, new List<string>() { blobId }, userId);
            Assert.True(copyResult);
            _logger.LogInformation("Copty to cBuy use storage connection: " + GetStorageConnectionString(BuyTenant));

            var removeResult = BlobUtil.RemoveBlob(TestTenant, userId, blobId);
            Assert.True(removeResult);
        }

        //cDba：cDba Server; Copy to cBuy: Azure blob
        [Xunit.Fact]
        public void AllBlobOperator_FTP_Test()
        {
            var filePath = @"File\";
            var uploadfile = filePath + "UploadImage.png";
            var heByte = GetFileBytes(uploadfile);
            blobInfo.Data = heByte;
            var success = BlobUtil.SaveBlob(DbaTenant, userId, blobInfo);
            _logger.LogInformation("cDba use storage connection: " + GetStorageConnectionString(DbaTenant));
            Assert.True(success);

            var existResult = BlobUtil.ExistBlob(DbaTenant, blobId);
            Assert.True(existResult);

            var blob = BlobUtil.GetBlobById(DbaTenant, userId, blobId);
            Assert.True(blob.Data.Any());
            using (var m = new MemoryStream(blob.Data))
            {
                var downfile = filePath + "downloadImage-" + blobId + "-ftp.png";
                using (var fs = new FileStream(downfile, FileMode.OpenOrCreate))
                {
                    m.WriteTo(fs);
                }
            }

            var copyResult = BlobUtil.CopyBlobsToOtherClient(DbaTenant, BuyTenant, new List<string>() { blobId }, userId);
            Assert.True(copyResult);
            _logger.LogInformation("Copty to cBuy use storage connection: " + GetStorageConnectionString(BuyTenant));

            var removeResult = BlobUtil.RemoveBlob(DbaTenant, userId, blobId);
            Assert.True(removeResult);
        }

        //cBuy：Azure blob; Copy to cTest：Local File
        [Xunit.Fact]
        public void AllBlobOperator_AzureBlob_Test()
        {
            var filePath = @"File\";
            var uploadfile = filePath + "UploadImage.png";
            var heByte = GetFileBytes(uploadfile);
            blobInfo.Data = heByte;
            var success = BlobUtil.SaveBlob(BuyTenant, userId, blobInfo);
            _logger.LogInformation("cBuy use storage connection: " + GetStorageConnectionString(BuyTenant));
            Assert.True(success);

            var existResult = BlobUtil.ExistBlob(BuyTenant, blobId);
            Assert.True(existResult);

            var blob = BlobUtil.GetBlobById(BuyTenant, userId, blobId);
            Assert.True(blob.Data.Any());
            using (var m = new MemoryStream(blob.Data))
            {
                var downfile = filePath + "downloadImage-" + blobId + "-blob.png";
                using (var fs = new FileStream(downfile, FileMode.OpenOrCreate))
                {
                    m.WriteTo(fs);
                }
            }

            var copyResult = BlobUtil.CopyBlobsToOtherClient(BuyTenant, TestTenant, new List<string>() { blobId }, userId);
            Assert.True(copyResult);
            _logger.LogInformation("Copty to cTest use storage connection: " + GetStorageConnectionString(TestTenant));

            var removeResult = BlobUtil.RemoveBlob(BuyTenant, userId, blobId);
            Assert.True(removeResult);
        }

        //cSale：Aliyun OSS; Copy to cTest：Local File
        [Xunit.Fact]
        public void AllBlobOperator_AliyunOSS_Test()
        {
            var filePath = @"File\";
            var uploadfile = filePath + "UploadImage.png";
            var heByte = GetFileBytes(uploadfile);
            blobInfo.Data = heByte;
            var success = BlobUtil.SaveBlob(SaleTenant, userId, blobInfo);
            _logger.LogInformation("cSale use storage connection: " + GetStorageConnectionString(SaleTenant));
            Assert.True(success);

            var existResult = BlobUtil.ExistBlob(SaleTenant, blobId);
            Assert.True(existResult);

            var blob = BlobUtil.GetBlobById(SaleTenant, userId, blobId);
            Assert.True(blob.Data.Any());
            using (var m = new MemoryStream(blob.Data))
            {
                var downfile = filePath + "downloadImage-" + blobId + "-blob.png";
                using (var fs = new FileStream(downfile, FileMode.OpenOrCreate))
                {
                    m.WriteTo(fs);
                }
            }

            var copyResult = BlobUtil.CopyBlobsToOtherClient(SaleTenant, TestTenant, new List<string>() { blobId }, userId);
            Assert.True(copyResult);
            _logger.LogInformation("Copty to cTest use storage connection: " + GetStorageConnectionString(TestTenant));

            var removeResult = BlobUtil.RemoveBlob(SaleTenant, userId, blobId);
            Assert.True(removeResult);
        }

        #region 不同文件的测试
        [Xunit.Fact]
        public void MultiFile_SaveBlob_Test()
        {
            //txt文件
            var blobId1 = "file-blobId-txt";
            var filePath = @"File\UploadTxt.txt";
            var heByte = GetFileBytes(filePath);
            blobInfo.Id = blobId1;
            blobInfo.Ext = "txt";
            blobInfo.FileFormat = DocFormat.Text.ToString();
            blobInfo.Data = heByte;
            var success = BlobUtil.SaveBlob(TestTenant, userId, blobInfo);
            Assert.True(success);
            if (success)
            {
                var blob = BlobUtil.GetBlobById(TestTenant, userId, blobId1);
                using (var m = new MemoryStream(blob.Data))
                {
                    var downfile = @"File\downloadTxt.txt";
                    using (var fs = new FileStream(downfile, FileMode.OpenOrCreate))
                    {
                        m.WriteTo(fs);
                    }
                }
            }

            //image文件
            blobId1 = "file-blobId-png";
            filePath = @"File\UploadImage.png";
            heByte = GetFileBytes(filePath);
            blobInfo.Id = blobId1;
            blobInfo.Ext = "png";
            blobInfo.FileFormat = ImageFormat.Png.ToString();
            blobInfo.Data = heByte;
            success = BlobUtil.SaveBlob(TestTenant, userId, blobInfo);
            Assert.True(success);
            if (success)
            {
                var blob = BlobUtil.GetBlobById(TestTenant, userId, blobId1);
                using (var m = new MemoryStream(blob.Data))
                {
                    var downfile = @"File\downloadImage.png";
                    using (var fs = new FileStream(downfile, FileMode.OpenOrCreate))
                    {
                        m.WriteTo(fs);
                    }
                }
            }

            //word文件
            blobId1 = "file-blobId-word";
            filePath = @"File\UploadWord.docx";
            heByte = GetFileBytes(filePath);
            blobInfo.Id = blobId1;
            blobInfo.Ext = "docx";
            blobInfo.FileFormat = DocFormat.Docx.ToString();
            blobInfo.Data = heByte;
            success = BlobUtil.SaveBlob(TestTenant, userId, blobInfo);
            Assert.True(success);
            if (success)
            {
                var blob = BlobUtil.GetBlobById(TestTenant, userId, blobId1);
                using (var m = new MemoryStream(blob.Data))
                {
                    var downfile = @"File\downloadWord.docx";
                    using (var fs = new FileStream(downfile, FileMode.OpenOrCreate))
                    {
                        m.WriteTo(fs);
                    }
                }
            }

            //pdf文件
            blobId1 = "file-blobId-pdf";
            filePath = @"File\UploadPdf.pdf";
            heByte = GetFileBytes(filePath);
            blobInfo.Id = blobId1;
            blobInfo.Ext = "pdf";
            blobInfo.FileFormat = DocFormat.Pdf.ToString();
            blobInfo.Data = heByte;
            success = BlobUtil.SaveBlob(TestTenant, userId, blobInfo);
            Assert.True(success);
            if (success)
            {
                var blob = BlobUtil.GetBlobById(TestTenant, userId, blobId1);
                using (var m = new MemoryStream(blob.Data))
                {
                    var downfile = @"File\downloadPdf.pdf";
                    using (var fs = new FileStream(downfile, FileMode.OpenOrCreate))
                    {
                        m.WriteTo(fs);
                    }
                }
            }

        }


        private byte[] GetFileBytes(string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var fsLen = (int)stream.Length;
                var heByte = new byte[fsLen];
                stream.Read(heByte, 0, fsLen);

                return heByte;
            }
        }
        #endregion

        #region 单个方法的测试
        [Xunit.Fact]
        public void SaveBlob_Test()
        {
            var filePath = @"File\UploadImage.png";
            var heByte = GetFileBytes(filePath);
            blobInfo.Ext = "png";
            blobInfo.FileFormat = ImageFormat.Png.ToString();
            blobInfo.Data = heByte;
            var success = BlobUtil.SaveBlob(TestTenant, userId, blobInfo);

            _logger.LogInformation("DevDb use storage connection: " + GetStorageConnectionString(TestTenant));
            Assert.True(success);
        }
        [Xunit.Fact]
        public void CopyBlobsToOtherClient_Test()
        {
            //blobId = "ef35812e-a2aa-4e51-9186-624e9d1f9a0b";
            var copyResult = BlobUtil.CopyBlobsToOtherClient(TestTenant, BuyTenant, new List<string> { blobId }, userId);
            _logger.LogInformation("DevDb use storage connection: " + GetStorageConnectionString(TestTenant));
            _logger.LogInformation("Customer1 use storage connection: " + GetStorageConnectionString(BuyTenant));
            Assert.True(copyResult);

            var existResult = BlobUtil.ExistBlob(BuyTenant, blobId);
            Assert.True(existResult);
        }
        [Xunit.Fact]
        public void ExistBlob_Test()
        {
            //blobId = "ef35812e-a2aa-4e51-9186-624e9d1f9a0b";
            var existResult = BlobUtil.ExistBlob(TestTenant, blobId);
            Assert.True(existResult);
        }
        [Xunit.Fact]
        public void GetContainers_Test()
        {
            var containers = BlobUtil.GetContainers(TestTenant);
            if (containers.Any())
            {
                _logger.LogInformation(string.Format("DevDb[{0}]'s file path has containers: {1}. ",
                    TestTenant.CloudType, containers.ToCommaSeparatedString()));
            }
            Assert.True(containers.Any());

            var containers1 = BlobUtil.GetContainers(BuyTenant);
            if (containers1.Any())
            {
                _logger.LogInformation(string.Format("Customer1[{0}]'s file path has containers: {1}. ",
                    BuyTenant.CloudType, containers1.ToCommaSeparatedString()));
            }
            Assert.True(containers1.Any());
        }
        [Xunit.Fact]
        public void GetContainerBlobIds_Test()
        {
            var blobIds = BlobUtil.GetContainerBlobIds(TestTenant, userId);
            if (blobIds.Any())
            {
                _logger.LogInformation(string.Format("DevDb[{0}] containers blobIds: {1}. ",
                    TestTenant.CloudType, blobIds.ToCommaSeparatedString()));
            }
            Assert.True(blobIds.Any());

            var blobIds1 = BlobUtil.GetContainerBlobIds(BuyTenant, userId);
            if (blobIds1.Any())
            {
                _logger.LogInformation(string.Format("Customer1[{0}] containers blobIds: {1}. ",
                    BuyTenant.CloudType, blobIds1.ToCommaSeparatedString()));
            }
            Assert.True(blobIds1.Any());
        }
        [Xunit.Fact]
        public void GetContainerBlobs_Test()
        {
            var blobIds = BlobUtil.GetContainerBlobs(TestTenant, userId);
            if (blobIds.Any())
            {
                _logger.LogInformation("devdb blobs is " + blobIds.Select(m => m.Id).ToCommaSeparatedString());
            }
            Assert.True(blobIds.Any());
            var blobIds1 = BlobUtil.GetContainerBlobs(BuyTenant, userId);
            if (blobIds1.Any())
            {
                _logger.LogInformation("Customer1 blobs is " + blobIds1.Select(m => m.Id).ToCommaSeparatedString());
            }
            Assert.True(blobIds.Any());
        }
        [Xunit.Fact]
        public void GetBlobById_Test()
        {
            //blobId = "ef35812e-a2aa-4e51-9186-624e9d1f9a0b";
            var blob = BlobUtil.GetBlobById(TestTenant, userId, blobId);
            Assert.True(blob.Data.Any());

            using (var m = new MemoryStream(blob.Data))
            {
                var filePath = @"File\";
                var downfile = filePath + "downloadImage.png";
                using (var fs = new FileStream(downfile, FileMode.OpenOrCreate))
                {
                    m.WriteTo(fs);
                }
            }
        }
        [Xunit.Fact]
        public void RemoveBlob_Test()
        {
            //blobId = "ef35812e-a2aa-4e51-9186-624e9d1f9a0b";
            var removeResult = BlobUtil.RemoveBlob(TestTenant, userId, blobId);
            Assert.True(removeResult);
        }
        #endregion


        [Xunit.Fact]
        public void Encryption_Test()
        {
            var encryptionStr = "测试加密Bytes的字符串";
            var key = "dev-cfwin-EncryptKey";
            string realEncryptionKey = Encryption.GetEncryptionKey(key, false, false, null);

            var encrypResult = Encryption.Encrypt(encryptionStr, realEncryptionKey);
            var decrypResult = Encryption.Decrypt(encrypResult, realEncryptionKey);

            _logger.LogInformation("---realEncryptionKey: " + realEncryptionKey);
            _logger.LogInformation("---encrypResult: " + encrypResult);
            _logger.LogInformation("---decrypResult: " + decrypResult);

            Assert.Equal(encryptionStr, decrypResult);
        }

        [Xunit.Fact]
        public void Test_EncryptVodPassword()
        {
            var dbaDatabasePassword = EncryptPasswordUtil.DecryptPassword(DbaTenant.DatabasePasswordHash,
                DbaTenant.PrivateEncryptKey);
            Assert.True(!string.IsNullOrEmpty(dbaDatabasePassword));
            _logger.LogInformation("----Dba's Database password: " + dbaDatabasePassword);
            var dbaStorageConnect = EncryptPasswordUtil.DecryptPassword(DbaTenant.StorageAccessKeyPasswordHash,
                DbaTenant.PrivateEncryptKey);
            Assert.True(!string.IsNullOrEmpty(dbaStorageConnect));
            _logger.LogInformation("----Dba's Storage connect string: " + dbaStorageConnect);
            var dbaQueueConnect = EncryptPasswordUtil.DecryptPassword(DbaTenant.QueueAccessKeyPasswordHash,
                DbaTenant.PrivateEncryptKey);
            Assert.True(!string.IsNullOrEmpty(dbaQueueConnect));
            _logger.LogInformation("----Dba's Queue connect string: " + dbaQueueConnect);
            var dbaVodConnect = EncryptPasswordUtil.DecryptPassword(DbaTenant.VodAccessKeyPasswordHash,
                DbaTenant.PrivateEncryptKey);
            Assert.True(!string.IsNullOrEmpty(dbaVodConnect));
            _logger.LogInformation("----Dba's Vod connect string: " + dbaVodConnect);

            var devdbDatabasePassword = EncryptPasswordUtil.DecryptPassword(TestTenant.DatabasePasswordHash,
                TestTenant.PrivateEncryptKey);
            Assert.True(!string.IsNullOrEmpty(devdbDatabasePassword));
            _logger.LogInformation("----DevDb's Database password: " + devdbDatabasePassword);
            var devdbStorageConnect = EncryptPasswordUtil.DecryptPassword(TestTenant.StorageAccessKeyPasswordHash,
                TestTenant.PrivateEncryptKey);
            Assert.True(!string.IsNullOrEmpty(devdbStorageConnect));
            _logger.LogInformation("----DevDb's Storage connect string: " + devdbStorageConnect);
            var devdbQueueConnect = EncryptPasswordUtil.DecryptPassword(TestTenant.QueueAccessKeyPasswordHash,
                TestTenant.PrivateEncryptKey);
            Assert.True(!string.IsNullOrEmpty(devdbQueueConnect));
            _logger.LogInformation("----DevDb's Queue connect string: " + devdbQueueConnect);
            var devdbVodConnect = EncryptPasswordUtil.DecryptPassword(TestTenant.VodAccessKeyPasswordHash,
                TestTenant.PrivateEncryptKey);
            Assert.True(!string.IsNullOrEmpty(devdbVodConnect));
            _logger.LogInformation("----DevDb's Vod connect string: " + devdbVodConnect);

            var BuyDatabasePassword = EncryptPasswordUtil.DecryptPassword(BuyTenant.DatabasePasswordHash,
                BuyTenant.PrivateEncryptKey);
            Assert.True(!string.IsNullOrEmpty(BuyDatabasePassword));
            _logger.LogInformation("----Buy's Database password: " + BuyDatabasePassword);
            var BuyStorageConnect = EncryptPasswordUtil.DecryptPassword(BuyTenant.StorageAccessKeyPasswordHash,
                BuyTenant.PrivateEncryptKey);
            Assert.True(!string.IsNullOrEmpty(BuyStorageConnect));
            _logger.LogInformation("----Buy's Storage connect string: " + BuyStorageConnect);
            var BuyQueueConnect = EncryptPasswordUtil.DecryptPassword(BuyTenant.QueueAccessKeyPasswordHash,
                BuyTenant.PrivateEncryptKey);
            Assert.True(!string.IsNullOrEmpty(BuyQueueConnect));
            _logger.LogInformation("----Buy's Queue connect string: " + BuyQueueConnect);
            var BuyVodConnect = EncryptPasswordUtil.DecryptPassword(BuyTenant.VodAccessKeyPasswordHash,
                BuyTenant.PrivateEncryptKey);
            Assert.True(!string.IsNullOrEmpty(BuyVodConnect));
            _logger.LogInformation("----Buy's Vod connect string: " + BuyVodConnect);
        }

        [Xunit.Fact]
        public void Test_GetVodPasswordHash()
        {
            var dbaPasswrod = "NN7WGhMmxgtedCLBwyayXr9m2v1NXZ";
            var dbaDatabasePassword = EncryptPasswordUtil.EncryptPassword(dbaPasswrod,
                DbaTenant.PrivateEncryptKey);
            Assert.True(!string.IsNullOrEmpty(dbaDatabasePassword));
            _logger.LogInformation("----Dba's Database password hash: " + dbaDatabasePassword);

            var testPasswrod = "NN7WGhMmxgtedCLBwyayXr9m2v1NXZ";
            var devdbDatabasePassword = EncryptPasswordUtil.EncryptPassword(testPasswrod,
                TestTenant.PrivateEncryptKey);
            Assert.True(!string.IsNullOrEmpty(devdbDatabasePassword));
            _logger.LogInformation("----DevDb's Database password: " + devdbDatabasePassword);

            var buyPasswrod = "NN7WGhMmxgtedCLBwyayXr9m2v1NXZ";
            var BuyDatabasePassword = EncryptPasswordUtil.EncryptPassword(buyPasswrod,
                BuyTenant.PrivateEncryptKey);
            Assert.True(!string.IsNullOrEmpty(BuyDatabasePassword));
            _logger.LogInformation("----Buy's Database password: " + BuyDatabasePassword);

            var salePasswrod = "NN7WGhMmxgtedCLBwyayXr9m2v1NXZ";
            var SaleDatabasePassword = EncryptPasswordUtil.EncryptPassword(salePasswrod,
                SaleTenant.PrivateEncryptKey);
            Assert.True(!string.IsNullOrEmpty(SaleDatabasePassword));
            _logger.LogInformation("----Buy's Database password: " + SaleDatabasePassword);

        }
    }
}
