using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Model.Workflow.Constants;

namespace KC.Model.Workflow
{
    [Table(Tables.WorkflowDefLog)]
    public class WorkflowDefLog : ProcessLogBase
    {
        public Guid WorkflowDefId { get; set; }
        /// <summary>
        /// 流程编码：wfd2012020200001
        /// </summary>
        [MaxLength(20)]
        public string Code { get; set; }

        /// <summary>
        /// 流程版本：wfv2012020200001
        /// </summary>
        [MaxLength(20)]
        public string Version { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        [MaxLength(512)]
        public string Name { get; set; }
    }
}
