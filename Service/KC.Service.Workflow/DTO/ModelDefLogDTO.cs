using KC.Service.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Workflow.DTO
{

    public class ModelDefLogDTO : ProcessLogBaseDTO
    {
        public int ModelDefId { get; set; }

        /// <summary>
        /// 模型名称
        /// </summary>
        [MaxLength(256)]
        public string ModelDefName { get; set; }
    }
}
