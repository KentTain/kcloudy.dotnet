using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.Admin
{
    /// <summary>
    /// 模型继承类型
    /// </summary>
    [DataContract]
    public enum TemplateType
    {
        /// <summary>
        /// 前端模板
        /// </summary>
        [Display(Name = "前端模板")]
        [Description("前端模板")]
        [EnumMember]
        BackEnd = 0,

        /// <summary>
        /// 后端模板
        /// </summary>
        [Display(Name = "后端模板")]
        [Description("后端模板")]
        [EnumMember]
        FrontEnd = 1,

    }
}
