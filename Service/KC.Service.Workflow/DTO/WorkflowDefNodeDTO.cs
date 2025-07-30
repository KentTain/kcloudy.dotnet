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
    public class WorkflowDefNodeDTO : DefNodeBaseDTO<WorkflowDefNodeRuleDTO>
    {
        [DataMember]
        public Guid? PrevNodeId { get; set; }
        [DataMember]
        public WorkflowDefNodeDTO PrevNode { get; set; }
        [DataMember]
        public Guid? NextNodeId { get; set; }
        [DataMember]
        public WorkflowDefNodeDTO NextNode { get; set; }
        [DataMember]
        public Guid? ReturnNodeId { get; set; }
        [DataMember]
        public WorkflowDefNodeDTO ReturnNode { get; set; }

        [DataMember]
        public Guid WorkflowDefId { get; set; }
        [DataMember]
        public WorkflowDefinitionDTO WorkflowDefinition { get; set; }
    }
}
