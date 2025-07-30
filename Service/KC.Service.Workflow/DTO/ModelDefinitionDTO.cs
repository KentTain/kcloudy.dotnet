using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Service.DTO;
using ProtoBuf;

namespace KC.Service.Workflow.DTO
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class ModelDefinitionDTO : PropertyBaseDTO<ModelDefFieldDTO>
    {
        [DataMember]
        public bool IsEditMode { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        [DataMember]
        [Display(Name = "业务类型")]
        public BusinessType BusinessType { get; set; }
        [DataMember]
        public string BusinessTypeString { get { return BusinessType.ToDescription(); } }
    }
}
