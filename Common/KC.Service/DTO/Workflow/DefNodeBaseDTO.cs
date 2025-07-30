using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Service.Enums.Workflow;

namespace KC.Service.DTO.Workflow
{
    /// <summary>
    /// 流程节点步骤
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class DefNodeBaseDTO<T> : DataPermitEntityDTO where T : RuleEntityBaseDTO
    {
        public DefNodeBaseDTO()
        {
            Id = Guid.NewGuid();
            //ExecuteOrgIds = new List<string>();
            //ExecuteRoleIds = new List<string>();
            //ExecuteUserIds = new List<string>();
            Rules = new List<T>();
        }

        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// 节点编码：wfn2012020200001
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string Code { get; set; }
        /// <summary>
        /// 节点名称
        /// </summary>
        [MaxLength(512)]
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 节点类型：0：Start-开始；1：Task-任务、2：Condition-条件、3：SubFlow-子流程、4：End-结束
        /// </summary>
        [DataMember]
        public WorkflowNodeType NodeType { get; set; }
        [DataMember]
        public string NodeTypeString { get { return NodeType.ToDescription(); } }

        /// <summary>
        /// 设置的执行结束间隔天数
        /// </summary>
        [DataMember]
        public int? DeadlineInterval { get; set; }

        /// <summary>
        /// 消息模板编码
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string MessageTemplateCode { get; set; }

        /// <summary>
        /// 节点离顶部距离
        /// </summary>
        [DataMember]
        public decimal LocTop { get; set; }
        /// <summary>
        /// 节点离左边距离
        /// </summary>
        [DataMember]
        public decimal LocLeft { get; set; }

        /// <summary>
        /// 前一节点编码
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string PrevNodeCode { get; set; }
        /// <summary>
        /// 后一节点编码
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string NextNodeCode { get; set; }
        /// <summary>
        /// 回退节点编码（如果NodeType==Condition，需要填写回退节点Code）
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string ReturnNodeCode { get; set; }

        #region 子流程设置（如果NodeType==SubFlow，需要填写子流程）
        /// <summary>
        /// 子流程Id
        /// </summary>
        [DataMember]
        public Guid? SubDefinitionId { get; set; }
        /// <summary>
        /// 子流程编码：wfd2012020200001
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string SubDefinitionCode { get; set; }
        /// <summary>
        /// 子流程版本：wfv2012020200001
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string SubDefinitionVersion { get; set; }
        /// <summary>
        /// 子流程名称
        /// </summary>
        [MaxLength(512)]
        [DataMember]
        public string SubDefinitionName { get; set; }

        #endregion

        /// <summary>
        /// 流程类型：0：SingleLine-单线审批、1：MultiLine-多线审批、2：WeightLine-权重审批
        /// </summary>
        [DataMember]
        public WorkflowType Type { get; set; }
        [DataMember]
        public string TypeString { get { return Type.ToDescription(); } }
        /// <summary>
        /// 权重值
        /// </summary>
        [DataMember]
        public decimal WeightValue { get; set; }

        /// <summary>
        /// 执行人设置类型
        ///     0：Executor，需要设置执行人（组织、角色、用户） </br>
        ///     10~13：CreateManager，流程发起人的主管 </br>
        ///     20~23：SubmitManager，上一流程节点的提交人的主管 </br>
        ///     30：FormAuditor，表单设置的审批人
        /// </summary>
        [DataMember]
        public ExecutorSetting ExecutorSetting { get; set; }
        /// <summary>
        /// 当ExecutorSetting=FormAuditor时，需要设置相关表单的字段名
        /// </summary>
        [MaxLength(500)]
        [DataMember]
        public string ExecutorFormFieldName { get; set; }
        /// <summary>
        /// 当ExecutorSetting=FormAuditor时，需要设置相关表单的字段显示名
        /// </summary>
        [MaxLength(1000)]
        [DataMember]
        public string ExecutorFormFieldDisplayName { get; set; }
        /// <summary>
        /// 通知人Id
        /// </summary>
        //[MaxLength(4000)] 
        [DataMember]
        public string NotifyUserIds { get; set; }
        /// <summary>
        /// 通知人姓名
        /// </summary>
        [MaxLength(4000)]
        [DataMember]
        public string NotifyUserNames { get; set; }

        /// <summary>
        /// 条件为true时的设置公式：WorkflowDefField.Name >=、=、<=、contains 值
        /// </summary>
        [DataMember]
        public List<T> Rules { get; set; }
    }
}
