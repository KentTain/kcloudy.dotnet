using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Enums.Workflow
{
    /// <summary>
    /// 流程任务状态
    /// </summary>
    [Serializable]
    [DataContract]
    public enum WorkflowTaskStatus
    {
        /// <summary>
        /// 未处理
        /// </summary>
        [Description("未处理")]
        [EnumMember]
        UnProcess = 0,
        /// <summary>
        /// 处理中
        /// </summary>
        [Description("处理中")]
        [EnumMember]
        Process = 1,
        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        [EnumMember]
        Finished = 2,
        /// <summary>
        /// 退回
        /// </summary>
        [Description("退回")]
        [EnumMember]
        Back = 3,
    }
}
