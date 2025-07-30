using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using KC.Service.DTO.Pay;
using Microsoft.AspNetCore.Mvc;
using KC.Service.Pay;
using System.Text;
using KC.Enums.Pay;
using KC.Framework.Base;
using KC.Service.Constants;
using KC.Storage.Util;
using KC.Common.ToolsHelper;
using KC.Framework.Exceptions;
using KC.Framework.Extension;
using KC.Service.Util;

namespace KC.Web.Pay.Controllers
{
    public class PaymentController : PaymentBaseController
    {
        protected IPayableService PayableService => ServiceProvider.GetService<IPayableService>();
        protected IReceivableService ReceivableService => ServiceProvider.GetService<IReceivableService>();
        protected IPaymentRecordService PaymentRecordService => ServiceProvider.GetService<IPaymentRecordService>();
        protected IOfflinePaymentService OfflinePaymentService => ServiceProvider.GetService<IOfflinePaymentService>();
        protected IOfflineUsageBillService OfflineUsageBillService => ServiceProvider.GetService<IOfflineUsageBillService>();

        public PaymentController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<PaymentController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        public JsonResult LoadCFCAInfo()
        {
            return GetServiceJsonResult(() =>
            {
                var uKeyInfo = AccountApiService.GetUkeyAuthenticationByMemberId(TenantName);
                return uKeyInfo;
            });
        }

        #region 票据

        //public JsonResult GetElectronicBillRecords(string orderId)
        //{
        //    _financeApiService = new Com.Service.Store.WebApiService.Platform.FinanceApiService(Tenant);

        //    return GetServiceJsonResult(() =>
        //    {
        //        var records = _financeApiService.GetElectronicBillRecords(orderId).Result;
        //        return records;
        //    });
        //}

        //public JsonResult GetElectronicBillRecordsOfAccountStatement(Guid id)
        //{
        //    _financeApiService = new Com.Service.Store.WebApiService.Platform.FinanceApiService(Tenant);

        //    return GetServiceJsonResult(() =>
        //    {
        //        return _financeApiService.GetElectronicBillRecordsOfAccountStatement(id).Result;
        //    });
        //}

        //public JsonResult GetOrderStillNeedPayAmount(List<string> orders)
        //{
        //    _financeApiService = new Com.Service.Store.WebApiService.Platform.FinanceApiService(Tenant);

        //    return GetServiceJsonResult(() =>
        //    {
        //        return _financeApiService.GetOrderStillNeedPayAmount(orders).Result;
        //    });
        //}

