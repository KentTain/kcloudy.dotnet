using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace KC.Enums.Pay
{
    /// <summary>
    /// 凭证提交状态
    /// </summary>
    public enum VoucherSubmitStatus
    {
        /// <summary>
        /// 待处理
        /// </summary>
        [Description("待处理")]
        Wait = 0,

        /// <summary>
        /// 已处理
        /// </summary>
        [Description("已处理")]
        Through = 1,

        /// <summary>
        /// 已取消
        /// </summary>
        [Description("已取消")]
        Cancel = 2,

        /// <summary>
        /// 已退回
        /// </summary>
        [Description("已退回")]
        Return = 3,

        /// <summary>
        /// 已生效，在shop表示订单已经支付
        /// </summary>
        [Description("已生效")]
        Effective = 4
    }
}
