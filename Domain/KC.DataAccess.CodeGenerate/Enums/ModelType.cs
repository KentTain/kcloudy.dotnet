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
    [DataContract]
    public enum ModelType
    {
        /// <summary>
        /// 数据模型
        /// </summary>
        [Display(Name = "数据模型")]
        [Description("数据模型")]
        [EnumMember]
        ModelDefinition = 0,

        /// <summary>
        /// 关系模型
        /// </summary>
        [Display(Name = "关系模型")]
        [Description("关系模型")]
        [EnumMember]
        RelationDefinition = 1,

        /// <summary>
        /// 接口定义
        /// </summary>
        [Display(Name = "接口定义")]
        [Description("接口定义")]
        [EnumMember]
        ApiDefinition = 2,

        /// <summary>
        /// 接口实现日志
        /// </summary>
        [Display(Name = "接口实现")]
        [Description("接口实现")]
        [EnumMember]
        ApiImplement = 3,

        /// <summary>
        /// 其他
        /// </summary>
        [Display(Name = "其他")]
        [Description("其他")]
        [EnumMember]
        Other = 99,
    }
}
