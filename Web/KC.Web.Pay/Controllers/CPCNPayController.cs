using KC.Common;
using KC.Service.DTO.Admin;
using KC.Service.DTO.Pay;
using KC.Enums.Pay;
using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Model.Component.Queue;
using KC.Service.Base;
using KC.Service.Constants;
using KC.Service.Enums;
using KC.Service.Pay;
using KC.Service.Pay.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Web;
using KC.Common.HttpHelper;

namespace KC.Web.Pay.Controllers
{
    //[Web.Extension.MenuFilter("文件管理", "文件模板管理", "/DocTemplate/Index",
    //        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-file-text", AuthorityId = "875540DC-9C97-4385-BF81-B9C6F8F1B91C",
    //        DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsExtPage = false, Level = 2)]
    public class CPCNPayController : PaymentBaseController
    {
        protected KC.Service.Component.IStorageQueueService StorageQueueService
        {
            get
            {
                //TODO: Storage with TenantName
                return ServiceProvider.GetService<KC.Service.Component.IStorageQueueService>();
            }
        }
        protected IPaymentService _docTemplateService => ServiceProvider.GetService<IPaymentService>();
        public CPCNPayController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<CPCNPayController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        //[Web.Extension.PermissionFilter("文件模板管理", "文件模板管理", "/DocTemplate/Index", "875540DC-9C97-4385-BF81-B9C6F8F1B91C",
        //    DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = true, ResultType = ResultType.ActionResult, AuthorityId = "875540DC-9C97-4385-BF81-B9C6F8F1B91C")]
        public ActionResult Index()
        {
            ViewBag.CurrentUserName = CurrentUserName;
            ViewBag.StatusList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<ApprovalStatus>();
            return View();
        }

        /// <summary>
        /// 中金支付开户 T1001
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult OpenAccount(PayBaseParamDTO paramDTO)
        {
            return GetServiceJsonResult(() =>
            {
                string postUrl = PayApiDomain + CPCNMethodConstants.OpenAccount;
                return PaymentReturn(paramDTO, postUrl);
            });
        }

        /// <summary>
        ///  企业-企业账户认证(打款认证)-申请[T1131]
        /// </summary>
        /// <param name="paramDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult BankAuthenticationAppliction(BankAuthenticationApplicationDTO paramDTO)
        {
            return GetServiceJsonResult(() =>
            {
                string postUrl = PayApiDomain + CPCNMethodConstants.BankAuthenticationAppliction;
                return PaymentReturn(paramDTO, postUrl);
            });
        }

        /// <summary>
        /// 企业-企业账户认证(打款认证)-验证[T1132]
        /// </summary>
        /// <param name="paramDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult BankAuthentication(BankAuthenticationDTO paramDTO)
        {
            return GetServiceJsonResult(() =>
            {
                string postUrl = PayApiDomain + CPCNMethodConstants.BankAuthentication;
                var returnModel = PaymentReturn(paramDTO, postUrl);
                return returnModel;
            });
        }

        /// <summary>
        /// 绑定/解绑银行卡
        /// </summary>
        /// <param name="paramDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult BindBankAccount(BindBankAccountDTO paramDTO)
        {
            return GetServiceJsonResult(() =>
            {
                string postUrl = PayApiDomain + CPCNMethodConstants.BindBankAccount;
                return PaymentReturn(paramDTO, postUrl);
            });
        }

        /// <summary>
        /// 资金账户余额查询
        /// </summary>
        /// <param name="paramDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SearchAccountBalance(PayBaseParamDTO paramDTO)
        {
            return GetServiceJsonResult(() =>
            {
                string postUrl = PayApiDomain + CPCNMethodConstants.SearchAccountBalance;
                return PaymentReturn(paramDTO, postUrl);
            });
        }

        /// <summary>
        /// 支付行信息模糊查询[T1017]
        /// </summary>
        /// <param name="paramDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult PaymentBankInfo(BankInfoFilterDTO paramDTO)
        {
            return GetServiceJsonResult(() =>
            {
                string postUrl = PayApiDomain + CPCNMethodConstants.PaymentBankInfo;
                return PaymentReturn(paramDTO, postUrl);
            });
        }

        /// <summary>
        /// 解冻/冻结 金额
        /// </summary>
        /// <param name="paramDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult FreezeAmt(FreezeAmtDTO paramDTO)
        {
            return GetServiceJsonResult(() =>
            {
                string postUrl = PayApiDomain + CPCNMethodConstants.FreezeAmt;
                return PaymentReturn(paramDTO, postUrl);
            });
        }

