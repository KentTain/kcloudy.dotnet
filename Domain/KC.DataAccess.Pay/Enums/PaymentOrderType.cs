using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.Pay
{
    /// <summary>
    /// 支付订单类型
    /// </summary>
    public enum PaymentOrderType
    {
        /// <summary>
        /// 入金
        /// </summary>
        [Description("入金")]
        BankIn = 1,

        /// <summary>
        /// 出金
        /// </summary>
        [Description("出金")]
        BankOut = 2,

        /// <summary>
        /// 订单支付
        /// </summary>
        [Description("订单支付")]
        OrderPay = 3,

        [Description("代付入金")]
        AgencyIn = 4,

        [Description("手续费")]
        Fee = 5
    }
}
