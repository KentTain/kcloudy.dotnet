using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace KC.Enums.Pay
{
    /// <summary>
    /// 业务请求结果 
    /// </summary>
    public enum BankBillStatus
    {
        /// <summary>
        /// 待处理
        /// </summary>
        [Description("待处理")]
        Wait = -1,
        /// <summary>
        /// 出票已登记,010004
        /// </summary>
        [Description("出票已登记")]
        Registered = 0,

        /// <summary>
        /// 提示收票待签收,030001
        /// </summary>
        [Description("提示收票待签收")]
        WaitForSign = 1,

        /// <summary>
        /// 提示收票已签收,030006
        /// </summary>
        [Description("提示收票已签收")]
        Signed = 2,

        /// <summary>
        /// 提示承兑待签收,020001
        /// </summary>
        [Description("提示承兑待签收")]
        AcceptanceWaitForSign = 3,

        /// <summary>
        /// 提示承兑已签收,020006
        /// </summary>
        [Description("提示承兑已签收")]
        AcceptanceSigned = 4,

        /// <summary>
        /// 提示付款待签收,200001
        /// </summary>
        [Description("提示付款待签收")]
        PaymentWaitForSign = 5,

        /// <summary>
        /// 逾期提示付款待签收,210001
        /// </summary>
        [Description("逾期提示付款待签收")]
        BeOverdue = 6,

        /// <summary>
        /// 提示付款已拒付,200312
        /// </summary>
        [Description("提示付款已拒付")]
        PaymentHasBeenRejected = 7,

        /// <summary>
        /// 结束已结清,000000
        /// </summary>
        [Description("结束已结清")]
        Complete = 8,

        /// <summary>
        /// 结束已作废,000002
        /// </summary>
        [Description("结束已作废")]
        Abandoned = 9

    }
}
