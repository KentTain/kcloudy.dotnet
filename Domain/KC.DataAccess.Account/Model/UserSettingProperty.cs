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
    [Table(Tables.UserSettingProperty)]
    [Serializable, DataContract(IsReference = true), ProtoContract]
    public class UserSettingProperty : PropertyAttributeBase
    {
        /// <summary>
        /// 系统设置属性Id
        /// </summary>
        [DataMember]
        [ProtoMember(1)]
        public int? SystemSettingPropertyAttrId { get; set; }

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
        [ForeignKey("UserSettingId")]
        public UserSetting UserSetting { get; set; }
    }
}
