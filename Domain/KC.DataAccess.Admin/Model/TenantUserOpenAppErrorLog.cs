using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.App;
using KC.Model.Admin.Constants;
using KC.Framework.Base;
using ProtoBuf;

namespace KC.Model.Admin
{
    [Serializable, DataContract(IsReference = true), ProtoContract]
    [Table(Tables.TenantUserOpenAppErrorLog)]
    public class TenantUserOpenAppErrorLog : ProcessLogBase
    {
        public TenantUserOpenAppErrorLog()
        {
            Type = ProcessLogType.Failure;
        }
        [DataMember]
        [ProtoMember(1)]
        public string TenantDisplayName { get; set; }
        [DataMember]
        [ProtoMember(2)]
        public OpenServerType OpenServerType { get; set; }
        [DataMember]
        [ProtoMember(3)]
        public int TenantId { get; set; }
        [ForeignKey("TenantId")]
        public TenantUser TenantUser { get; set; }
    }
}