        public JsonResult AddOfflineUsageBillRecord(OfflineUsageBillDTO dto, string blobId, string fileName)
        {
            return GetServiceJsonResult(() =>
            {
                if (dto.AmountOfMoney <= 0)
                {
                    throw new ArgumentException("金额不正确！");
                }
                var payable = PayableService.GetPayableByPayableNumber(dto.PayableNumber);
                if (payable == null)
                {
                    throw new ArgumentException("未找到应付信息！");
                }

                if (payable.AlreadyPayAmount == payable.PayableAmount || dto.AmountOfMoney > payable.PayableAmount - payable.AlreadyPayAmount)
                {
                    throw new ArgumentException("不需要支付：" + dto.AmountOfMoney.ToString("C"));
                }
                var tenantName = string.Empty;
                var tenantDisplayName = string.Empty;
                if (payable.Source == PayableSource.AccountStatement)
                {
                    //var accountStatement = AccountStatementService.GetAccountStatementRecordByNumber_V3(payable.OrderId);
                    //if (accountStatement == null)
                    //{
                    //    throw new ArgumentException("未找到对账单信息！");
                    //}
                    //if (payable.PayableAmount != dto.AmountOfMoney)
                    //{
                    //    throw new ArgumentException("对账单必须全额支付！");
                    //}

                    //tenantName = accountStatement.SellerTenantName;
                    //tenantDisplayName = accountStatement.Seller;
                }
                else if (payable.Source == PayableSource.PO)
                {
                    //var po = SupplyChainService.GetPOIncludeVendorByOrderId(payable.OrderId);
                    //if (po == null)
                    //{
                    //    throw new ArgumentException("未找到订单信息！");
                    //}
                    //tenantName = po.Vendor.VendorCode;
                    //tenantDisplayName = po.Vendor.VendorName;
                }

                //var existBillNumber = new Com.Service.Store.WebApiService.Platform.P_OrderApiService(Tenant).CheckBillNumber(dto.BillNumber).Result;
                //if (!existBillNumber)
                //{
                //    throw new ArgumentException("已存在票据号:" + dto.BillNumber);
                //}
                //var waiteConfirm = SupplyChainService.GetWaitConfirmPaymentAmount(payable.Source, payable.PayableNumber, payable.PayableAmount - payable.AlreadyPayAmount - dto.AmountOfMoney);
                //if (waiteConfirm.Item1 > 0)
                //{
                //    msg = waiteConfirm.Item2;
                //    return false;
                //}
                var userId = CurrentUserId;
                dto.PayableSource = payable.Source;
                dto.PayDateTime = DateTime.UtcNow;
                dto.AmountOfMoney = -dto.AmountOfMoney;
                dto.BusinessNumber = OtherUtilHelper.GetSerialNumber(SequenceNumberConstants.PaymentApply);
                dto.Customer = tenantDisplayName;
                dto.Attachments = new PaymentAttachmentDTO { BlobId = blobId, FileName = fileName, BusinessNumber = dto.BusinessNumber, Url = GetFileUrl(fileName, blobId, userId) };
                var addResult = OfflineUsageBillService.AddOfflineUsageBill(dto, CurrentUserDisplayName + "(" + CurrentUserName + ")");
                if (addResult)//给卖方添加数据
                {
                    BlobUtil.CopyTempsToClientBlob(Tenant, new List<string> { blobId }, userId);
                    switch (dto.PayableSource)
                    {
                        case PayableSource.PO:
                            dto.ReceivableSource = ReceivableSource.SCO;
                            break;
                    }

                    dto.ReceivableSource = PaymentRecordService.GetReceivableSource(dto.PayableSource.Value);
                    dto.PayableSource = null;
                    dto.Customer = Tenant.TenantDisplayName;
                    //new Com.Service.Store.WebApiService.Business.OrderApiService(Tenant).AddOfflineUsageBillRecord(dto, tenantName);
                }
                return addResult;
            });
        }

        public JsonResult LoadOfflineUsageBills(string payableNumber)
        {
            return GetServiceJsonResult(() =>
            {
                return OfflineUsageBillService.LoadOfflineUsageBill(payableNumber);
            });
        }

        public JsonResult CancelOfflineUsageBill(int id, string remark)
        {
            return GetServiceJsonResult(() =>
            {
                var modifyResult = OfflineUsageBillService.CancelOfflineUsageBill(id, remark, CurrentUserDisplayName + "(" + CurrentUserName + ")");
                if (modifyResult != null)//需删除卖方的线下支付数据
                {
                    var sellerTenantName = string.Empty;
                    if (modifyResult.Item3)
                    {
                        //var accountStatement = AccountStatementService.GetAccountStatementRecordByNumber_V3(modifyResult.Item2);
                        //if (accountStatement == null)
                        //    return false;
                        //sellerTenantName = accountStatement.SellerTenantName;
                    }
                    else
                    {
                        //var po = SupplyChainService.GetPOIncludeVendorByOrderId(modifyResult.Item2);
                        //if (po == null)
                        //{
                        //    return false;
                        //}
                        //sellerTenantName = po.Vendor.VendorCode;
                    }

                    //new Com.Service.Store.WebApiService.Business.OrderApiService(Tenant).RemoveOfflineUsageBillRecord(modifyResult.Item1, sellerTenantName);
                }
                return true;
            });
        }

