using System;
using System.Collections.Generic;
using System.Text;
using KC.Storage.BlobService;
using KC.Framework.Base;

namespace KC.Storage.Util.BlobHelper
{
    public partial class BlobProvider
    {
        public void CreateContainer(string containerName)
        {
            Provider.CreateContainer(containerName);
        }
        public void DeleteContainer(string containerName)
        {
            Provider.DeleteContainer(containerName);
        }

        public void SaveBlob(AuthToken token, BlobInfo blobObject, bool isUserLevelBlob = false, long expireAfterSecond = -1)
        {
            string containerName = token.ContainerName;

            string tenant = token.Tenant;
            string encryptionKey = token.EncryptionKey;
            string userId = token.Id;
            bool isInternal = token.Internal;

            Provider.SaveBlob(containerName, encryptionKey, isInternal, userId, blobObject, isUserLevelBlob, expireAfterSecond);
        }
        public void SaveBlobWithoutEncryption(string containerName, BlobInfo blobObject)
        {
            Provider.SaveBlobWithoutEncryption(containerName, blobObject);
        }
        public void SaveBlobs(AuthToken token, List<BlobInfo> blobObjects, bool isUserLevelBlob = false, long expireAfterSecond = -1)
        {
            foreach (BlobInfo blobObject in blobObjects)
            {
                SaveBlob(token, blobObject, isUserLevelBlob, expireAfterSecond);
            }
        }

        public void AppendBlobMetadata(string containerName, string blobId, Dictionary<string, string> extraMetadata)
        {
            Provider.AppendBlobMetadata(containerName, blobId, extraMetadata);
        }

        public void DeleteBlob(string containerName, params string[] blobId)
        {
            foreach (string oneBlobId in blobId)
            {
                Provider.DeleteBlob(containerName, oneBlobId);
            }
        }
        public void DeleteBlobByRelativePath(string relativePath)
        {
            Provider.DeleteBlobByRelativePath(relativePath);
        }
        public void DeleteBlob(AuthToken token, params string[] blobId)
        {
            if (null != token && !string.IsNullOrEmpty(token.ContainerName))
            {
                DeleteBlob(token.ContainerName, blobId);
            }
        }

        public void CopyBlob(string containerName, string desContainerName, string blobId)
        {
            Provider.CopyBlob(containerName, desContainerName, blobId);
        }

        public void CopyBlobs(string containerName, string desContainerName, params string[] blobId)
        {
            foreach (string oneBlobId in blobId)
            {
                Provider.CopyBlob(containerName, desContainerName, oneBlobId);
            }
        }
    }
}
