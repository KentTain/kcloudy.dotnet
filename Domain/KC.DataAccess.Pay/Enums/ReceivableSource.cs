using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.Pay
{
    /// <summary>
    /// 应收来源
    /// </summary>
    public enum ReceivableSource
    {
        /// <summary>
        /// 销售订单
        /// </summary>
        [Description("销售订单")]
        SO = 0,

        /// <summary>
        /// 赊销订单
        /// </summary>
        [Description("赊销订单")]
        SCO = 1,

        /// <summary>
        /// 预付款
        /// </summary>
        [Description("预付款")]
        AdvanceCharge = 2,

        /// <summary>
        /// 保证金
        /// </summary>
        [Description("保证金")]
        CautionMoney = 3,

        /// <summary>
        /// 融资订单
        /// </summary>
        [Description("融资订单")]
        FinancingReceivables = 4,

        /// <summary>
        /// 退还保证金
        /// </summary>
        [Description(" 退还保证金")]
        RefundCautionMoney = 5,

        /// <summary>
        /// 增值服务费
        /// </summary>
        [Description(" 增值服务费")]
        ValueAddedServiceFee = 6,

        /// <summary>
        /// 融资保证金
        /// </summary>
        [Description("融资保证金")]
        CashDeposit = 7,

        /// <summary>
        /// 融资退还保证金
        /// </summary>
        [Description("融资退还保证金")]
        RefundCashDeposit = 8,


        /// <summary>
        /// 对账单
        /// </summary>
        [Description("对账单")]
        AccountStatement = 9
    }
}
