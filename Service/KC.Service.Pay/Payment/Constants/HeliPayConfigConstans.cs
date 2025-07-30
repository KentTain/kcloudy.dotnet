using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Pay.Constants
{
    public class HeliPayConfigConstans
    {
        public const string PaymentMethod = "PaymentMethod";

        public const string PostUrl = "PostUrl";

        /// <summary>
        /// 交易类型,收银台支付:PCCashier,网银支付：OnlinePay，下单接口(扫码支付)：AppPay
        /// </summary>
        public const string P1_bizType = "P1_bizType";

        /// <summary>
        /// 合利宝分配的商户号
        /// </summary>
        public const string P2_customerNumber = "P2_customerNumber";

        /// <summary>
        /// 商户生成的用户唯一标识
        /// </summary>
        public const string P3_userId = "P3_userId";


        /// <summary>
        /// 商户系统内部订单号，要求50字符以内，同一商户号下订单号唯一
        /// </summary>
        public const string P4_orderId = "P4_orderId";


        /// <summary>
        /// 格式：yyyyMMddHHmmss14 位数字，精确到秒
        /// </summary>
        public const string P5_timestamp = "P5_timestamp";


        /// <summary>
        /// URLEncoder.encode(“商品名称”, “utf-8”)
        /// </summary>
        public const string P6_goodsName = "P6_goodsName";


        /// <summary>
        /// URLEncoder.encode(“商品描述”, “utf-8”)
        /// </summary>
        public const string P7_goodsDesc = "P7_goodsDesc";


        /// <summary>
        /// 订单金额，以元为单位，最小金额为0.01
        /// </summary>
        public const string P8_orderAmount = "P8_orderAmount";

        /// <summary>
        /// 过了订单有效时间的订单会被设置为取消状态不能再重新进行支付。
        /// </summary>
        public const string P9_period = "P9_period";

        /// <summary>
        /// Day：天 Hour：时 Minute：分
        /// </summary>
        public const string P10_periodUnit = "P10_periodUnit";

        /// <summary>
        /// 支付结束后显示的合作商户系统页面地址
        /// </summary>
        public const string P11_callbackUrl = "P11_callbackUrl";

        /// <summary>
        /// 合利宝平台在用户支付成功通知商户的地址
        /// </summary>
        public const string P12_serverCallbackUrl = "P12_serverCallbackUrl";

        /// <summary>
        /// 用户下单请求商户，商户响应的 IP
        /// </summary>
        public const string P13_orderIp = "P13_orderIp";

        /// <summary>
        /// ONLINE：网银 QUICK：快捷 ALIPAY：微信扫码 WXPAY：微信扫码
        /// </summary>
        public const string P14_productCode = "P14_productCode";

        /// <summary>
        /// URLEncoder.encode(“备注”, “utf-8”)
        /// </summary>
        public const string P15_desc = "P15_desc";

        /// <summary>
        /// 签名加密方式，使用MD5加密，接入商户需以MD5加密签名
        /// </summary>
        public const string signType = "signType";

        /// <summary>
        /// 签名
        /// </summary>
        public const string sign = "sign";

        /// <summary>
        /// 商户密钥
        /// </summary>
        public const string SignKey = "SignKey";
    }
}
