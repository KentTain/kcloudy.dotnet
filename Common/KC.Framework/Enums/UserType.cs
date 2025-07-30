using System.ComponentModel;
using System.Runtime.Serialization;

namespace KC.Framework.Base
{
    [DataContract]
    public enum UserType
    {
        [EnumMember]
        [Description("个人")]
        Person = 1,

        [EnumMember]
        [Description("组织")]
        Company = 2,

        [EnumMember]
        [Description("员工")]
        Staff = 4,
    }
}
