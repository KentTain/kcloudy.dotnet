using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Enums.Message
{
    [DataContract]
    public enum MessageType
    {
        [EnumMember]
        [Description("用户")]
        User = 0,

        [EnumMember]
        [Description("客户")]
        Customer = 1,

        [EnumMember]
        [Description("产品")]
        Offering = 2,

        [EnumMember]
        [Description("采购")]
        Purchase = 3,

        [EnumMember]
        [Description("合同")]
        Contact = 5,

        [EnumMember]
        [Description("对账单")]
        AccountStatement = 6,

        [EnumMember]
        [Description("销售")]
        Sales = 7,

        [EnumMember]
        [Description("融资产品")]
        FinancingOffering = 10,

        [EnumMember]
        [Description("融资订单")]
        FinancingOrder = 11,

        [EnumMember]
        [Description("融资额度")]
        FinancingCredit = 12,

        [EnumMember]
        [Description("信用评级")]
        CreditLevel = 13,

        [EnumMember]
        [Description("担保订单")]
        GuarantOrder = 14,

        [EnumMember]
        [Description("担保额度")]
        GuarantCredit = 15,

       

        [EnumMember]
        [Description("无类型")]
        None = 99,
    }
}
