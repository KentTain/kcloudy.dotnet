using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace KC.Enums.Portal
{
    [DataContract]
    public enum RequirementType
    {
        /// <summary>
        /// 询价
        /// </summary>
        [Display(Name = "询价")]
        [Description("询价")]
        [EnumMember]
        Inquiry = 0,

        /// <summary>
        /// 招标
        /// </summary>
        [Display(Name = "招标")]
        [Description("招标")]
        [EnumMember]
        Tenders = 1,

        /// <summary>
        /// 竞价
        /// </summary>
        [Display(Name = "竞价")]
        [Description("竞价")]
        [EnumMember]
        Bidding = 2,

    }
}
