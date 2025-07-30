using AutoMapper;
using KC.Common;
using KC.Common.ToolsHelper;
using KC.DataAccess.Pay.Repository;
using KC.Database.EFRepository;
using KC.Database.IRepository;
using KC.Service.DTO;
using KC.Service.DTO.Pay;
using KC.Enums.Pay;
using KC.Framework.Base;
using KC.Framework.Exceptions;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Model.Pay;
using KC.Service.Constants;
using KC.Service.EFService;
using KC.Service.Util;
using KC.Service.WebApiService.Business;
using KC.Service.Pay.Constants;
using KC.ThirdParty;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace KC.Service.Pay
{
    public interface IPaymentRecordService : IEFService
    {
        #region 付款申请记录

        bool AddPaymentRecord(PaymentApplyRecordDTO dto, string userId, string displayName);

        string AddCBSPaymentRecord(PaymentApplyRecordDTO dto, string userId, string displayName);

        List<PaymentApplyRecordDTO> LoadPaymentRecords(string payableNumber, TradingAccount account);

        bool CancelPayment(int id, string remark, string userId, string displayName, string userName, bool cancel = true);

        PaymentApplyRecordDTO GetPaymentRecordByPaymentNum(string paymentNum);

        bool CheckPaymentIsSuccessful(string payNum);

        List<TODODTO> LoadWaitPayments();

        string GetPaymentNumber(int id);

        bool ExistsPaymentRecord(string payableNumber, decimal paymentAmount, bool wx);

        #endregion

        #region 充值申请记录
        bool AddChargeOfWithdrawalsRecord(PaymentApplyRecordDTO dto, string userId, string displayName, string remark);
        bool AddPaymentOperationLog(string orderid, string userId, string displayName, string remark);
        decimal UpChargeRecord(string orderid, string userId, string displayName, string remark, bool? status);
        bool CencelChargeRecord(int id, string userId, string displayName);

        List<PaymentApplyRecordDTO> LoadChargingList();
        PaginatedBaseDTO<PaymentApplyRecordDTO> LoadPagenatedFindChargingList(int pageIndex, int pageSize);
        #endregion

        #region 白条

        bool AddVoucherPaymentRecord(VoucherPaymentRecordDTO dto, string @operator);

        VoucherPaymentRecordDTO GetVoucherInfo(int id);

        List<VoucherPaymentRecordDTO> LoadVoucherPaymentRecord(string orderId);

        bool CancelVoucher(int id, string remark, string displayName, string userName, bool cancel = true);

        bool ReturnVoucher(string id, string remark);

        bool SaveVoucher(int id, string certificateId, string content, string signature, bool canTransferable, string userId, string userDisplayName);
        #endregion

        /// <summary>
        /// 获取应付待处理数据
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="source"></param>
        /// <param name="type"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        PaginatedBaseDTO<PendingPayablesDTO> LoadPagenatedPendingPayables(string userName, string orderId, PayableSource? source, PaymentType? type, DateTime? startDate, DateTime? endDate, int pageIndex, int pageSize);

        /// <summary>
        /// 获取应收待处理数据
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="source"></param>
        /// <param name="type"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        PaginatedBaseDTO<PendingReceivablesDTO> LoadPagenatedPendingReceivables(string orderId, ReceivableSource? source, PaymentType? type, DateTime? startDate, DateTime? endDate, int pageIndex, int pageSize);

        ReceivableSource GetReceivableSource(PayableSource source);

        PayableSource GetPayableSource(ReceivableSource source);

    }

    public class PaymentRecordService : EFServiceBase, IPaymentRecordService
    {
        private readonly IMapper _mapper;

        /// <summary>
        /// ComAccountUnitOfWorkContext
        ///     use AccountUnitOfWorkContext.Commit() to save the context change(transaction).
        /// </summary>
        protected EFUnitOfWorkContextBase _unitOfWorkContext { get; private set; }

        private IReceivableRepository ReceivableRepository;
        private IPayableRepository PayableRepository;

        private IDbRepository<PayableAndReceivableRecord> PayableAndReceivableRecordRepository;
        private IDbRepository<PaymentRecord> PaymentRecordRepository;
        private IDbRepository<PaymentOperationLog> PaymentOperationLogRepository;
        private IDbRepository<PaymentAttachment> AttachmentRepository;
        private IDbRepository<OfflinePayment> OfflinePaymentRepository;
        private IDbRepository<OfflineUsageBill> OfflineUsageBillRepository;
        private IDbRepository<VoucherPaymentRecord> VoucherPaymentRecordRepository;

        private readonly IAccountApiService AccountApiService;
        public PaymentRecordService(
            Tenant tenant,
            IMapper mapper,
            IAccountApiService accountApiService,
            EFUnitOfWorkContextBase unitOfWorkContext,

            IReceivableRepository ReceivableRepository,
            IPayableRepository PayableRepository,

            IDbRepository<PayableAndReceivableRecord> PayableAndReceivableRecordRepository,
            IDbRepository<PaymentRecord> PaymentRecordRepository,
            IDbRepository<PaymentOperationLog> PaymentOperationLogRepository,
            IDbRepository<PaymentAttachment> AttachmentRepository,
            IDbRepository<OfflinePayment> OfflinePaymentRepository,
            IDbRepository<OfflineUsageBill> OfflineUsageBillRepository,
            IDbRepository<VoucherPaymentRecord> VoucherPaymentRecordRepository,

            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<PaymentService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfWorkContext = unitOfWorkContext;

            AccountApiService = accountApiService;

            this.ReceivableRepository = ReceivableRepository;

            this.PayableRepository = PayableRepository;
            this.PayableAndReceivableRecordRepository = PayableAndReceivableRecordRepository;
            this.PaymentRecordRepository = PaymentRecordRepository;
            this.PaymentOperationLogRepository = PaymentOperationLogRepository;
            this.AttachmentRepository = AttachmentRepository;
            this.OfflinePaymentRepository = OfflinePaymentRepository;
            this.OfflineUsageBillRepository = OfflineUsageBillRepository;
            this.VoucherPaymentRecordRepository = VoucherPaymentRecordRepository;
        }

        #region 付款申请记录
        public bool AddPaymentRecord(PaymentApplyRecordDTO dto, string userId, string displayName)
        {
            var log =
                _mapper.Map<PaymentOperationLog>(CreatePaymentOperationLog(dto.PaymentNumber, dto.OrderNumber, userId,
                    displayName, "申请付款:" + dto.PaymentAmount + ";账户体系:" + dto.PaymentTypeName));
            PaymentOperationLogRepository.Add(log, false);
            return PaymentRecordRepository.Add(_mapper.Map<PaymentRecord>(dto));
        }

        public string AddCBSPaymentRecord(PaymentApplyRecordDTO dto, string userId, string displayName)
        {
            dto.PaymentNumber = OtherUtilHelper.GetSerialNumber(SequenceNumberConstants.PaymentApply);
            var log =
                _mapper.Map<PaymentOperationLog>(CreatePaymentOperationLog(dto.PaymentNumber, dto.OrderNumber, userId,
                    displayName, "申请付款:" + dto.PaymentAmount));
            PaymentOperationLogRepository.Add(log, false);
            dto.Status = false;
            var result = PaymentRecordRepository.Add(_mapper.Map<PaymentRecord>(dto));
            if (result)
                return dto.PaymentNumber;
            return string.Empty;
        }

        public bool CencelChargeRecord(int id, string userId, string displayName)
        {
            var record = PaymentRecordRepository.GetById(id);
            if (record == null)
                return false;
            record.Status = false;
            PaymentRecordRepository.Modify(record, new[] { "Status" }, false);
            var log = _mapper.Map<PaymentOperationLog>(CreatePaymentOperationLog(record.PaymentNumber, record.OrderNumber, userId,
                   displayName, "取消充值:" + record.PaymentAmount));
            return PaymentOperationLogRepository.Add(log);
        }
        public List<PaymentApplyRecordDTO> LoadPaymentRecords(string payableNumber, TradingAccount account)
        {
            var entities = PaymentRecordRepository.FindAll(m => m.PayableNumber == payableNumber && m.TradingAccount == account).OrderByDescending(m => m.CreatedDate);
            return _mapper.Map<List<PaymentApplyRecordDTO>>(entities);
        }

        public bool CancelPayment(int id, string remark, string userId, string displayName, string userName, bool cancel = true)
        {
            var record = PaymentRecordRepository.GetById(id);
            if (record == null)
                return false;
            record.Status = false;
            record.Remark += " " + displayName + ":" + remark;
            PaymentRecordRepository.Modify(record, new[] { "Status", "Remark" }, false);
            var log = _mapper.Map<PaymentOperationLog>(CreatePaymentOperationLog(record.PaymentNumber, record.OrderNumber, userId,
                    displayName, remark + ",取消付款:" + record.PaymentAmount));
            var @operator = displayName + "(" + userName + ")";
            PayableAndReceivableRecordRepository.Add(new PayableAndReceivableRecord { Operator = @operator, AmountOfMoney = -record.PaymentAmount, PayableNumber = record.PayableNumber, PayDateTime = record.CreatedDate, PaymentType = PaymentType.AccountBalance, Remark = @operator + (cancel ? "撤销" : "退回") + "付款申请，付款金额为" + record.PaymentAmount.ToString("C2") + "，支付方式为余额" }, false);
            return PaymentOperationLogRepository.Add(log);
        }

        public PaymentApplyRecordDTO GetPaymentRecordByPaymentNum(string paymentNum)
        {
            return _mapper.Map<PaymentApplyRecordDTO>(PaymentRecordRepository.GetByFilter(m => m.PaymentNumber == paymentNum && m.Status == null));
        }

        public bool CheckPaymentIsSuccessful(string payNum)
        {
            return PaymentRecordRepository.GetByFilter(m => m.PaymentNumber == payNum && m.Status == true) != null;
        }

        public List<TODODTO> LoadWaitPayments()
        {
            var result = new List<TODODTO>();
            var entities = PayableRepository.FindAll(m => m.PayableAmount - m.AlreadyPayAmount > 0).ToList();
            entities.ForEach(k =>
            {
                result.Add(new TODODTO
                {
                    Name = k.Customer,
                    Number = k.OrderId,
                    DateTime = k.CreatedDate,
                    Amount = (k.PayableAmount - k.AlreadyPayAmount).ToString("N2") + "元",
                    Type = TODOType.POPayable,
                    RelevantId = k.Id.ToString(),
                    Source = k.Source
                });
            });
            return result;
        }

        public string GetPaymentNumber(int id)
        {
            var record = PaymentRecordRepository.GetById(id);
            if (record == null)
                return null;
            return record.PaymentNumber;
        }

        public bool ExistsPaymentRecord(string payableNumber, decimal paymentAmount, bool wx)
        {
            return PaymentRecordRepository.GetByFilter(m => m.PayableNumber == payableNumber && m.PaymentAmount == paymentAmount && (wx ? m.Remark.Contains("微信") : m.Remark.Contains("支付宝"))) != null;
        }

        #endregion

        #region 充值申请记录
        public bool AddChargeOfWithdrawalsRecord(PaymentApplyRecordDTO dto, string userId, string displayName, string remark)
        {
            var log =
                _mapper.Map<PaymentOperationLog>(CreatePaymentOperationLog(dto.PaymentNumber, dto.OrderNumber, userId,
                    displayName, remark + dto.PaymentAmount));
            PaymentOperationLogRepository.Add(log, false);
            return PaymentRecordRepository.Add(_mapper.Map<PaymentRecord>(dto));
        }
        public bool AddPaymentOperationLog(string orderid, string userId, string displayName, string remark)
        {
            var dto = PaymentRecordRepository.GetByFilter(m => !m.IsDeleted && m.PaymentNumber == orderid);
            if (dto == null)
                return false;
            var log =
            _mapper.Map<PaymentOperationLog>(CreatePaymentOperationLog(dto.PaymentNumber, dto.OrderNumber, userId,
                displayName, remark + dto.PaymentAmount));
            return PaymentOperationLogRepository.Add(log);
        }

        public decimal UpChargeRecord(string orderid, string userId, string displayName, string remark, bool? status)
        {
            var dto = PaymentRecordRepository.GetByFilter(m => !m.IsDeleted && m.Status != true && m.PaymentNumber == orderid);
            if (dto == null)
                return 0;
            dto.Status = status;
            PaymentRecordRepository.Modify(dto, new[] { "Status" }, false);
            var log =
         _mapper.Map<PaymentOperationLog>(CreatePaymentOperationLog(dto.PaymentNumber, dto.OrderNumber, userId,
             displayName, remark + dto.PaymentAmount));
            if (PaymentOperationLogRepository.Add(log))
            {
                return dto.PaymentAmount;
            }
            return 0;
        }
        public List<PaymentApplyRecordDTO> LoadChargingList()
        {
            Expression<Func<PaymentRecord, bool>> predicate = m => !m.IsDeleted && m.Status == null && m.Source == null;
            return _mapper.Map<List<PaymentApplyRecordDTO>>(PaymentRecordRepository.FindAll(predicate));
        }
        public PaginatedBaseDTO<PaymentApplyRecordDTO> LoadPagenatedFindChargingList(int pageIndex, int pageSize)
        {
            Expression<Func<PaymentRecord, bool>> predicate = m => !m.IsDeleted && m.Status == null && m.Source == null;
            var result = PaymentRecordRepository.FindPagenatedListWithCount(pageIndex, pageSize, predicate,
                m => new { m.CreatedDate }, false);
            int total = result.Item1;
            var list = _mapper.Map<List<PaymentApplyRecordDTO>>(result.Item2);
            return new PaginatedBaseDTO<PaymentApplyRecordDTO>(pageIndex, pageSize, total, list);
        }
        #endregion

        #region 白条
        public bool AddVoucherPaymentRecord(VoucherPaymentRecordDTO dto, string @operator)
        {
            var entity = _mapper.Map<VoucherPaymentRecord>(dto);

            PayableAndReceivableRecordRepository.Add(new PayableAndReceivableRecord { Operator = @operator, AmountOfMoney = -dto.Amounts, PayableNumber = dto.PayableNumber, PayDateTime = dto.PaymentDate, PaymentType = PaymentType.Voucher, Remark = @operator + "提交付款申请，付款金额为" + dto.Amounts.ToString("C2") + "，支付方式为白条" }, false);

            
            return VoucherPaymentRecordRepository.Add(entity);
        }

        public List<VoucherPaymentRecordDTO> LoadVoucherPaymentRecord(string orderId)
        {
            var entities = VoucherPaymentRecordRepository.FindAll(m => m.OrderId == orderId);
            return _mapper.Map<List<VoucherPaymentRecordDTO>>(entities);
        }

        public bool CancelVoucher(int id, string remark, string displayName, string userName, bool cancel = true)
        {
            var entity = VoucherPaymentRecordRepository.GetById(id);
            if (entity == null)
                return true;

            entity.Status = cancel ? VoucherSubmitStatus.Cancel : VoucherSubmitStatus.Return;
            VoucherPaymentRecordRepository.Modify(entity, new[] { "Status" }, false);

            var @operator = displayName + "(" + userName + ")";
            return PayableAndReceivableRecordRepository.Add(new PayableAndReceivableRecord { Operator = @operator, AmountOfMoney = -entity.Amounts, PayableNumber = entity.PayableNumber, PayDateTime = entity.CreatedDate, PaymentType = PaymentType.Voucher, Remark = @operator + (cancel ? "撤销" : "退回") + "付款申请，付款金额为" + entity.Amounts.ToString("C2") + "，支付方式" + PaymentType.Voucher.ToDescription() + (!cancel ? "退回原因:" + remark : "") });
        }

        public bool ReturnVoucher(string id, string remark)
        {
            var entity = VoucherPaymentRecordRepository.GetByFilter(m => m.VoucherId == id);
            if (entity == null)
                return true;

            entity.Status = VoucherSubmitStatus.Return;
            VoucherPaymentRecordRepository.Modify(entity, new[] { "Status" }, false);

            return PayableAndReceivableRecordRepository.Add(new PayableAndReceivableRecord { Operator = "Robot", AmountOfMoney = -entity.Amounts, PayableNumber = entity.PayableNumber, PayDateTime = entity.CreatedDate, PaymentType = PaymentType.Voucher, Remark = "对方退回:" + remark });
        }
        public VoucherPaymentRecordDTO GetVoucherInfo(int id)
        {
            var entity = VoucherPaymentRecordRepository.GetById(id);
            return _mapper.Map<VoucherPaymentRecordDTO>(entity);
        }

        /// <summary>
        /// 保存白条至卖方
        /// </summary>
        /// <param name="id"></param>
        /// <param name="certificateId"></param>
        /// <param name="content"></param>
        /// <param name="signature"></param>
        /// <param name="canTransferable"></param>
        /// <param name="userId"></param>
        /// <param name="userDisplayName"></param>
        /// <returns></returns>
        public bool SaveVoucher(int id, string certificateId, string content, string signature, bool canTransferable, string userId, string userDisplayName)
        {
            var dto = VoucherPaymentRecordRepository.GetById(id);
            if (dto == null)
                return false;
            var seller = string.Empty;
            var sellerTenantName = string.Empty;
            if (dto.OrderId.IndexOf("OOR", StringComparison.OrdinalIgnoreCase) > -1)
            {
            }
            else //对账单
            {
            }

            var cfcaInfo = AccountApiService.GetUkeyAuthenticationByMemberId(Tenant.TenantName).Result;
            if (cfcaInfo == null)
                throw new BusinessPromptException("未找到Ukey认证信息");

            content = content.Replace("&quot;", "\"");
            var certData = DESSercurityUtil.GetCertFromSignedData(cfcaInfo.Signature);
            if (certData == null)
                throw new BusinessPromptException("签名信息不正确");
            if (!DESSercurityUtil.VerifyPKCS1("create_ious" + content, signature, certData))
                throw new BusinessPromptException("签名信息和使用签名的证书不匹配");
            var postDto = new List<CertificateDTO>();
            postDto.Add(new CertificateDTO
            {
                OrderId = dto.OrderId,
                OperatorId = userId,
                OperatorDisplayName = userDisplayName,
                CustomerUserName = sellerTenantName,
                CustomerDisplayName = seller,
                Debtor = dto.Debtor,
                DebtorBank = dto.DebtorBank,
                DebtorBankNumber = dto.DebtorBankNumber,
                BuyerCertificate = signature,
                BuyerCertificateTime = DateTime.UtcNow,
                DebtorTenant = Tenant.TenantName,
                DebtorSocialCreditCode = dto.DebtorSocialCreditCode,
                CertificateId = certificateId,
                Creditor = dto.Creditor,
                CreditorBank = dto.CreditorBank,
                CreditorBankNumber = dto.CreditorBankNumber,
                CreditorSocialCreditCode = dto.CreditorSocialCreditCode,
                CreditorTenant = seller,
                IsTransfer = canTransferable,
                RepayTotalAmount = dto.Amounts,
                RepayDate = dto.PaymentDate,
                CertificateSignature = content,
                PaymentInfo = dto.PayableNumber,//应付编号
                //ContractNo = contract.ContractNo,
                VouchOrderId = dto.CreditUsageId,
                VouchTenantName = dto.FinancialInstitutionTenantName,
                VouchTenantDisplayName = dto.FinancialInstitutionName,
                //PlatformCertificate = DESSercurityUtil.CreatePKCS1Signature("create_ious" + content, GlobalDataBase.PlatformCertPwd),
                PlatformCertificateTime = DateTime.UtcNow
            });

            //var result = new Com.Service.Shop.WebApiService.Business.FinanceApiService(Tenant).GenerateWhiteBars(postDto);
            //if (!result.success)
            //    return false;
            //if (!result.Result.Success)
            //{
            //    throw new BusinessPromptException(result.Result.ErrorMessage);
            //}
            dto.Status = VoucherSubmitStatus.Through;
            return VoucherPaymentRecordRepository.Modify(dto, new[] { "Status" });
        }
        #endregion

        /// <summary>
        /// 获取应付待处理数据
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="source"></param>
        /// <param name="type"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PaginatedBaseDTO<PendingPayablesDTO> LoadPagenatedPendingPayables(string userName, string orderId, PayableSource? source, PaymentType? type, DateTime? startDate, DateTime? endDate, int pageIndex, int pageSize)
        {
            var paymentRecords = new List<PendingPayablesDTO>();
            var vouchers = new List<PendingPayablesDTO>();
            if (!type.HasValue || type == PaymentType.AccountBalance)
            {
                paymentRecords = LoadPendingPayablesOfAccountBalance(userName, orderId, source, startDate, endDate);
            }

            if ((!source.HasValue || source == PayableSource.PO) && (!type.HasValue || type == PaymentType.Voucher))
            {
                vouchers = LoadPendingPayablesOfVoucher(userName, orderId, startDate, endDate);
            }
            var result = paymentRecords.Concat(vouchers).OrderBy(m => m.CreateDateTime);
            return new PaginatedBaseDTO<PendingPayablesDTO>(pageIndex, pageSize, result.Count(), paymentRecords.Concat(vouchers).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList());
        }

        /// <summary>
        /// 获取应收待处理数据
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="source"></param>
        /// <param name="type"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PaginatedBaseDTO<PendingReceivablesDTO> LoadPagenatedPendingReceivables(string orderId, ReceivableSource? source, PaymentType? type, DateTime? startDate, DateTime? endDate, int pageIndex, int pageSize)
        {
            var paymentRecords = new List<PendingReceivablesDTO>();
            var vouchers = new List<PendingReceivablesDTO>();
            //2、获取线下票据待确认数据
            if (!type.HasValue || type == PaymentType.OfflineBill)
            {
                paymentRecords.AddRange(LoadPendingReceivablesOfflineUsageBill(orderId, source, startDate, endDate));
            }

            //3、获取线下支付待确认数据
            if (!type.HasValue || type == PaymentType.OfflinePayment)
            {
                var offlinePayments = LoadPendingReceivablesOfflinePayment(orderId, source, startDate, endDate);
                paymentRecords.AddRange(offlinePayments);
            }

            var result = paymentRecords.Concat(vouchers).OrderBy(m => m.CreateDateTime);
            return new PaginatedBaseDTO<PendingReceivablesDTO>(pageIndex, pageSize, result.Count(), paymentRecords.Concat(vouchers).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList());
        }

        public ReceivableSource GetReceivableSource(PayableSource source)
        {
            switch (source)
            {
                case PayableSource.AdvanceCharge:
                    return ReceivableSource.AdvanceCharge;
                case PayableSource.CautionMoney:
                    return ReceivableSource.CautionMoney;
                case PayableSource.BCO:
                    return ReceivableSource.SCO;
                case PayableSource.RefundCautionMoney:
                    return ReceivableSource.RefundCautionMoney;
                case PayableSource.AccountStatement:
                    return ReceivableSource.AccountStatement;
                case PayableSource.PO:
                default:
                    return ReceivableSource.SO;
            }
        }

        public PayableSource GetPayableSource(ReceivableSource source)
        {
            switch (source)
            {

                case ReceivableSource.AdvanceCharge:
                    return PayableSource.AdvanceCharge;
                case ReceivableSource.CautionMoney:
                    return PayableSource.CautionMoney;
                case ReceivableSource.SCO:
                    return PayableSource.BCO;
                case ReceivableSource.RefundCautionMoney:
                    return PayableSource.RefundCautionMoney;
                case ReceivableSource.AccountStatement:
                    return PayableSource.AccountStatement;
                case ReceivableSource.SO:
                default:
                    return PayableSource.PO;
            }
        }

        private List<PendingReceivablesDTO> LoadPendingReceivablesOfflineUsageBill(string orderId, ReceivableSource? source, DateTime? startDate, DateTime? endDate)
        {
            var result = new List<PendingReceivablesDTO>();
            Expression<Func<OfflineUsageBill, bool>> predicate = m => m.Status == OfflinePaymentStatus.WaitConfirm && m.AmountOfMoney > 0;
            if (!string.IsNullOrWhiteSpace(orderId))
                predicate = predicate.And(m => m.OrderId == orderId);
            if (source.HasValue)
                predicate = predicate.And(m => m.ReceivableSource == source);
            DateTime eDate;
            if (startDate.HasValue && endDate.HasValue)
            {
                eDate = endDate.Value.AddDays(1);
                predicate = predicate.And(m => m.CreatedDate >= startDate && m.CreatedDate < eDate);
            }
            else if (startDate.HasValue)
            {
                predicate = predicate.And(m => m.CreatedDate >= startDate);
            }
            else if (endDate.HasValue)
            {
                eDate = endDate.Value.AddDays(1);
                predicate = predicate.And(m => m.CreatedDate < eDate);
            }

            var entities = OfflineUsageBillRepository.FindAll(predicate).ToList();
            if (!entities.Any())
                return result;
            var businessNumbers = entities.Select(m => m.BusinessNumber).ToList();
            var attachments = AttachmentRepository.FindAll(m => businessNumbers.Contains(m.BusinessNumber)).ToList();
            entities.ForEach(k =>
            {
                var attachment = attachments.FirstOrDefault(m => m.BusinessNumber == k.BusinessNumber);
                result.Add(new PendingReceivablesDTO
                {
                    BillNo = k.BillNumber,
                    Source = k.ReceivableSource.HasValue ? k.ReceivableSource.Value : ReceivableSource.SO,
                    Customer = k.Customer,
                    OrderId = k.OrderId,
                    Id = k.BusinessNumber,
                    PaymentType = PaymentType.OfflineBill,
                    Remark = k.Remark,
                    ThisAmounts = k.AmountOfMoney,
                    CreateDateTime = k.CreatedDate,
                    FileUrl = attachment != null ? attachment.Url : string.Empty
                });
            });
            return result;
        }

        private List<PendingReceivablesDTO> LoadPendingReceivablesOfflinePayment(string orderId, ReceivableSource? source, DateTime? startDate, DateTime? endDate)
        {
            var result = new List<PendingReceivablesDTO>();
            Expression<Func<OfflinePayment, bool>> predicate = m => m.Status == OfflinePaymentStatus.WaitConfirm && m.AmountOfMoney > 0;
            if (!string.IsNullOrWhiteSpace(orderId))
                predicate = predicate.And(m => m.OrderId == orderId);
            if (source.HasValue)
                predicate = predicate.And(m => m.ReceivableSource == source);
            DateTime eDate;
            if (startDate.HasValue && endDate.HasValue)
            {
                eDate = endDate.Value.AddDays(1);
                predicate = predicate.And(m => m.CreatedDate >= startDate && m.CreatedDate < eDate);
            }
            else if (startDate.HasValue)
            {
                predicate = predicate.And(m => m.CreatedDate >= startDate);
            }
            else if (endDate.HasValue)
            {
                eDate = endDate.Value.AddDays(1);
                predicate = predicate.And(m => m.CreatedDate < eDate);
            }

            var entities = OfflinePaymentRepository.FindAll(predicate).ToList();
            if (!entities.Any())
                return result;
            var businessNumbers = entities.Select(m => m.BusinessNumber).ToList();
            var attachments = AttachmentRepository.FindAll(m => businessNumbers.Contains(m.BusinessNumber)).ToList();
            entities.ForEach(k =>
            {
                var attachment = attachments.FirstOrDefault(m => m.BusinessNumber == k.BusinessNumber);
                result.Add(new PendingReceivablesDTO
                {
                    Source = k.ReceivableSource.HasValue ? k.ReceivableSource.Value : ReceivableSource.SO,
                    Customer = k.Customer,
                    OrderId = k.OrderId,
                    Id = k.BusinessNumber,
                    PaymentType = PaymentType.OfflinePayment,
                    Remark = k.Remark,
                    ThisAmounts = k.AmountOfMoney,
                    CreateDateTime = k.CreatedDate,
                    FileUrl = attachment != null ? attachment.Url : string.Empty
                });
            });
            return result;
        }

        private List<PendingPayablesDTO> LoadPendingPayablesOfAccountBalance(string userName, string orderId, PayableSource? source, DateTime? startDate, DateTime? endDate)
        {
            var result = new List<PendingPayablesDTO>();
            Expression<Func<PaymentRecord, bool>> predicate = m => !m.IsDeleted && !m.Status.HasValue;
            if (!string.IsNullOrWhiteSpace(orderId))
            {
                predicate = predicate.And(m => m.OrderNumber == orderId);
            }
            if (source.HasValue)
            {
                predicate = predicate.And(m => m.Source == source);
            }
            DateTime eDate;
            if (startDate.HasValue && endDate.HasValue)
            {
                eDate = endDate.Value.AddDays(1);
                predicate = predicate.And(m => m.CreatedDate >= startDate && m.CreatedDate < eDate);
            }
            else if (startDate.HasValue)
            {
                predicate = predicate.And(m => m.CreatedDate >= startDate);
            }
            else if (endDate.HasValue)
            {
                eDate = endDate.Value.AddDays(1);
                predicate = predicate.And(m => m.CreatedDate < eDate);
            }
            var entities = PaymentRecordRepository.FindAll(predicate).ToList();
            entities.ForEach(k =>
            {
                result.Add(new PendingPayablesDTO { Id = k.Id, PaymentNumber = k.PaymentNumber, OrderId = k.OrderNumber, CreateDateTime = k.CreatedDate, Customer = k.Payee, PaymentType = PaymentType.AccountBalance, Source = k.Source.HasValue ? k.Source.Value : PayableSource.PO, ThisAmounts = k.PaymentAmount, CanCancel = k.CreatedBy == userName });
            });
            return result;
        }

        private List<PendingPayablesDTO> LoadPendingPayablesOfVoucher(string userName, string orderId, DateTime? startDate, DateTime? endDate)
        {
            var result = new List<PendingPayablesDTO>();
            Expression<Func<VoucherPaymentRecord, bool>> predicate = m => !m.IsDeleted && m.Status == VoucherSubmitStatus.Wait;
            if (!string.IsNullOrWhiteSpace(orderId))
            {
                predicate = predicate.And(m => m.OrderId == orderId);
            }
            DateTime eDate;
            if (startDate.HasValue && endDate.HasValue)
            {
                eDate = endDate.Value.AddDays(1);
                predicate = predicate.And(m => m.CreatedDate >= startDate && m.CreatedDate < eDate);
            }
            else if (startDate.HasValue)
            {
                predicate = predicate.And(m => m.CreatedDate >= startDate);
            }
            else if (endDate.HasValue)
            {
                eDate = endDate.Value.AddDays(1);
                predicate = predicate.And(m => m.CreatedDate < eDate);
            }
            var entities = VoucherPaymentRecordRepository.FindAll(predicate).ToList();
            entities.ForEach(k =>
            {
                result.Add(new PendingPayablesDTO
                {
                    Id = k.Id,
                    OrderId = k.OrderId,
                    CreateDateTime = k.CreatedDate,
                    Customer = k.Creditor,
                    PaymentType = PaymentType.Voucher,
                    Source = k.OrderId.IndexOf("OOR", StringComparison.OrdinalIgnoreCase) > -1 ? PayableSource.PO : PayableSource.AccountStatement,
                    ThisAmounts = k.Amounts,
                    CanCancel = k.CreatedBy == userName
                });
            });
            return result;
        }

        private PaymentOperationLogDTO CreatePaymentOperationLog(string paymentNumber, string referenceId, string operatorId, string @operator, string remark)
        {
            return new PaymentOperationLogDTO
            {
                PaymentNumber = paymentNumber,
                ReferenceId = referenceId,
                OperatorId = operatorId,
                Operator = @operator,
                Remark = remark
            };
        }
    }
}
