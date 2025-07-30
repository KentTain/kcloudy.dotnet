using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace KC.Enums.CRM
{
    [DataContract]
    public enum TracingType
    {
        [EnumMember] [Display(Name = "拜访")] [Description("拜访")] Manual = 0,
        [EnumMember] [Display(Name = "短信通知")] [Description("短信通知")] SmsNotify = 1,
        [EnumMember] [Display(Name = "邮件通知")] [Description("邮件通知")] EmailNotify = 2,
        [EnumMember] [Display(Name = "电话通知")] [Description("电话通知")] CallNotify = 3,
        [EnumMember] [Display(Name = "群发短信通知")] [Description("群发短信通知")] MassSmsNotify = 4,
        [EnumMember] [Display(Name = "群发邮件通知")] [Description("群发邮件通知")] MassEmailNotify = 5
    }
}
