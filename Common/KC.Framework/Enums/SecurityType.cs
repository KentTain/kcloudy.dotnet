using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Framework.Base
{
    [DataContract]
    public enum SecurityType
    {
        /// <summary>
        /// 安全码
        /// </summary>
        [Description("安全码")]
        [EnumMember]
        SecurityKey = 0,
        /// <summary>
        /// 请求头
        /// </summary>
        [Description("请求头")]
        [EnumMember]
        HeaderKey = 1,
        /// <summary>
        /// OAuth认证
        /// </summary>
        [Description("OAuth认证")]
        [EnumMember]
        OAuth = 2,
        /// <summary>
        /// 无需验证
        /// </summary>
        [Description("无")]
        [EnumMember]
        None = 99,
    }

    [DataContract]
    public enum WorkflowFormType
    {
        /// <summary>
        /// 表单数据接入
        /// </summary>
        [Description("表单数据接入")]
        [EnumMember]
        ModelDefinition = 0,
        /// <summary>
        /// 表单地址接入
        /// </summary>
        [Description("表单地址接入")]
        [EnumMember]
        FormAddress = 1,
        /// <summary>
        /// 不接入表单
        /// </summary>
        [Description("无")]
        [EnumMember]
        None = 99,
    }

    [DataContract]
    public enum HttpRequestType
    {
        /// <summary>
        /// Get
        /// </summary>
        [Description("Get")]
        [EnumMember]
        Get = 0,
        /// <summary>
        /// Post
        /// </summary>
        [Description("Post")]
        [EnumMember]
        Post = 1,
        /// <summary>
        /// Put
        /// </summary>
        [Description("Put")]
        [EnumMember]
        Put = 2,
        /// <summary>
        /// Delete
        /// </summary>
        [Description("Delete")]
        [EnumMember]
        Delete = 3,
    }
}
