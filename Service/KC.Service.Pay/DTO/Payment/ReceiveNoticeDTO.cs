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
    public class ReceiveNoticeDTO : PayBaseParamDTO
    {
        public string ptncode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string trdcode { get; set; }

        /// <summary>
        /// xml文本
        /// </summary>
        [DataMember]
        public string message { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        [DataMember]
        public string signature { get; set; }

        /// <summary>
        /// 支付记录表
        /// </summary>
        [DataMember]
        public OnlinePaymentRecordDTO OnlinePaymentRecord { get; set; }

        /// <summary>
        /// 支付流水
        /// </summary>
        [DataMember]
        public PaymentTradeRecordDTO PaymentTradeRecord { get; set; }

        /// <summary>
        /// 中金支付资金账户表
        /// </summary>
        [DataMember]
        public PaymentBankAccountDTO PaymentBankAccount { get; set; }
    }
}
