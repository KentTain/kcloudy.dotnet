using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace KC.Enums.Pay
{
    /// <summary>
    /// 交易类型
    /// </summary>
    public enum CashType
    {
        /// <summary>
        /// 交易订单
        /// </summary>
        [Description("交易订单")]
        Transaction = 0,

        /// <summary>
        /// 充值
        /// </summary>
        [Description("充值")]
        Recharge = 1,

        /// <summary>
        /// 提现
        /// </summary>
        [Description("提现")]
        Withdrawals = 2,

        /// <summary>
        /// 平台服务费
        /// </summary>
        [Description("平台服务费")]
        TransactionServiceFee = 3,

        /// <summary>
        /// 银行卡认证支付
        /// </summary>
        [Description("银行卡认证支付")]
        AuthPay = 4,

        /// <summary>
        /// 提现手续费
        /// </summary>
        [Description("提现手续费")]
        WithdrawalsServiceCharge = 5,

        /// <summary>
        /// 保证金
        /// </summary>
        [Description("保证金")]
        CautionMoney = 6,


        /// <summary>
        /// 系统升级费
        /// </summary>
        [Description("系统升级费")]
        SystemFee = 7,

        /// <summary>
        /// 预付款
        /// </summary>
        [Description("预付款")]
        AdvanceCharge = 8,

        /// <summary>
        /// 赊销订单
        /// </summary>
        [Description("赊销订单")]
        Repayment = 9,
        /// <summary>
        /// 退还保证金
        /// </summary>
        [Description(" 退还保证金")]
        RefundCautionMoney = 10,

        /// <summary>
        /// 融资订单
        /// </summary>
        [Description(" 融资订单")]
        FinancingOrder = 11,

        /// <summary>
        /// 增值服务费
        /// </summary>
        [Description(" 增值服务费")]
        ValueAddedServiceFee = 12,

        /// <summary>
        /// 凭证兑付
        /// </summary>
        [Description(" 凭证兑付")]
        VoucherRepayment = 13,
    }
}
