using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.App
{
    [DataContract]
    public enum BranchType
    {
        /// <summary>
        /// 主分支
        /// </summary>
        [Display(Name = "主分支")]
        [Description("主分支")]
        [EnumMember]
        Main = 0,

        /// <summary>
        /// 个人分支
        /// </summary>
        [Display(Name = "个人分支")]
        [Description("个人分支")]
        [EnumMember]
        Private = 1,
    }
}
