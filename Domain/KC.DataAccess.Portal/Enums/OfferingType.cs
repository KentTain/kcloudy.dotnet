using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace KC.Enums.Portal
{
    [DataContract]
    public enum OfferingType
    {
        /// <summary>
        /// 软件
        /// </summary>
        [Display(Name = "软件")]
        [Description("软件")]
        [EnumMember]
        Sofeware = 0,

        /// <summary>
        /// 服务
        /// </summary>
        [Display(Name = "服务")]
        [Description("服务")]
        [EnumMember]
        Service = 1,

        /// <summary>
        /// 设备
        /// </summary>
        [Display(Name = "设备")]
        [Description("设备")]
        [EnumMember]
        Equipment = 2,

        /// <summary>
        /// 赠品
        /// </summary>
        [Display(Name = "赠品")]
        [Description("赠品")]
        [EnumMember]
        Gift = 3,
    }
}
