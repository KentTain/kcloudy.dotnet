using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace KC.Enums.Portal
{

    [DataContract]
    public enum CompanyType
    {
        [EnumMember]
        [Description("企业客户")]
        Customer = 1,

        [EnumMember]
        [Description("供应商")]
        Supplier = 2,

        [EnumMember]
        [Description("分销商/经销商")]
        Retailer = 4,

        [EnumMember]
        [Description("机构")]
        Institute = 8,
    }
}
