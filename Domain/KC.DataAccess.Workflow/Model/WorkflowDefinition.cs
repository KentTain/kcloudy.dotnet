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
    /// 流程定义
    /// https://www.cnblogs.com/Relict/articles/2294642.html
    /// </summary>
    [Table(Tables.WorkflowDefinition)]
    public class WorkflowDefinition : DefinitionBase
    {
        public WorkflowDefinition()
        {
            WorkflowVersions = new List<WorkflowVerDefinition>();
            WorkflowNodes = new List<WorkflowDefNode>();
            WorkflowFields = new List<WorkflowDefField>();
        }

        public virtual List<WorkflowDefNode> WorkflowNodes { get; set; }
        public virtual List<WorkflowDefField> WorkflowFields { get; set; }
        public virtual List<WorkflowVerDefinition> WorkflowVersions { get; set; }

    }
}
