using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using KC.Framework.Extension;
using KC.Common;
using KC.Service.DTO;
using KC.Service.DTO.Workflow;

namespace KC.Service.Workflow.DTO
{
    /// <summary>
    /// 流程定义
    /// https://www.cnblogs.com/Relict/articles/2294642.html
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class WorkflowDefinitionDTO : DefinitionBaseDTO
    {
        public WorkflowDefinitionDTO()
        {
            WorkflowVersions = new List<WorkflowVerDefinitionDTO>();
            WorkflowNodes = new List<WorkflowDefNodeDTO>();
            WorkflowFields = new List<WorkflowDefFieldDTO>();
        }

        [DataMember]
        public bool IsEditMode { get; set; }

        [DataMember]
        public WorkflowStartExecuteData ExecuteData { get; set; }

        [DataMember]
        public List<WorkflowDefNodeDTO> WorkflowNodes { get; set; }
        [DataMember]
        public List<WorkflowDefFieldDTO> WorkflowFields { get; set; }
        [DataMember]
        public List<WorkflowVerDefinitionDTO> WorkflowVersions { get; set; }

    }
}
