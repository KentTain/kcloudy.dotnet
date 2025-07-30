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
    [Table(Tables.WorkflowVerDefNode)]
    public class WorkflowVerDefNode : DefNodeBase<WorkflowVerDefNodeRule>
    {
        public WorkflowVerDefNode()
        {
            Id = Guid.NewGuid();
        }

        //public Guid? PrevNodeId { get; set; }
        //public virtual WorkflowVerDefNode PrevNode { get; set; }
        //public Guid? NextNodeId { get; set; }
        //public virtual WorkflowVerDefNode NextNode { get; set; }
        //public Guid? ReturnNodeId { get; set; }
        //public virtual WorkflowVerDefNode ReturnNode { get; set; }

        /// <summary>
        /// 流程使用版本：wfv2012020200001
        /// </summary>
        public Guid WorkflowVerDefId { get; set; }
        [ForeignKey("WorkflowVerDefId")]
        public virtual WorkflowVerDefinition WorkflowVerDefinition { get; set; }
    }
}
