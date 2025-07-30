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
    public enum ApiInputBodyType
    {
        /// <summary>
        /// Body：Json
        /// </summary>
        [Display(Name = "Json")]
        [Description("Json")]
        [EnumMember]
        Json = 0,

        /// <summary>
        /// Body：Form
        /// </summary>
        [Display(Name = "Form")]
        [Description("Form")]
        [EnumMember]
        Form = 1,

        /// <summary>
        /// Query：Query
        /// </summary>
        [Display(Name = "Query")]
        [Description("Query")]
        [EnumMember]
        Query = 2,

        /// <summary>
        /// Query：Path
        /// </summary>
        [Display(Name = "Path")]
        [Description("Path")]
        [EnumMember]
        Path = 3,
    }
}
