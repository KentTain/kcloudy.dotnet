using System.ComponentModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KC.Service.Enums
{
    /// <summary>
    /// 支付方式
    /// </summary>
    public enum PayType
    {
        /// <summary>
        /// 现金
        /// </summary>
        [Description("现金")]
        Cash = 0,

        /// <summary>
        /// 信用额度
        /// </summary>
        [Description("信用额度")]
        CreditLine = 1
    }
}