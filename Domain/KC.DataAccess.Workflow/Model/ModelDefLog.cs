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
    [Table(Tables.ModelDefLog)]
    public class ModelDefLog : ProcessLogBase
    {
        public int ModelDefId { get; set; }

        /// <summary>
        /// 模型名称
        /// </summary>
        [MaxLength(256)]
        public string ModelDefName { get; set; }
    }
}
