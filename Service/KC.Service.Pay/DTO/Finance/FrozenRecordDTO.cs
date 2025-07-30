using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.Pay;
using KC.Service.DTO;

namespace KC.Service.DTO.Pay
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class FrozenRecordDTO : EntityDTO
    {
        [DataMember]
        public CashType CashType { get; set; }

        [DataMember]
        public string OrderId { get; set; }

        [DataMember]
        public string OrderAmount { get; set; }

        [DataMember]
        public string ConsumptionDate { get; set; }

        [DataMember]
        public string ConsumptionMoney { get; set; }

        /// <summary>
        /// 收款人userId
        /// </summary>
        [DataMember]
        public string PayeeCustomerId { get; set; }

        /// <summary>
        /// 支付人userId
        /// </summary>
        [DataMember]
        public string PaymentCustomerId { get; set; }

        public string TypeName
        {
            get
            {
                if (CashType == CashType.Withdrawals)
                {
                    if (PayeeCustomerId.Equals(PaymentCustomerId))
                        return "提现";
                    return "提现手续费";
                }
                return "销售订单";
            }
        }

        public string Status
        {
            get
            {
                if (CashType == CashType.Withdrawals)
                    return "提现中";
                return "交付中";
            }
        }
    }
}
