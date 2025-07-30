using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.Pay
{
    /// <summary>
    /// 支付状态  1 提交申请 2 审核中 3 审核通过 4 审核不通过 5 支付失败 6 支付成功
    /// </summary>
    public enum PaymentState
    {
        [Description("提交申请")]
        Apply = 1,

        [Description("审核中")]
        Auditing = 2,

        [Description("审核通过")]
        AuditSuccess = 3,

        [Description("审核不通过")]
        AuditFailed = 4,

        [Description("支付失败")]
        PaymentFailed = 5,

        [Description("支付成功")]
        PaymentSuccess = 6
    }
}
