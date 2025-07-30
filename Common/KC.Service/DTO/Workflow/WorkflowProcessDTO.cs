using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using System.Runtime.Serialization;
using KC.Service.Enums.Workflow;

namespace KC.Service.DTO.Workflow
{
    /// <summary>
    /// 流程执行处理过程
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class WorkflowProcessDTO : DefinitionBaseDTO
    {
        public WorkflowProcessDTO()
        {
            Tasks = new List<WorkflowProTaskDTO>();
            Context = new List<WorkflowProFieldDTO>();
        }

        /// <summary>
        /// 发起人Id
        /// </summary>
        [Display(Name = "发起人Id")]
        [MaxLength(128)]
        [DataMember]
        public string SubmitUserId { get; set; }
        /// <summary>
        /// 发起人
        /// </summary>
        [Display(Name = "发起人")]
        [MaxLength(128)]
        [DataMember]
        public string SubmitUserName { get; set; }

        /// <summary>
        /// 流程状态
        /// </summary>
        [DataMember]
        public WorkflowProcessStatus ProcessStatus { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [DataMember]
        public DateTime? StartDateTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        [DataMember]
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// 规定结束时间
        /// </summary>
        [DataMember]
        public DateTime? DeadlineDate { get; set; }

        /// <summary>
        /// 任务Id
        /// </summary>
        [DataMember]
        public Guid CurrentTaskId { get; set; }

        #region 流程定义数据
        /// <summary>
        /// 流程定义Id--WorkflowDefinition的Id
        /// </summary>
        [DataMember]
        public Guid WorkflowDefId { get; set; }
        /// <summary>
        /// 流程定义编码--WorkflowDefinition的Code：wfd2012020200001
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string WorkflowDefCode { get; set; }
        /// <summary>
        /// 流程定义名称
        /// </summary>
        [MaxLength(512)]
        [DataMember]
        public string WorkflowDefName { get; set; }
        #endregion

        /// <summary>
        /// 流程任务数据
        /// </summary>
        [DataMember]
        public List<WorkflowProTaskDTO> Tasks { get; set; }
        /// <summary>
        /// 流程表单数据
        /// </summary>
        [DataMember]
        public List<WorkflowProFieldDTO> Context { get; set; }
    }
}
