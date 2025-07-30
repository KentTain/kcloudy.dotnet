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
    public enum WebSitePageType
    {
        /// <summary>
        /// 系统自带
        /// </summary>
        [Description("系统自带")]
        [EnumMember]
        System = 0,
        /// <summary>
        /// 外部链接
        /// </summary>
        [Description("外部链接")]
        [EnumMember]
        Link = 1,

        /// <summary>
        /// 自定义
        /// </summary>
        [Description("自定义")]
        [EnumMember]
        Customize = 99,
    }


}
