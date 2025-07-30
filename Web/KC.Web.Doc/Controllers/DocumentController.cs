using KC.Common.FileHelper;
using KC.Enums.Doc;
using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.SecurityHelper;
using KC.Framework.Tenant;
using KC.Model.Component.Queue;
using KC.Service;
using KC.Service.Doc;
using KC.Service.DTO.Doc;
using KC.Service.Util;
using KC.Storage.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KC.Web.Doc.Controllers
{
    
    public class DocumentController : DocBaseController
    {
        protected KC.Service.Component.IStorageQueueService StorageQueueService
        {
            get
            {
                //TODO: Storage with TenantName
                return ServiceProvider.GetService<KC.Service.Component.IStorageQueueService>();
            }
        }
        protected IDocumentInfoService _docService => ServiceProvider.GetService<IDocumentInfoService>();
        public DocumentController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<DocumentController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        /// <summary>
        /// 三级菜单：文件管理/文件管理/文件列表
        /// </summary>
        [Web.Extension.MenuFilter("文件管理", "文件列表", "/Document/Index",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-list-alt", AuthorityId = "5FC67FC2-32B6-46C6-AE76-465444F218F5",
            DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 1, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("文件列表", "文件列表", "/Document/Index",
            "5FC67FC2-32B6-46C6-AE76-465444F218F5", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 1, IsPage = true, ResultType = ResultType.ActionResult)]
        public ActionResult Index()
        {
            ViewBag.CurrentUserName = CurrentUserName;
            return View();
        }

        #region 文件分类
        public JsonResult LoadDocCategoryTree(string name, int excludeId, int selectedId, bool hasAll = false, bool hasRoot = true, int maxLevel = 3)
        {
            var result = new List<DocCategoryDTO>();
            if (hasAll)
            {
                result.Add(new DocCategoryDTO()
                {
                    Id = 0,
                    TypeString = "",
                    Text = "所有文档",
                    Children = null,
                    Level = 1
                });
                result.Add(new DocCategoryDTO()
                {
                    Id = -1,
                    TypeString = "",
                    Text = "未分类文档",
                    Children = null,
                    Level = 1
                });
            }

            var data = _docService.LoadDocCategoryTree(CurrentUserId, name);
            if (data != null && data.Count() > 0)
                result.AddRange(data);

            if (hasRoot)
            {
                var rootMenu = new DocCategoryDTO() { Text = "顶级分类", Children = result, Level = 0 };
                var org = TreeNodeUtil.GetNeedLevelTreeNodeDTO(rootMenu, maxLevel, new List<int>() { excludeId }, new List<int>() { selectedId });
                return Json(new List<DocCategoryDTO> { org });
            }

            var orgList = TreeNodeUtil.LoadNeedLevelTreeNodeDTO(result, maxLevel, new List<int>() { excludeId }, new List<int>() { selectedId });
            return Json(orgList);
        }

        public PartialViewResult GetDocCategory(int id, int? pid)
        {
            DocCategoryDTO model;
            if (id > 0)
            {
                model = _docService.GetDocCategoryById(id);
                model.IsEditMode = true;
            }
            else
            {
                model = new DocCategoryDTO() { ParentId = pid.HasValue && pid.Value > 0 ? pid : 0 };
            }
            var dropItems = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum(null, new List<LableType>()
            {
                LableType.OutCompany,
                LableType.Other
            });
            ViewBag.LableTypeList = dropItems;

            return PartialView("_docCategoryForm", model);
        }

        [Web.Extension.PermissionFilter("文件列表", "保存文件分类", "/Document/SaveDocCategory",
            "D9278B88-342E-4FBE-9AD3-3E2401C1F040", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<JsonResult> SaveDocCategory(DocCategoryDTO model)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                if (model == null)
                    throw new ArgumentException(string.Format("传入对象为空"));

                //对象属性验证
                var errMsg = GetAllModelErrorMessages();
                if (!errMsg.IsNullOrEmpty())
                    throw new ArgumentException(errMsg);

                if (!model.IsEditMode)
                {
                    model.CreatedBy = CurrentUserId;
                    model.CreatedName = CurrentUserDisplayName;
                    model.CreatedDate = DateTime.UtcNow;
                }
                model.ModifiedBy = CurrentUserId;
                model.ModifiedName = CurrentUserDisplayName;
                model.ModifiedDate = DateTime.UtcNow;

                return await _docService.SaveDocCategoryAsync(model); ;
            });
        }

        [Web.Extension.PermissionFilter("文件列表", "删除文件分类", "/Document/RemoveDocCategory",
            "4701B3CA-8BF9-486F-8D57-96355080582D", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult RemoveDocCategory(int id)
        {
            return GetServiceJsonResult(() =>
            {
                var message = _docService.SoftRemoveDocCategoryById(id);
                if (string.IsNullOrEmpty(message))
                    throw new Exception(message);

                return true;
            });
        }

        [AllowAnonymous]
        public async Task<JsonResult> ExistCategoryName(int id, string name, string orginalName, bool isEditMode)
        {
            if (isEditMode && name.Equals(orginalName, StringComparison.OrdinalIgnoreCase))
                return Json(false);

            return Json(await _docService.ExistCategoryNameAsync(id, name));
        }
        #endregion

        #region 文件

        //[Web.Extension.PermissionFilter("文件列表", "加载文件列表", "/Document/FindDocuments",
        //    "9EFEE1AB-BD84-44C2-B892-807789FBB1B4", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
        //    Order = 4, IsPage = false, ResultType = ResultType.ContentResult)]
        public JsonResult FindDocuments(int page, int rows, string name, int? categoryId = 0)
        {
            var orgs = CurrentUser.OrganizationIds.Any()
                ? CurrentUser.OrganizationIds.Select(m => m.ToString()).ToList()
                : new List<string>();
            var roles = CurrentUser.RoleIds;
            var currentId = CurrentUserId;
            // 如果为系统管理员或行政经理，所有公开文档都可见
            if (IsSystemAdmin || CurrentUser.RoleIds.Contains(RoleConstants.ExecutorManagerRoleId))
            {
                orgs = null;
                roles = null;
                currentId = null;
            }

            var result = _docService.LoadPaginatedDocumentsByFilter(page, rows, categoryId, name, currentId, orgs, roles);
            return Json(result);
        }

        public PartialViewResult GetDocumentForm(int id, int selectCategoryId)
        {
            var model = new DocumentInfoDTO();
            if (selectCategoryId > 0)
                ViewBag.selectCategoryId = selectCategoryId;
            if (id > 0)
            {
                model = _docService.GetDocumentById(id);
                model.IsEditMode = true;
                model.DocCategoryId = selectCategoryId > 0 ? selectCategoryId : 0;
                ViewBag.Level = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<DocLevel>((int)model.Level);
            }
            else
            {
                ViewBag.Level = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<DocLevel>();
            }
            ViewBag.AppType = (Tenant.TenantType == TenantType.Enterprise) ? 1 : 0;
            return PartialView("_documentForm", model);
        }

        [Web.Extension.PermissionFilter("文件列表", "保存文件", "/Document/SaveDocument",
            "92A2C1B7-C2CA-4E38-AF86-C242428D5B7F", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 4, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult SaveDocument(DocumentInfoDTO model)
        {
            return GetServiceJsonResult(() =>
            {
                if (model == null)
                    throw new ArgumentException(string.Format("传入对象为空"));

                //对象属性验证
                var errMsg = GetAllModelErrorMessages();
                if (!errMsg.IsNullOrEmpty())
                    throw new ArgumentException(errMsg);

                if (!model.IsEditMode)
                {
                    model.CreatedBy = CurrentUserId;
                    model.CreatedName = CurrentUserDisplayName;
                    model.CreatedDate = DateTime.UtcNow;
                }
                model.ModifiedBy = CurrentUserId;
                model.ModifiedName = CurrentUserDisplayName;
                model.ModifiedDate = DateTime.UtcNow;

                var result = _docService.SaveDocument(model);
                if (result && model.Attachment != null)
                {
                    var blobIds = new List<string>();
                    if (model.Attachment != null)
                        blobIds.Add(model.Attachment.BlobId);
                    if (model.Template != null)
                        blobIds.Add(model.Template.BlobId);
                    if (blobIds.Any())
                        CopyTempsToClientAzureBlob(blobIds);
                }
                return result;
            });
        }

        [Web.Extension.PermissionFilter("文件列表", "删除文件", "/Document/DeleteDocument",
            "E88D025F-CEFE-48EB-BFE7-8D5DC5CFD6D2", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 5, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult DeleteDocument(string sid)
        {
            return GetServiceJsonResult(() => _docService.RemoveDocumentsByIds(sid));
        }


        /// <summary>
        /// 转移文件到某个文件分类下
        /// </summary>
        /// <param name="oldIds">需要转移的文件id</param>
        /// <param name="cateId">文件分类id</param>
        /// <returns></returns>
        [Web.Extension.PermissionFilter("文件列表", "文件转移", "/Document/MoveDocumentsToCategory",
            "0D9C41B3-E679-4FF1-8C9B-D8B8C100448A", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 6, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult MoveDocumentsToCategory(string oldIds, string cateId)
        {
            return GetServiceJsonResult(() => _docService.MoveDocumentsToCategory(oldIds, cateId));
        }

        public PartialViewResult GetSendEmailForm()
        {
            return PartialView("_sendEmailForm");
        }

        [Web.Extension.PermissionFilter("文件列表", "文件外发", "/Document/SendEmail",
            "3684ACDF-2851-4424-987E-80D6C6E8D9FD", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 7, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult SendEmail(List<string> emailList, List<DocumentInfoDTO> infos)
        {
            if ((emailList != null && emailList.Any()) && (infos != null && infos.Any()))
            {
                string nameAndurl = "文件名称：{0}\r\n下载链接：http://" + Request.Host + "/Admin/DataManage/DownloadFile?name={1}&id={2}";
                List<string> contents = new List<string>();
                string title = "Dear \r\n 以下是" + base.TenantName + "的文件，请点击链接下载，请注意查收。";
                contents.Add(title);
                foreach (var item in infos)
                {
                    if (!string.IsNullOrWhiteSpace(item.AttachmentBlob))
                    {
                        var time = DateTime.Now;
                        string md5 = MD5Provider.Hash(base.TenantName + CurrentUserId + time.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        CacheUtil.SetCache<string>(md5, item.AttachmentBlob, time.AddDays(1));
                        contents.Add(string.Format(nameAndurl, item.Name, md5, item.AttachmentBlob));
                    }
                }
                contents.Add("发件人：" + CurrentUserName);
                var body = string.Join("\r\n", contents);
                var email = new EmailInfo()
                {
                    UserId = base.CurrentUserId,
                    Tenant = base.TenantName,
                    EmailTitle = "文件下载",
                    EmailBody = body,
                    SendTo = emailList,
                    CcTo = new List<string>()
                };
                var isSucceed = StorageQueueService.InsertEmailQueue(email);

                if (isSucceed)
                {
                    return Json(new { result = true, Message = "发送成功" });
                }
                else
                {
                    return Json(new { result = false, Message = "发送失败" });
                }

            }
            return Json(new { result = false, Message = "数据错误" });
        }


        [Web.Extension.PermissionFilter("文件列表", "文件下载", "/Document/DownloadDocument",
            "D64C51D6-3E20-4CB5-861F-D7E67B1ED4BB", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 8, IsPage = false, ResultType = ResultType.ContentResult)]
        public ActionResult DownloadDocument(string blobId)
        {
            if (!string.IsNullOrWhiteSpace(blobId))
            {
                var blob = BlobUtil.GetBlobById(Tenant, CurrentUserId, blobId, false);
                if (blob != null && blob.Data != null)
                {
                    var eFormat = DocFormat.Unknown;
                    bool isSuccess = Enum.TryParse(blob.FileFormat, out eFormat);
                    if (isSuccess)
                    {
                        byte[] buffer = blob.Data;
                        string contentType = MimeTypeHelper.GetMineType(eFormat);
                        var stream = new MemoryStream(buffer);
                        string decodeFileName = Base64Provider.DecodeString(blob.FileName);
                        var fileName = decodeFileName.ReplaceLast(".", "-" + blob.Id + ".");
                        return File(stream, contentType, fileName);
                    }
                }
            }
            return Content("<script>alert('预览失败'); window.location.href = '/Admin/DataManage/Index'</script>");
        }

        #endregion

        #region 文件日志
        /// <summary>
        /// 三级菜单：文件管理/文件管理/文件日志
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("文件管理", "文件日志", "/Document/DocumentLog",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-list-alt", AuthorityId = "17F96B5C-5493-4213-9ACC-900CC1286D90",
            DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 3, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("文件日志", "文件日志", "/Document/DocumentLog",
            "17F96B5C-5493-4213-9ACC-900CC1286D90", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 2, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult DocumentLog()
        {
            return View();
        }

        public JsonResult LoadDocumentLogList(int page = 1, int rows = 10, string selectname = null)
        {
            var result = _docService.FindPaginatedDocumentLogs(page, rows, selectname);

            return Json(result);
        }
        #endregion
    }
}