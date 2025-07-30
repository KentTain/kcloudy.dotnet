using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.Pay;
using KC.Framework.Base;
using KC.Model.Pay.Constants;

namespace KC.Model.Pay
{
    [Serializable]
    [DataContract(IsReference = true)]
    [Table(Tables.OnlinePaymentRecord)]
    public class OnlinePaymentRecord : Entity
    {
        [Key]
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        [MaxLength(128)]  //交易号
        public string PaymentOrderId { get; set; }

        [DataMember]
        [Required]
        [MaxLength(128)] //商家支付订单号
        public string OrderNo { get; set; }

        [DataMember]
        [Required]   //订单金额,单位分
        public decimal OrderAmount { get; set; }

        [DataMember] //网关支付金额
        public decimal PaymentAmount { get; set; }

        [DataMember]
        [MaxLength(128)]  //订单提交时间,yyyyMMddHHmmss
        public string OrderDatetime { get; set; }

        [DataMember]
        [MaxLength(128)]  //网关支付时间,yyyyMMddHHmmss
        public string PayDatetime { get; set; }

        [DataMember]
        [MaxLength(128)]  //yyyyMMddHHmmss
        public string ReturnDatetime { get; set; }

        [DataMember]
        [MaxLength(128)]   //订单支付状态 0:初始状态，1：成功，2：失败，3：正在处理
        public string PayResult { get; set; }

        [DataMember]
        [MaxLength(128)]   //验签结果 中金扫码支付时存的是返回的二维码的URL
        public string VerifyResult { get; set; }

        /// <summary>
        /// 000000 为成功
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        public string ErrorCode { get; set; }

        //错误信息
        [DataMember]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 中金扫码支付时存的值是  微信扫码入金/支付宝扫码入金
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        public string BankName { get; set; }

        /// <summary>
        /// 平台流水号
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        public string BankNumber { get; set; }

        /// <summary>
        /// 币种 默认CNY
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        public string CurrencyType { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        public string PaymentMethod { get; set; }

        [DataMember]
        public int ConfigId { get; set; }

        [DataMember]
        public string MemberId { get; set; }

        /// <summary>
        /// 收款方
        /// </summary>
        [DataMember]
        public string PeeMemberId { get; set; }

        /// <summary>
        /// 查询的次数
        /// </summary>
        [DataMember]
        public int SearchCount { get; set; }

        /// <summary>
        /// 最后一次查询时间
        /// </summary>
        [DataMember]
        public DateTime NextSearchTime { get; set; }

        /// <summary>
        /// 支付的操作类型 入金 出金 订单支付 冻结/解冻资金
        /// </summary>
        [DataMember]
        public PaymentOperationType OperationType { get; set; }

        /// <summary>
        /// 第三方支付的类型
        /// </summary>
        [DataMember]
        public ThirdPartyType PaymentType { get; set; }

    }
}
