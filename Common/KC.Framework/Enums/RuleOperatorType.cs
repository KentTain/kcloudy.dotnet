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
    public enum RuleOperatorType
    {
        /// <summary>
        /// 等于
        /// </summary>
        [Description("等于")]
        [EnumMember]
        Equal = 0,
        /// <summary>
        /// 不等于
        /// </summary>
        [Description("不等于")]
        [EnumMember]
        NotEqual = 1,
        /// <summary>
        /// 包含
        /// </summary>
        [Description("包含")]
        [EnumMember]
        Contains = 2,

        /// <summary>
        /// 大于
        /// </summary>
        [Description("大于")]
        [EnumMember]
        Greater = 10,
        /// <summary>
        /// 小于
        /// </summary>
        [Description("小于")]
        [EnumMember]
        Less = 11,
        /// <summary>
        /// 大于等于
        /// </summary>
        [Description("大于等于")]
        [EnumMember]
        GreaterThanAndEqual = 12,
        /// <summary>
        /// 小于等于
        /// </summary>
        [Description("小于等于")]
        [EnumMember]
        LessThanAndEqual = 13,
        
    }
}
