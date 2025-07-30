using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;

namespace KC.Storage.BlobService
{
    public interface IBlobProvider
    {
        WriterBase GetWriter();

        List<string> GetContainers();
        void CreateContainer(string containerName);
        void DeleteContainer(string containerName);
        bool DoesContainerExist(string containerName);
        List<string> GetBlobIds(string containerName);
        List<string> GetBlobIdsWithMetadata(string containerName);
        bool DoesBlobExist(string containerName, string blobId);
        BlobInfo GetBlob(string containerName, string encryptionKey, bool isInternal, string userId, string blobId, bool useCacheIfAvailable = true);
        BlobInfo GetBlobWithoutEncryption(string containerName, string blobId, bool useCacheIfAvailable = true);
        DateTime GetBlobLastModifiedTime(string containerName, string blobId);
        Dictionary<string, string> GetBlobMetadata(string containerName, string blobId);
        List<BlobInfo> GetBlobs(string containerName, string encryptionKey, bool isInternal, string userId, List<string> blobIds, bool returnPlaceholderIfNotFound, bool isUserLevel = false);
        void SaveBlob(string containerName, string encryptionKey, bool isInternal, string userId, BlobInfo blobInfo, bool isUserLevelBlob = false, long expireAfterSecond = -1);
        void SaveBlobWithoutEncryption(string containerName, BlobInfo blobObject);

        void DeleteBlob(string containerName, string blobId);
        void DeleteBlobByRelativePath(string blobRelativePath);

        void AppendBlobMetadata(string containerName, string blobId, Dictionary<string, string> extraMetadata);

        void CopyBlob(string containerName, string desContainerName, string blobId);
    }
}
