using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace KC.Enums.Offering
{
    [DataContract]
    public enum OfferingPriceType
    {
        /// <summary>
        /// 产品面议
        /// </summary>
        [Display(Name = "产品面议")]
        [Description("产品面议")]
        [EnumMember]
        NegotiablePrice = 0,

        /// <summary>
        /// 产品单价
        /// </summary>
        [Display(Name = "产品单价")]
        [Description("产品单价")]
        [EnumMember]
        SimplePrice = 1,

        /// <summary>
        /// 产品区间价
        /// </summary>
        [Display(Name = "产品区间价")]
        [Description("产品区间价")]
        [EnumMember]
        RangePrice = 2,

        /// <summary>
        /// 产品阶梯价
        /// </summary>
        [Display(Name = "产品阶梯价")]
        [Description("产品阶梯价")]
        [EnumMember]
        LadderPrice = 3,
    }
}
