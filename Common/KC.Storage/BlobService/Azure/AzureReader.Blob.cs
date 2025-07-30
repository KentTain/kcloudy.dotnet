using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Azure.Storage.Blobs.Models;
using KC.Framework.Base;

namespace KC.Storage.BlobService
{
    internal partial class AzureReader
    {
        public override bool ExistBlob(string containerName, string blobId)
        {
            if (string.IsNullOrEmpty(blobId))
                return false;

            var container = this.Account.GetBlobContainerClient(containerName);
            if (!container.Exists())
                return false;

            string realBlobId = GetActualBlobId(blobId);
            var blob = container.GetBlobClient(realBlobId);
            return blob.Exists();
        }

        public override Framework.Base.BlobInfo GetBlob(string container, string encryptionKey, bool isInternal, string userId, string blobId, bool useCacheIfAvailable)
        {
            if (string.IsNullOrEmpty(blobId) || string.IsNullOrEmpty(container)) return null;

            var sw = new Stopwatch();
            sw.Start();

            int offset = -1;
            int length = -1;
            string realBlobId = GetActualBlobIdAndOffset(blobId, out offset, out length);
            if (useCacheIfAvailable && this.Cache != null && this.Cache.Exists(container, realBlobId))
            {
                var bi = this.Cache.GetBlob(container, encryptionKey, isInternal, userId, realBlobId);

                //LogUtil.LogDebug("Retrieve Blob from local store", blobId, sw.ElapsedMilliseconds);

                if (offset != -1)
                {
                    bi = GetBlobSegment(bi, offset, length, blobId);
                }
                return bi;
            }
            else
            {
                var blob = this.Account.GetBlobContainerClient(container).GetBlobClient(realBlobId);
                if (blob != null)
                {
                    byte[] cacheData, cacheMetadata;
                    var bi = GetBlob(container, encryptionKey, isInternal, userId, realBlobId, useCacheIfAvailable, out cacheData, out cacheMetadata);
                    //LogUtil.LogDebug("Retrieve Blob", blobId, sw.ElapsedMilliseconds);

                    if (useCacheIfAvailable && cacheData != null)
                    {
                        //this.Cache.Insert(container, realBlobId, cacheData, cacheMetadata);
                    }
                    if (offset != -1)
                    {
                        bi = GetBlobSegment(bi, offset, length, blobId);
                    }
                    return bi;
                }
                return null;
            }
        }

        public override DateTime GetBlobLastModifiedTime(string containerName, string blobId)
        {
            try
            {
                var blob = this.GetBlobClient(containerName, blobId);
                BlobProperties properties = blob.GetProperties();
                return properties.LastModified.DateTime;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public override Dictionary<string, string> GetBlobMetadata(string containerName, string blobId)
        {
            var blob = this.GetBlobClient(containerName, blobId);
            BlobProperties properties = blob.GetProperties();
            var blobMetadata = new Dictionary<string, string>();
            foreach (var key in properties.Metadata.Keys)
            {
                blobMetadata.Add(key, properties.Metadata[key]);
            }
            return blobMetadata;
        }

        public override Framework.Base.BlobInfo GetBlobWithoutEncryption(string containerName, string blobId, bool useCacheIfAvailable)
        {
            if (string.IsNullOrEmpty(blobId) || string.IsNullOrEmpty(containerName)) return null;

            var sw = new Stopwatch();
            sw.Start();

            int offset = -1;
            int length = -1;
            string realBlobId = GetActualBlobIdAndOffset(blobId, out offset, out length);
            if (useCacheIfAvailable && this.Cache != null && this.Cache.Exists(containerName, realBlobId))
            {
                var bi = this.Cache.GetBlobWithoutEncryption(containerName, realBlobId);
                //LogUtil.LogDebug("Retrieve Blob from local store", blobId, sw.ElapsedMilliseconds);
                if (offset != -1)
                {
                    bi = GetBlobSegment(bi, offset, length, blobId);
                }
                return bi;
            }
            else
            {
                var blob = GetBlobClient(containerName, realBlobId);
                if (blob != null)
                {
                    BlobProperties properties = blob.GetProperties();
                    var blobMetadata = properties.Metadata;
                    var type = blobMetadata.ContainsKey("Type") ? blobMetadata["Type"] : "Unknown";
                    var format = blobMetadata.ContainsKey("Format") ? blobMetadata["Format"] : "Unknown";
                    var fileName = blobMetadata.ContainsKey("FileName") ? blobMetadata["FileName"] : "Unknown";
                    var ext = blobMetadata.ContainsKey("ext") ? blobMetadata["ext"] : "Unknown";

                    var fileByteLength = properties.ContentLength;
                    var blobData = new byte[fileByteLength];
                    DownloadByteArray(blob, blobData);

                    var blobInfo = new Framework.Base.BlobInfo(realBlobId, type, format, fileName, ext, blobData) { Size = blobData.Length };
                    //LogUtil.LogDebug("Retrieve Blob", blobId, sw.ElapsedMilliseconds);

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
                return null;
            }
        }

        public List<Framework.Base.BlobInfo> GetBlobs(string container, string encryptionKey, bool isInternal, string userId, List<string> blobIds, bool returnPlaceholderIfNotFound, bool isUserLevel = false)
        {
            var blobInfos = new List<Framework.Base.BlobInfo>();
            foreach (string blobId in blobIds)
            {
                int offset = -1;
                int length = -1;
                string realBlobId = GetActualBlobIdAndOffset(blobId, out offset, out length);

                byte[] cacheData;
                byte[] cacheMetadata;
                var bi = GetBlob(container, encryptionKey, isInternal, userId,  realBlobId, false, out cacheData, out cacheMetadata);
                if (bi != null)
                {
                    if (offset != -1)
                    {
                        bi = GetBlobSegment(bi, offset, length, blobId);
                    }
                    blobInfos.Add(bi);
                }
            }
            return blobInfos;
        }
    }
}
