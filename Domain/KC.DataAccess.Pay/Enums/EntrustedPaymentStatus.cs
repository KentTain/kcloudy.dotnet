using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace KC.Enums.Pay
{
    /// <summary>
    /// 受托支付状态
    /// </summary>
    public enum EntrustedPaymentStatus
    {
        /// <summary>
        /// 已提交，等待放款
        /// </summary>
        [Description("已提交，等待放款")]
        Submit = 0,

        /// <summary>
        /// 已放款,未转账
        /// </summary>
        [Description("已放款,未转账")]
        TransferMoney = 1,

        /// <summary>
        /// 转账失败
        /// </summary>
        [Description("转账失败")]
        TransferMoneyFaild = 2,

        /// <summary>
        /// 取消或失败
        /// </summary>
        [Description("取消或失败")]
        Cancel = 99,

        /// <summary>
        /// 完成转账
        /// </summary>
        [Description("完成转账")]
        Completed = 100
    }
}
