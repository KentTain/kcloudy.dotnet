using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KC.Common.FileHelper;
using KC.Service.DTO.Doc;
using KC.Enums.Doc;
using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.SecurityHelper;
using KC.Framework.Tenant;
using KC.Model.Component.Queue;
using KC.Service;
using KC.Service.Enums;
using KC.Service.Doc;
using KC.Storage.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KC.Web.Doc.Controllers
{
    
    public class DocTemplateController : DocBaseController
    {
        protected KC.Service.Component.IStorageQueueService StorageQueueService
        {
            get
            {
                //TODO: Storage with TenantName
                return ServiceProvider.GetService<KC.Service.Component.IStorageQueueService>();
            }
        }
        protected IDocTemplateService _docTemplateService => ServiceProvider.GetService<IDocTemplateService>();
        public DocTemplateController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<DocTemplateController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        /// <summary>
        /// 三级菜单：文件管理/文件模板管理/文件模板列表
        /// </summary>
        [Web.Extension.MenuFilter("文件模板管理", "文件模板列表", "/DocTemplate/Index",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-list-alt", AuthorityId = "C1C54EC9-EC69-4E39-9175-19AFE998DA7D",
            DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 1, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("文件模板列表", "文件模板列表", "/DocTemplate/Index", "C1C54EC9-EC69-4E39-9175-19AFE998DA7D",
            DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 1, IsPage = true, ResultType = ResultType.ActionResult)]
        public ActionResult Index()
        {
            ViewBag.CurrentUserName = CurrentUserName;
            ViewBag.StatusList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<ApprovalStatus>();
            return View();
        }

        #region 文件模板
        /// <summary>
        /// 获取文件详情列表
        /// </summary>
        /// <param name="cateid"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="name"></param>
        /// <param name="type">0 我的文件 1 客户文件 2 报告 </param>
        /// <returns></returns>
        public JsonResult FindDocTemplates(int page, int rows, WorkflowBusStatus? status, string name)
        {
            var orgCodes = CurrentUserOrgCodes;
            var roleIds = CurrentUserRoleIds;
            var currentId = CurrentUserId;
            var currentName = CurrentUserName;

            //var orgIds = new List<string>();
            //var roleIds = new List<string>() { RoleConstants.ExecutorManagerRoleId };
            //var currentId = RoleConstants.AdminUserId;
            //var currentName = RoleConstants.AdminUserName;

            var result = _docTemplateService.LoadPaginatedDocTemplatesByCondition(page, rows, status, name, orgCodes, roleIds, currentId, currentName);
            return Json(result);
        }

        public PartialViewResult GetDocTemplateForm(int id)
        {
            var model = new DocTemplateDTO();
            if (id != 0)
            {
                model = _docTemplateService.GetDocTemplateById(id);
                model.IsEditMode = true;
                ViewBag.Level = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<DocLevel>((int)model.Level);
            }
            else
            {
                ViewBag.Level = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<DocLevel>();
            }

            return PartialView("_docTemplateForm", model);
        }

        [Web.Extension.PermissionFilter("文件模板列表", "保存文件模板", "/DocTemplate/SaveDocTemplate", "473B720E-2E8B-402D-80FF-271C4D3F52FB",
            DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 4, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "473B720E-2E8B-402D-80FF-271C4D3F52FB")]
        public JsonResult SaveDocTemplate(DocTemplateDTO model)
        {
            if (!model.IsEditMode)
            {
                model.CreatedBy = CurrentUserId;
                model.CreatedName = CurrentUserDisplayName;
                model.CreatedDate = DateTime.UtcNow;
            }
            model.ModifiedBy = CurrentUserId;
            model.ModifiedName = CurrentUserDisplayName;
            model.ModifiedDate = DateTime.UtcNow;

            var result = _docTemplateService.SaveDocTemplate(model);
            if (result)
            {
                var blobIds = new List<string>();
                if (model.Attachment != null)
                    blobIds.Add(model.Attachment.BlobId);
                if (blobIds.Any())
                    CopyTempsToClientAzureBlob(blobIds);
            }
            return Json(new { success = result, message = (result ? "保存成功" : "保存失败") });
        }

        /// <summary>
        /// 根据id删除相应文件
        /// </summary>
        /// <param name="id">文件对应的id</param>
        /// <returns></returns>
        [Web.Extension.PermissionFilter("文件模板列表", "删除文件模板", "/DocTemplate/DeleteDocTemplate", "6EBC4456-AECB-41AE-B17A-B9B666D13CF2",
            DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 5, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "6EBC4456-AECB-41AE-B17A-B9B666D13CF2")]
        public JsonResult DeleteDocTemplate(string sid)
        {
            return GetServiceJsonResult(() => _docTemplateService.RemoveDocTemplatesByIds(sid));
        }

        /// <summary>
        /// 邮件弹窗
        /// </summary>
        /// <returns></returns>
        public PartialViewResult GetSendEmailForm()
        {
            return PartialView("_sendEmailForm");
        }

        /// <summary>
        /// 发邮件
        /// </summary>
        /// <param name="emailList"></param>
        /// <param name="infos"></param>
        /// <returns></returns>
        [Web.Extension.PermissionFilter("文件模板列表", "文件模板外发", "/DocTemplate/SendEmail", "DB4A5E79-3DFD-42F3-A73E-14108FCE040C",
            DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 7, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "DB4A5E79-3DFD-42F3-A73E-14108FCE040C")]
        public JsonResult SendEmail(List<string> emailList, List<DocTemplateDTO> infos)
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

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="blobId"></param>
        /// <returns></returns>
        [Web.Extension.PermissionFilter("文件模板列表", "文件下载", "/DocTemplate/SendEmail", "23A6964A-9B2D-4322-BC6A-EC8076E3CC14",
            DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 8, IsPage = false, ResultType = ResultType.ContentResult, AuthorityId = "23A6964A-9B2D-4322-BC6A-EC8076E3CC14")]
        public ActionResult DownloadDocTemplate(string blobId)
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

        #region 文件模板日志
        /// <summary>
        /// 三级菜单：文件管理/文件模板管理/文件模板日志
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("文件模板管理", "文件模板日志", "/DocTemplate/DocTemplateLog",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-list-alt", AuthorityId = "A3667970-B11D-435C-A2CA-AD4B53A23FEA",
            DefaultRoleId = RoleConstants.ExecutorManagerRoleId, Order = 2, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("文件模板日志", "文件模板日志", "/DocTemplate/DocTemplateLog",
            "A3667970-B11D-435C-A2CA-AD4B53A23FEA", DefaultRoleId = RoleConstants.ExecutorManagerRoleId,
            Order = 2, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult DocTemplateLog()
        {
            return View();
        }

        public JsonResult LoadDocTemplateLogList(int page = 1, int rows = 10, string selectname = null)
        {
            var result = _docTemplateService.FindPaginatedDocTemplateLogs(page, rows, selectname);

            return Json(result);
        }
        #endregion
    }
}
