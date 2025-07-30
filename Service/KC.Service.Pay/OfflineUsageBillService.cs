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
using KC.Service.DTO.Pay;
using KC.Model.Pay;
using KC.Database.IRepository;
using KC.Framework.Tenant;
using KC.Database.EFRepository;
using KC.Enums.Pay;
using KC.Framework.Extension;
using System.ComponentModel;
using KC.DataAccess.Pay.Repository;
using KC.Service.DTO;
using KC.Framework.Exceptions;

namespace KC.Service.Pay
{
    public interface IOfflineUsageBillService : IEFService
    {
        OfflineUsageBillDTO GetByBusinessNumber(string businessNumber);

        bool AddOfflineUsageBill(OfflineUsageBillDTO dto, string @operator = null);

        List<OfflineUsageBillDTO> LoadOfflineUsageBill(string payableNumber);

        List<OfflineUsageBillDTO> LoadExpenditureOfflineUsageBillByOrder(string orderId);

        List<OfflineUsageBillDTO> LoadIncomeOfflineUsageBillByOrder(string orderId);

        Tuple<string, string, bool> CancelOfflineUsageBill(int id, string remark, string @operator);

        bool RemoveOfflineUsageBill(string businessNumber);

        ExecServiceResult ReturnOfflineUsageBill(string businessNumber, string remark, string @operator = null);

        ExecServiceResult ConfirmOfflineUsageBill(string businessNumber);

        PaginatedBaseDTO<OfflineUsageBillDTO> LoadOfflinePayable(string orderId, string billNumber, int pageIndex, int pageSize);

        PaginatedBaseDTO<OfflineUsageBillDTO> LoadOfflineReceviable(string orderId, string billNumber, int pageIndex, int pageSize);

        bool ConfirmPay(int id, string operatorId, string operatorName);

        bool ModifyBillCashPayment(string businessNumber, bool status);
    }

    public class OfflineUsageBillService : EFServiceBase, IOfflineUsageBillService
    {
        private readonly IMapper _mapper;

        /// <summary>
        /// ComAccountUnitOfWorkContext
        ///     use AccountUnitOfWorkContext.Commit() to save the context change(transaction).
        /// </summary>
        protected EFUnitOfWorkContextBase _unitOfWorkContext { get; private set; }

        private IPayableRepository PayableRepository;
        private IReceivableRepository ReceivableRepository;

        private IDbRepository<OfflineUsageBill> OfflineUsageBillRepository;
        private IDbRepository<PayableAndReceivableRecord> PayableAndReceivableRecordRepository;
        private IDbRepository<PaymentAttachment> AttachmentRepository;
        

        public OfflineUsageBillService(
            Tenant tenant,
            IMapper mapper,
            EFUnitOfWorkContextBase unitOfWorkContext,

            IReceivableRepository ReceivableRepository,
            IPayableRepository PayableRepository,

            IDbRepository<OfflineUsageBill> offlinePaymentRepository,
            IDbRepository<PayableAndReceivableRecord> payableAndReceivableRecordRepository,
            IDbRepository<PaymentAttachment> attachmentRepository,

            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<PaymentService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfWorkContext = unitOfWorkContext;

            this.ReceivableRepository = ReceivableRepository;
            this.PayableRepository = PayableRepository;

            this.OfflineUsageBillRepository = offlinePaymentRepository;
            this.PayableAndReceivableRecordRepository = payableAndReceivableRecordRepository;
            this.AttachmentRepository = attachmentRepository;
        }

        public OfflineUsageBillDTO GetByBusinessNumber(string businessNumber)
        {
            var entity = OfflineUsageBillRepository.GetByFilter(m => m.BusinessNumber == businessNumber);
            return _mapper.Map<OfflineUsageBillDTO>(entity);
        }

        public bool AddOfflineUsageBill(OfflineUsageBillDTO dto, string @operator = null)
        {
            if (!string.IsNullOrWhiteSpace(@operator))
            {
                var logDto = new PayableAndReceivableRecord
                {
                    PayableNumber = dto.PayableNumber,
                    AmountOfMoney = dto.AmountOfMoney,
                    PayDateTime = DateTime.UtcNow,
                    PaymentType = PaymentType.OfflineBill,
                    Remark = @operator + "提交线下票据(" + dto.BillNumber + ")，金额为" + (-dto.AmountOfMoney).ToString("C"),
                    Operator = @operator
                };
                PayableAndReceivableRecordRepository.Add(logDto, false);
            }

            AttachmentRepository.Add(_mapper.Map<PaymentAttachment>(dto.Attachments), false);
            return OfflineUsageBillRepository.Add(_mapper.Map<OfflineUsageBill>(dto));
        }

        public List<OfflineUsageBillDTO> LoadOfflineUsageBill(string payableNumber)
        {
            var entities = OfflineUsageBillRepository.FindAll(m => m.PayableNumber == payableNumber);
            var businessNumbers = entities.Select(m => m.BusinessNumber).ToList();
            var attachments = _mapper.Map<List<PaymentAttachmentDTO>>(AttachmentRepository.FindAll(m => businessNumbers.Contains(m.BusinessNumber)));
            var dtos = _mapper.Map<List<OfflineUsageBillDTO>>(entities);
            dtos.ForEach(k =>
            {
                k.Attachments = attachments.FirstOrDefault(m => m.BusinessNumber == k.BusinessNumber);
            });
            return dtos;
        }

