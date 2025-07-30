using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Service.DTO.Dict;
using ProtoBuf;

namespace KC.Web.Portal.Models
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class FiltrationAttributeDTO
    {

        public FiltrationAttributeDTO()
        {
            Attributes = new List<JsonDictionaryDTO>();
        }

        [DataMember]
        public string DisplayName { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public List<JsonDictionaryDTO> Attributes { get; set; }
    }
}
