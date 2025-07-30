using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace KC.Enums.Offering
{
    [DataContract]
    public enum ServiceAttrDataType
    {
        /// <summary>
        /// 字符串
        /// </summary>
        [Display(Name = "字符串")]
        [Description("字符串")]
        [EnumMember]
        String = 0,

        /// <summary>
        /// 布尔型
        /// </summary>
        [Display(Name = "布尔型")]
        [Description("布尔型")]
        [EnumMember]
        Bool = 1,

        /// <summary>
        /// 整型
        /// </summary>
        [Display(Name = "整型")]
        [Description("整型")]
        [EnumMember]
        Int = 2,

        /// <summary>
        /// 数值型
        /// </summary>
        [Display(Name = "数值型")]
        [Description("数值型")]
        [EnumMember]
        Decimal = 3,

        /// <summary>
        /// 金额
        /// </summary>
        [Display(Name = "金额")]
        [Description("金额")]
        [EnumMember]
        Currancy = 4,

        /// <summary>
        /// 日期型
        /// </summary>
        [Display(Name = "日期型")]
        [Description("日期型")]
        [EnumMember]
        Datetime = 5,

        /// <summary>
        /// 文本型
        /// </summary>
        [Display(Name = "文本型")]
        [Description("文本型")]
        [EnumMember]
        Text = 6,

        /// <summary>
        /// 其他
        /// </summary>
        [Display(Name = "其他")]
        [Description("其他")]
        [EnumMember]
        Other = 99,
    }
}
