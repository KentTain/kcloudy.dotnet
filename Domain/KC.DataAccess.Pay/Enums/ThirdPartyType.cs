using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.Pay
{
    public enum ThirdPartyType
    {
        [Description("通联支付)")]
        AllinpayConfigSign = 1,
        [Description("富友支付-B2C)")]
        FuiouConfigSign = 2,
        [Description("招行CBS支付)")]
        CMBCBSConfigSign = 3,
        [Description("银联支付)")]
        UnionpayConfigSign = 4,
        [Description("合利宝快捷支付)")]
        HeliPayConfigSign = 5,
        [Description("中金支付")]
        CPCNConfigSign = 6,
        [Description("渤海银行云账本")]
        BoHaiConfigSign = 7
    }
}
