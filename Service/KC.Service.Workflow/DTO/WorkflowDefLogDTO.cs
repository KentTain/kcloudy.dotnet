using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Service.DTO;

namespace KC.Service.Workflow.DTO
{
    [Serializable, DataContract(IsReference = true)]
    public class WorkflowDefLogDTO : ProcessLogBaseDTO
    {
        [DataMember]
        public Guid WorkflowDefId { get; set; }

        /// <summary>
        /// 流程编码：wfd2012020200001
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        /// 流程版本：wfv2012020200001
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string Version { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        [MaxLength(512)]
        [DataMember]
        public string Name { get; set; }
    }
}