        /// <summary>
        /// 订单支付
        /// </summary>
        /// <param name="paramDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult OrderPay(OrderPayDTO paramDTO)
        {
            return GetServiceJsonResult(() =>
            {
                string postUrl = PayApiDomain + CPCNMethodConstants.OrderPay;
                return PaymentReturn(paramDTO, postUrl);
            });
        }

        /// <summary>
        /// 通知类
        /// </summary>
        /// <param name="receiveNoticeDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public string Notice(ReceiveNoticeDTO receiveNoticeDTO)
        {
            string postUrl = PayApiDomain + CPCNMethodConstants.Notice;
            //Logger.LogInfo(receiveNoticeDTO.ptncode + "," + receiveNoticeDTO.trdcode + "," + receiveNoticeDTO.signature + "," + receiveNoticeDTO.message);
            var postData = KC.Common.ToolsHelper.OtherUtilHelper.GetPostData(receiveNoticeDTO);
            var result = HttpWebRequestHelper.WebClientDownload(postUrl, postData);
            var retString = string.Empty;
            if (result.Item1)
            {
                //Logger.LogInfo(result.Item2.ToString());
                var returnModel = SerializeHelper.FromJson<PaymentReturnModel>(result.Item2);
                retString = returnModel.ReturnData == null ? "" : returnModel.ReturnData.ToString();
            }
            //Logger.LogInfo(retString);
            return retString;
        }

        /// <summary>
        /// 充值交易
        /// </summary>
        /// <param name="paramDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult InTransaction(InTransactionDTO paramDTO)
        {
            return GetServiceJsonResult(() =>
            {
                string postUrl = PayApiDomain + CPCNMethodConstants.InTransaction;
                var returnModel = PaymentReturn(paramDTO, postUrl);
                returnModel.ReturnData = HttpUtility.UrlDecode(returnModel.ReturnData == null ? "" : returnModel.ReturnData.ToString(), System.Text.Encoding.UTF8);
                return returnModel;
            });
        }

        /// <summary>
        /// 扫码入金
        /// </summary>
        /// <param name="paramDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult QRInTransaction(InTransactionDTO paramDTO)
        {
            return GetServiceJsonResult(() =>
            {
                string postUrl = PayApiDomain + CPCNMethodConstants.QRInTransaction;
                var returnModel = PaymentReturn(paramDTO, postUrl);
                returnModel.ReturnData = HttpUtility.UrlDecode(returnModel.ReturnData == null ? "" : returnModel.ReturnData.ToString(), System.Text.Encoding.UTF8);
                return returnModel;
            });
        }


        /// <summary>
        /// 充值交易查询
        /// </summary>
        /// <param name="paramDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult InTransactionSearch(InTransactionSearchDTO paramDTO)
        {
            return GetServiceJsonResult(() =>
            {
                string postUrl = PayApiDomain + CPCNMethodConstants.InTransactionSearch;
                return PaymentReturn(paramDTO, postUrl);
            });
        }

        /// <summary>
        /// 出金
        /// </summary>
        /// <param name="paramDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult OutTransaction(OutTransactionDTO paramDTO)
        {
            return GetServiceJsonResult(() =>
            {
                string postUrl = PayApiDomain + CPCNMethodConstants.OutTransaction;
                return PaymentReturn(paramDTO, postUrl);
            });
        }

        /// <summary>
        /// 查找可出金余额
        /// </summary>
        /// <param name="paramDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SearchWithdrawAmt(PayBaseParamDTO paramDTO)
        {
            return GetServiceJsonResult(() =>
            {
                string postUrl = PayApiDomain + CPCNMethodConstants.SearchWithdrawAmt;
                return PaymentReturn(paramDTO, postUrl);
            });
        }

