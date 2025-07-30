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
    public class SystemSettingDTO : PropertyBaseDTO<SystemSettingPropertyDTO>
    {
        /// <summary>
        /// 设置系统编号（SequenceName--SystemSetting：SSC2018120100001）
        /// </summary>
        [MaxLength(20)]
        [Display(Name = "设置编号")]
        [ProtoMember(1)]
        public string Code { get; set; }

        /// <summary>
        /// 应用程序Id
        /// </summary>
        [DataMember]
        [ProtoMember(2)]
        [Display(Name = "应用程序Id")]
        public Guid? ApplicationId { get; set; }
        /// <summary>
        /// 应用程序名称
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        [ProtoMember(3)]
        [Display(Name = "应用程序名称")]
        public string ApplicationName { get; set; }

    }
}
