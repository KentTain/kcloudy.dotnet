using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using KC.Service.DTO.Pay;
using KC.Common;
using Microsoft.AspNetCore.Mvc;
using KC.Service.Pay;
using System.Text;
using KC.Enums.Pay;
using KC.Service.DTO;
using KC.Service;
using KC.Common.FileHelper;
using KC.Framework.Extension;
using KC.Common.ToolsHelper;
using KC.Service.Pay.Constants;
using KC.Framework.Base;
using KC.Service.Constants;
using KC.Framework.Exceptions;
using KC.Service.Pay.WebApiService.Platform;
using KC.Model.Component.Queue;
using KC.Service.WebApiService.Business;
using KC.Common.HttpHelper;

namespace KC.Web.Pay.Controllers
{
    public class CapitalManagerController : PaymentBaseController
    {
        protected KC.Service.Component.IStorageQueueService StorageQueueService
        {
            get
            {
                //TODO: Storage with TenantName
                return ServiceProvider.GetService<KC.Service.Component.IStorageQueueService>();
            }
        }
        protected IPayableService PayableService => ServiceProvider.GetService<IPayableService>();
        protected IReceivableService ReceivableService => ServiceProvider.GetService<IReceivableService>();
        protected IPaymentRecordService PaymentRecordService => ServiceProvider.GetService<IPaymentRecordService>();
        protected IFinanceApiService FinanceApiService => ServiceProvider.GetService<IFinanceApiService>();
        protected IConfigApiService ConfigApiService => ServiceProvider.GetService<IConfigApiService>();
        public CapitalManagerController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<CapitalManagerController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        #region 资金管理首页
        public ActionResult AssetSummary()
        {
            ViewBag.Summary = GetAssetSummary();
            var paymentInfo = GetPaymentInfo(Tenant.TenantName, ThirdPartyType.CPCNConfigSign);
            var phone = paymentInfo != null ? paymentInfo.Phone : string.Empty;
            ViewBag.IsTradePhoneSet = !string.IsNullOrEmpty(phone);
            ViewBag.IsTradePasswordSet = paymentInfo != null && !string.IsNullOrEmpty(paymentInfo.TradePassword);
            var configs = ConfigApiService.GetAllConfigsByType(ConfigType.PaymentMethod);
            ViewBag.IsActiveCNPC = configs != null ? configs.Any(m => m.ConfigSign == (int)ThirdPartyType.CPCNConfigSign) : false;
            ViewBag.IsActiveBoHai = configs != null ? configs.Any(m => m.ConfigSign == (int)ThirdPartyType.BoHaiConfigSign) : false;
            var paymentAccounts = GetPaymentAccounts(Tenant.TenantName);
            ViewBag.CNPCAccount = paymentAccounts != null ? paymentAccounts.FirstOrDefault(m => m.PaymentType == ThirdPartyType.CPCNConfigSign) : new PaymentBankAccountDTO();
            ViewBag.BoHaiAccount = paymentAccounts != null ? paymentAccounts.FirstOrDefault(m => m.PaymentType == ThirdPartyType.BoHaiConfigSign) : new PaymentBankAccountDTO();

            return View();
        }

        public JsonResult QueryAssetSummary()
        {
            return GetServiceJsonResult(() =>
            {
                return GetAssetSummary();
            });
        }

        private Tuple<decimal, decimal> GetAssetSummary()
        {
            try
            {
                var bankAcount = GetPaymentBankAccount();
                return bankAcount == null ? new Tuple<decimal, decimal>(0, 0) : new Tuple<decimal, decimal>(bankAcount.SysAvailableBalance, bankAcount.SysFrozenAmount);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, ex);
            }
            return new Tuple<decimal, decimal>(0, 0);
        }

