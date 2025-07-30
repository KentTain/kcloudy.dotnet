using KC.Framework.Extension;
using KC.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.Pay;

namespace KC.Service.DTO.Pay
{
    [Serializable, DataContract(IsReference = true)]
    public class PaymentSuccessRecordDTO : EntityDTO
    {

        public PaymentSuccessRecordDTO()
        {
            OrderNumbers = new Dictionary<string, decimal>();
        }

        [DataMember]
        public DateTime PayDateTime { get; set; }

        [DataMember]
        public decimal PayAmount { get; set; }

        [DataMember]
        public string PayNumber { get; set; }

        /// <summary>
        /// 应付单号
        /// </summary>
        [DataMember]
        public string PayableNumber { get; set; }

        /// <summary>
        /// 应付类型
        /// </summary>
        [DataMember]
        public CashType PayableType { get; set; }

        [DataMember]
        public string BillNumber { get; set; }

        [DataMember]
        public string BillType { get; set; }

        [DataMember]
        public string AccountStatementId { get; set; }

        [DataMember]
        public string AccountStatementNumber { get; set; }

        [DataMember]
        public string VoucherId { get; set; }

        [DataMember]
        public string VoucherNumber { get; set; }

        [DataMember]
        public string PayerTenant { get; set; }

        [DataMember]
        public string Payer { get; set; }

        [DataMember]
        public string PayeeTenant { get; set; }

        [DataMember]
        public string Payee { get; set; }

        [DataMember]
        public PaymentType PaymentType { get; set; }

        [DataMember]
        public Dictionary<string,decimal> OrderNumbers { get; set; }

        [DataMember]
        public string OrderNumber { get; set; }

        [DataMember]
        public string AttachmentUrl { get; set; }

        [DataMember]
        public string Remark { get; set; }

        public string PaymentTypeName
        {
            get
            {
                return PaymentType.ToDescription();
            }
        }


        public string PayDateTimeStr
        {
            get
            {
                return PayDateTime.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
    }
}
