using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Diagnostics;
using KC.Framework.Util;
using KC.Framework.Extension;
using KC.Framework.Base;
using KC.Framework.Tenant;

namespace KC.Storage.BlobService
{
    internal class FtpSystemReader : ReaderBase
    {
        private readonly string endpoint;
        private readonly string accessKey;
        private readonly string secretKey;

        public FtpSystemReader(string connectionString)
            : base()
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("FTP Server's reader connect string is empy or null.", "connectionString");

            var keyValues = connectionString.KeyValuePairFromConnectionString();
            endpoint = keyValues[ConnectionKeyConstant.BlobEndpoint];
            accessKey = keyValues[ConnectionKeyConstant.AccessName];
            secretKey = keyValues[ConnectionKeyConstant.AccessKey];
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentException("FTP Server's  reader connect string is wrong. It can't set the BlobEndpoint Value.", "connectionString");
            if (string.IsNullOrWhiteSpace(accessKey))
                throw new ArgumentException("FTP Server's  reader connect string is wrong. It can't set the AccessKey Value.", "connectionString");
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new ArgumentException("FTP Server's  reader connect string is wrong. It can't set the SecretAccountKey Value.", "connectionString");
            
            endpoint = endpoint.EndWithSlash();
        }

        private FtpWebRequest GetFtpServer(string dirName)
        {
            var ftpServer = (FtpWebRequest)FtpWebRequest.Create(new Uri(dirName));
            ftpServer.Credentials = new NetworkCredential(accessKey, secretKey);
            ftpServer.KeepAlive = false;
            //ftpServer.EnableSsl = true;
            return ftpServer;
        }

        public override bool ExistContainer(string container)
        {
            var dirList = ListContainers();
            foreach (var str in dirList)
            {
                if (str.Trim() == container.Trim())
                {
                    return true;
                }
            }
            return false;
        }

        public override bool ExistBlob(string container, string blobId)
        {
            var fileList = ListBlobIds(container);
            foreach (var str in fileList)
            {
                if (str.Trim() == blobId.Trim())
                {
                    return true;
                }
            }
            return false;
        }

        public override List<string> ListContainers()
        {
            FtpWebResponse response = null;
            Stream stream = null;
            StreamReader reader = null;
            try
            {
                var ftp = GetFtpServer(this.endpoint);
                ftp.Method = WebRequestMethods.Ftp.ListDirectory;
                response = (FtpWebResponse)ftp.GetResponse();
                stream = response.GetResponseStream();
                if (stream == null) return new List<string>();

                reader = new StreamReader(stream, Encoding.Default);
                var line = reader.ReadLine();
                var result = new List<string>();
                while (line != null)
                {
                    result.Add(line);
                    line = reader.ReadLine();
                }

                //var m = string.Empty;
                //foreach (var str in result)
                //{
                //    int dirPos = str.IndexOf("<DIR>");
                //    if (dirPos > 0)
                //    {
                //        /*判断 Windows 风格*/
                //        m += str.Substring(dirPos + 5).Trim() + "\n";
                //    }
                //    else if (str.Trim().Substring(0, 1).ToUpper() == "D")
                //    {
                //        /*判断 Unix 风格*/
                //        var dir = str.Substring(54).Trim();
                //        if (dir != "." && dir != "..")
                //        {
                //            m += dir + "\n";
                //        }
                //    }
                //}

                return result;
            }
            catch (Exception ex)
            {
                LogUtil.LogError("FtpSystemReader", "ListContainers Error: " + ex.Message);
                return new List<string>();
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (response != null)
                    response.Close();
                if (reader != null)
                    reader.Close();
            }
        }

        public override List<string> ListBlobIds(string container)
        {
            if (string.IsNullOrEmpty(container))
                return new List<string>();

            FtpWebResponse response = null;
            Stream stream = null;
            StreamReader reader = null;
            var dirName = this.endpoint + container;
            try
            {
                var ftp = GetFtpServer(dirName);
                ftp.Method = WebRequestMethods.Ftp.ListDirectory;
                response = (FtpWebResponse)ftp.GetResponse();
                stream = response.GetResponseStream();
                if (stream == null) return new List<string>();

                reader = new StreamReader(stream, Encoding.Default);
                var line = reader.ReadLine();
                var result = new List<string>();
                while (line != null)
                {
                    result.Add(line);
                    line = reader.ReadLine();
                }

                return result;
            }
            catch (Exception ex)
            {
                LogUtil.LogError("FtpSystemReader", dirName + " ListBlobIds Error: " + ex.Message);
                return new List<string>();
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (response != null)
                    response.Close();
                if (reader != null)
                    reader.Close();
            }
        }

        public override List<string> ListBlobIdsWithMetadata(string container)
        {
            return ListBlobIds(container);
        }

