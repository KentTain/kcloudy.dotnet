using AutoMapper;
using KC.Common;
using KC.DataAccess.Pay.Repository;
using KC.Database.EFRepository;
using KC.Database.IRepository;
using KC.Service.DTO.Pay;
using KC.Enums.Pay;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Model.Pay;
using KC.Service.EFService;
using KC.Service.WebApiService.Business;
using KC.ThirdParty;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace KC.Service.Pay
{
    public interface IReceivableService : IEFService
    {
        List<ReceivableDTO> LoadTop5();
        List<ReceivableDTO> GetReceivableByFilter(string orderId, ReceivableType? type, ReceivableSource? source,
            DateTime? startDate, DateTime? endDate, int page = 1, int rows = 15, string order = "asc");

        NpoiMemoryStream ExportReceivables(string orderId, ReceivableType? type, ReceivableSource? source,
            DateTime? startDate, DateTime? endDate, List<ReceivableDTO> marketData = null);

        bool AddReceivable(List<ReceivableDTO> receivables, PayableDTO payable = null);

        decimal GetReceivableAmount();

        Func<ReceivableDTO, bool> GetReceivablePredicate(string orderId, ReceivableType? type, ReceivableSource? source,
            DateTime? startDate, DateTime? endDate);

        ReceivableDTO FindById(int id);
        ReceivableDTO FindByPayableNumber(string payableNumber);

        List<PayableAndReceivableRecordDTO> LoadLogs(string payableNumber);

        bool RemoveReceivableWithCautionMoney(string payableNumber);

        bool ModifyReceivable(string payableNumber, PaymentType type);
    }
    public class ReceivableService : EFServiceBase, IReceivableService
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
        private IDbRepository<CautionMoneyLog> CautionMoneyLogRepository;
        

        public ReceivableService(
            Tenant tenant,
            IMapper mapper,
            EFUnitOfWorkContextBase unitOfWorkContext,

            IReceivableRepository ReceivableRepository,
            IPayableRepository PayableRepository,

            IDbRepository<PayableAndReceivableRecord> PayableAndReceivableRecordRepository,
            IDbRepository<CautionMoneyLog> CautionMoneyLogRepository,

            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<PaymentService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfWorkContext = unitOfWorkContext;

            this.ReceivableRepository = ReceivableRepository;

            this.PayableRepository = PayableRepository;
            this.PayableAndReceivableRecordRepository = PayableAndReceivableRecordRepository;
            this.CautionMoneyLogRepository = CautionMoneyLogRepository;
        }

        public ReceivableDTO FindById(int id)
        {
            return _mapper.Map<ReceivableDTO>(ReceivableRepository.GetById(id));
        }

        public ReceivableDTO FindByPayableNumber(string payableNumber)
        {
            return _mapper.Map<ReceivableDTO>(ReceivableRepository.GetByFilter(m => m.PayableNumber == payableNumber));
        }


        public List<ReceivableDTO> LoadTop5()
        {
            var entities = ReceivableRepository.LoadLatelyReceivables(5);
            return _mapper.Map<List<ReceivableDTO>>(entities);
        }

        public List<ReceivableDTO> GetReceivableByFilter(string orderId, ReceivableType? type, ReceivableSource? source,
     DateTime? startDate, DateTime? endDate, int page = 1, int rows = 15, string order = "asc")
        {
            var predicate = GetPredicate(orderId, type, source, startDate, endDate);
            var data = ReceivableRepository.GetPayableByFilter(predicate, order);
            return _mapper.Map<List<ReceivableDTO>>(data);
        }

        public NpoiMemoryStream ExportReceivables(string orderId, ReceivableType? type, ReceivableSource? source,
            DateTime? startDate, DateTime? endDate, List<ReceivableDTO> marketData = null)
        {
            var predicate = GetPredicate(orderId, type, source, startDate, endDate);
            DataTable dt;
            try
            {
                var data = ReceivableRepository.FindAll(predicate);
                if (data.Any())
                {
                    var receivables = _mapper.Map<List<ReceivableDTO>>(data);
                    receivables.ForEach(k => { k.ReceivableAmount = k.ReceivableAmount - k.AlreadyPayAmount; });
                    var jsonData = SerializeHelper.ToJson(receivables.Concat(marketData == null ? new List<ReceivableDTO>() : marketData));

                    dt = jsonData.ToDataTable();
                }
                else if (marketData != null)
                {
                    var jsonData = SerializeHelper.ToJson(marketData);
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
            return new NPOIExcelWriter(false).ExportToExcelStream(dt, "应收汇总", "", new[] { "SourceName", "OrderId", "ReceivableAmount", "UnPaidTotal", "StartDateStr", "Customer", "Remark", "Status" }, new[] { "应收来源", "关联订单号", "应收金额", "未收金额", "应收款日期", "付款人", "备注", "付款状态" });
        }

        public bool AddReceivable(List<ReceivableDTO> receivables, PayableDTO payable = null)
        {
            if (payable != null)
                PayableRepository.Add(_mapper.Map<Payable>(payable), false);
            return ReceivableRepository.Add(_mapper.Map<List<Receivable>>(receivables)) > 0;
        }

        public decimal GetReceivableAmount()
        {
            return ReceivableRepository.GetReceivableAmount();
        }

        public Func<ReceivableDTO, bool> GetReceivablePredicate(string orderId, ReceivableType? type, ReceivableSource? source,
            DateTime? startDate, DateTime? endDate)
        {
            Expression<Func<ReceivableDTO, bool>> predicate = m => m.ReceivableAmount != m.AlreadyPayAmount;
            if (!string.IsNullOrWhiteSpace(orderId))
                predicate = predicate.And(m => orderId.Equals(m.OrderId, StringComparison.OrdinalIgnoreCase));
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
            return predicate.Compile();
        }

        public List<PayableAndReceivableRecordDTO> LoadLogs(string payableNumber)
        {
            var entities = PayableAndReceivableRecordRepository.FindAll(m => m.PayableNumber == payableNumber && m.AmountOfMoney > 0);
            return _mapper.Map<List<PayableAndReceivableRecordDTO>>(entities);
        }

        public bool RemoveReceivableWithCautionMoney(string payableNumber)
        {
            ReceivableRepository = new ReceivableRepository(_unitOfWorkContext);
            var receivable = ReceivableRepository.GetByFilter(m => m.PayableNumber == payableNumber);
            if (receivable == null || receivable.Source != ReceivableSource.RefundCautionMoney)
                return false;
            var log = CautionMoneyLogRepository.GetByFilter(m => m.Remark == payableNumber);
            if (log != null)
            {
                log.Status = CautionMoneyPayStatus.Cancel;
                CautionMoneyLogRepository.Modify(log, new[] { "Status" }, false);
            }

            return ReceivableRepository.RemoveById(receivable.Id);
        }

        public bool ModifyReceivable(string payableNumber, PaymentType type)
        {
            var receivable = ReceivableRepository.GetByFilter(m => m.PayableNumber == payableNumber);
            if (receivable == null || receivable.AlreadyPayAmount == receivable.ReceivableAmount)
                return false;
            receivable.AlreadyPayAmount = receivable.ReceivableAmount;
            var pr = new PayableAndReceivableRecord
            {
                PayableNumber = payableNumber,
                PayDateTime = DateTime.UtcNow,
                AmountOfMoney = receivable.ReceivableAmount,
                PaymentType = type
            };
            PayableAndReceivableRecordRepository.Add(pr, false);
            return ReceivableRepository.Modify(receivable, new[] { "AlreadyPayAmount" });
        }

        private Expression<Func<Receivable, bool>> GetPredicate(string orderId, ReceivableType? type, ReceivableSource? source,
            DateTime? startDate, DateTime? endDate)
        {
            Expression<Func<Receivable, bool>> predicate = m => m.ReceivableAmount != m.AlreadyPayAmount;
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
