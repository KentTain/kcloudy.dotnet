using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using System.Runtime.Serialization;
using KC.Framework.Extension;
using KC.Service.Enums.Workflow;

namespace KC.Service.DTO.Workflow
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class WorkflowProTaskDTO : DefNodeBaseDTO<WorkflowProTaskRuleDTO>
    {
        public WorkflowProTaskDTO()
        {
            TaskExecutes = new List<WorkflowProTaskExecuteDTO>();
        }

        /// <summary>
        /// 流程任务状态
        /// </summary>
        [DataMember]
        public WorkflowTaskStatus TaskStatus { get; set; }
        [DataMember]
        public string TaskStatusString { get { return TaskStatus.ToDescription(); } }

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

        #region 当任务处理人记录
        /// <summary>
        /// 同意人Ids
        /// </summary>
        public string AgreeUserIds { get; set; }
        /// <summary>
        /// 同意人姓名
        /// </summary>
        public string AgreeUserNames { get; set; }

        /// <summary>
        /// 不同意人Ids
        /// </summary>
        public string DisagreeUserIds { get; set; }
        /// <summary>
        /// 不同意人姓名
        /// </summary>
        public string DisagreeUserNames { get; set; }

        /// <summary>
        /// 未处理人Ids
        /// </summary>
        public string UnProcessUserIds { get; set; }
        /// <summary>
        /// 未处理人姓名
        /// </summary>
        public string UnProcessUserNames { get; set; }

        /// <summary>
        /// 所有处理人Ids
        /// </summary>
        public string AllUserIds { get; set; }
        /// <summary>
        /// 所有处理人姓名
        /// </summary>
        public string AllUserNames { get; set; }
        #endregion

        [DataMember]
        public Guid? PrevNodeId { get; set; }
        [DataMember]
        public WorkflowProTaskDTO PrevNode { get; set; }
        [DataMember]
        public Guid? NextNodeId { get; set; }
        [DataMember]
        public WorkflowProTaskDTO NextNode { get; set; }
        [DataMember]
        public Guid? ReturnNodeId { get; set; }
        [DataMember]
        public WorkflowProTaskDTO ReturnNode { get; set; }


        [DataMember]
        public Guid ProcessId { get; set; }
        [DataMember]
        public string ProcessCode { get; set; }
        [DataMember]
        public string ProcessVersion { get; set; }
        [DataMember]
        public string ProcessName { get; set; }

        [DataMember]
        public List<WorkflowProTaskExecuteDTO> TaskExecutes { get; set; }

    }
}
