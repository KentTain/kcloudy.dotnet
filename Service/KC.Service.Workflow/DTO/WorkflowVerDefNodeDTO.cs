using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Service.DTO.Workflow;

namespace KC.Service.Workflow.DTO
{
    /// <summary>
    /// 流程节点步骤
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class WorkflowVerDefNodeDTO : DefNodeBaseDTO<WorkflowVerDefNodeRuleDTO>
    {
        public WorkflowVerDefNodeDTO()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// 流程使用版本：wfv2012020200001
        /// </summary>
        [DataMember]
        public Guid WorkflowVerDefId { get; set; }

        [DataMember]
        public WorkflowVerDefinitionDTO WorkflowVerDefinition { get; set; }
    }
}
