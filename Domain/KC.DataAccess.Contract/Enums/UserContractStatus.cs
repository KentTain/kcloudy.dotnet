using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.Contract
{
    public enum UserContractStatus
    {
        /// <summary>
        /// 待上传方审核
        /// </summary>
        [Display(Name = "待上传方审核")]
        [Description("待上传方审核")]
        Comfirm = 0,

        /// <summary>
        /// 待审核
        /// </summary>
        [Display(Name = "待审核")]
        [Description("待审核")]
        Submited = 1,

        /// <summary>
        /// 已审核
        /// </summary>
        [Display(Name = "已审核")]
        [Description("已审核")]
        Agree = 2,
        /// <summary>
        /// 不同意
        /// </summary>
        [Display(Name = "已退回")]
        [Description("已退回")]
        Disagree = 3,

        /// <summary>
        /// 待签署
        /// </summary>
        [Display(Name = "待签署")]
        [Description("待签署")]
        WaitSign = 4,

        /// <summary>
        /// 已签署
        /// </summary>
        [Display(Name = "已签署")]
        [Description("已签署")]
        Sign = 5,

        /// <summary>
        /// 待确认作废
        /// </summary>
        [Display(Name = "待确认作废")]
        [Description("待确认作废")]
        WaitAbandoned = 6,

        /// <summary>
        /// 已作废
        /// </summary>
        [Display(Name = "已作废")]
        [Description("已作废")]
        Abandoned = 7

    } 
}
