using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using KC.Framework.Base;

namespace KC.Storage.BlobService
{
    internal class BlobCache : BlobBase
    {
        private readonly string TempStorageFolder;

        public BlobCache(string tempStorageFolder)
            : base()
        {
            this.TempStorageFolder = tempStorageFolder;
        }

        private Dictionary<string, string> GetCacheMetadata(string filename)
        {
            string metadataFileName = filename + ".metadata";

            if (!File.Exists(metadataFileName))
                return new Dictionary<string, string>();

            try
            {
                string metadataXml = File.ReadAllText(metadataFileName, Encoding.UTF8);
                return ParseMetadataXml(metadataXml);
            }
            catch (Exception)
            {
                return new Dictionary<string, string>();
            }
        }

        public void Insert(string container, string blobId, byte[] blobContent, byte[] blobMetadata)
        {
            if (!Directory.Exists(this.TempStorageFolder))
            {
                Directory.CreateDirectory(this.TempStorageFolder);
            }

            string filename = this.TempStorageFolder + container + "__" + blobId;
            
            File.WriteAllBytes(filename, blobContent);
            File.WriteAllBytes(filename + ".metadata", blobMetadata);
        }

        public void InsertWithoutEncryption(string container, string blobId, byte[] blobContent)
        {
            string filename = this.TempStorageFolder + container + "__" + blobId;
            File.WriteAllBytes(filename, blobContent);
        }

        public bool Exists(string container, string blobId)
        {
            string cacheFilename = this.TempStorageFolder + container + "__" + blobId;
            return File.Exists(cacheFilename);
        }

        public BlobInfo GetBlob(string container, string encryptionKey, bool isInternal, string userId, string blobId)
        {
            string filePath = this.TempStorageFolder + container + "__" + blobId;

            Dictionary<string, string> blobMetadata = GetCacheMetadata(filePath);
            CheckBlobAccess(container, isInternal, userId, blobId, blobMetadata);

            string type = blobMetadata.ContainsKey("Type") ? blobMetadata["Type"] : "Unknown";
            string format = blobMetadata.ContainsKey("Format") ? blobMetadata["Format"] : "Unknown";
            string fileName = blobMetadata.ContainsKey("FileName") ? blobMetadata["FileName"] : "Unknown";
            string ext = blobMetadata.ContainsKey("ext") ? blobMetadata["ext"] : "Unknown";
            byte[] data = File.ReadAllBytes(filePath);
            byte[] blobActualData = DecryptIfNeeded(encryptionKey, isInternal, userId, data, blobMetadata);

            if (blobActualData.Length == 0)
                throw new Exception("Blob " + blobId + " has zero length from cache");

            return new BlobInfo(blobId, type, format, fileName, ext, blobActualData);
        }

        public BlobInfo GetBlobWithoutEncryption(string container, string blobId)
        {
            string filename = this.TempStorageFolder + container + "__" + blobId;

            return new BlobInfo(blobId, null, null, null, null, File.ReadAllBytes(filename));
        }

        public void Delete(string container, string blobId)
        {
            string filename = this.TempStorageFolder + container + "__" + blobId;
            if (File.Exists(filename))
            {
                File.Delete(filename);
                File.Delete(filename + ".metadata");
            }
        }
    }
}