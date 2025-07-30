using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KC.Enums.Pay
{
    public enum WithdrawType
    {
        /// <summary>
        /// 按费率收费
        /// </summary>
        [Display(Name = "按费率收费")]
        [Description("按费率收费")]
        Disposable = 0,
        /// <summary>
        /// 按单笔收费
        /// </summary>
        [Display(Name = "按单笔收费")]
        [Description("按单笔收费")]
        Discount = 1,
        /// <summary>
        /// 按阶梯收费
        /// </summary>
        [Display(Name = "阶梯式收费")]
        [Description("阶梯式收费")]
        Ladder = 2,
    }
}
