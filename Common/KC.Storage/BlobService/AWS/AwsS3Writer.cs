using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using KC.Framework.Util;
using KC.Framework.Extension;
using KC.Framework.Tenant;

namespace KC.Storage.BlobService
{
    internal class AwsS3Writer : WriterBase
    {
        //private readonly AmazonS3Config Config;
        //private readonly AWSCredentials Credentials;
        //private readonly IAmazonS3 client;
        private readonly BlobCache Cache;
        private readonly string endpoint;
        private readonly string accessKey;
        private readonly string secretKey;

        public AwsS3Writer(string connectionString, BlobCache cache)
        {
            if(string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("AWS S3's connectionString string is empy or null.", "connectionString");
            
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

            //ServicePointManager.SecurityProtocol =
            //        SecurityProtocolType.Tls
            //        | SecurityProtocolType.Tls11
            //        | SecurityProtocolType.Tls12;
            ////ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();
            //ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;

            //var Config = new AmazonS3Config()
            //{
            //    ServiceURL = endpoint,
            //    ForcePathStyle = true,
            //    SignatureVersion = "2",
            //    //BufferSize = 15 * 1024 * 1024, //15M
            //    //RegionEndpoint = Amazon.RegionEndpoint.CNNorth1
            //    //RegionEndpoint = Amazon.RegionEndpoint.USEast1,
            //};
            //var Credentials = new BasicAWSCredentials(accessKey, secretKey);
            //client = new AmazonS3Client(Credentials, Config);

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

        protected override void CreateContainer(string container)
        {
            using (IAmazonS3 client = GetAWSClient())
            {
                client.EnsureBucketExistsAsync(container);
            }
        }

        protected override void DeleteContainer(string container)
        {
            using (IAmazonS3 client = GetAWSClient())
            {
                var listResponse = client.ListObjectsAsync(container).Result;
                if (listResponse.HttpStatusCode != HttpStatusCode.OK)
                {
                    LogUtil.LogError(
                        string.Format(
                            "AWS get the container({0}) blobs is failed. the response HttpStatusCode is {1}. The error message: {2}. ",
                            container, listResponse.HttpStatusCode,
                            listResponse.ResponseMetadata != null && listResponse.ResponseMetadata.Metadata != null
                                ? listResponse.ResponseMetadata.Metadata.ToCommaSeparatedStringByFilter(m => m.Value)
                                : string.Empty));
                }

                var blobIds = listResponse.S3Objects.Select(s3Object => s3Object.Key).ToList();
                if (!blobIds.Any())
                    return;

                var request = new DeleteObjectsRequest()
                {
                    BucketName = container,
                };
                foreach (var blobId in blobIds)
                {
                    request.AddKey(blobId);
                }

                var response = client.DeleteObjectsAsync(request).Result;
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    LogUtil.LogError(
                        string.Format(
                            "AWS delete the container({0})'s all blobs is failed. the response HttpStatusCode is {1}. The error message: {2}. ",
                            container, response.HttpStatusCode,
                            response.DeleteErrors != null && response.DeleteErrors.Any()
                                ? response.DeleteErrors.Select(m => m.Message).ToCommaSeparatedString()
                                : string.Empty));
                }
            }
        }

        protected override void WriteBlob(string container, string blobId, byte[] blobData, Dictionary<string, string> metadata)
        {
            using (IAmazonS3 client = GetAWSClient())
            {
                client.EnsureBucketExistsAsync(container);

                var request = new PutObjectRequest()
                {
                    BucketName = container,
                    CannedACL = S3CannedACL.PublicRead,
                    Key = blobId,
                    InputStream = new MemoryStream(blobData)
                };
                if (metadata != null)
                {
                    foreach (string metadataKey in metadata.Keys)
                    {
                        request.Metadata[metadataKey] = metadata[metadataKey];
                    }
                }

                var response = client.PutObjectAsync(request).Result;
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    LogUtil.LogError(
                        string.Format(
                            "AWS S3 WriteBlob the container({0}) blob({1}) is failed. the response HttpStatusCode is {2}. The error message: {3}. ",
                            container, blobId, response.HttpStatusCode,
                            response.ResponseMetadata != null && response.ResponseMetadata.Metadata != null
                                ? response.ResponseMetadata.Metadata.ToCommaSeparatedStringByFilter(m => m.Value)
                                : string.Empty));
                }
            }
        }

        protected override void DeleteBlob(string container, string blobId)
        {
            using (IAmazonS3 client = GetAWSClient())
            {
                var request = new DeleteObjectRequest()
                {
                    BucketName = container,
                    Key = blobId
                };
                var response = client.DeleteObjectAsync(request).Result;
                if (response.HttpStatusCode != HttpStatusCode.NoContent)
                {
                    LogUtil.LogError(
                        string.Format(
                            "AWS delete the container({0}) blob({1}) is failed. the response HttpStatusCode is {2}. The error message: {3}. ",
                            container, blobId, response.HttpStatusCode,
                            response.ResponseMetadata != null && response.ResponseMetadata.Metadata != null
                                ? response.ResponseMetadata.Metadata.ToCommaSeparatedStringByFilter(m => m.Value)
                                : string.Empty));
                }
            }
        }

        public override void CopyBlob(string containerName, string desContainerName, string blobId)
        {
            if (string.IsNullOrEmpty(blobId)) return;

            using (IAmazonS3 client = GetAWSClient())
            {
                client.EnsureBucketExistsAsync(desContainerName);

                var response = client.CopyObjectAsync(containerName, blobId, desContainerName, blobId).Result;
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    LogUtil.LogError(
                        string.Format(
                            "AWS S3 GetObject(blobId={1}) from the container({0}) is failed. the response HttpStatusCode is {2}. The error message: {3}. ",
                            containerName, blobId, response.HttpStatusCode,
                            response.ResponseMetadata != null && response.ResponseMetadata.Metadata != null
                                ? response.ResponseMetadata.Metadata.ToCommaSeparatedStringByFilter(m => m.Value)
                                : string.Empty));
                }
            }
        }

        protected override void WriteBlobMetadata(string container, string blobId, bool clearExisting, Dictionary<string, string> blobMetadata)
        {
            throw new NotImplementedException();
        }

    }
}
