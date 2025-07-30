using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Storage.Util;
using KC.Framework.Base;

namespace KC.Storage.BlobService
{
    partial class AzureProvider
    {
        public override void CreateContainer(string containerName)
        {
            this.Writer.AddContainer(containerName);
        }
        public override void DeleteContainer(string containerName)
        {
            this.Writer.RemoveContainer(containerName);// To make sure delete does not fail.
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

            long size = fileBytes.Length;
            Dictionary<string, string> extraMetadata = null;
            Dictionary<string, string> metadata = BuildMetadata(containerName, isInternal, userId, blobInfo, expireAfterSecond, isEncypted, isUserLevelBlob, extraMetadata);
            this.Writer.SaveBlob(containerName, blobInfo.Id, dataToWrite, metadata);
        }

        public override void SaveBlobWithoutEncryption(string containerName, BlobInfo blobObject)
        {
            string blobId = blobObject.Id;
            byte[] dataToWrite = blobObject.Data;
            Dictionary<string, string> metadata = this.Reader.GetBlobMetadata(containerName, blobId);
            metadata["Id"] = blobObject.Id;
            metadata["Type"] = blobObject.FileType;
            metadata["Format"] = blobObject.FileFormat;
            metadata["LastModifiedTime"] = GetModifiedTime();
            metadata["Size"] = blobObject.Size.ToString();
            this.Writer.SaveBlob(containerName, blobId, dataToWrite, metadata);
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

