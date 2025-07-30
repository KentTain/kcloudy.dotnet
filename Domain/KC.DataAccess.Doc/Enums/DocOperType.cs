using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Enums.Doc
{
    [DataContract]
    public enum DocOperType
    {
        [EnumMember]
        [Display(Name = "创建")]
        [Description("创建")]
        Create = 0,

        [EnumMember]
        [Display(Name = "更新")]
        [Description("更新")]
        Update = 1,

        [EnumMember]
        [Display(Name = "下载")]
        [Description("下载")]
        Download = 2,

        [EnumMember]
        [Display(Name = "删除")]
        [Description("删除")]
        Delete = 3,

        [EnumMember]
        [Display(Name = "发送")]
        [Description("发送")]
        Send = 4,

        [EnumMember]
        [Display(Name = "归档")]
        [Description("归档")]
        Archive = 5,
    }
}
