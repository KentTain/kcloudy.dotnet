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
    public class OfflinePaymentDTO : EntityDTO
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

        [DataMember]
        public PaymentAttachmentDTO Attachments { get; set; }
    }
}
