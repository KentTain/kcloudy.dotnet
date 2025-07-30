using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.Portal
{
    [DataContract]
    public enum WebSiteColumnType
    {
        /// <summary>
        /// 图片式
        /// </summary>
        [Description("图片式")]
        [EnumMember]
        Image = 0,

        /// <summary>
        /// 卡片式
        /// </summary>
        [Description("卡片式")]
        [EnumMember]
        Card = 1,

        /// <summary>
        /// 产品式
        /// </summary>
        [Description("产品式")]
        [EnumMember]
        Product = 2,

        /// <summary>
        /// 轮播式
        /// </summary>
        [Description("轮播式")]
        [EnumMember]
        Slide = 3,

        /// <summary>
        /// 列表式
        /// </summary>
        [Description("列表式")]
        [EnumMember]
        List = 4,

        /// <summary>
        /// 自定义
        /// </summary>
        [Description("自定义")]
        [EnumMember]
        Customize = 99,
    }

}
