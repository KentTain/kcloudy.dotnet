using KC.Common;
using KC.Common.FileHelper;
using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.SecurityHelper;
using KC.Framework.Tenant;
using KC.Service.Doc;
using KC.Service.DTO;
using KC.Storage.Util;
using KC.Web.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;

namespace KC.WebApi.Doc.Controllers
{
    /// <summary>
    /// 文件资源
    /// </summary>
    [Route("api/[controller]")]
    [EnableCors(Web.Util.StaticFactoryUtil.MyAllowSpecificOrigins)]
    public class ResourcesController : Web.Controllers.WebApiBaseController
    {
        protected IDocumentInfoService _docService => ServiceProvider.GetService<IDocumentInfoService>();

        public ResourcesController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<ResourcesController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="id">图片的BlobId</param>
        /// <param name="isTempFile">是否临时文件：默认为false</param>
        /// <param name="uId">用户的UserId：可为空</param>
        /// <returns>图片流</returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("ShowImage")]
        public IActionResult ShowImage(string id, bool isTempFile = false, string uId = null)
        {
            //var authToken = GetAuthToken(false);
            //var blob = BlobProvider.GetBlob(authToken, blobId);
            var blob = BlobUtil.GetBlobById(Tenant, uId, id, isTempFile);
            if (blob == null)
                return NotFound();

            var eFormat = ImageFormat.Unknown;
            bool isSuccess = Enum.TryParse(blob.FileFormat, out eFormat);
            if (!isSuccess)
                return BadRequest();

            byte[] buffer = blob.Data;
            return new ImageResult(buffer, eFormat);
        }

        /// <summary>
        /// 获取文件
        /// </summary>
        /// <param name="id">文件的BlobId</param>
        /// <param name="isTempFile">是否临时文件：默认为false</param>
        /// <param name="uId">用户的UserId：可为空</param>
        /// <returns>文件流</returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("DownloadFile")]
        public IActionResult DownloadFile(string id, bool isTempFile = false, string uId = null)
        {
            //var authToken = GetAuthToken(false);
            //var blob = BlobProvider.GetBlob(authToken, blobId);
            var blob = BlobUtil.GetBlobById(Tenant, uId, id, isTempFile);
            if (blob == null)
                return NotFound();

            var docFormat = DocFormat.Unknown;
            bool isSuccess = Enum.TryParse(blob.FileFormat, out docFormat);
            if (!isSuccess)
                return BadRequest();

            byte[] buffer = blob.Data;
            string contentType = MimeTypeHelper.GetMineType(docFormat);
            string decodeFileName = Base64Provider.DecodeString(blob.FileName);
            var fileName = decodeFileName.ReplaceLast(".", "-" + blob.Id + ".");

            return File(buffer, contentType, fileName);
        }

        [HttpPost]
        [RequestSizeLimit(100_000_000)] //最大100m左右
        //[DisableRequestSizeLimit]  //或者取消大小的限制
        [Route("Upload")]
        public IActionResult Upload()
        {
            string parm = Request.Query["parm"];
            IFormFile file = Request.Form.Files.Count != 0 ? Request.Form.Files[0] : null;
            if ("config".Equals(parm))
            {
                return Upload(parm);
            }
            else if ("listimage".Equals(parm))
            {
                return ListFile("", true);
            }
            else if ("listfile".Equals(parm))
            {
                return ListFile("", false);
            }
            // UEditor 上传图片
            else if ("uploadimage".Equals(parm))
            {
                return UploadFile(file, false, true);
            }
            return UploadFile(file, false);
        }

