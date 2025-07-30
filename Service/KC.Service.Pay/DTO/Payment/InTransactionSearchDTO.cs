using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO.Pay
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class InTransactionSearchDTO : PayBaseParamDTO
    {
        /// <summary>
        /// T2006 交易请求报文中合作方交易流水号
        /// </summary>
        [DataMember]
        [Required]
        public string ScrSrl { get; set; }

        /// <summary>
        /// 支付订单号
        /// </summary>
        [DataMember]
        public string PaymentOrderId { get; set; }

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
