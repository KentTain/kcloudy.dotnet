using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using KC.Service.EFService;
using KC.Service.WebApiService.Business;
using KC.DataAccess.Pay.Repository;
using KC.Service.DTO.Pay;
using KC.Service.DTO;
using KC.Model.Pay;
using KC.Database.IRepository;
using KC.Framework.Tenant;
using KC.Database.EFRepository;
using KC.Framework.Base;
using KC.Service.Pay.WebApiService.Platform;
using KC.Enums.Pay;
using KC.Framework.Extension;
using KC.Service.Pay.Response;
using KC.Common.HttpHelper;
using KC.Common;
using KC.Framework.SecurityHelper;
using Microsoft.Extensions.Logging;

namespace KC.Service.Pay
{
    public interface IPaymentService : IEFService
    {
        bool SaveOpenAccount(OpenAccountDTO openAccountDTO);

        PaymentReturnModel Payment(bool isAdminPortal, string userId, int configId, int orderAmount, string billNo, string orderTime,
            string goodsName = null, string remark = "");
        PaymentReturnModel PaymentFrontUrl(string userId, Object response, int sign);
        PaymentReturnModel PaymentBackUrl(string userId, Object response, int sign);
        string PaymentBack(string userId, Object response, int sign);

        #region 支付记录：OnlinePaymentRecord
        List<OnlinePaymentRecord> QueryCPCNChargeLastHourNotComplete();
        List<OnlinePaymentRecord> QueryCBSPayLastDayNotComplete();

        PaginatedBaseDTO<OnlinePaymentRecordDTO> GetOnlinePaymentRecordByPage(DateTime? beginTime, DateTime? endTime, string memberId, string peeMemberId, int pageIndex, int pageSize);
        PaginatedBaseDTO<OnlinePaymentRecordDTO> GetOnlinePaymentRecordsByFilter(DateTime? beginTime, DateTime? endTime, bool isExpended = false, string memberId = "", List<string> memberIds = null, int pageIndex = 1, int pageSize = 10);
        PaginatedBaseDTO<OnlinePaymentRecordDTO> GetOnlinePaymentRecordsTop10(string memberId, int pageIndex = 1, int pageSize = 10);

        OnlinePaymentRecordDTO GetOnlinePaymentRecordByBankNumber(string bankNumber);
        OnlinePaymentRecordDTO GetOnlinePaymentRecordByPaymentId(string paymentOrderId);
        OnlinePaymentRecordDTO GetOnlinePaymentRecordByOrderNoAndAmount(string orderNo, Decimal amount, string bankName);
        OnlinePaymentRecordDTO GetCBSOnlinePaymentRecordByTime(string payDateTime, string peeMemberId);

        bool IsOnlinePaymentSuccess(string paymentOrderId);

        bool UpdatePaymentRecord(OnlinePaymentRecord entity);
        bool AddPaymentRecord(OnlinePaymentRecord entity);
        OnlinePaymentRecord GetByPaymentOrderId(string paymentOrderId);
        OnlinePaymentRecord GetByPaymentOrderIdAndOrderAmount(string paymentOrderId, decimal orderAmount);
        #endregion

        #region 银行卡：BankAccount
        List<BankAccount> GetBankAccountByMemberId(string memberId, string searchInfo);
        BankAccountDTO GetBankAccountById(int id, string memberId);
        bool RemoveBankById(int BankId);
        bool SaveBankAccount(BankAccountDTO bankAccount);
        List<BankAccount> GetMemberBankByBankNumber(string bankNumber);

        BankAccountDTO GetBindBankAccount(string memberId, ThirdPartyType type = ThirdPartyType.CPCNConfigSign);
        bool SaveBankAuthenticationAppliction(BankAuthenticationApplicationDTO bankAuthenticationApplicationDTO);
        bool BindBankAccount(BindBankAccountDTO bindBankAccountDTO);

        Dictionary<string, List<ThirdPartyType>> GetMembersPaymentAccountTypes(List<string> memberIds);
        #endregion

        #region 虚拟户：PaymentBankAccount

        List<PaymentBankAccountDTO> GetPaymentAccounts(string memberId);

        PaymentBankAccountDTO GetPlatformAccount(ThirdPartyType thirdPartyType);
        PaymentBankAccountDTO GetPaymentBankAccountByMemberId(string memberId, ThirdPartyType type = ThirdPartyType.CPCNConfigSign);
        PaymentBankAccountDTO GetPaymentBankAccountByAccountNum(string accountNum, ThirdPartyType type = ThirdPartyType.CPCNConfigSign);

        bool SavePaymentBankAccount(PaymentBankAccountDTO paymentBankAccount);

        #endregion

        #region 支付设置
        PaymentInfoDTO GetPaymentInfo(string memberId, ThirdPartyType thirdPartyType);

        bool CheckDefalutPayOpen(string memberId);
        bool IsOpenPayment(string tenantName, ThirdPartyType thirdPartyType);

        bool SavePaymentPhoneById(int paymentInfoId, string phone);
        bool SavePaymentPasswordById(int paymentInfoId, string password);
        #endregion

        #region 银行接口调用记录：PaymentTradeRecord
        PaymentTradeRecord GetTradeRecordsByLastSuccessRecord(string memberId, PaymentOperationType operationType, int referenceId);
        bool SavePaymentTradeRecord(PaymentTradeRecordDTO tradeRecord);
        #endregion

        #region 支付接口
        bool SaveFreezeAmt(FreezeAmtDTO freezeAmtDTO);
        bool SaveOrderPay(OrderPayDTO orderPay);
        bool SaveInTransaction(InTransactionDTO paramDTO);
        bool SaveInTransactionSearch(InTransactionSearchDTO paramDTO);
        bool SaveReceiveNotice(ReceiveNoticeDTO paramDTO);
        bool OutTransaction(OutTransactionDTO paramDTO);
        #endregion

