using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Pay.SDK
{
    /// <summary>
    /// 富友支付
    /// </summary>
    public class FuioupayMethod
    {
        /// <summary>
        /// 支付方法
        /// </summary>
        public const string PayMethod = "smpGate.do";

        /// <summary>
        /// 支付结果查询，在客户完成支付后，富友系统根据页面跳转URL,后台通知URL，向该地址发送http请求。
        /// </summary>
        public const string QueryMethod = "smpQueryGate.do";

        /// <summary>
        /// 支付结果查询(直接返回) 
        /// </summary>
        public const string QueryResultMethod = "smpAQueryGate.do";

        /// <summary>
        /// 订单退款
        /// </summary>
        public const string RefundMethod = "newSmpRefundGate.do";
    }

}
