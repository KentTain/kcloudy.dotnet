using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Pay.Response
{
    /// <summary>
    /// 富友支付返回的数据
    /// </summary>
    public class FuioupayResponse
    {
        /// <summary>
        /// 富友分配给各合作商户的唯一识别码,0002900F0006944
        /// </summary>
        [DataMember]
        public string mchnt_cd { get; set; }

        /// <summary>
        /// 客户支付后商户网站产生的一个唯一的定单号，该订单号应该在相当长的时间内不重复。富友通过订单号来唯一确认一笔订单的重复性。
        /// </summary>
        [DataMember]
        public string order_id { get; set; }

        /// <summary>
        /// YYYYMMDD,20110322
        /// </summary>
        [DataMember]
        public string order_date { get; set; }

        /// <summary>
        /// 客户支付订单的总金额，一笔订单一个，以分为单位。不可以为零，必需符合金额标准。
        /// </summary>
        [DataMember]
        public string order_amt { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        [DataMember]
        public string order_st { get; set; }

        /// <summary>
        /// 0000 表示成功 其他失败
        /// </summary>
        [DataMember]
        public string order_pay_code { get; set; }

        /// <summary>
        /// 错误中文描述
        /// </summary>
        [DataMember]
        public string order_pay_error { get; set; }

        /// <summary>
        /// 保留字段
        /// </summary>
        [DataMember]
        public string resv1 { get; set; }

        /// <summary>
        /// 富友流水号,供商户查询支付交易状态及对账用
        /// </summary>
        [DataMember]
        public string fy_ssn { get; set; }

        /// <summary>
        /// MD5 摘要数据
        /// </summary>
        [DataMember]
        public string md5 { get; set; }
    }
}