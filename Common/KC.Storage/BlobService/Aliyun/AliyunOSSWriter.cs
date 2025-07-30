using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using KC.Framework.Util;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using Aliyun.OSS;
using Aliyun.OSS.Common;

namespace KC.Storage.BlobService
{
    internal class AliyunOSSWriter : WriterBase
    {
        //private readonly AmazonS3Config Config;
        //private readonly AWSCredentials Credentials;
        //private readonly IAmazonS3 client;
        private readonly BlobCache Cache;
        private readonly string endpoint;
        private readonly string accessKey;
        private readonly string secretKey;

        private readonly string preBucket = "kcloudy-aliyun-storage-";

        public AliyunOSSWriter(string connectionString, BlobCache cache)
        {
            if(string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Aliyun OSS's connectionString string is empy or null.", "connectionString");
            
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

        protected override void CreateContainer(string container)
        {
            string bucket = preBucket + container;
            try
            {
                OssClient client = GetAliyunOSSClient();
                var existContainer = client.DoesBucketExist(bucket);
                if (!existContainer)
                {
                    //var request = new CreateBucketRequest(bucket, StorageClass.Standard, CannedAccessControlList.PublicReadWrite);
                    //var response = client.CreateBucket(request);

                    client.CreateBucket(bucket);
                }
            }
            catch (OssException ex)
            {
                LogUtil.LogError(string.Format("CreateContainer failed with error info: {0}; Error info: {1}. " + Environment.NewLine + "RequestID:{2}" + Environment.NewLine + "HostID:{3}",
                                  ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId));
            }
        }

        protected override void DeleteContainer(string container)
        {
            string bucket = preBucket + container;
            try
            {
                OssClient client = GetAliyunOSSClient();
                client.DeleteBucket(bucket);
            }
            catch (OssException ex)
            {
                LogUtil.LogError(string.Format("DeleteContainer failed with error info: {0}; Error info: {1}. " + Environment.NewLine + "RequestID:{2}" + Environment.NewLine + "HostID:{3}",
                                  ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId));
            }
        }

        protected override void WriteBlob(string container, string blobId, byte[] blobData, Dictionary<string, string> metadata)
        {
            string bucket = preBucket + container;
            try
            {
                OssClient client = GetAliyunOSSClient();
                var existContainer = client.DoesBucketExist(bucket);
                if (!existContainer)
                {
                    //var request = new CreateBucketRequest(bucket, StorageClass.Standard, CannedAccessControlList.PublicReadWrite);
                    //var response = client.CreateBucket(request);

                    client.CreateBucket(bucket);
                }

                ObjectMetadata objectMetadata = new ObjectMetadata();
                if (metadata != null)
                {
                    foreach (string metadataKey in metadata.Keys)
                    {
                        objectMetadata.UserMetadata[metadataKey] = metadata[metadataKey];
                    }
                }

                //var request = new PutObjectRequest(bucket, blobId, new MemoryStream(blobData), objectMetadata);
                //var response = client.PutObject(request);

                var response = client.PutObject(bucket, blobId, new MemoryStream(blobData), objectMetadata);
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    LogUtil.LogError(
                        string.Format(
                            "Aliyun OSS WriteBlob the container({0}) blob({1}) is failed. the response HttpStatusCode is {2}. ",
                            bucket, blobId, response.HttpStatusCode));
                }
            }
            catch (OssException ex)
            {
                LogUtil.LogError(string.Format("WriteBlob failed with error info: {0}; Error info: {1}. " + Environment.NewLine + "RequestID:{2}" + Environment.NewLine + "HostID:{3}",
                                  ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId));
            }
        }

        protected override void DeleteBlob(string container, string blobId)
        {
            string bucket = preBucket + container;
            try
            {
                OssClient client = GetAliyunOSSClient();
                var response = client.DeleteObject(bucket, blobId);
                if (response.HttpStatusCode != HttpStatusCode.NoContent)
                {
                    LogUtil.LogError(
                        string.Format(
                            "AWS delete the container({0}) blob({1}) is failed. the response HttpStatusCode is {2}.",
                            bucket, blobId, response.HttpStatusCode));
                }
            }
            catch (OssException ex)
            {
                LogUtil.LogError(string.Format("Failed with error info: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
                                  ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId));
            }
        }

        public override void CopyBlob(string container, string desContainerName, string blobId)
        {
            if (string.IsNullOrEmpty(blobId)) return;

            string bucket = preBucket + container;
            try
            {
                OssClient client = GetAliyunOSSClient();
                var existContainer = client.DoesBucketExist(bucket);
                if (!existContainer)
                {
                    //var request = new CreateBucketRequest(bucket, StorageClass.Standard, CannedAccessControlList.PublicReadWrite);
                    //var response = client.CreateBucket(request);

                    client.CreateBucket(bucket);
                }

                var copyRequest = new CopyObjectRequest(bucket, blobId, preBucket + desContainerName, blobId);
                var response = client.CopyObject(copyRequest);
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    LogUtil.LogError(
                        string.Format(
                            "Aliyun OSS CopyObject(blobId={1}) from the container({0}) is failed. the response HttpStatusCode is {2}. ",
                            bucket, blobId, response.HttpStatusCode));
                }
            }
            catch (OssException ex)
            {
                LogUtil.LogError(string.Format("Failed with error info: {0}; Error info: {1}. " + Environment.NewLine + "RequestID:{2}" + Environment.NewLine + "HostID:{3}",
                                  ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId));
            }
        }

        protected override void WriteBlobMetadata(string container, string blobId, bool clearExisting, Dictionary<string, string> blobMetadata)
        {
            throw new NotImplementedException();
        }
    }
}
