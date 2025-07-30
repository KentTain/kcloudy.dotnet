using System.ComponentModel;

namespace KC.Enums.Customer
{
    public enum AddressType
    {
        [Description("公司地址")]
        CompanyAddress = 0,
        [Description("收货地址")]
        ShippingAddress = 1,
        [Description("发货地址")]
        DeliveryAddress =2,
    }
}
