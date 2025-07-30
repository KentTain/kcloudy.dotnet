using KC.Service.DTO;
using KC.Framework.Tenant;
using ProtoBuf;
using System;
using System.Runtime.Serialization;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace KC.Service.DTO.Account
{
    [Serializable, DataContract(IsReference = true), ProtoContract]
    public class UserSettingPropertyDTO : PropertyAttributeBaseDTO
    {
        /// <summary>
        /// 设置Id
        /// </summary>
        [DataMember]
        [ProtoMember(2)]
        public int UserSettingId { get; set; }

        /// <summary>
        /// 设置编码
        /// </summary>
        [DataMember]
        [ProtoMember(3)]
        [MaxLength(20)]
        public int UserSettingCode { get; set; }

        [DataMember]
        [ProtoMember(4)]
        public string UserSettingName { get; set; }

        [DataMember]
        [ProtoMember(5)]
        public string IsSystemSetting { get; set; }

        /// <summary>
        /// 系统设置属性Id
        /// </summary>
        [DataMember]
        [ProtoMember(1)]
        public int? SystemSettingPropertyAttrId { get; set; }
    }
}
