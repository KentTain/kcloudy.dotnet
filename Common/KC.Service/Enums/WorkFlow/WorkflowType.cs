using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.Enums.Workflow
{
    /// <summary>
    /// 流程审批类型
    /// </summary>
    [Serializable]
    [DataContract]
    public enum WorkflowType
    {
        /// <summary>
        /// 单人审批
        /// </summary>
        [Display(Name = "单人审批")]
        [Description("单人审批")]
        [EnumMember]
        SingleLine = 0,

        /// <summary>
        /// 多人审批
        /// </summary>
        [Display(Name = "多人审批")]
        [Description("多人审批")]
        [EnumMember]
        MultiLine = 1,

        /// <summary>
        /// 权重审批
        /// </summary>
        [Display(Name = "权重审批")]
        [Description("权重审批")]
        [EnumMember]
        WeightLine = 2,
    }
}