        public JsonResult ReturnOfflineUsageBill(string orderId, string businessNumber, string remark)
        {
            return GetServiceJsonResult(() =>
            {
                var buyerTenantName = string.Empty;
                //var so = SupplyChainService.GetSOIncludeCustomerByOrderId(orderId);
                //if (so == null)
                //{
                //    var accountStatement = AccountStatementService.GetAccountStatementRecordByNumber_V3(orderId);
                //    if (accountStatement == null)
                //        return false;
                //    buyerTenantName = accountStatement.BuyerTenantName;
                //}
                //else
                //    buyerTenantName = so.Customer.CustomerCode;
                //var apiResult = new Com.Service.Store.WebApiService.Business.OrderApiService(Tenant).ReturnOfflineUsageBill(businessNumber, remark, buyerTenantName);
                var apiResult = ExecServiceResult.Success;
                //if (apiResult.ResultType == Service.ServiceResultType.Success)
                {
                    if (apiResult == ExecServiceResult.Success)
                    {
                        return OfflineUsageBillService.ReturnOfflineUsageBill(businessNumber, remark, CurrentUserDisplayName + "(" + CurrentUserName + ")") == ExecServiceResult.Success;
                    }
                    else if (apiResult == ExecServiceResult.NotFound)//需删除卖方数据
                    {
                        return OfflineUsageBillService.RemoveOfflineUsageBill(businessNumber);
                    }
                }
                return false;
            });
        }

        public JsonResult ConfirmOfflineUsageBill(string orderId, string businessNumber)
        {
            return GetServiceJsonResult(() =>
            {
                return true;
                //return SupplyChainService.ConfirmOfflineUsageBillAndUpdateSO(orderId, businessNumber, CurrentUserDisplayName + "(" + CurrentUserName + ")");
            });
        }

        #endregion

        #region 线下支付

        public JsonResult AddOfflinePaymentRecord(string payableNumber, decimal paymentAmount, string blobId, string fileName, string remark)
        {
            return GetServiceJsonResult(() =>
            {
                #region 检查应付条件

                if (paymentAmount <= 0)
                {
                    throw new ArgumentException("金额不正确！");
                }
                var payable = PayableService.GetPayableByPayableNumber(payableNumber);
                if (payable == null)
                {
                    throw new ArgumentException("未找到应付信息！");
                }

                if (payable.AlreadyPayAmount == payable.PayableAmount)
                {
                    throw new ArgumentException("此笔应付已支付完成。");
                }

                if (payable.PayableAmount - payable.AlreadyPayAmount < paymentAmount)
                {
                    throw new ArgumentException("此提交金额超出应付限制，请重新输入。");
                }
                #endregion

                var tenantName = string.Empty;
                var tenantDisplayName = string.Empty;

                var userId = CurrentUserId;
                var dto = new OfflinePaymentDTO
                {
                    PayableNumber = payableNumber,
                    Remark = remark,
                    AmountOfMoney = -paymentAmount,
                    PayDateTime = DateTime.UtcNow,
                    OrderId = payable.OrderId,
                    PayableSource = payable.Source,
                    Customer = tenantName,
                    BusinessNumber = OtherUtilHelper.GetSerialNumber(SequenceNumberConstants.PaymentApply)
                };

                var attachement = new PaymentAttachmentDTO { BlobId = blobId, FileName = fileName, BusinessNumber = dto.BusinessNumber, Url = GetFileUrl(fileName, blobId, userId) };
                dto.Attachments = attachement;
                var addResult = OfflinePaymentService.AddOfflinePayment(dto, CurrentUserDisplayName + "(" + CurrentUserName + ")");
                if (addResult)//给对方添加数据
                {
                    BlobUtil.CopyTempsToClientBlob(Tenant, new List<string> { blobId }, userId);
                    dto.ReceivableSource = PaymentRecordService.GetReceivableSource(dto.PayableSource.Value);
                    dto.PayableSource = null;
                    dto.Customer = Tenant.TenantDisplayName;
                    //TODO: 
                }
                return addResult;
            });
        }

