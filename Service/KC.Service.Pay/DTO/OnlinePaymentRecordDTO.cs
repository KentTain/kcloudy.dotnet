using KC.Service.DTO;
using KC.Enums.Pay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO.Pay
{
    [Serializable, DataContract(IsReference = true)]
    public class OnlinePaymentRecordDTO : EntityDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]  //交易号
        public string PaymentOrderId { get; set; }

        [DataMember] //商家支付订单号
        public string OrderNo { get; set; }

        [DataMember]  //订单金额,单位分
        public decimal OrderAmount { get; set; }

        [DataMember] //网关支付金额
        public decimal PaymentAmount { get; set; }

        [DataMember] //订单提交时间,yyyyMMddHHmmss
        public string OrderDatetime { get; set; }

        [DataMember]  //网关支付时间,yyyyMMddHHmmss
        public string PayDatetime { get; set; }

        [DataMember]  //yyyyMMddHHmmss
        public string ReturnDatetime { get; set; }

        [DataMember]   //订单支付状态
        public string PayResult { get; set; }

        [DataMember]   //验签结果
        public string VerifyResult { get; set; }

        [DataMember]
        public string ErrorCode { get; set; }

        //错误信息
        [DataMember]
        public string ErrorMessage { get; set; }

        [DataMember]
        public string BankName { get; set; }

        [DataMember]
        public string BankNumber { get; set; }

        [DataMember]
        public string CurrencyType { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        [DataMember]
        public string PaymentMethod { get; set; }

        [DataMember]
        public int ConfigId { get; set; }

        [DataMember]
        public string MemberId { get; set; }

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
        /// 第三方支付的类型
        /// </summary>
        [DataMember]
        public ThirdPartyType PaymentType { get; set; }

        /// <summary>
        /// 支付的操作类型 入金 出金 订单支付 冻结/解冻资金
        /// </summary>
        [DataMember]
        public PaymentOperationType OperationType { get; set; }
    }
}
