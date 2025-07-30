using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using System.Runtime.Serialization;
using KC.Framework.Extension;

namespace KC.Service.DTO.Workflow
{
    /// <summary>
    /// 流程定义
    /// https://www.cnblogs.com/Relict/articles/2294642.html
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class DefinitionBaseDTO : EntityDTO
    {
        public DefinitionBaseDTO()
        {
            Id = Guid.NewGuid();
        }

        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 流程编码：wfd2012020200001
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        /// 流程版本：wfv2012020200001
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string Version { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        [MaxLength(512)]
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 流程状态
        /// </summary>
        [DataMember]
        public WorkflowBusStatus Status { get; set; }
        [DataMember]
        public string StatusString { get { return Status.ToDescription(); } }

        /// <summary>
        /// 设置的执行结束间隔天数
        /// </summary>
        [DataMember]
        public int? DefDeadlineInterval { get; set; }
        /// <summary>
        /// 消息模板编码
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string DefMessageTemplateCode { get; set; }

        #region 设置调用外部接口的安全认证数据
        /// <summary>
        /// 接口安全类型<br/>
        ///     SecurityKey = 0,<br/>
        ///     HeaderKey = 1,<br/>
        ///     OAuth = 2,<br/>
        ///     None = 99,
        /// </summary>
        [DataMember]
        public SecurityType SecurityType { get; set; }
        [DataMember]
        public string SecurityTypeString { get { return SecurityType.ToDescription(); } }
        /// <summary>
        /// 访问外部接口的认证地址
        /// </summary>
        [MaxLength(2000)]
        [DataMember]
        public string AuthAddress { get; set; }
        /// <summary>
        /// 访问外部接口的认证地址附加参数
        /// </summary>
        [MaxLength(200)]
        [DataMember]
        public string AuthAddressParams { get; set; }
        /// <summary>
        /// 访问外部接口的认证Scope
        /// </summary>
        [MaxLength(200)]
        public string AuthScope { get; set; }
        /// <summary>
        /// 访问外部接口的Key
        /// </summary>
        [MaxLength(500)]
        [DataMember]
        public string AuthKey { get; set; }
        /// <summary>
        /// 访问外部接口的Secret
        /// </summary>
        [MaxLength(2000)]
        [DataMember]
        public string AuthSecret { get; set; }

        #endregion

        #region 设置表单详情的Api地址
        /// <summary>
        /// 表单接入方式 </br>
        ///     0 ：ModelDefinition--表单数据接入
        ///     1 ：FormAddress--表单地址接入
        ///     99：None--无
        /// </summary>
        [DataMember]
        public WorkflowFormType WorkflowFormType { get; set; }
        [DataMember]
        public string WorkflowFormTypeString { get { return WorkflowFormType.ToDescription(); } }
        /// <summary>
        /// FormType=FormAddree时：应用表单可访问地址
        /// </summary>
        [MaxLength(4000)]
        [DataMember]
        public string AppFormDetailApiUrl { get; set; }
        /// <summary>
        /// FormType=FormAddree时：应用表单可访问的QueryString
        /// </summary>
        [MaxLength(4000)]
        [DataMember]
        public string AppFormDetailQueryString { get; set; }
        #endregion

        #region 设置应用回调Api地址
        /// <summary>
        /// 业务审批通过后，应用回调地址
        /// </summary>
        [MaxLength(4000)]
        [DataMember]
        public string AppAuditSuccessApiUrl { get; set; }
        /// <summary>
        /// 业务审批退回后，应用回调地址
        /// </summary>
        [MaxLength(4000)]
        [DataMember]
        public string AppAuditReturnApiUrl { get; set; }
        /// <summary>
        /// 业务审批需要传出的QueryString
        /// </summary>
        [MaxLength(4000)]
        [DataMember]
        public string AppAuditQueryString { get; set; }
        #endregion

        /// <summary>
        /// 流程描述
        /// </summary>
        [MaxLength(4000)]
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int? CategoryId { get; set; }

        [DataMember]
        public string CategoryName { get; set; }

    }
}
