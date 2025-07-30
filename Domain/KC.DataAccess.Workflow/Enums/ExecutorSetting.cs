using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.Workflow
{
    [Serializable]
    [DataContract]
    public enum ExecutorSetting
    {
        /// <summary>
        /// 设置组织角色及用户
        /// </summary>
        [Description("设置组织角色及用户")]
        [EnumMember]
        Executor = 0,

        /// <summary>
        /// 流程发起人的主管
        /// </summary>
        [Description("流程发起人的主管")]
        [EnumMember]
        CreatorManager = 10,
        /// <summary>
        /// 流程发起人所在组织
        /// </summary>
        [Description("流程发起人所在组织")]
        [EnumMember]
        CreatorOrganization = 11,
        /// <summary>
        /// 流程发起人所属角色
        /// </summary>
        [Description("流程发起人所属角色")]
        [EnumMember]
        CreatorRole = 12,

        /// <summary>
        /// 流程发起人的上级主管
        /// </summary>
        [Description("流程发起人的上级主管")]
        [EnumMember]
        CreatorSuperior = 13,

        /// <summary>
        /// 提交审批人的主管
        /// </summary>
        [Description("提交审批人的主管")]
        [EnumMember]
        SubmitterManager = 20,
        /// <summary>
        /// 提交审批人所在组织
        /// </summary>
        [Description("提交审批人所在组织")]
        [EnumMember]
        SubmitterOrganization = 21,
        /// <summary>
        /// 提交审批人所属角色
        /// </summary>
        [Description("提交审批人所属角色")]
        [EnumMember]
        SubmitterRole = 22,
        /// <summary>
        /// 提交审批人的上级主管
        /// </summary>
        [Description("提交审批人的上级主管")]
        [EnumMember]
        SubmitterSuperior = 23,

        /// <summary>
        /// 表单设置的审批人字段
        /// </summary>
        [Description("表单设置的审批人")]
        [EnumMember]
        FormAuditor = 30,

        /// <summary>
        /// 表单设置的审批组织字段
        /// </summary>
        [Description("表单设置的审批组织")]
        [EnumMember]
        FormAuditOrg = 31,

        /// <summary>
        /// 表单设置的审批角色字段
        /// </summary>
        [Description("表单设置的审批角色")]
        [EnumMember]
        FormAuditRole = 32,
    }
}
