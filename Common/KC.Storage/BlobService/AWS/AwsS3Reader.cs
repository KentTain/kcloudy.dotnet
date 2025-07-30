using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using KC.Framework.Util;
using KC.Framework.Extension;
using KC.Framework.Base;
using KC.Framework.Tenant;

namespace KC.Storage.BlobService
{
    internal class AwsS3Reader : ReaderBase
    {
        //private readonly AmazonS3Config Config;
        //private readonly AWSCredentials Credentials;
        //private readonly IAmazonS3 client;
        private readonly BlobCache Cache;
        private readonly string endpoint;
        private readonly string accessKey;
        private readonly string secretKey;

        public AwsS3Reader(string connectionString, BlobCache cache)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("AWS S3's NAS reader connect string is empy or null.", "connectionString");

            var keyValues = connectionString.KeyValuePairFromConnectionString();
            endpoint = keyValues[ConnectionKeyConstant.BlobEndpoint];
            accessKey = keyValues[ConnectionKeyConstant.AccessName];
            secretKey = keyValues[ConnectionKeyConstant.AccessKey];
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentException("AWS S3's NAS reader connect string is wrong. It can't set the BlobEndpoint Value.", "connectionString");
            if (string.IsNullOrWhiteSpace(accessKey))
                throw new ArgumentException("AWS S3's NAS reader connect string is wrong. It can't set the AccessKey Value.", "connectionString");
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new ArgumentException("AWS S3's NAS reader connect string is wrong. It can't set the SecretAccountKey Value.", "connectionString");

            //var config = new AmazonS3Config()
            //{
            //    ServiceURL = endpoint,
            //    ForcePathStyle = true,
            //    SignatureVersion = "2",
            //    BufferSize = 15 * 1024 * 1024, //15M
            //    //RegionEndpoint = Amazon.RegionEndpoint.CNNorth1
            //    //RegionEndpoint = Amazon.RegionEndpoint.USEast1,
            //};
            //var credentials = new BasicAWSCredentials(accessKey, secretKey);
            //client = new AmazonS3Client(credentials, config);

            //ServicePointManager.SecurityProtocol =
            //        SecurityProtocolType.Tls
            //        | SecurityProtocolType.Tls11
            //        | SecurityProtocolType.Tls12;
            ////ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();
            //ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;

            this.Cache = cache;
        }

