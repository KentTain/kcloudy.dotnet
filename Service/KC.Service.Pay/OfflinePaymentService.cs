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

namespace KC.Service.Pay
{
    public interface IOfflinePaymentService : IEFService
    {
        OfflinePaymentDTO GetByBusinessNumber(string businessNumber);

        OfflinePaymentDTO GetByBusinessNumberIncludeAttachment(string businessNumber);

        bool AddOfflinePayment(OfflinePaymentDTO dto, string @operator = null);

        List<OfflinePaymentDTO> LoadOfflinePayment(string payableNumber);

        List<OfflinePaymentDTO> LoadExpenditureOfflinePaymentByOrder(string orderId);

        List<OfflinePaymentDTO> LoadIncomeOfflinePaymentByOrder(string orderId);

        Tuple<string, string, PayableSource> CancelOfflinePayment(int id, string remark, string @operator);

        bool RemoveOfflinePayment(string businessNumber);

        ExecServiceResult ReturnOfflinePayment(string businessNumber, string remark, string @operator = null);

        ExecServiceResult ConfirmOfflinePayment(string businessNumber, string @operator = null);
    }

    public class OfflinePaymentService : EFServiceBase, IOfflinePaymentService
    {
        private readonly IMapper _mapper;

        /// <summary>
        /// ComAccountUnitOfWorkContext
        ///     use AccountUnitOfWorkContext.Commit() to save the context change(transaction).
        /// </summary>
        protected EFUnitOfWorkContextBase _unitOfWorkContext { get; private set; }

        private IDbRepository<OfflinePayment> OfflinePaymentRepository;
        private IDbRepository<PayableAndReceivableRecord> PayableAndReceivableRecordRepository;
        private IDbRepository<PaymentAttachment> AttachmentRepository;
        

        public OfflinePaymentService(
            Tenant tenant,
            IMapper mapper,
            EFUnitOfWorkContextBase unitOfWorkContext,

            IDbRepository<OfflinePayment> offlinePaymentRepository,
            IDbRepository<PayableAndReceivableRecord> payableAndReceivableRecordRepository,
            IDbRepository<PaymentAttachment> attachmentRepository,

            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<PaymentService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfWorkContext = unitOfWorkContext;

            this.OfflinePaymentRepository = offlinePaymentRepository;
            this.PayableAndReceivableRecordRepository = payableAndReceivableRecordRepository;
            this.AttachmentRepository = attachmentRepository;
        }

        public OfflinePaymentDTO GetByBusinessNumber(string businessNumber)
        {
            var entity = OfflinePaymentRepository.GetByFilter(m => m.BusinessNumber == businessNumber);
            if (entity == null)
                return null;
            return _mapper.Map<OfflinePaymentDTO>(entity);
        }

        public OfflinePaymentDTO GetByBusinessNumberIncludeAttachment(string businessNumber)
        {
            var entity = OfflinePaymentRepository.GetByFilter(m => m.BusinessNumber == businessNumber);
            if (entity == null)
                return null;
            var attachment = _mapper.Map<PaymentAttachmentDTO>(AttachmentRepository.GetByFilter(m => businessNumber == m.BusinessNumber));
            var dto = _mapper.Map<OfflinePaymentDTO>(entity);
            dto.Attachments = attachment;
            return dto;
        }

        public bool AddOfflinePayment(OfflinePaymentDTO dto, string @operator = null)
        {
            if (!string.IsNullOrWhiteSpace(@operator))
            {
                var logDto = new PayableAndReceivableRecord
                {
                    PayableNumber = dto.PayableNumber,
                    AmountOfMoney = dto.AmountOfMoney,
                    PayDateTime = DateTime.UtcNow,
                    PaymentType = PaymentType.OfflinePayment,
                    Remark = @operator + "提交线下支付，付款金额为" + (-dto.AmountOfMoney).ToString("C"),
                    Operator = @operator
                };
                PayableAndReceivableRecordRepository.Add(logDto, false);
            }
            AttachmentRepository.Add(_mapper.Map<PaymentAttachment>(dto.Attachments), false);
            return OfflinePaymentRepository.Add(_mapper.Map<OfflinePayment>(dto));
        }

        public List<OfflinePaymentDTO> LoadOfflinePayment(string payableNumber)
        {
            var entities = OfflinePaymentRepository.FindAll(m => m.PayableNumber == payableNumber);
            var businessNumbers = entities.Select(m => m.BusinessNumber).ToList();
            var attachments = _mapper.Map<List<PaymentAttachmentDTO>>(AttachmentRepository.FindAll(m => businessNumbers.Contains(m.BusinessNumber)));
            var dtos = _mapper.Map<List<OfflinePaymentDTO>>(entities);
            dtos.ForEach(k =>
            {
                k.Attachments = attachments.FirstOrDefault(m => m.BusinessNumber == k.BusinessNumber);
            });
            return dtos;
        }

        public List<OfflinePaymentDTO> LoadExpenditureOfflinePaymentByOrder(string orderId)
        {
            var entities = OfflinePaymentRepository.FindAll(m => m.OrderId == orderId && m.Status != OfflinePaymentStatus.Cancel && m.AmountOfMoney < 0);
            var businessNumbers = entities.Select(m => m.BusinessNumber).ToList();
            var attachments = _mapper.Map<List<PaymentAttachmentDTO>>(AttachmentRepository.FindAll(m => businessNumbers.Contains(m.BusinessNumber)));
            var dtos = _mapper.Map<List<OfflinePaymentDTO>>(entities);
            dtos.ForEach(k =>
            {
                k.Attachments = attachments.FirstOrDefault(m => m.BusinessNumber == k.BusinessNumber);
            });
            return dtos;
        }

