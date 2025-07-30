using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KC.Framework.Base;
using KC.Model.Workflow.Constants;

namespace KC.Model.Workflow
{
    /// <summary>
    /// 流程节点步骤
    /// </summary>
    [Table(Tables.WorkflowDefNode)]
    public class WorkflowDefNode : DefNodeBase<WorkflowDefNodeRule>
    {
        //public Guid? PrevNodeId { get; set; }
        //public virtual WorkflowDefNode PrevNode { get; set; }
        //public Guid? NextNodeId { get; set; }
        //public virtual WorkflowDefNode NextNode { get; set; }
        //public Guid? ReturnNodeId { get; set; }
        //public virtual WorkflowDefNode ReturnNode { get; set; }

        public Guid WorkflowDefId { get; set; }
        [ForeignKey("WorkflowDefId")]
        public virtual WorkflowDefinition WorkflowDefinition { get; set; }
    }
}
