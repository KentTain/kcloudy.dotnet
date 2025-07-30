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
    public class InTransactionDTO : PayBaseParamDTO
    {
        /// <summary>
        /// 充值金额
        /// </summary>
        [DataMember]
        [Required]

        public Decimal Amount { get; set; }

        /// <summary>
        /// 用途
        /// </summary>
        [DataMember]
        [Required]
        public string Usage { get; set; }

        /// <summary>
        /// 支付订单号
        /// </summary>
        [DataMember]
        [Required]
        public string PaymentOrderId { get; set; }

        [DataMember]
        public string PeeMemberId { get; set; }

        [DataMember]
        public string OrderNo { get; set; }

        /// <summary>
        /// 扫码支付 3：支付宝  4：微信
        /// </summary>
        [DataMember]
        public int SecPayType { get; set; }

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