        /// <summary>
        /// 支付页面
        /// </summary>
        /// <returns></returns>
        public JsonResult OpenPayment()
        {
            return GetServiceJsonResult(() =>
            {
                PayBaseParamDTO paramDTO = new PayBaseParamDTO();
                paramDTO.MemberId = Tenant.TenantName;
                paramDTO.UserName = CurrentUserName;
                paramDTO.Timestamp = ConvertDateTimeToInt(DateTime.UtcNow);
                paramDTO.EncryptString = OtherUtilHelper.GetStrByModel(paramDTO, Tenant.PrivateEncryptKey);

                string postUrl = PayApiDomain + CPCNMethodConstants.OpenAccount;
                var postData = OtherUtilHelper.GetPostData(paramDTO);
                var result = HttpWebRequestHelper.WebClientDownload(postUrl, postData);
                return SerializeHelper.FromJson<JsonPaymentReturnModel>(result.Item2).Result;
            });
        }
        #endregion

        #region 收入/支出视图
        /// <summary>
        /// 收入视图
        /// </summary>
        /// <returns></returns>
        public ActionResult Income()
        {
            return View();
        }
        /// <summary>
        /// 支出视图
        /// </summary>
        /// <returns></returns>
        public ActionResult Expenditure()
        {
            return View();
        }

        /// <summary>
        /// 获取现金收入数据
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public JsonResult GetCashIncomeData(DateTime? startDate, DateTime? endDate, int page = 1, int rows = 10)
        {
            decimal countMoney = 0;
            var result = PaymentService.GetPaginatedCashUsageDetailByFilter(true, CurrentUserId, page, rows, startDate, endDate, ref countMoney);
            var newReult = new CapitalManagerPaginatedBaseDTO<CashUsageDetailDTO>(result.pageIndex, result.pageSize, result.total, result.rows, countMoney);
            return Json(newReult);
        }

        /// <summary>
        /// 获取现金支出数据
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public JsonResult GetCashExpenditureData(DateTime? startDate, DateTime? endDate, int page = 1, int rows = 10)
        {
            decimal countMoney = 0;
            var result = PaymentService.GetPaginatedCashUsageDetailByFilter(false, CurrentUserId, page, rows, startDate,
       endDate, ref countMoney);
            var newReult = new CapitalManagerPaginatedBaseDTO<CashUsageDetailDTO>(result.pageIndex, result.pageSize, result.total, result.rows, countMoney);
            return Json(newReult);
        }
        #endregion

        

        #region 充值

        public ActionResult Charge()
        {
            ViewBag.PostUrl = PayApiDomain + CPCNMethodConstants.SubmitCharge;
            var model = GetPaymentBankAccount();
            return View(model);
        }

        public ActionResult ChargeFlow()
        {
            return View();
        }

        public JsonResult CreateChargeDataUrl(decimal chargeAmount)
        {
            return GetServiceJsonResult(() =>
            {
                if (!(chargeAmount >= 100 && chargeAmount <= 1000000))
                {
                    throw new BusinessPromptException("100元<=单笔充值金额<=1000000元");
                }
                if (!PaymentService.IsOpenPayment(TenantName, ThirdPartyType.CPCNConfigSign))
                {
                    throw new BusinessPromptException("请先设置交易密码和验证手机号再进行操作！");
                }
                var bindBank = CheckOpenPaymentAndBindBank();
                if (!bindBank.Item1)
                {
                    throw new BusinessPromptException("您还未开通财富共赢钱包，请先开通后再进行操作！");
                }
                if (!bindBank.Item2)
                {
                    throw new BusinessPromptException("您还未绑定银行账户，请先绑定银行账户后再进行操作！");
                }
                var number = OtherUtilHelper.GetSerialNumber(SequenceNumberConstants.CZ);
                //给平台插入充值流水记录
                var resultPlat = FinanceApiService.ApplyForRecharge(chargeAmount, number, null);
                if (resultPlat.Result)
                {
                    //加入定时任务，轮询支付状态
                    //TenantStore.AddCheckChargeTask(Tenant.TenantName, number);
                    var dto = new PaymentViewModelDTO();
                    dto.MemberId = TenantName;
                    dto.UserName = CurrentUserName;
                    dto.Amount = decimal.Parse(chargeAmount.ToString("0.##"));
                    dto.OrderType = "充值";
                    dto.PaymentOrderId = number;
                    dto.Usage = "充值" + chargeAmount.ToString("0.##");
                    dto.Timestamp = ConvertDateTimeToInt(DateTime.UtcNow);
                    dto.EncryptString = OtherUtilHelper.GetStrByModel(dto, Tenant.PrivateEncryptKey);
                    return dto;
                }
                throw new BusinessPromptException("插入平台充值记录失败！");
            });

        }
        public JsonResult AddChargeRecord(decimal chargeAmount)
        {
            if (chargeAmount <= 0)
            {
                return ThrowErrorJsonMessage(false, "充值金额不正确");
            }
            return GetServiceJsonResult(() =>
            {
                var number = OtherUtilHelper.GetSerialNumber(SequenceNumberConstants.CZ);
                var paymentRecord = new PaymentApplyRecordDTO();
                paymentRecord.OrderNumber = number;
                paymentRecord.PayableNumber = number;
                paymentRecord.PaymentNumber = number;
                paymentRecord.PayeeTenant = TenantName;
                paymentRecord.Payee = TenantDisplayName;
                paymentRecord.PaymentAmount = chargeAmount;
                paymentRecord.Remark = "充值:" + chargeAmount;
                return PaymentRecordService.AddChargeOfWithdrawalsRecord(paymentRecord, CurrentUserId, CurrentUserDisplayName, "申请充值：");
            });
        }

