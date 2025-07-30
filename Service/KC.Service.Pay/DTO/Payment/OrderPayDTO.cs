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
    public class OrderPayDTO : PayBaseParamDTO
    {
        /// <summary>
        /// 支付订单号
        /// </summary>
        [DataMember]
        [Required]
        public string PaymentOrderId { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        [DataMember]
        [Required]
        public string OrderNo { get; set; }

        /// <summary>
        /// 支付订单类型
        /// </summary>
        [DataMember]
        [Required]
        public int PaymentType { get; set; }

        /// <summary>
        /// 付款金额
        /// </summary>
        [DataMember]
        [Required]
        public decimal Amount { get; set; }

        /// <summary>
        /// 收款方账号
        /// </summary>
        [DataMember]
        [Required]
        public string PayeeAccountNumber { get; set; }

        /// <summary>
        /// 收付款账号名
        /// </summary>
        [DataMember]
        [Required]
        public string PayeeAccountName { get; set; }

        /// <summary>
        /// 付款方式 1  普通订单支付 2收款方收款成功后，再冻结资金
        /// </summary>
        [DataMember]
        [Required]
        public int PayType { get; set; }

        /// <summary>
        /// 用途
        /// </summary>
        [DataMember]
        [Required]
        public string Usage { get; set; }

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

        /// <summary>
        /// 收款方自己信息
        /// </summary>
        [DataMember]
        public PaymentBankAccountDTO PeePaymentBankAccount { get; set; }
    }
}