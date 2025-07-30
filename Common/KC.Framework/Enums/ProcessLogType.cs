using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Framework.Base
{
    [DataContract]
    public enum ProcessLogType
    {
        [EnumMember]
        [Description("成功")]
        Success = 0,
        [EnumMember]
        [Description("失败")]
        Failure = 1,
        [EnumMember]
        [Description("日常")]
        Ordinary = 2,
    }
}
