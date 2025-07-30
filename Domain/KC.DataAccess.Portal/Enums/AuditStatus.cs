using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Enums.Portal
{
    [DataContract]
    public enum AuditStatus
    {
        /// <summary>
        /// 注册未认证
        /// </summary>
        [Display(Name = "注册未认证")]
        [Description("注册未认证")]
        [EnumMember]
        UnSubmit = 0,

        /// <summary>
        /// 提交认证
        /// </summary>
        [Display(Name = "提交认证")]
        [Description("提交认证")]
        [EnumMember]
        AuditPending = 1,

        /// <summary>
        /// 认证通过
        /// </summary>
        [Display(Name = "认证通过")]
        [Description("认证通过")]
        [EnumMember]
        Approved = 2,

        /// <summary>
        /// 认证不通过
        /// </summary>
        [Display(Name = "认证不通过")]
        [Description("认证不通过")]
        [EnumMember]
        Disagree = 3,
    }
}
