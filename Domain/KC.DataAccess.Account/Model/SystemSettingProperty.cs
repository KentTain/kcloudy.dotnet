using KC.Framework.Base;
using KC.Model.Account.Constants;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Model.Account
{
    [Table(Tables.SystemSettingProperty)]
    [Serializable, DataContract(IsReference = true), ProtoContract]
    public class SystemSettingProperty : PropertyAttributeBase
    {
        /// <summary>
        /// 设置Id
        /// </summary>
        [DataMember]
        [ProtoMember(2)]
        [MaxLength(128)]
        public int SystemSettingId { get; set; }

        /// <summary>
        /// 设置编码
        /// </summary>
        [DataMember]
        [ProtoMember(3)]
        [MaxLength(20)]
        public string SystemSettingCode { get; set; }

        [DataMember]
        [ProtoMember(4)]
        [ForeignKey("SystemSettingId")]
        public SystemSetting SystemSetting { get; set; }
    }
}
