using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using KC.Service.EFService;
using KC.Service.WebApiService.Business;
using KC.DataAccess.Pay.Repository;
using KC.Service.DTO.Pay;
using KC.Model.Pay;
using KC.Database.IRepository;
using KC.Framework.Tenant;
using KC.Database.EFRepository;
using KC.Enums.Pay;
using KC.Framework.Extension;
using KC.Common;
using KC.Framework.Exceptions;
using KC.ThirdParty;
using System.Data;

namespace KC.Service.Pay
{
    public interface IPayableService : IEFService
    {
        PayableDTO GetPayableById(int id);
        PayableDTO GetPayableByPayableNumber(string payableNumber);
        PayableDTO GetPayableByOrderNumber(string orderNumber);

        List<PayableDTO> LoadTop5();
        List<PayableDTO> GetPayableByFilter(string orderId, PayableType? type, PayableSource? source,
            DateTime? startDate, DateTime? endDate, string order = "asc");

        NpoiMemoryStream ExportPayables(string orderId, PayableType? type, PayableSource? source,
            DateTime? startDate, DateTime? endDate, List<PayableDTO> payables = null);

        bool AddPayable(List<PayableDTO> payables);
        decimal GetPayableAmount();

        bool CheckPayable(PayableSource source);
        bool Cancel(int id);
        bool CancelPayableWithCautionMoney(int id);

        List<PayableAndReceivableRecordDTO> LoadLogs(string payableNumber);
    }

    public class PayableService : EFServiceBase, IPayableService
    {
        private readonly IMapper _mapper;

        /// <summary>
        /// ComAccountUnitOfWorkContext
        ///     use AccountUnitOfWorkContext.Commit() to save the context change(transaction).
        /// </summary>
        protected EFUnitOfWorkContextBase _unitOfWorkContext { get; private set; }
        
        private IPayableRepository PayableRepository;
        private IDbRepository<PayableAndReceivableRecord> PayableAndReceivableRecordRepository;
        private IDbRepository<EntrustedPaymentRecord> EntrustedPaymentRecordRepository;
        private IDbRepository<CautionMoneyLog> CautionMoneyLogRepository;


        public PayableService(
            Tenant tenant,
            IMapper mapper,
            EFUnitOfWorkContextBase unitOfWorkContext,

            IPayableRepository payableRepository,
            IDbRepository<PayableAndReceivableRecord> payableAndReceivableRecordRepository,
            IDbRepository<EntrustedPaymentRecord> entrustedPaymentRecordRepository,
            IDbRepository<CautionMoneyLog> CautionMoneyLogRepository,

            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<PaymentService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfWorkContext = unitOfWorkContext;

            this.PayableRepository = payableRepository;
            this.PayableAndReceivableRecordRepository = payableAndReceivableRecordRepository;
            this.EntrustedPaymentRecordRepository = entrustedPaymentRecordRepository;
            this.CautionMoneyLogRepository = CautionMoneyLogRepository;
        }

        public PayableDTO GetPayableById(int id)
        {
            return _mapper.Map<PayableDTO>(PayableRepository.GetById(id));
        }

        public PayableDTO GetPayableByPayableNumber(string payableNumber)
        {
            return _mapper.Map<PayableDTO>(PayableRepository.GetByFilter(m => m.PayableNumber == payableNumber));
        }

        public PayableDTO GetPayableByOrderNumber(string orderNumber)
        {
            return _mapper.Map<PayableDTO>(PayableRepository.GetByFilter(m => m.OrderId == orderNumber));
        }

        public List<PayableDTO> LoadTop5()
        {
            var entities = PayableRepository.LoadLatelyPayables(5);
            return _mapper.Map<List<PayableDTO>>(entities);
        }

        public List<PayableDTO> GetPayableByFilter(string orderId, PayableType? type, PayableSource? source,
            DateTime? startDate, DateTime? endDate, string order = "asc")
        {
            var predicate = GetPredicate(orderId, type, source, startDate, endDate);
            var data = PayableRepository.GetPayableByFilter(predicate, order);
            return _mapper.Map<List<PayableDTO>>(data);
        }

