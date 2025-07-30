using KC.Framework.Base;
using KC.Model.App.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Model.App
{
    /// <summary>
    /// 安全设置参数
    /// </summary>
    [Table(Tables.SecurityParameter)]
    [Serializable, DataContract(IsReference = true)]
    public class SecurityParameter : EntityBase
    {
        [Key]
        [DataMember]
        public int ParemeterId { get; set; }
        [DataMember]
        public string Key { get; set; }
        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public int TargetSettingId { get; set; }
        [DataMember]
        [ForeignKey("TargetSettingId")]
        public AppTargetApiSetting TargetSetting { get; set; }
    }
}
