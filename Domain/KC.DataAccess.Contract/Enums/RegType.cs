using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.Contract
{
    public enum RegType
    {
        /// <summary>
        /// 组织机构代码证
        /// </summary>
        [Description("组织机构代码证")]
        [Display(Name = "组织机构代码证")]
        OrgCode = 0,
        /// <summary>
        /// 统一社会信用代码
        /// </summary>
        [Display(Name = "统一社会信用代码")]
        [Description("统一社会信用代码")]
        OrgUSCC = 1,
        /// <summary>
        /// 未知
        /// </summary>
        [Display(Name = "签署合同")]
        [Description("待签署")]
        UnKnown = 99
    }
    
}
