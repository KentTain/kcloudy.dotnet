using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using KC.Service.Contract;
using KC.Service.DTO.Contract;
using KC.Framework.Base;
using KC.Storage.Util;
using Microsoft.AspNetCore.Http;
using KC.Framework.Extension;
using KC.Framework.Util;
using KC.Service.Constants;
using KC.Model.Component.Queue;
using KC.Common;
using KC.Service.Base;

namespace KC.Web.Contract.Controllers
{
    [Web.Extension.MenuFilter("合同管理", "印章管理", "/ElectronicSign/Index",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-file-text", AuthorityId = "E96A2CFF-060C-47F2-8A48-36EB7E443FE5",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsExtPage = false, Level = 2)]
    public class ElectronicSignController : Web.Controllers.TenantWebBaseController
    {
        private IElectronicSignService _eSignService => ServiceProvider.GetService<IElectronicSignService>();
        private IAuthorizationService AuthorizationService => ServiceProvider.GetService<IAuthorizationService>();

        protected KC.Service.Component.IStorageQueueService StorageQueueService
        {
            get
            {
                //TODO: Storage with TenantName
                return ServiceProvider.GetService<KC.Service.Component.IStorageQueueService>();
            }
        }


        public ElectronicSignController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<ElectronicSignController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        #region 个人印章
        [Web.Extension.MenuFilter("印章管理", "个人印章", "/ElectronicSign/PersonalSignature",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-file-text", AuthorityId = ApplicationConstant.DefaultAuthorityId,
            DefaultRoleId = RoleConstants.DefaultRoleId, Order = 1, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("个人印章", "个人印章", "/ElectronicSign/PersonalSignature", ApplicationConstant.DefaultAuthorityId,
            DefaultRoleId = RoleConstants.DefaultRoleId, Order = 1, IsPage = true, ResultType = ResultType.ActionResult, AuthorityId = ApplicationConstant.DefaultAuthorityId)]
        public async Task<IActionResult> PersonalSignature()
        {
            var model = await _eSignService.GetElectronicPersonAsync(CurrentUserId);
            if (model == null)//企业和机构
            {
                model = new ElectronicPersonDTO()
                {
                    TenantName = TenantDisplayName,
                    UserId = CurrentUserId,
                    UserName = CurrentUserName,
                    Email = CurrentUserEmail,
                    Mobile = CurrentUserPhone,
                };
            }

            return View(model);
        }

        /// <summary>
        /// 注销个人印章
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> DeletePersonAccount()
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                return await _eSignService.RemoveElectronicPerson(CurrentUserId);
            });
        }
        #endregion

        #region 企业印章
        [Web.Extension.MenuFilter("印章管理", "企业印章", "/ElectronicSign/EnterpriseSignature",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-file-text", AuthorityId = "40716DEA-AB23-4DAD-AB25-4608CAC7C73F",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("企业印章", "企业印章", "/ElectronicSign/EnterpriseSignature", "40716DEA-AB23-4DAD-AB25-4608CAC7C73F",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = true, ResultType = ResultType.ActionResult, AuthorityId = "40716DEA-AB23-4DAD-AB25-4608CAC7C73F")]
        public async Task<IActionResult> EnterpriseSignature()
        {
            var model = await _eSignService.GetElectronicOrganizationAsync();
            if (model == null)//企业和机构
            {
                model = new ElectronicOrganizationDTO() {
                    Name = Tenant.TenantDisplayName,
                    //OrgNumber = Tenant.OrgUSCC,
                };
            }

            return View(model);
        }

        /// <summary>
        /// 检验印章状态
        /// </summary>
        /// <returns></returns>
        //public JsonResult IsSeal(bool? isPersonal)
        //{
        //    var userid = "";
        //    if (isPersonal.HasValue && isPersonal.Value)
        //    {
        //        userid = CurrentUserId;
        //    }
        //    return GetServiceJsonResult(() => _eSignService.IsSeal(userid, TenantName));
        //}

        /// <summary>
        /// 保存印章 到本地
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SubmitSeal(string mobile, string code, string blobId, string IdCade)
        {
            if (string.IsNullOrEmpty(mobile))
                return ThrowErrorJsonMessage(false, "手机号码不能为空");
            if (!mobile.IsMobile())
                return ThrowErrorJsonMessage(false, "手机号码格式不正确");
            if (string.IsNullOrEmpty(blobId))
                return ThrowErrorJsonMessage(false, "请上传企业授权书");
            if (string.IsNullOrEmpty(IdCade))
                return ThrowErrorJsonMessage(false, "请上传被授权人身份证");
            if (HttpContext.Session.GetString(mobile) == null)
                return ThrowErrorJsonMessage(false, "手机验证码错误");
            if (HttpContext.Session.GetString(mobile) != code)
                return ThrowErrorJsonMessage(false, "手机验证码错误");

            var success = BlobUtil.CopyTempsToClientBlob(Tenant, new List<string> { blobId, IdCade }, CurrentUserId);
            
            if (!success)
                return ThrowErrorJsonMessage(false, "文件保存失败，请重试!");
            return ThrowErrorJsonMessage(true, "操作成功!");
        }

        [HttpPost]
        public JsonResult SubmitSealPhone(string mobile, string code, string blobId, string IdCade)
        {
            if (string.IsNullOrEmpty(mobile))
                return ThrowErrorJsonMessage(false, "手机号码不能为空");
            if (!mobile.IsMobile())
                return ThrowErrorJsonMessage(false, "手机号码格式不正确");
            if (string.IsNullOrEmpty(blobId))
                return ThrowErrorJsonMessage(false, "请上传企业授权书");
            if (string.IsNullOrEmpty(IdCade))
                return ThrowErrorJsonMessage(false, "请上传被授权人身份证");
            if (HttpContext.Session.GetString(mobile) == null)
                return ThrowErrorJsonMessage(false, "手机验证码错误");
            if (HttpContext.Session.GetString(mobile) != code)
                return ThrowErrorJsonMessage(false, "手机验证码错误");

            var success = BlobUtil.CopyTempsToClientBlob(Tenant, new List<string> { blobId, IdCade }, CurrentUserId);

            if (!success)
                return ThrowErrorJsonMessage(false, "文件保存失败，请重试!");
            return ThrowErrorJsonMessage(true, "操作成功!");
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[ValidateMvcCaptcha("VerificationCode")]
        public JsonResult GenerateVerfiyPhoneCode(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile))
                return ThrowErrorJsonMessage(false, "手机号码不能为空");
            if (!mobile.IsMobile())
                return ThrowErrorJsonMessage(false, "手机号码格式不正确");
            if (!ModelState.IsValid)
                return ThrowErrorJsonMessage(false, "图形验证码不正确");
            if (!IsSendPhoneCode(mobile))
                return Json(new { success = true, message = string.Empty });


            var ra = new Random();
            var code = ra.Next(100000, 1000000).ToString();
            HttpContext.Session.SetString(mobile, code);
            SetSessionPhoneCode(mobile, code);

            LogUtil.LogInfo("----Set Phone Code in Session: " + code + ",Phone:" + mobile);
            var message = "贵司正在大陆之星赊销商城申请注册使用“e签宝”电子签章业务，本次注册验证码为：" + code + "。贵司已同意并授权贵司相关人员处理该注册事项，并保证申请时提交的所有资料均真实、合法、有效。否则，由此引起的一切法律后果均由贵司承担。为了保证贵司的账户安全，请勿泄露此验证码，如有疑问请致电400-788-8586";

            return Json(new { success = StorageQueueService.InsertSmsQueue(new SmsInfo() { Phone = new List<long> { long.Parse(mobile) }, SmsContent = message, Type = SmsType.Notice, Tenant = TenantName }), message = string.Empty });
        }
        private void SetSessionPhoneCode(string phone, string code)
        {
            var prefix = "CreateElectronic-PhoneCode-";
            var utcNow = DateTime.UtcNow;
            var content = new SmsContent();
            content.PhoneNumber = phone;
            content.ExpiredDateTime = utcNow.AddMinutes(TimeOutConstants.PhoneCodeTimeout);
            content.PhoneCode = code;
            HttpContext.Session.SetString(prefix + phone, SerializeHelper.ToJson(content));
        }
        private bool IsSendPhoneCode(string phone)
        {
            var prefix = "CreateElectronic-PhoneCode-";
            var utcNow = DateTime.UtcNow;
            var phoneCode = SerializeHelper.FromJson<SmsContent>(HttpContext.Session.GetString(prefix + phone));
            if (phoneCode == null)
                return true;
            if (phoneCode.ExpiredDateTime > utcNow)//有效期内，不用重复发送
                return false;
            return true;
        }

        /// <summary>
        /// e签宝服务条款
        /// </summary>
        /// <returns></returns>
        public PartialViewResult GetElectronicSignServiceClause()
        {
            return PartialView("_electronicSignServiceClause");
        }

        /// <summary>
        /// 注销印章
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> DeleteSealAccount()
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                return await _eSignService.RemoveElectronicOrganization();
            });
        }
        #endregion
    }
}