using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Linq;


namespace KC.Framework.Base
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class ConfigEntity : Entity
    {
        public ConfigEntity()
        {
            ConfigType = ConfigType.UNKNOWN;
            ConfigAttributes = new List<ConfigAttribute>();
        }

        /// <summary>
        /// 配置Id
        /// </summary>
        [Key]
        [DataMember]
        public int ConfigId { get; set; }
        /// <summary>
        /// 配置类型：KC.Enums.Core.ConfigType
        /// </summary>
        [DataMember]
        public ConfigType ConfigType { get; set; }
        /// <summary>
        /// 配置标记
        /// </summary>
        [DataMember]
        public int ConfigSign { get; set; }
        /// <summary>
        /// 配置名称
        /// </summary>
        [DataMember]
        [MaxLength(50)]
        public string ConfigName { get; set; }
        /// <summary>
        /// 配置描述
        /// </summary>
        [DataMember]
        [MaxLength(4000)]
        public string ConfigDescription { get; set; }
        /// <summary>
        /// 配置生成的XML
        /// </summary>
        [DataMember]
        public string ConfigXml { get; set; }
        /// <summary>
        /// 配置图片链接
        /// </summary>
        [DataMember]
        public string ConfigImgUrl { get; set; }
        /// <summary>
        /// 配置状态：KC.Enums.Core.ConfigStatus
        /// </summary>
        [DataMember]
        public ConfigStatus State { get; set; }

        [DataMember]
        [MaxLength(128)]
        public string ConfigCode { get; set; }

        [DataMember]
        public virtual ICollection<ConfigAttribute> ConfigAttributes { get; set; }

        public new string GetEnityObjectXml()
        {
            var doc = base.GetDocumentRoot();

            this.SerializeToXml(doc.Root);
            return doc.ToString(SaveOptions.DisableFormatting);
        }

        protected override XElement SerializeToXml(XElement element)
        {
            base.SerializeToXml(element);

            element.Add(new XElement("ConfigId", this.ConfigId));
            element.Add(new XElement("ConfigType", this.ConfigType));
            element.Add(new XElement("ConfigName", this.ConfigName));

            return element;
        }

        public ConfigEntity(string xml)
            : base(xml)
        {
            try
            {
                var element = base.GetRootElement(xml);

                ConfigId = element.Element("ConfigId") != null
                    ? int.Parse(element.Element("ConfigId").Value)
                    : 0;
                ConfigType = element.Element("ConfigType") != null
                    ? (ConfigType)Enum.Parse(typeof(ConfigType), element.Element("ConfigType").Value)
                    : ConfigType.UNKNOWN;
                ConfigName = element.Element("ConfigName") != null
                    ? element.Element("ConfigName").Value
                    : string.Empty;
            }
            catch (Exception)
            {
                throw new Exception("Invalid ConfigEntity Definition");
            }
        }
    }
}
