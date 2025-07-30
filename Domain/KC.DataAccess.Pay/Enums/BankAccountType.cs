using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.Pay
{
    public enum BankAccountType
    {
        
        /// <summary>
        /// 一般存款账户
        /// </summary>
        [Display(Name = "一般存款账户")]
        [Description("一般存款账户")]
        NormalAccount = 0,

        /// <summary>
        /// 基本存款账户
        /// </summary>
        [Display(Name = "基本存款账户")]
        [Description("基本存款账户")]
        BasicAccount = 1,

        /// <summary>
        /// 临时存款账户
        /// </summary>
        [Display(Name = "临时存款账户")]
        [Description("临时存款账户")]
        TempAccount = 2,

        /// <summary>
        /// 专用存款账户
        /// </summary>
        [Display(Name = "专用存款账户")]
        [Description("专用存款账户")]
        SpecialAccount = 3,

        /// <summary>
        /// 个人账户
        /// </summary>
        [Display(Name = "个人账户")]
        [Description("个人账户")]
        PersonalAccount = 4,

        /// <summary>
        /// 其他
        /// </summary>
        [Display(Name = "其他")]
        [Description("其他")]
        Other = 99,
    }
}
