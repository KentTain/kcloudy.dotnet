using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;

namespace KC.Service.DTO.Workflow
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class WorkflowProFieldDTO : DefFieldBaseDTO<WorkflowProFieldDTO>
    {

        [DataMember]
        public Guid ProcessId { get; set; }
        [DataMember]
        public WorkflowProcessDTO Process { get; set; }
    }
}
