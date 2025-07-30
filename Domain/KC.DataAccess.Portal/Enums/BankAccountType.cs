using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.Portal
{
    /// <summary>
    /// 银行账户类型
    /// </summary>
    [DataContract]
    public enum BankAccountType
    {
        /// <summary>
        /// 一般存款账户
        /// </summary>
        [Display(Name = "一般存款账户")]
        [Description("一般存款账户")]
        [EnumMember]
        NormalAccount = 0,

        /// <summary>
        /// 基本存款账户
        /// </summary>
        [Display(Name = "基本存款账户")]
        [Description("基本存款账户")]
        [EnumMember]
        BasicAccount = 1,

        /// <summary>
        /// 临时存款账户
        /// </summary>
        [Display(Name = "临时存款账户")]
        [Description("临时存款账户")]
        [EnumMember]
        TempAccount = 2,

        /// <summary>
        /// 专用存款账户
        /// </summary>
        [Display(Name = "专用存款账户")]
        [Description("专用存款账户")]
        [EnumMember]
        SpecialAccount = 3,

        /// <summary>
        /// 个人账户
        /// </summary>
        [Display(Name = "个人账户")]
        [Description("个人账户")]
        [EnumMember]
        PersonalAccount = 4,

        /// <summary>
        /// 其他
        /// </summary>
        [Display(Name = "其他")]
        [Description("其他")]
        [EnumMember]
        Other = 99,
    }
}
