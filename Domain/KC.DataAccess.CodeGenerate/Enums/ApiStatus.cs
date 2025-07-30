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
    public enum ApiStatus
    {
        /// <summary>
        /// 定义接口
        /// </summary>
        [Display(Name = "定义接口")]
        [Description("定义接口")]
        [EnumMember]
        Definition = 0,

        /// <summary>
        /// 实现接口
        /// </summary>
        [Display(Name = "实现接口")]
        [Description("实现接口")]
        [EnumMember]
        Implement = 1,


        /// <summary>
        /// 发布接口
        /// </summary>
        [Display(Name = "发布接口")]
        [Description("发布接口")]
        [EnumMember]
        Publish = 2,
    }
}