        public ActionResult Charge(PaymentViewModelDTO model)
        {
            //查找此支付订单是否已经完成。
            var onlinePaymentRecord = PaymentService.GetOnlinePaymentRecordByPaymentId(model.PaymentOrderId);
            if (onlinePaymentRecord != null)
            {
                if (onlinePaymentRecord.PayResult == "1")
                {
                    ViewBag.Message = string.Format("此充值订单已完成。订单号为：{0}", model.PaymentOrderId);
                    return View("~Error");
                }
                if (onlinePaymentRecord.PayResult == "2")
                {
                    ViewBag.Message = string.Format("此充值订单失败，失败原因{0}。订单号为：{1}", onlinePaymentRecord.ErrorMessage, model.PaymentOrderId);
                    return View("~Error");
                }
                if (onlinePaymentRecord.PayResult == "3")
                {
                    ViewBag.Message = string.Format("此充值订单正在充值中。订单号为：{0}", model.PaymentOrderId);
                    return View("~Error");
                }

                InTransactionSearchDTO paramDTO = new InTransactionSearchDTO();
                paramDTO.MemberId = model.MemberId;
                paramDTO.UserName = model.UserName;
                paramDTO.PaymentOrderId = model.PaymentOrderId;
                paramDTO.Timestamp = ConvertDateTimeToInt(DateTime.UtcNow);
                var tenantUser = TenantStore.GetTenantByName(model.MemberId).Result;
                paramDTO.EncryptString = KC.Common.ToolsHelper.OtherUtilHelper.GetStrByModel(paramDTO, tenantUser.PrivateEncryptKey);
                string postUrl = PayApiDomain + CPCNMethodConstants.InTransactionSearch;
                var paymetReurnModel = PaymentReturn(paramDTO, postUrl);
                if (paymetReurnModel.Success)
                {
                    ViewBag.Message = string.Format("此充值订单已完成。订单号为：{0}", model.PaymentOrderId);
                    return View("~Error");
                }
                else
                {
                    if (paymetReurnModel.ReturnData.ToString() == "3")
                    {
                        ViewBag.Message = string.Format("该充值订单正在处理中，订单号为：{0}", model.PaymentOrderId);
                        return View("~Error");
                    }
                    ViewBag.Message = string.Format("充值订单异常，请联系客服。订单号为：{0}", model.PaymentOrderId);
                    return View("~Error");
                }
            }


            var paymentInfo = GetPaymentInfo(model.MemberId, ThirdPartyType.CPCNConfigSign);
            model.Phone = paymentInfo.Phone;

            var isLegal = IsLegalData(model, model.EncryptString).Result;
            if (!isLegal)
            {
                ViewBag.Message = string.Format("数据检验失败");
                return View("~Error");
            }

            return View(model);
        }


        public ActionResult OrderPayment(PaymentViewModelDTO model)
        {
            //model.Amount = 1;
            //model.GoodsName = "商品123333";
            //model.OrderAmount = 1;
            //model.OrderNo = "SSO2018050300002";
            //model.OrderType = "采购订单";
            //model.Payee = "千里马公司";
            //model.PayeeTenant = "UR2018051700011";
            //model.PaymentOrderId = "PayR2018050710272758888";
            //model.Usage = "采购订单008";
            //model.MemberId = "DevDB";
            //model.PayType = 2;
            //查找此支付订单是否已经完成。
            var payResult = PaymentService.IsOnlinePaymentSuccess(model.PaymentOrderId);
            if (payResult)
            {
                ViewBag.Message = string.Format("此订单已完成。订单号为：{0}", model.PaymentOrderId);
                return View("~Error");
            }

            var paymentInfo = GetPaymentInfo(model.MemberId, ThirdPartyType.CPCNConfigSign);
            model.Phone = paymentInfo.Phone;
            //var tenantUser = TenantStore.GetTenantByName(model.MemberId);
            //model.Timestamp = ConvertDateTimeToInt(DateTime.UtcNow);
            //model.EncryptString = KC.Common.ToolsHelper.OtherUtilHelper.GetStrByModel(model, tenantUser.PrivateEncryptKey);
            var isLegal = IsLegalData(model, model.EncryptString).Result;
            if (!isLegal)
            {
                ViewBag.Message = string.Format("数据检验失败！");
                return View("~Error");
            }

            return View(model);
        }

        public ActionResult Withdraw(PaymentViewModelDTO model)
        {
            //查找此支付订单是否已经完成。
            var payResult = PaymentService.IsOnlinePaymentSuccess(model.PaymentOrderId);
            if (payResult)
            {
                ViewBag.Message = string.Format("此提现订单已完成。订单号为：{0}", model.PaymentOrderId);
                return View("~Error");
            }
            var paymentInfo = GetPaymentInfo(model.MemberId, ThirdPartyType.CPCNConfigSign);
            model.Phone = paymentInfo.Phone;
            var isLegal = IsLegalData(model, model.EncryptString).Result;
            if (!isLegal)
            {
                ViewBag.Message = string.Format("数据检验失败");
                return View("~Error");
            }

            return View(model);
        }

