using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace KC.Enums.Offering
{
    [DataContract]
    public enum ProductPropertyType
    {
        /// <summary>
        /// 产品规格
        /// </summary>
        [Display(Name = "产品规格")]
        [Description("产品规格")]
        [EnumMember]
        Specification = 1,

        /// <summary>
        /// 产品图片
        /// </summary>
        [Display(Name = "产品图片")]
        [Description("产品图片")]
        [EnumMember]
        Image = 2,

        /// <summary>
        /// 产品文件
        /// </summary>
        [Display(Name = "产品文件")]
        [Description("产品文件")]
        [EnumMember]
        File = 4,

        /// <summary>
        /// 产品视频
        /// </summary>
        [Display(Name = "产品视频")]
        [Description("产品视频")]
        [EnumMember]
        Video = 8,

        /// <summary>
        /// 产品音频
        /// </summary>
        [Display(Name = "产品音频")]
        [Description("产品音频")]
        [EnumMember]
        Audio = 16,

        /// <summary>
        /// 服务区域
        /// </summary>
        [Display(Name = "服务区域")]
        [Description("服务区域")]
        [EnumMember]
        Area = 32,

        /// <summary>
        /// 服务提供商
        /// </summary>
        [Display(Name = "服务提供商")]
        [Description("服务提供商")]
        [EnumMember]
        ServiceProvider = 64,

        /// <summary>
        /// 品牌
        /// </summary>
        [Display(Name = "品牌")]
        [Description("品牌")]
        [EnumMember]
        Brand = 128,
    }
}
