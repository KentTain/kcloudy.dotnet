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
    public class PayableAndReceivableRecordDTO : EntityDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string PayableNumber { get; set; }

        /// <summary>
        /// 付款收款时间
        /// </summary>
        [DataMember]
        public DateTime PayDateTime { get; set; }

        public string PayDateTimeStr
        {
            get
            {

                return PayDateTime.AddHours(8).ToString("yyyy-MM-dd HH:mm");
            }
        }

        /// <summary>
        /// 付款收款金额
        /// </summary>
        [DataMember]
        public decimal AmountOfMoney { get; set; }

        [DataMember]
        public PaymentType PaymentType { get; set; }

        public string PaymentTypeStr
        {
            get
            {
                return PaymentType.ToDescription();
            }
        }

        [DataMember]
        public string Remark { get; set; }

        [DataMember]
        public string Operator{ get; set; }
    }
}
