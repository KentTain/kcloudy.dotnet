using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Service.DTO;
using ProtoBuf;

namespace KC.Service.DTO.Dict
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class JsonDictionaryDTO : EntityBaseDTO
    {
        [DataMember]
        public string Key { get; set; }
        [DataMember]
        public string Value { get; set; }
    }


    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class JsonDictionarysDTO : EntityBaseDTO
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Key { get; set; }
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public string Value1 { get; set; }
    }
}
