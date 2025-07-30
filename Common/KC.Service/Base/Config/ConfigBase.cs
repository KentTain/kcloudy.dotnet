using System;
using System.Runtime.Serialization;
using System.Xml.Linq;
using KC.Framework.Base;
using ProtoBuf;

namespace KC.Service.Base
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract]
    [ProtoInclude(101, typeof(EmailConfig))]
    [ProtoInclude(102, typeof(SmsConfig))]
    [ProtoInclude(103, typeof(CallConfig))]
    [ProtoInclude(104, typeof(WeixinConfig))]
    public abstract class ConfigBase : Entity
    {
        protected ConfigBase()
        {
            ConfigType = ConfigType.UNKNOWN;
        }

        [ProtoMember(50)]
        [DataMember]
        public ConfigType ConfigType { get; set; }
        [ProtoMember(51)]
        [DataMember]
        public string TenantName { get; set; }
        [ProtoMember(52)]
        [DataMember]
        public int ConfigId { get; set; }
        [ProtoMember(53)]
        [DataMember]
        public string ConfigName { get; set; }
        [ProtoMember(54)]
        [DataMember]
        public string ConfigDescription { get; set; }

        protected ConfigBase(string xml)
            : base(xml)
        {
            try
            {
                var element = base.GetRootElement(xml);

                ConfigType = element.Element("ConfigType") != null
                    ? (ConfigType) Enum.Parse(typeof (ConfigType), element.Element("ConfigType").Value)
                    : ConfigType.UNKNOWN;

                TenantName = element.Element("TenantName") != null
                    ? element.Element("TenantName").Value
                    : string.Empty;

                ConfigId = element.Element("ConfigId") != null
                    ? int.Parse(element.Element("ConfigId").Value)
                    : 0;

                ConfigName = element.Element("ConfigName") != null
                    ? element.Element("ConfigName").Value
                    : string.Empty;

                ConfigDescription = element.Element("ConfigDescription") != null
                    ? element.Element("ConfigDescription").Value
                    : string.Empty;
            }
            catch (Exception)
            {
                throw new Exception("Invalid ConfigBase Definition");
            }
        }

        protected override XElement SerializeToXml(XElement element)
        {
            base.SerializeToXml(element);

            element.Add(new XElement("ConfigType", this.ConfigType.ToString()));
            element.Add(new XElement("TenantName", this.TenantName));
            element.Add(new XElement("ConfigId", this.ConfigId));
            element.Add(new XElement("ConfigName", this.ConfigName));
            element.Add(new XElement("ConfigDescription", this.ConfigDescription));
            return element;
        }

        public abstract string GetConfigObjectXml();
    }
}
