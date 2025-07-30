using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Framework.Base
{
    [Serializable]
    [DataContract]
    public enum RuleType
    {
        /// <summary>
        /// 无
        /// </summary>
        [Description("无")]
        [EnumMember]
        None = 0,
        /// <summary>
        /// 并且
        /// </summary>
        [Description("并且")]
        [EnumMember]
        And = 1,
        /// <summary>
        /// 或者
        /// </summary>
        [Description("或者")]
        [EnumMember]
        Or = 2,
        
    }
}
