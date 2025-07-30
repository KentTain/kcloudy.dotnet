using KC.Framework.Base;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.DTO.App
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class AppSettingPropertyDTO : PropertyAttributeBaseDTO
    {
        [DataMember]
        [ProtoMember(1)]
        public bool IsEditMode { get; set; }
        /// <summary>
        /// 设置Id
        /// </summary>
        [DataMember]
        [ProtoMember(2)]
        public int AppSettingId { get; set; }

        /// <summary>
        /// 应用设置编码
        /// </summary>
        [DataMember]
        [ProtoMember(3)]
        [MaxLength(20)]
        public string AppSettingCode { get; set; }

        [DataMember]
        [ProtoMember(4)]
        public string AppSettingName { get; set; }
    }
}
