using KC.Service.DTO;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.DTO.Contract
{
    [Serializable, DataContract(IsReference = true)]
    public class SearchUser : EntityBaseDTO
    {
        [DataMember]
        public string UserId { get; set; }
        [DataMember]
        public string MemberId { get; set; }
        [DataMember]
        public string DisplayName { get; set; }
        [DataMember]
        public string IndustryName { get; set; }
        [DataMember]
        public string CompanyAddress { get; set; }
        [DataMember]
        public string ContactPhone { get; set; }
        [DataMember]
        public string ContactName { get; set; }
    }
}
