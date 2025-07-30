using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace KC.Enums.Contract
{
    [DataContract]
    public enum ClientType
    {
        //[EnumMember] [Display(Name = "无效客户")] [Description("无效客户")] Useless = -1,

        [EnumMember] [Display(Name = "新增客户")] [Description("新增客户")] Potential = 0,

        [EnumMember] [Display(Name = "潜在客户")] [Description("潜在客户")] Normal = 1,

        [EnumMember] [Display(Name = "重点客户")] [Description("重点客户")] Important = 2,

        [EnumMember] [Display(Name = "跟踪客户")] [Description("跟踪客户")] Tracking = 3,

        [EnumMember] [Display(Name = "即成客户")] [Description("即成客户")] Pick = 4
    }

    [DataContract]
    public enum CustomerType
    {
        [EnumMember] [Display(Name = "机构")] [Description("机构")] Organization = 0,

        [EnumMember] [Display(Name = "个人")] [Description("个人")] Personal = 1,

        [EnumMember] [Display(Name = "企业")] [Description("企业")] Company = 2,
    }
}
