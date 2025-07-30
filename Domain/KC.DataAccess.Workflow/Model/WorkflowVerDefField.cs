using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Model.Workflow.Constants;

namespace KC.Model.Workflow
{
    [Table(Tables.WorkflowVerDefField)]
    public class WorkflowVerDefField : DefFieldBase<WorkflowVerDefField>
    {
        /// <summary>
        /// 流程使用版本：wfv2012020200001
        /// </summary>
        public Guid WorkflowVerDefId { get; set; }
        [ForeignKey("WorkflowVerDefId")]
        public virtual WorkflowVerDefinition WorkflowVerDefinition { get; set; }
    }
}
