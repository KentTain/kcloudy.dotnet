using KC.Framework.Base;
using KC.Model.Admin;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Model.Admin
{
    [Serializable, DataContract(IsReference = true), ProtoContract]
    public class TenantUserSetting : PropertyAttributeBase
    {
        /// <summary>
        /// 租户Id
        /// </summary>
        [DataMember]
        [ProtoMember(1)]
        public int TenantId { get; set; }

        [DataMember]
        [ProtoMember(2)]
        //[ForeignKey("TenantId")]
        public TenantUser TenantUser { get; set; }
    }
}
