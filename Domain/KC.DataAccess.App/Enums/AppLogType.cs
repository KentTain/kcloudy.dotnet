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
    [DataContract]
    public enum AppLogType
    {
        /// <summary>
        /// 应用日志
        /// </summary>
        [Display(Name = "应用日志")]
        [Description("应用日志")]
        [EnumMember]
        Application = 0,

        /// <summary>
        /// 应用设置日志
        /// </summary>
        [Display(Name = "应用设置日志")]
        [Description("应用设置日志")]
        [EnumMember]
        AppSetting = 1,

        /// <summary>
        /// 其他
        /// </summary>
        [Display(Name = "其他")]
        [Description("其他")]
        [EnumMember]
        Other = 99,
    }
}
