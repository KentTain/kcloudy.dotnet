using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KC.Framework.Base;

namespace KC.Storage.BlobService
{
    internal abstract class ReaderBase : BlobBase
    {
        public ReaderBase()
        {
        }

        public abstract bool ExistContainer(string containerName);
        public abstract bool ExistBlob(string containerName, string blobId);
        public abstract List<string> ListContainers();
        public abstract List<string> ListBlobIds(string containerName);
        public abstract List<string> ListBlobIdsWithMetadata(string container);
        public abstract BlobInfo GetBlob(string container, string encryptionKey, bool isInternal, string userId, string blobId, bool attemptCache);
        public abstract BlobInfo GetBlobWithoutEncryption(string containerName, string blobId, bool useCacheIfAvailable);
        public abstract DateTime GetBlobLastModifiedTime(string containerName, string blobId);
        public abstract Dictionary<string, string> GetBlobMetadata(string containerName, string blobId);
    }
}