        public JsonResult SubmitInTransactionSearch(PaymentViewModelDTO model)
        {
            PaymentReturnModel returnModel = new PaymentReturnModel();
            //根据memberId 获取TenantUser 的PricateEncryptKey
            var tenantUser = TenantStore.GetTenantByName(model.MemberId).Result;
            //查找此支付订单是否已经完成。
            var payResult = PaymentService.IsOnlinePaymentSuccess(model.PaymentOrderId);
            if (payResult)
            {
                return GetServiceJsonResult(() =>
                {
                    returnModel.Success = true;
                    return returnModel;
                });
            }

            InTransactionSearchDTO paramDTO = new InTransactionSearchDTO();
            paramDTO.MemberId = model.MemberId;
            paramDTO.UserName = model.UserName;
            paramDTO.PaymentOrderId = model.PaymentOrderId;
            paramDTO.Timestamp = ConvertDateTimeToInt(DateTime.UtcNow);
            paramDTO.EncryptString = KC.Common.ToolsHelper.OtherUtilHelper.GetStrByModel(paramDTO, tenantUser.PrivateEncryptKey);

            return InTransactionSearch(paramDTO);
        }

        [HttpPost]
        public JsonResult SubmitQRCharge(PaymentViewModelDTO model)
        {
            PaymentReturnModel returnModel = new PaymentReturnModel();
            //根据memberId 获取TenantUser 的PricateEncryptKey
            var tenantUser = TenantStore.GetTenantByName(model.MemberId).Result;
            //查找此支付订单是否已经完成。
            var payResult = PaymentService.IsOnlinePaymentSuccess(model.PaymentOrderId);
            if (payResult)
            {
                return GetServiceJsonResult(() =>
                {
                    returnModel.Success = false;
                    returnModel.ErrorMessage = "此订单已扫码支付成功，不可重复提交！";
                    return returnModel;
                });
            }
            InTransactionDTO paramDTO = new InTransactionDTO();
            paramDTO.MemberId = model.MemberId;
            paramDTO.UserName = model.UserName;
            paramDTO.Usage = model.Usage;
            paramDTO.PaymentOrderId = model.PaymentOrderId;
            paramDTO.PeeMemberId = model.PayeeTenant;
            paramDTO.OrderNo = model.OrderNo;
            paramDTO.SecPayType = model.SecPayType;
            paramDTO.Amount = model.Amount * 100; //单位为分
            paramDTO.Timestamp = ConvertDateTimeToInt(DateTime.UtcNow);
            paramDTO.EncryptString = KC.Common.ToolsHelper.OtherUtilHelper.GetStrByModel(paramDTO, tenantUser.PrivateEncryptKey);

            return QRInTransaction(paramDTO);
        }


        /// <summary>
        /// 提交充值申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SubmitCharge(PaymentViewModelDTO model)
        {
            PaymentReturnModel returnModel = new PaymentReturnModel();
            //根据memberId 获取TenantUser 的PricateEncryptKey
            var tenantUser = TenantStore.GetTenantByName(model.MemberId).Result;
            ////校验数据
            //returnModel = VerfiyData(model.MemberId, model.PhoneCode, model.Password, tenantUser);
            //if (!returnModel.Success)
            //{
            //    return GetServiceJsonResult(() =>
            //    {
            //        return returnModel;
            //    });
            //}
            //查找此支付订单是否已经完成。
            var payResult = PaymentService.IsOnlinePaymentSuccess(model.PaymentOrderId);
            if (payResult)
            {
                return GetServiceJsonResult(() =>
                {
                    returnModel.Success = false;
                    returnModel.ErrorMessage = "此订单已充值成功，不可重复提交！";
                    return returnModel;
                });
            }

            InTransactionDTO paramDTO = new InTransactionDTO();
            paramDTO.MemberId = model.MemberId;
            paramDTO.UserName = model.UserName;
            paramDTO.Usage = model.Usage;
            paramDTO.PaymentOrderId = model.PaymentOrderId;
            paramDTO.Amount = model.Amount * 100; //单位为分
            paramDTO.Timestamp = ConvertDateTimeToInt(DateTime.UtcNow);
            paramDTO.EncryptString = KC.Common.ToolsHelper.OtherUtilHelper.GetStrByModel(paramDTO, tenantUser.PrivateEncryptKey);

            return InTransaction(paramDTO);
        }

