using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Service.DTO;
using ProtoBuf;

namespace KC.Service.DTO.Admin
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract]
    public class TenantUserLoopTaskDTO : EntityDTO
    {
        [DataMember]
        [ProtoMember(1)]
        public int Id { get; set; }

        [DataMember]
        [ProtoMember(2)]
        public string BusinessId { get; set; }

        [DataMember]
        [ProtoMember(3)]
        public string Status { get; set; }

        [DataMember]
        [ProtoMember(4)]
        public string Type { get; set; }

        [DataMember]
        [ProtoMember(5)]
        public string TenantName { get; set; }
    }
}
