using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.DTO.Message
{
    [Serializable, DataContract(IsReference = true)]
    public class SendUserDTO
    {
        [DataMember]
        public string UserId { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string DisplayName { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string PhoneNumber { get; set; }
        
    }
}