        public JsonResult SubmitInTransactionSearch(string chargeNumber)
        {
            return GetServiceJsonResult(() =>
            {
                PaymentReturnModel returnModel = new PaymentReturnModel();
                //查找此支付订单是否已经完成。
                var payResult = PaymentService.IsOnlinePaymentSuccess(chargeNumber);
                if (payResult)
                {
                    return UpChargeRecordState(chargeNumber);
                }

                InTransactionSearchDTO paramDTO = new InTransactionSearchDTO();
                paramDTO.MemberId = TenantName;
                paramDTO.UserName = TenantDisplayName;
                paramDTO.PaymentOrderId = chargeNumber;
                paramDTO.Timestamp = ConvertDateTimeToInt(DateTime.UtcNow);
                paramDTO.EncryptString = OtherUtilHelper.GetStrByModel(paramDTO, Tenant.PrivateEncryptKey);
                string postUrl = PayApiDomain + CPCNMethodConstants.InTransactionSearch;
                var postData = OtherUtilHelper.GetPostData(paramDTO);
                var paymentReturn = SerializeHelper.FromJson<JsonPaymentReturnModel>(HttpWebRequestHelper.WebClientDownload(postUrl, postData).Item2).Result;
                if (paymentReturn != null && paymentReturn.Success)
                {
                    return UpChargeRecordState(chargeNumber, string.IsNullOrEmpty(paymentReturn.ErrorCode) && paymentReturn.ErrorMessage == "操作成功！", paymentReturn.ErrorMessage);
                }
                Logger.LogError("查询中间充值状态失败。错误编码：" + paymentReturn.ErrorCode + " 错误信息：" + paymentReturn.ErrorMessage);
                throw new BusinessPromptException(paymentReturn.ErrorMessage);
            });
        }

        public bool UpChargeRecordState(string chargeNumber, bool bl = true, string mess = "")
        {
            //给平台插入充值流水记录
            var resultPlat = FinanceApiService.ApplyForRecharge(0, chargeNumber, bl, mess);
            return resultPlat.Result;
        }

