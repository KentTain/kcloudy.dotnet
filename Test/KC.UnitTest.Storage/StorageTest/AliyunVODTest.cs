using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using KC.Framework.Tenant;
using KC.Framework.Extension;
using System.Text;
using Aliyun.OSS;
using System.IO;
using Aliyun.OSS.Common;
using KC.Common.FileHelper;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.vod.Model.V20170321;
using ClientException = Aliyun.Acs.Core.Exceptions.ClientException;
using Aliyun.Acs.Core.Profile;
using KC.Framework.SecurityHelper;
using KC.Framework.Base;
using KC.Common;
using KC.VOD;
using Aliyun.Acs.Core.Http;

namespace KC.UnitTest.Storage.Aliyun
{
    public class AliyunVODTest : KC.UnitTest.Storage.StorageTestBase
    {
        private static string fileType = FileType.Image.ToString();
        private static string fileFormat = ImageFormat.Png.ToString();

        private static string userId = RoleConstants.AdminUserId.ToLower();
        private static ILogger _logger;
        public AliyunVODTest(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(AliyunVODTest));
        }

        private string GetVodConnectionString(Tenant tenant)
        {
            try
            {
                return
                string.Format(
                    @"BlobEndpoint={0};AccountName={1};AccountKey={2}",
                    tenant.VodEndpoint, tenant.VodAccessName,
                    EncryptPasswordUtil.DecryptPassword(tenant.VodAccessKeyPasswordHash, tenant.PrivateEncryptKey));
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        [Xunit.Fact]
        public void Test_File_VideoSetting()
        {
            var blobId = "file-blobId-video";
            var blobName = "UploadVideo.mp4";
            var filePath = @"File\UploadVideo.mp4";
            var downloadFileName = @"File\downloadVideo.mp4";

            //获取上传凭证
            var videoAuth = VodUtil.GetVideoSetting(TestTenant, blobName);
            var requestId = videoAuth.RequestId;
            var videoId = videoAuth.VideoId;
            var endpoint = videoAuth.UploadEndpoint.Replace("https", "http");
            var accessKey = videoAuth.AccessKeyId;
            var accessSecret = videoAuth.AccessKeySecret;
            var sts = videoAuth.SecurityToken;
            var bucket = videoAuth.UploadBucket;
            var fileName = videoAuth.UploadFileName;
            _logger.LogInformation(string.Format("---Bucket:【{0}】 with FileName【{1}】 in Endpoint【{2}】 return VideoId【{3}】 ", bucket, fileName, endpoint, videoId));

            UploadToOSS(endpoint, accessKey, accessSecret, sts, bucket, fileName, filePath);
        }

        [Xunit.Fact]
        public void Test_File_Video()
        {
            var blobId = "file-blobId-video";
            var blobName = "UploadVideo.mp4";
            var filePath = @"File\UploadVideo.mp4";
            var downloadFileName = @"File\downloadVideo.mp4";

            //上传视频
            byte[] fileBytes = GetFileBytes(filePath);
            var resultVideoId = VodUtil.UploadVideo(TestTenant, blobId, "mp4", fileBytes, null);

            //刷新凭证
            var videoAuth = VodUtil.RefreshVideoSetting(TestTenant, resultVideoId);
            _logger.LogInformation(string.Format("---Refresh Bucket:【{0}】 with FileName【{1}】 in Endpoint【{2}】 return VideoId【{3}】 ", videoAuth.UploadBucket, videoAuth.UploadFileName, videoAuth.UploadEndpoint, videoAuth.VideoId));

            //获取视频播放地址
            var dictFile = VodUtil.GetVideoPlayAddress(TestTenant, resultVideoId);
            foreach(var keyPair in dictFile)
            {
                _logger.LogInformation(string.Format("---get play url【{0}】", keyPair));
            }

        }

        #region VOD
        private static DefaultAcsClient GetAliyunVodClient()
        {
            // 点播服务接入区域
            var regionId = "cn-shanghai";
            //var accessKey = "LTAI4G3LCfpphDx3pLBgbHmY";
            //var accessSecret = "Ov8acS2cQACMYZSJ9sDWcwVgTvvQP6";
            var accessKey = "LTAI4GBeijVm43GrG42P1AqZ";
            var accessSecret = "NN7WGhMmxgtedCLBwyayXr9m2v1NXZ";
            //var accessKey = "LTAI4FnNnNq65N2TYAM7TGHQ";
            //var accessSecret = "NIhak9qyCDMIlTnsrIspQGjInapwHs";
            IClientProfile profile = DefaultProfile.GetProfile(regionId, accessKey, accessSecret);
            // DefaultProfile.AddEndpoint(regionId, regionId, "vod", "vod." + regionId + ".aliyuncs.com");
            return new DefaultAcsClient(profile);
        }

        /// <summary>
        /// 获取视频上传地址和凭证
        /// </summary>
        /// <returns></returns>
        private VodUploadSetting GetVideoAuthAddress(string blobId)
        {
            VodUploadSetting result = null;
            try
            {
                // 构造请求
                var request = new CreateUploadVideoRequest();
                request.Title = blobId;
                request.FileName = blobId;
                request.AcceptFormat = FormatType.JSON;
                //设置请求超时时间
                //request.SetReadTimeoutInMilliSeconds(1000);
                //request.SetConnectTimeoutInMilliSeconds(1000);

                var client = GetAliyunVodClient();
                // 发起请求，并得到响应结果
                var response = client.GetAcsResponse(request);
                var addressString = Base64Provider.DecodeString(response.UploadAddress);
                var authString = Base64Provider.DecodeString(response.UploadAuth);
                var address = SerializeHelper.FromJson<UploadAddress>(addressString);
                var auth = SerializeHelper.FromJson<UploadAuth>(authString);

                result = new VodUploadSetting();
                result.RequestId = response.RequestId;
                result.VideoId = response.VideoId;
                result.UploadEndpoint = address?.Endpoint;
                result.UploadBucket = address?.Bucket;
                result.UploadFileName = address?.FileName;
                result.AccessKeyId = auth?.AccessKeyId;
                result.AccessKeySecret = auth?.AccessKeySecret;
                result.SecurityToken = auth?.SecurityToken;
                result.Expiration = auth != null ? auth.Expiration : new TimeSpan(0);
                return result;
            }
            catch (ServerException ex)
            {
                _logger.LogError(ex.ToString());
            }
            catch (ClientException ex)
            {
                _logger.LogError(ex.ToString());
            }

            return result;
        }

        /// <summary>
        /// 刷新视频上传凭证
        /// </summary>
        /// <returns></returns>
        private VodUploadSetting RefreshVideoAuth(string voidId)
        {
            VodUploadSetting result = null;
            try
            {
                // 构造请求
                var request = new RefreshUploadVideoRequest();
                request.VideoId = voidId;

                var client = GetAliyunVodClient();
                // 发起请求，并得到响应结果
                var response = client.GetAcsResponse(request);
                var addressString = Base64Provider.DecodeString(response.UploadAddress);
                var authString = Base64Provider.DecodeString(response.UploadAuth);
                var address = SerializeHelper.FromJson<UploadAddress>(addressString);
                var auth = SerializeHelper.FromJson<UploadAuth>(authString);

                result = new VodUploadSetting();
                result.RequestId = response.RequestId;
                result.VideoId = response.VideoId;
                result.UploadEndpoint = address?.Endpoint;
                result.UploadBucket = address?.Bucket;
                result.UploadFileName = address?.FileName;
                result.AccessKeyId = auth?.AccessKeyId;
                result.AccessKeySecret = auth?.AccessKeySecret;
                result.SecurityToken = auth?.SecurityToken;
                result.Expiration = auth != null ? auth.Expiration : new TimeSpan(0);

                return result;
            }
            catch (ServerException ex)
            {
                _logger.LogError(ex.ToString());
            }
            catch (ClientException ex)
            {
                _logger.LogError(ex.ToString());
            }

            return result;
        }


        /// <summary>
        /// 获取播放地址
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetVideoPlayAddresses(string voidId)
        {
            var result  = new Dictionary<string, string>();
            try
            {
                // 构造请求
                var request = new GetPlayInfoRequest();
                request.VideoId = voidId;
                // request.AuthTimeout = 3600;

                var client = GetAliyunVodClient();
                // 发起请求，并得到响应结果
                var response = client.GetAcsResponse(request);
                var playInfoList = response.PlayInfoList;
                foreach (var playInfo in response.PlayInfoList)
                {
                    result.Add(playInfo.JobId, playInfo.PlayURL);
                }
                return result;
            }
            catch (ServerException ex)
            {
                _logger.LogError(ex.ToString());
            }
            catch (ClientException ex)
            {
                _logger.LogError(ex.ToString());
            }

            return result;
        }

        /// <summary>
        /// 获取视频播放凭证
        /// </summary>
        /// <returns></returns>
        private GetVideoPlayAuthResponse GetVideoPlayAuth(string voidId)
        {
            try
            {
                // 构造请求
                var request = new GetVideoPlayAuthRequest();
                request.VideoId = voidId;
                //request.AuthInfoTimeout = 3000;

                var client = GetAliyunVodClient();
                // 发起请求，并得到响应结果
                var response = client.GetAcsResponse(request);
                return response;
            }
            catch (ServerException ex)
            {
                _logger.LogError(ex.ToString());
            }
            catch (ClientException ex)
            {
                _logger.LogError(ex.ToString());
            }

            return null;
        }

        /// <summary>
        /// 获取图片上传地址和凭证
        /// </summary>
        /// <returns></returns>
        private CreateUploadImageResponse GetImageAuthAddress()
        {
            try
            {
                // 构造请求
                var request = new CreateUploadImageRequest();
                request.ImageType = "cover";
                request.ImageExt = "jpg";

                var client = GetAliyunVodClient();
                // 发起请求，并得到响应结果
                var response = client.GetAcsResponse(request);
                _logger.LogInformation("RequestId = " + response.RequestId);
                _logger.LogInformation("ImageId = " + response.ImageId);
                _logger.LogInformation("ImageUrl = " + response.ImageURL);
                _logger.LogInformation("UploadAddress = " + response.UploadAddress);
                _logger.LogInformation("UploadAuth = " + response.UploadAuth);
                return response;
            }
            catch (ServerException ex)
            {
                _logger.LogError(ex.ToString());
            }
            catch (ClientException ex)
            {
                _logger.LogError(ex.ToString());
            }

            return null;
        }

        /// <summary>
        /// 获取辅助媒资上传地址和凭证
        /// </summary>
        /// <returns></returns>
        private CreateUploadAttachedMediaResponse GetAttachedMediaAuthAddress()
        {
            try
            {
                // 构造请求
                var request = new CreateUploadAttachedMediaRequest();
                request.BusinessType = "watermark";
                request.MediaExt = "gif";
                request.Title = "this is a sample";

                var client = GetAliyunVodClient();
                // 发起请求，并得到响应结果
                var response = client.GetAcsResponse(request);
                _logger.LogInformation("RequestId = " + response.RequestId);
                _logger.LogInformation("MediaId = " + response.MediaId);
                _logger.LogInformation("MediaURL = " + response.MediaURL);
                _logger.LogInformation("UploadAddress = " + response.UploadAddress);
                _logger.LogInformation("UploadAuth = " + response.UploadAuth);
                return response;
            }
            catch (ServerException ex)
            {
                _logger.LogError(ex.ToString());
            }
            catch (ClientException ex)
            {
                _logger.LogError(ex.ToString());
            }

            return null;
        }


        private class UploadAddress
        {
            public string Bucket { get; set; }
            public string Endpoint { get; set; }
            public string FileName { get; set; }
        }

        private class UploadAuth
        {
            public string AccessKeyId { get; set; }
            public string AccessKeySecret { get; set; }
            public string SecurityToken { get; set; }
            public TimeSpan Expiration { get; set; }
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

        // 创建OssClient实例。
        protected OssClient GetAliyunOSSClient(string endpoint, string accessKey, string accessSecret, string sts)
        {
            var client = new OssClient(endpoint,
                accessKey, accessSecret, sts);
            return client;
        }

        private void UploadToOSS(string endpoint, string accessKey, string accessSecret, string sts, string bucket, string blobId, string filePath)
        {
            var client = GetAliyunOSSClient(endpoint, accessKey, accessSecret, sts);
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
    }
}
