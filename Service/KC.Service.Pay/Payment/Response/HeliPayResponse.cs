using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Pay.Response
{
    public class HeliPayResponse
    {
        /// <summary>
        /// 收银台支付接口
        /// </summary>
        [DataMember]
        public string rt1_bizType { get; set; }

        /// <summary>
        /// 返回码
        /// </summary>
        [DataMember]
        public string rt2_retCode { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>
        [DataMember]
        public string rt3_retMsg { get; set; }

        /// <summary>
        /// 商户编号
        /// </summary>

        [DataMember]
        public string rt4_customerNumber { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        [DataMember]
        public string rt5_userId { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        [DataMember]
        public string rt6_orderId { get; set; }

        /// <summary>
        /// 订单金额
        /// </summary>
        [DataMember]
        public string rt7_orderAmount { get; set; }

        /// <summary>
        /// 订单状态 INIT：未支付 SUCCESS:已支付 FAILED：失败 DOING：处理中
        /// </summary>
        [DataMember]
        public string rt8_orderStatus { get; set; }

        /// <summary>
        /// 合利宝平台唯一流水号
        /// </summary>
        [DataMember]
        public string rt9_serialNumber { get; set; }

        /// <summary>
        /// 订单完成时间   yyyy-MM-dd HH:mm:ss
        /// </summary>

        [DataMember]
        public string rt10_completeDate { get; set; }

        /// <summary>
        /// 产品编码 PC_CASHIER： 收银台
        /// </summary>
        [DataMember]
        public string rt11_productCode { get; set; }

        /// <summary>
        /// 备注 URLEncoder.encode(“备注”, “utf-8”)
        /// </summary>
        [DataMember]
        public string rt12_desc { get; set; }

        /// <summary> 
        /// 签名类型 签名加密方式，使用MD5加密
        /// </summary>
        [DataMember]
        public string signType { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        [DataMember]
        public string sign { get; set; }

    }

}