        public JsonResult UpChargeRecord(string chargeNumber)
        {
            return GetServiceJsonResult(() =>
            {
                var payment = PaymentRecordService.GetPaymentRecordByPaymentNum(chargeNumber);
                if (payment == null)
                    return false;
                var onlinePaymentresult = PaymentService.GetOnlinePaymentRecordByPaymentId(chargeNumber);
                if (onlinePaymentresult != null && onlinePaymentresult.PayResult != "0" && onlinePaymentresult.PayResult != "3")
                {
                    if (onlinePaymentresult.PayResult == "1")
                    {
                        var amount = PaymentRecordService.UpChargeRecord(chargeNumber, CurrentUserId, CurrentUserDisplayName, "充值成功：", true);
                        if (amount <= 0)
                            return false;
                        //给平台插入充值流水记录
                        var resultPlat = FinanceApiService.ApplyForRecharge(amount, chargeNumber, true);
                        if (!resultPlat.Result)
                        {
                            PaymentRecordService.AddPaymentOperationLog(chargeNumber, CurrentUserId, CurrentUserDisplayName, "错误信息：" + onlinePaymentresult.ErrorMessage + "插入平台充值流水记录失败，充值金额：");
                        }
                        return resultPlat.Result;
                    }
                    else
                    {
                        var amount = PaymentRecordService.UpChargeRecord(chargeNumber, CurrentUserId, CurrentUserDisplayName, "充值失败：", false);
                        var resultPlat = FinanceApiService.ApplyForRecharge(amount, chargeNumber, false, onlinePaymentresult.ErrorMessage);
                        Logger.LogError("中金接口状态查询失败: 返回状态" + onlinePaymentresult.PayResult);
                        if (!(resultPlat != null && resultPlat.Result))
                        {
                            throw new BusinessPromptException("充值失败！");
                        }
                    }
                }
                else
                {
                    Logger.LogError("中金接口状态查询失败: 返回状态" + onlinePaymentresult.PayResult);
                    throw new BusinessPromptException("接口状态查询失败！");
                }
                return false;
            });
        }
        public JsonResult CencelChargeRecord(int id)
        {
            return GetServiceJsonResult(() =>
            {
                return PaymentRecordService.CencelChargeRecord(id, CurrentUserId, CurrentUserDisplayName);
            });
        }
        public JsonResult FindChargingList(int page = 1, int rows = 10)
        {
            var model = PaymentRecordService.LoadPagenatedFindChargingList(page, rows);
            return Json(model);
        }

        public JsonResult LoadChargeFlow(DateTime? startDate, DateTime? endDate, RechargeStatus? status, int page = 1, int rows = 15)
        {
            return
                Json(FinanceApiService.GetRechargeRecords(startDate, endDate, status, page, rows).Result);
        }

        #endregion

        #region 提现
        public ActionResult WithdrawalsFlow()
        {
            return View();
        }

        public JsonResult LoadWithdrawalsFlow(DateTime? startDate, DateTime? endDate, WithdrawalsStatus? status, int page = 1, int rows = 15)
        {
            return
                Json(FinanceApiService.GetWithdrawalsRecords(startDate, endDate, status, page, rows).Result);
        }

        public ActionResult Withdrawals()
        {
            var dto = new WithdrawalsDTO();
            //绑定的银行卡
            var bank = GetBindBankAccount();
            if (bank != null)
            {
                var banklist = new List<BankAccountDTO>();
                banklist.Add(bank);
                dto.BankList = banklist;
            }
            //账户余额
            var money = GetPaymentBankAccount();
            if (money != null)
            {
                dto.AvailableBalance = (money.CFWinTotalAmount - money.CFWinFreezeAmount) / 100M;
            }

            var financeApiService = FinanceApiService;
            dto.TodayMax = financeApiService.GetTodayWithdrawAmount().Result;
            dto.Rule = financeApiService.GetWithdrawRule().Result;

            PayBaseParamDTO paramDTO = new PayBaseParamDTO();
            paramDTO.MemberId = TenantName;
            paramDTO.UserName = CurrentUserName;
            paramDTO.Timestamp = ConvertDateTimeToInt(DateTime.UtcNow);
            paramDTO.EncryptString = OtherUtilHelper.GetStrByModel(paramDTO, Tenant.PrivateEncryptKey);
            string postUrl = PayApiDomain + CPCNMethodConstants.SearchWithdrawAmt;
            var postData = OtherUtilHelper.GetPostData(paramDTO);
            var paymentReturn = SerializeHelper.FromJson<JsonPaymentReturnModel>(HttpWebRequestHelper.WebClientDownload(postUrl, postData).Item2).Result;
            if (paymentReturn != null && paymentReturn.Success)
            {
                var amt = paymentReturn.ReturnAmtData;
                dto.T0AvailableBalance = amt.T0CtAmtA00;
                dto.T1AvailbaleBalance = amt.T1CtAmtA00;
            }

            return View(dto);
        }

