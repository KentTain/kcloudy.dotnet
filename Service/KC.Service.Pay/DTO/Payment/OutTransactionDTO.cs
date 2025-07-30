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
    public class OutTransactionDTO : PayBaseParamDTO
    {

        /// <summary>
        /// 支付订单号
        /// </summary>
        [DataMember]
        [Required]
        public string PaymentOrderId { get; set; }

        /// <summary>
        /// 出金金额
        /// </summary>
        [DataMember]
        [Required]
        public decimal Amount { get; set; }

        /// <summary>
        /// 出金标识  1：T0出金 2:T1出金（默认）
        /// </summary>
        [DataMember]
        [Required]
        public int BalFlag { get; set; }

        /// <summary>
        /// 用途
        /// </summary>
        [DataMember]
        [Required]
        public string Usage { get; set; }

        /// <summary>
        /// 手续费
        /// </summary>
        [DataMember]
        [Required]
        public decimal FeeAmount { get; set; }

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