        public JsonResult LoadOfflinePayments(string payableNumber)
        {
            return GetServiceJsonResult(() =>
            {
                return OfflinePaymentService.LoadOfflinePayment(payableNumber);
            });
        }

        public JsonResult CancelOfflinePayment(int id, string remark)
        {
            return GetServiceJsonResult(() =>
            {
                var modifyResult = OfflinePaymentService.CancelOfflinePayment(id, remark, CurrentUserDisplayName + "(" + CurrentUserName + ")");
                if (modifyResult != null)//需删除卖方的线下支付数据
                {
                    if (modifyResult.Item3 == PayableSource.PO)
                    {
                    }
                    else if (modifyResult.Item3 == PayableSource.RefundCautionMoney)
                    {
                    }
                    else if (modifyResult.Item3 == PayableSource.AccountStatement)
                    {
                    }
                }
                return true;
            });
        }

        public JsonResult ReturnOfflinePayment(string orderId, string businessNumber, string remark)
        {
            return GetServiceJsonResult(() =>
            {
                var dto = OfflinePaymentService.GetByBusinessNumber(businessNumber);
                if (dto == null)
                    return false;
                var receivable = ReceivableService.FindByPayableNumber(dto.PayableNumber);
                if (receivable == null)
                    return false;

                var tenantName = string.Empty;
                if (receivable.Source == ReceivableSource.RefundCautionMoney)
                {
                    //tenantName = accountStatement.BuyerTenantName;
                    //tenantDisplayName = accountStatement.Buyer;
                }
                else if (receivable.Source == ReceivableSource.AccountStatement)
                {
                    //tenantName = po.Vendor.VendorCode;
                }
                else if (receivable.Source == ReceivableSource.SO)
                {
                    //tenantName = so.Customer.CustomerCode;
                }

                return OfflinePaymentService.RemoveOfflinePayment(businessNumber);
            });
        }

