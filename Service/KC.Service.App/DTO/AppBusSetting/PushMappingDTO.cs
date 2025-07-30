
using KC.Service.DTO;
using ProtoBuf;
using System;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.DTO.App
{
    /// <summary>
    /// 推送对象与接口接收对象的字段映射
    /// </summary>
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class PushMappingDTO : EntityBaseDTO
    {
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
        public AppTargetApiSettingDTO TargetSetting { get; set; }
    }
}
