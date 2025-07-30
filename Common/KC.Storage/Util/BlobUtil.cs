using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KC.Storage.Util.BlobHelper;
using KC.Framework.Util;
using KC.Framework.Extension;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Framework.SecurityHelper;
using KC.Common.FileHelper;

namespace KC.Storage.Util
{
    public static class BlobUtil
    {
        public const string PrefixSmallThumbnailName = "t";
        public const string BlobTempContainer = "clienttempforregister";

        /// <summary>
        /// 保存文件到Azure Blob中：二进制流文件
        /// </summary>
        /// <param name="tenant">文档所在的源租户：Tenant</param>
        /// <param name="userId">上传文档用户Id</param>
        /// <param name="buffer">文档二进制</param>
        /// <param name="blobId">文档ID</param>
        /// <param name="fileType">文档类型：FileType</param>
        /// <param name="fileFormat">文档格式：如果FileType.Image，那么文档格式为：ImageFormat</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="isTempAuth">是否保存到临时文件夹</param>
        /// <param name="with">With of the Thumbnail</param>
        /// <param name="height">Heith of the Thumbnail</param>
        public static bool SaveBlob(Tenant tenant, string userId, BlobInfo blobInfo, bool isTempAuth = false, int? with = null, int? height = null)
        {
            try
            {
                var blobProvider = new BlobProvider(tenant);
                var container = tenant.TenantName.ToLower();
                var authToken = GetAuthToken(container, userId, isTempAuth);
                //string encodeFileName = Base64Provider.EncodeString(fileName);
                //var bi = new BlobInfo(blobId, fileType, fileFormat, encodeFileName, buffer);
                blobProvider.SaveBlob(authToken, blobInfo);

                if (blobInfo.FileType == KC.Common.FileHelper.FileType.Image.ToString() && with != null && height != null)
                {
                    var imageThumbnailUtil = new KC.Common.FileHelper.ImageThumbnailMake(blobInfo.Data);
                    var format = (KC.Common.FileHelper.ImageFormat)Enum.Parse(typeof(KC.Common.FileHelper.ImageFormat), blobInfo.FileFormat);
                    int iWith = with != 0 ? (int)with : 16;
                    int iHeight = height != 0 ? (int)height : 16;
                    byte[] bytes = imageThumbnailUtil.GetOutputBytes(iWith, iHeight, format);
                    var tbi = blobInfo.Clone();
                    tbi.Id = PrefixSmallThumbnailName + blobInfo.Id;
                    tbi.Data = bytes;
                    blobProvider.SaveBlob(authToken, tbi);
                }

                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                if (ex.InnerException != null)
                    LogUtil.LogException(ex.InnerException);
                return false;
            }
        }

