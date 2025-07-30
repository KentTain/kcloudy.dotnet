using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.Pay
{
    public enum PaymentOperationType
    {
        /// <summary>
        /// 开户
        /// </summary>
        [Description("开户")]
        OpenAccount = 1,

        /// <summary>
        /// 结算账户绑定
        /// </summary>
        [Description("结算账户绑定")]
        SettlementBinding = 2,

        /// <summary>
        /// 结算账户解绑
        /// </summary>
        [Description("结算账户解绑")]
        SettlementUnbinding = 3,

        /// <summary>
        /// 查询可 T0/T1 出金额度
        /// </summary>
        [Description("查询可 T0/T1 出金额度")]
        WithdrawAmt = 4,

        /// <summary>
        /// 资金账户余额查询
        /// </summary>
        [Description("资金账户余额查询")]
        FundAccountBalance = 5,
        /// <summary>
        /// 入金
        /// </summary>
        [Description("入金")]
        BankIn = 6,

        /// <summary>
        /// 出金
        /// </summary>
        [Description("出金")]
        BankOut = 7,

        /// <summary>
        /// 订单支付
        /// </summary>
        [Description("订单支付")]
        OrderPay = 8,

        /// <summary>
        /// 冻结资金
        /// </summary>
        [Description("冻结资金")]
        FreezeAmt = 9,

        /// <summary>
        /// 解冻资金
        /// </summary>
        [Description("解冻资金")]
        UnFreezeAmt = 10,

        /// <summary>
        /// 认证银行账户
        /// </summary>
        [Description("认证")]
        AuthenticationBankAccount = 11,

        /// <summary>
        /// 认证申请
        /// </summary>
        [Description("认证申请")]
        AuthenticationBankApplication = 12,

        /// <summary>
        /// 入金查询
        /// </summary>
        [Description("入金查询")]
        BankInSearch = 13,

        /// <summary>
        /// 更新密码
        /// </summary>
        [Description("更新密码")]
        UpdatePassword = 14,

        /// <summary>
        /// 重置密码
        /// </summary>
        [Description("重置密码")]
        ResetPassword = 15
    }
}
