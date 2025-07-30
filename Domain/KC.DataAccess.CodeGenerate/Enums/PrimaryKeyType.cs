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
    public enum PrimaryKeyType
    {
        /// <summary>
        /// 自增
        /// </summary>
        [Display(Name = "自增")]
        [Description("自增")]
        [EnumMember]
        IDENTITY = 0,

        /// <summary>
        /// UUID
        /// </summary>
        [Display(Name = "UUID")]
        [Description("UUID")]
        [EnumMember]
        UUID = 1,


        /// <summary>
        /// 雪花
        /// </summary>
        [Display(Name = "雪花")]
        [Description("雪花")]
        [EnumMember]
        SnowFlake = 2,
    }
}
