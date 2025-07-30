using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO.Pay
{
    /// <summary>
    /// 中金支付BaseDTO
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class CPCNBaseDTO
    {
        /// <summary>
        /// 中金支付交易是否成功
        /// </summary>
        [DataMember]
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 支付流水
        /// </summary>
        [DataMember]
        public PaymentTradeRecordDTO PaymentTradeRecord { get; set; }
    }

}
