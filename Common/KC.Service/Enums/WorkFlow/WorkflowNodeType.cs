using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Enums.Workflow
{
    [Serializable]
    [DataContract]
    public enum WorkflowNodeType
    {
        /// <summary>
        /// 开始
        /// </summary>
        [Description("开始")]
        [EnumMember]
        Start = 0,
        /// <summary>
        /// 任务
        /// </summary>
        [Description("任务")]
        [EnumMember]
        Task = 1,
        /// <summary>
        /// 条件
        /// </summary>
        [Description("条件")]
        [EnumMember]
        Condition = 2,
        /// <summary>
        /// 子流程
        /// </summary>
        [Description("子流程")]
        [EnumMember]
        SubFlow = 3,
        /// <summary>
        /// 结束
        /// </summary>
        [Description("结束")]
        [EnumMember]
        End = 4,
    }
}
