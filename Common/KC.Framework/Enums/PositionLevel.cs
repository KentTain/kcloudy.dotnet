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
    public enum PositionLevel
    {
        /// <summary>
        /// 普通员工
        /// </summary>
        [EnumMember]
        [Description("普通员工")]
        Staff = 0,

        /// <summary>
        /// 主管
        /// </summary>
        [EnumMember]
        [Description("主管")]
        Mananger = 1,

    }
}
