using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Framework.Base
{
    /// <summary>
    /// 流程业务状态
    /// </summary>
    [Serializable]
    [DataContract]
    public enum WorkflowBusStatus
    {
        /// <summary>
        /// 草稿
        /// </summary>
        [Display(Name = "草稿")]
        [Description("草稿")]
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
