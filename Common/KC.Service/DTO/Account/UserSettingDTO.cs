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
    public class UserSettingDTO : PropertyBaseDTO<UserSettingPropertyDTO>
    {
        /// <summary>
        /// 设置系统编号（SequenceName--UserSetting：USC2018120100001）
        /// </summary>
        [MaxLength(20)]
        [Display(Name = "设置编号")]
        [ProtoMember(1)]
        public string Code { get; set; }

        /// <summary>
        /// 用户UserId
        /// </summary>
        [DataMember]
        [ProtoMember(2)]
        [MaxLength(128)]
        public string UserId { get; set; }

        [DataMember]
        [ProtoMember(3)]
        [MaxLength(20)]
        public string UserCode { get; set; }

        [DataMember]
        [ProtoMember(4)]
        [MaxLength(512)]
        public string UserDisplayName { get; set; }

        #region 系统设置相关
        [DataMember]
        [ProtoMember(5)]
        public bool IsSystemSetting { get; set; }
        /// <summary>
        /// 设置编码
        /// </summary>
        [DataMember]
        [ProtoMember(6)]
        public int? SystemSettingId { get; set; }

        [Display(Name = "系统设置编号")]
        [ProtoMember(7)]
        [MaxLength(20)]
        public string SystemSettingCode { get; set; }

        /// <summary>
        /// 应用程序Id
        /// </summary>
        [DataMember]
        [ProtoMember(8)]
        [Display(Name = "应用程序Id")]
        public Guid? ApplicationId { get; set; }
        /// <summary>
        /// 应用程序名称
        /// </summary>
        [DataMember]
        [ProtoMember(9)]
        [Display(Name = "应用程序名称")]
        [MaxLength(128)]
        public string ApplicationName { get; set; }
        #endregion
    }
}