        public JsonResult ConfirmOfflinePayment(string orderId, string businessNumber)
        {
            var msg = string.Empty;
            return GetServiceJsonResult(() =>
            {
                var offlineRecord = OfflinePaymentService.GetByBusinessNumberIncludeAttachment(businessNumber);
                if (offlineRecord == null)
                {
                    throw new ArgumentException("未找到线下付款信息！");
                }

                var receivable = ReceivableService.FindByPayableNumber(offlineRecord.PayableNumber);
                if (receivable == null)
                {
                    throw new ArgumentException("未找到应收信息！");
                }

                var tenantName = string.Empty;
                var tenantDisplayName = string.Empty;

                if (receivable.Source == ReceivableSource.AccountStatement)
                {
                    //tenantName = accountStatement.BuyerTenantName;
                    //tenantDisplayName = accountStatement.Buyer;
                }
                else if (receivable.Source == ReceivableSource.RefundCautionMoney)
                {
                    //tenantName = po.Vendor.VendorCode;
                }
                else if (receivable.Source == ReceivableSource.SO)
                {
                    //tenantName = so.Customer.CustomerCode;
                }

                if (receivable.AlreadyPayAmount == receivable.ReceivableAmount || offlineRecord.AmountOfMoney > receivable.ReceivableAmount - receivable.AlreadyPayAmount)
                {
                    throw new ArgumentException("不需要支付：" + offlineRecord.AmountOfMoney.ToString("C"));
                }

                //1、先确认对方数据
                //var apiResult = new Com.Service.Store.WebApiService.Business.OrderApiService(Tenant).ConfirmOfflinePayment(businessNumber, tenantName);
                var apiResult = ExecServiceResult.Success;
                if (apiResult != ExecServiceResult.Success)
                {
                    msg = "对方处理失败！";
                    return false;
                }

                //2、处理本方数据
                if (apiResult == ExecServiceResult.Success)
                {
                    var confirmResult = OfflinePaymentService.ConfirmOfflinePayment(businessNumber, CurrentUserDisplayName + "(" + CurrentUserName + ")") == ExecServiceResult.Success;
                    if (!confirmResult)
                    {
                        msg = "确认线下支付失败";
                        Logger.LogError(string.Format("对方确认线下支付失败,OrderId:{0},PayableNumber:{1},businessNumber:{2}", orderId, offlineRecord.PayableNumber, businessNumber));
                        return false;
                    }
                    var source = new List<ReceivableSource>
                     {
                         ReceivableSource.SO,
                         ReceivableSource.AdvanceCharge,
                         ReceivableSource.CautionMoney,
                         ReceivableSource.AccountStatement
                     };
                    //当前方是卖方
                    if (source.Contains(receivable.Source))
                    {
                        var attachmentUrl = offlineRecord.Attachments != null ? offlineRecord.Attachments.Url : string.Empty;
                        //处理买方PO
                        //var postResult = new OrderApiService(Tenant).OfflinePaymentOfOrder(offlineRecord.PayableNumber, businessNumber, offlineRecord.AmountOfMoney, attachmentUrl, tenantName);
                        //if (postResult.success && postResult.Result)
                        {
                            if (receivable.Source == ReceivableSource.SO)
                            {
                                //处理so
                                //return SupplyChainService.ModifySOPaymentStatus(so.SONumber);
                            }
                            else if (receivable.Source == ReceivableSource.AdvanceCharge)
                            {
                                //return SupplyChainService.ModifySOAdvanceChargeStatus(so.SONumber, offlineRecord.AmountOfMoney, null);
                            }
                            else if (receivable.Source == ReceivableSource.CautionMoney)
                            {
                                //return SupplyChainService.ModifySOCautionMoneyStatus(so.SONumber, offlineRecord.AmountOfMoney, null);
                            }
                            else if (receivable.Source == ReceivableSource.AccountStatement)
                            {
                                //return SupplyChainService.PaymentOfAccountStatement(orderId);
                            }
                        }
                        //else
                        {
                            Logger.LogError(string.Format("卖方确认线下支付后，处理买方PO失败,OrderId:{0},PayableNumber:{1},businessNumber:{2}", orderId, offlineRecord.PayableNumber, businessNumber));
                            msg = "确认线下支付后，处理订单数据失败！";
                            return false;
                        }
                    }//当前方是买方
                    else if (receivable.Source == ReceivableSource.RefundCautionMoney)
                    {
                        //SupplyChainService.OfflinePaymentOfRefundCautionMoney(orderId, businessNumber, offlineRecord.AmountOfMoney);
                    }
                }
                else if (apiResult == ExecServiceResult.NotFound)//需删除卖方数据
                {
                    return OfflinePaymentService.RemoveOfflinePayment(businessNumber);
                }
                return true;
            });
        }

        #endregion

        #region 白条

        public JsonResult AddVoucherPayment(VoucherPaymentRecordDTO dto)
        {
            return GetServiceJsonResult(() =>
            {
                var payable = PayableService.GetPayableByPayableNumber(dto.PayableNumber);
                if (payable == null)
                {
                    throw new BusinessPromptException("未找到应付信息");
                }

                if (payable.AlreadyPayAmount == payable.PayableAmount || dto.Amounts > payable.PayableAmount - payable.AlreadyPayAmount)
                {
                    throw new BusinessPromptException("不需要支付：" + dto.Amounts.ToString("C"));
                }
                if (payable.Source == PayableSource.AccountStatement)
                {
                    if (payable.PayableAmount != dto.Amounts)
                    {
                        throw new BusinessPromptException("对账单必须全额支付");
                    }
                }

                return PaymentRecordService.AddVoucherPaymentRecord(dto, CurrentUserDisplayName + "(" + CurrentUserName + ")");
            });
        }

        public JsonResult LoadVoucherPaymentRecord(string orderId)
        {
            return GetServiceJsonResult(() =>
            {
                return PaymentRecordService.LoadVoucherPaymentRecord(orderId);
            });
        }

