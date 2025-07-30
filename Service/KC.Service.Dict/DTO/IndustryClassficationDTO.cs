using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using KC.Service.DTO;
//using KC.Service.DTO.CreditLevel;
//using KC.Service.DTO.CreditLevel.Target;
using ProtoBuf;

namespace KC.Service.DTO.Dict
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class IndustryClassficationDTO : TreeNodeDTO<IndustryClassficationDTO>
    {
        public IndustryClassficationDTO()
        {
            //IndustryStandard = new List<IndustryStandardDTO>();
        }

        [DataMember]
        [DefaultValue(true)]
        public bool IsValid { get; set; }
        [DataMember]
        public string ParentName { get; set; }

        //public List<IndustryStandardDTO> IndustryStandard { get; set; }
    }
}
