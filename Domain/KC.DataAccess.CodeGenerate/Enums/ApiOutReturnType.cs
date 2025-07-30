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
    /// Body、Query类型
    /// </summary>
    [DataContract]
    public enum ApiOutReturnType
    {
        /// <summary>
        /// Json
        /// </summary>
        [Display(Name = "Json")]
        [Description("Json")]
        [EnumMember]
        Json = 0,

        /// <summary>
        /// Raw
        /// </summary>
        [Display(Name = "Raw")]
        [Description("Raw")]
        [EnumMember]
        Raw = 1,

        /// <summary>
        /// File
        /// </summary>
        [Display(Name = "File")]
        [Description("File")]
        [EnumMember]
        File = 2,

        /// <summary>
        /// None
        /// </summary>
        [Display(Name = "None")]
        [Description("None")]
        [EnumMember]
        None = 99,
    }
}
