using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.DTO.App
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class AppSettingDTO : PropertyBaseDTO<AppSettingPropertyDTO>
    {
        [DataMember]
        [ProtoMember(1)]
        public bool IsEditMode { get; set; }

        /// <summary>
        /// 应用设置编号（SequenceName--AppSetting：ASC2018120100001）
        /// </summary>
        [MaxLength(20)]
        [Display(Name = "设置编号")]
        [DataMember]
        [ProtoMember(2)]
        public string Code { get; set; }

        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        [ProtoMember(3)]
        public Guid ApplicationId { get; set; }

        [DataMember]
        [ProtoMember(4)]
        public string ApplicationName { get; set; }
    }
}
