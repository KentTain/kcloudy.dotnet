using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Model.Workflow.Constants;
using KC.Framework.Base;
using KC.Service.DTO;
using System.Runtime.Serialization;

namespace KC.Service.Workflow.DTO
{
    /// <summary>
    /// 流程条件的规则
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class WorkflowDefNodeRuleDTO : RuleEntityBaseDTO
    {
        [MaxLength(20)]
        [DataMember]
        public string WorkflowNodeCode { get; set; }
        [DataMember]
        public Guid WorkflowNodeId { get; set; }

        [DataMember]
        public WorkflowDefNodeDTO WorkflowNode { get; set; }
    }
}