        [HttpPost]
        public JsonResult ApplyForWithdrawal(decimal amount, int Paymentdate)
        {
            if (amount <= 0)
            {
                return ThrowErrorJsonMessage(false, "提现金额不正确");
            }
            if (!(Paymentdate == 0 || Paymentdate == 1))
            {
                return ThrowErrorJsonMessage(false, "到账时间选择有误");
            }
            return GetServiceJsonResult(() =>
            {
                var paymentRecord = new PaymentApplyRecordDTO();
                var number = OtherUtilHelper.GetSerialNumber(SequenceNumberConstants.TX);
                var resultPlat = FinanceApiService.ApplyForWithdrawal(amount, number, null, "", Paymentdate);
                if (resultPlat.Result != null)
                {
                    if (resultPlat.Result.Item1)
                    {
                        OutTransactionDTO paramDTO = new OutTransactionDTO();
                        paramDTO.MemberId = TenantName;
                        paramDTO.UserName = CurrentUserName;
                        paramDTO.Usage = paymentRecord.Remark;
                        paramDTO.PaymentOrderId = number;
                        paramDTO.Amount = amount * 100; //单位为分
                        paramDTO.BalFlag = (Paymentdate + 1);
                        paramDTO.FeeAmount = decimal.Parse(resultPlat.Result.Item2) * 100; //单位为分
                        paramDTO.Timestamp = ConvertDateTimeToInt(DateTime.UtcNow);
                        paramDTO.EncryptString = OtherUtilHelper.GetStrByModel(paramDTO, Tenant.PrivateEncryptKey);
                        string postUrl = PayApiDomain + CPCNMethodConstants.OutTransaction;
                        var postData = OtherUtilHelper.GetPostData(paramDTO);
                        var paymentReturn = SerializeHelper.FromJson<JsonPaymentReturnModel>(HttpWebRequestHelper.WebClientDownload(postUrl, postData).Item2).Result;
                        if (paymentReturn != null)
                        {
                            var result = FinanceApiService.ApplyForWithdrawal(amount, number, paymentReturn.Success, paymentReturn.ErrorMessage, Paymentdate);
                            if (result.Result != null)
                            {
                                if (resultPlat.Result.Item1)
                                {
                                    return true;
                                }
                                throw new BusinessPromptException(result.Result.Item2);
                            }
                        }
                        else
                        {
                            Logger.LogError("提现接口对象返回空");
                        }
                    }
                    else
                    {
                        throw new BusinessPromptException(resultPlat.Result.Item2);
                    }
                }
                return false;

            });

        }

        #endregion

        #region 系统升级

        public ActionResult UpdateApplication()
        {
            return View();
        }

        public ActionResult SeeUpdateFee()
        {
            return View();
        }

        public JsonResult SubmitApplication(List<ApplicationInfo> model)
        {
            return GetServiceJsonResult(() =>
            {
                //发送短信
                string smsMessage = string.Format("【财富共赢】{0}向贵司提交了增值服务申请，请前往签署服务协议。", TenantDisplayName);
                StorageQueueService.InsertSmsQueue(new SmsInfo() { Phone = new List<long> { long.Parse("1") }, SmsContent = smsMessage, Type = SmsType.Notice, Tenant = TenantName });
                //存在自选的项目去生成合同
                if (model.Any())
                {
                    string postUrl = GlobalConfig.EconWebDomain + "CurrencySign/" + "MakeAddValueServiceContract";
                    var postData = OtherUtilHelper.GetPostData(model);
                    var result = HttpWebRequestHelper.WebClientDownload(postUrl, postData);
                    return SerializeHelper.FromJson<bool>(result.Item2);
                }
                return true;
            });
        }

        public ActionResult SystemUpdate()
        {
            ViewBag.Version = Tenant.Version;

            return View();
        }

        /// <summary>
        /// 是否有缴费记录
        /// </summary>
        /// <returns></returns>
        public JsonResult CheckHavePaymentRecord()
        {
            return GetServiceJsonResult(() =>
            {
                return PayableService.CheckPayable(PayableSource.SystemFee);
            });
        }

