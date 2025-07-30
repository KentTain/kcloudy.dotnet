using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Enums.Doc
{
    [DataContract]
    public enum DocLevel
    {
        [EnumMember]
        [Description("普通")]
        LevelOne = 0,

        [EnumMember]
        [Description("重要")]
        LevelTwo = 1,

        [EnumMember]
        [Description("机密")]
        LevelThree = 2,

        [EnumMember]
        [Description("绝密")]
        LevelFour = 3,
    }
}