        public JsonResult CancelVoucher(int id, string remark, bool cancel = true)
        {
            return GetServiceJsonResult(() =>
            {
                return PaymentRecordService.CancelVoucher(id, remark, CurrentUserDisplayName, CurrentUserName, cancel);
            });
        }

        /// <summary>
        /// 买方内部审核通过白条，推送白条数据到卖方
        /// </summary>
        /// <param name="id"></param>
        /// <param name="certificateId"></param>
        /// <param name="content"></param>
        /// <param name="sign"></param>
        /// <param name="canTransferable"></param>
        /// <returns></returns>
        public JsonResult CreateVoucher(int id, string certificateId, string content, string sign, bool canTransferable)
        {
            return GetServiceJsonResult(() =>
            {
                return PaymentRecordService.SaveVoucher(id, certificateId, content, sign, canTransferable, CurrentUserName, CurrentUserDisplayName);
            });
        }

        /// <summary>
        /// 卖方通过白条
        /// </summary>
        /// <returns></returns>
        public JsonResult ConfirmVoucher(CertificateDTO dto)
        {
            return GetServiceJsonResult(() =>
            {
                var cfcaInfo = AccountApiService.GetUkeyAuthenticationByMemberId(TenantName).Result;
                if (cfcaInfo == null)
                    throw new BusinessPromptException("Ukey签名信息未找到");
                var certData = DESSercurityUtil.GetCertFromSignedData(cfcaInfo.Signature);
                if (certData == null)
                    throw new BusinessPromptException("签名信息不正确");
                dto.CertificateSignature = dto.CertificateSignature.Replace("&quot;", "\"");
                if (!DESSercurityUtil.VerifyPKCS1("create_ious" + dto.CertificateSignature, dto.SellerCertificate, certData))
                    throw new BusinessPromptException("签名信息和使用签名的证书不匹配");
                if (!string.IsNullOrWhiteSpace(dto.VouchOrderId) && !string.IsNullOrWhiteSpace(dto.VouchTenantName))//买方向担保机构领用额度编号，卖方签收白条时，也需要向担保机构领用额度
                {
                    #region 卖方需要向担保机构领用额度
                    
                    #endregion
                }
                //var result = new Com.Service.Shop.WebApiService.Business.FinanceApiService(Tenant).UpdateVoucherStatusForShop(dto, Enums.Market.Order.VoucherStatus.InForce, CurrentUserId, CurrentUserDisplayName);
                //卖方签收后，白条还未生效，需要买方再确认一次才生效，才需要保存白条数据到卖方
                //if (result.Result == null)
                //    return false;
                //if (result.Result.Success)
                //    return true;
                //throw new BusinessPromptException(result.Result.ErrorMessage);
                //if (result.Result.Success)
                //    return PaymentRecordService.VoucherPaymentSuccess(dto);
                //return result.Result.Success;

                return true;
            });
        }

        /// <summary>
        /// 卖方退回白条
        /// </summary>
        /// <returns></returns>
        public JsonResult ReturnVoucher(string id, string orderId, string remark)
        {
            return GetServiceJsonResult(() =>
            {
                return PaymentRecordService.ReturnVoucher(id, remark);
            });
        }

        #endregion

        /// <summary>
        /// 查询支付结果
        /// </summary>
        /// <param name="payNum"></param>
        /// <returns></returns>
        public JsonResult QueryPayResult(string payNum)
        {
            return GetServiceJsonResult(() =>
            {
                return PaymentRecordService.CheckPaymentIsSuccessful(payNum);
            });
        }

        public JsonResult CancelPayment(int id, string remark, bool cancel = true)
        {
            return GetServiceJsonResult(() =>
            {
                return PaymentRecordService.CancelPayment(id, remark, CurrentUserId, CurrentUserDisplayName, CurrentUserName, cancel);
            });
        }

