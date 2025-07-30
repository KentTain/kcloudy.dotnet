using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.Message
{
    [DataContract]
    public enum NewsBulletinType
    {
        /// <summary>
        /// 内部通知
        /// </summary>
        [Description("内部通知")]
        [EnumMember]
        Internal = 0,

        /// <summary>
        /// 外部公告
        /// </summary>
        [Description("外部公告")]
        [EnumMember]
        External = 1,

        /// <summary>
        /// 帮助类型
        /// </summary>
        [Description("帮助类型")]
        [EnumMember]
        Help = 2,

        /// <summary>
        /// 新闻
        /// </summary>
        [Description("新闻")]
        [EnumMember]
        News = 3,

        /// <summary>
        /// 活动
        /// </summary>
        [Description("活动")]
        [EnumMember]
        Activity = 4,
    }
}
