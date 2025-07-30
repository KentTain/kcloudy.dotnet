using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Model.Workflow.Constants;
using KC.Enums.Workflow;
using KC.Framework.Base;

namespace KC.Model.Workflow
{
    [Table(Tables.WorkflowProTaskExecute)]
    public class WorkflowProTaskExecute : EntityBase
    {
        public WorkflowProTaskExecute()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //数据库生成默认值
        public int Id { get; set; }

        /// <summary>
        /// 执行人Ids
        /// </summary>
        [MaxLength(128)]
        public string ExecuteUserId { get; set; }
        /// <summary>
        /// 执行人姓名
        /// </summary>
        [MaxLength(50)]
        public string ExecuteUserName { get; set; }

        /// <summary>
        /// 执行时间
        /// </summary>
        public DateTime ExecuteDateTime { get; set; }

        /// <summary>
        /// 下一节点的审核人Ids
        /// </summary>
        //[MaxLength(4000)] 
        public string NextAuditorUserIds { get; set; }
        /// <summary>
        /// 下一节点的审核人姓名列表
        /// </summary>
        [MaxLength(4000)]
        public string NextAuditorUserNames { get; set; }

        /// <summary>
        /// 流程任务执行状态
        /// </summary>
        public WorkflowExecuteStatus ExecuteStatus { get; set; }

        /// <summary>
        /// 执行附件
        /// </summary>
        [MaxLength(4000)]
        public string ExecuteFileBlob { get; set; }

        /// <summary>
        /// 执行描述
        /// </summary>
        [MaxLength(4000)]
        public string ExecuteRemark { get; set; }

        public Guid TaskId { get; set; }
        [ForeignKey("TaskId")]
        public virtual WorkflowProTask Task { get; set; }
    }
}