        /// <summary>
        /// 保存文件到Azure Blob中：本地的文档路径
        /// </summary>
        /// <param name="tenant">文档所在的源租户：Tenant</param>
        /// <param name="userId">上传文档用户Id</param>
        /// <param name="filePath">本地文档所在的路径</param>
        /// <param name="blobId">文档ID</param>
        /// <param name="fileType">文档类型：FileType</param>
        /// <param name="fileFormat">文档格式：如果FileType.Image，那么文档格式为：ImageFormat</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="isTempAuth">是否保存到临时文件夹</param>
        /// <param name="with">With of the Thumbnail</param>
        /// <param name="height">Heith of the Thumbnail</param>
        public static bool SaveBlob(Tenant tenant, string userId, string filePath, BlobInfo blobInfo, bool isTempAuth = false, int? with = null, int? height = null)
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var fsLen = (int)stream.Length;
                var heByte = new byte[fsLen];
                stream.Read(heByte, 0, fsLen);
                blobInfo.Data = heByte;
                return SaveBlob(tenant, userId, blobInfo, isTempAuth, with, height);
            }
        }

        /// <summary>
        /// 将临时文件夹中的文件拷贝到用户文件夹下
        /// </summary>
        /// <param name="tenant">文档所在的源租户：Tenant</param>
        /// <param name="userId">上传文档用户Id，可以为空</param>
        /// <param name="blobIds">修转移的文档ID列表</param>
        /// <param name="containThumbnail">是否转移Thumbnail文件</param>
        /// <returns></returns>
        public static bool CopyTempsToClientBlob(Tenant tenant, List<string> blobIds, string userId, bool containThumbnail = false)
        {
            try
            {
                var blobProvider = new BlobProvider(tenant);
                var container = tenant.TenantName.ToLower();

                var tempAuthToken = GetAuthToken(container, userId, true);
                var authToken = GetAuthToken(container, userId, false);

                blobProvider.CopyBlobs(tempAuthToken.ContainerName, authToken.ContainerName, blobIds.ToArray());
                //blobProvider.DeleteBlob(tempAuthToken, blobIds.ToArray());
                if (containThumbnail)
                {
                    var thumbBlobIds = blobIds.FixStringList(PrefixSmallThumbnailName, "");
                    blobProvider.CopyBlobs(tempAuthToken.ContainerName, authToken.ContainerName, thumbBlobIds.ToArray());
                    //blobProvider.DeleteBlob(tempAuthToken, thumbBlobIds.ToArray());
                }

                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return false;
            }

        }
        /// <summary>
        /// 将租户文件夹中的文件拷贝到其他租户文件夹下(非剪切)
        /// </summary>
        /// <param name="sourceTenant">文档所在的源租户：Tenant</param>
        /// <param name="targetTenant">移至指定的目标租户：Tenant</param>
        /// <param name="userId">上传文档用户Id，可以为空</param>
        /// <param name="blobIds">修转移的文档ID列表</param>
        /// <param name="containThumbnail">是否转移Thumbnail文件</param>
        /// <returns></returns>
        public static bool CopyBlobsToOtherClient(Tenant sourceTenant, Tenant targetTenant, List<string> blobIds, string userId, bool containThumbnail = false)
        {
            try
            {
                var blobProvider = new BlobProvider(sourceTenant);
                var container = sourceTenant.TenantName.ToLower();

                var authToken = GetAuthToken(container, userId, false);
                blobProvider.CopyBlobsToOtherTenant(targetTenant, blobIds.ToArray(), authToken.Id,
                    authToken.EncryptionKey, authToken.Internal);
                //BlobProvider.DeleteBlob(tempAuthToken, blobIds.ToArray());
                if (containThumbnail)
                {
                    var thumbBlobIds = blobIds.FixStringList(PrefixSmallThumbnailName, "");
                    blobProvider.CopyBlobsToOtherTenant(targetTenant, thumbBlobIds.ToArray(), authToken.Id,
                        authToken.EncryptionKey, authToken.Internal);
                    //BlobProvider.DeleteBlob(tempAuthToken, thumbBlobIds.ToArray());
                }

                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return false;
            }

        }
        /// <summary>
        /// 将租户文件夹中的所有文件拷贝到其他租户文件夹下(非剪切)
        /// </summary>
        /// <param name="sourceTenant">文档所在的源租户：Tenant</param>
        /// <param name="targetTenant">移至指定的目标租户：Tenant</param>
        /// <param name="userId">上传文档用户Id，可以为空</param>
        /// <param name="containThumbnail">是否转移Thumbnail文件</param>
        /// <returns></returns>
        public static bool CopyAllBlobsToOtherClient(Tenant sourceTenant, Tenant targetTenant, string userId, bool containThumbnail = false)
        {
            try
            {
                var blobProvider = new BlobProvider(sourceTenant);
                var container = sourceTenant.TenantName.ToLower();

                var blobIds = blobProvider.GetAllBlobIds(container);
                var authToken = GetAuthToken(container, userId, false);
                blobProvider.CopyBlobsToOtherTenant(targetTenant, blobIds.ToArray(), authToken.Id,
                    authToken.EncryptionKey, authToken.Internal);
                //BlobProvider.DeleteBlob(tempAuthToken, blobIds.ToArray());
                if (containThumbnail)
                {
                    var thumbBlobIds = blobIds.FixStringList(PrefixSmallThumbnailName, "");
                    blobProvider.CopyBlobsToOtherTenant(targetTenant, thumbBlobIds.ToArray(), authToken.Id,
                        authToken.EncryptionKey, authToken.Internal);
                    //BlobProvider.DeleteBlob(tempAuthToken, thumbBlobIds.ToArray());
                }

                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return false;
            }

        }
        /// <summary>
        /// 将源文件生产一个新文件
        /// </summary>
        /// <param name="tenant">需要查询的Tenant</param>
        /// <param name="userId">上传文档用户Id，可为空</param>
        /// <param name="blobId">文档Id：BlobId</param>
        /// <returns>新文件BlobId</returns>
        public static string TransfromBlob(Tenant tenant, string userId, string blobId)
        {
            try
            {
                var blobProvider = new BlobProvider(tenant);
                var container = tenant.TenantName.ToLower();

                var newBlodId = Guid.NewGuid().ToString().ToLower();
                var authToken = GetAuthToken(container, userId, false);
                var blob = blobProvider.GetBlob(authToken, blobId);
                if (blob != null && blob.Data != null)
                {
                    var bi = new BlobInfo(newBlodId, blob.FileType, blob.FileFormat, blob.FileName, blob.Ext, blob.Data);
                    blobProvider.SaveBlob(authToken, bi);

                    return newBlodId;
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
            }

            return string.Empty;
        }
        
        /// <summary>
        /// 根据文件Id查询Tenant下的文件
        /// </summary>
        /// <param name="tenant">需要查询的Tenant</param>
        /// <param name="userId">上传文档用户Id，可为空</param>
        /// <param name="blobId">文档Id：BlobId</param>
        /// <param name="isTempAuth">是否在临时文件夹下</param>
        /// <returns>文件BlobInfo</returns>
        public static BlobInfo GetBlobById(Tenant tenant, string userId, string blobId, bool isTempAuth = false)
        {
            try
            {
                var blobProvider = new BlobProvider(tenant);
                var container = tenant.TenantName.ToLower();

                var authToken = GetAuthToken(container, userId, isTempAuth);
                return blobProvider.GetBlob(authToken, blobId);
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return null;
            }
        }
        /// <summary>
        /// 获取Tenant下的所有文件
        /// </summary>
        /// <param name="tenant">需要查询的Tenant</param>
        /// <param name="userId">上传文档用户Id，可为空</param>
        /// <returns></returns>
        public static List<BlobInfo> GetContainerBlobs(Tenant tenant, string userId)
        {
            try
            {
                var blobProvider = new BlobProvider(tenant);
                var container = tenant.TenantName.ToLower();

                var authToken = GetAuthToken(container, userId, false);
                var blobs = blobProvider.GetBlobs(authToken);

                return blobs;
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return null;
            }
        }
        
        /// <summary>
        /// 删除Tenant下的一个文件
        /// </summary>
        /// <param name="tenant">需要查询的Tenant</param>
        /// <param name="userId">上传文档用户Id，可为空</param>
        /// <param name="blobId">文档Id：BlobId</param>
        /// <returns>是否成功</returns>
        public static bool RemoveBlob(Tenant tenant, string userId, string blobId)
        {
            try
            {
                var blobProvider = new BlobProvider(tenant);
                var container = tenant.TenantName.ToLower();

                var authToken = GetAuthToken(container, userId, false);
                var blobIds = new string[] { blobId };
                blobProvider.DeleteBlob(authToken, blobIds);

                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return false;
            }

        }
        /// <summary>
        /// 删除Tenant下的多个文件
        /// </summary>
        /// <param name="tenant">需要查询的Tenant</param>
        /// <param name="userId">上传文档用户Id，可为空</param>
        /// <param name="blobIds">文档Id列表：List<BlobId></BlobId></param>
        /// <returns>是否成功</returns>
        public static bool RemoveBlobs(Tenant tenant, string userId, List<string> blobIds)
        {
            try
            {
                var blobProvider = new BlobProvider(tenant);
                var container = tenant.TenantName.ToLower();

                var authToken = GetAuthToken(container, userId, false);
                blobProvider.DeleteBlob(authToken, blobIds.ToArray());

                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return false;
            }
        }
        /// <summary>
        /// 文件Id在Tenant下的是否存在
        /// </summary>
        /// <param name="tenant">文档所在文件夹</param>
        /// <param name="blobId">文档Id：BlobId</param>
        /// <returns>是否存在</returns>
        public static bool ExistBlob(Tenant tenant, string blobId)
        {
            try
            {
                var blobProvider = new BlobProvider(tenant);
                var container = tenant.TenantName.ToLower();

                return blobProvider.DoesBlobExist(container, blobId);
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return false;
            }
        }
        
        /// <summary>
        /// 获取Tenant所在存储中的所有租户的租户列表
        /// </summary>
        /// <param name="tenant">需要查询的Tenant</param>
        /// <returns></returns>
        public static List<string> GetContainers(Tenant tenant)
        {
            try
            {
                var blobProvider = new BlobProvider(tenant);
                return blobProvider.GetContainers();
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return null;
            }
        }
        /// <summary>
        /// 获取Tenant下的所有文件的Id列表
        /// </summary>
        /// <param name="tenant">需要查询的Tenant</param>
        /// <param name="userId">上传文档用户Id，可为空</param>
        /// <returns></returns>
        public static List<string> GetContainerBlobIds(Tenant tenant, string userId)
        {
            try
            {
                var blobProvider = new BlobProvider(tenant);
                var container = tenant.TenantName.ToLower();

                var authToken = GetAuthToken(container, userId, false);
                var blobs = blobProvider.GetAllBlobIds(authToken);

                return blobs;
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return null;
            }
        }
        /// <summary>
        /// 获取Tenant下的某种类型的文件的Id列表
        /// </summary>
        /// <param name="tenant">需要查询的Tenant</param>
        /// <param name="userId">上传文档用户Id，可为空</param>
        /// <param name="fileType">查询的文件类型</param>
        /// <returns></returns>
        public static IEnumerable<string> GetTenantBlobIdsWithType(Tenant tenant, string userId, KC.Common.FileHelper.FileType fileType)
        {
            try
            {
                var blobs = GetContainerBlobs(tenant, userId);

                return blobs.Where(b => b.FileType == fileType.ToString()).Select(b => b.Id);
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return null;
            }
        }
        
        /// <summary>
        /// 获取Tenant下Blob文件的最后修改时间
        /// </summary>
        /// <param name="tenant">需要查询的Tenant</param>
        /// <param name="blobId">文档Id：BlobId</param>
        /// <returns></returns>
        public static DateTime GetBlobLastModifiedTime(Tenant tenant, string blobId)
        {
            var blobProvider = new BlobProvider(tenant);
            var container = tenant.TenantName.ToLower();
            return blobProvider.GetBlobLastModifiedTime(container, blobId);
        }

        private static AuthToken GetAuthToken(string container, string userId, bool isTempAuth)
        {
            if (isTempAuth)
            {
                return new AuthToken()
                {
                    Id = RoleConstants.AdminUserId.ToLower(),
                    ContainerName = BlobUtil.BlobTempContainer.ToLower(),
                    Tenant = TenantConstant.DbaTenantName.ToLower(),
                    DomainName = container.ToLower(),
                    Internal = true,
                    EncryptionKey = string.Empty
                };
            }

            return new AuthToken()
            {
                Id = userId,
                ContainerName = container.ToLower(),
                Tenant = container.ToLower(),
                DomainName = container.ToLower(),
                Internal = true,
                EncryptionKey = string.Empty
            };
        }

        public static bool CopyBlobToTemp(Tenant tenant, string copyUserId, string copyblobId, string blobId,string userId=null)
        {
            var blobInfo = GetBlobById(tenant, copyUserId, copyblobId);
            if (blobInfo == null || blobInfo.Data.IsNullOrEmpty())
                return false;
            if (string.IsNullOrWhiteSpace(userId))
                userId = copyUserId;
            return SaveBlob(tenant, userId, blobInfo, true);
        }

        public static string TestStorageConnection(CloudType storageType, string connectionString, byte[] heByte)
        {
            KC.Storage.BlobService.IBlobProvider Provider;
            switch (storageType)
            {
                case CloudType.FileSystem:
                    if (connectionString.StartsWith("ftp://"))
                    {
                        Provider = new KC.Storage.BlobService.FtpSystemProvider(connectionString);
                    }
                    else
                    {
                        Provider = new KC.Storage.BlobService.FileSystemProvider(connectionString);
                    }
                    break;
                case CloudType.Azure:
                    {
                        Provider = new KC.Storage.BlobService.AzureProvider(connectionString);
                    }
                    break;
                case CloudType.AWS:
                    {
                        Provider = new KC.Storage.BlobService.AwsS3Provider(connectionString);
                    }
                    break;
                case CloudType.Aliyun:
                    {
                        Provider = new KC.Storage.BlobService.AliyunOSSProvider(connectionString);
                    }
                    break;
                default:
                    {
                        throw new ArgumentException("Unknown storage type: " + storageType);
                    }
                    //break;
            }

            var tenant = TenantConstant.DbaTenantName.ToLower();
            var userId = RoleConstants.AdminUserId.ToLower();
            var blobId = Guid.NewGuid().ToString().ToLower();
            var fileType = FileType.Image.ToString().ToLower();
            var fileFormat = DocFormat.Png.ToString().ToLower();
            string fileName = "TestStorageConnection".ToLower();

            var blobInfo = new BlobInfo(blobId, fileType, fileFormat, fileName, "txt", heByte);
            try
            {
                Provider.SaveBlob(tenant, string.Empty, true, userId, blobInfo);
                return string.Empty;
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format(
                    "使用存储[存储方式：{0}--{1}]出错; " + Environment.NewLine +
                    "错误消息：{2}; " + Environment.NewLine +
                    "错误详情：{3}",
                    storageType, connectionString, ex.Message, ex.StackTrace);
                LogUtil.LogError(errorMsg);
                return errorMsg;
            }
        }

    }
}
