using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace KC.Enums.Pay
{
    public enum CautionMoneyPayStatus
    {
        [Description("待支付")]
        WaitPay = 0,

        [Description("已支付")]
        Paid = 1,

        [Description("已取消")]
        Cancel = 2
    }
}