        public NpoiMemoryStream ExportPayables(string orderId, PayableType? type, PayableSource? source,
            DateTime? startDate, DateTime? endDate, List<PayableDTO> payables = null)
        {
            var predicate = GetPredicate(orderId, type, source, startDate, endDate);
            DataTable dt;
            try
            {
                var data = PayableRepository.FindAll(predicate);
                if (data.Any())
                {
                    var payableDTOs = _mapper.Map<List<PayableDTO>>(data);
                    payableDTOs.ForEach(k => { k.PayableAmount -= k.AlreadyPayAmount; });
                    var jsonData = SerializeHelper.ToJson(payableDTOs.Concat(payables == null ? new List<PayableDTO>() : payables));
                    dt = jsonData.ToDataTable();
                }
                else if (payables != null)
                {
                    var jsonData = SerializeHelper.ToJson(payables);
                    dt = jsonData.ToDataTable();
                }
                else
                {
                    dt = new DataTable();
                }
            }
            catch (Exception ex)
            {
                dt = new DataTable();
            }
            return new NPOIExcelWriter(false).ExportToExcelStream(dt, "应付汇总", "", new[] { "SourceName", "OrderId", "PayableAmount", "UnpaidTotal", "StartDateStr", "Customer", "Remark", "Status" }, new[] { "应付来源", "关联订单号", "应付金额", "未付金额", "应付款日期", "收款人", "备注", "付款状态" });
        }

        public bool AddPayable(List<PayableDTO> payables)
        {
            return PayableRepository.Add(_mapper.Map<List<Payable>>(payables)) > 0;
        }

        /// <summary>
        /// 查询应付汇总
        /// </summary>
        /// <returns></returns>
        public decimal GetPayableAmount()
        {
            return PayableRepository.GetPayableAmount();
        }

        public bool CheckPayable(PayableSource source)
        {
            return PayableRepository.FindAll(m => m.Source == source && m.AlreadyPayAmount != m.PayableAmount).Any();
        }

        public bool Cancel(int id)
        {
            var entity = PayableRepository.GetById(id);
            if (entity == null || (entity.Source != PayableSource.SystemFee && entity.Source != PayableSource.RefundCautionMoney))
                return false;
            if (entity.AlreadyPayAmount > 0)
                throw new BusinessPromptException("该订单已付部分金额，不可取消!");

            //var removeResult = new Core.SystemVersionService(Tenant).DeleteSystemVersionRecordByOrderId(Tenant.TenantName, entity.OrderId);
            //if (removeResult == null || !removeResult.Success)
            //{
            //    throw new BusinessPromptException(removeResult == null ? "处理失败！" : removeResult.ErrorMessage);
            //}
            return PayableRepository.RemoveById(id);
        }

        public bool CancelPayableWithCautionMoney(int id)
        {
            PayableRepository = new PayableRepository(_unitOfWorkContext);
            var entity = PayableRepository.GetById(id);
            if (entity == null || (entity.Source != PayableSource.SystemFee && entity.Source != PayableSource.RefundCautionMoney))
                return false;
            var log = CautionMoneyLogRepository.GetByFilter(m => m.Remark == entity.PayableNumber);
            if (log != null)
            {
                log.Status = CautionMoneyPayStatus.Cancel;
                CautionMoneyLogRepository.Modify(log, new[] { "Status" }, false);
            }
            if (entity.Source == PayableSource.RefundCautionMoney)//需删掉对方应收
            {
                //new OrderApiService(Tenant).RemoveReceivableWithCautionMoney(entity.PayableNumber, entity.CustomerTenant);
            }
            return PayableRepository.RemoveById(id);
        }

        public List<PayableAndReceivableRecordDTO> LoadLogs(string payableNumber)
        {
            var entities = PayableAndReceivableRecordRepository.FindAll(m => m.PayableNumber == payableNumber && m.AmountOfMoney < 0);
            return _mapper.Map<List<PayableAndReceivableRecordDTO>>(entities);
        }

        private Expression<Func<Payable, bool>> GetPredicate(string orderId, PayableType? type, PayableSource? source,
            DateTime? startDate, DateTime? endDate)
        {
            var date = DateTime.UtcNow.AddMinutes(-10);
            Expression<Func<Payable, bool>> predicate = m => m.PayableAmount != m.AlreadyPayAmount || (m.PayableAmount == m.AlreadyPayAmount && m.ModifiedDate > date);
            if (!string.IsNullOrWhiteSpace(orderId))
                predicate = predicate.And(m => m.OrderId.Contains(orderId));
            if (type.HasValue)
            {
                predicate = predicate.And(m => m.Type == type.Value);
            }
            if (source.HasValue)
            {
                predicate = predicate.And(m => m.Source == source.Value);
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                var start = startDate.Value;
                var end = endDate.Value;
                predicate = predicate.And(m => m.StartDate >= start && m.StartDate <= end);
            }
            else if (startDate.HasValue)
            {
                var start = startDate.Value;
                predicate = predicate.And(m => m.StartDate >= start);
            }
            else if (endDate.HasValue)
            {
                var end = endDate.Value;
                predicate = predicate.And(m => m.StartDate <= end);
            }
            return predicate;
        }
    }
}
