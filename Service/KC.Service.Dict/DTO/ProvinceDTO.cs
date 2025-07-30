using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using KC.Service.DTO;
using ProtoBuf;

namespace KC.Service.DTO.Dict
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class ProvinceDTO: EntityBaseDTO
    {
        public ProvinceDTO()
        {
            Cities = new List<CityDTO>();
        }
        [DataMember]
        public int ProvinceId { get; set; }
        [DataMember]
        [MaxLength(512)]
        public string Name { get; set; }
        [DataMember]
        public virtual List<CityDTO> Cities { get; set; }
    }
}
