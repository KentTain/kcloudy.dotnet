using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.App
{
    /// <summary>
    /// 模板类型
    /// </summary>
    [DataContract]
    public enum TemplateType
    {
        /// <summary>
        /// 后端模板
        /// </summary>
        [Display(Name = "后端模板")]
        [Description("后端模板")]
        [EnumMember]
        BackEnd = 1,

        /// <summary>
        /// 前端模板
        /// </summary>
        [Display(Name = "前端模板")]
        [Description("前端模板")]
        [EnumMember]
        FrontEnd = 2,

        /// <summary>
        /// 移动模板
        /// </summary>
        [Display(Name = "移动模板")]
        [Description("移动模板")]
        [EnumMember]
        Mobile = 4,

        /// <summary>
        /// H5模板
        /// </summary>
        [Display(Name = "H5模板")]
        [Description("H5模板")]
        [EnumMember]
        H5 = 8,
    }
}
