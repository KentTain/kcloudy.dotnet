using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.Portal
{
    /// <summary>
    /// 底部菜单类型
    /// </summary>
    [DataContract]
    public enum LinkType
    {
        /// <summary>
        /// 关于我们
        /// </summary>
        [EnumMember]
        [Display(Name = "关于我们")]
        [Description("关于我们")]
        AboutUs = 0,
        /// <summary>
        /// 新手指引
        /// </summary>
        [EnumMember]
        [Display(Name = "帮助中心")]
        [Description("帮助中心")]
        NoviceGuide = 1,
        [EnumMember]
        [Display(Name = "合作机构")]
        [Description("合作机构")]
        CooperationAgency=3,
        /// <summary>
        /// 友情链接
        /// </summary>
        [EnumMember]
        [Display(Name = "友情链接")]
        [Description("友情链接")]
        Links = 2,
        /// <summary>
        /// 咨询服务
        /// </summary>
        [EnumMember]
        [Display(Name = "咨询服务")]
        [Description("咨询服务")]
        AdvisoryServices =4,

        /// <summary>
        /// 其他
        /// </summary>
        [EnumMember]
        [Display(Name = "其他")]
        [Description("其他")]
        Other = 99



    }
}
