using KC.Service.DTO;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.DTO.App
{
    /// <summary>
    /// 安全设置参数
    /// </summary>
    [Serializable, DataContract(IsReference = true)]
    public class SecurityParameterDTO : EntityBaseDTO
    {
        [DataMember]
        public int ParemeterId { get; set; }
        [DataMember]
        public string Key { get; set; }
        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public int TargetSettingId { get; set; }
        [DataMember]
        public AppTargetApiSettingDTO TargetSetting { get; set; }
    }
}
