using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using KC.Storage.Util;
using KC.Framework.Util;
using KC.Framework.Extension;
using KC.Framework.Base;
using KC.Framework.Tenant;

namespace KC.Storage.BlobService
{
    public class FileSystemProvider : ProviderBase, IBlobProvider
    {
        private readonly string RootPath;
        private readonly string StorageConnectionString;
        private readonly FileSystemReader Reader;
        private readonly FileSystemWriter Writer;

        //public FileSystemProvider()
        //{
        //    this.StorageConnectionString = ConfigUtil.GetConfigItem("StorageConnectionString");
        //    if (string.IsNullOrWhiteSpace(StorageConnectionString))
        //        throw new ArgumentException("Local Storage's connect string is empy or null.", "StorageConnectionString");

        //    var keyValues = StorageConnectionString.KeyValuePairFromConnectionString();
        //    var filePath = keyValues["blobendpoint"];
        //    if (string.IsNullOrWhiteSpace(filePath))
        //    {
        //        //TODO: 向后兼容配置文件，需要去除配置节点：BlobFileSystemPath
        //        this.RootPath = ConfigUtil.GetConfigItem("BlobFileSystemPath");
        //        //throw new ArgumentException("Local Storage's connect string is wrong. It can't set the FilePath Value.", "connectionString");
        //    }
        //    else
        //    {
        //        this.RootPath = filePath;
        //    }
            
        //    this.Reader = new FileSystemReader(this.RootPath);
        //    this.Writer = new FileSystemWriter(this.RootPath);
        //}

        public FileSystemProvider(Tenant tenant)
        {
            var filePath = tenant.StorageEndpoint;
            this.RootPath = string.IsNullOrWhiteSpace(filePath)
                ? KC.Framework.Base.GlobalConfig.TempFilePath + "\\"//TODO: 向后兼容配置文件，需要去除配置节点：BlobFileSystemPath
                : filePath.EndsWith("\\") ? filePath : filePath + "\\";

            this.Reader = new FileSystemReader(this.RootPath);
            this.Writer = new FileSystemWriter(this.RootPath);
        }

        public FileSystemProvider(string connectionString)
        {
            this.StorageConnectionString = connectionString;
            if (string.IsNullOrWhiteSpace(StorageConnectionString))
                throw new ArgumentException("Local Storage's connect string is empy or null.", "StorageConnectionString");

            var keyValues = StorageConnectionString.KeyValuePairFromConnectionString();
            var filePath = keyValues[ConnectionKeyConstant.BlobEndpoint];
            this.RootPath = string.IsNullOrWhiteSpace(filePath)
                ? KC.Framework.Base.GlobalConfig.TempFilePath + "\\" //TODO: 向后兼容配置文件，需要去除配置节点：BlobFileSystemPath
                : filePath.EndsWith("\\") ? filePath : filePath + "\\";

            this.Reader = new FileSystemReader(this.RootPath);
            this.Writer = new FileSystemWriter(this.RootPath);
        }

        public override WriterBase GetWriter()
        {
            return this.Writer;
        }

        public override List<string> GetContainers()
        {
            return this.Reader.ListContainers();
        }
        public override bool DoesContainerExist(string containerName)
        {
            return this.Reader.ExistContainer(containerName);
        }

        public override void CreateContainer(string containerName)
        {
            this.Writer.AddContainer(containerName);
        }

        public override void DeleteContainer(string containerName)
        {
            this.Writer.RemoveContainer(containerName);
        }

        public override List<string> GetBlobIds(string containerName)
        {
            return this.Reader.ListBlobIds(containerName);
        }

        public override List<string> GetBlobIdsWithMetadata(string containerName)
        {
            return this.Reader.ListBlobIdsWithMetadata(containerName);
        }

        public override bool DoesBlobExist(string containerName, string blobId)
        {
            return this.Reader.ExistBlob(containerName, blobId);
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

        public override List<BlobInfo> GetBlobs(string containerName, string encryptionKey, bool isInternal, string userId,  List<string> ids, bool returnPlaceholderIfNotFound, bool isUserLevel = false)
        {
            bool attemptCache = false;
            List<BlobInfo> blobInfos = new List<BlobInfo>();
            foreach (string blobId in ids)
            {
                BlobInfo blob = this.Reader.GetBlob(containerName, encryptionKey, isInternal, userId, blobId, attemptCache);
                blobInfos.Add(blob);
            }
            return blobInfos;
        }

        public override void SaveBlob(string containerName, string encryptionKey, bool isInternal, string userId, BlobInfo blobInfo, bool isUserLevelBlob = false, long expireAfterSecond = -1)
        {
            encryptionKey = Encryption.GetEncryptionKey(encryptionKey, isUserLevelBlob, isInternal, userId);

            bool isEncypted = false;
            byte[] dataToWrite;
            byte[] fileBytes = blobInfo.Data;
            if (!string.IsNullOrEmpty(encryptionKey))
            {
                isEncypted = true;
                dataToWrite = Encryption.Encrypt(fileBytes, encryptionKey);
                if (dataToWrite == null)
                {
                    throw new Exception("Blob encrypted data is empty");
                }
            }
            else
            {
                dataToWrite = fileBytes;
            }

            long fileSize = fileBytes.Length;
            Dictionary<string, string> extraMetadata = null;
            Dictionary<string, string> metadata = BuildMetadata(containerName, isInternal, userId, blobInfo, expireAfterSecond, isEncypted, isUserLevelBlob, extraMetadata);
            this.Writer.SaveBlob(containerName, blobInfo.Id, dataToWrite, metadata);
        }

        public override void SaveBlobWithoutEncryption(string containerName, BlobInfo blobObject)
        {
            this.Writer.SaveBlob(containerName, blobObject.Id, blobObject.Data, null);
        }

        public override void AppendBlobMetadata(string containerName, string blobId, Dictionary<string, string> metadata)
        {
            this.Writer.AppendBlobMetadata(containerName, blobId, metadata);
        }


        public override void DeleteBlob(string containerName, string blobId)
        {
            this.Writer.RemoveBlob(containerName, blobId);
        }

        public override void DeleteBlobByRelativePath(string blobRelativePath)
        {
            this.Writer.RemoveBlob(null, blobRelativePath);
        }

        public override void CopyBlob(string containerName, string desContainerName, string blobId)
        {
            this.Writer.CopyBlob(containerName, desContainerName, blobId);
        }
    }
}
