using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KC.Enums.Pay
{
    public enum TODOType
    {
        /// <summary>
        /// 采购订单
        /// </summary>
        [Display(Name = "采购订单")]
        [Description("采购订单")]
        PO = 0,

        /// <summary>
        /// 销售订单
        /// </summary>
        [Display(Name = "销售订单")]
        [Description("销售订单")]
        SO = 1,

        /// <summary>
        /// 采购应付账款
        /// </summary>
        [Display(Name = "应付账款")]
        [Description("应付账款")]
        POPayable = 2,

        /// <summary>
        /// 赊销和融资应付账款
        /// </summary>
        [Display(Name = "应付账款")]
        [Description("应付账款")]
        FinancingPayable = 3,

        /// <summary>
        /// 充值订单
        /// </summary>
        [Display(Name = "充值订单")]
        [Description("充值订单")]
        Charge = 4,

        /// <summary>
        /// 授信订单
        /// </summary>
        [Display(Name = "授信订单")]
        [Description("授信订单")]
        Provider = 5,

        /// <summary>
        /// 领用订单
        /// </summary>
        [Display(Name = "领用订单")]
        [Description("领用订单")]
        Usage = 6,

        /// <summary>
        /// 应付-所有
        /// </summary>
        [Display(Name = "应付-所有")]
        [Description("应付-所有")]
        AllPayable = 7
    }
}
