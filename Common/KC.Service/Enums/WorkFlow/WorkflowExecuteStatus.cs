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
    public enum WorkflowExecuteStatus
    {
        /// <summary>
        /// 未处理
        /// </summary>
        [Description("未处理")]
        [EnumMember]
        Init = 0,
        /// <summary>
        /// 同意：每个节点的人将流程提交至下一节点
        /// </summary>
        [Description("同意")]
        [EnumMember]
        Approve = 1,
        /// <summary>
        /// 退回：可退回到某个节点继续流转，退回到发起节点，或退回到前节点
        /// </summary>
        [Description("退回")]
        [EnumMember]
        Return = 2,
        /// <summary>
        /// 撤回：节点执行完后、下一节点执行前，可以收回进行修改然后再提交
        /// </summary>
        [Description("撤回")]
        [EnumMember]
        Revoke = 3,
        /// <summary>
        /// 取消：流程发起人执行取消流程
        /// </summary>
        [Description("取消")]
        [EnumMember]
        Cancel = 4,
        /// <summary>
        /// 中止：流程提前结束，当前节点之后的其它节点不再执行
        /// </summary>
        [Description("中止")]
        [EnumMember]
        Pause = 5,
    }
}
