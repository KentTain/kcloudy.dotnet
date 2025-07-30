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
    public enum MessageTemplateType
    {
        [EnumMember]
        [Description("邮件模板")]
        EmailMessage = 0,

        [EnumMember]
        [Description("站内信模板")]
        InsideMessage = 1,

        [EnumMember]
        [Description("短信模板")]
        SmsMessage = 2,
    }
}
