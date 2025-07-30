using KC.Framework.Base;
using KC.Model.App.Constants;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Model.App
{
    [Table(Tables.AppSetting)]
    [Serializable, DataContract(IsReference = true), ProtoContract]
    public class AppSetting : PropertyBase<AppSettingProperty>
    {
        /// <summary>
        /// 应用设置编号（SequenceName--AppSetting：ASC2018120100001）
        /// </summary>
        [MaxLength(20)]
        [Display(Name = "设置编号")]
        [ProtoMember(1)]
        public string Code { get; set; }

        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        [ProtoMember(7)]
        public Guid ApplicationId { get; set; }

        [DataMember]
        [ProtoMember(8)]
        [ForeignKey("ApplicationId")]
        public Application Application { get; set; }
    }
}
