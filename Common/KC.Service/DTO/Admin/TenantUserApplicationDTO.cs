using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Service.DTO;
using ProtoBuf;
using KC.Framework.Tenant;
using KC.Service.Enums.Admin;

namespace KC.Service.DTO.Admin
{
    [Serializable]
    [DataContract(IsReference = true)]
    [ProtoContract]
    public class TenantUserApplicationDTO : EntityBaseDTO
    {
        [DataMember]
        [ProtoMember(1)]
        public int Id { get; set; }
        [DataMember]
        [ProtoMember(2)]
        public Guid ApplicationId { get; set; }
        [DataMember]
        [ProtoMember(3)]
        public string ApplicationName { get; set; }
        [DataMember]
        [ProtoMember(4)]
        public string DomainName { get; set; }
        [DataMember]
        [ProtoMember(5)]
        public ApplicationStatus AppStatus { get; set; }
        [DataMember]
        [ProtoMember(6)]
        public string AppStatusName { get; set; }

        [DataMember]
        [ProtoMember(7)]
        public string WebSiteName { get; set; }
        [DataMember]
        [ProtoMember(8)]
        public int TenantId { get; set; }
        [DataMember]
        [ProtoMember(9)]
        public TenantUserDTO TenantUser { get; set; }
        //[DataMember]
        //public virtual ICollection<TenantUserAppModuleDTO> ApplicationModules { get; set; }
        //[DataMember]
        //public virtual ICollection<TenantUserOperationLogDTO> TenantUserOperationLog { get; set; }
        [DataMember]
        public TenantVersion Version { get; set; }


        [DataMember]
        [ProtoMember(11)]
        public string DomainIp { get; set; }
    }
}
