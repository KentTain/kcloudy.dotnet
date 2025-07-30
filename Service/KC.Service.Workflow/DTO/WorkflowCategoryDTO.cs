using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Service.DTO;
using System.Runtime.Serialization;

namespace KC.Service.Workflow.DTO
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class WorkflowCategoryDTO : TreeNodeDTO<WorkflowCategoryDTO>
    {
        public WorkflowCategoryDTO()
        {
            Definitions = new List<WorkflowDefinitionDTO>();
        }

        [DataMember]
        public bool IsEditMode { get; set; }

        [DataMember]
        public string ParentName { get; set; }

        [MaxLength(4000)]
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public List<WorkflowDefinitionDTO> Definitions { get; set; }
    }
}
