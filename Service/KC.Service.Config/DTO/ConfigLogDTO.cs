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
using KC.Model.Config.Constants;

namespace KC.Service.DTO.Config
{
    [Serializable, DataContract(IsReference = true)]
    public class ConfigLogDTO : ProcessLogBaseDTO
    {
        [DataMember]
        public int ConfigId { get; set; }

        /// <summary>
        /// 模型名称
        /// </summary>
        [MaxLength(256)]
        [DataMember]
        public string ConfigName { get; set; }
    }
}
