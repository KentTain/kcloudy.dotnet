using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Util;
using KC.Framework.Extension;
using KC.Framework.Base;
using KC.Framework.Tenant;
using Aliyun.OSS;
using Aliyun.OSS.Common;

namespace KC.Storage.BlobService
{
    internal class AliyunOSSReader : ReaderBase
    {
        //private readonly AmazonS3Config Config;
        //private readonly AWSCredentials Credentials;
        //private readonly IAmazonS3 client;
        private readonly BlobCache Cache;
        private readonly string endpoint;
        private readonly string accessKey;
        private readonly string secretKey;

        private readonly string preBucket = "kcloudy-aliyun-storage-";

        public AliyunOSSReader(string connectionString, BlobCache cache)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Aliyun OSS's NAS reader connect string is empy or null.", "connectionString");

            var keyValues = connectionString.KeyValuePairFromConnectionString();
            endpoint = keyValues[ConnectionKeyConstant.BlobEndpoint];
            accessKey = keyValues[ConnectionKeyConstant.AccessName];
            secretKey = keyValues[ConnectionKeyConstant.AccessKey];
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentException("Aliyun OSS's NAS reader connect string is wrong. It can't set the BlobEndpoint Value.", "connectionString");
            if (string.IsNullOrWhiteSpace(accessKey))
                throw new ArgumentException("Aliyun OSS's NAS reader connect string is wrong. It can't set the AccessKey Value.", "connectionString");
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new ArgumentException("Aliyun OSS's NAS reader connect string is wrong. It can't set the SecretAccountKey Value.", "connectionString");

            this.Cache = cache;
        }

        protected OssClient GetAliyunOSSClient()
        {
            var config = new ClientConfiguration();
            config.MaxErrorRetry = 3;
            //config.ConnectionTimeout = 5000;
            // 设置是否支持CNAME。CNAME是指将自定义域名绑定到存储空间上。
            config.IsCname = false;
            var client = new OssClient(endpoint, accessKey, secretKey, config);
            return client;
        } 

        public override bool ExistContainer(string container)
        {
            string bucket = preBucket + container; 
            return ListContainers().Contains(bucket);
        }

        public override bool ExistBlob(string container, string blobId)
        {
            string bucket = preBucket + container;
            try
            {
                OssClient client = GetAliyunOSSClient();
                return client.DoesObjectExist(bucket, blobId);
            }
            catch (OssException ex)
            {
                LogUtil.LogError(string.Format("ExistBlob failed with error info: {0}; Error info: {1}. " + Environment.NewLine + "RequestID:{2}" + Environment.NewLine + "HostID:{3}",
                                  ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId));
                return false;
            }
        }

        public override List<string> ListContainers()
        {
            try
            {
                OssClient client = GetAliyunOSSClient();
                var buckets = client.ListBuckets();
                return buckets.Select(m => m.Name).ToList();
            }
            catch (OssException ex)
            {
                LogUtil.LogError(string.Format("ListContainers failed with error info: {0}; Error info: {1}. " + Environment.NewLine + "RequestID:{2}" + Environment.NewLine + "HostID:{3}",
                                  ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId));
                return null;
            }
        }

