using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KC.Framework.Util;
using KC.Framework.Base;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;

namespace KC.Storage.BlobService
{
    internal partial class AzureReader : ReaderBase
    {
        private readonly BlobServiceClient Account;
        private readonly BlobCache Cache;

        public AzureReader(string connectionString, BlobCache cache)
            : base()
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Azure Storage's connect string is empy or null.", "connectionString");

            this.Account = new BlobServiceClient(connectionString);
            this.Cache = cache;
        }

        private BlobClient GetBlobClient(string containerName, string blobId, bool isBlockBlob = false)
        {
            if (string.IsNullOrWhiteSpace(containerName))
            {
                return null;
            }
            else
            {
                // Create the container and return a container client object
                var container = Account.GetBlobContainerClient(containerName);
                container.CreateIfNotExists();

                return isBlockBlob ? container.GetBlobClient(blobId) : container.GetBlobClient(blobId);
            }
        }

        private Framework.Base.BlobInfo GetBlob(string containerName, string encryptionKey, bool isInternal, string userId, string blobId, bool generateCacheData, out byte[] cacheData, out byte[] cacheMetadata)
        {
            cacheData = null;
            cacheMetadata = null;

            if (string.IsNullOrEmpty(blobId))
                return null;

            var blob = GetBlobClient(containerName, blobId);
            if (blob == null)
                return null;

            BlobProperties properties = blob.GetProperties();
            IDictionary<string, string> blobMetadata = properties.Metadata;
            var type = blobMetadata.ContainsKey("Type") ? blobMetadata["Type"] : "Unknown";
            var format = blobMetadata.ContainsKey("Format") ? blobMetadata["Format"] : "Unknown";
            var fileName = blobMetadata.ContainsKey("FileName") ? blobMetadata["FileName"] : "Unknown";
            var ext = blobMetadata.ContainsKey("ext") ? blobMetadata["ext"] : "Unknown";

            CheckBlobAccess(containerName, isInternal, userId, blobId, blobMetadata);

            var fileByteLength = properties.ContentLength;
            var data = new byte[fileByteLength];

            var success = DownloadByteArray(blob, data);
            if (!success) return null;

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

            byte[] blobActualData = DecryptIfNeeded(encryptionKey, isInternal, userId, data, blobMetadata);
            var bi = new Framework.Base.BlobInfo(blobId, type, format, fileName, ext, blobActualData);
            bi.Size = bi.Data.Length;
            return bi;
        }

        private bool DownloadByteArray(BlobClient blob, byte[] data)
        {
            try
            {
                int i = 0;
                do
                {
                    i++;
                    using (var stream = new MemoryStream(data))
                    {
                        var result = blob.DownloadTo(stream);
                        if (result.Status == 200)
                            return true;
                    }
                } while (i > 3);
                
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(string.Format(
                    "----blob.DownloadByteArray throw error: {0}",
                    ex.Message));
                return false;
            }
        }
    }
}