        public List<OfflineUsageBillDTO> LoadExpenditureOfflineUsageBillByOrder(string orderId)
        {
            var entities = OfflineUsageBillRepository.FindAll(m => m.OrderId == orderId && m.Status != OfflinePaymentStatus.Cancel && m.AmountOfMoney < 0);
            var businessNumbers = entities.Select(m => m.BusinessNumber).ToList();
            var attachments = _mapper.Map<List<PaymentAttachmentDTO>>(AttachmentRepository.FindAll(m => businessNumbers.Contains(m.BusinessNumber)));
            var dtos = _mapper.Map<List<OfflineUsageBillDTO>>(entities);
            dtos.ForEach(k =>
            {
                k.Attachments = attachments.FirstOrDefault(m => m.BusinessNumber == k.BusinessNumber);
            });
            return dtos;
        }

        public List<OfflineUsageBillDTO> LoadIncomeOfflineUsageBillByOrder(string orderId)
        {
            var entities = OfflineUsageBillRepository.FindAll(m => m.OrderId == orderId && m.Status != OfflinePaymentStatus.Cancel && m.AmountOfMoney > 0);
            var businessNumbers = entities.Select(m => m.BusinessNumber).ToList();
            var attachments = _mapper.Map<List<PaymentAttachmentDTO>>(AttachmentRepository.FindAll(m => businessNumbers.Contains(m.BusinessNumber)));
            var dtos = _mapper.Map<List<OfflineUsageBillDTO>>(entities);
            dtos.ForEach(k =>
            {
                k.Attachments = attachments.FirstOrDefault(m => m.BusinessNumber == k.BusinessNumber);
            });
            return dtos;
        }

        public Tuple<string, string, bool> CancelOfflineUsageBill(int id, string remark, string @operator)
        {
            var entity = OfflineUsageBillRepository.GetById(id);
            if (entity == null)
                return null;

            var logDto = new PayableAndReceivableRecord
            {
                PayableNumber = entity.PayableNumber,
                AmountOfMoney = entity.AmountOfMoney,
                PayDateTime = DateTime.UtcNow,
                PaymentType = PaymentType.OfflineBill,
                Remark = @operator + "取消线下票据(" + entity.BillNumber + ")，金额为" + (-entity.AmountOfMoney).ToString("C"),
                Operator = @operator
            };
            PayableAndReceivableRecordRepository.Add(logDto, false);

            entity.SupplementRemark = remark;
            entity.Status = OfflinePaymentStatus.Cancel;
            var result = OfflineUsageBillRepository.Modify(entity, new[] { "SupplementRemark", "Status" });
            return result ? new Tuple<string, string, bool>(entity.BusinessNumber, entity.OrderId, entity.PayableSource == PayableSource.AccountStatement) : null;
        }

        /// <summary>
        /// 当买方取消后，需删除卖方的数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool RemoveOfflineUsageBill(string businessNumber)
        {
            AttachmentRepository.Remove(m => m.BusinessNumber == businessNumber, false);
            return OfflineUsageBillRepository.Remove(m => m.BusinessNumber == businessNumber) > 0;
        }


        /// <summary>
        /// 卖方退回之后，通过api调用修改买方数据
        /// </summary>
        /// <param name="businessNumber"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public ExecServiceResult ReturnOfflineUsageBill(string businessNumber, string remark, string @operator = null)
        {
            var entity = OfflineUsageBillRepository.GetByFilter(m => m.BusinessNumber == businessNumber);
            if (entity == null)
                return ExecServiceResult.NotFound;
            var logDto = new PayableAndReceivableRecord
            {
                PayableNumber = entity.PayableNumber,
                AmountOfMoney = entity.AmountOfMoney,
                PayDateTime = DateTime.UtcNow,
                PaymentType = PaymentType.OfflineBill,
                Remark = string.IsNullOrWhiteSpace(@operator) ? "对方退回线下票据(" + entity.BillNumber + ")，金额为" + (-entity.AmountOfMoney).ToString("C") : @operator + "退回对方线下票据(" + entity.BillNumber + ")，金额为" + (entity.AmountOfMoney).ToString("C"),
                Operator = string.IsNullOrWhiteSpace(@operator) ? "Robot" : @operator
            };
            PayableAndReceivableRecordRepository.Add(logDto, false);
            entity.SupplementRemark = remark;
            entity.Status = OfflinePaymentStatus.Reject;
            return OfflineUsageBillRepository.Modify(entity, new[] { "SupplementRemark", "Status" }) ? ExecServiceResult.Success : ExecServiceResult.Fail;
        }

