using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using KC.Storage.Util;
using KC.Framework.Base;

namespace KC.Storage.BlobService
{
    public abstract class ProviderBase : BlobBase, IBlobProvider
    {
        protected byte[] DecryptIfNeeded(byte[] data, Dictionary<string, string> metadata, string token)
        {
            bool isEncrypted = metadata.ContainsKey("IsEncrypted") ? isEncrypted = bool.Parse(metadata["IsEncrypted"]) : false;
            if (isEncrypted)
            {
                bool isUserLevelBlob = metadata.ContainsKey("IsUserLevelBlob") ? bool.Parse(metadata["IsUserLevelBlob"]) : false;
                string encryptionKey = Encryption.GetEncryptionKey(token, isUserLevelBlob);
                return Encryption.Decrypt(data, encryptionKey);
            }
            else
            {
                return data;
            }
        }
        
        protected Dictionary<string, string> GetCacheMetadata(string filename)
        {
            string metadataFileName = filename + ".metadata";

            if (!File.Exists(metadataFileName))
            {
                return new Dictionary<string, string>();
            }

            var metadataDict = new Dictionary<string, string>();

            try
            {
                string metadataXml = Encoding.UTF8.GetString(File.ReadAllBytes(metadataFileName));
                XDocument metadataXDoc = XDocument.Parse(metadataXml);

                if (metadataXDoc.Root == null) return metadataDict;

                foreach (XElement descendant in metadataXDoc.Root.Descendants())
                {
                    if (!metadataDict.ContainsKey(descendant.Name.LocalName))
                    {
                        metadataDict.Add(descendant.Name.LocalName, descendant.Value);
                    }
                }
            }
            catch (Exception)
            {
                return new Dictionary<string, string>();
            }

            return metadataDict;
        }

        public abstract WriterBase GetWriter();

        public abstract List<string> GetContainers();
        public abstract bool DoesContainerExist(string containerName);
        public abstract void CreateContainer(string containerName);
        public abstract void DeleteContainer(string containerName);
        public abstract List<string> GetBlobIds(string containerName);

        public abstract List<string> GetBlobIdsWithMetadata(string containerName);

        public abstract bool DoesBlobExist(string containerName, string blobId);
        public abstract BlobInfo GetBlob(string containerName, string encryptionKey, bool isInternal, string userId, string blobId, bool useCacheIfAvailable = true);
        public abstract BlobInfo GetBlobWithoutEncryption(string containerName, string blobId, bool useCacheIfAvailable = true);
        public abstract DateTime GetBlobLastModifiedTime(string containerName, string blobId);
        public abstract Dictionary<string, string> GetBlobMetadata(string containerName, string blobId);
        public abstract List<BlobInfo> GetBlobs(string containerName, string encryptionKey, bool isInternal, string userId, List<string> blobIds, bool returnPlaceholderIfNotFound, bool isUserLevel = false);
        public abstract void SaveBlob(string containerName, string encryptionKey, bool isInternal, string userId, BlobInfo blobInfo, bool isUserLevelBlob = false, long expireAfterSecond = -1);
        public abstract void SaveBlobWithoutEncryption(string containerName, BlobInfo blobObject);
        public abstract void AppendBlobMetadata(string containerName, string blobId, Dictionary<string, string> extraMetadata);
  
        public abstract void DeleteBlob(string containerName, string blobId);
        public abstract void DeleteBlobByRelativePath(string blobRelativePath);

        public abstract void CopyBlob(string containerName, string desContainerName, string blobId);
    }
}
