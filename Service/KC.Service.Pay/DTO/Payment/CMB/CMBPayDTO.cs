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
    public class CMBPayDTO : PayBaseParamDTO
    {
        public CMBPayDTO()
        {
            CCYNBR = "10";
        }
        /// <summary>
        /// 支付订单号
        /// </summary>
        [DataMember]
        public string PaymentOrderId { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        [DataMember]
        public string OrderNo { get; set; }

        /// <summary>
        /// 付款金额
        /// </summary>
        [DataMember]
        public decimal Amount { get; set; }

        /// <summary>
        /// 付款方账号
        /// </summary>
        [DataMember]
        public string AccountNumber { get; set; }

        /// <summary>
        /// 付款账号名
        /// </summary>
        [DataMember]
        public string AccountName { get; set; }

        /// <summary>
        /// 付款方银行+卡号
        /// </summary>
        [DataMember]
        public string BankName { get; set; }

        /// <summary>
        /// 收款人的memberId
        /// </summary>
        [DataMember]
        public string PayeeMemberId { get; set; }

        /// <summary>
        /// 用途
        /// </summary>
        [DataMember]
        public string Usage { get; set; }

        /// <summary>
        /// 付方客户号
        /// </summary>
        [DataMember]
        public string CLTNBR { get; set; }

        /// <summary>
        /// 币种默认人民币 ：10
        /// </summary>
        [DataMember]
        public string CCYNBR { get; set; }

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
    }
}
