using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace KC.Enums.Offering
{
    [DataContract]
    public enum OfferingStatus
    {
        /// <summary>
        /// 草稿
        /// </summary>
        [Display(Name = "草稿")]
        [Description("草稿")]
        [EnumMember]
        Draft = 0,

        /// <summary>
        /// 提交审核
        /// </summary>
        [Display(Name = "提交审核")]
        [Description("提交审核")]
        [EnumMember]
        AuditPending = 1,

        /// <summary>
        /// 审核上架
        /// </summary>
        [Display(Name = "审核上架")]
        [Description("审核上架")]
        [EnumMember]
        Approved = 2,

        /// <summary>
        /// 审核退回
        /// </summary>
        [Display(Name = "审核退回")]
        [Description("审核退回")]
        [EnumMember]
        Disagree = 3,

        /// <summary>
        /// 已下架
        /// </summary>
        [Display(Name = "已下架")]
        [Description("已下架")]
        [EnumMember]
        Trash = 4,
    }
}