        Tuple<bool, List<CBSAccount>, string> GetCBSAccounts(bool isAdminPortal, string displayName, ConfigType payType);

        Tuple<bool, string> CBSPaymentMethod(bool isAdminPortal, string userId, string companyName, string cbsAccount, int configSign, int configState, ConfigType payType,
            int orderAmount, string orderId, CashType type = CashType.Transaction, PaymentRecordDTO paymentInfo = null);

        PaginatedBaseDTO<CashUsageDetailDTO> GetPaginatedCashUsageDetailByFilter(bool isPayee, string userId,
            int pageIndex, int pageSize,
            DateTime? startDate, DateTime? endDate, ref decimal countMoney);
    }

    public class PaymentService : EFServiceBase, IPaymentService
    {
        private readonly IMapper _mapper;

        /// <summary>
        /// ComAccountUnitOfWorkContext
        ///     use AccountUnitOfWorkContext.Commit() to save the context change(transaction).
        /// </summary>
        protected EFUnitOfWorkContextBase _unitOfWorkContext { get; private set; }

        private PaymentFactoryUtil _paymentFactoryUtil { get { return new PaymentFactoryUtil(this.Tenant, this); } }
        private IBankAccountRepository BankAccountRepository;
        private IPaymentBankAccountRepository PaymentBankAccountRepository;

        private IDbRepository<OnlinePaymentRecord> OnlinePaymentRecordRepository;
        private IDbRepository<PaymentTradeRecord> PaymentTradeRecordsRepository;
        private IDbRepository<PaymentInfo> PaymentInfoRepository;

        private IDbRepository<CashUsageDetail> CashUsageDetailRepository;
        private readonly IConfigApiService ConfigApiService;

        public PaymentService(
            Tenant tenant,
            IMapper mapper,
            IAccountApiService accountApiService,
            IConfigApiService configApiService,
            EFUnitOfWorkContextBase unitOfWorkContext,

            IBankAccountRepository docTemplateRepository,
            IPaymentBankAccountRepository paymentBankAccountRepository,
            IDbRepository<OnlinePaymentRecord> onlinePaymentRecordRepository,
            IDbRepository<PaymentTradeRecord> paymentTradeRecordsRepository,
            IDbRepository<PaymentInfo> paymentInfoRepository,
            IDbRepository<CashUsageDetail> cashUsageDetailRepository,

            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<PaymentService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfWorkContext = unitOfWorkContext;
            ConfigApiService = configApiService;

            this.BankAccountRepository = docTemplateRepository;
            this.PaymentBankAccountRepository = paymentBankAccountRepository;
            this.OnlinePaymentRecordRepository = onlinePaymentRecordRepository;
            this.PaymentTradeRecordsRepository = paymentTradeRecordsRepository;
            this.PaymentInfoRepository = paymentInfoRepository;
            this.CashUsageDetailRepository = cashUsageDetailRepository;
        }

        public PaymentReturnModel Payment(bool isAdminPortal, string userId, int configId, int orderAmount, string billNo, string orderTime,
            string goodsName = null, string remark = "")
        {
            var configEntity = ConfigApiService.GetConfigById(configId);
            return _paymentFactoryUtil.Payment(configEntity, userId, isAdminPortal, orderAmount, billNo, orderTime,
                goodsName, remark);
        }

        public PaymentReturnModel PaymentFrontUrl(string userId, Object response, int sign)
        {
            var configEntity = ConfigApiService.GetConfigsByTypeAndSign(ConfigType.PaymentMethod, sign);
            return _paymentFactoryUtil.PaymentFrontUrl(userId, configEntity, response);
        }

        public PaymentReturnModel PaymentBackUrl(string userId, Object response, int sign)
        {
            var configEntity = ConfigApiService.GetConfigsByTypeAndSign(ConfigType.PaymentMethod, sign);
            return _paymentFactoryUtil.PaymentBackUrl(userId, configEntity, response);
        }

        public string PaymentBack(string userId, object response, int sign)
        {
            var paymentReturnModel = PaymentBackUrl(userId, response, sign);
            if (paymentReturnModel.Success)
            {
                return "";
            }
            return paymentReturnModel.ErrorMessage;
        }

        /// <summary>
        /// 保存开虚拟户信息
        /// </summary>
        /// <param name="openAccountDTO"></param>
        /// <returns></returns>
        public bool SaveOpenAccount(OpenAccountDTO openAccountDTO)
        {
            if (openAccountDTO.IsSuccess)
            {
                var bankAccount = _mapper.Map<PaymentBankAccount>(openAccountDTO.CPCNBankAccount);
                PaymentBankAccountRepository.Add(bankAccount, false);
                var paymentInfo = _mapper.Map<PaymentInfo>(openAccountDTO.PaymentInfo);
                PaymentInfoRepository.Add(paymentInfo, false);
                //关联Id赋值到流水表
                openAccountDTO.PaymentTradeRecord.ReferenceId = bankAccount.Id;
            }
            var paymentTradeRecord = _mapper.Map<PaymentTradeRecord>(openAccountDTO.PaymentTradeRecord);
            PaymentTradeRecordsRepository.Add(paymentTradeRecord, false);

            return _unitOfWorkContext.Commit() > 0;
        }

        #region 支付记录：OnlinePaymentRecord

