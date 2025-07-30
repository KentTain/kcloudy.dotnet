using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.Pay
{
    /// <summary>
    /// 应付类型
    /// </summary>
    public enum PayableType
    {
        /// <summary>
        /// 应付账款
        /// </summary>
        [Description("应付账款")]
        AccountsPayable=0,

        /// <summary>
        /// 应付票据
        /// </summary>
        [Description("应付票据")]
        BillPayable=1
    }
}
