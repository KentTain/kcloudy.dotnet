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
    public class PaymentBHFreezeDTO : EntityDTO
    {
        [DataMember]
        public int Id { get; set; }


        [DataMember]
        public string MemberId { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>
        [DataMember]
        public string PaymentOrderId { get; set; }

        /// <summary>
        /// 商家支付订单号
        /// </summary>
        [DataMember]
        public string OrderNo { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        [DataMember]

        public decimal Amount { get; set; }

        /// <summary>
        /// 标记 1.冻结 2，解冻
        /// </summary>
        [DataMember]
        public int Flag { get; set; }

        /// <summary>
        /// 冻结码
        /// </summary>
        [DataMember]
        public string FrzenCode { get; set; }

        /// <summary>
        /// 对应的冻结ID
        /// </summary>
        [DataMember]
        public int FrzenId { get; set; }

        [DataMember]
        public string Remark { get; set; }
    }
}
