using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.Contract
{
    public enum ElectronicSignStatus
    {
        [Display(Name = "草稿")]
        [Description("草稿")]
        Draft = 0,

        /// <summary>
        /// 已提交
        /// </summary>
        [Display(Name = "待审核")]
        [Description("待审核")]
        Submited = 1,

        /// <summary>
        /// 已通过
        /// </summary>
        [Display(Name = "已通过")]
        [Description("已通过")]
        Agree = 2,
        /// <summary>
        /// 申请不通过
        /// </summary>
        [Display(Name = "申请不通过")]
        [Description("申请不通过")]
        Disagree = 3,

        /// <summary>
        /// 已注销
        /// </summary>
        [Display(Name = "已注销")]
        [Description("已注销")]
        Cancellation = 4
    }
    
}
