using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.vod.Model.V20170321;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using KC.Common;
using KC.Common.FileHelper;
using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.SecurityHelper;
using KC.Framework.Tenant;
using KC.Framework.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace KC.VOD.VodService
{
    public class AliyunVodProvider : VodProviderBase, IVodProvider
    {
        private readonly string StorageConnectionString;
        private readonly string endpoint;
        private readonly string accessKey;
        private readonly string secretKey;

        public AliyunVodProvider(Tenant tenant)
            : this(tenant.GetVodConnectionString())
        {
        }
        public AliyunVodProvider(string connectionString)
        {
            this.StorageConnectionString = connectionString;
            try
            {
                if (string.IsNullOrWhiteSpace(this.StorageConnectionString))
                    throw new ArgumentException("Aliyun Vod's NAS reader connect string is empy or null.", "connectionString");

                var keyValues = this.StorageConnectionString.KeyValuePairFromConnectionString();
                endpoint = keyValues[ConnectionKeyConstant.VodEndpoint];
                accessKey = keyValues[ConnectionKeyConstant.AccessName];
                secretKey = keyValues[ConnectionKeyConstant.AccessKey];
                if (string.IsNullOrWhiteSpace(endpoint))
                    throw new ArgumentException("Aliyun Vod's NAS reader connect string is wrong. It can't set the BlobEndpoint Value.", "connectionString");
                if (string.IsNullOrWhiteSpace(accessKey))
                    throw new ArgumentException("Aliyun Vod's NAS reader connect string is wrong. It can't set the AccessKey Value.", "connectionString");
                if (string.IsNullOrWhiteSpace(secretKey))
                    throw new ArgumentException("Aliyun Vod's NAS reader connect string is wrong. It can't set the SecretAccountKey Value.", "connectionString");
            }
            catch (Exception ex)
            {
                LogUtil.LogError("Initilize the Aliyun OSS Provider connection throw error. " + ex.Message);
            }
        }

        private DefaultAcsClient GetAliyunVodClient()
        {
            // 点播服务接入区域
            var profile = DefaultProfile.GetProfile(endpoint, accessKey, secretKey);
            // DefaultProfile.AddEndpoint(regionId, regionId, "vod", "vod." + regionId + ".aliyuncs.com");
            return new DefaultAcsClient(profile);
        }

        /// <summary>
        /// 获取视频上传地址和凭证
        /// </summary>
        /// <returns></returns>
        public override VodUploadSetting GetVideoSetting(string blobId)
        {
            VodUploadSetting result = null;
            try
            {
                // 构造请求
                var request = new CreateUploadVideoRequest();
                request.Title = blobId;
                request.FileName = blobId;
                request.AcceptFormat = Aliyun.Acs.Core.Http.FormatType.JSON;
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
                LogUtil.LogError(ex.ToString());
            }
            catch (Aliyun.Acs.Core.Exceptions.ClientException ex)
            {
                LogUtil.LogError(ex.ToString());
            }

            return result;
        }
        /// <summary>
        /// 刷新视频上传凭证
        /// </summary>
        /// <returns></returns>
        public override VodUploadSetting RefreshVideoSetting(string videoId)
        {
            VodUploadSetting result = null;
            try
            {
                // 构造请求
                var request = new RefreshUploadVideoRequest();
                request.VideoId = videoId;

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
                LogUtil.LogError(ex.ToString());
            }
            catch (Aliyun.Acs.Core.Exceptions.ClientException ex)
            {
                LogUtil.LogError(ex.ToString());
            }

            return result;
        }

        /// <summary>
        /// 上传视频
        /// </summary>
        /// <param name="blobId">文件BlobId</param>
        /// <param name="ext">文件扩展名</param>
        /// <param name="blobData">文件二进制流</param>
        /// <param name="metadata">文件元数据</param>
        /// <returns>返回视频Id，用于获取视频信息</returns>
        public override string UploadVideo(string blobId, string ext, byte[] blobData, Dictionary<string, string> metadata)
        {
            try
            {
                //获取上传凭证
                var videoAuth = GetVideoSetting(blobId + "." + ext);
                var requestId = videoAuth.RequestId;
                var videoId = videoAuth.VideoId;
                var endpoint = videoAuth.UploadEndpoint.Replace("https", "http");
                var accessKey = videoAuth.AccessKeyId;
                var accessSecret = videoAuth.AccessKeySecret;
                var securityToken = videoAuth.SecurityToken;
                var bucket = videoAuth.UploadBucket;
                var ufileName = videoAuth.UploadFileName;
                LogUtil.LogDebug(string.Format("---Bucket:【{0}】 with FileName【{1}】 in Endpoint【{2}】 return VideoId【{3}】 ", bucket, ufileName, endpoint, videoId));

                //文件元信息
                var objectMetadata = new ObjectMetadata();
                if (metadata != null)
                {
                    foreach (string metadataKey in metadata.Keys)
                    {
                        objectMetadata.UserMetadata[metadataKey] = metadata[metadataKey];
                    }
                }

                // 上传文件并设置文件元信息
                var ossClient = new OssClient(endpoint, accessKey, accessSecret, securityToken);
                var response = ossClient.PutObject(bucket, ufileName, new MemoryStream(blobData), objectMetadata);
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    LogUtil.LogError(
                        string.Format(
                            "Aliyun OSS WriteBlob the container({0}) blob({1}) is failed. the response HttpStatusCode is {2}. ",
                            bucket, blobId, response.HttpStatusCode));
                }

                return response.HttpStatusCode == HttpStatusCode.OK ? videoId : null;
            }
            catch (OssException ex)
            {
                LogUtil.LogError(string.Format("UploadVideo failed with error info: {0}; Error info: {1}. " + Environment.NewLine + "RequestID:{2}" + Environment.NewLine + "HostID:{3}",
                                  ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId));

                return null;
            }
        }

        /// <summary>
        /// 获取视频播放地址
        /// </summary>
        /// <param name="videoId">上传后返回的视频Id</param>
        /// <returns></returns>
        public override List<string> GetVideoPlayAddress(string videoId)
        {
            var result = new List<string>();
            try
            {
                // 构造请求
                var request = new GetPlayInfoRequest();
                request.VideoId = videoId;
                // request.AuthTimeout = 3600;

                var client = GetAliyunVodClient();
                // 发起请求，并得到响应结果
                var response = client.GetAcsResponse(request);
                var playInfoList = response.PlayInfoList;
                foreach (var playInfo in response.PlayInfoList)
                {
                    result.Add(playInfo.PlayURL);
                }
                return result;
            }
            catch (ServerException ex)
            {
                LogUtil.LogError(ex.ToString());
            }
            catch (Aliyun.Acs.Core.Exceptions.ClientException ex)
            {
                LogUtil.LogError(ex.ToString());
            }

            return result;
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
    }

}
