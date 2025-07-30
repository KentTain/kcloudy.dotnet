using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Service.DTO.Workflow;

namespace KC.Service.Workflow.DTO
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class WorkflowDefFieldDTO : DefFieldBaseDTO<WorkflowDefFieldDTO>
    {
        [DataMember]
        public Guid WorkflowDefId { get; set; }

        [DataMember]
        public WorkflowDefinitionDTO WorkflowDefinition { get; set; }
    }
}
