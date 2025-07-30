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
    public enum RelationType
    {
        /// <summary>
        /// 主从关系
        /// </summary>
        [Display(Name = "主从关系")]
        [Description("主从关系")]
        [EnumMember]
        OneByMany = 0,

        /// <summary>
        /// 引用关系
        /// </summary>
        [Display(Name = "引用关系")]
        [Description("引用关系")]
        [EnumMember]
        OneByOne = 1,
    }
}
