using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace KC.Enums.Pay
{
    public enum OfflinePaymentStatus
    {
        [Description("待确认")]
        WaitConfirm = 0,

        [Description("已支付")]
        Paid = 1,

        [Description("已取消")]
        Cancel = 2,

        [Description("被退回")]
        Reject = 3
    }
}
