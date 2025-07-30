using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Framework.Base
{
    [Serializable]
    [DataContract]
    public enum ConfigType : int
    {
        [EnumMember]
        [Description("未知")]
        UNKNOWN = 0,
        [EnumMember]
        [Description("电子邮件")]
        EmailConfig = 1,
        [EnumMember]
        [Description("短信平台")]
        SmsConfig = 2,
        [EnumMember]
        [Description("支付方式")]
        PaymentMethod = 3,
        [EnumMember]
        [Description("ID5")]
        ID5 = 4,
        [EnumMember]
        [Description("呼叫平台")]
        CallConfig = 5,
        [EnumMember]
        [Description("物流平台")]
        LogisticsPlatform = 6,
        [EnumMember]
        [Description("微信公众号")]
        WeixinConfig = 7,
        [EnumMember]
        [Description("电子签章")]
        ContractConfig = 8,
        [EnumMember]
        [Description("独立域名")]
        OwnDomain = 9,
    }
}
