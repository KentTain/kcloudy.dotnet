using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using KC.Service.DTO;
using ProtoBuf;

namespace KC.Service.DTO.Supplier
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class CityDTO : EntityBaseDTO
    {
        [DataMember]
        public int Id { get; set; }

        [MaxLength(512)]
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int ProvinceId { get; set; }
        [DataMember]
        public string ProvinceName { get; set; }
    }
}
