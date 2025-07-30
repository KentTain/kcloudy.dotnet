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
    [Table(Tables.WorkflowVerDefNodeRule)]
    public class WorkflowVerDefNodeRule : RuleEntityBase
    {
        [MaxLength(20)]
        public string WorkflowVerNodeCode { get; set; }
        public Guid WorkflowVerNodeId { get; set; }
        [ForeignKey("WorkflowVerNodeId")]
        public virtual WorkflowVerDefNode WorkflowVerNode { get; set; }
    }
}
