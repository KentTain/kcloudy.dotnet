using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.CodeGenerate
{
    /// <summary>
    /// 模型继承类型
    /// </summary>
    [DataContract]
    public enum ModelBaseType
    {
        /// <summary>
        /// 基础类型
        /// </summary>
        [Display(Name = "基础类型")]
        [Description("基础类型")]
        [EnumMember]
        EntityBase = 0,

        /// <summary>
        /// 实体类型
        /// </summary>
        [Display(Name = "实体类型")]
        [Description("实体类型")]
        [EnumMember]
        Entity = 1,

        /// <summary>
        /// 树型结构
        /// </summary>
        [Display(Name = "树型结构")]
        [Description("树型结构")]
        [EnumMember]
        TreeNode = 2,

        /// <summary>
        /// 属性设置
        /// </summary>
        [Display(Name = "属性设置")]
        [Description("属性设置")]
        [EnumMember]
        Property = 3,

        /// <summary>
        /// 属性值设置
        /// </summary>
        [Display(Name = "属性值设置")]
        [Description("属性值设置")]
        [EnumMember]
        PropertyAttribute = 4,
    }
}
