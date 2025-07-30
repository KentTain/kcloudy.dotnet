using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace KC.Enums.Offering
{
    [DataContract]
    public enum OfferingPropertyType
    {
        /// <summary>
        /// 商品详情
        /// </summary>
        [Display(Name = "商品详情")]
        [Description("商品详情")]
        [EnumMember]
        Detail = 1,

        /// <summary>
        /// 商品图片
        /// </summary>
        [Display(Name = "商品图片")]
        [Description("商品图片")]
        [EnumMember]
        Image = 2,

        /// <summary>
        /// 商品文件
        /// </summary>
        [Display(Name = "商品文件")]
        [Description("商品文件")]
        [EnumMember]
        File = 4,

        /// <summary>
        /// 商品视频
        /// </summary>
        [Display(Name = "商品视频")]
        [Description("商品视频")]
        [EnumMember]
        Video = 8,

        /// <summary>
        /// 商品音频
        /// </summary>
        [Display(Name = "商品音频")]
        [Description("商品音频")]
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

        /// <summary>
        /// 付款方式
        /// </summary>
        [Display(Name = "付款方式")]
        [Description("付款方式")]
        [EnumMember]
        PaymentInfo = 256,
    }
}