        /// <summary>
        /// 查询半天内中金入金数据
        /// </summary>
        /// <returns></returns>
        public List<OnlinePaymentRecord> QueryCPCNChargeLastHourNotComplete()
        {
            var startDate = DateTime.UtcNow.AddHours(-12);
            //获取一个小时之前，入金 payResult为0或者正在处理。次数小于5次，下一个查询时间小于当前时间的记录
            var onlinePaymentList = OnlinePaymentRecordRepository.FindAll(m => (m.OrderNo == "入金" || m.OrderNo == "扫码入金") && !m.IsDeleted &&
            (m.PayResult == null || m.PayResult == "0" || m.PayResult == "3") && m.CreatedDate >= startDate
            && m.SearchCount <= 5 && m.NextSearchTime <= DateTime.UtcNow).ToList();
            return onlinePaymentList;
        }

        public List<OnlinePaymentRecord> QueryCBSPayLastDayNotComplete()
        {
            var startDate = DateTime.UtcNow.AddHours(-24);
            //获取一天之前，CBS支付 payResult为0或者正在处理。次数小于5次，下一个查询时间小于当前时间的记录
            var onlinePaymentList = OnlinePaymentRecordRepository.FindAll(m => m.PaymentType == ThirdPartyType.CMBCBSConfigSign && !m.IsDeleted &&
            (m.PayResult == null || m.PayResult == "0" || m.PayResult == "3") && m.CreatedDate >= startDate
            && m.SearchCount <= 5 && m.NextSearchTime <= DateTime.UtcNow).ToList();
            return onlinePaymentList;
        }

