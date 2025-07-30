using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using System.Runtime.Serialization;

namespace KC.Service.DTO.Workflow
{
    /// <summary>
    /// 流程条件的规则
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class WorkflowProTaskRuleDTO : RuleEntityBaseDTO
    {
        [MaxLength(20)]
        [DataMember]
        public string TaskCode { get; set; }
        [DataMember]
        public Guid TaskId { get; set; }

        [DataMember]
        public WorkflowProTaskDTO Task { get; set; }
    }
}
