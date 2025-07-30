using System.ComponentModel;
using System.Runtime.Serialization;

namespace KC.Enums.Portal
{
    [DataContract]
    public enum AddressType
    {
        [Description("公司地址")]
        [EnumMember]
        CompanyAddress = 0,
        [Description("收货地址")]
        [EnumMember]
        ShippingAddress = 1,
        [Description("发货地址")]
        [EnumMember]
        DeliveryAddress = 2,
    }
}
