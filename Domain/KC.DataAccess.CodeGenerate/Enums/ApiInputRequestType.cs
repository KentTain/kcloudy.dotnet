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
    /// 请求类型
    /// </summary>
    [DataContract]
    public enum ApiInputRequestType
    {
        /// <summary>
        /// Body
        /// </summary>
        [Display(Name = "Body")]
        [Description("Body")]
        [EnumMember]
        Body = 0,

        /// <summary>
        /// Query
        /// </summary>
        [Display(Name = "Query")]
        [Description("Query")]
        [EnumMember]
        Query = 1,

        /// <summary>
        /// Header
        /// </summary>
        [Display(Name = "Header")]
        [Description("Header")]
        [EnumMember]
        Header = 2,
    }
}
