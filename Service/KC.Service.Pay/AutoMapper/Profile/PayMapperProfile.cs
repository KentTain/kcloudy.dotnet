using AutoMapper;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Framework.Base;
using KC.Service.DTO;
using KC.Model.Pay;
using KC.Service.DTO.Pay;

namespace KC.Service.Pay.AutoMapper.Profile
{
    public partial class PayMapperProfile : global::AutoMapper.Profile
    {
        public PayMapperProfile()
        {
            CreateMap<Entity, EntityDTO>();
            CreateMap<EntityDTO, Entity>();

            CreateMap<ProcessLogBase, ProcessLogBaseDTO>()
                .ForMember(target => target.TypeString, config => config.Ignore());
            CreateMap<ProcessLogBaseDTO, ProcessLogBase>();

            //CreateMap<TreeNode, TreeNodeDTO>()
            //    .IncludeBase<Entity, EntityDTO>();
            //CreateMap<TreeNodeDTO, TreeNode>()
            //    .IncludeBase<EntityDTO, Entity>();

            CreateMap<BankAccount, BankAccountDTO>();
            CreateMap<BankAccountDTO, BankAccount>();

            CreateMap<PaymentBankAccount, PaymentBankAccountDTO>()
                .ForMember(target => target.StateStr, config => config.Ignore())
                .ForMember(target => target.PaymentTypeStr, config => config.Ignore())
                .ForMember(target => target.IsError, config => config.Ignore());
            CreateMap<PaymentBankAccountDTO, PaymentBankAccount>();


            CreateMap<PaymentTradeRecord, PaymentTradeRecordDTO>();
            CreateMap<PaymentTradeRecordDTO, PaymentTradeRecord>();

            CreateMap<OnlinePaymentRecord, OnlinePaymentRecordDTO>();
            CreateMap<OnlinePaymentRecordDTO, OnlinePaymentRecord>();

            CreateMap<PaymentInfo, PaymentOperationLogDTO>();
            CreateMap<PaymentOperationLogDTO, PaymentInfo>();


            #region 应收应付

            CreateMap<Payable, PayableDTO>()
                .IncludeBase<Entity, EntityDTO>();
            CreateMap<PayableDTO, Payable>()
                .IncludeBase<EntityDTO, Entity>();

            CreateMap<Receivable, ReceivableDTO>()
                .IncludeBase<Entity, EntityDTO>();
            CreateMap<ReceivableDTO, Receivable>()
                .IncludeBase<EntityDTO, Entity>();

            CreateMap<PayableAndReceivableRecord, PayableAndReceivableRecordDTO>()
                .IncludeBase<Entity, EntityDTO>();
            CreateMap<PayableAndReceivableRecordDTO, PayableAndReceivableRecord>()
                .IncludeBase<EntityDTO, Entity>();

            CreateMap<EntrustedPaymentRecord, EntrustedPaymentDTO>()
                .IncludeBase<Entity, EntityDTO>();
            #endregion

            #region 线下支付

            CreateMap<OfflinePayment, OfflinePaymentDTO>()
               .IncludeBase<Entity, EntityDTO>();
            CreateMap<OfflinePaymentDTO, OfflinePayment>()
                .IncludeBase<EntityDTO, Entity>();
            CreateMap<OfflineUsageBill, OfflineUsageBillDTO>()
               .IncludeBase<Entity, EntityDTO>();
            CreateMap<OfflineUsageBillDTO, OfflineUsageBill>()
                .IncludeBase<EntityDTO, Entity>();

            CreateMap<PaymentAttachment, PaymentAttachmentDTO>()
                .IncludeBase<Entity, EntityDTO>();
            CreateMap<PaymentAttachmentDTO, PaymentAttachment>()
                .IncludeBase<EntityDTO, Entity>();
            #endregion

            #region 申请付款记录

            CreateMap<PaymentRecord, PaymentApplyRecordDTO>()
                .IncludeBase<Entity, EntityDTO>();
            CreateMap<PaymentApplyRecordDTO, PaymentRecord>()
                .IncludeBase<EntityDTO, Entity>();
            CreateMap<PaymentOperationLogDTO, PaymentOperationLog>()
                .IncludeBase<ProcessLogBaseDTO, ProcessLogBase>();
            #endregion

            #region 白条记录

            CreateMap<VoucherPaymentRecord, VoucherPaymentRecordDTO>()
                .IncludeBase<Entity, EntityDTO>();
            CreateMap<VoucherPaymentRecordDTO, VoucherPaymentRecord>()
                .IncludeBase<EntityDTO, Entity>();

            #endregion
        }
    }
}
