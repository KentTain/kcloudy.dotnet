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
    [Table(Tables.WorkflowProField)]
    public class WorkflowProField : DefFieldBase<WorkflowProField>
    {
        public Guid ProcessId { get; set; }
        [ForeignKey("ProcessId")]
        public virtual WorkflowProcess Process { get; set; }
    }
}
