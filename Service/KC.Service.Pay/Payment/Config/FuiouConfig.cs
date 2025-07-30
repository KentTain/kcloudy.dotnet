using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Pay.Config
{
    public class FuioupayConfig
    {
        /// <summary>
        /// 提交地址
        /// </summary>
        public string PostUrl { get; set; }

        /// <summary>
        /// 商户代码
        /// </summary>
        public string mchnt_cd { get; set; }

        /// <summary>
        /// 32位商户密钥
        /// </summary>
        public string mchnt_key { get; set; }

        /// <summary>
        /// 页面跳转URL
        /// </summary>
        public string page_notify_url { get; set; }

        /// <summary>
        /// 后台通知URL
        /// </summary>
        public string back_notify_url { get; set; }

        /// <summary>
        /// 超时时间
        /// </summary>
        public string order_valid_time { get; set; }

        /// <summary>
        /// 银行代码
        /// </summary>
        public string iss_ins_cd { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public string ver { get; set; }

        /// <summary>
        /// ‘B2C’ – B2C支付 ‘B2B’ – B2B支付  ‘FYCD’ – 预付卡 ‘SXF’ – 随心富
        /// </summary>
        public string order_pay_type { get; set; }
    }
    /// <summary>
    /// 富友支付返回的订单状态
    /// </summary>
    public struct FuiouOrderState
    {
        /// <summary>
        /// 订单已生成(初始状态)
        /// </summary>
        public static readonly string Init = "00";

        /// <summary>
        /// 订单已撤消
        /// </summary>
        public static readonly string Rescinded = "01";

        /// <summary>
        /// 订单已合并
        /// </summary>
        public static readonly string Merged = "02";

        /// <summary>
        /// 订单已过期
        /// </summary>
        public static readonly string Expired = "03";

        /// <summary>
        /// 订单已确认(等待支付)
        /// </summary>
        public static readonly string WaitForPayment = "04";

        /// <summary>
        /// 订单支付失败
        /// </summary>
        public static readonly string PaymentFailure = "05";

        /// <summary>
        /// 订单已支付
        /// </summary>
        public static readonly string AlreadyPaid = "11";

        /// <summary>
        /// 已发货
        /// </summary>
        public static readonly string AlreadyShipped = "18";

        /// <summary>
        /// 已确认收货
        /// </summary>
        public static readonly string ConfirmedReceipt = "19";

    }
}