        /// <summary>
        /// 插入一条付款记录
        /// </summary>
        /// <returns></returns>
        public JsonResult SetPayable(int version)
        {
            return GetServiceJsonResult(() =>
            {
                var newVersion = ((TenantVersion)version).ToDescription();
                var oldVersion = Tenant.Version.ToDescription();
                var title = string.Format("{0}系统升级请求", Tenant.TenantName);
                var content = string.Format("您好！租户（{0}，租户号：{1}）向您发出了升级为（{2}）的请求，请密切关注升级费用的进账并及时处理，谢谢！", Tenant.TenantDisplayName, Tenant.TenantName, newVersion);
                
                var orderAmount = 36000;
                var isSuccess = PayableService.AddPayable(new List<PayableDTO> { new PayableDTO { Type= PayableType.AccountsPayable,Source = PayableSource.SystemFee, OrderAmount= orderAmount,PayableAmount=orderAmount, PayableNumber=OtherUtilHelper.GetSerialNumber(SequenceNumberConstants.PaymentNo),StartDate=DateTime.UtcNow.Date,  Customer = PlatformAccountName,
                        CustomerTenant = PlatformAccountNum, Remark="系统升级费"} });
                return isSuccess;
            });
        }

        #endregion

        #region 交易流水

        public ActionResult TransactionFlow()
        {
            ViewBag.TypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<CashType>();
            ViewBag.AccountList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<TradingAccount>();

            return View();
        }
        public JsonResult LoadTransactionFlows(CashType? type, TradingAccount? account, DateTime? startDate, DateTime? endDate, int page = 1, int rows = 10)
        {
            var data = FinanceApiService.LoadTransactionFlows(type, account, startDate, endDate, page, rows);
            if (data.Result == null)
            {
                return Json(new PaginatedBaseDTO<CashUsageDetailDTO>(1, rows, 0, new List<CashUsageDetailDTO>()));
            }
            return Json(data.Result);
        }

        public JsonResult LoadCashReceipts(string customer, string num, DateTime? startDate, DateTime? endDate, int page = 1, int rows = 10)
        {
            return
                Json(FinanceApiService.GetCashReceipts(customer, num, startDate, endDate, page, rows).Result);
        }

        public JsonResult LoadCashExpenditure(string vendor, string num, DateTime? startDate, DateTime? endDate, int page = 1, int rows = 10)
        {
            return Json(FinanceApiService.GetCashExpenditure(vendor, num, startDate, endDate, page, rows).Result);
        }

        public JsonResult LoadUsedBill(string vendor, string num, DateTime? startDate, DateTime? endDate, int page = 1, int rows = 10)
        {
            return Json(FinanceApiService.GetUsedBill(vendor, num, startDate, endDate, page, rows).Result);
        }

        public JsonResult LoadGetBill(string customer, string num, DateTime? startDate, DateTime? endDate, int page = 1, int rows = 10)
        {
            return Json(FinanceApiService.GetBill(customer, num, startDate, endDate, page, rows).Result);
        }

        #endregion

        #region  应收应付

        /// <summary>
        /// 应收款汇总、应付款汇总
        /// </summary>
        /// <returns></returns>
        public JsonResult QueryReceivablesAndPayable()
        {
            //需要加上融资系统的数据
            var receivableAmount = 0M;
            var payableAmount = 0M;
            try
            {
                receivableAmount = ReceivableService.GetReceivableAmount();
                payableAmount = PayableService.GetPayableAmount();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
            }
            var result = new ServiceResult<Tuple<decimal, decimal>>(ServiceResultType.Success,
                    string.Empty, new Tuple<decimal, decimal>(payableAmount, receivableAmount));
            return Json(result);

        }

        public JsonResult LoadLogs(string num, bool payable)
        {
            return GetServiceJsonResult(() =>
            {
                if (payable)
                    return PayableService.LoadLogs(num);
                return ReceivableService.LoadLogs(num);
            });
        }

        #region 应收

        public ActionResult Receivables(string orderId, ReceivableSource? source)
        {
            ViewBag.OrderId = orderId;
            ViewBag.Source = source;
            ViewBag.SourceList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<ReceivableSource>();
            return View();
        }

