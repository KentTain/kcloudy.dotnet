using System.ComponentModel;
using System.Runtime.Serialization;

namespace KC.Framework.Base
{
    [DataContract]
    public enum OrganizationType
    {
        /// <summary>
        /// 内部组织
        /// </summary>
        [EnumMember]
        [Description("内部组织")]
        Internal = 1,
        /// <summary>
        /// 注册组织
        /// </summary>
        [EnumMember]
        [Description("注册组织")]
        Outside = 2,
        /// <summary>
        /// 虚拟组织
        /// </summary>
        [EnumMember]
        [Description("虚拟组织")]
        Virtual = 3,
    }
}
