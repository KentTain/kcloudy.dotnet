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
    /// 推送对象与接口接收对象的字段映射
    /// </summary>
    [Table(Tables.PushMapping)]
    [Serializable, DataContract(IsReference = true)]
    public class PushMapping : EntityBase
    {
        [Key]
        [DataMember]
        public int MappingId { get; set; }
        /// <summary>
        /// 源对象字段名称
        /// </summary>
        [DataMember]
        public string SourceObjectField { get; set; }
        /// <summary>
        /// 源对象字段类型
        /// </summary>
        [DataMember]
        public string SourceObjectType { get; set; }
        /// <summary>
        /// 目标对象字段名称
        /// </summary>
        [DataMember]
        public string TargetObjectField { get; set; }
        /// <summary>
        /// 目标对象字段类型
        /// </summary>
        [DataMember]
        public string TargetObjectType { get; set; }
        /// <summary>
        /// 推送对象接口设置Id
        /// </summary>
        [DataMember]
        public int TargetSettingId { get; set; }
        [DataMember]
        [ForeignKey("TargetSettingId")]
        public AppTargetApiSetting TargetSetting { get; set; }
    }
}
