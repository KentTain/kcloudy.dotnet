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
    public enum ApiMethodType
    {
        /// <summary>
        /// 新增接口
        /// </summary>
        [Display(Name = "新增接口")]
        [Description("新增接口")]
        [EnumMember]
        Create = 0,

        /// <summary>
        /// 删除接口
        /// </summary>
        [Display(Name = "删除接口")]
        [Description("删除接口")]
        [EnumMember]
        Remove = 1,

        /// <summary>
        /// 修改接口
        /// </summary>
        [Display(Name = "修改接口")]
        [Description("修改接口")]
        [EnumMember]
        Update = 2,


        /// <summary>
        /// 删除接口
        /// </summary>
        [Display(Name = "删除接口")]
        [Description("删除接口")]
        [EnumMember]
        Delete = 3,


        /// <summary>
        /// 查询接口
        /// </summary>
        [Display(Name = "查询接口")]
        [Description("查询接口")]
        [EnumMember]
        Search = 4,

        /// <summary>
        /// 导出接口
        /// </summary>
        [Display(Name = "导出接口")]
        [Description("导出接口")]
        [EnumMember]
        Export = 5,

        /// <summary>
        /// 导入接口
        /// </summary>
        [Display(Name = "导入接口")]
        [Description("导入接口")]
        [EnumMember]
        Import = 6,

        /// <summary>
        /// 其他
        /// </summary>
        [Display(Name = "其他")]
        [Description("其他")]
        [EnumMember]
        Other = 99,
    }
}