        public JsonResult CreatePaymentData(string paymentNumber)
        {
            return GetServiceJsonResult(() =>
            {
                if (!IsOpenSecuritySetting())
                {
                    throw new BusinessPromptException("未开通财富共赢钱包或未设置支付密码和绑定支付手机，请设置后再操作！");
                }

                var payment = PaymentRecordService.GetPaymentRecordByPaymentNum(paymentNumber);
                if (payment == null)
                {
                    throw new BusinessPromptException("不需要支付！");
                }
                var payable = PayableService.GetPayableByPayableNumber(payment.PayableNumber);
                if (payable == null)
                {
                    throw new BusinessPromptException("未找到对应的应付信息！");
                }
                if (payable.AlreadyPayAmount + payment.PaymentAmount > payable.PayableAmount)
                {
                    throw new BusinessPromptException("支付信息不匹配，应付小于" + payment.PaymentAmount.ToString("C"));
                }
                var sellerPayment = CheckOpenPaymentAndBindBank(payment.PayeeTenant);
                if (!sellerPayment.Item1 || !sellerPayment.Item2)
                {
                    throw new BusinessPromptException("卖方【" + payment.Payee + "】未开通财富共赢钱包或绑定银行卡。请提醒卖方开通");
                }
                var needFrozen = !(payment.Source.HasValue &&
                              (payment.Source == PayableSource.PlatformServiceFee ||
                               payment.Source == PayableSource.SystemFee ||
                               payment.Source == PayableSource.CautionMoney ||
                               payment.Source == PayableSource.ValueAddedServiceFee ||
                               payment.Source == PayableSource.RefundCautionMoney));
                //如果是退还保证金，还需插入保证金操作记录
                if (payable.Source == PayableSource.RefundCautionMoney)
                {
                    
                }
                else if (payable.Source == PayableSource.PO)
                {
                    
                }
                else if (payable.Source == PayableSource.AccountStatement)
                {
                    needFrozen = false;//对账单现金付款不需要冻结
                }

                //加入定时任务，轮询支付状态
                //new Service.WebApiService.Business.TenantUserApiService().AddCheckPayTask(Tenant.TenantName, payment.PaymentNumber);
                var dto = new PaymentViewModelDTO();
                dto.MemberId = TenantName;
                dto.UserName = CurrentUserName;
                dto.Amount = decimal.Parse(payment.PaymentAmount.ToString("0.##"));
                dto.OrderNo = payable.Source == PayableSource.SystemFee || payable.Source == PayableSource.ValueAddedServiceFee ? payable.PayableNumber : payment.OrderNumber;
                dto.OrderType = payment.Source.HasValue ? payment.Source.Value.ToDescription() : "充值";
                dto.PayeeTenant = payment.PayeeTenant;
                dto.Payee = payment.Payee;
                dto.PaymentOrderId = payment.PaymentNumber;
                dto.Usage = payment.Remark;
                dto.OrderAmount = decimal.Parse(payable.OrderAmount.ToString("0.##"));
                dto.Timestamp = ConvertDateTimeToInt(DateTime.UtcNow);
                dto.PayType = !needFrozen ? 1 : 2;
                dto.ThirdPartyType = payment.PaymentType;
                dto.EncryptString = OtherUtilHelper.GetStrByModel(dto, Tenant.PrivateEncryptKey);
                return dto;
            });
        }

        private string GetFileUrl(string fileName, string blobId, string userId)
        {
            var apiDoamin = GlobalConfig.DocWebDomain.Replace(TenantConstant.SubDomain, Tenant.TenantName);
            var showImgDomain = apiDoamin + "api/Resources/ShowImage/";
            var showFileDomain = apiDoamin + "api/Resources/DownloadFile/";
            var extensionStr = fileName.Split('.');
            if (GlobalConfig.UploadConfig.ImageExt.Split(',').Contains(extensionStr.Last().ToLower()))
            {
                return showImgDomain + blobId + "?uId=" + userId;
            }
            return showFileDomain + blobId + "?uId=" + userId;
        }
    }
}
