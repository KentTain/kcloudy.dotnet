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
    public class FreezeAmtDTO : PayBaseParamDTO
    {
        /// <summary>
        /// 支付订单号
        /// </summary>
        [DataMember]
        [Required]
        public string PaymentOrderId { get; set; }
        /// <summary>
        /// 解冻金额
        /// </summary>
        [DataMember]
        [Required]
        public decimal FreezeAmt { get; set; }

        /// <summary>
        /// 1 冻结 2 解冻
        /// </summary>
        [DataMember]
        [Required]
        public int BusiType { get; set; }

        /// <summary>
        /// 用途
        /// </summary>
        [DataMember]
        [Required]
        public string Usage { get; set; }

        /// <summary>
        /// 解冻码，渤海银行 渤海解冻必填 
        /// </summary>
        [DataMember]
        public string FrzenCode { get; set; }

        /// <summary>
        /// 是否渤海云账本 渤海必填
        /// </summary>
        [DataMember]
        public bool IsBoHai { get; set; }

        /// <summary>
        /// 订单编号 渤海必填
        /// </summary>
        [DataMember]
        public string OrderNo { get; set; }
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
        /// 支付记录表
        /// </summary>
        [DataMember]
        public OnlinePaymentRecordDTO OnlinePaymentRecord { get; set; }

    }
}