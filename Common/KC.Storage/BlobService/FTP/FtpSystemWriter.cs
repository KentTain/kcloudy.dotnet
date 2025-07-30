using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using KC.Framework.Util;
using KC.Framework.Extension;
using KC.Framework.Tenant;

namespace KC.Storage.BlobService
{
    internal class FtpSystemWriter : WriterBase
    {
        private readonly string endpoint;
        private readonly string accessKey;
        private readonly string secretKey;

        public FtpSystemWriter(string connectionString)
            : base()
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("FTP Server's Writer connect string is empy or null.", "connectionString");

            var keyValues = connectionString.KeyValuePairFromConnectionString();
            endpoint = keyValues[ConnectionKeyConstant.BlobEndpoint];
            accessKey = keyValues[ConnectionKeyConstant.AccessName];
            secretKey = keyValues[ConnectionKeyConstant.AccessKey];
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentException("FTP Server's Writer connect string is wrong. It can't set the BlobEndpoint Value.", "connectionString");
            if (string.IsNullOrWhiteSpace(accessKey))
                throw new ArgumentException("FTP Server's Writer connect string is wrong. It can't set the AccessKey Value.", "connectionString");
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new ArgumentException("FTP Server's Writer connect string is wrong. It can't set the SecretAccountKey Value.", "connectionString");

            endpoint = endpoint.EndWithSlash();
        }

        private FtpWebRequest GetFtpServer(string dirName)
        {
            var ftpServer = (FtpWebRequest)FtpWebRequest.Create(new Uri(dirName));
            ftpServer.Credentials = new NetworkCredential(accessKey, secretKey);
            ftpServer.KeepAlive = false;
            ftpServer.UseBinary = true;
            //ftpServer.EnableSsl = true;
            return ftpServer;
        }

        protected override void CreateContainer(string container)
        {
            var dirName = this.endpoint + container;
            FtpWebResponse response = null;
            Stream stream = null;
            StreamReader reader = null;
            try
            {
                // dirName = name of the directory to create.
                var reqFTP = GetFtpServer(dirName);
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                reqFTP.UseBinary = true;
                response = (FtpWebResponse) reqFTP.GetResponse();
                stream = response.GetResponseStream();
                if (stream == null) return;

                reader = new StreamReader(stream);
                var result = reader.ReadToEnd();
                LogUtil.LogInfo("---CreateContainer result: " + result);
            }
            catch (Exception ex)
            {
                LogUtil.LogError("FtpSystemWriter", dirName + " CreateContainer Error: " + ex.Message);
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

        protected override void DeleteContainer(string container)
        {
            var dirName = this.endpoint + container;
            FtpWebResponse response = null;
            Stream stream = null;
            StreamReader reader = null;
            try
            {
                // dirName = name of the directory to create.
                var reqFTP = GetFtpServer(dirName);
                reqFTP.Method = WebRequestMethods.Ftp.RemoveDirectory;
                reqFTP.UseBinary = true;

                response = (FtpWebResponse)reqFTP.GetResponse();
                long size = response.ContentLength;
                stream = response.GetResponseStream();
                if (stream == null) return;
                
                reader = new StreamReader(stream);
                var result = reader.ReadToEnd();
                LogUtil.LogInfo("---DeleteContainer result: " + result);
            }
            catch (Exception ex)
            {
                LogUtil.LogError("FtpSystemWriter", dirName + " DeleteContainer Error: " + ex.Message);
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

        protected override void WriteBlob(string container, string blobId, byte[] blobData, Dictionary<string, string> blobMetadata)
        {
            //CreateContainer(container);

            Stream stream = null;
            var dirName = this.endpoint + container + "/" + blobId;
            try
            {
                var reqFTP = GetFtpServer(dirName);
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
                reqFTP.UseBinary = true;
                reqFTP.ContentLength = blobData.Length;

                stream = reqFTP.GetRequestStream();
                if (stream == null) return;

                stream.Write(blobData, 0, blobData.Length);
            }
            catch (Exception ex)
            {
                LogUtil.LogError("FtpSystemWriter", dirName + " WriteBlob Error: " + ex.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            if (blobMetadata != null)
            {
                WriteBlobMetadata(container, blobId, true, blobMetadata);
            }
        }

        protected override void WriteBlobMetadata(string container, string blobId, bool clearExisting, Dictionary<string, string> blobMetadata)
        {
            var metadataXml = MetadataToXml(blobMetadata);
            var blobData = Encoding.UTF8.GetBytes(metadataXml);
            WriteBlob(container, blobId + ".metadata", blobData, null);
        }

        protected override void DeleteBlob(string container, string blobId)
        {
            var dirName = this.endpoint + container + "/" + blobId;
            FtpWebResponse response = null;
            Stream stream = null;
            StreamReader reader = null;
            try
            {
                // dirName = name of the directory to create.
                var reqFTP = GetFtpServer(dirName);
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;
                reqFTP.UseBinary = true;

                response = (FtpWebResponse)reqFTP.GetResponse();
                long size = response.ContentLength;
                stream = response.GetResponseStream();
                if (stream == null) return;

                reader = new StreamReader(stream);
                var result = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                LogUtil.LogError("FtpSystemWriter", dirName + " DeleteBlob Error: " + ex.Message);
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

        public override void CopyBlob(string container, string descontainer, string blobId)
        {
            if (string.IsNullOrEmpty(blobId)) return;

            //this.Client.RetryPolicy = RetryPolicies.Retry(20, TimeSpan.Zero);
            CreateContainer(descontainer);
            string fileName = this.endpoint + container + "/" + blobId;
            string desFileName = this.endpoint + descontainer + "/" + blobId;

            string fileMetaName = this.endpoint + container + "/" + blobId + ".metadata";
            string desMetaFileName = this.endpoint + descontainer + "/" + blobId + ".metadata";

            CopyTo(fileName, desFileName); //复制文件
            CopyTo(fileMetaName, desMetaFileName); //复制文件
        }

        private bool CopyTo(string sourceFtpUrl, string targetFtpUrl)
        {
            FtpWebResponse sourceResponse = null;
            Stream sourceStream = null;

            Stream targetStream = null;
            try
            {
                var sourceServer = GetFtpServer(sourceFtpUrl);
                sourceServer.Method = WebRequestMethods.Ftp.DownloadFile;
                sourceResponse = (FtpWebResponse)sourceServer.GetResponse();
                sourceStream = sourceResponse.GetResponseStream();
                if (sourceStream == null) return false;

                byte[] data = ToByteArray(sourceStream);

                var targetServer = GetFtpServer(targetFtpUrl);
                targetServer.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
                targetStream = targetServer.GetRequestStream();
                targetStream.Write(data, 0, data.Length);

                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogError("FtpSystemReader", "GetBlobWithoutEncryption Error: " + ex.Message);
                return false;
            }
            finally
            {
                if (sourceStream != null)
                    sourceStream.Close();
                if (sourceResponse != null)
                    sourceResponse.Close();

                if (targetStream != null)
                    targetStream.Close();
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
    }
}
