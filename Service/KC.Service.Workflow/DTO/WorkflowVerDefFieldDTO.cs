using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Service.DTO.Workflow;

namespace KC.Service.Workflow.DTO
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class WorkflowVerDefFieldDTO : DefFieldBaseDTO<WorkflowVerDefFieldDTO>
    {
        /// <summary>
        /// 流程使用版本：wfv2012020200001
        /// </summary>
        [DataMember]
        public Guid WorkflowVerDefId { get; set; }

        [DataMember]
        public WorkflowVerDefinitionDTO WorkflowVerDefinition { get; set; }
    }
}
