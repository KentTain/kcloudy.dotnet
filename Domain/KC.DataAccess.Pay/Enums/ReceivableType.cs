using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.Pay
{
    /// <summary>
    /// 应收来源
    /// </summary>
    public enum ReceivableType
    {
        /// <summary>
        /// 应收账款
        /// </summary>
        [Description("应收账款")]
        AccountsReceivable=0,

        /// <summary>
        /// 应收票据
        /// </summary>
        [Description("应收票据")]
        BillReceivable=1
    }
}
