using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using KC.Service.Pay;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using KC.Common.HttpHelper;
using KC.Framework.SecurityHelper;
using KC.Service.Pay.SDK;
using KC.Common.ToolsHelper;
using KC.Service.Pay.Constants;
using KC.Service.DTO.Pay;
using KC.Enums.Pay;
using KC.WebApi.Pay.Models;
using KC.Component.Util;
using KC.Framework.Base;
using KC.Service.Pay.WebApiService.Platform;
using KC.Service.WebApiService.Business;

namespace KC.WebApi.Pay.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class CPCNPayApiController : PayBaseController
    {
        private IPaymentService PaymentService => ServiceProvider.GetService<IPaymentService>();
        private IPaymentApiService PaymentApiService => ServiceProvider.GetService<IPaymentApiService>();
        private IConfigApiService ConfigApiService => ServiceProvider.GetService<IConfigApiService>();

        public CPCNPayApiController(
            Tenant tenant, IServiceProvider serviceProvider,
            ILogger<CPCNPayApiController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        /// <summary>
        /// 中金支付开户 T1001
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult OpenAccount(PayBaseParamDTO paramDTO)
        {
            string memberId = paramDTO.MemberId;
            string userName = paramDTO.UserName;
            string postXML = string.Empty;
            string returnXML = string.Empty;
            PaymentReturnModel returnModel = new PaymentReturnModel();
            try
            {
                //先查看是否开通
                var isOpenPayment = PaymentService.IsOpenPayment(memberId, ThirdPartyType.CPCNConfigSign);
                if (isOpenPayment)
                {
                    returnModel.Success = false;
                    returnModel.ErrorMessage = "已开通钱包，不需要重复开通。";
                    return Json(returnModel);
                }

                //获取中金支付的配置
                CPCNConfigDTO configModel = new CPCNConfigDTO();
                returnModel = GetCPCNConfig(configModel);
                if (!returnModel.Success)
                {
                    return Json(returnModel);
                }
                //查询是否认证
                var unitAuthentication = PaymentApiService.GetUnitAuthenticationByMemberName(memberId).Result;
                if (unitAuthentication == null)
                {
                    returnModel.Success = false;
                    returnModel.ErrorMessage = "企业未认证！";
                    returnModel.ErrorCode = "01";
                    return Json(returnModel);
                }
                if (string.IsNullOrEmpty(unitAuthentication.UnifiedSocialCreditCode))
                {
                    returnModel.Success = false;
                    returnModel.ErrorMessage = "开通失败！原因：统一社会信用代码为空。请您登出系统后，以会员身份登录，重新进行企业认证并完善企业法人信息，谢谢！";
                    returnModel.ErrorCode = "01";
                    return Json(returnModel);
                }
                if (string.IsNullOrEmpty(unitAuthentication.LegalPerson) || string.IsNullOrEmpty(unitAuthentication.LegalPersonIdentityCardNumber))
                {
                    if (string.IsNullOrEmpty(unitAuthentication.ManagersPerson) || string.IsNullOrEmpty(unitAuthentication.ManagersPersonIdentityCardNumber))
                    {
                        returnModel.Success = false;
                        returnModel.ErrorMessage = "开通失败！原因：法人信息/经办人信息不完善。请您登出系统后，以会员身份登录，重新进行企业认证并完善企业法人信息，谢谢！";
                        returnModel.ErrorCode = "01";
                        return Json(returnModel);
                    }
                }

                if (string.IsNullOrEmpty(unitAuthentication.CompanyAddress))
                {
                    returnModel.Success = false;
                    returnModel.ErrorMessage = "开通失败！原因：公司地址为空，请完善企业基本信息";
                    returnModel.ErrorCode = "01";
                    return Json(returnModel);
                }
                //流水号唯一
                var ptnSrl = OtherUtilHelper.GetSerialNumber("ZJ");
                KC.WebApi.Pay.Models.AccountTransaction.OpenAccount.MSG msg = new KC.WebApi.Pay.Models.AccountTransaction.OpenAccount.MSG();

                //公共信息
                msg.MSGHD = GetMessageHead(configModel, CPCNInterfaceConstants.OpenAccount);

                //流水号
                Srl srl = new Srl();
                //账户信息
                CltAcc cltAcc = new CltAcc();
                //客户信息
                KC.WebApi.Pay.Models.AccountTransaction.OpenAccount.Clt clt = new KC.WebApi.Pay.Models.AccountTransaction.OpenAccount.Clt();


                srl.PtnSrl = ptnSrl;
                msg.Srl = srl;

                cltAcc.CltNo = memberId;
                cltAcc.CltNm = unitAuthentication.CompanyName;
                msg.CltAcc = cltAcc;

                clt.Kd = 1;
                clt.Nm = string.IsNullOrEmpty(unitAuthentication.LegalPerson) ? unitAuthentication.ManagersPerson : unitAuthentication.LegalPerson;
                clt.CdTp = "A";
                clt.CdNo = string.IsNullOrEmpty(unitAuthentication.LegalPersonIdentityCardNumber) ? unitAuthentication.ManagersPersonIdentityCardNumber : unitAuthentication.LegalPersonIdentityCardNumber;
                clt.UscId = unitAuthentication.UnifiedSocialCreditCode;

                clt.MobNo = unitAuthentication.ContactPhone;
                clt.Email = unitAuthentication.CompanyEmail;
                clt.Addr = unitAuthentication.CompanyAddress;

                msg.Clt = clt;

                msg.FcFlg = 1;
                msg.AccTp = 1;

                postXML = CPCNPayment.ModelToXml(msg, configModel.Version);

                OpenAccountDTO openAccountDTO = new OpenAccountDTO();
                returnXML = PostCPCNData(configModel, CPCNInterfaceConstants.OpenAccount, postXML);

                var cpcnRetModel = CPCNPayment.XmlToModel<KC.WebApi.Pay.Models.AccountTransaction.OpenAccount.ReturnResponse.MSG>(returnXML);
                //中金返回的流水号
                string retSrl = "";
                var retCode = cpcnRetModel.MSGHD.RspCode;
                var retMessage = cpcnRetModel.MSGHD.RspMsg;
                //中金返回的账号
                var retSubNo = "";
                //交易是否成功
                bool isSuccess = false;
                PaymentBankAccountDTO bankAccount = new PaymentBankAccountDTO();
                //返回成功的处理
                if (retCode.Equals(CPCNReturnCodeConstants.Success))
                {
                    isSuccess = true;
                    var retCltAcc = cpcnRetModel.CltAcc;
                    retSubNo = retCltAcc.SubNo;
                    bankAccount.AccountNo = retSubNo;
                    bankAccount.AccountName = retCltAcc.CltNm;
                    bankAccount.BankEId = retCltAcc.BnkEid;
                    bankAccount.MemberId = memberId;
                    bankAccount.State = 1;
                    bankAccount.OpenBankCode = retCltAcc.OpenBkCd;
                    bankAccount.OpenBankName = retCltAcc.OpenBkNm;
                    bankAccount.PaymentType = ThirdPartyType.CPCNConfigSign;
                    bankAccount.CreatedBy = userName;
                    bankAccount.CFWinFreezeAmount = 0;
                    bankAccount.CFWinTotalAmount = 0;
                    bankAccount.AmountUpdateTime = DateTime.Now;
                    retSrl = cpcnRetModel.Srl.PlatSrl;
                }
                else
                {
                    returnModel.Success = false;
                    returnModel.ErrorCode = retCode;
                    returnModel.ErrorMessage = string.Format("交易失败，失败原因：{0}", retMessage);
                    Logger.LogError(string.Format("中金支付交易失败，失败原因：{0}，平台流水号：{1}。请求的XML:{2}，返回的XML{3}", retMessage, ptnSrl, postXML, returnXML));
                }

                openAccountDTO.IsSuccess = isSuccess;
                //交易流水
                var tradeRecord = AddPaymentTradeRecord(postXML, returnXML, memberId, ptnSrl, retSrl, CPCNInterfaceConstants.OpenAccount, PaymentOperationType.OpenAccount,
                    isSuccess, retMessage, retCode, ThirdPartyType.CPCNConfigSign, userName);
                openAccountDTO.PaymentTradeRecord = tradeRecord;
                //开户信息
                openAccountDTO.CPCNBankAccount = bankAccount;

                //支付表信息
                PaymentInfoDTO paymentInfo = new PaymentInfoDTO();
                paymentInfo.TenantName = memberId;
                paymentInfo.State = 1;
                paymentInfo.PaymentType = ThirdPartyType.CPCNConfigSign;
                paymentInfo.PaymentAccount = retSubNo;
                paymentInfo.CreatedBy = userName;
                openAccountDTO.PaymentInfo = paymentInfo;

                var saveResult = PaymentService.SaveOpenAccount(openAccountDTO);
                if (!saveResult)
                {
                    returnModel.Success = false;
                    if (isSuccess)
                    {
                        Logger.LogError(string.Format("中金支付交易成功，但是平台处理失败：往平台推送开户信息数据出错。平台流水号：{0}，中金流水号：{1}。请求的XML:{2}，返回的XML{3}", ptnSrl, retSrl, postXML, returnXML));
                    }
                    returnModel.ErrorMessage = "开通账户失败！";
                }

                return Json(returnModel);
            }
            catch (Exception ex)
            {
                Logger.LogError(memberId + "开通失败" + ex.Message + string.Format("请求的XML:{0}，返回的XML{1}", postXML, returnXML));
                returnModel.Success = false;
                returnModel.ErrorMessage = "开通失败，请联系客服！";
                return Json(returnModel);
            }
        }


        /// <summary>
        /// 企业-企业账户认证(打款认证)-申请[T1131]
        /// </summary>
        /// <param name="paramDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult BankAuthenticationAppliction(BankAuthenticationApplicationDTO paramDTO)
        {
            string memberId = paramDTO.MemberId;
            string userName = paramDTO.UserName;
            int bankAccountId = paramDTO.BankAccountId;
            string postXML = string.Empty;
            string returnXML = string.Empty;

            PaymentReturnModel returnModel = new PaymentReturnModel();
            try
            {
                //获取中金支付的配置
                CPCNConfigDTO configModel = new CPCNConfigDTO();
                returnModel = GetCPCNConfig(configModel);
                if (!returnModel.Success)
                {
                    return Json(returnModel);
                }

                //先查看是否开通
                var isOpenPayment = PaymentService.IsOpenPayment(memberId, ThirdPartyType.CPCNConfigSign);
                if (!isOpenPayment)
                {
                    returnModel.Success = false;
                    returnModel.ErrorMessage = "未开通财富共赢钱包！";
                    return Json(returnModel);
                }

                //流水号唯一
                var ptnSrl = OtherUtilHelper.GetSerialNumber("ZJ");
                //获取银行卡账号信息
                var bankAccountDTO = PaymentService.GetBankAccountById(paramDTO.BankAccountId, memberId);
                if (bankAccountDTO == null || bankAccountDTO.Id == 0)
                {
                    returnModel.Success = false;
                    returnModel.ErrorMessage = "未找到银行卡信息！";
                    return Json(returnModel);
                }

                KC.WebApi.Pay.Models.AccountTransaction.BankAuthenticationApplication.MSG msg = new KC.WebApi.Pay.Models.AccountTransaction.BankAuthenticationApplication.MSG();
                //公共信息
                msg.MSGHD = GetMessageHead(configModel, CPCNInterfaceConstants.BankAuthenticationApplication);

                //流水号
                Srl srl = new Srl();
                srl.PtnSrl = ptnSrl;
                msg.Srl = srl;

                BkAcc bkAcc = new BkAcc();
                bkAcc.BkId = bankAccountDTO.BankId;
                bkAcc.AccNm = bankAccountDTO.AccountName;
                bkAcc.AccNo = bankAccountDTO.AccountNum;
                msg.BkAcc = bkAcc;

                postXML = CPCNPayment.ModelToXml(msg, configModel.Version);
                returnXML = PostCPCNData(configModel, CPCNInterfaceConstants.BankAuthenticationApplication, postXML);

                var cpcnRetModel = CPCNPayment.XmlToModel<KC.WebApi.Pay.Models.AccountTransaction.BankAuthenticationApplication.ReturnResponse.MSG>(returnXML);
                //中金返回的流水号
                string retSrl = "";
                var retCode = cpcnRetModel.MSGHD.RspCode;
                var retMessage = cpcnRetModel.MSGHD.RspMsg;
                //交易是否成功
                bool isSuccess = false;


                //返回成功的处理
                if (retCode.Equals(CPCNReturnCodeConstants.Success))
                {
                    isSuccess = true;
                    bankAccountDTO.BankState = BankAccountState.Authenticating;
                    retSrl = cpcnRetModel.Srl.PlatSrl;
                }
                else
                {
                    returnModel.Success = false;
                    returnModel.ErrorCode = retCode;
                    returnModel.ErrorMessage = string.Format("交易失败，失败原因：{0}", retMessage);
                    Logger.LogError(string.Format("中金支付交易失败，失败原因：{0}，平台流水号：{1}。请求的XML:{2}，返回的XML{3}", retMessage, ptnSrl, postXML, returnXML));
                }

                //交易流水
                var tradeRecord = AddPaymentTradeRecord(postXML, returnXML, memberId, ptnSrl, retSrl, CPCNInterfaceConstants.BankAuthenticationApplication, PaymentOperationType.AuthenticationBankApplication, isSuccess, retMessage, retCode, ThirdPartyType.CPCNConfigSign, userName);
                tradeRecord.ReferenceId = bankAccountDTO.Id;
                paramDTO.IsSuccess = isSuccess;
                paramDTO.PaymentTradeRecord = tradeRecord;
                paramDTO.BankAccount = bankAccountDTO;

                var saveResult = PaymentService.SaveBankAuthenticationAppliction(paramDTO);
                if (!saveResult)
                {
                    returnModel.Success = false;
                    if (isSuccess)
                    {
                        Logger.LogError(string.Format("中金支付交易成功，但是平台处理失败：往平台推送开户信息数据出错。平台流水号：{0}，中金流水号：{1}。请求的XML:{2}，返回的XML{3}", ptnSrl, retSrl, postXML, returnXML));
                    }
                    returnModel.ErrorMessage = "银行认证申请失败！";
                }

                return Json(returnModel);
            }
            catch (Exception ex)
            {
                Logger.LogError(memberId + "银行认证申请失败" + ex.Message + string.Format("请求的XML:{0}，返回的XML{1}", postXML, returnXML));
                returnModel.Success = false;
                returnModel.ErrorMessage = "银行认证申请失败，请联系客服！";
                return Json(returnModel);
            }
        }

        /// <summary>
        ///  企业-企业账户认证(打款认证)-验证[T1132]
        /// </summary>
        /// <param name="paramDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult BankAuthentication(BankAuthenticationDTO paramDTO)
        {
            string memberId = paramDTO.MemberId;
            string userName = paramDTO.UserName;
            decimal amount = paramDTO.Amount; //单位为分
            string postXML = string.Empty;
            string returnXML = string.Empty;

            PaymentReturnModel returnModel = new PaymentReturnModel();
            try
            {
                //获取中金支付的配置
                CPCNConfigDTO configModel = new CPCNConfigDTO();
                returnModel = GetCPCNConfig(configModel);
                if (!returnModel.Success)
                {
                    return Json(returnModel);
                }

                //T1131的流水号唯一
                var ptnSrl = "";

                var bankAccountResult = PaymentService.GetPaymentBankAccountByMemberId(memberId);
                if (bankAccountResult == null)
                {
                    returnModel.Success = false;
                    returnModel.ErrorMessage = "未开通财富共赢钱包！";
                    return Json(returnModel);
                }
                //获取银行卡信息
                var bankAccountDTO = PaymentService.GetBankAccountById(paramDTO.BankAccountId, memberId);
                if (bankAccountDTO == null || bankAccountDTO.Id == 0)
                {
                    returnModel.Success = false;
                    returnModel.ErrorMessage = "未找到银行卡信息！";
                    return Json(returnModel);
                }
                //获取T1131的流水号
                var tradeRecord = PaymentService.GetTradeRecordsByLastSuccessRecord(memberId, PaymentOperationType.AuthenticationBankApplication, bankAccountDTO.Id);
                if (tradeRecord != null)
                {
                    ptnSrl = tradeRecord.SrlNo;
                }

                else
                {
                    returnModel.Success = false;
                    returnModel.ErrorMessage = "获取流水号失败！";
                    return Json(returnModel);
                }

                KC.WebApi.Pay.Models.AccountTransaction.BankAuthentication.MSG msg = new KC.WebApi.Pay.Models.AccountTransaction.BankAuthentication.MSG();
                //公共信息
                msg.MSGHD = GetMessageHead(configModel, CPCNInterfaceConstants.BankAuthentication);

                //流水号
                Srl srl = new Srl();
                srl.PtnSrl = ptnSrl;
                msg.Srl = srl;

                msg.Amount = amount;

                postXML = CPCNPayment.ModelToXml(msg, configModel.Version);

                returnXML = PostCPCNData(configModel, CPCNInterfaceConstants.BankAuthentication, postXML);

                var cpcnRetModel = CPCNPayment.XmlToModel<KC.WebApi.Pay.Models.AccountTransaction.BankAuthentication.ReturnResponse.MSG>(returnXML);
                //中金返回的流水号
                string retSrl = "";
                var retCode = cpcnRetModel.MSGHD.RspCode;
                var retMessage = cpcnRetModel.MSGHD.RspMsg;
                //交易是否成功
                bool isSuccess = false;
                BankAuthenticationApplicationDTO bankAuthenticationApplicationDTO = new BankAuthenticationApplicationDTO();
                //返回成功的处理
                if (retCode.Equals(CPCNReturnCodeConstants.Success))
                {

                    var state = cpcnRetModel.Stat;
                    if (state == 10)
                    {
                        isSuccess = true;
                        //更新专账户的状态
                        bankAccountDTO.BankState = BankAccountState.AuthenticateSuccess;
                    }
                    else
                    {
                        returnModel.Success = false;
                        Logger.LogError(string.Format("输入错误！请重新输入，剩余验证次数{0}次.请求的XML:{1}，返回的XML{2}", cpcnRetModel.AvailableVeriCount, postXML, returnXML));
                        returnModel.ErrorMessage = string.Format("输入错误！请重新输入，剩余验证次数{0}次.", cpcnRetModel.AvailableVeriCount);
                        if (cpcnRetModel.AvailableVeriCount <= 0)
                        {
                            bankAccountDTO.BankState = BankAccountState.AuthenticateFailed;
                        }
                    }

                }
                var newPtnStl = OtherUtilHelper.GetSerialNumber("ZJ");
                //交易流水
                var tradeRecordModel = AddPaymentTradeRecord(postXML, returnXML, memberId, newPtnStl, retSrl, CPCNInterfaceConstants.BankAuthenticationApplication,
                    PaymentOperationType.AuthenticationBankAccount, isSuccess, retMessage, retCode, ThirdPartyType.CPCNConfigSign, userName);
                tradeRecord.ReferenceId = bankAccountDTO.Id;

                bankAuthenticationApplicationDTO.IsSuccess = isSuccess;
                bankAuthenticationApplicationDTO.PaymentTradeRecord = tradeRecordModel;
                bankAuthenticationApplicationDTO.BankAccount = bankAccountDTO;

                var saveResult = PaymentService.SaveBankAuthenticationAppliction(bankAuthenticationApplicationDTO);
                if (!saveResult)
                {
                    returnModel.Success = false;
                    if (isSuccess)
                    {
                        Logger.LogError(string.Format("中金支付交易成功，但是平台处理失败：往平台推送开户信息数据出错。平台流水号：{0}，中金流水号：{1}。请求的XML:{2}，返回的XML{3}", ptnSrl, retSrl, postXML, returnXML));
                    }
                    returnModel.ErrorMessage = "银行认证失败！";
                }

                return Json(returnModel);
            }
            catch (Exception ex)
            {
                Logger.LogError(memberId + "银行认证失败" + ex.Message + string.Format("请求的XML:{0}，返回的XML{1}", postXML, returnXML));
                returnModel.Success = false;
                returnModel.ErrorMessage = "银行认证失败，请联系客服！";
                return Json(returnModel);
            }
        }

        /// <summary>
        ///  绑定/解绑银行卡
        /// </summary>
        /// <param name="paramDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult BindBankAccount(BindBankAccountDTO paramDTO)
        {
            string memberId = paramDTO.MemberId;
            string userName = paramDTO.UserName;
            int bankId = paramDTO.BankId;
            int bindState = paramDTO.BindState;
            string postXML = string.Empty;
            string returnXML = string.Empty;

            PaymentReturnModel returnModel = new PaymentReturnModel();

            try
            {
                //获取中金支付的配置
                CPCNConfigDTO configModel = new CPCNConfigDTO();
                returnModel = GetCPCNConfig(configModel);
                if (!returnModel.Success)
                {
                    return Json(returnModel);
                }
                //流水号唯一
                var ptnSrl = OtherUtilHelper.GetSerialNumber("ZJ");

                //中金支付账号信息
                var paymentBankAccount = PaymentService.GetPaymentBankAccountByMemberId(memberId);
                if (paymentBankAccount == null)
                {
                    returnModel.Success = false;
                    returnModel.ErrorMessage = "未开通财富共赢钱包！";
                    return Json(returnModel);
                }


                //银行账号信息
                var bankAccount = PaymentService.GetBankAccountById(bankId, memberId);
                if (bankAccount == null)
                {
                    returnModel.Success = false;
                    returnModel.ErrorMessage = "选择的银行账号不存在！";
                    return Json(returnModel);
                }

                KC.WebApi.Pay.Models.AccountTransaction.SettlementAccount.MSG msg = new KC.WebApi.Pay.Models.AccountTransaction.SettlementAccount.MSG();
                //公共信息
                msg.MSGHD = GetMessageHead(configModel, CPCNInterfaceConstants.SettlementAccount);

                //流水号
                Srl srl = new Srl();
                srl.PtnSrl = ptnSrl;
                msg.Srl = srl;

                //账户信息
                CltAcc cltAcc = new CltAcc();
                cltAcc.SubNo = paymentBankAccount.AccountNo;
                cltAcc.CltNm = paymentBankAccount.AccountName;
                cltAcc.CltNo = memberId;
                msg.CltAcc = cltAcc;

                //绑定状态为绑定/变更信息时 需填写结算账户信息
                if (bindState == 1)
                {
                    //结算账户信息
                    BkAcc bkAcc = new BkAcc();

                    bkAcc.AccNm = bankAccount.AccountName;
                    bkAcc.AccNo = bankAccount.AccountNum;
                    bkAcc.AccTp = bankAccount.AccountType;
                    bkAcc.BkId = bankAccount.BankId;
                    bkAcc.CrdTp = bankAccount.BankAccountType;
                    bkAcc.CdTp = bankAccount.CardType;
                    bkAcc.CdNo = bankAccount.CardNumber;
                    bkAcc.CrsMk = bankAccount.CrossMark;
                    bkAcc.OpenBkCd = bankAccount.OpenBankCode;
                    bkAcc.OpenBkNm = bankAccount.OpenBankName;
                    bkAcc.PrcCd = bankAccount.ProvinceCode;
                    bkAcc.PrcNm = bankAccount.ProvinceName;
                    bkAcc.CityCd = bankAccount.CityCode;
                    bkAcc.CityNm = bankAccount.CityName;

                    msg.BkAcc = bkAcc;
                }

                msg.FcFlg = bindState;

                postXML = CPCNPayment.ModelToXml(msg, configModel.Version);
                returnXML = PostCPCNData(configModel, CPCNInterfaceConstants.SettlementAccount, postXML);

                var cpcnRetModel = CPCNPayment.XmlToModel<KC.WebApi.Pay.Models.AccountTransaction.SettlementAccount.ReturnResponse.MSG>(returnXML);
                //中金返回的流水号
                string retSrl = "";
                var retCode = cpcnRetModel.MSGHD.RspCode;
                var retMessage = cpcnRetModel.MSGHD.RspMsg;
                //交易是否成功
                bool isSuccess = false;
                BindBankAccountDTO bindBankAccountDTO = new BindBankAccountDTO();

                //返回成功的处理
                if (retCode.Equals(CPCNReturnCodeConstants.Success))
                {
                    isSuccess = true;
                    if (bindState == 1)
                    {
                        bankAccount.BankState = BankAccountState.Binding;
                        paymentBankAccount.State = 3;
                        paymentBankAccount.BindBankAccountId = bankAccount.Id;
                        paymentBankAccount.BindBankAccount = bankAccount.AccountNum;
                        paymentBankAccount.BindBankAccountName = bankAccount.AccountName;
                        paymentBankAccount.BindBankId = bankAccount.BankId;
                    }
                    else if (bindState == 3)
                    {
                        //删除绑定处理
                        bankAccount.BankState = BankAccountState.AuthenticateSuccess;
                        paymentBankAccount.State = 4;
                        paymentBankAccount.BindBankAccountId = 0;
                        paymentBankAccount.BindBankAccount = "";
                        paymentBankAccount.BindBankAccountName = "";
                        paymentBankAccount.BindBankId = "";
                    }

                    retSrl = cpcnRetModel.Srl.PlatSrl;
                }
                else
                {
                    returnModel.Success = false;
                    returnModel.ErrorCode = retCode;
                    returnModel.ErrorMessage = retMessage;
                }

                var paymentOperationType = bindState == 1 ? PaymentOperationType.SettlementBinding : PaymentOperationType.SettlementUnbinding;
                //交易流水
                var tradeRecordModel = AddPaymentTradeRecord(postXML, returnXML, memberId, ptnSrl, retSrl, CPCNInterfaceConstants.SettlementAccount,
                    PaymentOperationType.SettlementBinding, isSuccess, retMessage, retCode, ThirdPartyType.CPCNConfigSign, userName);
                tradeRecordModel.ReferenceId = paymentBankAccount.Id;

                bindBankAccountDTO.IsSuccess = isSuccess;
                bindBankAccountDTO.PaymentTradeRecord = tradeRecordModel;
                bindBankAccountDTO.BankAccount = bankAccount;
                bindBankAccountDTO.PaymentBankAccount = paymentBankAccount;

                var saveResult = PaymentService.BindBankAccount(bindBankAccountDTO);
                if (!saveResult)
                {
                    returnModel.Success = false;
                    if (isSuccess)
                    {
                        Logger.LogError(string.Format("中金支付交易成功，但是平台处理失败：往平台推送开户信息数据出错。平台流水号：{0}，中金流水号：{1}。请求的XML:{2}，返回的XML{3}", ptnSrl, retSrl, postXML, returnXML));
                    }
                    returnModel.ErrorMessage = "绑定/解绑银行卡失败！";
                }

                return Json(returnModel);
            }
            catch (Exception ex)
            {
                Logger.LogError(memberId + "绑定/解绑银行卡失败" + ex.Message + string.Format("请求的XML:{0}，返回的XML{1}", postXML, returnXML));
                returnModel.Success = false;
                returnModel.ErrorMessage = "绑定/解绑银行卡失败，请联系客服！";
                return Json(returnModel);
            }
        }

        /// <summary>
        /// 支付行信息模糊查询[T1017]
        /// </summary>
        /// <param name="paramDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult PaymentBankInfo(BankInfoFilterDTO paramDTO)
        {
            string memberId = paramDTO.MemberId;
            string userName = paramDTO.UserName;

            PaymentReturnModel returnModel = new PaymentReturnModel();
            //获取中金支付的配置
            CPCNConfigDTO configModel = new CPCNConfigDTO();
            returnModel = GetCPCNConfig(configModel);
            if (!returnModel.Success)
            {
                return Json(returnModel);
            }

            KC.WebApi.Pay.Models.AccountTransaction.PaymentBankInfo.MSG msg = new KC.WebApi.Pay.Models.AccountTransaction.PaymentBankInfo.MSG();
            //公共信息
            msg.MSGHD = GetMessageHead(configModel, CPCNInterfaceConstants.PaymentBankInfo);

            //中金支付账号信息
            var cpcnBankAccount = PaymentService.GetPaymentBankAccountByMemberId(memberId);
            if (cpcnBankAccount == null)
            {
                returnModel.Success = false;
                returnModel.ErrorMessage = "未开通财富共赢钱包！";
                return Json(returnModel);
            }


            msg.QryFlag = paramDTO.QryFlag;
            msg.BkId = paramDTO.BankId;
            msg.OpenBkCd = paramDTO.OpenBankCode;
            msg.OpenBkNm = paramDTO.OpenBankName;
            msg.CityCd = paramDTO.CityCode;
            msg.QueryNum = paramDTO.QueryNum;

            string xmlStr = CPCNPayment.ModelToXml(msg, configModel.Version);
            string retStr = PostCPCNData(configModel, CPCNInterfaceConstants.PaymentBankInfo, xmlStr);
            //返回的xml没有对数据外加一个父节点，需要对字符串处理。
            retStr = retStr.Replace("</MSGHD>", "</MSGHD><PayBnks>");
            retStr = retStr.Replace("</MSG>", "</PayBnks></MSG>");
            var cpcnRetModel = CPCNPayment.XmlToModel<KC.WebApi.Pay.Models.AccountTransaction.PaymentBankInfo.ReturnResponse.MSG>(retStr);
            var retCode = cpcnRetModel.MSGHD.RspCode;
            var retMessage = cpcnRetModel.MSGHD.RspMsg;

            //返回成功的处理
            if (retCode.Equals(CPCNReturnCodeConstants.Success))
            {
                returnModel.ReturnData = cpcnRetModel.PayBnks;
            }
            else
            {
                returnModel.Success = false;
                returnModel.ErrorCode = retCode;
                returnModel.ErrorMessage = retMessage;
            }

            return Json(returnModel);
        }

        /// <summary>
        /// 查询可T0/T1资金
        /// 此交易主要是配合 T2004、 T2009 交易使用。
        /// 通过该交易告知合作方 T2004、 T2009 交易中 T0/T1 出金时允许的出金额度情况。
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SearchWithdrawAmt(PayBaseParamDTO paramDTO)
        {
            var memberId = paramDTO.MemberId;
            bool isSuccess = false;
            PaymentReturnModel returnModel = new PaymentReturnModel();
            //获取中金支付的配置
            CPCNConfigDTO configModel = new CPCNConfigDTO();
            returnModel = GetCPCNConfig(configModel);
            if (!returnModel.Success)
            {
                return Json(returnModel);
            }

            KC.WebApi.Pay.Models.AccountTransaction.WithdrawAmt.MSG msg = new KC.WebApi.Pay.Models.AccountTransaction.WithdrawAmt.MSG();
            //公共信息
            msg.MSGHD = GetMessageHead(configModel, CPCNInterfaceConstants.WithdrawAmt);

            //中金支付账号信息
            var cpcnBankAccount = PaymentService.GetPaymentBankAccountByMemberId(memberId);
            if (cpcnBankAccount == null)
            {
                returnModel.Success = false;
                returnModel.ErrorMessage = "未开通财富共赢钱包！";
                return Json(returnModel);
            }

            CltAcc cltAcc = new CltAcc();
            cltAcc.SubNo = cpcnBankAccount.AccountNo;
            cltAcc.CltNm = cpcnBankAccount.AccountName;
            msg.CltAcc = cltAcc;

            string xmlStr = CPCNPayment.ModelToXml(msg, configModel.Version);
            string retStr = PostCPCNData(configModel, CPCNInterfaceConstants.WithdrawAmt, xmlStr);

            var cpcnRetModel = CPCNPayment.XmlToModel<KC.WebApi.Pay.Models.AccountTransaction.WithdrawAmt.ReturnResponse.MSG>(retStr);

            var retCode = cpcnRetModel.MSGHD.RspCode;
            var retMessage = cpcnRetModel.MSGHD.RspMsg;

            //返回成功的处理
            if (retCode.Equals(CPCNReturnCodeConstants.Success))
            {
                isSuccess = true;
                returnModel.Success = true;
                ReturnAmtDTO amt = new ReturnAmtDTO();
                amt.BalanceAmt = cpcnRetModel.AcsAmt.BalAmt;
                amt.FreezeAmt = cpcnRetModel.AcsAmt.FrzAmt;
                amt.UseAmt = cpcnRetModel.AcsAmt.UseAmt;
                amt.T1CtAmtA00 = cpcnRetModel.T1Amt.CtAmtA00;
                amt.T0CtAmtA00 = cpcnRetModel.T0Amt.CtAmtA00;

                returnModel.ReturnAmtData = amt;
            }
            else
            {
                returnModel.Success = false;
                returnModel.ErrorCode = retCode;
                returnModel.ErrorMessage = retMessage;
            }

            //交易流水
            var tradeRecord = AddPaymentTradeRecord(xmlStr, retStr, memberId, "", "", CPCNInterfaceConstants.WithdrawAmt,
                PaymentOperationType.WithdrawAmt, isSuccess, retMessage, retCode, ThirdPartyType.BoHaiConfigSign, paramDTO.UserName);
            PaymentService.SavePaymentTradeRecord(tradeRecord);


            return Json(returnModel);
        }

        /// <summary>
        /// 资金账户余额查询
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SearchAccountBalance(PayBaseParamDTO paramDTO)
        {
            PaymentReturnModel returnModel = new PaymentReturnModel();
            //中金支付账号信息
            var cpcnBankAccount = PaymentService.GetPaymentBankAccountByMemberId(paramDTO.MemberId);
            if (cpcnBankAccount == null)
            {
                returnModel.Success = false;
                returnModel.ErrorMessage = "未开通财富共赢钱包！";
                return Json(returnModel);
            }
            returnModel = GetSearchAccountBanlace(paramDTO, cpcnBankAccount);
            if (returnModel.Success)
            {
                var amt = returnModel.ReturnAmtData;
                //更新冻结资金和总金额
                cpcnBankAccount.FreezeAmount = amt.FreezeAmt;
                cpcnBankAccount.TotalAmount = amt.BalanceAmt;
                cpcnBankAccount.AmountUpdateTime = DateTime.Now;
                if (!PaymentService.SavePaymentBankAccount(cpcnBankAccount))
                {
                    returnModel.Success = false;
                    returnModel.ErrorMessage = "更新余额失败！";
                    return Json(returnModel);
                }
            }
            return Json(returnModel);
        }

        /// <summary>
        /// 资金账户余额查询
        /// </summary>
        /// <param name="paramDTO"></param>
        /// <returns></returns>
        private PaymentReturnModel GetSearchAccountBanlace(PayBaseParamDTO paramDTO, PaymentBankAccountDTO cpcnBankAccount)
        {
            var memberId = paramDTO.MemberId;
            PaymentReturnModel returnModel = new PaymentReturnModel();
            string postXML = string.Empty;
            string returnXML = string.Empty;
            string userName = paramDTO.UserName;
            try
            {
                //获取中金支付的配置
                CPCNConfigDTO configModel = new CPCNConfigDTO();
                returnModel = GetCPCNConfig(configModel);
                if (!returnModel.Success)
                {
                    return returnModel;
                }

                KC.WebApi.Pay.Models.AccountTransaction.FundAccountBalance.MSG msg = new KC.WebApi.Pay.Models.AccountTransaction.FundAccountBalance.MSG();
                //公共信息
                msg.MSGHD = GetMessageHead(configModel, CPCNInterfaceConstants.FundAccountBalance);

                CltAcc cltAcc = new CltAcc();
                cltAcc.SubNo = cpcnBankAccount.AccountNo;
                cltAcc.CltNm = cpcnBankAccount.AccountName;
                msg.CltAcc = cltAcc;

                postXML = CPCNPayment.ModelToXml(msg, configModel.Version);
                returnXML = PostCPCNData(configModel, CPCNInterfaceConstants.FundAccountBalance, postXML);

                var cpcnRetModel = CPCNPayment.XmlToModel<KC.WebApi.Pay.Models.AccountTransaction.FundAccountBalance.ReturnResponse.MSG>(returnXML);

                var retCode = cpcnRetModel.MSGHD.RspCode;
                var retMessage = cpcnRetModel.MSGHD.RspMsg;

                //返回成功的处理
                if (retCode.Equals(CPCNReturnCodeConstants.Success))
                {
                    ReturnAmtDTO amt = new ReturnAmtDTO();
                    amt.BalanceAmt = cpcnRetModel.Amt.BalAmt;
                    amt.FreezeAmt = cpcnRetModel.Amt.FrzAmt;
                    amt.UseAmt = cpcnRetModel.Amt.UseAmt;
                    returnModel.ReturnAmtData = amt;
                }
                else
                {
                    returnModel.Success = false;
                    returnModel.ErrorCode = retCode;
                    returnModel.ErrorMessage = retMessage;
                }
                var PaymentTradeRecord = AddPaymentTradeRecord(postXML, returnXML, memberId, "", "", CPCNInterfaceConstants.FundAccountBalance,
                    PaymentOperationType.FundAccountBalance, returnModel.Success, retMessage, retCode, ThirdPartyType.CPCNConfigSign, userName);
                PaymentService.SavePaymentTradeRecord(PaymentTradeRecord);
                return returnModel;
            }
            catch (Exception ex)
            {
                Logger.LogError(memberId + "查询账号金额失败,系统出错！" + ex.Message + string.Format("请求的XML:{0}，返回的XML{1}", postXML, returnXML));
                returnModel.Success = false;
                returnModel.ErrorMessage = "查询账号金额失败！";
                return returnModel;
            }
        }

        /// <summary>
        /// 冻结/解冻
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult FreezeAmt(FreezeAmtDTO paramDTO)
        {
            return Json(FreezeAmount(paramDTO));
        }

        /// <summary>
        /// 冻结/解冻方法
        /// </summary>
        /// <param name="paramDTO"></param>
        /// <returns></returns>
        private PaymentReturnModel FreezeAmount(FreezeAmtDTO paramDTO)
        {
            var memberId = paramDTO.MemberId;
            var freezeAmt = paramDTO.FreezeAmt;
            var usage = paramDTO.Usage;
            var busType = paramDTO.BusiType;
            var userName = paramDTO.UserName;
            string postXML = string.Empty;
            string returnXML = string.Empty;

            PaymentReturnModel returnModel = new PaymentReturnModel();
            try
            {
                //获取中金支付的配置
                CPCNConfigDTO configModel = new CPCNConfigDTO();
                returnModel = GetCPCNConfig(configModel);
                if (!returnModel.Success)
                {
                    return returnModel;
                }
                //流水号唯一
                var ptnSrl = OtherUtilHelper.GetSerialNumber("ZJ");

                KC.WebApi.Pay.Models.PayTransaction.FreezeAmt.MSG msg = new KC.WebApi.Pay.Models.PayTransaction.FreezeAmt.MSG();
                //公共信息
                msg.MSGHD = GetMessageHead(configModel, CPCNInterfaceConstants.FreezeAmt);

                //中金支付账号信息
                var cpcnBankAccount = PaymentService.GetPaymentBankAccountByMemberId(memberId);
                if (cpcnBankAccount == null)
                {
                    returnModel.Success = false;
                    returnModel.ErrorMessage = "未开通财富共赢钱包！";
                    return returnModel;
                }
                //加锁
                new RedisDistributedLock().DoDistributedLock("CPCNPayController" + memberId, () =>
                {
                    msg.Amt = new KC.WebApi.Pay.Models.PayTransaction.FreezeAmt.Amt
                    {
                        AclAmt = freezeAmt,
                        CcyCd = "CNY"
                    };
                    msg.CltAcc = new CltAcc
                    {
                        SubNo = cpcnBankAccount.AccountNo,
                        CltNm = cpcnBankAccount.AccountName
                    };
                    msg.TrsFlag = busType == 1 ? "A00" : busType == 2 ? "B00" : "";
                    msg.Usage = usage == null ? "" : usage.Length > 24 ? usage.Substring(0, 24) : usage;
                    msg.Srl = new Srl { PtnSrl = ptnSrl };

                    postXML = CPCNPayment.ModelToXml(msg, configModel.Version);
                    returnXML = PostCPCNData(configModel, CPCNInterfaceConstants.FreezeAmt, postXML);//解冻资金申请提交到中金

                    var cpcnRetModel = CPCNPayment.XmlToModel<KC.WebApi.Pay.Models.PayTransaction.FreezeAmt.ReturnResponse.MSG>(returnXML);

                    var retCode = cpcnRetModel.MSGHD.RspCode;
                    var retMessage = cpcnRetModel.MSGHD.RspMsg;

                    bool isSuccess = false;
                    string retSrl = string.Empty;

                    //返回成功的处理
                    if (retCode.Equals(CPCNReturnCodeConstants.Success))
                    {
                        retSrl = cpcnRetModel.Srl.PlatSrl;
                        //查询中金余额
                        var searchReturnModel = GetSearchAccountBanlace(paramDTO, cpcnBankAccount);
                        if (searchReturnModel.Success)
                        {
                            isSuccess = true;
                            //获取本地存储的资金信息
                            cpcnBankAccount = PaymentService.GetPaymentBankAccountByMemberId(memberId);
                            var returnAmtDTO = searchReturnModel.ReturnAmtData;
                            cpcnBankAccount.TotalAmount = returnAmtDTO.BalanceAmt;
                            cpcnBankAccount.FreezeAmount = returnAmtDTO.FreezeAmt;
                            if (busType == 1)//冻结
                            {
                                cpcnBankAccount.CFWinFreezeAmount += freezeAmt;
                            }
                            else//解冻
                            {
                                cpcnBankAccount.CFWinFreezeAmount -= freezeAmt;
                            }
                            paramDTO.PaymentBankAccount = cpcnBankAccount;
                        }
                    }
                    else
                    {
                        returnModel.Success = false;
                        returnModel.ErrorCode = retCode;
                        returnModel.ErrorMessage = string.Format("交易失败，失败原因：{0}", retMessage);
                        Logger.LogError(string.Format("中金支付交易失败，失败原因：{0}，平台流水号：{1}。请求的XML:{2}，返回的XML{3}", retMessage, ptnSrl, postXML, returnXML));
                    }

                    var paymentOperationType = busType == 1 ? PaymentOperationType.FreezeAmt : PaymentOperationType.UnFreezeAmt;
                    var orderNo = busType == 1 ? "冻结" : "解冻";

                    //交易流水
                    var tradeRecord = AddPaymentTradeRecord(postXML, returnXML, memberId, ptnSrl, retSrl, CPCNInterfaceConstants.FreezeAmt,
                        paymentOperationType, isSuccess, retMessage, retCode, ThirdPartyType.CPCNConfigSign, userName);

                    var nowString = DateTime.Now.ToString("yyyyMMddhhmmss");
                    var onlinePayment = AddOnlinePaymentRecords(paramDTO.PaymentOrderId, orderNo, paramDTO.FreezeAmt, paramDTO.FreezeAmt,
                        nowString, nowString, (isSuccess ? "1" : "0"), retCode, ptnSrl, configModel.ConfigId, memberId, "中金支付", ThirdPartyType.CPCNConfigSign);

                    paramDTO.IsSuccess = isSuccess;
                    paramDTO.PaymentTradeRecord = tradeRecord;
                    paramDTO.OnlinePaymentRecord = onlinePayment;
                    bool saveResult = PaymentService.SaveFreezeAmt(paramDTO);
                    if (!saveResult)
                    {
                        returnModel.Success = false;
                        if (isSuccess)
                        {
                            Logger.LogError(string.Format("中金支付交易成功，但是平台处理失败：往平台推送开户信息数据出错。平台流水号：{0}，中金流水号：{1}。请求的XML:{2}，返回的XML{3}", ptnSrl, retSrl, postXML, returnXML));
                        }
                        returnModel.ErrorMessage = "操作失败！";
                    }

                });
                return returnModel;
            }
            catch (Exception ex)
            {
                Logger.LogError(memberId + "冻结/解冻失败,系统出错！" + ex.Message + string.Format("请求的XML:{0}，返回的XML{1}", postXML, returnXML));
                returnModel.Success = false;
                returnModel.ErrorMessage = "冻结/解冻失败！";
                return returnModel;
            }
        }


        /// <summary>
        /// 订单支付
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult OrderPay(OrderPayDTO paramDTO)
        {
            return Json(OrderPayment(paramDTO));
        }

        private PaymentReturnModel OrderPayment(OrderPayDTO paramDTO)
        {
            var memberId = paramDTO.MemberId;
            var userName = paramDTO.UserName;
            string postXML = string.Empty;
            string returnXML = string.Empty;

            PaymentReturnModel returnModel = new PaymentReturnModel();
            try
            {
                //获取中金支付的配置
                CPCNConfigDTO configModel = new CPCNConfigDTO();
                returnModel = GetCPCNConfig(configModel);
                if (!returnModel.Success)
                {
                    return returnModel;
                }
                //流水号唯一
                var ptnSrl = OtherUtilHelper.GetSerialNumber("ZJ");

                KC.WebApi.Pay.Models.PayTransaction.OrderPay.MSG msg = new KC.WebApi.Pay.Models.PayTransaction.OrderPay.MSG();
                //公共信息
                msg.MSGHD = GetMessageHead(configModel, CPCNInterfaceConstants.OrderPay);

                //中金支付账号信息
                var cpcnBankAccount = PaymentService.GetPaymentBankAccountByMemberId(memberId);

                if (cpcnBankAccount == null)
                {
                    returnModel.Success = false;
                    returnModel.ErrorMessage = "未开通财富共赢钱包！";
                    return returnModel;
                }
                //获取收款方账号信息
                var peeBankAccount = PaymentService.GetPaymentBankAccountByAccountNum(paramDTO.PayeeAccountNumber);
                if (peeBankAccount == null)
                {
                    returnModel.Success = false;
                    returnModel.ErrorMessage = "收款方账户不存在！";
                    return returnModel;
                }
                //加锁
                new RedisDistributedLock().DoDistributedLock("CPCNPayController" + memberId, () =>
                {
                    KC.WebApi.Pay.Models.PayTransaction.OrderPay.billInfo billInfo = new KC.WebApi.Pay.Models.PayTransaction.OrderPay.billInfo();

                    billInfo.PSubNo = cpcnBankAccount.AccountNo;
                    billInfo.PNm = cpcnBankAccount.AccountName;

                    billInfo.RSubNo = paramDTO.PayeeAccountNumber;
                    billInfo.RCltNm = paramDTO.PayeeAccountName;

                    billInfo.OrderNo = paramDTO.OrderNo;
                    billInfo.BillNo = paramDTO.PaymentOrderId;

                    billInfo.AclAmt = paramDTO.Amount;
                    billInfo.PayFee = 0;
                    billInfo.PayeeFee = 0;
                    billInfo.CcyCd = "CNY";
                    billInfo.Usage = paramDTO.Usage == null ? "" : (paramDTO.Usage.Length > 24 ? paramDTO.Usage.Substring(0, 24) : paramDTO.Usage);

                    msg.billInfo = billInfo;
                    msg.TrsFlag = paramDTO.PayType == 1 ? "A00" : paramDTO.PayType == 2 ? "B00" : "";

                    postXML = CPCNPayment.ModelToXml(msg, configModel.Version);
                    returnXML = PostCPCNData(configModel, CPCNInterfaceConstants.OrderPay, postXML);

                    var cpcnRetModel = CPCNPayment.XmlToModel<KC.WebApi.Pay.Models.PayTransaction.OrderPay.ReturnResponse.MSG>(returnXML);

                    var retCode = cpcnRetModel.MSGHD.RspCode;
                    var retMessage = cpcnRetModel.MSGHD.RspMsg;

                    bool isSuccess = false;
                    string retSrl = string.Empty;

                    //返回成功的处理
                    if (retCode.Equals(CPCNReturnCodeConstants.Success))
                    {
                        retSrl = cpcnRetModel.Srl.PlatSrl;
                        //查询
                        var searchReturnModel = GetSearchAccountBanlace(paramDTO, cpcnBankAccount);
                        PayBaseParamDTO peeParamDTO = new PayBaseParamDTO();
                        peeParamDTO.MemberId = peeBankAccount.MemberId;
                        //查询收款方的资金信息
                        //var peeSearchReturnModel = GetSearchAccountBanlace(paramDTO, cpcnBankAccount);
                        var payeeReturnModel = GetSearchAccountBanlace(peeParamDTO, peeBankAccount);
                        if (searchReturnModel.Success && payeeReturnModel.Success)
                        {
                            isSuccess = true;

                            //更新金额
                            cpcnBankAccount = PaymentService.GetPaymentBankAccountByMemberId(memberId);
                            var returnAmtDTO = searchReturnModel.ReturnAmtData;
                            cpcnBankAccount.TotalAmount = returnAmtDTO.BalanceAmt;
                            cpcnBankAccount.FreezeAmount = returnAmtDTO.FreezeAmt;
                            cpcnBankAccount.CFWinTotalAmount = cpcnBankAccount.CFWinTotalAmount - paramDTO.Amount;
                            paramDTO.PaymentBankAccount = cpcnBankAccount;

                            //加锁
                            new RedisDistributedLock().DoDistributedLock("CPCNPayController" + peeBankAccount.MemberId, () =>
                            {
                                //更新收款人的金额
                                peeBankAccount = PaymentService.GetPaymentBankAccountByAccountNum(paramDTO.PayeeAccountNumber);
                                var peeReturnAmtDTO = payeeReturnModel.ReturnAmtData;
                                peeBankAccount.TotalAmount = peeReturnAmtDTO.BalanceAmt;
                                peeBankAccount.FreezeAmount = peeReturnAmtDTO.FreezeAmt;
                                peeBankAccount.CFWinTotalAmount = peeBankAccount.CFWinTotalAmount + paramDTO.Amount;
                                peeBankAccount.CFWinFreezeAmount = paramDTO.PayType == 2 ? peeBankAccount.CFWinFreezeAmount + paramDTO.Amount : peeBankAccount.CFWinFreezeAmount;
                                paramDTO.PeePaymentBankAccount = peeBankAccount;
                            });
                        }
                    }
                    else
                    {
                        returnModel.Success = false;
                        returnModel.ErrorCode = retCode;
                        returnModel.ErrorMessage = string.Format("交易失败，失败原因：{0}", retMessage);
                        Logger.LogError(string.Format("中金支付交易失败，失败原因：{0}，平台流水号：{1}。请求的XML:{2}，返回的XML{3}", retMessage, ptnSrl, postXML, returnXML));
                    }

                    //交易流水
                    var tradeRecord = AddPaymentTradeRecord(postXML, returnXML, memberId, ptnSrl, retSrl, CPCNInterfaceConstants.OrderPay, PaymentOperationType.OrderPay, isSuccess, retMessage, retCode, ThirdPartyType.CPCNConfigSign, userName);
                    var nowString = DateTime.Now.ToString("yyyyMMddhhmmss");
                    var onlinePayment = AddOnlinePaymentRecords(paramDTO.PaymentOrderId, paramDTO.OrderNo, paramDTO.Amount, paramDTO.Amount,
                       nowString, nowString, (isSuccess ? "1" : "0"), retCode, ptnSrl, configModel.ConfigId, memberId, "中金支付", ThirdPartyType.CPCNConfigSign);
                    onlinePayment.PeeMemberId = peeBankAccount.MemberId;

                    paramDTO.IsSuccess = isSuccess;
                    paramDTO.PaymentTradeRecord = tradeRecord;
                    paramDTO.OnlinePaymentRecord = onlinePayment;
                    bool saveResult = PaymentService.SaveOrderPay(paramDTO);

                    if (!saveResult)
                    {
                        returnModel.Success = false;
                        if (isSuccess)
                        {
                            Logger.LogError(string.Format("中金支付交易成功，但是平台处理失败：往平台推送开户信息数据出错。平台流水号：{0}，中金流水号：{1}。请求的XML:{2}，返回的XML{3}", ptnSrl, retSrl, postXML, returnXML));
                        }
                        returnModel.ErrorMessage = "操作失败！";
                    }
                });
                return returnModel;

            }
            catch (Exception ex)
            {
                Logger.LogError(memberId + "支付订单支付失败,系统出错！" + ex.Message + string.Format("请求的XML:{0}，返回的XML{1}", postXML, returnXML));
                returnModel.Success = false;
                returnModel.ErrorMessage = "支付订单支付失败！";
                return returnModel;
            }
        }

        /// <summary>
        /// 充值交易
        /// </summary>
        /// <param name="paramDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult InTransaction(InTransactionDTO paramDTO)
        {
            var memberId = paramDTO.MemberId;
            var userName = paramDTO.UserName;
            string postXML = string.Empty;
            string returnXML = string.Empty;

            PaymentReturnModel returnModel = new PaymentReturnModel();
            try
            {
                //获取中金支付的配置
                CPCNConfigDTO configModel = new CPCNConfigDTO();
                returnModel = GetCPCNConfig(configModel);
                if (!returnModel.Success)
                {
                    return Json(returnModel);
                }
                //流水号唯一
                var ptnSrl = OtherUtilHelper.GetSerialNumber("ZJ");

                KC.WebApi.Pay.Models.InAndOutTransaction.NetBankIn.MSG msg = new KC.WebApi.Pay.Models.InAndOutTransaction.NetBankIn.MSG();
                //公共信息
                msg.MSGHD = GetMessageHead(configModel, CPCNInterfaceConstants.InTransaction);

                //中金支付账号信息
                var cpcnBankAccount = PaymentService.GetPaymentBankAccountByMemberId(memberId);
                if (cpcnBankAccount == null)
                {
                    returnModel.Success = false;
                    returnModel.ErrorMessage = "未开通财富共赢钱包！";
                    return Json(returnModel);
                }

                CltAcc cltAcc = new CltAcc();
                cltAcc.CltNm = cpcnBankAccount.AccountName;
                cltAcc.SubNo = cpcnBankAccount.AccountNo;
                msg.CltAcc = cltAcc;

                KC.WebApi.Pay.Models.InAndOutTransaction.NetBankIn.Amt amt = new KC.WebApi.Pay.Models.InAndOutTransaction.NetBankIn.Amt();
                amt.AclAmt = paramDTO.Amount;
                amt.CcyCd = "CNY";
                msg.Amt = amt;

                msg.BankID = cpcnBankAccount.BindBankId;
                //msg.BankID = "700";//700是模拟网银
                msg.Usage = paramDTO.Usage == null ?
                    "" : (paramDTO.Usage.Length > 24 ? paramDTO.Usage.Substring(0, 24) : paramDTO.Usage);

                //流水号
                Srl srl = new Srl();
                srl.PtnSrl = ptnSrl;
                msg.Srl = srl;

                msg.PayAccType = 1;

                postXML = CPCNPayment.ModelToXml(msg, configModel.Version);
                returnXML = PostCPCNData(configModel, CPCNInterfaceConstants.InTransaction, postXML);

                var cpcnRetModel = CPCNPayment.XmlToModel<KC.WebApi.Pay.Models.InAndOutTransaction.NetBankIn.ReturnResponse.MSG>(returnXML);

                var retCode = cpcnRetModel.MSGHD.RspCode;
                var retMessage = cpcnRetModel.MSGHD.RspMsg;

                bool isSuccess = false;
                string retSrl = string.Empty;

                //返回成功的处理
                if (retCode.Equals(CPCNReturnCodeConstants.Success))
                {
                    isSuccess = true;
                    returnModel.ReturnData = cpcnRetModel.Url;
                    retSrl = cpcnRetModel.Srl.PlatSrl;
                }
                else
                {
                    returnModel.Success = false;
                    returnModel.ErrorCode = retCode;
                    returnModel.ErrorMessage = string.Format("交易失败，失败原因：{0}", retMessage);
                    Logger.LogError(string.Format("中金支付交易失败，失败原因：{0}，平台流水号：{1}。请求的XML:{2}，返回的XML{3}", retMessage, ptnSrl, postXML, returnXML));
                }

                //交易流水
                var tradeRecord = AddPaymentTradeRecord(postXML, returnXML, memberId, ptnSrl, retSrl, CPCNInterfaceConstants.InTransaction, PaymentOperationType.BankIn, isSuccess, retMessage, retCode, ThirdPartyType.CPCNConfigSign, userName);
                //插入OnlinePayment表
                var nowString = DateTime.Now.ToString("yyyyMMddhhmmss");
                var onlinePayment = AddOnlinePaymentRecords(paramDTO.PaymentOrderId, "入金", paramDTO.Amount, paramDTO.Amount,
                   nowString, nowString, "0", retCode, ptnSrl, configModel.ConfigId, memberId, "中金支付", ThirdPartyType.CPCNConfigSign);
                paramDTO.OnlinePaymentRecord = onlinePayment;
                paramDTO.PaymentTradeRecord = tradeRecord;

                bool saveResult = PaymentService.SaveInTransaction(paramDTO);

                if (!saveResult)
                {
                    returnModel.Success = false;
                    if (isSuccess)
                    {
                        Logger.LogError(string.Format("中金支付交易成功，但是平台处理失败：往平台推送开户信息数据出错。平台流水号：{0}，中金流水号：{1}。请求的XML:{2}，返回的XML{3}", ptnSrl, retSrl, postXML, returnXML));
                    }
                    returnModel.ErrorMessage = "操作失败！";
                }

                return Json(returnModel);
            }

            catch (Exception ex)
            {
                Logger.LogError(memberId + "入金失败,系统出错！" + ex.Message + string.Format("请求的XML:{0}，返回的XML{1}", postXML, returnXML));
                returnModel.Success = false;
                returnModel.ErrorMessage = "充值失败！";
                return Json(returnModel);
            }
        }


        /// <summary>
        /// 扫码入金
        /// </summary>
        /// <param name="paramDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult QRInTransaction(InTransactionDTO paramDTO)
        {
            //付款方
            var memberId = paramDTO.MemberId;
            //收款方
            var peeMemberId = paramDTO.PeeMemberId;
            var userName = paramDTO.UserName;
            string postXML = string.Empty;
            string returnXML = string.Empty;
            //扫码 支付方式
            int secPayType = paramDTO.SecPayType;
            var bankName = secPayType == 3 ? "支付宝扫码入金" : "微信扫码入金";

            PaymentReturnModel returnModel = new PaymentReturnModel();
            //根据支付订单号来判断是否重新提交的扫码请求
            var paymentOrder = PaymentService.GetOnlinePaymentRecordByOrderNoAndAmount(paramDTO.OrderNo, paramDTO.Amount, bankName);
            if (paymentOrder != null && !string.IsNullOrEmpty(paymentOrder.VerifyResult))
            {
                returnModel.Success = true;
                returnModel.ReturnData = paymentOrder.VerifyResult;
                return Json(returnModel);
            }

            try
            {
                //获取中金支付的配置
                CPCNConfigDTO configModel = new CPCNConfigDTO();
                returnModel = GetCPCNConfig(configModel);
                if (!returnModel.Success)
                {
                    return Json(returnModel);
                }
                //流水号唯一
                var ptnSrl = OtherUtilHelper.GetSerialNumber("ZJ");

                KC.WebApi.Pay.Models.CPCN.InAndOutTransaction.QRPay.MSG msg = new Models.CPCN.InAndOutTransaction.QRPay.MSG();
                //公共信息
                msg.MSGHD = GetMessageHead(configModel, CPCNInterfaceConstants.QRPayIn);

                //中金支付账号信息
                var cpcnBankAccount = PaymentService.GetPaymentBankAccountByMemberId(peeMemberId);
                if (cpcnBankAccount == null)
                {
                    returnModel.Success = false;
                    returnModel.ErrorMessage = "未开通财富共赢钱包！";
                    return Json(returnModel);
                }

                CltAcc cltAcc = new CltAcc();
                cltAcc.CltNm = cpcnBankAccount.AccountName;
                cltAcc.SubNo = cpcnBankAccount.AccountNo;
                msg.CltAcc = cltAcc;

                Models.CPCN.InAndOutTransaction.QRPay.Amt amt = new Models.CPCN.InAndOutTransaction.QRPay.Amt();
                amt.AclAmt = paramDTO.Amount;
                amt.CcyCd = "CNY";
                msg.Amt = amt;
                msg.ReqFlg = 1;
                msg.TrsFlag = "A00";

                msg.Usage = paramDTO.Usage == null ?
                    "" : (paramDTO.Usage.Length > 24 ? paramDTO.Usage.Substring(0, 24) : paramDTO.Usage);

                Models.CPCN.InAndOutTransaction.QRPay.PayInfo payInfo = new Models.CPCN.InAndOutTransaction.QRPay.PayInfo();
                payInfo.GoodsDesc = "扫码入金";
                payInfo.PayType = 6;
                payInfo.SecPayType = secPayType;
                payInfo.Subject = paramDTO.OrderNo;

                msg.PayInfo = payInfo;
                //流水号
                Srl srl = new Srl();
                srl.PtnSrl = ptnSrl;
                msg.Srl = srl;

                postXML = CPCNPayment.ModelToXml(msg, configModel.Version);
                returnXML = PostCPCNData(configModel, CPCNInterfaceConstants.QRPayIn, postXML);

                var cpcnRetModel = CPCNPayment.XmlToModel<Models.CPCN.InAndOutTransaction.QRPay.ReturnResponse.MSG>(returnXML);

                var retCode = cpcnRetModel.MSGHD.RspCode;
                var retMessage = cpcnRetModel.MSGHD.RspMsg;

                bool isSuccess = false;
                string retSrl = string.Empty;

                //返回成功的处理
                if (retCode.Equals(CPCNReturnCodeConstants.Success))
                {
                    isSuccess = true;
                    retSrl = cpcnRetModel.Srl.PlatSrl;
                }
                else
                {
                    returnModel.Success = false;
                    returnModel.ErrorCode = retCode;
                    returnModel.ErrorMessage = string.Format("交易失败，失败原因：{0}", retMessage);
                    Logger.LogError(string.Format("中金支付交易失败，失败原因：{0}，平台流水号：{1}。请求的XML:{2}，返回的XML{3}", retMessage, ptnSrl, postXML, returnXML));
                }

                //交易流水
                var tradeRecord = AddPaymentTradeRecord(postXML, returnXML, peeMemberId, ptnSrl, retSrl, CPCNInterfaceConstants.QRPayIn, PaymentOperationType.BankIn, isSuccess, retMessage, retCode, ThirdPartyType.CPCNConfigSign, userName);
                //插入OnlinePayment表
                var nowString = DateTime.Now.ToString("yyyyMMddhhmmss");
                var onlinePayment = AddOnlinePaymentRecords(paramDTO.PaymentOrderId, paramDTO.OrderNo, paramDTO.Amount, paramDTO.Amount,
                   nowString, nowString, "0", retCode, ptnSrl, configModel.ConfigId, memberId, "中金支付", ThirdPartyType.CPCNConfigSign);
                onlinePayment.VerifyResult = cpcnRetModel.ImageUrl;//把二维码链接存起来
                onlinePayment.PeeMemberId = paramDTO.PeeMemberId;
                onlinePayment.BankName = bankName;
                onlinePayment.OrderNo = paramDTO.OrderNo;

                paramDTO.OnlinePaymentRecord = onlinePayment;
                paramDTO.PaymentTradeRecord = tradeRecord;

                bool saveResult = PaymentService.SaveInTransaction(paramDTO);

                if (!saveResult)
                {
                    returnModel.Success = false;
                    if (isSuccess)
                    {
                        Logger.LogError(string.Format("中金支付交易成功，但是平台处理失败：往平台推送开户信息数据出错。平台流水号：{0}，中金流水号：{1}。请求的XML:{2}，返回的XML{3}", ptnSrl, retSrl, postXML, returnXML));
                    }
                    returnModel.ErrorMessage = "操作失败！";
                }
                else
                {
                    returnModel.ReturnData = cpcnRetModel.ImageUrl;
                }
                return Json(returnModel);
            }

            catch (Exception ex)
            {
                Logger.LogError(memberId + "入金失败,系统出错！" + ex.Message + string.Format("请求的XML:{0}，返回的XML{1}", postXML, returnXML));
                returnModel.Success = false;
                returnModel.ErrorMessage = "充值失败！";
                return Json(returnModel);
            }
        }


        /// <summary>
        /// 入金查询
        /// </summary>
        /// <param name="paramDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult InTransactionSearch(InTransactionSearchDTO paramDTO)
        {
            var memberId = paramDTO.MemberId;
            var userName = paramDTO.UserName;
            string postXML = string.Empty;
            string returnXML = string.Empty;

            PaymentReturnModel returnModel = new PaymentReturnModel();
            try
            {
                //获取中金支付的配置
                CPCNConfigDTO configModel = new CPCNConfigDTO();
                returnModel = GetCPCNConfig(configModel);
                if (!returnModel.Success)
                {
                    return Json(returnModel);
                }


                //中金支付账号信息
                var cpcnBankAccount = PaymentService.GetPaymentBankAccountByMemberId(memberId);
                if (cpcnBankAccount == null)
                {
                    returnModel.Success = false;
                    returnModel.ErrorMessage = "未开通财富共赢钱包！";
                    return Json(returnModel);
                }
                //加锁
                new RedisDistributedLock().DoDistributedLock("CPCNPayController" + memberId, () =>
                {
                    //渠道_网银入金_结果查询[T2012]
                    KC.WebApi.Pay.Models.InAndOutTransaction.NetBankInSearch.MSG msg = new KC.WebApi.Pay.Models.InAndOutTransaction.NetBankInSearch.MSG();
                    //公共信息
                    msg.MSGHD = GetMessageHead(configModel, CPCNInterfaceConstants.InTransactionSearch);
                    //入金记录
                    var onlinePayment = PaymentService.GetOnlinePaymentRecordByPaymentId(paramDTO.PaymentOrderId);
                    //入金状态不等于1时去中金查询
                    if (onlinePayment.PayResult != "1")
                    {
                        msg.OrgSrl = onlinePayment.BankNumber;
                        //流水号唯一
                        var ptnSrl = OtherUtilHelper.GetSerialNumber("ZJ");

                        postXML = CPCNPayment.ModelToXml(msg, configModel.Version);
                        returnXML = PostCPCNData(configModel, CPCNInterfaceConstants.InTransactionSearch, postXML);

                        var cpcnRetModel = CPCNPayment.XmlToModel<KC.WebApi.Pay.Models.InAndOutTransaction.NetBankInSearch.ReturnResponse.MSG>(returnXML);

                        var retCode = cpcnRetModel.MSGHD.RspCode;
                        var retMessage = cpcnRetModel.MSGHD.RspMsg;

                        bool isSuccess = false;
                        string retSrl = string.Empty;
                        decimal retAmount = 0;
                        int state = 0;
                        string opion = string.Empty;

                        //返回成功的处理
                        if (retCode.Equals(CPCNReturnCodeConstants.Success))
                        {
                            isSuccess = true;
                            retSrl = cpcnRetModel.Srl.PlatSrl;
                            retAmount = cpcnRetModel.Amt.AclAmt;
                            state = cpcnRetModel.State;
                            opion = cpcnRetModel.Opion;
                            onlinePayment.PayResult = state.ToString();
                            //返回成功
                            if (state == 1)
                            {
                                //查询
                                var searchReturnModel = GetSearchAccountBanlace(paramDTO, cpcnBankAccount);
                                if (searchReturnModel.Success)
                                {
                                    isSuccess = true;
                                    onlinePayment.ErrorMessage = "充值成功";
                                    //更新金额

                                    cpcnBankAccount = PaymentService.GetPaymentBankAccountByMemberId(memberId);
                                    var returnAmtDTO = searchReturnModel.ReturnAmtData;
                                    cpcnBankAccount.TotalAmount = returnAmtDTO.BalanceAmt;
                                    cpcnBankAccount.FreezeAmount = returnAmtDTO.FreezeAmt;
                                    cpcnBankAccount.CFWinTotalAmount = cpcnBankAccount.CFWinTotalAmount + retAmount;
                                    paramDTO.PaymentBankAccount = cpcnBankAccount;
                                }
                            }
                            else if (state == 2)
                            {
                                if (opion.Contains("不存在此支付交易"))
                                {
                                    onlinePayment.PayResult = "3";
                                    retMessage = string.Format("充值订单：{0}，正在处理中", paramDTO.PaymentOrderId);
                                    returnModel.ReturnData = "3";
                                }
                                else
                                {
                                    retMessage = string.Format("充值订单：{0}，充值失败。失败原因：{1}", paramDTO.PaymentOrderId, opion);
                                    Logger.LogError(string.Format("中金支付交易失败，失败原因：{0}，平台流水号：{1}。请求的XML:{1}，返回的XML{2}", retMessage, ptnSrl, postXML, returnXML));
                                    returnModel.ReturnData = state;
                                }
                                isSuccess = false;
                                onlinePayment.ErrorMessage = opion;
                                returnModel.ErrorMessage = retMessage;
                            }
                            else if (state == 3)
                            {
                                isSuccess = false;
                                onlinePayment.ErrorMessage = opion;
                                retMessage = string.Format("充值订单：{0}，正在处理中", paramDTO.PaymentOrderId);
                                returnModel.ReturnData = state;
                                returnModel.ErrorMessage = retMessage;
                            }
                        }
                        else
                        {
                            returnModel.Success = false;
                            returnModel.ErrorCode = retCode;
                            returnModel.ErrorMessage = string.Format("交易失败，失败原因：{0}", retMessage);
                            Logger.LogError(string.Format("中金支付交易失败，失败原因：{0}，平台流水号：{1}。请求的XML:{1}，返回的XML{2}", retMessage, ptnSrl, postXML, returnXML));
                        }
                        //设置查询次数
                        SetOnlinePaymentRecordSearchCount(onlinePayment);
                        //交易流水
                        var tradeRecord = AddPaymentTradeRecord(postXML, returnXML, memberId, ptnSrl, retSrl, CPCNInterfaceConstants.InTransactionSearch, PaymentOperationType.BankInSearch, isSuccess, retMessage, retCode, ThirdPartyType.CPCNConfigSign, userName);
                        paramDTO.IsSuccess = isSuccess;
                        paramDTO.PaymentTradeRecord = tradeRecord;
                        paramDTO.OnlinePaymentRecord = onlinePayment;
                        bool saveResult = PaymentService.SaveInTransactionSearch(paramDTO);
                        if (!saveResult)
                        {
                            returnModel.Success = false;
                            if (isSuccess)
                            {
                                Logger.LogError(string.Format("中金支付交易成功，但是平台处理失败：往平台推送开户信息数据出错。平台流水号：{0}，中金流水号：{1}。请求的XML:{2}，返回的XML{3}", ptnSrl, retSrl, postXML, returnXML));
                            }
                            returnModel.ErrorMessage = "操作失败！";
                        }
                    }
                    else
                    {
                        //入金成功后返回true
                        returnModel.Success = true;
                    }
                });
                return Json(returnModel);

            }
            catch (Exception ex)
            {
                Logger.LogError(memberId + "充值查询失败,系统出错！" + ex.Message + string.Format("请求的XML:{0}，返回的XML{1}", postXML, returnXML));
                returnModel.Success = false;
                returnModel.ErrorMessage = "充值查询失败！";
                return Json(returnModel);
            }
        }

        /// <summary>
        /// 通知相关的入口
        /// </summary>
        /// <param name="receiveNoticeDTO"></param>
        /// <returns></returns>
        public JsonResult Notice(ReceiveNoticeDTO receiveNoticeDTO)
        {
            PaymentReturnModel returnModel = new PaymentReturnModel();
            var message = receiveNoticeDTO.message;

            try
            {
                //base64转string
                var xmlMessage = Base64Provider.DecodeString(message);
                //获取中金支付的配置
                CPCNConfigDTO config = new CPCNConfigDTO();
                returnModel = GetCPCNConfig(config);
                if (!returnModel.Success)
                {
                    return Json(returnModel);
                }
                ////验证签名。
                //string path = string.Format(@"{0}PayCert\CPCN\", AppDomain.CurrentDomain.BaseDirectory) + "trzprod.cer";
                //string password = config.Password;
                //var signature = CPCNPayment.GetPubSignature(path, xmlMessage);
                ////验证失败。
                //if (signature != receiveNoticeDTO.signature)
                //{
                //    returnModel.Success = false;
                //    returnModel.ErrorMessage = "验证签名失败！";
                //    Logger.LogError(string.Format("{0}，验证签名失败！请求的签名:{1},验证的签名:{2}，请求的信息：{3}。返回的XML：{4}", receiveNoticeDTO.ptncode,
                //        receiveNoticeDTO.signature, signature, message, xmlMessage));
                //}
                //else
                //{
                switch (receiveNoticeDTO.trdcode)
                {
                    case "T2008":
                        returnModel = InTransactionNotice(receiveNoticeDTO, xmlMessage, config);
                        break;

                    case "T1131":
                        returnModel.Success = true;
                        break;

                    case "T1189":
                        returnModel.Success = true;
                        break;
                    default:
                        returnModel.Success = false;
                        returnModel.ErrorMessage = "暂不支持此交易";
                        returnModel.ReturnData = "暂不支持此交易";
                        Logger.LogError(receiveNoticeDTO.trdcode + "，暂不支持此交易。请求的XML：" + message);
                        break;
                }
                //}

            }
            catch (Exception ex)
            {
                Logger.LogError("通知出错！" + ex.Message + string.Format("接口ptncode:{0},请求的信息：{1}", receiveNoticeDTO.ptncode, message));
                returnModel.Success = false;
                returnModel.ErrorMessage = "通知出错！";
            }

            return Json(returnModel);
        }

        /// <summary>
        /// 入金通知
        /// </summary>
        /// <returns></returns>
        private PaymentReturnModel InTransactionNotice(ReceiveNoticeDTO receiveNoticeDTO, string xmlMessage, CPCNConfigDTO config)
        {
            PaymentReturnModel returnModel = new PaymentReturnModel();
            returnModel.Success = true;
            var inNoticeModel = CPCNPayment.XmlToModel<Models.CPCN.InAndOutTransaction.InNotice.MSG>(xmlMessage);
            //inNoticeModel.TrsFlag = 0;//模拟线下入金测试
            if (inNoticeModel != null)
            {
                var amount = inNoticeModel.Amt.AclAmt;
                KC.WebApi.Pay.Models.CPCN.InAndOutTransaction.InNotice.Srl srl = inNoticeModel.Srl;
                BkAcc bkAcc = inNoticeModel.BkAcc;
                var restTime = inNoticeModel.RestTime;
                CltAcc cltAcc = inNoticeModel.CltAcc;
                var subNo = cltAcc.SubNo;

                var platSrl = srl.PlatSrl;
                var srcPtnSrl = srl.SrcPtnSrl;
                var usage = inNoticeModel.Usage;
                var opion = inNoticeModel.Opion;

                //根据资金账号获取账户信息
                var paymentBankAccount = PaymentService.GetPaymentBankAccountByAccountNum(subNo);

                if (paymentBankAccount == null)
                {
                    returnModel.Success = false;
                    returnModel.ErrorMessage = "资金账号不存在！";
                    returnModel.ReturnData = "资金账号不存在！";
                    Logger.LogError(string.Format("资金账号不存在,请求的xml：{0}", xmlMessage));
                    return returnModel;
                }
                var memberId = paymentBankAccount.MemberId;
                receiveNoticeDTO.MemberId = memberId;
                receiveNoticeDTO.UserName = "Root";
                //加锁
                new RedisDistributedLock().DoDistributedLock("CPCNPayController" + memberId, () =>
                {
                    //成功后做处理
                    if (inNoticeModel.State == 1)
                    {
                        OnlinePaymentRecordDTO getOnlinePayment = new OnlinePaymentRecordDTO();
                        //银行发起
                        if (inNoticeModel.TrsFlag == 0)
                        {

                            var paymentOrderId = OtherUtilHelper.GetSerialNumber("RJ");
                            getOnlinePayment = AddOnlinePaymentRecords(paymentOrderId, "线下入金", amount, amount,
           restTime, restTime, "1", "000000", platSrl, config.ConfigId, memberId, "中金支付", ThirdPartyType.CPCNConfigSign);

                        }
                        else if (inNoticeModel.TrsFlag == 1)
                        {
                            getOnlinePayment = PaymentService.GetOnlinePaymentRecordByBankNumber(srcPtnSrl);
                            if (getOnlinePayment != null && getOnlinePayment.PayResult != "1")
                            {
                                getOnlinePayment.PayResult = "1";
                            }
                        }

                        //查询
                        var searchReturnModel = GetSearchAccountBanlace(receiveNoticeDTO, paymentBankAccount);
                        if (searchReturnModel.Success)
                        {
                            receiveNoticeDTO.IsSuccess = true;
                            //更新金额
                            var returnAmtDTO = searchReturnModel.ReturnAmtData;
                            paymentBankAccount.TotalAmount = returnAmtDTO.BalanceAmt;
                            paymentBankAccount.FreezeAmount = returnAmtDTO.FreezeAmt;
                            paymentBankAccount.CFWinTotalAmount = paymentBankAccount.CFWinTotalAmount + amount;
                            receiveNoticeDTO.PaymentBankAccount = paymentBankAccount;
                        }
                        //交易流水
                        var tradeRecord = AddPaymentTradeRecord(xmlMessage, "", memberId, platSrl, srcPtnSrl, CPCNInterfaceConstants.InTransactionSearch,
                        PaymentOperationType.BankInSearch, receiveNoticeDTO.IsSuccess, "入金通知", "000000", ThirdPartyType.CPCNConfigSign, "Root");
                        receiveNoticeDTO.PaymentTradeRecord = tradeRecord;

                        receiveNoticeDTO.OnlinePaymentRecord = getOnlinePayment;
                        bool saveResult = PaymentService.SaveReceiveNotice(receiveNoticeDTO);
                        if (!saveResult)
                        {
                            returnModel.Success = false;
                            if (receiveNoticeDTO.IsSuccess)
                            {
                                Logger.LogError(string.Format("平台处理失败：往平台推送开户信息数据出错。平台流水号：{0}，中金流水号：{1}。请求的XML:{2}，返回的XML{3}",
                                    platSrl, srcPtnSrl, xmlMessage, ""));
                            }
                            returnModel.ErrorMessage = "操作失败！";
                        }
                        else
                        {
                            if (getOnlinePayment != null && getOnlinePayment.BankName != null && getOnlinePayment.BankName.Contains("扫码入金"))
                            {
                                //IMemberService memberService = new MemberService();
                                //var signature = memberService.GetSignatureByMemberId(getOnlinePayment.PeeMemberId);
                                //var paymentApiService = new PaymentApiService(getOnlinePayment.PeeMemberId, signature);
                                //var result = paymentApiService.QRCoreInNotic(getOnlinePayment.PaymentOrderId).Result;
                                //if (!result)
                                //{
                                //    Logger.LogError("扫码入金通知支付系统失败，订单编号为：" + getOnlinePayment.PaymentOrderId);
                                //}
                            }
                        }
                        returnModel.Success = true;
                    }
                    //已经处理过的返回
                    else
                    {
                        returnModel.Success = false;
                        returnModel.ErrorMessage = "已处理过的请求！";
                    }
                });

                //处理成功后去检查是否CBS支付订单
                if (returnModel.Success)
                {
                    //查询是否是CBS的支付订单，如果是，需要冻结金额。
                    var getCBSOnlinePaymentRecord = PaymentService.GetCBSOnlinePaymentRecordByTime(usage, memberId);
                    if (getCBSOnlinePaymentRecord != null)
                    {
                        FreezeAmtDTO paramDTO = new FreezeAmtDTO();
                        paramDTO.MemberId = memberId;
                        paramDTO.FreezeAmt = amount;
                        paramDTO.Usage = "CBS支付冻结";
                        paramDTO.BusiType = 1;
                        paramDTO.UserName = "Robot";
                        paramDTO.PaymentOrderId = OtherUtilHelper.GetSerialNumber("DJ");
                        returnModel = FreezeAmount(paramDTO);
                        if (!returnModel.Success)
                        {
                            Logger.LogError(string.Format("平台处理失败：中金入金推送成功，但是CBS冻结资金失败！冻结资金的PaymentOrderId:{0}", paramDTO.PaymentOrderId));
                        }
                        //到账后通知。
                        var noticeUrl = GlobalConfig.PayWebDomain.Replace("subdomain", memberId) + "/Home/CBSCallBack?paymentOrderId=" + getCBSOnlinePaymentRecord.PaymentOrderId;
                        HttpWebRequestHelper.WebClientDownload(noticeUrl);
                    }
                }

                //}
                //else if (inNoticeModel.TrsFlag == 1)
                //{
                //    returnModel.Success = true;
                //}

                KC.WebApi.Pay.Models.CPCN.InAndOutTransaction.InNotice.ReturnResponse.MSG msg = new Models.CPCN.InAndOutTransaction.InNotice.ReturnResponse.MSG();
                MSGHD msghd = new MSGHD();
                if (returnModel.Success)
                {
                    msghd.RspCode = "000000";
                    msghd.RspMsg = "";
                }
                else
                {
                    msghd.RspCode = "ERRRRR";
                    msghd.RspMsg = returnModel.ErrorMessage;
                }

                msg.MSGHD = msghd;
                KC.WebApi.Pay.Models.CPCN.InAndOutTransaction.InNotice.Srl retSrl = new KC.WebApi.Pay.Models.CPCN.InAndOutTransaction.InNotice.Srl();
                retSrl.PlatSrl = platSrl;
                retSrl.SrcPtnSrl = srcPtnSrl;
                msg.Srl = retSrl;

                var retXml = CPCNPayment.ModelToXml(msg, config.Version);

                returnModel.ReturnData = Base64Provider.EncodeString(retXml);
            }
            return returnModel;
        }

        /// <summary>
        /// 出金
        /// </summary>
        /// <param name="paramDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult OutTransaction(OutTransactionDTO paramDTO)
        {
            var memberId = paramDTO.MemberId;
            var userName = paramDTO.UserName;
            string postXML = string.Empty;
            string returnXML = string.Empty;

            PaymentReturnModel returnModel = new PaymentReturnModel();
            try
            {
                PaymentBankAccountDTO paymentBankAccount = new PaymentBankAccountDTO();
                paymentBankAccount = PaymentService.GetPlatformAccount(ThirdPartyType.CPCNConfigSign);
                if (paymentBankAccount == null)
                {
                    returnModel.Success = false;
                    returnModel.ErrorMessage = "提现失败，请联系客服！";
                    Logger.LogError("未找到平台的结算账户！");
                    return Json(returnModel);
                }

                //获取中金支付的配置
                CPCNConfigDTO configModel = new CPCNConfigDTO();
                returnModel = GetCPCNConfig(configModel);
                if (!returnModel.Success)
                {
                    return Json(returnModel);
                }
                //流水号唯一
                var ptnSrl = OtherUtilHelper.GetSerialNumber("ZJ");

                KC.WebApi.Pay.Models.CPCN.InAndOutTransaction.BankOutApply.MSG msg = new KC.WebApi.Pay.Models.CPCN.InAndOutTransaction.BankOutApply.MSG();
                //公共信息
                msg.MSGHD = GetMessageHead(configModel, CPCNInterfaceConstants.OutTransaction);

                //中金支付账号信息
                var cpcnBankAccount = PaymentService.GetPaymentBankAccountByMemberId(memberId);
                if (cpcnBankAccount == null)
                {
                    returnModel.Success = false;
                    returnModel.ErrorMessage = "未开通财富共赢钱包！";
                    return Json(returnModel);
                }
                bool isSuccess = false;
                //加锁
                new RedisDistributedLock().DoDistributedLock("CPCNPayController" + memberId, () =>
                {

                    CltAcc cltAcc = new CltAcc();
                    cltAcc.CltNm = cpcnBankAccount.AccountName;
                    cltAcc.SubNo = cpcnBankAccount.AccountNo;
                    msg.CltAcc = cltAcc;

                    KC.WebApi.Pay.Models.CPCN.InAndOutTransaction.BankOutApply.BkAcc bkAcc = new KC.WebApi.Pay.Models.CPCN.InAndOutTransaction.BankOutApply.BkAcc();
                    bkAcc.AccNm = cpcnBankAccount.BindBankAccountName;
                    bkAcc.AccNo = cpcnBankAccount.BindBankAccount;
                    msg.BkAcc = bkAcc;

                    KC.WebApi.Pay.Models.CPCN.InAndOutTransaction.BankOutApply.Amt amt = new KC.WebApi.Pay.Models.CPCN.InAndOutTransaction.BankOutApply.Amt();
                    amt.AclAmt = paramDTO.Amount;
                    amt.FeeAmt = 0;
                    amt.TAmt = paramDTO.Amount;
                    amt.CcyCd = "CNY";
                    msg.Amt = amt;

                    msg.TrsFlag = "A00";
                    msg.BalFlag = paramDTO.BalFlag == 1 ? "T0" : "T1";

                    msg.Usage = "提现";

                    //流水号
                    Srl srl = new Srl();
                    srl.PtnSrl = ptnSrl;
                    msg.Srl = srl;


                    postXML = CPCNPayment.ModelToXml(msg, configModel.Version);
                    returnXML = PostCPCNData(configModel, CPCNInterfaceConstants.OutTransaction, postXML);

                    var cpcnRetModel = CPCNPayment.XmlToModel<KC.WebApi.Pay.Models.CPCN.InAndOutTransaction.BankOutApply.ReturnResponse.MSG>(returnXML);

                    var retCode = cpcnRetModel.MSGHD.RspCode;
                    var retMessage = cpcnRetModel.MSGHD.RspMsg;


                    string retSrl = string.Empty;

                    //返回成功的处理
                    if (retCode.Equals(CPCNReturnCodeConstants.Success))
                    {
                        retSrl = cpcnRetModel.Srl.PlatSrl;
                        //查询
                        var searchReturnModel = GetSearchAccountBanlace(paramDTO, cpcnBankAccount);

                        if (searchReturnModel.Success)
                        {
                            isSuccess = true;
                            //更新金额
                            cpcnBankAccount = PaymentService.GetPaymentBankAccountByMemberId(memberId);
                            var returnAmtDTO = searchReturnModel.ReturnAmtData;
                            cpcnBankAccount.TotalAmount = returnAmtDTO.BalanceAmt;
                            cpcnBankAccount.FreezeAmount = returnAmtDTO.FreezeAmt;
                            cpcnBankAccount.CFWinTotalAmount = cpcnBankAccount.CFWinTotalAmount - paramDTO.Amount;
                            paramDTO.PaymentBankAccount = cpcnBankAccount;

                        }
                    }
                    else
                    {
                        returnModel.Success = false;
                        returnModel.ErrorCode = retCode;
                        returnModel.ErrorMessage = string.Format("提现失败，失败原因：{0}", retMessage);
                        Logger.LogError(string.Format("中金支付交易失败，失败原因：{0}，平台流水号：{1}。请求的XML:{2}，返回的XML{3}", retMessage, ptnSrl, postXML, returnXML));
                    }

                    //交易流水
                    var tradeRecord = AddPaymentTradeRecord(postXML, returnXML, memberId, ptnSrl, retSrl, CPCNInterfaceConstants.OutTransaction, PaymentOperationType.BankOut,
                        isSuccess, retMessage, retCode, ThirdPartyType.CPCNConfigSign, userName);
                    var nowString = DateTime.Now.ToString("yyyyMMddhhmmss");
                    var paymentOrderId = OtherUtilHelper.GetSerialNumber("Out");
                    var onlinePayment = AddOnlinePaymentRecords(paramDTO.PaymentOrderId, "出金", paramDTO.Amount, paramDTO.Amount,
                       nowString, nowString, (isSuccess ? "1" : "0"), retCode, ptnSrl, configModel.ConfigId, memberId, "中金支付", ThirdPartyType.CPCNConfigSign);

                    paramDTO.IsSuccess = isSuccess;
                    paramDTO.PaymentTradeRecord = tradeRecord;
                    paramDTO.OnlinePaymentRecord = onlinePayment;
                    bool saveResult = PaymentService.OutTransaction(paramDTO);

                    if (!saveResult)
                    {
                        returnModel.Success = false;
                        if (isSuccess)
                        {
                            Logger.LogError(string.Format("中金支付交易成功，但是平台处理失败：往平台推送开户信息数据出错。平台流水号：{0}，中金流水号：{1}.请求的XML:{2}，返回的XML{3}", ptnSrl, retSrl, postXML, returnXML));
                        }
                        returnModel.ErrorMessage = "操作失败！";
                    }
                });

                //提现成功后支付手续费
                if (isSuccess)
                {
                    OrderPayDTO orderPayDTO = new OrderPayDTO();
                    var paymentFeeOrderId = OtherUtilHelper.GetSerialNumber("Fee");
                    if (paramDTO.FeeAmount > 0)
                    {
                        orderPayDTO.MemberId = paramDTO.MemberId;
                        orderPayDTO.UserName = paramDTO.UserName;
                        orderPayDTO.Usage = "出金";

                        orderPayDTO.OrderNo = "出金手续费";
                        orderPayDTO.PaymentOrderId = paymentFeeOrderId;
                        orderPayDTO.PayeeAccountName = paymentBankAccount.AccountName;
                        orderPayDTO.PayeeAccountNumber = paymentBankAccount.AccountNo;
                        orderPayDTO.PayType = 1;
                        orderPayDTO.Amount = paramDTO.FeeAmount;
                        var orderReturnModel = OrderPayment(orderPayDTO);
                        if (!orderReturnModel.Success)
                        {
                            returnModel.Success = false;
                            returnModel.ErrorMessage = "提现失败，请联系客服！";
                            Logger.LogError(string.Format("已提现，手续费收取失败,手续费支付Id为：{4}。失败原因：{0}，平台流水号：{1}。请求的XML:{2}，返回的XML{3}", orderReturnModel.ErrorMessage, ptnSrl, postXML, returnXML, paymentFeeOrderId));
                            return Json(returnModel);
                        }
                    }

                }

                return Json(returnModel);

            }

            catch (Exception ex)
            {
                Logger.LogError(memberId + "提现失败,系统出错！" + ex.Message + string.Format("请求的XML:{0}，返回的XML{1}", postXML, returnXML));
                returnModel.Success = false;
                returnModel.ErrorMessage = "提现失败！";
                return Json(returnModel);
            }
        }

        #region 私有方法

        /// <summary>
        /// 获取请求头部信息
        /// </summary>
        /// <param name="configModel">配置model</param>
        /// <param name="trCode">中金交易代码</param>
        /// <returns></returns>
        private MSGHD GetMessageHead(CPCNConfigDTO configModel, string trCode)
        {
            MSGHD msghd = new MSGHD();
            msghd.BkCd = configModel.BkCode;
            msghd.PtnCd = configModel.PtnCode;
            msghd.TrCd = trCode;
            msghd.TrDt = DateTime.Now.ToString("yyyyMMdd");
            msghd.TrTm = DateTime.Now.ToString("HHmmss");
            msghd.TrSrc = "F";
            return msghd;
        }

        /// <summary>
        /// 获取中金支付的配置
        /// </summary>
        /// <param name="configModel"></param>
        /// <returns></returns>
        private PaymentReturnModel GetCPCNConfig(CPCNConfigDTO configModel)
        {
            PaymentReturnModel returnModel = new PaymentReturnModel();
            returnModel.Success = true;
            returnModel.ErrorMessage = "财富共赢钱包配置有误！";
            var configEntity = ConfigApiService.GetConfigsByTypeAndSign(ConfigType.PaymentMethod,(int)ThirdPartyType.CPCNConfigSign);
            if (configEntity == null || configEntity.ConfigAttributes.Count <= 0)
            {
                returnModel.Success = false;
                Logger.LogError(returnModel.ErrorMessage);
                return returnModel;
            }
            configModel.ConfigId = configEntity.ConfigId;
            var attributes = configEntity.ConfigAttributes.ToList();
            //资金托管方编号
            var configAttribute = attributes.Find(m => m.Name.Equals(CPCNConfigConstants.bkcode, StringComparison.CurrentCultureIgnoreCase));
            if (configAttribute == null)
            {
                returnModel.Success = false;
                Logger.LogError(returnModel.ErrorMessage);
                return returnModel;
            }
            configModel.BkCode = configAttribute.Value;

            //合作方编号
            configAttribute = attributes.Find(m => m.Name.Equals(CPCNConfigConstants.ptncode, StringComparison.CurrentCultureIgnoreCase));
            if (configAttribute == null)
            {
                returnModel.Success = false;
                Logger.LogError(returnModel.ErrorMessage);
                return returnModel;
            }
            configModel.PtnCode = configAttribute.Value;

            //合作方的签名证书
            configAttribute = attributes.Find(m => m.Name.Equals(CPCNConfigConstants.SingCertName, StringComparison.CurrentCultureIgnoreCase));
            if (configAttribute == null)
            {
                returnModel.Success = false;
                Logger.LogError(returnModel.ErrorMessage);
                return returnModel;
            }
            configModel.SingCertName = configAttribute.Value;

            //合作方的签名证书口令
            configAttribute = attributes.Find(m => m.Name.Equals(CPCNConfigConstants.password, StringComparison.CurrentCultureIgnoreCase));
            if (configAttribute == null)
            {
                returnModel.Success = false;
                Logger.LogError(returnModel.ErrorMessage);
                return returnModel;
            }
            configModel.Password = configAttribute.Value;

            //交易地址
            configAttribute = attributes.Find(m => m.Name.Equals(CPCNConfigConstants.TransactionPostUrl, StringComparison.CurrentCultureIgnoreCase));
            if (configAttribute == null)
            {
                returnModel.Success = false;
                Logger.LogError(returnModel.ErrorMessage);
                return returnModel;
            }
            configModel.TransactionPostUrl = configAttribute.Value;

            //版本
            configAttribute = attributes.Find(m => m.Name.Equals(CPCNConfigConstants.Version, StringComparison.CurrentCultureIgnoreCase));
            if (configAttribute == null)
            {
                returnModel.Success = false;
                Logger.LogError(returnModel.ErrorMessage);
                return returnModel;
            }
            configModel.Version = configAttribute.Value;
            returnModel.ErrorMessage = "操作成功！";
            return returnModel;
        }

        /// <summary>
        /// 提交数据到中金支付
        /// </summary>
        /// <param name="config">中金支付的配置</param>
        /// <param name="trdCode">中金支付提供的接口名</param>
        /// <param name="xmlMessage">xml数据</param>
        /// <returns>中金返回的XML数据</returns>
        private string PostCPCNData(CPCNConfigDTO config, string trdCode, string xmlMessage)
        {
            var paramDic = new Dictionary<string, string>();
            paramDic["ptncode"] = config.PtnCode;
            paramDic["trdcode"] = trdCode;
            //转成base64位的字符串
            paramDic["message"] = Base64Provider.EncodeString(xmlMessage);
            string path = string.Format(@"{0}PayCert\CPCN\", AppDomain.CurrentDomain.BaseDirectory) + config.SingCertName;
            string password = config.Password;
            var signature = CPCNPayment.GetSignature(path, password, xmlMessage);
            paramDic["signature"] = signature;
            //交易接口地址
            string postUrl = config.TransactionPostUrl;
            var retStr = HttpWebRequestHelper.DoPost(postUrl, paramDic);
            return Base64Provider.DecodeString(retStr);
        }

        #endregion
    }
}
