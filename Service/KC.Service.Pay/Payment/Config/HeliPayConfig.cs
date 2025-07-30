using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Pay.Config
{
    public class HeliPayConfig
    {
        /// <summary>
        /// 提交地址
        /// </summary>
        public string PostUrl { get; set; }

        /// <summary>
        /// 交易类型,收银台支付:PCCashier,网银支付：OnlinePay，下单接口(扫码支付)：AppPay
        /// </summary>
        public string P1_bizType { get; set; }

        /// <summary>
        /// 合利宝分配的商户号
        /// </summary>
        public string P2_customerNumber { get; set; }

        /// <summary>
        /// 商户生成的用户唯一标识
        /// </summary>
        public string P3_userId { get; set; }

        /// <summary>
        /// 过了订单有效时间的订单会被设置为取消状态不能再重新进行支付。
        /// </summary>
        public string P9_period { get; set; }

        /// <summary>
        /// Day：天 Hour：时 Minute：分
        /// </summary>
        public string P10_periodUnit { get; set; }

        /// <summary>
        /// 支付结束后显示的合作商户系统页面地址
        /// </summary>
        public string P11_callbackUrl { get; set; }

        /// <summary>
        /// 合利宝平台在用户支付成功通知商户的地址
        /// </summary>
        public string P12_serverCallbackUrl { get; set; }

        /// <summary>
        /// ONLINE：网银 QUICK：快捷 ALIPAY：微信扫码 WXPAY：微信扫码
        /// </summary>
        public string P14_productCode { get; set; }


        /// <summary>
        /// 签名加密方式，使用MD5加密，接入商户需以MD5加密签名
        /// </summary>
        public string signType { get; set; }

        /// <summary>
        /// 商户密钥
        /// </summary>
        public string SignKey { get; set; }
    }
}
