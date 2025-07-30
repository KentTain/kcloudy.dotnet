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
    public enum ServiceAppStatus
    {
        /// <summary>
        /// 未提交审批
        /// </summary>
        [Display(Name = "未提交")]
        [Description("未提交")]
        [EnumMember]
        Draft = 0,

        /// <summary>
        /// 审批中
        /// </summary>
        [Display(Name = "审批中")]
        [Description("审批中")]
        [EnumMember]
        AuditPending = 1,

        /// <summary>
        /// 审批通过
        /// </summary>
        [Display(Name = "审批通过")]
        [Description("审批通过")]
        [EnumMember]
        Approved = 2,

        /// <summary>
        /// 审批不通过
        /// </summary>
        [Display(Name = "审批不通过")]
        [Description("审批不通过")]
        [EnumMember]
        Disagree = 3,
    }
}