        public JsonResult LoadReceivableTop5()
        {
            var result = new List<ReceivableDTO>();
            try
            {
                var shopReceivable = ReceivableService.LoadTop5();
                result = shopReceivable.OrderBy(m => m.StartDate).Take(5).ToList();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
            }

            return Json(new ServiceResult<List<ReceivableDTO>>(ServiceResultType.Success, "", result));
        }

        
        public JsonResult LoadReceivables(string orderId, ReceivableType? type, ReceivableSource? source, DateTime? startDate, DateTime? endDate, int page = 1, int rows = 15, string order = "asc")
        {
            try
            {
                var data = ReceivableService.GetReceivableByFilter(orderId, type, source, startDate, endDate, page, rows, order);
                var filteData = new List<ReceivableDTO>();
                if (order == "asc")
                    filteData = data.OrderBy(m => m.StartDate).ToList();
                else
                    filteData = data.OrderByDescending(m => m.StartDate).ToList();
                var result = new PaginatedBaseDTO<ReceivableDTO>(page, rows, filteData.Count,
                    filteData.Skip((page - 1) * rows).Take(rows).ToList());
                return Json(result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, ex);
            }
            return Json(new PaginatedBaseDTO<ReceivableDTO>(page, rows, 0, new List<ReceivableDTO>()));
        }