        /// <summary>
        /// 根据条件分页获取OnlinePaymentRecord数据
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="memberId"></param>
        /// <param name="peeMemberId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PaginatedBaseDTO<OnlinePaymentRecordDTO> GetOnlinePaymentRecordByPage(DateTime? beginTime, DateTime? endTime, string memberId, string peeMemberId, int pageIndex, int pageSize)
        {
            Expression<Func<OnlinePaymentRecord, bool>> predicate = m => m.PayResult == "1" && !m.IsDeleted;
            if (beginTime.HasValue)
            {
                predicate = predicate.And(m => m.CreatedDate >= beginTime);
            }
            if (endTime.HasValue)
            {
                predicate = predicate.And(m => m.CreatedDate <= endTime);
            }
            if (!string.IsNullOrWhiteSpace(memberId))
            {
                predicate = predicate.And(m => m.MemberId.Equals(memberId, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrWhiteSpace(peeMemberId))
            {
                predicate = predicate.And(m => m.PeeMemberId.Equals(peeMemberId, StringComparison.OrdinalIgnoreCase));
            }

            var data = OnlinePaymentRecordRepository.FindPagenatedListWithCount(pageIndex, pageSize, predicate, m => m.ModifiedDate, false);
            var totalnum = data.Item1;
            var rows = _mapper.Map<List<OnlinePaymentRecordDTO>>(data.Item2);
            var result = new PaginatedBaseDTO<OnlinePaymentRecordDTO>(pageIndex, pageSize, totalnum, rows);
            return result;
        }
        public PaginatedBaseDTO<OnlinePaymentRecordDTO> GetOnlinePaymentRecordsByFilter(DateTime? beginTime, DateTime? endTime, bool isExpended = false, string memberId = "", List<string> memberIds = null, int pageIndex = 1, int pageSize = 10)
        {
            var totalnum = 0;
            var rows = new List<OnlinePaymentRecordDTO>();
            Expression<Func<OnlinePaymentRecord, bool>> predicate = m => !m.IsDeleted;
            predicate = predicate.And(m => m.PayResult == "1");
            predicate = predicate.And(m => m.MemberId != "" && m.PeeMemberId != "");
            predicate = predicate.And(m => m.MemberId != null && m.PeeMemberId != null);
            if (beginTime.HasValue)
            {
                predicate = predicate.And(m => m.CreatedDate >= beginTime);
            }
            if (endTime.HasValue)
            {
                var end = endTime.Value.AddDays(1);
                predicate = predicate.And(m => m.CreatedDate < end);
            }
            if (isExpended)
            {
                if (!string.IsNullOrWhiteSpace(memberId))
                {
                    predicate = predicate.And(m => m.MemberId.Equals(memberId, StringComparison.OrdinalIgnoreCase));
                }
                if (memberIds == null)
                {
                    totalnum = 0;
                    rows = new List<OnlinePaymentRecordDTO>();
                    var nullResult = new PaginatedBaseDTO<OnlinePaymentRecordDTO>(pageIndex, pageSize, totalnum, rows);
                    return nullResult;
                }
                if (memberIds.Any())
                {
                    predicate = predicate.And(m => memberIds.Contains(m.PeeMemberId));
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(memberId))
                {
                    predicate = predicate.And(m => m.PeeMemberId.Equals(memberId, StringComparison.OrdinalIgnoreCase));
                }
                if (memberIds == null)
                {
                    totalnum = 0;
                    rows = new List<OnlinePaymentRecordDTO>();
                    var nullResult = new PaginatedBaseDTO<OnlinePaymentRecordDTO>(pageIndex, pageSize, totalnum, rows);
                    return nullResult;
                }
                if (memberIds.Any())
                {
                    predicate = predicate.And(m => memberIds.Contains(m.MemberId));
                }
            }

            var data = OnlinePaymentRecordRepository.FindPagenatedListWithCount(pageIndex, pageSize, predicate, m => m.ModifiedDate, false);
            totalnum = data.Item1;
            rows = _mapper.Map<List<OnlinePaymentRecordDTO>>(data.Item2);

            var result = new PaginatedBaseDTO<OnlinePaymentRecordDTO>(pageIndex, pageSize, totalnum, rows);
            return result;
        }

        public PaginatedBaseDTO<OnlinePaymentRecordDTO> GetOnlinePaymentRecordsTop10(string memberId, int pageIndex = 1, int pageSize = 10)
        {
            var totalnum = 0;
            var rows = new List<OnlinePaymentRecordDTO>();
            Expression<Func<OnlinePaymentRecord, bool>> predicate = m => !m.IsDeleted;
            predicate = predicate.And(m => m.PayResult == "1");
            predicate = predicate.And(m => m.MemberId != "" && m.PeeMemberId != "");
            predicate = predicate.And(m => m.MemberId != null && m.PeeMemberId != null);
            predicate = predicate.And(m => m.MemberId.Equals(memberId, StringComparison.OrdinalIgnoreCase) || m.PeeMemberId.Equals(memberId, StringComparison.OrdinalIgnoreCase));

            var data = OnlinePaymentRecordRepository.FindPagenatedListWithCount(pageIndex, pageSize, predicate, m => m.ModifiedDate, false);
            totalnum = data.Item1;
            rows = _mapper.Map<List<OnlinePaymentRecordDTO>>(data.Item2);

            var result = new PaginatedBaseDTO<OnlinePaymentRecordDTO>(pageIndex, pageSize, totalnum, rows);
            return result;
        }

        public OnlinePaymentRecordDTO GetOnlinePaymentRecordByBankNumber(string bankNumber)
        {
            var data = OnlinePaymentRecordRepository.FindAll(m => m.BankNumber == bankNumber).OrderByDescending(m => m.Id).FirstOrDefault();
            return _mapper.Map<OnlinePaymentRecordDTO>(data);
        }

        /// <summary>
        /// 根据支付订单Id查询支付记录
        /// </summary>
        /// <param name="paymentOrderId"></param>
        /// <returns></returns>
        public OnlinePaymentRecordDTO GetOnlinePaymentRecordByPaymentId(string paymentOrderId)
        {
            var data = OnlinePaymentRecordRepository.FindAll(m => m.PaymentOrderId == paymentOrderId).OrderByDescending(m => m.Id).FirstOrDefault();
            return _mapper.Map<OnlinePaymentRecordDTO>(data);
        }

        public OnlinePaymentRecordDTO GetOnlinePaymentRecordByOrderNoAndAmount(string orderNo, Decimal amount, string bankName)
        {
            var data = OnlinePaymentRecordRepository.FindAll(m => m.OrderNo == orderNo && m.PaymentAmount == amount && m.BankName.Contains(bankName)).OrderByDescending(m => m.Id).FirstOrDefault();
            return _mapper.Map<OnlinePaymentRecordDTO>(data);
        }

        /// <summary>
        /// 根据支付时间查询记录
        /// </summary>
        /// <param name="payDateTime"></param>
        /// <returns></returns>
        public OnlinePaymentRecordDTO GetCBSOnlinePaymentRecordByTime(string payDateTime, string peeMemberId)
        {
            var data = OnlinePaymentRecordRepository.FindAll(m => m.PayDatetime == payDateTime
            && m.PaymentType == ThirdPartyType.CMBCBSConfigSign && m.PeeMemberId == peeMemberId).OrderByDescending(m => m.Id).FirstOrDefault();
            return _mapper.Map<OnlinePaymentRecordDTO>(data);
        }

        /// <summary>
        /// 根据paymentOrderId判断是否支付成功
        /// </summary>
        /// <param name="paymentOrderId"></param>
        /// <returns></returns>
        public bool IsOnlinePaymentSuccess(string paymentOrderId)
        {
            return OnlinePaymentRecordRepository.FindAll(m => m.PaymentOrderId == paymentOrderId && m.PayResult == "1").Count > 0;
        }

        /// <summary>
        /// 添加支付信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool AddPaymentRecord(OnlinePaymentRecord entity)
        {
            return OnlinePaymentRecordRepository.Add(entity);
        }
        /// <summary>
        /// 银行回调接口，更新支付结果
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool UpdatePaymentRecord(OnlinePaymentRecord entity)
        {
            //先去更新平台的数据
            PaymentApiService _paymentApiService = new PaymentApiService(Tenant, HttpClientFactory, null);
            //查出平台的数据后赋值
            var platformEntity = _paymentApiService.GetPlatPaymentRecord(entity).Result;
            platformEntity.ErrorCode = entity.ErrorCode;
            platformEntity.PayResult = entity.PayResult;
            platformEntity.VerifyResult = entity.VerifyResult;
            platformEntity.BankName = entity.BankName;
            platformEntity.BankNumber = entity.PaymentOrderId;
            platformEntity.ReturnDatetime = entity.ReturnDatetime;
            platformEntity.PayDatetime = entity.PayDatetime;
            platformEntity.PaymentAmount = entity.PaymentAmount;
            platformEntity.ModifiedBy = entity.ModifiedBy;
            //更新平台的数据
            var updatePaymenResult = _paymentApiService.UpdatePaymentRecord(platformEntity);
            if (updatePaymenResult.Result)
            {
                //更新租户库的数据
                return OnlinePaymentRecordRepository.Modify(entity);
            }
            return false;
        }

        public OnlinePaymentRecord GetByPaymentOrderId(string paymentOrderId)
        {
            return OnlinePaymentRecordRepository.FindAll(m => m.PaymentOrderId.Equals(paymentOrderId)).FirstOrDefault();
        }
        /// <summary>
        /// 查询支付信息
        /// </summary>
        /// <param name="paymentOrderId"></param>
        /// <param name="orderAmount"></param>
        /// <returns></returns>
        public OnlinePaymentRecord GetByPaymentOrderIdAndOrderAmount(string paymentOrderId, decimal orderAmount)
        {
            return OnlinePaymentRecordRepository.FindAll(m => m.PaymentOrderId.Equals(paymentOrderId) && m.OrderAmount == orderAmount).FirstOrDefault();
        }
        #endregion

        #region 银行卡管理相关：BankAccount

        public List<BankAccount> GetBankAccountByMemberId(string memberId, string searchInfo)
        {
            return BankAccountRepository.GetBankAccountByMemberId(memberId, searchInfo == null ? "" : searchInfo);
        }

        /// <summary>
        /// 根据Id获取租户银行信息表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BankAccountDTO GetBankAccountById(int id, string memberId)
        {
            var data = BankAccountRepository.FindAll(m => m.Id == id && m.MemberId == memberId).FirstOrDefault();
            return _mapper.Map<BankAccountDTO>(data);
        }
        public BankAccountDTO GetBindBankAccount(string memberId, ThirdPartyType type = ThirdPartyType.CPCNConfigSign)
        {
            var data = BankAccountRepository.GetByFilter(m => m.BankState == BankAccountState.Binding && !m.IsDeleted && m.MemberId == memberId);
            return _mapper.Map<BankAccountDTO>(data);
        }
        public bool RemoveBankById(int BankId)
        {
            var bankAccount = BankAccountRepository.FindAll(m => m.Id == BankId).FirstOrDefault();
            if (bankAccount == null)
            {
                return false;
            }
            bankAccount.IsDeleted = true;
            return BankAccountRepository.Modify(bankAccount);
        }

        public bool SaveBankAccount(BankAccountDTO bankAccount)
        {
            var data = _mapper.Map<BankAccount>(bankAccount);
            if (data.Id > 0)
            {
                return BankAccountRepository.Modify(data);
            }
            return BankAccountRepository.Add(data);
        }

        public List<BankAccount> GetMemberBankByBankNumber(string bankNumber)
        {
            return BankAccountRepository.FindAll(m => m.AccountNum == bankNumber && !m.IsDeleted).ToList();
        }

        /// <summary>
        /// 中金支付 提交银行验证申请
        /// </summary>
        /// <param name="bankAuthenticationApplicationDTO"></param>
        /// <returns></returns>
        public bool SaveBankAuthenticationAppliction(BankAuthenticationApplicationDTO bankAuthenticationApplicationDTO)
        {
            var paymentTradeRecord = _mapper.Map<PaymentTradeRecord>(bankAuthenticationApplicationDTO.PaymentTradeRecord);
            //if (bankAuthenticationApplicationDTO.IsSuccess)
            //{
            var bankAccount = _mapper.Map<BankAccount>(bankAuthenticationApplicationDTO.BankAccount);
            BankAccountRepository.Modify(bankAccount, new[] { "BankState", "ModifiedBy", "ModifiedDate" }, false);
            //}
            PaymentTradeRecordsRepository.Add(paymentTradeRecord, false);

            return _unitOfWorkContext.Commit() > 0;
        }

        /// <summary>
        /// 绑定结算卡
        /// </summary>
        /// <param name="bindBankAccountDTO"></param>
        /// <returns></returns>
        public bool BindBankAccount(BindBankAccountDTO bindBankAccountDTO)
        {
            if (bindBankAccountDTO.IsSuccess)
            {
                var bankAccount = _mapper.Map<BankAccount>(bindBankAccountDTO.BankAccount);
                BankAccountRepository.Modify(bankAccount, false);
                var payBankAccount = _mapper.Map<PaymentBankAccount>(bindBankAccountDTO.PaymentBankAccount);
                PaymentBankAccountRepository.Modify(payBankAccount, false);
            }
            var payRecord = _mapper.Map<PaymentTradeRecord>(bindBankAccountDTO.PaymentTradeRecord);
            PaymentTradeRecordsRepository.Add(payRecord, false);

            return _unitOfWorkContext.Commit() > 0;
        }
        #endregion

        #region 虚拟户：PaymentBankAccount
        /// <summary>
        /// 获取的平台账户
        /// </summary>
        /// <param name="thirdPartyType"></param>
        /// <returns></returns>
        public PaymentBankAccountDTO GetPlatformAccount(ThirdPartyType thirdPartyType)
        {
            var data = PaymentBankAccountRepository.GetByFilter(m => m.IsPlatformAccount && m.PaymentType == thirdPartyType && !m.IsDeleted);
            return _mapper.Map<PaymentBankAccountDTO>(data);
        }

        /// <summary>
        /// 根据租户Id获取支付账户信息
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public PaymentBankAccountDTO GetPaymentBankAccountByMemberId(string memberId, ThirdPartyType type = ThirdPartyType.CPCNConfigSign)
        {
            var data = PaymentBankAccountRepository.GetBankAccountByMemberId(memberId, type);
            return _mapper.Map<PaymentBankAccountDTO>(data);
        }

        /// <summary>
        /// 根据账户号码获取账户信息
        /// </summary>
        /// <param name="accountNum"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public PaymentBankAccountDTO GetPaymentBankAccountByAccountNum(string accountNum, ThirdPartyType type = ThirdPartyType.CPCNConfigSign)
        {
            var data = PaymentBankAccountRepository.GetByFilter(m => m.AccountNo == accountNum && m.PaymentType == type);
            return _mapper.Map<PaymentBankAccountDTO>(data);
        }

        /// <summary>
        /// 根据租户Id获取账户信息列表
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public List<PaymentBankAccountDTO> GetPaymentAccounts(string memberId)
        {
            var data = PaymentBankAccountRepository.GetPaymentAccounts(memberId);
            return _mapper.Map<List<PaymentBankAccountDTO>>(data);
        }

        /// <summary>
        /// 更新中金支付账户信息表
        /// </summary>
        /// <returns></returns>
        public bool SavePaymentBankAccount(PaymentBankAccountDTO paymentBankAccount)
        {
            var data = _mapper.Map<PaymentBankAccount>(paymentBankAccount);
            return PaymentBankAccountRepository.Modify(data);
        }

        /// <summary>
        /// 根据传入租户Id获取租户已开通的支付体系类型
        /// </summary>
        /// <param name="memberIds"></param>
        /// <returns></returns>
        public Dictionary<string, List<ThirdPartyType>> GetMembersPaymentAccountTypes(List<string> memberIds)
        {
            var accounts = new List<PaymentBankAccount>();
            var dic = new Dictionary<string, List<ThirdPartyType>>();
            if (memberIds.Any())
            {
                accounts = PaymentBankAccountRepository.FindAll(m => memberIds.Contains(m.MemberId) && m.State != 2 && !m.IsDeleted).ToList();
            }
            if (accounts.Any())
            {
                var existUsers = accounts.Distinct(a => a.MemberId).Select(a => a.MemberId).ToList();
                existUsers.ForEach(u =>
                {
                    dic.Add(u, new List<ThirdPartyType>());
                    accounts.ForEach(a =>
                    {
                        if (a.MemberId.Equals(u, StringComparison.OrdinalIgnoreCase))
                        {
                            dic[u].Add(a.PaymentType);
                        }
                    });
                });
            }
            if (dic.Any())
            {
                return dic;
            }
            return null;
        }
        #endregion

        #region 支付设置
        /// <summary>
        /// 检查默认的支付是否开通
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public bool CheckDefalutPayOpen(string memberId)
        {
            return true;
        }

        /// <summary>
        /// 查找组织是否开通支付
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="thirdPartyType"></param>
        /// <returns></returns>
        public bool IsOpenPayment(string tenantName, ThirdPartyType thirdPartyType)
        {
            return PaymentInfoRepository.FindAll(m => m.TenantName == tenantName && !m.IsDeleted && m.PaymentType == thirdPartyType).Count > 0;
        }

        public PaymentInfoDTO GetPaymentInfo(string memberId, ThirdPartyType thirdPartyType)
        {
            var data = PaymentInfoRepository.GetByFilter(m => m.TenantName == memberId && !m.IsDeleted && m.PaymentType == thirdPartyType);

            return _mapper.Map<PaymentInfoDTO>(data);
        }

        public bool SavePaymentPhoneById(int paymentInfoId, string phone)
        {
            var paymentInfo = PaymentInfoRepository.GetByFilter(m => m.Id == paymentInfoId);
            if (paymentInfo == null)
            {
                return false;
            }
            paymentInfo.Phone = phone;
            return PaymentInfoRepository.Modify(paymentInfo);
        }

        public bool SavePaymentPasswordById(int paymentInfoId, string password)
        {
            var paymentInfo = PaymentInfoRepository.GetByFilter(m => m.Id == paymentInfoId);
            if (paymentInfo == null)
            {
                return false;
            }
            paymentInfo.TradePassword = password;
            return PaymentInfoRepository.Modify(paymentInfo);
        }
        #endregion

        #region 银行接口调用记录：PaymentTradeRecord
        /// <summary>
        /// 查询最后一条成功的流水记录
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="operationType"></param>
        /// <param name="referenceId"></param>
        /// <returns></returns>
        public PaymentTradeRecord GetTradeRecordsByLastSuccessRecord(string memberId, PaymentOperationType operationType, int referenceId)
        {
            return PaymentTradeRecordsRepository.FindAll(m => m.MemberId == memberId && m.OperationType == operationType && m.ReferenceId == referenceId && m.IsSuccess)
                .OrderByDescending(m => m.CreatedDate).FirstOrDefault();
        }

        /// <summary>
        /// 存交易流水
        /// </summary>
        /// <param name="tradeRecords"></param>
        /// <returns></returns>
        public bool SavePaymentTradeRecord(PaymentTradeRecordDTO tradeRecord)
        {
            var data = _mapper.Map<PaymentTradeRecord>(tradeRecord);
            return PaymentTradeRecordsRepository.Add(data);
        }

        #endregion

        #region 支付接口
        /// <summary>
        /// 冻结/解冻奖金成功后的操作
        /// </summary>
        /// <param name="freezeAmtDTO"></param>
        /// <returns></returns>
        public bool SaveFreezeAmt(FreezeAmtDTO freezeAmtDTO)
        {
            if (freezeAmtDTO.IsSuccess)
            {
                var paymentBankAccount = _mapper.Map<PaymentBankAccount>(freezeAmtDTO.PaymentBankAccount);
                PaymentBankAccountRepository.Modify(paymentBankAccount, new[] { "TotalAmount", "FreezeAmount", "CFWinFreezeAmount", "ModifiedDate" }, false);
            }
            var paymentTradeRecord = _mapper.Map<PaymentTradeRecord>(freezeAmtDTO.PaymentTradeRecord);
            PaymentTradeRecordsRepository.Add(paymentTradeRecord, false);
            var onlineTradeRecord = _mapper.Map<OnlinePaymentRecord>(freezeAmtDTO.OnlinePaymentRecord);
            OnlinePaymentRecordRepository.Add(onlineTradeRecord, false);
            return _unitOfWorkContext.Commit() > 0;
        }

        /// <summary>
        /// 支付保存
        /// </summary>
        /// <param name="orderPay"></param>
        /// <returns></returns>
        public bool SaveOrderPay(OrderPayDTO orderPay)
        {
            if (orderPay.IsSuccess)
            {
                var paymentBankAccount = _mapper.Map<PaymentBankAccount>(orderPay.PaymentBankAccount);
                PaymentBankAccountRepository.Modify(paymentBankAccount, false);
                //根据收款方 中金支付的账号查找信息
                var peePaymentBankAccount = _mapper.Map<PaymentBankAccount>(orderPay.PeePaymentBankAccount);
                PaymentBankAccountRepository.Modify(peePaymentBankAccount, false);
            }
            var paymentTradeRecord = _mapper.Map<PaymentTradeRecord>(orderPay.PaymentTradeRecord);
            PaymentTradeRecordsRepository.Add(paymentTradeRecord, false);
            var onlineTradeRecord = _mapper.Map<OnlinePaymentRecord>(orderPay.OnlinePaymentRecord);
            OnlinePaymentRecordRepository.Add(onlineTradeRecord, false);
            return _unitOfWorkContext.Commit() > 0;
        }

        /// <summary>
        /// 保存充值
        /// </summary>
        /// <param name="paramDTO"></param>
        /// <returns></returns>
        public bool SaveInTransaction(InTransactionDTO paramDTO)
        {
            var paymentTradeRecord = _mapper.Map<PaymentTradeRecord>(paramDTO.PaymentTradeRecord);
            PaymentTradeRecordsRepository.Add(paymentTradeRecord, false);
            var onlineTradeRecord = _mapper.Map<OnlinePaymentRecord>(paramDTO.OnlinePaymentRecord);
            OnlinePaymentRecordRepository.Add(onlineTradeRecord, false);
            return _unitOfWorkContext.Commit() > 0;
        }

        /// <summary>
        /// 保存入金查询数据
        /// </summary>
        /// <param name="paramDTO"></param>
        /// <returns></returns>
        public bool SaveInTransactionSearch(InTransactionSearchDTO paramDTO)
        {
            if (paramDTO.IsSuccess)
            {
                var paymentBankAccount = _mapper.Map<PaymentBankAccount>(paramDTO.PaymentBankAccount);
                PaymentBankAccountRepository.Modify(paymentBankAccount, false);
            }

            var paymentTradeRecord = _mapper.Map<PaymentTradeRecord>(paramDTO.PaymentTradeRecord);
            PaymentTradeRecordsRepository.Add(paymentTradeRecord, false);
            var onlineTradeRecord = _mapper.Map<OnlinePaymentRecord>(paramDTO.OnlinePaymentRecord);
            OnlinePaymentRecordRepository.Modify(onlineTradeRecord, false);
            return _unitOfWorkContext.Commit() > 0;
        }

        public bool SaveReceiveNotice(ReceiveNoticeDTO paramDTO)
        {
            if (paramDTO.IsSuccess)
            {
                var paymentBankAccount = _mapper.Map<PaymentBankAccount>(paramDTO.PaymentBankAccount);
                PaymentBankAccountRepository.Modify(paymentBankAccount, false);
            }
            var paymentTradeRecord = _mapper.Map<PaymentTradeRecord>(paramDTO.PaymentTradeRecord);
            PaymentTradeRecordsRepository.Add(paymentTradeRecord, false);
            var onlineTradeRecord = _mapper.Map<OnlinePaymentRecord>(paramDTO.OnlinePaymentRecord);
            if (onlineTradeRecord.Id > 0)
            {
                OnlinePaymentRecordRepository.Modify(onlineTradeRecord, false);
            }
            else
            {
                OnlinePaymentRecordRepository.Add(onlineTradeRecord, false);
            }
            return _unitOfWorkContext.Commit() > 0;
        }

        /// <summary>
        /// 保存出金
        /// </summary>
        /// <param name="paramDTO"></param>
        /// <returns></returns>
        public bool OutTransaction(OutTransactionDTO paramDTO)
        {
            if (paramDTO.IsSuccess)
            {
                var paymentBankAccount = _mapper.Map<PaymentBankAccount>(paramDTO.PaymentBankAccount);
                PaymentBankAccountRepository.Modify(paymentBankAccount, false);
            }
            var paymentTradeRecord = _mapper.Map<PaymentTradeRecord>(paramDTO.PaymentTradeRecord);
            PaymentTradeRecordsRepository.Add(paymentTradeRecord, false);
            var onlineTradeRecord = _mapper.Map<OnlinePaymentRecord>(paramDTO.OnlinePaymentRecord);
            OnlinePaymentRecordRepository.Add(onlineTradeRecord, false);
            return _unitOfWorkContext.Commit() > 0;
        }
        #endregion

        public Tuple<bool, List<CBSAccount>, string> GetCBSAccounts(bool isAdminPortal, string displayName, ConfigType payType)
        {
            var configEntity = ConfigApiService.GetConfigByType(payType);
            var attribuesResult = configEntity.ConfigAttributes;
            if (attribuesResult.Any())
            {
                var initpayConfig = new CMBCBSPayment(Tenant, this).InitpayConfig(attribuesResult);
                if (initpayConfig.Item1.Success)
                {
                    var cbsConfig = initpayConfig.Item2;
                    if (cbsConfig == null)
                    {
                        return new Tuple<bool, List<CBSAccount>, string>(false, null, "支付配置异常，请联系客服，谢谢！");
                    }
                    var sign = MD5Provider.Hash(displayName + "|" + cbsConfig.Key);

                    var postString = string.Format("accountName={0}&sign={1}", displayName, sign);

                    var url = cbsConfig.PayUrl + "/" + cbsConfig.QueryBankAccountInfoByAccountNameMethod;
                    var downloadData = HttpWebRequestHelper.WebClientDownload(url, postString);
                    if (downloadData.Item1)
                    {
                        var response = SerializeHelper.FromJson<CBSQueryBankAccountInfoResponse>(downloadData.Item2);
                        if (response == null)
                        {
                            return new Tuple<bool, List<CBSAccount>, string>(false, null, "系统异常");
                        }
                        if (response.code.Equals("000000") && response.result.Any())
                        {
                            var accounts = new List<CBSAccount>();
                            response.result.ForEach(k =>
                            {
                                var account = new CBSAccount();
                                account.BankNumber = k.bankAccount;
                                account.BankType = k.bankType;
                                account.CompanyName = k.accountName;
                                account.OpenBankName = k.openBankName;
                                account.BankAddress = k.openAccountProvince + k.openAccountCity;
                                accounts.Add(account);
                            });

                            return new Tuple<bool, List<CBSAccount>, string>(true, accounts, string.Empty);
                        }
                        return new Tuple<bool, List<CBSAccount>, string>(false, null, "未找到银行账户信息");
                    }
                    return new Tuple<bool, List<CBSAccount>, string>(false, null, downloadData.Item2);
                }

            }
            return new Tuple<bool, List<CBSAccount>, string>(false, null, "支付配置异常，请联系客服，谢谢！");
        }

        public Tuple<bool, string> CBSPaymentMethod(bool isAdminPortal, string userId, string companyName, string cbsAccount, int configSign, int configState, ConfigType payType,
            int orderAmount, string orderId, CashType type = CashType.Transaction, PaymentRecordDTO paymentInfo = null)
        {
            //var createResult = CreatePaymentPlatform(userId, companyName, currentUserMemberId, configSign, configState, payType, orderAmount, orderId, type);
            //if (!createResult.Item1)
            //    return createResult;
            var amount = (orderAmount / 100).ToString("F");
            var configEntity = ConfigApiService.GetConfigByType(payType);
            var attribuesResult = configEntity.ConfigAttributes;
            if (attribuesResult.Any())
            {
                var initpayConfig = new CMBCBSPayment(Tenant, this).InitpayConfig(attribuesResult);
                if (initpayConfig.Item1.Success)
                {
                    var cbsConfig = initpayConfig.Item2;
                    if (cbsConfig == null)
                    {
                        return new Tuple<bool, string>(false, "支付配置异常，请联系客服，谢谢！");
                    }
                    //var paymentOrderId = createResult.Item2.Split(',')[0];
                    var paymentOrderId = "";

                    //rpPaymentId +"|" +paymentAccounts +"|"+depositAccounts +"|"+amount +"|"+backNotifyUrl +"|" +key
                    var sign =
                        MD5Provider.Hash(paymentOrderId + "|" + cbsAccount + "|" + cbsConfig.depositAccounts + "|" +
                                   amount + "|" + cbsConfig.back_notify_url + "|" + cbsConfig.Key);
                    var postString = string.Format("erpPaymentId={0}&paymentAccounts={1}&depositAccounts={2}&amount={3}&purpose={4}&sign={5}&returnUrl={6}", paymentOrderId, cbsAccount, cbsConfig.depositAccounts, amount, cbsConfig.purpose, sign, cbsConfig.back_notify_url);
                    var url = cbsConfig.PayUrl + "/" + cbsConfig.PaymentMethod;
                    var downloadData = HttpWebRequestHelper.WebClientDownload(url, postString);
                    if (downloadData.Item1)
                    {
                        var response = SerializeHelper.FromJson<CBSPayResponse>(downloadData.Item2);
                        if (response == null)
                        {
                            return new Tuple<bool, string>(false, "系统异常");
                        }
                        if (response.code.Equals("000000")) //提交成功，等待财务确认支付
                        {
                            return new Tuple<bool, string>(true, "");
                        }
                        else //失败了，不会调用回调地址
                        {
                            //var paymentService = new OnlinePaymentRecordService();
                            var onlinePaymentRecord = GetByPaymentOrderIdAndOrderAmount(paymentOrderId,
                                orderAmount);
                            if (onlinePaymentRecord == null)
                            {
                                Logger.LogError(paymentOrderId + "支付订单未找到");
                                return new Tuple<bool, string>(false, "系统异常");
                            }
                            var localNow = DateTime.UtcNow.ToLocalDateTimeStr("yyyyMMddHHmmss");
                            onlinePaymentRecord.PayResult = "0"; //失败
                            onlinePaymentRecord.PayDatetime = localNow;
                            onlinePaymentRecord.ReturnDatetime = localNow;
                            onlinePaymentRecord.ErrorCode = response.code + ";" + response.description;
                            UpdatePaymentRecord(onlinePaymentRecord);
                            return new Tuple<bool, string>(false, response.description);
                        }
                    }
                    return new Tuple<bool, string>(false, downloadData.Item2);
                }
            }
            return new Tuple<bool, string>(false, "未知错误");
        }

        public PaginatedBaseDTO<CashUsageDetailDTO> GetPaginatedCashUsageDetailByFilter(bool isPayee, string userId, int pageIndex, int pageSize,
          DateTime? startDate, DateTime? endDate, ref decimal countMoney)
        {
            Expression<Func<CashUsageDetail, bool>> predicate = m => !m.IsDeleted && (isPayee ? m.PayeeCustomerId.Equals(userId, StringComparison.CurrentCultureIgnoreCase) : m.PaymentCustomerId.Equals(userId, StringComparison.CurrentCultureIgnoreCase));
            if (startDate.HasValue && endDate.HasValue)
            {
                predicate =
                    predicate.And(m => m.ConsumptionDate >= startDate.Value && m.ConsumptionDate <= endDate.Value);
            }
            else if (startDate.HasValue)
            {
                predicate =
                    predicate.And(m => m.ConsumptionDate >= startDate.Value);
            }
            else if (endDate.HasValue)
            {
                predicate =
                    predicate.And(m => m.ConsumptionDate <= endDate.Value);
            }
            var result = CashUsageDetailRepository.FindAll(predicate, m => m.Id).ToList();
            var data = result.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            var total = result.Count;
            countMoney = result.Select(m => m.ConsumptionMoney).Sum();
            var rows = _mapper.Map<List<CashUsageDetailDTO>>(data);
            return new PaginatedBaseDTO<CashUsageDetailDTO>(pageIndex, pageSize, total, rows);
        }
    }
}
