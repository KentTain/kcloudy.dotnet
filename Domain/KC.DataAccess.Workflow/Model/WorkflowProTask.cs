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
    [Table(Tables.WorkflowProTask)]
    public class WorkflowProTask : DefNodeBase<WorkflowProTaskRule>
    {
        public WorkflowProTask()
        {
            TaskExecutes = new List<WorkflowProTaskExecute>();
        }

        /// <summary>
        /// 流程任务状态
        /// </summary>
        public WorkflowTaskStatus TaskStatus { get; set; }

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

        //public Guid? PrevNodeId { get; set; }
        //public virtual WorkflowProTask PrevNode { get; set; }
        //public Guid? NextNodeId { get; set; }
        //public virtual WorkflowProTask NextNode { get; set; }
        //public Guid? ReturnNodeId { get; set; }
        //public virtual WorkflowProTask ReturnNode { get; set; }

        public Guid ProcessId { get; set; }
        [ForeignKey("ProcessId")]
        public virtual WorkflowProcess Process { get; set; }

        public virtual ICollection<WorkflowProTaskExecute> TaskExecutes { get; set; }

    }
}
