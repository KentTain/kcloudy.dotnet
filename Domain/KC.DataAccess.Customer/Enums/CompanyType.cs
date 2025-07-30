using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace KC.Enums.CRM
{

    [DataContract]
    public enum CompanyType
    {
        [EnumMember]
        [Description("企业客户")]
        Customer = 0,

        [EnumMember]
        [Description("供应商")]
        Supplier = 1,

        [EnumMember]
        [Description("分销商/经销商")]
        Retailer = 2,

        [EnumMember]
        [Description("机构")]
        Institute = 3,
    }
}