        public List<OfflinePaymentDTO> LoadIncomeOfflinePaymentByOrder(string orderId)
        {
            var entities = OfflinePaymentRepository.FindAll(m => m.OrderId == orderId && m.Status != OfflinePaymentStatus.Cancel && m.AmountOfMoney > 0);
            var businessNumbers = entities.Select(m => m.BusinessNumber).ToList();
            var attachments = _mapper.Map<List<PaymentAttachmentDTO>>(AttachmentRepository.FindAll(m => businessNumbers.Contains(m.BusinessNumber)));
            var dtos = _mapper.Map<List<OfflinePaymentDTO>>(entities);
            dtos.ForEach(k =>
            {
                k.Attachments = attachments.FirstOrDefault(m => m.BusinessNumber == k.BusinessNumber);
            });
            return dtos;
        }

        public Tuple<string, string, PayableSource> CancelOfflinePayment(int id, string remark, string @operator)
        {
            var entity = OfflinePaymentRepository.GetById(id);
            if (entity == null)
                return null;
            var logDto = new PayableAndReceivableRecord
            {
                PayableNumber = entity.PayableNumber,
                AmountOfMoney = entity.AmountOfMoney,
                PayDateTime = DateTime.UtcNow,
                PaymentType = PaymentType.OfflinePayment,
                Remark = @operator + "取消线下支付，付款金额为" + (-entity.AmountOfMoney).ToString("C"),
                Operator = @operator
            };
            PayableAndReceivableRecordRepository.Add(logDto, false);
            entity.SupplementRemark = remark;
            entity.Status = OfflinePaymentStatus.Cancel;
            var result = OfflinePaymentRepository.Modify(entity, new[] { "SupplementRemark", "Status" });
            return result ? new Tuple<string, string, PayableSource>(entity.BusinessNumber, entity.OrderId, entity.PayableSource ?? PayableSource.PO) : null;
        }

        /// <summary>
        /// 当买方取消后，需删除卖方的数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool RemoveOfflinePayment(string businessNumber)
        {
            AttachmentRepository.Remove(m => m.BusinessNumber == businessNumber, false);
            return OfflinePaymentRepository.Remove(m => m.BusinessNumber == businessNumber) > 0;
        }


        /// <summary>
        /// 卖方退回之后，通过api调用修改买方数据
        /// </summary>
        /// <param name="businessNumber"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public ExecServiceResult ReturnOfflinePayment(string businessNumber, string remark, string @operator = null)
        {
            var entity = OfflinePaymentRepository.GetByFilter(m => m.BusinessNumber == businessNumber);
            if (entity == null)
                return ExecServiceResult.NotFound;
            entity.SupplementRemark = remark;
            entity.Status = OfflinePaymentStatus.Reject;
            var logDto = new PayableAndReceivableRecord
            {
                PayableNumber = entity.PayableNumber,
                AmountOfMoney = entity.AmountOfMoney,
                PayDateTime = DateTime.UtcNow,
                PaymentType = PaymentType.OfflinePayment,
                Remark = string.IsNullOrWhiteSpace(@operator) ? "对方退回线下支付，金额:" + (-entity.AmountOfMoney).ToString("C") : @operator + "退回线下支付，金额:" + (entity.AmountOfMoney).ToString("C"),
                Operator = string.IsNullOrWhiteSpace(@operator) ? "Robot" : @operator
            };
            PayableAndReceivableRecordRepository.Add(logDto, false);
            return OfflinePaymentRepository.Modify(entity, new[] { "SupplementRemark", "Status" }) ? ExecServiceResult.Success : ExecServiceResult.Fail;
        }

        /// <summary>
        /// 卖方确认买方提交的线下支付记录，确认后需要调用api修改买方的数据
        /// </summary>
        /// <param name="businessNumber"></param>
        /// <returns></returns>
        public ExecServiceResult ConfirmOfflinePayment(string businessNumber, string @operator = null)
        {
            var entity = OfflinePaymentRepository.GetByFilter(m => m.BusinessNumber == businessNumber);
            if (entity == null)
                return ExecServiceResult.NotFound;
            entity.Status = OfflinePaymentStatus.Paid;

            //应收/应付记录
            var logDto = new PayableAndReceivableRecord
            {
                PayableNumber = entity.PayableNumber,
                AmountOfMoney = entity.AmountOfMoney,
                PayDateTime = DateTime.UtcNow,
                PaymentType = PaymentType.OfflinePayment,
                Remark = string.IsNullOrWhiteSpace(@operator) ? "对方确认线下支付，金额:" + (-entity.AmountOfMoney).ToString("C") : @operator + "确认线下支付，金额:" + (entity.AmountOfMoney).ToString("C"),
                Operator = string.IsNullOrWhiteSpace(@operator) ? "Robot" : @operator
            };
            PayableAndReceivableRecordRepository.Add(logDto, false);
            return OfflinePaymentRepository.Modify(entity, new[] { "Status" }) ? ExecServiceResult.Success : ExecServiceResult.Fail;
        }
    }

    public enum ExecServiceResult
    {
        /// <summary>
        /// 成功
        /// </summary>
        [Description("成功")]
        Success = 0,
        /// <summary>
        /// 未找到
        /// </summary>
        [Description("未找到")]
        NotFound = 1,

        /// <summary>
        /// 失败"
        /// </summary>
        [Description("失败")]
        Fail = 2
    }
}