        private IActionResult Upload(string parm)
        {
            if ("config".Equals(parm))
            {
                var domainName = GetDocumentApiUrl(Tenant.TenantName);
                var configContent = GetUEditorConfig();
                configContent = configContent.Replace("{ApiDomain}", domainName).Replace("{imageMaxSize}", (GlobalConfig.UploadConfig.ImageMaxSize * 1024 * 1024).ToString()).Replace("{fileMaxSize}", (GlobalConfig.UploadConfig.FileMaxSize * 1024 * 1024).ToString());
                return Content(configContent.Replace("\r\n", ""));
                //return Content(SerializeHelper.ToJson(JObject.Parse(configContent)));
            }

            return Json(new
            {
                success = false,
                message = "请选择上传文件",
                //ueditor所需参数
                state = "FALSE",
                error = "请选择上传文件",
                url = string.Empty,
                id = string.Empty,
                title = string.Empty,
                original = string.Empty
            });
        }
        /// <summary>
        /// 获取文档接口地址：http://[tenantName].docapi.kcloudy.com/api/ </br>
        ///     本地测试接口地址：http://[tenantName].localhost:2006/api/
        /// </summary>
        /// <param name="tenantName">租户代码</param>
        /// <returns></returns>
        private string GetDocumentApiUrl(string tenantName)
        {
            if (string.IsNullOrEmpty(GlobalConfig.DocWebDomain))
                return null;

            return GlobalConfig.GetTenantWebApiDomain(GlobalConfig.DocWebDomain, tenantName);
        }
        private string GetUEditorConfig()
        {
            var url = GlobalConfig.ResWebDomain + "js/ueditor/config.json";

            try
            {
                string result = Common.HttpHelper.HttpWebRequestHelper.DoGet(url, null);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError("从【" + url + "】获取UEditor的配置文件出错，错误消息为：" + ex.Message);
                var configPath = ServerPath + "/wwwroot/js/ueditor/config.json";
                try
                {
                    return System.IO.File.ReadAllText(configPath);
                }
                catch (Exception ex1)
                {
                    Logger.LogError("从【" + configPath + "】获取UEditor的配置文件出错，错误消息为：" + ex1.Message);
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 上传文件至临时文件夹，1天后清除
        /// </summary>
        /// <returns>文件流</returns>
        [HttpPost]
        [RequestSizeLimit(100_000_000)] //最大100m左右
        //[DisableRequestSizeLimit]  //或者取消大小的限制
        [AllowAnonymous]
        [Route("UploadFileToTemp")]
        public IActionResult UploadFileToTemp()
        {
            string parm = Request.Query["parm"];
            IFormFile file = Request.Form.Files.Count != 0 ? Request.Form.Files[0] : null;
            if ("config".Equals(parm))
            {
                return Upload(parm);
            }
            else if ("listimage".Equals(parm))
            {
                return ListFile("", true);
            }
            else if ("listfile".Equals(parm))
            {
                return ListFile("", false);
            }
            // UEditor 上传图片
            else if ("uploadimage".Equals(parm))
            {
                return UploadFile(file, true, true);
            }

            return UploadFile(file, true);
        }

        private JsonResult UploadFile(IFormFile file, bool isTempFile = false, bool isUEditor = false)
        {
            var result = new UploadResult() {
                Success = true,
                Message = string.Empty,
                //ueditor所需参数
                State = "SUCCESS",
                Error = string.Empty,
                url = string.Empty,
                Id = string.Empty,
                title = string.Empty,
                original = string.Empty
            };
            if (file == null)
            {
                result.Success = false;
                result.Message = "请选择上传文件";
                //ueditor所需参数
                result.State = "FALSE";
                result.Error = "请选择上传文件";
                return Json(result);
            }

            #region 传入参数
            string appId = null;
            string appName = null;
            string fileName = file.FileName;
            string uploadType = isUEditor ? "uploadimage" : "uploadfile";
            string md5 = null;
            string tenantName = null;
            string blobId = null;
            string uploadFileName = null;
            string extensionStr = null;
            string userId = null;
            bool isWatermake = false;
            bool isTopdf = false;
            bool isArchive = true;
            long fileSize = file.Length;

            foreach (var form in Request.Form)
            {
                if (form.Key.Equals("appId", StringComparison.OrdinalIgnoreCase))
                {
                    appId = form.Value;
                }
                else if (form.Key.Equals("appName", StringComparison.OrdinalIgnoreCase))
                {
                    appName = form.Value;
                }
                else if (form.Key.Equals("uploadType", StringComparison.OrdinalIgnoreCase))
                {
                    uploadType = form.Value;
                }
                else if (form.Key.Equals("md5", StringComparison.OrdinalIgnoreCase))
                {
                    md5 = form.Value;
                }
                else if (form.Key.Equals("tenantName", StringComparison.OrdinalIgnoreCase))
                {
                    tenantName = form.Value;
                }
                else if (form.Key.Equals("userId", StringComparison.OrdinalIgnoreCase))
                {
                    userId = form.Value;
                    if (string.IsNullOrWhiteSpace(userId))
                        userId = "";
                }
                else if (form.Key.Equals("blobId", StringComparison.OrdinalIgnoreCase))
                {
                    blobId = form.Value;
                }
                else if (form.Key.Equals("name", StringComparison.OrdinalIgnoreCase))
                {
                    uploadFileName = form.Value;
                }
                else if (form.Key.Equals("ext", StringComparison.OrdinalIgnoreCase))
                {
                    extensionStr = form.Value;
                }
                else if (form.Key.Equals("isWatermake", StringComparison.OrdinalIgnoreCase))
                {
                    bool.TryParse(form.Value, out isWatermake);
                }
                else if (form.Key.Equals("isTopdf", StringComparison.OrdinalIgnoreCase))
                {
                    bool.TryParse(form.Value, out isTopdf);
                }
                else if (form.Key.Equals("isArchive", StringComparison.OrdinalIgnoreCase))
                {
                    bool.TryParse(form.Value, out isArchive);
                }
                else if (form.Key.Equals("size", StringComparison.OrdinalIgnoreCase))
                {
                    long.TryParse(form.Value, out fileSize);
                }
            }

            #endregion

            #region 检查：文件限制
            if (string.IsNullOrWhiteSpace(tenantName))
                tenantName = Tenant.TenantName;
            if (string.IsNullOrWhiteSpace(blobId))
                blobId = Guid.NewGuid().ToString();
            if (string.IsNullOrWhiteSpace(extensionStr))
                extensionStr = fileName.Remove(0, fileName.LastIndexOf('.') + 1).ToLower();
            if (string.IsNullOrEmpty(uploadFileName))
                uploadFileName = fileName;

            var isImage = "uploadimage".Equals(uploadType, StringComparison.OrdinalIgnoreCase);
            var fileType = MimeTypeHelper.GetFileType(extensionStr).ToString();
            var fileFormat = MimeTypeHelper.GetFileFormatByExt(extensionStr);

            var allowExtension = isImage
                ? GlobalConfig.UploadConfig.ImageExt.Split(',')
                : GlobalConfig.UploadConfig.FileExt.Split(',');

            //检查扩展名
            if (!allowExtension.Contains(extensionStr))
            {
                var msg = isImage
                    ? "不允许上传此类型的文件。 \n只允许" + GlobalConfig.UploadConfig.ImageExt + " 格式。"
                    : "不允许上传此类型的文件。 \n只允许" + GlobalConfig.UploadConfig.FileExt + " 格式。";
                
                result.Success = false;
                result.Message = msg;
                //ueditor所需参数
                result.State = "FALSE";
                result.Error = msg;
                return Json(result);
            }

            //文件大小
            var maxSize = isImage
                    ? GlobalConfig.UploadConfig.ImageMaxSize * 1024 * 1024
                    : GlobalConfig.UploadConfig.FileMaxSize * 1024 * 1024;
            if (file.Length > maxSize)
            {
                var msg = "文件大小超过限制：" + maxSize / 1024 / 1024 + "M";
                result.Success = false;
                result.Message = msg;
                //ueditor所需参数
                result.State = "FALSE";
                result.Error = msg;
                return Json(result);
            }

            #endregion 检查：文件限制

            #region 上传附件

            try
            {
                result.Success = true;
                result.Message = string.Empty;
                result.Id = blobId;
                // ueditor所需参数
                result.State = "SUCCESS";
                result.Error = string.Empty;
                result.title = fileName;
                result.original = fileName;
                result.size = fileSize;
                result.type = extensionStr;
                result.url = "?id=" + blobId + "&uId=" + userId;
                result.Result = new BlobInfoDTO()
                {
                    TenantName = Tenant.TenantName,
                    BlobId = blobId,
                    Ext = extensionStr,
                    FileType = fileType,
                    FileFormat = fileFormat,
                    BlobName = fileName,
                    Size = fileSize
                };
                // 如果进行了分片
                if (Request.Form.Keys.Any(m => m == "chunk"))
                {
                    //获得上传的分片数据流
                    var chunkFile = file;
                    if (null == chunkFile)
                    {
                        return Json(new { success = false, message = "上传的文件不能为空" });
                    }

                    string path = Path.Combine(TempDir, md5);
                    if (CreateFileFolder(path))
                    {
                        //取得chunk和chunks
                        int chunk = Convert.ToInt32(Request.Form["chunk"]);//当前分片在上传分片中的顺序（从0开始）
                        //int chunks = Convert.ToInt32(Request.Form["chunks"]);//总分片数

                        var checkfileName = Path.Combine(path, chunk.ToString());
                        using (var addFile = new FileStream(checkfileName, FileMode.Create, FileAccess.Write))
                        {
                            using (var addWriter = new BinaryWriter(addFile))
                            {
                                var buffer = new byte[file.Length];
                                using (var reader = file.OpenReadStream())
                                {
                                    //addWriter.Write(reader.ReadBytes((int)file.Length));
                                    using (var tempReader = new BinaryReader(reader))
                                    {
                                        addWriter.Write(tempReader.ReadBytes((int)file.Length));
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    var success = false;
                    var blobUserId = string.IsNullOrWhiteSpace(userId) ? "" : userId;
                    var encodeFileName = Base64Provider.EncodeString(uploadFileName);
                    // 判断文档是否要生成水印
                    if (isImage && isWatermake)
                    {
                        var addWatermarkResult = AddWatermarkToImage(file, blobId, extensionStr, "鑫亚科技：" + TenantConstant.DbaTenantName);
                        isWatermake = addWatermarkResult.Item1;
                        if (addWatermarkResult.Item1)
                        {
                            using (var stream = new FileStream(addWatermarkResult.Item2, FileMode.Open, FileAccess.Read))
                            {
                                var fsLen = (int)stream.Length;
                                var heByte = new byte[fsLen];
                                stream.Read(heByte, 0, fsLen);

                                result.Result.Size = fsLen;
                                var blobThumbnailInfo = new BlobInfo(blobId, fileType, fileFormat, encodeFileName, extensionStr, heByte);
                                success = BlobUtil.SaveBlob(Tenant, blobUserId, blobThumbnailInfo, isTempFile);
                                CleanTempFile(addWatermarkResult.Item2);
                            }
                        }
                    }
                    // 为处理或是生成水印失败后，上传文档
                    if (!success)
                    {
                        var fileBuffer = new byte[file.Length];
                        using (var reader = file.OpenReadStream())
                        {
                            reader.Read(fileBuffer, 0, fileBuffer.Length);
                        }
                        var blobInfo = new BlobInfo(blobId, fileType, fileFormat, encodeFileName, extensionStr, fileBuffer);
                        success = BlobUtil.SaveBlob(Tenant, blobUserId, blobInfo, isTempFile);
                        result.Success = success;
                        if (!success)
                        {
                            result.Success = false;
                            result.Message = "上传文件失败，请重试！";
                            //ueditor所需参数
                            result.State = "FALSE";
                            result.Error = "上传文件失败，请重试！";
                        }
                    }
                    // 上传成功后，对文档进行归档处理
                    if (success && isArchive)
                    {
                        var doc = new Service.DTO.Doc.DocumentInfoDTO()
                        {
                            IsEditMode = false,
                            IsArchive = true,
                            Name = uploadFileName,
                            Type = Enums.Doc.LableType.Private,
                            Level = Enums.Doc.DocLevel.LevelOne,
                            UploadedTime = DateTime.UtcNow,
                            Ext = extensionStr,
                            FileType = fileType,
                            FileFormat = fileFormat,
                            Size = fileSize,
                            HasTemplates = false,
                            IsValid = true,
                            IsPublic = true,
                            CanEdit = false,
                            CanSend = false,
                            TenantName = Tenant.TenantName,
                            AttachmentBlob = SerializeHelper.ToJson(result.Result),
                            IsDeleted = false,
                            CreatedBy = RoleConstants.AdminUserId,
                            CreatedName = RoleConstants.AdminUserName,
                            CreatedDate = DateTime.UtcNow,
                            ModifiedBy = RoleConstants.AdminUserId,
                            ModifiedName = RoleConstants.AdminUserName,
                            ModifiedDate = DateTime.UtcNow,
                        };
                        _docService.SaveDocument(doc);
                    }
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                
                result.Success = false;
                result.Message = ex.Message;
                result.Id = string.Empty;
                // ueditor所需参数
                result.State = "FALSE";
                result.Error = ex.Message;
                result.title = fileName;
                result.original = fileName;
                result.size = fileSize;
                result.type = extensionStr;
                return Json(result);
            }

            #endregion 上传附件
        }
        private JsonResult ListFile(string userId, bool isImage = false)
        {
            var start = string.IsNullOrEmpty(Request.Query["start"]) ? 0 : Convert.ToInt32(Request.Query["start"]);

            var fileList = new List<string>();
            fileList = isImage
                ? BlobUtil.GetTenantBlobIdsWithType(Tenant, userId, FileType.Image).ToList()
                : BlobUtil.GetTenantBlobIdsWithType(Tenant, userId, FileType.Excel).ToList();

            return Json(new
            {
                success = true,
                message = string.Empty,
                state = "SUCCESS",
                list = fileList.Select(x => new { url = string.Format("?id={0}&uId={1}", x, userId) }),
                ids = fileList.Select(x => new { blobId = x }),
                start = start,
                size = 20,
                total = fileList.Count
            });
        }

        /// <summary>
        /// 分片验证,验证对应分片文件是否存在，大小是否吻合
        /// </summary>
        /// <param name="name">文件md5</param>
        /// <param name="chunkIndex">分片序列</param>
        /// <param name="size">分片文件的大小</param>
        /// <returns></returns>
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("ChunkCheck")]
        public JsonResult ChunkCheck(string name, int chunkIndex, long size)
        {
            var chunkFile = Path.Combine(TempDir, name, chunkIndex.ToString());
            //分片已存在
            if (System.IO.File.Exists(chunkFile) && size == System.IO.File.ReadAllBytes(chunkFile).LongLength)
            {
                return Json(new { ifExist = true });
            }
            //分片不存在
            return Json(new { ifExist = false });
        }

        /// <summary>
        /// 分片合并操作 </br>
        ///     并发锁: 避免多线程同时触发合并操作 </br>
        ///     清理: 合并清理不再需要的分片文件、文件夹、tmp文件
        /// </summary>
        /// <param name="folder">分片文件所在的文件夹名称</param>
        /// <param name="name">合并后的文件名</param>
        /// <param name="chunks">分片总数</param>
        /// <param name="type">文件类型</param>
        /// <param name="blobId">文件名</param>
        /// <param name="ext">后缀名</param>
        /// <param name="userId">用户id</param>
        /// <param name="isArchive">上传后，是否归档</param>
        /// <returns></returns>
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("ChunksMerge")]
        public JsonResult ChunksMerge(string folder, string name, int chunks, string type, string blobId, string ext, string userId, bool isArchive = true)
        {
            // 分片存储目录
            var chunkFileDir = Path.Combine(TempDir, folder);
            // 分片列表
            var chunkFiles = GetChunks(chunkFileDir);

            // 检查是否满足合并条件：分片数量是否足够
            if (chunks == chunkFiles.Count)
            {
                var success = false;
                //TODO 同步指定合并的对象
                blobId = string.IsNullOrWhiteSpace(blobId) ? Guid.NewGuid().ToString() : blobId;
                ext = string.IsNullOrWhiteSpace(ext)
                    ? name.Remove(0, name.LastIndexOf('.') + 1).ToLower()
                    : ext;
                var blobUserId = string.IsNullOrWhiteSpace(userId) ? "" : userId;
                //var fileType = type.ToLower()== "uploadimage" ? FileType.Image.ToString() : MimeTypeHelper.GetFileType(ext).ToString();
                //var fileFormat = MimeTypeHelper.GetImageFormatByExt(ext).ToString();
                var fileType = MimeTypeHelper.GetFileType(ext).ToString();
                var fileFormat = MimeTypeHelper.GetFileFormatByExt(ext);
                var size = 0L;

                // 按照名称排序文件，这里分片都是按照数字命名的
                chunkFiles = chunkFiles.OrderBy(m => int.Parse(m.Name)).ToList();
                var newFilePath = Path.Combine(TempDir, name);
                try
                {
                    foreach (var file in chunkFiles)
                    {
                        using (var chunkFile = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                        {
                            var fsLen = (int)chunkFile.Length;
                            var heByte = new byte[fsLen];
                            chunkFile.Read(heByte, 0, fsLen);
                            using (var addFile = new FileStream(newFilePath, FileMode.Append, FileAccess.Write))
                            {
                                using (var addWriter = new BinaryWriter(addFile))
                                {
                                    addWriter.Write(heByte);
                                }
                            }
                        }
                        file.Delete();
                    }

                    //Copy
                    using (var stream = new FileStream(newFilePath, FileMode.Open, FileAccess.Read))
                    {
                        size = stream.Length;
                        var fsLen = (int)stream.Length;
                        var heByte = new byte[fsLen];
                        stream.Read(heByte, 0, fsLen);

                        var encodeFileName = Base64Provider.EncodeString(name);
                        var blobInfo = new BlobInfo(blobId, fileType, fileFormat, encodeFileName, ext, heByte);
                        success = BlobUtil.SaveBlob(Tenant, blobUserId, blobInfo, true);
                    }

                    // 上传成功后，对文档进行归档处理
                    if (success && isArchive)
                    {
                        System.IO.File.Delete(newFilePath);
                        // 对文档进行归档处理
                        var blobInfoDto = new BlobInfoDTO()
                        {
                            TenantName = Tenant.TenantName,
                            BlobId = blobId,
                            Ext = ext,
                            FileType = fileType,
                            FileFormat = fileFormat,
                            BlobName = name,
                            Size = size
                        };
                        var doc = new Service.DTO.Doc.DocumentInfoDTO()
                        {
                            IsEditMode = false,
                            IsArchive = true,
                            Name = name,
                            Type = Enums.Doc.LableType.Private,
                            Level = Enums.Doc.DocLevel.LevelOne,
                            UploadedTime = DateTime.UtcNow,
                            Ext = ext,
                            FileType = fileType,
                            FileFormat = fileFormat,
                            Size = size,
                            HasTemplates = false,
                            IsValid = true,
                            IsPublic = true,
                            CanEdit = false,
                            CanSend = false,
                            TenantName = Tenant.TenantName,
                            AttachmentBlob = SerializeHelper.ToJson(blobInfoDto),
                            IsDeleted = false,
                            CreatedBy = RoleConstants.AdminUserId,
                            CreatedName = RoleConstants.AdminUserName,
                            CreatedDate = DateTime.UtcNow,
                            ModifiedBy = RoleConstants.AdminUserId,
                            ModifiedName = RoleConstants.AdminUserName,
                            ModifiedDate = DateTime.UtcNow,
                        };
                        _docService.SaveDocument(doc);
                    }
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        success = false,
                        message = ex.Message
                    });
                }
                //清理：文件夹，tmp文件
                CleanSpace(chunkFileDir);
                return Json(new
                {
                    success = success,
                    message = success ? string.Empty : "上传文件失败，请重试！",
                    id = blobId,
                    result = new BlobInfoDTO()
                    {
                        TenantName = Tenant.TenantName,
                        BlobId = blobId,
                        FileType = fileType,
                        FileFormat = fileFormat,
                        BlobName = name,
                        Ext = ext,
                        Size = size,
                    }
                });
            }
            return Json(new
            {
                success = false,
                message = "文件分片数量不足，请等待！"
            });
        }

        /// <summary>
        /// 获取指定文件的所有分片
        /// </summary>
        /// <param name="folder">文件夹路径</param>
        /// <returns></returns>
        private List<FileInfo> GetChunks(string folder)
        {
            if (Directory.Exists(folder))
                return new DirectoryInfo(folder).GetFiles().ToList();
            return new List<FileInfo>();
        }

        /// <summary>
        /// 创建存放上传的文件的文件夹
        /// </summary>
        /// <param name="name">文件夹路径</param>
        /// <returns></returns>
        private bool CreateFileFolder(string name)
        {
            //创建存放分片文件的临时文件夹
            if (!Directory.Exists(name))
            {
                try
                {
                    Directory.CreateDirectory(name);
                }
                catch (SecurityException)
                {
                    return false;
                }
            }

            var tempFile = name + ".tmp";

            if (!System.IO.File.Exists(tempFile))
            {
                try
                {
                    //创建一个对应的文件，用来记录上传分片文件的修改时间，用于清理长期未完成的垃圾分片
                    using (System.IO.File.Create(tempFile))
                    {
                    }
                }
                catch (SecurityException ex)
                {
                    Logger.LogError(ex, ex.Message);
                    return false;
                }
            }
            else
            {
                //TODO 多线程访问
                System.IO.File.SetLastWriteTimeUtc(tempFile, DateTime.UtcNow);
            }

            return true;
        }
        private void CleanSpace(string folder)
        {
            try
            {
                //删除分片文件夹
                Directory.Delete(folder);
                //删除tmp文件
                System.IO.File.Delete(folder + ".tmp");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
            }
        }
        private void CleanTempFile(string filePath)
        {
            try
            {
                //删除tmp文件
                System.IO.File.Delete(filePath);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
            }
        }

        private Tuple<bool, string> AddWatermarkToImage(IFormFile file, string blobId, string extensionStr, string text)
        {
            string tempFilePath = Path.Combine(TempDir, blobId + "_tmp." + extensionStr);
            string newFilePath = Path.Combine(TempDir, blobId + "." + extensionStr);
            try
            {
                using (var addFile = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
                {
                    using (var addWriter = new BinaryWriter(addFile))
                    {
                        using (var stream = file.OpenReadStream())
                        {
                            using (var tempReader = new BinaryReader(stream))
                            {
                                addWriter.Write(tempReader.ReadBytes((int)stream.Length));
                            }
                        }
                    }
                }

                ThirdParty.WatermarkHelper.AddWatermarkToImage(tempFilePath, newFilePath, null, text);
                CleanTempFile(tempFilePath);
                return new Tuple<bool, string>(true, newFilePath);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
            }
            return new Tuple<bool, string>(false, newFilePath);
        }
    }

    [Serializable, DataContract(IsReference = true)]
    public class UploadResult
    {
        [DataMember]
        public bool Success { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string Id { get; set; }

        //ueditor所需参数
        [DataMember]
        public string State { get; set; }
        [DataMember]
        public string Error { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string original { get; set; }
        [DataMember]
        public long size { get; set; }
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public string url { get; set; }
        [DataMember]
        public BlobInfoDTO Result { get; set; }
    }
}