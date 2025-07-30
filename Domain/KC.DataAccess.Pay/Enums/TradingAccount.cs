using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace KC.Enums.Pay
{
    /// <summary>
    /// 交易账户
    /// </summary>
    public enum TradingAccount
    {
        /// <summary>
        /// 财富钱包
        /// </summary>
        [Description("财富钱包")]
        CFWallet = 0,

        /// <summary>
        /// CBS
        /// </summary>
        [Description("CBS")]
        CBS = 1
    }
}
