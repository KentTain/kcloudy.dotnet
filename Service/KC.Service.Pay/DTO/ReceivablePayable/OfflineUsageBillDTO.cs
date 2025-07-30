using KC.Enums.Pay;
using KC.Framework.Extension;
using KC.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO.Pay
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class OfflineUsageBillDTO : EntityDTO
    {
        public int Id { get; set; }

        [DataMember]
        public string OrderId { get; set; }

        [DataMember]
        public string PayableNumber { get; set; }

        [DataMember]
        public DateTime PayDateTime { get; set; }

        /// <summary>
        /// +收入，-支出
        /// </summary>
        [DataMember]
        public decimal AmountOfMoney { get; set; }

        [DataMember]
        public OfflinePaymentStatus Status { get; set; }

        /// <summary>
        /// 票据号
        /// </summary>
        [DataMember]
        public string BillNumber { get; set; }

        /// <summary>
        /// 票据类型，true:银票，false:商票
        /// </summary>
        [DataMember]
        public bool BankBill { get; set; }

        [DataMember]
        public string Remark { get; set; }

        [DataMember]
        public string SupplementRemark { get; set; }

        public string PayDateTimeStr
        {
            get
            {

                return PayDateTime.AddHours(8).ToString("yyyy-MM-dd");
            }
        }

        public string StatusStr
        {
            get
            {
                if (AmountOfMoney < 0)
                    return Status.ToDescription();
                else
                {
                    switch (Status)
                    {
                        default:
                        case OfflinePaymentStatus.WaitConfirm:
                            return "待确认";
                        case OfflinePaymentStatus.Paid:
                            return "已确认";
                        case OfflinePaymentStatus.Reject:
                            return "已退回";
                    }
                }
            }
        }

        public string BillType
        {
            get
            {
                return BankBill ? "银行承兑汇票" : "商业承兑汇票";
            }
        }
        [DataMember]
        public string BusinessNumber { get; set; }

        [DataMember]
        public ReceivableSource? ReceivableSource { get; set; }

        [DataMember]
        public PayableSource? PayableSource { get; set; }

        /// <summary>
        /// 付款方/收款方
        /// </summary>
        [DataMember]
        public string Customer { get; set; }

        /// <summary>
        /// 是否兑付,null:等待买方确认付款，false:等待卖方确认收款，true:买方已确认付款或卖方已确认收款
        /// </summary>
        public bool? CashPayment { get; set; }

        [DataMember]
        public PaymentAttachmentDTO Attachments { get; set; }
    }
}
