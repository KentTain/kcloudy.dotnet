using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;

namespace KC.Storage.BlobService
{
    partial class AzureProvider
    {
        public override List<string> GetContainers()
        {
            return this.Reader.ListContainers();
        }
        public override bool DoesContainerExist(string containerName)
        {
            return this.Reader.ExistContainer(containerName);
        }

        public override bool DoesBlobExist(string containerName, string blobId)
        {
            return this.Reader.ExistBlob(containerName, blobId);
        }

        public override List<string> GetBlobIds(string containerName)
        {
            return this.Reader.ListBlobIds(containerName);
        }

        public override List<string> GetBlobIdsWithMetadata(string containerName)
        {
            return this.Reader.ListBlobIdsWithMetadata(containerName);
        }

        public override BlobInfo GetBlob(string containerName, string encryptionKey, bool isInternal, string userId, string blobId, bool useCacheIfAvailable)
        {
            return this.Reader.GetBlob(containerName, encryptionKey, isInternal, userId, blobId, useCacheIfAvailable);
        }

        public override BlobInfo GetBlobWithoutEncryption(string containerName, string blobId, bool useCacheIfAvailable)
        {
            return this.Reader.GetBlobWithoutEncryption(containerName, blobId, useCacheIfAvailable);
        }

        public override DateTime GetBlobLastModifiedTime(string containerName, string blobId)
        {
            return this.Reader.GetBlobLastModifiedTime(containerName, blobId);
        }

        public override Dictionary<string, string> GetBlobMetadata(string containerName, string blobId)
        {
            return this.Reader.GetBlobMetadata(containerName, blobId);
        }

        public override List<BlobInfo> GetBlobs(string container, string encryptionKey, bool isInternal, string userId, List<string> blobIds, bool returnPlaceholderIfNotFound, bool isUserLevel = false)
        {
            return this.Reader.GetBlobs(container, encryptionKey, isInternal, userId,  blobIds, returnPlaceholderIfNotFound, isUserLevel);
        }
    }
}