        public ActionResult ExportReceivables(string orderId, ReceivableType? type, ReceivableSource? source, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var stream = ReceivableService.ExportReceivables(orderId, type, source, startDate, endDate);
                var title = string.Empty;
                if (type.HasValue)
                    title = type.Value.ToDescription();
                if (source.HasValue)
                    title += (string.IsNullOrWhiteSpace(title) ? "" : "-") + source.Value.ToDescription();
                var fileName = "应收汇总" + title + ".xls";
                var contentType = MimeTypeHelper.GetMineType(DocFormat.Xls);
                return File(stream, contentType, fileName);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, ex);
            }
            return Content("导出出现异常");
        }

        public JsonResult RemindBuyerPayment(int id, string customerTenant, bool isLocal)
        {
            return GetServiceJsonResult(() =>
            {
                var receivable = ReceivableService.FindById(id);
                if (receivable == null)
                    return false;
                return true;
            });
        }

        #region 应收待处理

        public ActionResult PendingReceivables()
        {
            ViewBag.SourceList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<ReceivableSource>();
            ViewBag.TypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<PaymentType>();

            return View();
        }

        public JsonResult LoadPendingReceivables(string orderId, ReceivableSource? source, PaymentType? type, DateTime? startDate, DateTime? endDate, int page = 1, int rows = 15)
        {
            var count = 0;
            var items = new List<PendingReceivablesDTO>();
            try
            {
                var result = PaymentRecordService.LoadPagenatedPendingReceivables(orderId, source, type, startDate, endDate, page, rows);
                count = result.total;
                items = result.rows.OrderBy(m => m.CreateDateTime).ToList();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, ex);
            }
            return Json(new PaginatedBaseDTO<PendingReceivablesDTO>(page, rows, count, items));
        }

        #endregion

        public ActionResult ConfirmReceivables(int id)
        {
            var dto = ReceivableService.FindById(id);
            return View(dto);
        }

        #endregion

        #region 应付
        public ActionResult Payables(string orderId, PayableSource? source)
        {
            ViewBag.OrderId = orderId;
            ViewBag.Source = source;
            ViewBag.SourceList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<PayableSource>();
            return View();
        }

        public JsonResult LoadPayableTop5()
        {
            var result = new List<PayableDTO>();
            try
            {
                var shopPayable = PayableService.LoadTop5();
                result = shopPayable.OrderBy(m => m.StartDate).Take(5).ToList();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, ex);
            }

            return Json(new ServiceResult<List<PayableDTO>>(ServiceResultType.Success, "", result));
        }

        public JsonResult LoadPayables(string orderId, PayableType? type, PayableSource? source, DateTime? startDate, DateTime? endDate, int page = 1, int rows = 15, string order = "asc")
        {
            try
            {
                var data = PayableService.GetPayableByFilter(orderId, type, source, startDate, endDate, order);
                var filteData = new List<PayableDTO>();
                if (order == "asc")
                    filteData = data.OrderBy(m => m.StartDate).ToList();
                else
                    filteData = data.OrderByDescending(m => m.StartDate).ToList();
                var result = new PaginatedBaseDTO<PayableDTO>(page, rows, filteData.Count(),
                       filteData.Skip((page - 1) * rows).Take(rows).ToList());
                return Json(result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, ex);
            }
            return Json(new PaginatedBaseDTO<PayableDTO>(page, rows, 0, new List<PayableDTO>()));
        }

        public ActionResult ExportPayables(string orderId, PayableType? type, PayableSource? source, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var stream = PayableService.ExportPayables(orderId, type, source, startDate, endDate, null);
                var title = string.Empty;
                if (type.HasValue)
                    title = '-' + type.Value.ToDescription();
                if (source.HasValue)
                    title += "-" + source.Value.ToDescription();
                var fileName = "应付汇总" + title + ".xls";
                var contentType = MimeTypeHelper.GetMineType(DocFormat.Xls);
                return File(stream, contentType, fileName);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, ex);
            }
            return Content("导出出现异常");
        }

        public JsonResult CancelPayable(int id, PayableSource souce)
        {
            return GetServiceJsonResult(() =>
            {
                if (id < 1)
                    return false;
                if (souce == PayableSource.RefundCautionMoney)
                    return PayableService.CancelPayableWithCautionMoney(id);
                return PayableService.Cancel(id);
            });
        }

        #region 应付待处理

        public ActionResult PendingPayables()
        {
            ViewBag.SourceList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<PayableSource>();
            ViewBag.TypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<PaymentType>();

            return View();
        }

        public JsonResult LoadPendingPayables(string orderId, PayableSource? source, PaymentType? type, DateTime? startDate, DateTime? endDate, int page = 1, int rows = 15)
        {
            var count = 0;
            var items = new List<PendingPayablesDTO>();
            try
            {
                var result = PaymentRecordService.LoadPagenatedPendingPayables(CurrentUserName, orderId, source, type, startDate, endDate, page, rows);
                count = result.total;
                items = result.rows;
            }
            catch (Exception ex)
            {

            }

            return Json(new PaginatedBaseDTO<PendingPayablesDTO>(page, rows, count, items));
        }

        public JsonResult GetVoucherInfo(int id)
        {
            return GetServiceJsonResult(() =>
            {
                return PaymentRecordService.GetVoucherInfo(id);
            });
        }

        #endregion

        #endregion

        #region 资金总额

        public ActionResult CapitalSummary()
        {
            ViewBag.Summary = GetAssetSummary();
            return View();
        }


        #endregion

        #endregion

        #region 冻结资金

        public JsonResult LoadFrozenRecords(int page = 1, int rows = 10, string order = "asc")
        {
            var data = FinanceApiService.LoadFrozenRecords(page, rows, order);
            var result = new List<FrozenRecordDTO>();
            if (data.Result.rows != null && data.Result.rows.Any())
            {
                data.Result.rows.ForEach(k =>
                {
                    result.Add(new FrozenRecordDTO
                    {
                        OrderId = k.Key,
                        //OrderAmount = k.order,
                        //ConsumptionDate = so.Item2,
                        ConsumptionMoney = decimal.Parse(k.Value).ToString("C")
                    });
                });

            }
            return Json(new PaginatedBaseDTO<FrozenRecordDTO>(page, rows, data.Result.total, result));
        }

        #endregion
    }

    public class CapitalManagerPaginatedBaseDTO<T> : EntityBaseDTO where T : EntityBaseDTO
    {
        public CapitalManagerPaginatedBaseDTO(int pageIndex, int pageSize, int total, List<T> rows, decimal countMoney)
        {
            this.pageIndex = pageIndex;
            this.pageSize = pageSize;
            this.total = total;
            this.rows = rows;
            this.countMoney = countMoney.ToString("N");
        }

        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public int total { get; set; }
        public List<T> rows { get; set; }

        public string countMoney { get; set; }
    }
}