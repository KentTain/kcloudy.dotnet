using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Framework.Base
{
    [DataContract]
    public enum AttributeDataType
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
        /// 日期时间
        /// </summary>
        [Display(Name = "日期时间")]
        [Description("日期时间")]
        [EnumMember]
        DateTime = 4,

        /// <summary>
        /// 文本
        /// </summary>
        [Display(Name = "文本")]
        [Description("文本")]
        [EnumMember]
        Text = 5,

        /// <summary>
        /// 富文本
        /// </summary>
        [Display(Name = "富文本")]
        [Description("富文本")]
        [EnumMember]
        RichText = 6,

        /// <summary>
        /// 文件
        /// </summary>
        [Display(Name = "文件")]
        [Description("文件")]
        [EnumMember]
        File = 7,

        /// <summary>
        /// 图片
        /// </summary>
        [Display(Name = "图片")]
        [Description("图片")]
        [EnumMember]
        Image = 8,

        /// <summary>
        /// 对象
        /// </summary>
        [Display(Name = "对象")]
        [Description("对象")]
        [EnumMember]
        Object = 9,

        /// <summary>
        /// 列表
        /// </summary>
        [Display(Name = "列表")]
        [Description("列表")]
        [EnumMember]
        List = 10,
    }

}
