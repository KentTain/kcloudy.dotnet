using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Model.Workflow.Constants;

namespace KC.Model.Workflow
{
    [Table(Tables.WorkflowDefField)]
    public class WorkflowDefField : DefFieldBase<WorkflowDefField>
    {
        public Guid WorkflowDefId { get; set; }
        [ForeignKey("WorkflowDefId")]
        public virtual WorkflowDefinition WorkflowDefinition { get; set; }
    }
}
