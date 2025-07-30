using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.Pay
{
    /// <summary>
    /// 应付来源
    /// </summary>
    public enum PayableSource
    {
        /// <summary>
        /// 采购订单
        /// </summary>
        [Description("采购订单")]
        PO = 0,

        /// <summary>
        /// 赊购订单
        /// </summary>
        [Description("赊购订单")]
        BCO = 1,

        /// <summary>
        /// 融资订单
        /// </summary>
        [Description("融资订单")]
        FO = 2,

        /// <summary>
        /// 平台服务费
        /// </summary>
        [Description("平台服务费")]
        PlatformServiceFee = 3,

        /// <summary>
        /// 系统升级费
        /// </summary>
        [Description("系统升级费")]
        SystemFee = 4,

        /// <summary>
        /// 预付款
        /// </summary>
        [Description("预付款")]
        AdvanceCharge = 5,

        /// <summary>
        /// 保证金
        /// </summary>
        [Description("保证金")]
        CautionMoney = 6,

        /// <summary>
        /// 退还保证金
        /// </summary>
        [Description("退还保证金")]
        RefundCautionMoney = 7,

        /// <summary>
        /// 增值服务费
        /// </summary>
        [Description("增值服务费")]
        ValueAddedServiceFee = 8,

        /// <summary>
        /// 融资服务费
        /// </summary>
        [Description("融资服务费")]
        ApplyServiceFee = 9,

        /// <summary>
        /// 融资保证金
        /// </summary>
        [Description("融资保证金")]
        CashDeposit = 10,

        /// <summary>
        /// 融资退还保证金
        /// </summary>
        [Description("融资退还保证金")]
        RefundCashDeposit = 11,

        /// <summary>
        /// 对账单
        /// </summary>
        [Description("对账单")]
        AccountStatement =12

    }
}
