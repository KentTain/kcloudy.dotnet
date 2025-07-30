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
    [Table(Tables.AppSettingProperty)]
    [Serializable, DataContract(IsReference = true), ProtoContract]
    public class AppSettingProperty : PropertyAttributeBase
    {
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
        [ForeignKey("AppSettingId")]
        public AppSetting AppSetting { get; set; }
    }
}