        /// <summary>
        /// 提交订单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SubmitOrderPay(PaymentViewModelDTO model)
        {
            PaymentReturnModel returnModel = new PaymentReturnModel();
            //根据memberId 获取TenantUser 的PricateEncryptKey
            var tenantUser = TenantStore.GetTenantByName(model.MemberId).Result;
            //校验数据
            returnModel = VerfiyData(model.MemberId, model.PhoneCode, model.Password, tenantUser);
            if (!returnModel.Success)
            {
                return GetServiceJsonResult(() =>
                {
                    return returnModel;
                });
            }
            //查找此支付订单是否已经完成。
            var payResult = PaymentService.IsOnlinePaymentSuccess(model.PaymentOrderId);
            if (payResult)
            {
                return GetServiceJsonResult(() =>
                {
                    returnModel.Success = false;
                    returnModel.ErrorMessage = "此订单已交易成功，不可重复提交！";
                    return returnModel;
                });
            }

            OrderPayDTO paramDTO = new OrderPayDTO();
            paramDTO.MemberId = model.MemberId;
            paramDTO.UserName = model.UserName;
            paramDTO.Usage = model.Usage;
            paramDTO.OrderNo = model.OrderNo;
            paramDTO.PaymentOrderId = model.PaymentOrderId;
            paramDTO.PayeeAccountName = model.Payee;
            var payeePaymentInfo = GetPaymentInfo(model.PayeeTenant, ThirdPartyType.CPCNConfigSign);
            if (payeePaymentInfo == null)
            {
                returnModel.Success = false;
                returnModel.ErrorMessage = "收款方没有开通财富共赢钱包账号";
                return GetServiceJsonResult(() =>
                {
                    return returnModel;
                });
            }
            paramDTO.PayeeAccountNumber = payeePaymentInfo.PaymentAccount;
            paramDTO.PayType = model.PayType;
            paramDTO.Amount = model.Amount * 100; //单位为分
            paramDTO.Timestamp = ConvertDateTimeToInt(DateTime.UtcNow);
            paramDTO.EncryptString = KC.Common.ToolsHelper.OtherUtilHelper.GetStrByModel(paramDTO, tenantUser.PrivateEncryptKey);

            return OrderPay(paramDTO);
        }

        /// <summary>
        /// 提交出金
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SubmitWithdraw(PaymentViewModelDTO model)
        {
            PaymentReturnModel returnModel = new PaymentReturnModel();
            //根据memberId 获取TenantUser 的PricateEncryptKey
            var tenantUser = TenantStore.GetTenantByName(model.MemberId).Result;
            //校验数据
            returnModel = VerfiyData(model.MemberId, model.PhoneCode, model.Password, tenantUser);
            if (!returnModel.Success)
            {
                return GetServiceJsonResult(() =>
                {
                    return returnModel;
                });
            }
            //查找此支付订单是否已经完成。
            var payResult = PaymentService.IsOnlinePaymentSuccess(model.PaymentOrderId);
            if (payResult)
            {
                return GetServiceJsonResult(() =>
                {
                    returnModel.Success = false;
                    returnModel.ErrorMessage = "此订单已交易成功，不可重复提交！";
                    return returnModel;
                });
            }

            OutTransactionDTO paramDTO = new OutTransactionDTO();
            paramDTO.MemberId = model.MemberId;
            paramDTO.UserName = model.UserName;
            paramDTO.Usage = model.Usage;
            paramDTO.PaymentOrderId = model.PaymentOrderId;
            paramDTO.Amount = model.Amount * 100; //单位为分
            paramDTO.BalFlag = 2;
            paramDTO.FeeAmount = model.FeeAmount * 100; //单位为分
            paramDTO.Timestamp = ConvertDateTimeToInt(DateTime.UtcNow);
            paramDTO.EncryptString = KC.Common.ToolsHelper.OtherUtilHelper.GetStrByModel(paramDTO, tenantUser.PrivateEncryptKey);

            return OutTransaction(paramDTO);
        }

