using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KC.Enums.Pay
{
    public enum AuditStatus
    {
        /// <summary>
        /// 未认证
        /// </summary>
        [Display(Name = "未认证")]
        [Description("未认证")]
        Unrecognized = -1,
        /// <summary>
        /// 审核中
        /// </summary>
        [Display(Name = "审核中")]
        [Description("审核中")]
        Audit = 0,
        /// <summary>
        /// 认证失败
        /// </summary>
        [Display(Name = "认证失败")]
        [Description("认证失败")]
        Failed = 1,
        /// <summary>
        /// 认证通过
        /// </summary>
        [Display(Name = "认证通过")]
        [Description("认证通过")]
        Through = 2,
        /// <summary>
        /// 审查中
        /// </summary>
        [Display(Name = "审查中")]
        [Description("审查中")]
        Review = 3,
        /// <summary>
        /// 审查通过
        /// </summary>
        [Display(Name = "审查通过")]
        [Description("审查通过")]
        ReviewThrough = 4,
        /// <summary>
        /// 审查不通过
        /// </summary>
        [Display(Name = "审查不通过")]
        [Description("审查不通过")]
        ReviewPass = 5,
    }
}
