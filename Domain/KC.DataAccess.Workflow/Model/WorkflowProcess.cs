using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Model.Workflow.Constants;
using KC.Enums.Workflow;

namespace KC.Model.Workflow
{
    /// <summary>
    /// 流程执行处理过程
    /// </summary>
    [Table(Tables.WorkflowProcess)]
    public class WorkflowProcess : DefinitionBase
    {
        public WorkflowProcess()
        {
            Tasks = new List<WorkflowProTask>();
            Context = new List<WorkflowProField>();
        }

        /// <summary>
        /// 发起人Id
        /// </summary>
        [Display(Name = "发起人Id")]
        [MaxLength(128)]
        public string SubmitUserId { get; set; }
        /// <summary>
        /// 发起人
        /// </summary>
        [Display(Name = "发起人")]
        [MaxLength(128)]
        public string SubmitUserName { get; set; }
        /// <summary>
        /// 流程状态
        /// </summary>
        public WorkflowProcessStatus ProcessStatus { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDateTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// 规定结束时间
        /// </summary>
        public DateTime? DeadlineDate { get; set; }

        public Guid CurrentTaskId { get; set; }

        #region 流程定义数据
        /// <summary>
        /// 流程定义Id--WorkflowDefinition的Id
        /// </summary>
        public Guid WorkflowDefId { get; set; }
        /// <summary>
        /// 流程定义编码--WorkflowDefinition的Code：wfd2012020200001
        /// </summary>
        [MaxLength(20)]
        public string WorkflowDefCode { get; set; }
        /// <summary>
        /// 流程定义名称
        /// </summary>
        [MaxLength(512)]
        public string WorkflowDefName { get; set; }
        #endregion

        /// <summary>
        /// 流程任务数据
        /// </summary>
        public virtual ICollection<WorkflowProTask> Tasks { get; set; }

        /// <summary>
        /// 流程表单数据
        /// </summary>
        public virtual ICollection<WorkflowProField> Context { get; set; }
    }
}
