using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Model.Workflow.Constants;
using KC.Framework.Base;
using System.Runtime.Serialization;

namespace KC.Model.Workflow
{
    /// <summary>
    /// 流程定义
    /// https://www.cnblogs.com/Relict/articles/2294642.html
    /// </summary>
    public abstract class DefinitionBase : Entity
    {
        public DefinitionBase()
        {
            Id = Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 流程编码：wfd2012020200001
        /// </summary>
        [MaxLength(20)]
        public string Code { get; set; }

        /// <summary>
        /// 流程版本：wfv2012020200001
        /// </summary>
        [MaxLength(20)]
        public string Version { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        [MaxLength(512)]
        public string Name { get; set; }

        /// <summary>
        /// 流程状态
        /// </summary>
        public WorkflowBusStatus Status { get; set; }

        /// <summary>
        /// 设置的默认执行结束间隔天数
        /// </summary>
        public int? DefDeadlineInterval { get; set; }
        /// <summary>
        /// 消息模板编码
        /// </summary>
        [MaxLength(20)]
        public string DefMessageTemplateCode { get; set; }

        #region 设置调用外部接口的安全认证数据
        /// <summary>
        /// 接口安全类型<br/>
        ///     SecurityKey = 0,<br/>
        ///     HeaderKey = 1,<br/>
        ///     OAuth = 2,<br/>
        ///     None = 99,
        /// </summary>
        public SecurityType SecurityType { get; set; }

        /// <summary>
        /// 访问外部接口的认证地址
        /// </summary>
        [MaxLength(2000)]
        public string AuthAddress { get; set; }
        /// <summary>
        /// 访问外部接口的认证地址附加参数
        /// </summary>
        [MaxLength(200)]
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
        public string AuthKey { get; set; }
        /// <summary>
        /// 访问外部接口的Secret
        /// </summary>
        [MaxLength(2000)]
        public string AuthSecret { get; set; }
        #endregion

        #region 设置表单详情的Api地址

        /// <summary>
        /// 表单接入方式 </br>
        ///     0 ：ModelDefinition--表单数据接入
        ///     1 ：FormAddress--表单地址接入
        ///     99：None--无
        /// </summary>
        public WorkflowFormType WorkflowFormType { get; set; }
        /// <summary>
        /// 应用表单可访问地址
        /// </summary>
        [MaxLength(4000)]
        public string AppFormDetailApiUrl { get; set; }
        /// <summary>
        /// 应用表单可访问的QueryString
        /// </summary>
        [MaxLength(4000)]
        public string AppFormDetailQueryString { get; set; }
        #endregion

        #region 设置应用回调Api地址

        /// <summary>
        /// 业务审批通过后，应用回调地址
        /// </summary>
        [MaxLength(4000)]
        public string AppAuditSuccessApiUrl { get; set; }
        /// <summary>
        /// 业务审批退回后，应用回调地址
        /// </summary>
        [MaxLength(4000)]
        public string AppAuditReturnApiUrl { get; set; }

        /// <summary>
        /// 业务审批需要传出的QueryString
        /// </summary>
        [MaxLength(4000)]
        public string AppAuditQueryString { get; set; }

        #endregion

        /// <summary>
        /// 流程描述
        /// </summary>
        [MaxLength(4000)]
        public string Description { get; set; }

        public int? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual WorkflowCategory WorkflowCategory { get; set; }
    }
}
