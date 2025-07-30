using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Component.Base
{
    [Serializable]
    [DataContract]
    public enum QueueActionType
    {
        /// <summary>
        /// 执行成功后删除队列
        /// </summary>
        [Description("执行成功后删除队列")]
        [EnumMember]
        DeleteAfterExecuteAction = 0,

        /// <summary>
        /// 执行成功后保留队列
        /// </summary>
        [Description("执行成功后保留队列")]
        [EnumMember]
        KeepQueueAction = 1,

        /// <summary>
        /// 失败后重新执行
        /// </summary>
        [Description("执行失败后重新执行")]
        [EnumMember]
        FailedRepeatActon = 2
    }
}
