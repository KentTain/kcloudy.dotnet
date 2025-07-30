using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Model.Workflow.Constants;
using KC.Framework.Base;

namespace KC.Model.Workflow
{
    [Table(Tables.WorkflowCategory)]
    public class WorkflowCategory : TreeNode<WorkflowCategory>
    {
        public WorkflowCategory()
        {
            Definitions = new List<WorkflowDefinition>();
        }

        [MaxLength(4000)]
        public string Description { get; set; }

        public virtual ICollection<WorkflowDefinition> Definitions { get; set; }
    }
}
