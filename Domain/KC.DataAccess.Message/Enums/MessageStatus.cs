using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.Message
{
    [DataContract]
    public enum MessageStatus
    {
        /// <summary>
        /// 未读
        /// </summary>
        [EnumMember]
        [Description("未读")]
        Unread = 0,
        /// <summary>
        /// 已读
        /// </summary>
        [EnumMember]
        [Description("已读")]
        Read = 1,
        /// <summary>
        /// 已删除
        /// </summary>
        [EnumMember]
        [Description("已删除")]
        Deleted = 2
    }
}
