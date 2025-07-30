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
    public class SystemSettingPropertyDTO : PropertyAttributeBaseDTO
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

        /// <summary>
        /// 设置名称
        /// </summary>
        [DataMember]
        [ProtoMember(4)]
        public string SystemSettingName { get; set; }
    }
}
