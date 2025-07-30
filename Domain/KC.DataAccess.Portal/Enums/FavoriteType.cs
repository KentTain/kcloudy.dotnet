using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.Portal
{
    /// <summary>
    /// 关注类型
    /// </summary>
    [DataContract]
    public enum FavoriteType
    {
        [Description("产品")]
        [EnumMember]
        Offering = 0,

        [Description("企业")]
        [EnumMember]
        Company = 1,

        [Description("采购需求")]
        [EnumMember]
        Requirement = 2,
    }

}
