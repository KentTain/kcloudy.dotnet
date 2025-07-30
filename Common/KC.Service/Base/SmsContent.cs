using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.Base
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class SmsContent
    {
        [DataMember]
        public string PhoneNumber { get; set; }
        [DataMember]
        public string PhoneCode { get; set; }
        [DataMember]
        public string CallBackSessionId { get; set; }
        [DataMember]
        public DateTime ExpiredDateTime { get; set; }
    }
}
