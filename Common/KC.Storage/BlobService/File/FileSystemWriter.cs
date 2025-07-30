using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KC.Storage.BlobService
{
    internal class FileSystemWriter : WriterBase
    {
        private readonly string RootPath;

        public FileSystemWriter(string rootPath)
            : base()
        {
            this.RootPath = rootPath;
        }

        protected override void CreateContainer(string containerName)
        {
            string dirName = this.RootPath + containerName;
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
        }

        protected override void DeleteContainer(string containerName)
        {
            var path = Path.Combine(this.RootPath, containerName);
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        protected override void WriteBlob(string container, string blobId, byte[] blobData, Dictionary<string, string> blobMetadata)
        {
            CreateContainer(container);
            string fileName = this.RootPath + container + "\\" + blobId;
            File.WriteAllBytes(fileName, blobData);
            if (blobMetadata != null)
            {
                WriteBlobMetadata(container, blobId, true, blobMetadata);
            }
        }

        protected override void WriteBlobMetadata(string container, string blobId, bool clearExisting, Dictionary<string, string> blobMetadata)
        {
            string fileName = this.RootPath + container + "\\" + blobId + ".metadata";
            if (!clearExisting)
            {
                //Merge existing metadata
                if (File.Exists(fileName))
                {
                    string originalXml = File.ReadAllText(fileName, Encoding.UTF8);
                    Dictionary<string, string> originalMetadata = ParseMetadataXml(originalXml);
                    foreach (string key in originalMetadata.Keys)
                    {
                        if (!blobMetadata.ContainsKey(key))
                        {
                            blobMetadata.Add(key, originalMetadata[key]);
                        }
                    }
                }
            }
            string metadataXml = MetadataToXml(blobMetadata);
            File.WriteAllBytes(fileName, Encoding.UTF8.GetBytes(metadataXml));
        }

        protected override void DeleteBlob(string container, string blobId)
        {
            string realBlobId = GetActualBlobId(blobId);
            string fileName = this.RootPath + container + "\\" + realBlobId;

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            string fileNameMetadata = this.RootPath + container + "\\" + realBlobId + ".metadata";

            if (File.Exists(fileNameMetadata))
            {
                File.Delete(fileNameMetadata);
            }
        }

        public override void CopyBlob(string containerName, string desContainerName, string blobId)
        {
            if (string.IsNullOrEmpty(blobId)) return;

            //this.Client.RetryPolicy = RetryPolicies.Retry(20, TimeSpan.Zero);
            CreateContainer(desContainerName);
            string fileName = this.RootPath + containerName + "\\" + blobId;
            string desFileName = this.RootPath + desContainerName + "\\" + blobId;

            string fileMetaName = this.RootPath + containerName + "\\" + blobId + ".metadata";
            string desMetaFileName = this.RootPath + desContainerName + "\\" + blobId + ".metadata";

            File.Copy(fileName, desFileName, true); //复制文件
            File.Copy(fileMetaName, desMetaFileName, true); //复制文件
        }
    }
}
