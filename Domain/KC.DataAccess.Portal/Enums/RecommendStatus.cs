using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace KC.Enums.Portal
{
    [DataContract]
    public enum RecommendStatus
    {
        /// <summary>
        /// 草稿
        /// </summary>
        [Display(Name = "草稿")]
        [Description("草稿")]
        [EnumMember]
        Draft = 0,

        /// <summary>
        /// 提交推荐
        /// </summary>
        [Display(Name = "提交推荐")]
        [Description("提交推荐")]
        [EnumMember]
        AuditPending = 1,

        /// <summary>
        /// 推荐上架
        /// </summary>
        [Display(Name = "推荐上架")]
        [Description("推荐上架")]
        [EnumMember]
        Approved = 2,

        /// <summary>
        /// 推荐退回
        /// </summary>
        [Display(Name = "推荐退回")]
        [Description("推荐退回")]
        [EnumMember]
        Disagree = 3,

        /// <summary>
        /// 推荐下架
        /// </summary>
        [Display(Name = "推荐下架")]
        [Description("推荐下架")]
        [EnumMember]
        Trash = 4,
    }
}