        /// <summary>
        /// 发送手机验证码
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <param name="type">请求类型： 1 充值 2 订单支付 3 出金</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public JsonResult GenerateVerfiyPhoneCode(string memberId, int type, string amount, string payeeName)
        {

            var paymentInfo = GetPaymentInfo(memberId, ThirdPartyType.CPCNConfigSign);
            string phone = paymentInfo.Phone;

            if (string.IsNullOrEmpty(phone))
            {
                return Json(new { message = "手机号码不能为空", success = false });
            }
            var bl = phone.IsMobile();
            if (!bl)
            {
                return Json(new { message = "手机号码格式不正确", success = false });
            }

            if (!IsSendPhoneCode(phone))
                return Json(new { success = true, message = string.Empty });
            var ra = new Random();
            var code = ra.Next(100000, 1000000).ToString();
            HttpContext.Session.SetString(phone, code);
            SetSessionPhoneCode(phone, code);
            Logger.LogInformation("----Set Phone Code in Session: " + code + ",Phone:" + phone);
            var message = "";
            if (type == 1)
            {
                message = string.Format("{0}（充值验证码），您正在进行充值，金额为{1}元，请勿向他人泄露您的验证码。", code, amount);
            }
            else if (type == 2)
            {
                message = string.Format("{0}（交易验证码），您正在向{1}进行支付，金额为{2}元，请勿向他人泄露您的验证码。", code, payeeName, amount);
            }
            else if (type == 3)
            {
                message = string.Format("{0}（交易验证码），您正在进行提现，金额为{1}元，请勿向他人泄露您的验证码。", code, amount);
            }

            //var storageType = ConfigUtil.GetConfigItem("BlobStorage").ToLower();
            //Logger.LogInformation(storageType + "  StorageQueueService: " + typeof(StorageQueueService).FullName);

            return Json(new { success = StorageQueueService.InsertSmsQueue(new SmsInfo() { Phone = new List<long> { long.Parse(phone) }, SmsContent = message, Type = SmsType.Notice, Tenant = memberId }), message = string.Empty });
        }

        private void SetSessionPhoneCode(string phone, string code)
        {
            var prefix = "CreateElectronic-phonecode-";
            var utcNow = DateTime.UtcNow;
            var content = new SmsContent();
            content.PhoneNumber = phone;
            content.ExpiredDateTime = utcNow.AddMinutes(TimeOutConstants.PhoneCodeTimeout);
            content.PhoneCode = code;
            HttpContext.Session.SetString(prefix + phone, SerializeHelper.ToJson(content));
        }
        private bool IsSendPhoneCode(string phone)
        {
            var prefix = "CreateElectronic-phonecode-";
            var utcNow = DateTime.UtcNow;
            var phoneCode = SerializeHelper.FromJson<SmsContent>(HttpContext.Session.GetString(prefix + phone));
            if (phoneCode == null)
                return true;
            //if (phoneCode.ExpiredDateTime > utcNow)//有效期内，不用重复发送
            //    return false;
            return true;
        }

        /// <summary>
        /// 验证提交过来的数据
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="phone"></param>
        /// <param name="code"></param>
        /// <param name="password"></param>
        /// <param name="tenantUser"></param>
        /// <returns></returns>
        private PaymentReturnModel VerfiyData(string memberId, string code, string password, Tenant tenantUser)
        {
            PaymentReturnModel returnModel = new PaymentReturnModel();
            returnModel.Success = false;
            var paymentInfo = GetPaymentInfo(memberId, ThirdPartyType.CPCNConfigSign);
            string phone = paymentInfo.Phone;


            #region 验证密码


            if (paymentInfo == null)
            {
                returnModel.ErrorMessage = "密码错误！";

                return returnModel;
            }
            if (string.IsNullOrEmpty(paymentInfo.TradePassword))
            {
                returnModel.ErrorMessage = "未设置交易密码！";

                return returnModel;
            }


            var encryPassword = KC.Common.ToolsHelper.OtherUtilHelper.GetEncryptionString(password, tenantUser.PrivateEncryptKey);
            if (paymentInfo.TradePassword != encryPassword)
            {
                returnModel.ErrorMessage = "交易密码错误！";

                return returnModel;
            }
            #endregion


            #region 校验

            if (string.IsNullOrEmpty(phone))
            {
                returnModel.ErrorMessage = "手机号码不能为空";

                return returnModel;
            }
            var bl = System.Text.RegularExpressions.Regex.IsMatch(phone, @"^[1]+[3,5,4,7,8]+\d{9}");

            if (HttpContext.Session.GetString(phone) != null)
            {
                if (HttpContext.Session.GetString(phone) != code)
                {
                    returnModel.ErrorMessage = "手机验证码错误";

                    return returnModel;
                }
                else
                {
                    HttpContext.Session.SetString(phone, "");
                }
            }
            else
            {
                returnModel.ErrorMessage = "手机验证码错误";

                return returnModel;
            }

            if (!bl)
            {
                returnModel.ErrorMessage = "手机号码格式不正确";

                return returnModel;
            }

            #endregion

            returnModel.Success = true;
            return returnModel;
        }
    }
}
