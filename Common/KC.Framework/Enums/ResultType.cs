using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Framework.Base
{
    [DataContract]
    public enum ResultType
    {
        /// <summary>
        /// ActionResult
        /// </summary>
        [EnumMember]
        [Description("ActionResult")]
        ActionResult = 0,
        /// <summary>
        /// JsonResult
        /// </summary>
        [EnumMember]
        [Description("JsonResult")]
        JsonResult = 1,
        /// <summary>
        /// ContentResult
        /// </summary>
        [EnumMember]
        [Description("ContentResult")]
        ContentResult = 2,
        /// <summary>
        /// RedirectResult
        /// </summary>
        [EnumMember]
        [Description("RedirectResult")]
        RedirectResult = 3,
        /// <summary>
        /// ImageResult
        /// </summary>
        [EnumMember]
        [Description("ImageResult")]
        ImageResult = 4,

    }
}
