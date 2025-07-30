using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace KC.Enums.Pay
{
    public enum PaymentType
    {
        /// <summary>
        /// 余额
        /// </summary>
        [Description("余额")]
        AccountBalance = 0,

        /// <summary>
        /// 线上票据
        /// </summary>
        [Description("线上票据")]
        Bill = 1,

        /// <summary>
        /// 线下票据
        /// </summary>
        [Description("线下票据")]
        OfflineBill = 2,

        /// <summary>
        /// CBS
        /// </summary>
        [Description("CBS")]
        CBS = 3,

        /// <summary>
        /// 信用凭证
        /// </summary>
        [Description("信用凭证")]
        Voucher = 4,

        /// <summary>
        /// 线下支付
        /// </summary>
        [Description("线下支付")]
        OfflinePayment = 5,

        /// <summary>
        /// 受托支付
        /// </summary>
        [Description("受托支付")]
        EntrustedPayment = 6,

        /// <summary>
        /// 积分抵扣
        /// </summary>
        [Description("积分抵扣")]
        IntegralDeduction = 7,


        /// <summary>
        /// 微信扫码支付
        /// </summary>
        [Description("微信扫码支付")]
        WXScanQRCode = 8,

        /// <summary>
        /// 支付宝扫码支付
        /// </summary>
        [Description("支付宝扫码支付")]
        AlipayScanQRCode = 9,
    }
}
