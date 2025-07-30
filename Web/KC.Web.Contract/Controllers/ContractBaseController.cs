using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KC.Common.FileHelper;
using KC.Framework.Extension;
using KC.Framework.SecurityHelper;
using KC.Framework.Tenant;
using KC.Service;
using KC.Service.Contract.WebApiService;
using KC.Service.DTO;
using KC.Service.WebApiService.Business;
using KC.Storage.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KC.Web.Contract.Controllers
{
    public abstract class ContractBaseController : Web.Controllers.TenantWebBaseController
    {
        protected ITenantUserApiService _tenantStore => ServiceProvider.GetService<ITenantUserApiService>();
        protected IAccountApiService AccountApiService => ServiceProvider.GetService<IAccountApiService>();

        public ContractBaseController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        #region 选小图标控件: Shared/_SelectUserPartial.cshtml
        [Microsoft.AspNetCore.Mvc.HttpGet]
        public Microsoft.AspNetCore.Mvc.PartialViewResult _SelectIconPartial()
        {
            return PartialView();
        }

        #endregion 选小图标控件: Shared/_SelectUserPartial.cshtml

        #region 选人控件: Shared/_SelectUserPartial.cshtml
        /// <summary>
        /// 获取所有的部门信息及下属员工
        /// </summary>
        /// <returns></returns>
        [Microsoft.AspNetCore.Mvc.HttpGet]
        public Microsoft.AspNetCore.Mvc.JsonResult GetRootOrganizationsWithUsers()
        {
            var res = AccountApiService.LoadOrgTreesWithUsers();
            return Json(res);
        }

        /// <summary>
        /// 获取相关部门以及角色信息及下属员工
        /// </summary>
        /// <param name="searchModel">筛选条件：部门Ids列表、角色Ids列表</param>
        /// <returns></returns>
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public async Task<Microsoft.AspNetCore.Mvc.JsonResult> GetOrgsWithUsersByRoleIdsAndOrgids(Service.DTO.Search.OrgTreesAndRolesWithUsersSearchDTO searchModel)
        {
            var orgs = await AccountApiService.LoadOrgTreesWithUsersByOrgIds(searchModel);
            var roles = await AccountApiService.LoadRolesWithUsersByRoleIds(searchModel);
            return this.Json(new { orgInfos = orgs, roleInfos = roles });
        }

        #endregion 选人控件: Shared/_SelectUserPartial.cshtml

        //[AuthenticateAndAuthorizeTenant]
        [HttpGet]
        public IActionResult DownloadContract(string blobId, string userId, string id = "")
        {
            if (string.IsNullOrEmpty(userId))
                userId = TenantName;
            var tenant = _tenantStore.GetTenantByName(userId).Result;
            var blob = BlobUtil.GetBlobById(tenant, CurrentUserId, blobId);
            if (blob != null && blob.Data != null && blob.Data.Length > 0)
            {
                DocFormat docFormat = DocFormat.Unknown;
                bool isSuccess = Enum.TryParse(blob.FileFormat, out docFormat);
                if (isSuccess)
                {

                    byte[] buffer = blob.Data;
                    string contentType = MimeTypeHelper.GetMineType(docFormat);

                    var stream = new MemoryStream(buffer);
                    string decodeFileName = Base64Provider.DecodeString(blob.FileName);
                    if (!string.IsNullOrEmpty(id))
                    {
                        //var dlog = new ContractGroupOperationLogDTO()
                        //{
                        //    ContractGroupId = new Guid(id),
                        //    ContractGroupProgress = ContractGroupProgress.DownContract,
                        //    OperatorId = CurrentUserName,
                        //    Operator = CurrentUserDisplayName,
                        //    OperateDate = DateTime.UtcNow.ToLocalDateTime(),
                        //    Type = ProcessLogType.Success,
                        //    NotContractGroupUsers = "",
                        //    Remark = "下载合同"
                        //};

                        //给pdf添加水印  https://www.starlu.com/  打印时间

                        //WatermarkHelper.AddWatermarkToPdf(stream, ms,
                        //    "https://www.starlu.com/ " + System.DateTime.Now.ToLongTimeString());

                        //var pdf = WatermarkHelper.AddWatermarkToPdf(stream, "https://www.starlu.com/ " + System.DateTime.UtcNow.AddHours(8).ToLocalTime());

                        //MemoryStream ms = new MemoryStream(pdf);

                        //return File(ms, contentType, decodeFileName.ReplaceLast(".", "-" + blob.Id + "."));
                    }
                    return File(stream, contentType, decodeFileName.ReplaceLast(".", "-" + blob.Id + "."));
                }
                else
                {
                    return Json(new { ResultType = ServiceResultType.Error, Message = "不支持所保存的文件格式。" });
                }
            }
            return Json(new { ResultType = ServiceResultType.Error, Message = "未找到文件。" });
        }
        //[AuthenticateAndAuthorizeTenant]
        [HttpGet]
        public void PrintPDF(string blobId, string userId)
        {
            if (string.IsNullOrEmpty(userId))
                userId = TenantName;
            var tenant = _tenantStore.GetTenantByName(userId).Result;
            var blob = BlobUtil.GetBlobById(tenant, CurrentUserId, blobId);
            if (blob != null && blob.Data != null && blob.Data.Length > 0)
            {
                //MemoryStream PDFData = new MemoryStream(blob.Data);
                //var pdf = WatermarkHelper.AddWatermarkToPdf(PDFData, "https://www.starlu.com/ " + System.DateTime.Now.ToLocalTime());
                //System.Web.HttpContext.Current.Response.Clear();
                //System.Web.HttpContext.Current.Response.ClearContent();
                //System.Web.HttpContext.Current.Response.ClearHeaders();
                //System.Web.HttpContext.Current.Response.ContentType = "application/pdf";
                //System.Web.HttpContext.Current.Response.Charset = string.Empty;
                //System.Web.HttpContext.Current.Response.Cache.SetCacheability(System.Web.HttpCacheability.Public);
                //System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "inline; filename=" + blob.FileName.UnicodeToChinese().Replace(" ", "").Replace(":", "-") + ".pdf");
                //System.Web.HttpContext.Current.Response.OutputStream.Write(pdf.ToArray(), 0, pdf.ToArray().Length);
                //System.Web.HttpContext.Current.Response.OutputStream.Flush();
                //System.Web.HttpContext.Current.Response.OutputStream.Close();
            }

        }
    }
}