        /// <summary>
        /// 卖方确认买方提交的线下支付记录，确认后需要调用api修改买方的数据
        /// </summary>
        /// <param name="businessNumber"></param>
        /// <returns></returns>
        public ExecServiceResult ConfirmOfflineUsageBill(string businessNumber)
        {
            var entity = OfflineUsageBillRepository.GetByFilter(m => m.BusinessNumber == businessNumber);
            if (entity == null)
                return ExecServiceResult.NotFound;
            entity.Status = OfflinePaymentStatus.Paid;
            //应收/应付记录
            var logDto = new PayableAndReceivableRecordDTO
            {
                PayableNumber = entity.PayableNumber,
                AmountOfMoney = entity.AmountOfMoney,
                PayDateTime = DateTime.UtcNow,
                PaymentType = PaymentType.OfflineBill,
                Remark = "对方确认线下票据(" + entity.BillNumber + ")，金额为" + (-entity.AmountOfMoney).ToString("C"),
                Operator = "Robot"
            };
            PayableAndReceivableRecordRepository.Add(_mapper.Map<PayableAndReceivableRecord>(logDto), false);
            var result = OfflineUsageBillRepository.Modify(entity, new[] { "Status" }) ? ExecServiceResult.Success : ExecServiceResult.Fail;
            return result;
        }

        public PaginatedBaseDTO<OfflineUsageBillDTO> LoadOfflinePayable(string orderId, string billNumber, int pageIndex, int pageSize)
        {
            return LoadOfflineReceviable(orderId, billNumber, true, pageIndex, pageSize);
        }

        public PaginatedBaseDTO<OfflineUsageBillDTO> LoadOfflineReceviable(string orderId, string billNumber, int pageIndex, int pageSize)
        {
            return LoadOfflineReceviable(orderId, billNumber, false, pageIndex, pageSize);
        }

        private PaginatedBaseDTO<OfflineUsageBillDTO> LoadOfflineReceviable(string orderId, string billNumber, bool payable, int pageIndex, int pageSize)
        {
            Expression<Func<OfflineUsageBill, bool>> predicate = m => m.Status == OfflinePaymentStatus.Paid;
            if (payable)
                predicate = predicate.And(m => m.AmountOfMoney < 0);
            else
                predicate = predicate.And(m => m.AmountOfMoney > 0);
            if (!string.IsNullOrWhiteSpace(orderId))
                predicate = predicate.And(m => m.OrderId == orderId);
            if (!string.IsNullOrWhiteSpace(billNumber))
                predicate = predicate.And(m => m.BillNumber == billNumber);
            var result = OfflineUsageBillRepository.FindPagenatedListWithCount<OfflineUsageBill>(pageIndex, pageSize, predicate, "Id", false);
            var bills = _mapper.Map<List<OfflineUsageBillDTO>>(result.Item2);
            if (bills.Any())
            {
                var businessNumbers = bills.Select(m => m.BusinessNumber).ToList();
                var attachments = _mapper.Map<List<PaymentAttachmentDTO>>(AttachmentRepository.FindAll(m => businessNumbers.Contains(m.BusinessNumber)));
                bills.ForEach(k =>
                {
                    k.Attachments = attachments.FirstOrDefault(m => m.BusinessNumber == k.BusinessNumber);
                });
            }
            return new PaginatedBaseDTO<OfflineUsageBillDTO>(pageIndex, pageSize, result.Item1, bills);
        }

        public bool ConfirmPay(int id, string operatorId, string operatorName)
        {
            var entity = OfflineUsageBillRepository.GetById(id);
            if (entity == null)
                return true;
            if (entity.AmountOfMoney > 0)//收款方
            {
                if (!entity.CashPayment.HasValue)
                    throw new BusinessPromptException("等待买方确认付款！");
                entity.CashPayment = true;
            }
            else
            {
                entity.CashPayment = false;
            }

            var result = OfflineUsageBillRepository.Modify(entity, new[] { "CashPayment" });
            if (result)
            {
                if (entity.AmountOfMoney > 0)
                {
                    var receivable = ReceivableRepository.GetByFilter(m => m.PayableNumber == entity.PayableNumber);
                    if (receivable == null)
                        return false;
                    //new OrderApiService(Tenant).ModifyBillCashPayment(entity.BusinessNumber, true, receivable.CustomerTenant);
                    //释放买方额度
                    //new Com.Service.Shop.WebApiService.Business.FinanceApiService(Tenant).ReleaseCredit(entity.OrderId, entity.CreditUsageDetailId, entity.AmountOfMoney, operatorId, operatorName);
                }
                else
                {
                    var payable = PayableRepository.GetByFilter(m => m.PayableNumber == entity.PayableNumber);
                    if (payable == null)
                        return false;
                    //new OrderApiService(Tenant).ModifyBillCashPayment(entity.BusinessNumber, false, payable.CustomerTenant);
                }
            }

            return result;
        }

        public bool ModifyBillCashPayment(string businessNumber, bool status)
        {
            var entity = OfflineUsageBillRepository.GetByFilter(m => m.BusinessNumber == businessNumber);
            if (entity == null)
                return true;
            entity.CashPayment = status;
            return OfflineUsageBillRepository.Modify(entity, new[] { "CashPayment" });
        }
    }
}
