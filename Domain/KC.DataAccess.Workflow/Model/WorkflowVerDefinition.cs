using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KC.Model.Workflow.Constants;
using KC.Framework.Base;
using System;
using System.Collections.Generic;

namespace KC.Model.Workflow
{
    /// <summary>
    /// 流程版本
    /// </summary>
    [Table(Tables.WorkflowVerDefinition)]
    public class WorkflowVerDefinition : DefinitionBase
    {
        public WorkflowVerDefinition()
        {
            WorkflowVerNodes = new List<WorkflowVerDefNode>();
            WorkflowVerFields = new List<WorkflowVerDefField>();
        }

        public Guid WorkflowVerDefId { get; set; }
        [ForeignKey("WorkflowVerDefId")]
        public virtual WorkflowDefinition WorkflowDefinition { get; set; }

        public virtual List<WorkflowVerDefNode> WorkflowVerNodes { get; set; }
        public virtual List<WorkflowVerDefField> WorkflowVerFields { get; set; }

    }
}
