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
    [Table(Tables.UserSetting)]
    [Serializable, DataContract(IsReference = true), ProtoContract]
    public class UserSetting : PropertyBase<UserSettingProperty>
    {
        /// <summary>
        /// 设置系统编号（SequenceName--UserSetting：USC2018120100001）
        /// </summary>
        [MaxLength(20)]
        [Display(Name = "设置编号")]
        [ProtoMember(1)]
        public string Code { get; set; }

        #region 系统设置相关
        [DataMember]
        [ProtoMember(2)]
        public bool IsSystemSetting { get; set; }
        /// <summary>
        /// 设置编码
        /// </summary>
        [DataMember]
        [ProtoMember(3)]
        public int? SystemSettingId { get; set; }

        [MaxLength(20)]
        [Display(Name = "系统设置编号")]
        [ProtoMember(4)]
        public string SystemSettingCode { get; set; }
        /// <summary>
        /// 应用程序Id
        /// </summary>
        [DataMember]
        [ProtoMember(5)]
        [Display(Name = "应用程序Id")]
        public Guid? ApplicationId { get; set; }
        /// <summary>
        /// 应用程序名称
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        [ProtoMember(6)]
        [Display(Name = "应用程序名称")]
        public string ApplicationName { get; set; }
        #endregion

        /// <summary>
        /// 用户UserId
        /// </summary>
        [DataMember]
        [ProtoMember(7)]
        public string UserId { get; set; }
        [DataMember]
        [ProtoMember(8)]
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