        protected AmazonS3Client GetAWSClient()
        {
            var config = new AmazonS3Config()
            {
                ServiceURL = endpoint,
                ForcePathStyle = true,
                SignatureVersion = "2",
                BufferSize = 15 * 1024 * 1024, //15M
                //RegionEndpoint = Amazon.RegionEndpoint.CNNorth1
                //RegionEndpoint = Amazon.RegionEndpoint.USEast1,
            };
            var credentials = new BasicAWSCredentials(accessKey, secretKey);

            ServicePointManager.SecurityProtocol =
                    SecurityProtocolType.Tls
                    | SecurityProtocolType.Tls11
                    | SecurityProtocolType.Tls12;
            //ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();
            ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;

            var client = new AmazonS3Client(credentials, config);
            return client;
        } 

        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }

        public override bool ExistContainer(string containerName)
        {
            using (IAmazonS3 client = GetAWSClient())
            {
                return client.DoesS3BucketExistAsync(containerName).Result;
            }
        }

        public override bool ExistBlob(string containerName, string blobId)
        {
            using (IAmazonS3 client = GetAWSClient())
            {
                using (var response = client.GetObjectAsync(containerName, blobId).Result)
                {
                    if (response.HttpStatusCode != HttpStatusCode.OK)
                    {
                        LogUtil.LogError(
                            string.Format(
                                "The AWS S3 get container({0})'s blob({1}) is failed. the response HttpStatusCode is {2}. The error message: {3}. ",
                                containerName, blobId, response.HttpStatusCode,
                                response.ResponseMetadata != null && response.ResponseMetadata.Metadata != null
                                    ? response.ResponseMetadata.Metadata.ToCommaSeparatedStringByFilter(m => m.Value)
                                    : string.Empty));
                        return false;
                    }

                    var stream = response.ResponseStream;
                    return stream.Length > 0;
                }
            }
        }

        public override List<string> ListContainers()
        {
            using (IAmazonS3 client = GetAWSClient())
            {
                var response = client.ListBucketsAsync().Result;
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    LogUtil.LogError(
                        string.Format(
                            "AWS S3 get all container's is failed. the response HttpStatusCode is {0}. The error message: {1}. ",
                            response.HttpStatusCode,
                            response.ResponseMetadata != null && response.ResponseMetadata.Metadata != null
                                ? response.ResponseMetadata.Metadata.ToCommaSeparatedStringByFilter(m => m.Value)
                                : string.Empty));
                    return null;
                }
                return response.Buckets.Select(m => m.BucketName).ToList();
            }
        }

        public override List<string> ListBlobIds(string containerName)
        {
            using (IAmazonS3 client = GetAWSClient())
            {
                var response = client.ListObjectsAsync(containerName).Result;
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    LogUtil.LogError(
                        string.Format(
                            "AWS S3 get all container's blobIds is failed. the response HttpStatusCode is {0}. The error message: {1}. ",
                            response.HttpStatusCode,
                            response.ResponseMetadata != null && response.ResponseMetadata.Metadata != null
                                ? response.ResponseMetadata.Metadata.ToCommaSeparatedStringByFilter(m => m.Value)
                                : string.Empty));
                    return null;
                }
                return response.S3Objects.Select(m => m.BucketName).ToList();
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

            int offset = -1;
            int length = -1;
            string realBlobId = GetActualBlobIdAndOffset(blobId, out offset, out length);
            if (useCacheIfAvailable && this.Cache != null && this.Cache.Exists(container, realBlobId))
            {
                BlobInfo bi = this.Cache.GetBlob(container, encryptionKey, isInternal, userId, realBlobId);
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
                BlobInfo bi = GetBlob(container, encryptionKey, isInternal, userId,  realBlobId,
                    useCacheIfAvailable, out cacheData, out cacheMetadata);
                //LogUtil.LogDebug("Retrieve Blob from AWS S3 store. ", blobId, sw.ElapsedMilliseconds);

                if (useCacheIfAvailable && this.Cache != null && cacheData != null)
                {
                    this.Cache.Insert(container, realBlobId, cacheData, cacheMetadata);
                }
                if (offset != -1)
                {
                    bi = GetBlobSegment(bi, offset, length, blobId);
                }
                return bi;
            }
        }

        public override BlobInfo GetBlobWithoutEncryption(string containerName, string blobId, bool useCacheIfAvailable)
        {
            if (string.IsNullOrEmpty(blobId) || string.IsNullOrEmpty(containerName)) return null;

            var sw = new Stopwatch();
            sw.Start();

            int offset = -1;
            int length = -1;
            string realBlobId = GetActualBlobIdAndOffset(blobId, out offset, out length);
            if (useCacheIfAvailable && this.Cache != null && this.Cache.Exists(containerName, realBlobId))
            {
                BlobInfo bi = this.Cache.GetBlobWithoutEncryption(containerName, realBlobId);
                //LogUtil.LogDebug("Retrieve Blob from local cache store. ", blobId, sw.ElapsedMilliseconds);
                if (offset != -1)
                {
                    bi = GetBlobSegment(bi, offset, length, blobId);
                }
                return bi;
            }
            else
            {
                using (IAmazonS3 client = GetAWSClient())
                {
                    var response = client.GetObjectAsync(containerName, blobId).Result;
                    if (response.HttpStatusCode != HttpStatusCode.OK)
                    {
                        LogUtil.LogError(
                            string.Format(
                                "The AWS S3 get container({0})'s blob({1}) is failed. the response HttpStatusCode is {2}. The error message: {3}. ",
                                containerName, blobId, response.HttpStatusCode,
                                response.ResponseMetadata != null && response.ResponseMetadata.Metadata != null
                                    ? response.ResponseMetadata.Metadata.ToCommaSeparatedStringByFilter(m => m.Value)
                                    : string.Empty));
                        return null;
                    }

                    try
                    {
                        byte[] data;
                        
                        //使用直接读取流的方式：response.ResponseStream
                        using (var stream = response.ResponseStream)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                stream.CopyTo(memoryStream);
                                data = memoryStream.GetBuffer();
                            }
                        }

                        //TODO：先下载到本地，再读取成字节流，需要替换成直接读取流的方式：response.ResponseStream
                        //var rootPath = AppDomain.CurrentDomain.BaseDirectory;
                        //var filePath = rootPath + @"File\" + Guid.NewGuid();
                        //response.WriteResponseStreamToFileAsync(filePath, true, new System.Threading.CancellationToken());
                        //using (var stream = new FileInfo(filePath).OpenRead())
                        //{
                        //    var bytes = new byte[stream.Length];
                        //    stream.Read(bytes, 0, bytes.Length);

                        //    data = bytes;
                        //}

                        var blobMetadata = new Dictionary<string, string>();
                        foreach (var key in response.Metadata.Keys)
                        {
                            blobMetadata.Add(key, response.Metadata[key]);
                        }

                        string type = blobMetadata.ContainsKey("type") ? blobMetadata["type"] : "Unknown";
                        string format = blobMetadata.ContainsKey("format") ? blobMetadata["format"] : "Unknown";
                        string fileName = blobMetadata.ContainsKey("filename") ? blobMetadata["filename"] : "Unknown";
                        string ext = blobMetadata.ContainsKey("ext") ? blobMetadata["ext"] : "Unknown";
                        var blobInfo = new BlobInfo(blobId, type, format, fileName, ext, data);
                        blobInfo.Size = blobInfo.Data.Length;
                        //LogUtil.LogDebug("Retrieve Blob from AWS S3 store. ", blobId, sw.ElapsedMilliseconds);

                        if (useCacheIfAvailable && this.Cache != null)
                        {
                            this.Cache.InsertWithoutEncryption(containerName, blobId, blobInfo.Data);
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

        public override DateTime GetBlobLastModifiedTime(string containerName, string blobId)
        {
            try
            {
                var metadata = GetBlobMetadata(containerName, blobId);

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

        public override Dictionary<string, string> GetBlobMetadata(string containerName, string blobId)
        {
            using (IAmazonS3 client = GetAWSClient())
            {
                var response = client.GetObjectMetadataAsync(containerName, blobId).Result;
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    LogUtil.LogError(
                        string.Format(
                            "The AWS S3 get blob({0})'s metadata in container({1}) is failed. the response HttpStatusCode is {2}. The error message: {3}. ",
                            blobId, containerName, response.HttpStatusCode,
                            response.ResponseMetadata != null && response.ResponseMetadata.Metadata != null
                                ? response.ResponseMetadata.Metadata.ToCommaSeparatedStringByFilter(m => m.Value)
                                : string.Empty));
                    return null;
                }

                var blobMetadata = new Dictionary<string, string>();
                foreach (var key in response.Metadata.Keys)
                {
                    blobMetadata.Add(key, response.Metadata[key]);
                }

                return blobMetadata;
            }
        }

        private BlobInfo GetBlob(string containerName, string encryptionKey, bool isInternal, string userId, string blobId, bool generateCacheData, out byte[] cacheData, out byte[] cacheMetadata)
        {
            cacheData = null;
            cacheMetadata = null;

            if (string.IsNullOrEmpty(blobId))
                return null;

            using (IAmazonS3 client = GetAWSClient())
            {
                using (var response = client.GetObjectAsync(containerName, blobId).Result)
                {
                    if (response.HttpStatusCode != HttpStatusCode.OK)
                    {
                        LogUtil.LogError(
                            string.Format(
                                "The AWS S3 get container({0})'s blob({1}) is failed. the response HttpStatusCode is {2}. The error message: {3}. ",
                                containerName, blobId, response.HttpStatusCode,
                                response.ResponseMetadata != null && response.ResponseMetadata.Metadata != null
                                    ? response.ResponseMetadata.Metadata.ToCommaSeparatedStringByFilter(m => m.Value)
                                    : string.Empty));
                        return null;
                    }

                    var blobMetadata = new Dictionary<string, string>();
                    foreach (var key in response.Metadata.Keys)
                    {
                        var k = key.Replace("x-amz-meta-", "");
                        blobMetadata.Add(k, response.Metadata[key]);
                    }

                    CheckBlobAccess(containerName, isInternal, userId, blobId, blobMetadata);

                    byte[] data;
                    //使用直接读取流的方式：response.ResponseStream
                    using (var stream = response.ResponseStream)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            stream.CopyTo(memoryStream);
                            data = memoryStream.GetBuffer();
                        }
                    }

                    //TODO：先下载到本地，再读取成字节流，需要替换成直接读取流的方式：response.ResponseStream
                    //var rootPath = AppDomain.CurrentDomain.BaseDirectory;
                    //var filePath = rootPath + @"File\" + Guid.NewGuid();
                    //response.WriteResponseStreamToFileAsync(filePath, true, new System.Threading.CancellationToken());
                    //using (var stream = new FileInfo(filePath).OpenRead())
                    //{
                    //    var bytes = new byte[stream.Length];
                    //    stream.Read(bytes, 0, bytes.Length);

                    //    data = bytes;
                    //}

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
        }
    }
}
