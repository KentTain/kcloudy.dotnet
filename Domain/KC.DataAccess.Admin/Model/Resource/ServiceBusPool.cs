using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using KC.Enums.App;
using KC.Framework.Tenant;
using KC.Model.Admin.Constants;
using KC.Framework.Base;
using KC.Framework;
using ProtoBuf;

namespace KC.Model.Admin
{
    [Serializable, DataContract(IsReference = true), ProtoContract]
    [Table(Tables.ServiceBusPool)]
    public class ServiceBusPool : Entity
    {
        public ServiceBusPool()
        {
            CanEdit = true;
        }

        [Key]
        [DataMember]
        [ProtoMember(1)]
        public int ServiceBusPoolId { get; set; }
        [DataMember]
        [ProtoMember(2)]
        public string Endpoint { get; set; }
        [DataMember]
        [ProtoMember(3)]
        public string AccessName { get; set; }
        [DataMember]
        [ProtoMember(4)]
        public string AccessKeyPasswordHash { get; set; }
        [DataMember]
        [ProtoMember(5)]
        public DateTime? PasswordExpiredTime { get; set; }
        [DataMember]
        [ProtoMember(6)]
        public int TenantCount { get; set; }
        [DataMember]
        [ProtoMember(7)]
        public bool CanEdit { get; set; }
        [DataMember]
        [ProtoMember(8)]
        public CloudType CloudType { get; set; }
        [DataMember]
        [ProtoMember(9)]
        public ServiceBusType ServiceBusType { get; set; }
        [DataMember]
        [ProtoMember(10)]
        public string Extend1 { get; set; }
        [DataMember]
        [ProtoMember(11)]
        public string Extend2 { get; set; }
        [DataMember]
        [ProtoMember(12)]
        public string Extend3 { get; set; }
    }
}
