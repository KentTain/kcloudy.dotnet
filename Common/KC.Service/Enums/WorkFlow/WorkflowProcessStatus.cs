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
    public enum WorkflowProcessStatus
    {
        /// <summary>
        /// 草稿
        /// </summary>
        [Description("草稿")]
        [EnumMember]
        Draft = 0,
        /// <summary>
        /// 审核中：每个节点的人将流程提交至下一节点
        /// </summary>
        [Description("审核中")]
        [EnumMember]
        Auditing = 1,
        /// <summary>
        /// 回退：打回表单，不再执行任务
        /// </summary>
        [Description("回退")]
        [EnumMember]
        Regress = 2,
        /// <summary>
        /// 取消：流程发起人执行取消流程
        /// </summary>
        [Description("取消")]
        [EnumMember]
        Cancel = 3,
        /// <summary>
        /// 中止：流程提前结束，当前节点之后的其它节点不再执行
        /// </summary>
        [Description("中止")]
        [EnumMember]
        Pause = 4,
        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        [EnumMember]
        Finished = 9,

    }
}
