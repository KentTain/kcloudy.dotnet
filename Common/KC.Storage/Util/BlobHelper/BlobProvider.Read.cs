using System;
using System.Collections.Generic;
using KC.Framework.Base;

namespace KC.Storage.Util.BlobHelper
{
    public partial class BlobProvider
    {
        public List<string> GetContainers()
        {
            return Provider.GetContainers();
        }
        public bool DoesContainerExist(string containerName)
        {
            return Provider.DoesContainerExist(containerName);
        }
        public List<string> GetAllBlobIds(string containerName)
        {
            return Provider.GetBlobIds(containerName);
        }

        public List<string> GetAllBlobIds(AuthToken token)
        {
            string tenant = token.Tenant;
            string encryptionKey = token.EncryptionKey;
            string userId = token.Id;
            string containerName = token.ContainerName;
            bool isInternal = token.Internal;

            return Provider.GetBlobIds(containerName);
        }

        public List<string> GetBlobIdsWithMetadata(string containerName)
        {
            return Provider.GetBlobIdsWithMetadata(containerName);
        }
        public List<BlobInfo> GetBlobs(AuthToken token, List<string> blobIds, bool returnPlaceholderIfNotFound, bool isUserLevelBlob = false)
        {
            string tenant = token.Tenant;
            string encryptionKey = token.EncryptionKey;
            string userId = token.Id;
            string containerName = token.ContainerName;
            bool isInternal = token.Internal;
            return Provider.GetBlobs(containerName, encryptionKey, isInternal, userId,  blobIds, returnPlaceholderIfNotFound, isUserLevelBlob);
        }

        public List<BlobInfo> GetBlobs(AuthToken token)
        {
            string tenant = token.Tenant;
            string encryptionKey = token.EncryptionKey;
            string userId = token.Id;
            string containerName = token.ContainerName;
            bool isInternal = token.Internal;
            var blobIds = GetAllBlobIds(containerName);

            return Provider.GetBlobs(containerName, encryptionKey, isInternal, userId, blobIds, false, false);
        }

        public bool DoesBlobExist(AuthToken token, string blobId)
        {
            return DoesBlobExist(token.ContainerName, blobId);
        }
        public bool DoesBlobExist(string containerName, string blobId)
        {
            return Provider.DoesBlobExist(containerName, blobId);
        }
        public BlobInfo GetBlob(AuthToken token, string blobId, bool useCacheIfAvailable = false)
        {
            string tenant = token.Tenant;
            string encryptionKey = token.EncryptionKey;
            string userId = token.Id;
            string container = token.ContainerName;
            bool isInternal = token.Internal;
            return Provider.GetBlob(container, encryptionKey, isInternal, userId, blobId, useCacheIfAvailable);
        }

        public BlobInfo GetBlobWithoutEncryption(string containerName, string blobId, bool useCacheIfAvailable = false)
        {
            return Provider.GetBlobWithoutEncryption(containerName, blobId, useCacheIfAvailable);
        }
        public Dictionary<string, string> GetBlobMetadata(string containerName, string blobId)
        {
            return Provider.GetBlobMetadata(containerName, blobId);
        }
        public DateTime GetBlobLastModifiedTime(string containerName, string blobId)
        {
            return Provider.GetBlobLastModifiedTime(containerName, blobId);
        }
    }
}
