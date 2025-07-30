using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace KC.Enums.CRM
{
    [DataContract]
    public enum CustomerSource
    {
        [EnumMember] [Display(Name = "手工录入")] [Description("手工录入")] Manual = 0,

        [EnumMember] [Display(Name = "数据导入")] [Description("数据导入")] Import = 1,

        [EnumMember] [Display(Name = "推送")] [Description("推送")] ThirdParty = 2,

        [EnumMember] [Display(Name = "电话营销")] [Description("电话营销")] TelephoneMarket = 3,

        [EnumMember] [Display(Name = "网络营销")] [Description("网络营销")] NetworksMarket = 4,

        [EnumMember] [Display(Name = "上门推销")] [Description("上门推销")] DoorSelling = 5,

        [EnumMember] [Display(Name = "媒体宣传")] [Description("媒体宣传")] Media = 6
    }
}
