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
    public enum WebSiteItemType
    {
        /// <summary>
        /// 左右结构
        /// </summary>
        [Description("左右结构")]
        [EnumMember]
        LeftRight = 0,
        /// <summary>
        /// 左中右结构
        /// </summary>
        [Description("左中右结构")]
        [EnumMember]
        LeftCenterRight = 1,
        /// <summary>
        /// 上下结构
        /// </summary>
        [Description("上下结构")]
        [EnumMember]
        TopBottom = 2,

        /// <summary>
        /// 上中下结构
        /// </summary>
        [Description("上中下结构")]
        [EnumMember]
        TopCenterBottom = 3,

    }

}
