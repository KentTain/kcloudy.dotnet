using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using KC.Storage.Util;
using KC.Framework.Base;
using KC.Framework.Extension;

namespace KC.Storage.BlobService
{
    public abstract class BlobBase
    {
        protected internal static string GetActualBlobId(string blobId)
        {
            int offset;
            int length;
            return GetActualBlobIdAndOffset(blobId, out offset, out length);
        }

        protected internal static string GetActualBlobIdAndOffset(string blobId, out int offset, out int length)
        {
            offset = -1;
            length = -1;

            if (!blobId.Contains("_"))
            {
                return blobId;
            }

            try
            {
                string[] splitStr = blobId.Split('_');
                if (blobId.StartsWith("t") && splitStr.Length == 5)
                {
                    blobId = splitStr[0];
                    if (!int.TryParse(splitStr[3], out offset))
                    {
                        return blobId;
                    }

                    if (!int.TryParse(splitStr[4], out length))
                    {
                        return blobId;
                    }
                }
                else if (splitStr.Length == 3 || splitStr.Length == 5)
                {
                    blobId = splitStr[0];
                    if (!int.TryParse(splitStr[1], out offset))
                    {
                        return blobId;
                    }

                    if (!int.TryParse(splitStr[2], out length))
                    {
                        return blobId;
                    }
                }
            }
            catch (Exception)
            {
            }
            return blobId;
        }

        protected internal static BlobInfo GetBlobSegment(BlobInfo bi, int offset, int length, string segBlobId)
        {
            if (bi == null) return null;

            bi.Id = segBlobId;
            bi.Size = length;
            byte[] chunkData = new byte[length];
            Buffer.BlockCopy(bi.Data, offset, chunkData, 0, length);
            bi.Data = chunkData;
            return bi;
        }

        protected internal static string GetModifiedTime()
        {
            return DateTime.UtcNow.ToString(DateTimeExtensions.FMT_yMdHms1, CultureInfo.GetCultureInfo("en-us"));
        }


        protected static void CheckBlobAccess(string tenant, bool isInternal, string userId, string blobId, IDictionary<string, string> blobMetadata)
        {
            if (isInternal) return;

            // Tenant check
            if (blobMetadata.ContainsKey("Tenant"))
            {
                if (!string.Equals(tenant, blobMetadata["Tenant"]))
                {
                    throw new Exception("Blob Access Violation: " + blobId + " belongs to another tenant.");
                }
            }

            bool isUserLevel = blobMetadata.ContainsKey("IsUserLevelBlob") ? bool.Parse(blobMetadata["IsUserLevelBlob"]) : false;
            if (isUserLevel && blobMetadata.ContainsKey("UserId"))
            {
                string checkId = isInternal ? "Internal" : userId;
                if (!string.Equals(checkId, blobMetadata["UserId"]))
                {
                    throw new Exception("Blob Access Violation: " + blobId + " belongs to another user.");
                }
            }

            // Expiration Check
            if (blobMetadata.ContainsKey("Expiration"))
            {
                long expiration = long.Parse(blobMetadata["Expiration"]);
                if (expiration > 0)
                {
                    long createTicks = blobMetadata.ContainsKey("CreateTime") ? long.Parse(blobMetadata["CreateTime"]) : 0;
                    long elapsedTicks = DateTime.UtcNow.Ticks - createTicks;
                    TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
                    if (elapsedSpan.TotalSeconds > expiration)
                    {
                        throw new Exception("Blob Access Violation: " + blobId + " has already expired.");
                    }
                }
            }
        }

        protected static byte[] DecryptIfNeeded(string encryptionKey, bool isInternal, string userId, byte[] data, IDictionary<string, string> metadata)
        {
            bool isEncrypted = metadata.ContainsKey("IsEncrypted") ? isEncrypted = bool.Parse(metadata["IsEncrypted"]) : false;
            if (isEncrypted)
            {
                bool isUserLevelBlob = metadata.ContainsKey("IsUserLevelBlob") ? bool.Parse(metadata["IsUserLevelBlob"]) : false;
                string realEncryptionKey = Encryption.GetEncryptionKey(encryptionKey, isUserLevelBlob, isInternal, userId);
                return Encryption.Decrypt(data, realEncryptionKey);
            }
            else
            {
                return data;
            }
        }

        public static Dictionary<string, string> BuildMetadata(string tenant, bool isInternal, string userId, BlobInfo blobInfo, long expireAfterSecond, bool isEncypted, bool isUserLevelBlob, Dictionary<string, string> extraMetadata = null)
        {
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            metadata.Add("Tenant", tenant);
            metadata.Add("UserId", isInternal ? "Internal" : userId);
            metadata.Add("Id", blobInfo.Id);
            metadata.Add("Type", string.IsNullOrWhiteSpace(blobInfo.FileType) ? "Unknown" : blobInfo.FileType);
            metadata.Add("Format", string.IsNullOrWhiteSpace(blobInfo.FileFormat) ? "Unknown" : blobInfo.FileFormat);
            metadata.Add("FileName", blobInfo.FileName);
            metadata.Add("Ext", blobInfo.Ext);
            metadata.Add("Size", blobInfo.Size + string.Empty);
            metadata.Add("CreateTime", DateTime.UtcNow.Ticks.ToString());
            metadata.Add("LastModifiedTime", GetModifiedTime());
            metadata.Add("Expiration", expireAfterSecond.ToString());
            metadata.Add("IsEncrypted", isEncypted ? "True" : "False");
            metadata.Add("IsUserLevelBlob", isUserLevelBlob ? "True" : "False");
            if (extraMetadata != null)
            {
                foreach (var metadataItem in extraMetadata)
                {
                    metadata.Add(metadataItem.Key, metadataItem.Value);
                }
            }
            return metadata;
        }

        protected internal static string MetadataToXml(IDictionary<string, string> metadata)
        {
            StringBuilder metadataXmlSb = new StringBuilder();
            metadataXmlSb.Append("<Metadata>");
            foreach (var metadataEntry in metadata)
            {
                metadataXmlSb.Append(string.Format("<{0}>{1}</{0}>", metadataEntry.Key, metadataEntry.Value));
            }
            metadataXmlSb.Append("</Metadata>");
            return metadataXmlSb.ToString();
        }

        protected internal static Dictionary<string, string> ParseMetadataXml(string metadataXmlString)
        {
            Dictionary<string, string> metadata = new Dictionary<string, string>();

            try
            {
                XDocument metadataXDoc = XDocument.Parse(metadataXmlString);
                if (metadataXDoc.Root != null)
                {
                    foreach (XElement descendant in metadataXDoc.Root.Descendants())
                    {
                        metadata.Add(descendant.Name.LocalName, descendant.Value);
                    }
                }
            }
            catch
            {
            }

            return metadata;
        }
    }
}