using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using KC.Framework.Extension;
using KC.Service.DTO;
using ProtoBuf;
using KC.Framework.Base;

namespace KC.Service.DTO.Training
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class ConfigEntityDTO : EntityDTO
    {
        public ConfigEntityDTO()
        {
            ConfigType = Framework.Base.ConfigType.UNKNOWN;
            ConfigAttributes = new List<ConfigAttributeDTO>();
        }

        [DataMember]
        public bool IsEditMode { get; set; }

        [DataMember]
        public int ConfigId { get; set; }

        [DataMember]
        public Framework.Base.ConfigType ConfigType { get; set; }

        /// <summary>
        /// 配置标记
        /// </summary>
        [DataMember]
        public int ConfigSign { get; set; }

        [DataMember]
        public string ConfigTypeStr
        {
            get { return ConfigType.ToDescription(); }
        }

        [DataMember]
        [MaxLength(50)]
        public string ConfigName { get; set; }

        [DataMember]
        [MaxLength(4000)]
        public string ConfigDescription { get; set; }

        [DataMember]
        public string ConfigXml { get; set; }

        [DataMember]
        public string ConfigImgUrl { get; set; }

        [DataMember]
        public ConfigStatus State { get; set; }

        [DataMember]
        public string StateStr
        {
            get { return State.ToDescription(); }
        }

        [DataMember]
        public string ConfigCode { get; set; }

        [DataMember]
        public List<ConfigAttributeDTO> ConfigAttributes { get; set; }
    }
}