        public override List<string> ListBlobIds(string container)
        {
            string bucket = preBucket + container;
            try
            {
                OssClient client = GetAliyunOSSClient();
                var response = client.ListObjects(bucket);
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    LogUtil.LogError(
                        string.Format(
                            "Aliyun OSS get all container's blobIds is failed. the response HttpStatusCode is {0}. ",
                            response.HttpStatusCode));
                    return null;
                }
                return response.ObjectSummaries.Select(m => m.Key).ToList();
            }
            catch (OssException ex)
            {
                LogUtil.LogError(string.Format("ListContainers failed with error info: {0}; Error info: {1}. " + Environment.NewLine + "RequestID:{2}" + Environment.NewLine + "HostID:{3}",
                                  ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId));
                return null;
            }
        }

        public override List<string> ListBlobIdsWithMetadata(string container)
        {
            return ListBlobIds(container);
        }

        public override BlobInfo GetBlob(string container, string encryptionKey, bool isInternal, string userId, string blobId, bool useCacheIfAvailable)
        {
            if (string.IsNullOrEmpty(blobId) || string.IsNullOrEmpty(container)) return null;

            var sw = new Stopwatch();
            sw.Start();

            string bucket = preBucket + container;
            int offset = -1;
            int length = -1;
            string realBlobId = GetActualBlobIdAndOffset(blobId, out offset, out length);
            if (useCacheIfAvailable && this.Cache != null)
            {
                BlobInfo bi = this.Cache.GetBlob(bucket, encryptionKey, isInternal, userId, realBlobId);
                //LogUtil.LogDebug("Retrieve Blob from local cache store. ", blobId, sw.ElapsedMilliseconds);

                if (offset != -1)
                {
                    bi = GetBlobSegment(bi, offset, length, blobId);
                }
                return bi;
            }
            else
            {
                byte[] cacheData, cacheMetadata;
                BlobInfo bi = GetBlob(container, encryptionKey, isInternal, userId, realBlobId,
                    useCacheIfAvailable, out cacheData, out cacheMetadata);
                //LogUtil.LogDebug("Retrieve Blob from Aliyun OSS store. ", blobId, sw.ElapsedMilliseconds);

                if (useCacheIfAvailable && this.Cache != null && cacheData != null)
                {
                    this.Cache.Insert(bucket, realBlobId, cacheData, cacheMetadata);
                }
                if (offset != -1)
                {
                    bi = GetBlobSegment(bi, offset, length, blobId);
                }
                return bi;
            }
        }

        public override BlobInfo GetBlobWithoutEncryption(string container, string blobId, bool useCacheIfAvailable)
        {
            if (string.IsNullOrEmpty(blobId) || string.IsNullOrEmpty(container)) return null;

            var sw = new Stopwatch();
            sw.Start();

            string bucket = preBucket + container;
            int offset = -1;
            int length = -1;
            string realBlobId = GetActualBlobIdAndOffset(blobId, out offset, out length);
            if (useCacheIfAvailable && this.Cache != null)
            {
                BlobInfo bi = this.Cache.GetBlobWithoutEncryption(bucket, realBlobId);
                //LogUtil.LogDebug("Retrieve Blob from local cache store. ", blobId, sw.ElapsedMilliseconds);
                if (offset != -1)
                {
                    bi = GetBlobSegment(bi, offset, length, blobId);
                }
                return bi;
            }
            else
            {
                OssClient client = GetAliyunOSSClient();
                {
                    var response = client.GetObject(bucket, blobId);
                    if (response.HttpStatusCode != HttpStatusCode.OK)
                    {
                        LogUtil.LogError(
                            string.Format(
                                "The Aliyun OSS get container({0})'s blob({1}) is failed. the response HttpStatusCode is {2}. ",
                                bucket, blobId, response.HttpStatusCode));
                        return null;
                    }

                    try
                    {
                        byte[] data;
                        using (var stream = response.Content)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                stream.CopyTo(memoryStream);
                                data = memoryStream.GetBuffer();
                            }

                            //var bytes = new byte[stream.Length];
                            //stream.Read(bytes, 0, bytes.Length);

                            //data = bytes;
                        }

                        var blobMetadata = new Dictionary<string, string>();
                        if (response.Metadata != null && response.Metadata.UserMetadata != null)
                        {
                            foreach (var key in response.Metadata.UserMetadata.Keys)
                            {
                                blobMetadata.Add(key, response.Metadata.UserMetadata[key]);
                            }
                        }

                        string type = blobMetadata.ContainsKey("type") ? blobMetadata["type"] : "Unknown";
                        string format = blobMetadata.ContainsKey("format") ? blobMetadata["format"] : "Unknown";
                        string fileName = blobMetadata.ContainsKey("filename") ? blobMetadata["filename"] : "Unknown";
                        string ext = blobMetadata.ContainsKey("ext") ? blobMetadata["ext"] : "Unknown";
                        var blobInfo = new BlobInfo(blobId, type, format, fileName, ext, data);
                        blobInfo.Size = blobInfo.Data.Length;
                        //LogUtil.LogDebug("Retrieve Blob from Aliyun OSS store. ", blobId, sw.ElapsedMilliseconds);

                        if (useCacheIfAvailable && this.Cache != null)
                        {
                            this.Cache.InsertWithoutEncryption(bucket, blobId, blobInfo.Data);
                        }
                        if (offset != -1)
                        {
                            blobInfo = GetBlobSegment(blobInfo, offset, length, blobId);
                        }
                        return blobInfo;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
            }
        }

        public override DateTime GetBlobLastModifiedTime(string container, string blobId)
        {
            try
            {
                var metadata = GetBlobMetadata(container, blobId);

                DateTime result;
                if (DateTime.TryParse(metadata["LastModifiedTime"], out result))
                {
                    return result;
                }

                return DateTime.MinValue;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public override Dictionary<string, string> GetBlobMetadata(string container, string blobId)
        {
            string bucket = preBucket + container;
            try
            {
                OssClient client = GetAliyunOSSClient();
                var response = client.GetObjectMetadata(bucket, blobId);
                var blobMetadata = new Dictionary<string, string>();
                if (response.UserMetadata != null)
                {
                    foreach (var key in response.UserMetadata.Keys)
                    {
                        blobMetadata.Add(key, response.UserMetadata[key]);
                    }
                }
                    

                return blobMetadata;
            }
            catch (OssException ex)
            {
                LogUtil.LogError(string.Format("GetBlobMetadata failed with error info: {0}; Error info: {1}. " + Environment.NewLine + "RequestID:{2}" + Environment.NewLine + "HostID:{3}",
                                  ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId));
                return null;
            }
        }

        private BlobInfo GetBlob(string container, string encryptionKey, bool isInternal, string userId, string blobId, bool generateCacheData, out byte[] cacheData, out byte[] cacheMetadata)
        {
            cacheData = null;
            cacheMetadata = null;

            if (string.IsNullOrEmpty(blobId))
                return null;

            string bucket = preBucket + container;
            try
            {
                OssClient client = GetAliyunOSSClient();
                using (var response = client.GetObject(bucket, blobId))
                {
                    if (response.HttpStatusCode != HttpStatusCode.OK)
                    {
                        LogUtil.LogError(
                            string.Format(
                                "The Aliyun OSS get container({0})'s blob({1}) is failed. the response HttpStatusCode is {2}.",
                                bucket, blobId, response.HttpStatusCode));
                        return null;
                    }

                    var blobMetadata = new Dictionary<string, string>();
                    if (response.Metadata != null && response.Metadata.UserMetadata != null)
                    {
                        foreach (var key in response.Metadata.UserMetadata.Keys)
                        {
                            blobMetadata.Add(key, response.Metadata.UserMetadata[key]);
                        }
                    }

                    CheckBlobAccess(bucket, isInternal, userId, blobId, blobMetadata);

                    byte[] data;
                    using (var stream = response.Content)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            stream.CopyTo(memoryStream);
                            data = memoryStream.GetBuffer();
                        }

                        //var bytes = new byte[stream.Length];
                        //stream.Read(bytes, 0, bytes.Length);

                        //data = bytes;
                    }

                    // Store Cache Data
                    if (generateCacheData)
                    {
                        cacheData = new byte[data.Length];
                        Buffer.BlockCopy(data, 0, cacheData, 0, data.Length);

                        string metadataXml = MetadataToXml(blobMetadata);
                        cacheMetadata = Encoding.UTF8.GetBytes(metadataXml);
                    }
                    else
                    {
                        cacheData = null;
                        cacheMetadata = null;
                    }

                    string type = blobMetadata.ContainsKey("type") ? blobMetadata["type"] : "Unknown";
                    string format = blobMetadata.ContainsKey("format") ? blobMetadata["format"] : "Unknown";
                    string fileName = blobMetadata.ContainsKey("filename") ? blobMetadata["filename"] : "Unknown";
                    string ext = blobMetadata.ContainsKey("ext") ? blobMetadata["ext"] : "Unknown";
                    byte[] blobActualData = DecryptIfNeeded(encryptionKey, isInternal, userId, data, blobMetadata);
                    var bi = new BlobInfo(blobId, type, format, fileName, ext, blobActualData);
                    bi.Size = bi.Data.Length;
                    return bi;
                }
            }
            catch (OssException ex)
            {
                LogUtil.LogError(string.Format("GetBlob failed with error info: {0}; Error info: {1}. " + Environment.NewLine + "RequestID:{2}" + Environment.NewLine + "HostID:{3}",
                                  ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId));
                return null;
            }
        }
    }
}
