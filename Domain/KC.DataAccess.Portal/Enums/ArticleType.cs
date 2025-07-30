using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.Portal
{
    [DataContract]
    public enum ArticleType
    {
        /// <summary>
        /// 普通类型
        /// </summary>
        [Description("普通类型")]
        [EnumMember]
        Ordinary = 0,

        /// <summary>
        /// 网站信息
        /// </summary>
        [Description("网站信息")]
        [EnumMember]
        Info = 1,

        /// <summary>
        /// 帮助类型
        /// </summary>
        [Description("帮助类型")]
        [EnumMember]
        Help = 2,

        /// <summary>
        /// 单页
        /// </summary>
        [Description("单页")]
        [EnumMember]
        SinglePage = 3,

        /// <summary>
        /// 活动
        /// </summary>
        [Description("活动")]
        [EnumMember]
        Activity = 4
    }
}
