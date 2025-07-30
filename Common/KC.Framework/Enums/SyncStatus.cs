using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Framework.Base
{
    [DataContract]
    public enum SyncStatus
    {
        /// <summary>
        /// 未推送
        /// </summary>
        [Description("未推送")]
        [EnumMember]
        SyncPending = 0,
        /// <summary>
        /// 已推送
        /// </summary>
        [Description("已推送")]
        [EnumMember]
        SyncSuccess = 1,

        /// <summary>
        /// 推送失败
        /// </summary>
        [Description("推送失败")]
        [EnumMember]
        SyncFailed = 2,
    }
}
