using System;
using System.Runtime.Serialization;
using ProtoBuf;

namespace KC.Service.Pay.Config
{/// <summary>
    /// 通联支付配置
    /// </summary>
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class AllinpayConfig
    {
        /// <summary>
        /// 提交地址
        /// </summary>
        [DataMember]
        public string PostUrl { get; set; }

        /// <summary>
        /// 字符集,默认填 1；1 代表 UTF-8、2 代表 GBK、3 代表GB2312；
        /// </summary>
        [DataMember]
        public string InputCharset { get; set; }

        /// <summary>
        /// 跳转地址，客户的取货地址
        /// </summary>
        [DataMember]
        public string PickupUrl { get; set; }
        /// <summary>
        /// 通知地址，通知商户网站支付结果的 url 地址
        /// </summary>
        [DataMember]
        public string ReceiveUrl { get; set; }
        /// <summary>
        /// 版本号，固定填 v1.0
        /// </summary>
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public string QueryVersion { get; set; }
        /// <summary>
        /// 语言种类，默认填 1，固定选择值：1；1 代表简体中文、2 代表繁体中文、3 代表英文
        /// </summary>
        [DataMember]
        public string Language { get; set; }
        /// <summary>
        /// 签名类型，默认填 1，固定选择值：0、1；0 表示订单上送和交易结果通知都使用 MD5 进行签名1 表示商户用使用 MD5 算法验签上送订单，通联交易结果通知使用证书签名
        /// </summary>
        [DataMember]
        public string SignType { get; set; }
        /// <summary>
        /// 商户号，数字串，商户在通联申请开户的商户号
        /// </summary>
        [DataMember]
        public string MerchantId { get; set; }
        /// <summary>
        /// 订单号，字符串，只允许使用字母、数字、- 、_,并以字母或数字开头；每商户提交的订单号，必须在当天的该商户所有交易中唯一
        /// </summary>
        [DataMember]
        public string OrderNo { get; set; }
        /// <summary>
        /// 订单金额，整型数字，金额与币种有关，如果是人民币，则单位是分，即 10 元提交时金额应为 1000；如果是美元，单位是美分，即 10 美元提交时金额为 1000
        /// </summary>
        [DataMember]
        public string OrderAmount { get; set; }
        /// <summary>
        /// 币种类型，默认填 0，0 和 156 代表人民币、840 代表美元、344 代表港币，跨境支付商户不建议使用 0
        /// </summary>
        [DataMember]
        public string OrderCurrency { get; set; }
        /// <summary>
        /// 订单时间，日期格式：yyyyMMDDhhmmss，例如：20121116020101
        /// </summary>
        [DataMember]
        public string OrderDatetime { get; set; }
        /// <summary>
        /// 支付方式
        /// 固定选择值：0、1、4、11、23、28
        /// 接入互联网关时，默认为间连模式，填 0
        /// 若需接入外卡支付，只支持直连模式，即固定上送
        /// payType = 23，issuerId=visa 或 mastercard
        /// 0 代表未指定支付方式，即显示该商户开通的所有支付方式
        /// 1 个人储蓄卡网银支付
        /// 4 企业网银支付
        /// 11 个人信用卡网银支付
        /// 23 外卡支付
        /// 28 认证支付
        ///非直连模式，设置为 0；直连模式，值为非 0 的固定选择值
        /// </summary>
        [DataMember]
        public string PayType { get; set; }
        /// <summary>
        /// MD5 KEY
        /// </summary>
        [DataMember]
        public string MD5Key { get; set; }

        #region 代付时使用
        [DataMember]
        public string PayMerchantId { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string TrxCode { get; set; }
        [DataMember]
        public string BusinessCode { get; set; }
        [DataMember]
        public string Level { get; set; }
        [DataMember]
        public string SigneCert { get; set; }
        [DataMember]
        public string SigneCertPwd { get; set; }
        [DataMember]
        public string VerifyCert { get; set; }
        [DataMember]
        public string ProcessServlet { get; set; }

        #endregion
    }
}
