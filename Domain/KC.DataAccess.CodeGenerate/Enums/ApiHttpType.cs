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
    public enum ApiHttpType
    {
        /// <summary>
        /// Get
        /// </summary>
        [Display(Name = "Get")]
        [Description("Get")]
        [EnumMember]
        Get = 0,

        /// <summary>
        /// Post
        /// </summary>
        [Display(Name = "Post")]
        [Description("Post")]
        [EnumMember]
        Post = 1,

        /// <summary>
        /// Put
        /// </summary>
        [Display(Name = "Put")]
        [Description("Put")]
        [EnumMember]
        Put = 2,

        /// <summary>
        /// Delete
        /// </summary>
        [Display(Name = "Delete")]
        [Description("Delete")]
        [EnumMember]
        Delete = 3,
    }
}
