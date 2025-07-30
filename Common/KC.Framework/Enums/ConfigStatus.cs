using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace KC.Framework.Base
{
    [Serializable]
    [DataContract]
    public enum ConfigStatus : int
    {
        [EnumMember]
        [Description("草稿")]
        Draft = 0,
        [EnumMember]
        [Description("启用")]
        Enable = 1,
        [EnumMember]
        [Description("停用")]
        Disable = 2,

    }
}
