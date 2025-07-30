using KC.Service.DTO;
using KC.Framework.Tenant;
using ProtoBuf;
using System;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.DTO.Admin
{
    [Serializable, DataContract(IsReference = true), ProtoContract]
    public class TenantUserSettingDTO : PropertyAttributeBaseDTO
    {
        /// <summary>
        /// 租户Id
        /// </summary>
        [DataMember]
        [ProtoMember(1)]
        public int TenantId { get; set; }

        [DataMember]
        [ProtoMember(2)]
        public Tenant Tenant { get; set; }
    }
}