        private BlobInfo ReadBlob(string containerName, string encryptionKey, bool isInternal, string userId, string blobId)
        {
            if (string.IsNullOrEmpty(blobId) || string.IsNullOrEmpty(containerName)) return null;

            var dirName = this.endpoint + containerName + "/" + blobId;
            FtpWebResponse response = null;
            Stream stream = null;
            try
            {
                var ftp = GetFtpServer(dirName);
                ftp.Method = WebRequestMethods.Ftp.DownloadFile;
                response = (FtpWebResponse)ftp.GetResponse();
                stream = response.GetResponseStream();
                if (stream == null) return null;

                byte[] data = ToByteArray(stream);
                if (data.Length == 0)
                    throw new Exception("Blob " + blobId + " has zero length");

                Dictionary<string, string> blobMetadata = GetBlobMetadata(containerName, blobId);
                CheckBlobAccess(containerName, isInternal, userId, blobId, blobMetadata);

                string type = blobMetadata.ContainsKey("Type") ? blobMetadata["Type"] : "Unknown";
                string format = blobMetadata.ContainsKey("Format") ? blobMetadata["Format"] : "Unknown";
                string fileName = blobMetadata.ContainsKey("FileName") ? blobMetadata["FileName"] : "Unknown";
                string ext = blobMetadata.ContainsKey("ext") ? blobMetadata["ext"] : "Unknown";
                byte[] blobActualData = DecryptIfNeeded(encryptionKey, isInternal, userId, data, blobMetadata);

                if (blobActualData.Length == 0)
                    throw new Exception("Blob " + blobId + " has zero length");

                return new BlobInfo(blobId, type, format, fileName, ext, blobActualData);
            }
            catch (Exception ex)
            {
                LogUtil.LogError("FtpSystemReader", dirName + " GetBlobMetadata Error: " + ex.Message);
                return null;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (response != null)
                    response.Close();
            }
        }

        private Byte[] ToByteArray(Stream stream)
        {
            MemoryStream ms = new MemoryStream();
            byte[] chunk = new byte[4096];
            int bytesRead;
            while ((bytesRead = stream.Read(chunk, 0, chunk.Length)) > 0)
            {
                ms.Write(chunk, 0, bytesRead);
            }

            return ms.ToArray();
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

            var bi = new BlobInfo(realBlobId, null, null, null, null);
            if (ExistBlob(container, realBlobId))
            {
                var dirName = this.endpoint + container + "/" + blobId;
                FtpWebResponse response = null;
                Stream stream = null;
                try
                {
                    var ftp = GetFtpServer(dirName);
                    ftp.Method = WebRequestMethods.Ftp.DownloadFile;
                    response = (FtpWebResponse)ftp.GetResponse();
                    stream = response.GetResponseStream();
                    if (stream == null) return null;

                    byte[] data = ToByteArray(stream);
                    if (data.Length == 0)
                        throw new Exception("Blob " + blobId + " has zero length");

                    bi.Data = data;
                    bi.Size = data.Length;
                }
                catch (Exception ex)
                {
                    LogUtil.LogError("FtpSystemReader", dirName + " GetBlobWithoutEncryption Error: " + ex.Message);
                    return null;
                }
                finally
                {
                    if (stream != null)
                        stream.Close();
                    if (response != null)
                        response.Close();
                }
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
            var dirName = this.endpoint + container + "/" + blobId;
            FtpWebResponse response = null;
            Stream stream = null;
            try
            {
                var ftp = GetFtpServer(dirName);
                ftp.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                response = (FtpWebResponse)ftp.GetResponse();
                stream = response.GetResponseStream();
                if (stream == null) return DateTime.MinValue;

                return response.LastModified;
            }
            catch (Exception ex)
            {
                LogUtil.LogError("FtpSystemReader", dirName + " GetBlobLastModifiedTime Error: " + ex.Message);
                return DateTime.MinValue;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (response != null)
                    response.Close();
            }
        }

        public override Dictionary<string, string> GetBlobMetadata(string container, string blobId)
        {
            var dirName = this.endpoint + container + "/" + blobId + ".metadata";

            FtpWebResponse response = null;
            Stream stream = null;
            StreamReader reader = null;
            try
            {
                var ftp = GetFtpServer(dirName);
                ftp.Method = WebRequestMethods.Ftp.DownloadFile;
                response = (FtpWebResponse) ftp.GetResponse();
                stream = response.GetResponseStream();
                if (stream == null) return new Dictionary<string, string>();

                reader = new StreamReader(stream, Encoding.Default);
                var metadataXml = reader.ReadToEnd();

                var blobMetadata = ParseMetadataXml(metadataXml);
                return blobMetadata;
            }
            catch (Exception ex)
            {
                LogUtil.LogError("FtpSystemReader", "GetBlobMetadata Error: " + ex.Message);
                return new Dictionary<string, string>();
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (response != null)
                    response.Close();
                if (reader != null)
                    reader.Close();
            }
        }
    }
}
