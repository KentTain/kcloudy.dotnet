using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO.Pay.CMB
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class CMBPaySearchDTO : PayBaseParamDTO
    {
        /// <summary>
        /// 支付订单号
        /// </summary>
        [DataMember]
        public string PaymentOrderId { get; set; }

        /// <summary>
        /// CBS返回的ID
        /// </summary>
        [DataMember]
        public string CBSReturnId { get; set; }

        /// <summary>
        /// 支付记录表
        /// </summary>
        [DataMember]
        public List<OnlinePaymentRecordDTO> OnlinePaymentRecord { get; set; }

        /// <summary>
        /// 支付流水
        /// </summary>
        [DataMember]
        public PaymentTradeRecordDTO PaymentTradeRecord { get; set; }

    }
}
