
using KC.Framework.Base;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KC.Service.DTO.Workflow;

namespace KC.Service.Workflow.DTO
{
    /// <summary>
    /// 流程版本
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class WorkflowVerDefinitionDTO : DefinitionBaseDTO
    {
        public WorkflowVerDefinitionDTO()
        {
            WorkflowVerNodes = new List<WorkflowVerDefNodeDTO>();
            WorkflowVerFields = new List<WorkflowVerDefFieldDTO>();
        }

        [DataMember]
        public Guid WorkflowVerDefId { get; set; }

        [DataMember]
        public WorkflowDefinitionDTO WorkflowDefinition { get; set; }
        [DataMember]
        public List<WorkflowVerDefNodeDTO> WorkflowVerNodes { get; set; }
        [DataMember]
        public List<WorkflowVerDefFieldDTO> WorkflowVerFields { get; set; }

    }
}
