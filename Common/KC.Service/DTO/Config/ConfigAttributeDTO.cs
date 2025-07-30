using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Service.DTO;
using System.Runtime.Serialization;
using ProtoBuf;

namespace KC.Service.DTO.Config
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class ConfigAttributeDTO : EntityDTO
    {
        [DataMember]
        public int PropertyAttributeId { get; set; }

        [MaxLength(256)]
        [DataMember]
        public string DisplayName { get; set; }

        [MaxLength(1024)]
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int ConfigId { get; set; }

        [DataMember]
        public string ConfigName { get; set; }

        [DataMember]
        public Framework.Base.AttributeDataType DataType { get; set; }

        [MaxLength(256)]
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public string Value1 { get; set; }

        [DataMember]
        public string Value2 { get; set; }

        [DataMember]
        public bool IsProviderAttr { get; set; }

        //[DataMember]
        //public int Priority { get; set; }

        [DataMember]
        public int Index { get; set; }
        [DataMember]
        public bool CanEdit { get; set; }

        [DataMember]
        public bool IsFileAttr { get; set; }
    }
}
