using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using System.Runtime.Serialization;
using KC.Service.DTO;

namespace KC.Service.Workflow.DTO
{
    /// <summary>
    /// 流程条件的规则
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class WorkflowVerDefNodeRuleDTO : RuleEntityBaseDTO
    {
        [MaxLength(20)]
        [DataMember]
        public string WorkflowVerNodeCode { get; set; }
        [DataMember]
        public Guid WorkflowVerNodeId { get; set; }

        [DataMember]
        public WorkflowVerDefNodeDTO WorkflowVerNode { get; set; }
    }
}
