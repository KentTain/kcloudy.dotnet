using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace KC.Enums.Offering
{
    [DataContract]
    public enum OfferingVersion
    {
        /// <summary>
        /// 免费版
        /// </summary>
        [Display(Name = "免费版")]
        [Description("免费版")]
        [EnumMember]
        Free = 1,

        /// <summary>
        /// 试用版
        /// </summary>
        [Display(Name = "试用版")]
        [Description("试用版")]
        [EnumMember]
        TryOut = 2,

        /// <summary>
        /// 会员版
        /// </summary>
        [Display(Name = "会员版")]
        [Description("会员版")]
        [EnumMember]
        Normal = 4,

        /// <summary>
        /// 企业版
        /// </summary>
        [Display(Name = "企业版")]
        [Description("企业版")]
        [EnumMember]
        Enterprise = 8,

        /// <summary>
        /// 集团版
        /// </summary>
        [Display(Name = "集团版")]
        [Description("集团版")]
        [EnumMember]
        Group = 16,
    }
}
