
using KC.Enums.Pay;
using KC.Framework.Base;
using KC.Framework.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Web;
using KC.Service.Pay.Response;
using System.Collections.Specialized;
using Microsoft.Extensions.Primitives;

namespace KC.Web.Pay.Controllers
{
    //[Web.Extension.MenuFilter("文件管理", "文件管理", "/Document/Index",
    //        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-folder", AuthorityId = "2C838EC1-AFD2-4ABC-8A41-9A74A3597D5A",
    //        DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsExtPage = false, Level = 2)]
    public class OnlinePaymentController : PaymentBaseController
    {
        public OnlinePaymentController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<OnlinePaymentController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        //[Web.Extension.PermissionFilter("文件管理", "文件管理", "/Document/Index", "2C838EC1-AFD2-4ABC-8A41-9A74A3597D5A",
        //    DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = true, ResultType = ResultType.ActionResult, AuthorityId = "2C838EC1-AFD2-4ABC-8A41-9A74A3597D5A")]
        public IActionResult Index()
        {
            ViewBag.CurrentUserName = CurrentUserName;
            return View();
        }

        #region 支付

        /// <summary>
        /// 创建支付单
        /// </summary>
        /// <param name="configSign">配置标记</param>
        /// <param name="configState">配置状态</param>
        /// <param name="payType"></param>
        /// <param name="orderAmount"></param>
        /// <param name="orderId"></param>
        /// <param name="type"></param>
        /// <param name="items"></param>
        /// <param name="asNumber"></param>
        /// <returns></returns>
        //public JsonResult CreatePayment(int configSign, int configState, int payType, int orderAmount, string orderId, CashType type = CashType.Transaction, List<AccountStatementPaymentItemDTO> items = null, string asNumber = null)
        //{
        //    return GetServiceJsonResult(() => PaymentService.CreatePaymentPlatform(CurrentUserId, CurrentUserDisplayName,
        //        CurrentUserMemberId, configSign, configState, payType, orderAmount, orderId, type, items, asNumber));
        //}



        #region 通联支付

        public IActionResult AllinpayPaymentFrontUrl(AllinPayResponse response)
        {

            return PaymentPickup(response, (int)ThirdPartyType.AllinpayConfigSign);
        }

        public IActionResult AllinpayPaymentBackUrl(AllinPayResponse response)
        {
            return PaymentReceive(response, (int)ThirdPartyType.AllinpayConfigSign);
        }

        #endregion

        #region 富友支付

        public IActionResult FuioupayPaymentFrontUrl(FuioupayResponse response)
        {

            return PaymentPickup(response, (int)ThirdPartyType.FuiouConfigSign);
        }

        public IActionResult FuioupayPaymentBackUrl(FuioupayResponse response)
        {
            return PaymentReceive(response, (int)ThirdPartyType.FuiouConfigSign);
        }

        #endregion

        #region 招行CBS支付



        /// <summary>
        /// 获取平台客户招行cbs账户,根据登录用户公司名称从cbs中间库查询
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCBSAccounts(bool isAdminPortal, ConfigType payType)
        {
            var displayName = CurrentUserDisplayName;

            var result = PaymentService.GetCBSAccounts(isAdminPortal, displayName, payType);
            if (result.Item1)
            {
                return Json(new { success = true, result.Item2 });
            }

            return ThrowErrorJsonMessage(false, result.Item3);
        }



        /// <summary>
        /// cbs在线支付
        /// </summary>
        /// <param name="cbsAccount"></param>
        /// <param name="payType"></param>
        /// <param name="orderAmount"></param>
        /// <param name="orderId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public JsonResult CBSPaymentMethod(bool isAdminPortal, string cbsAccount, int configSign, int configState, ConfigType payType, int orderAmount, string orderId, CashType type = CashType.Transaction)
        {
            var result = PaymentService.CBSPaymentMethod(isAdminPortal, CurrentUserId, CurrentUserDisplayName, cbsAccount, configSign, configState, payType,
             orderAmount, orderId, type);
            return ThrowErrorJsonMessage(result.Item1, result.Item2);
        }


        public ContentResult CBSBackUrl(string userId, CMBCBSResponse response)
        {
            var paymentReturnModel = PaymentService.PaymentBackUrl(CurrentUserId, response, (int)ThirdPartyType.CMBCBSConfigSign);
            if (!paymentReturnModel.Success)
            {
                return Content(paymentReturnModel.ErrorMessage);
            }
            return Content("Success");
        }

        #endregion

        #region 银联

        public IActionResult UnionpayPaymentFrontUrl()
        {
            UnionResponse response = new UnionResponse();
            response.Collection = GetFormCollection(Request.Form);
            response.HttpMethod = Request.Method;
            return PaymentPickup(response, (int)ThirdPartyType.UnionpayConfigSign);
        }

        public IActionResult UnionpayPaymentBackUrl()
        {
            UnionResponse response = new UnionResponse();
            response.Collection = GetFormCollection(Request.Form);
            response.HttpMethod = Request.Method;
            return PaymentReceive(response, (int)ThirdPartyType.UnionpayConfigSign);
        }

        private NameValueCollection GetFormCollection(IFormCollection form)
        {
            var result = new NameValueCollection();
            foreach(var key in form.Keys)
            {
                StringValues value = new StringValues();
                var success = form.TryGetValue(key, out value);
                if (success)
                    result.Add(key, value);
            }
            return result;
        }
        #endregion

        #region 合利支付
        public IActionResult HeliPayPaymentFrontUrl(HeliPayResponse response)
        {
            return PaymentPickup(response, (int)ThirdPartyType.HeliPayConfigSign);
        }

        public IActionResult HeliPayPaymentBackUrl(HeliPayResponse response)
        {
            return PaymentReceive(response, (int)ThirdPartyType.HeliPayConfigSign);
        }

        #endregion

        #endregion
    }
}