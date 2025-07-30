using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using KC.Framework.Util;
using KC.Framework.Extension;
using KC.Framework.Base;

namespace KC.Storage.BlobService
{
    internal class FileSystemReader : ReaderBase
    {
        private readonly string RootPath;

        public FileSystemReader(string rootPath)
            : base()
        {
            this.RootPath = rootPath;
        }

        public override bool ExistContainer(string container)
        {
            if (string.IsNullOrEmpty(container)) { return false; }
            return Directory.Exists(this.RootPath + container + "\\");
        }

        public override bool ExistBlob(string container, string blobId)
        {
            if (string.IsNullOrEmpty(blobId)) return false;

            string dirName = this.RootPath + container + "\\";
            return File.Exists(dirName + GetActualBlobId(blobId));
        }

        public override List<string> ListContainers()
        {
            var dirName = this.RootPath;
            var result = System.IO.Directory.GetDirectories(dirName)
                .ReplaceFirst(Regex.Replace(this.RootPath, @"\\", @"\\"), string.Empty);
            return result.ToList();
        }

        public override List<string> ListBlobIds(string container)
        {
            if (string.IsNullOrEmpty(container))
                return new List<string>();

            string dirName = this.RootPath + container;
            List<string> allFileNames = System.IO.Directory.GetFiles(dirName).ToList().ConvertAll(f => Path.GetFileName(f));
            return allFileNames.Where(f => !f.EndsWith(".metadata")).ToList();
        }

        public override List<string> ListBlobIdsWithMetadata(string container)
        {
            if (string.IsNullOrEmpty(container))
                return new List<string>();

            string dirName = this.RootPath + container;
            List<string> allFileNames = System.IO.Directory.GetFiles(dirName).ToList().ConvertAll(f => Path.GetFileName(f));
            return allFileNames.ToList();
        }


        private BlobInfo ReadBlob(string containerName, string encryptionKey, bool isInternal, string userId, string blobId)
        {
            if (string.IsNullOrEmpty(blobId) || string.IsNullOrEmpty(containerName)) return null;

            string filePath = this.RootPath + containerName + "\\" + blobId;
            if (!File.Exists(filePath))
                return null;

            Dictionary<string, string> blobMetadata = GetBlobMetadata(containerName, blobId);
            CheckBlobAccess(containerName, isInternal, userId, blobId, blobMetadata);

            string type = blobMetadata.ContainsKey("Type") ? blobMetadata["Type"] : "Unknown";
            string format = blobMetadata.ContainsKey("Format") ? blobMetadata["Format"] : "Unknown";
            string fileName = blobMetadata.ContainsKey("FileName") ? blobMetadata["FileName"] : "Unknown";
            string ext = blobMetadata.ContainsKey("ext") ? blobMetadata["ext"] : "Unknown";
            byte[] data = File.ReadAllBytes(filePath);
            byte[] blobActualData = DecryptIfNeeded(encryptionKey, isInternal, userId, data, blobMetadata);

            if (blobActualData.Length == 0)
                throw new Exception("Blob " + blobId + " has zero length");

            return new BlobInfo(blobId, type, format, fileName, ext, blobActualData);
        }

        public override BlobInfo GetBlob(string container, string encryptionKey, bool isInternal, string userId, string blobId, bool attemptCache)
        {
            var sw = new Stopwatch();
            sw.Start();

            int offset = -1;
            int length = -1;
            string realBlobId = GetActualBlobIdAndOffset(blobId, out offset, out length);

            BlobInfo bi = ReadBlob(container, encryptionKey, isInternal, userId,  realBlobId);

            if (offset != -1)
            {
                GetBlobSegment(bi, offset, length, blobId);
            }

            LogUtil.LogDebug("Retrieve Blob", blobId, sw.ElapsedMilliseconds);
            return bi;
        }

        public override BlobInfo GetBlobWithoutEncryption(string container, string blobId, bool useCacheIfAvailable)
        {
            if (string.IsNullOrEmpty(blobId) || string.IsNullOrEmpty(container)) return null;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            int offset = -1;
            int length = -1;
            string realBlobId = GetActualBlobIdAndOffset(blobId, out offset, out length);

            BlobInfo bi = new BlobInfo(realBlobId, null, null, null, null, null);

            string dirName = this.RootPath + container + "\\";
            if (File.Exists(dirName + realBlobId))
            {
                bi.Data = File.ReadAllBytes(dirName + realBlobId);
                bi.Size = bi.Data.Length;
            }
            else
            {
                throw new Exception("Blob " + blobId + " not found");
            }
            if (bi.Data.Length == 0)
            {
                throw new Exception("Blob " + blobId + " has zero length");
            }

            if (offset != -1)
            {
                bi = GetBlobSegment(bi, offset, length, blobId);
            }

            LogUtil.LogDebug("Retrieve Blob", blobId, sw.ElapsedMilliseconds);
            return bi;
        }

        public override DateTime GetBlobLastModifiedTime(string container, string blobId)
        {
            string fileName = this.RootPath + container + "\\" + blobId;

            if (!File.Exists(fileName))
                return DateTime.MinValue;

            return new FileInfo(fileName).LastWriteTimeUtc;
        }

        public override Dictionary<string, string> GetBlobMetadata(string container, string blobId)
        {
            string fileName = this.RootPath + container + "\\" + blobId + ".metadata";

            if (!File.Exists(fileName))
                return new Dictionary<string,string>();

            string metadataXml = File.ReadAllText(fileName, Encoding.UTF8);
            Dictionary<string, string> blobMetadata = ParseMetadataXml(metadataXml);
            return blobMetadata;
        }
    }
}
