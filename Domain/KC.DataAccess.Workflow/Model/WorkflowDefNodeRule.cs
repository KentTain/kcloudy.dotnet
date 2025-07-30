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
    /// 流程条件的规则
    /// </summary>
    [Table(Tables.WorkflowDefNodeRule)]
    public class WorkflowDefNodeRule : RuleEntityBase
    {
        [MaxLength(20)]
        public string WorkflowNodeCode { get; set; }
        public Guid WorkflowNodeId { get; set; }
        [ForeignKey("WorkflowNodeId")]
        public virtual WorkflowDefNode WorkflowNode { get; set; }
    }
}
