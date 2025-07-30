using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace KC.Enums.Pay
{
    /// <summary>
    /// 充值状态
    /// </summary>
    public enum RechargeStatus
    {
        /// <summary>
        /// 等待充值
        /// </summary>
        [Description("等待充值")]
        Waiting = 0,

        /// <summary>
        /// 充值成功
        /// </summary>
        [Description("充值成功")]
        Success = 1,

        /// <summary>
        /// 充值失败
        /// </summary>
        [Description("充值失败")]
        Faild = 2
    }

    public enum WithdrawalsStatus
    {
        /// <summary>
        /// 等待处理
        /// </summary>
        [Description("等待处理")]
        Waiting = 0,

        /// <summary>
        /// 已处理
        /// </summary>
        [Description("已处理")]
        Success = 1,
    }
}
