using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Linq;
using ProtoBuf;

namespace KC.Service.Base
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class WeixinConfig : ConfigBase
    {
        public WeixinConfig()
        {
            ConfigType = Framework.Base.ConfigType.WeixinConfig;
        }
        /// <summary>
        /// 微信公众平台AppId
        /// </summary>
        [ProtoMember(1)]
        [DataMember]
        public string AppId { get; set; }
        /// <summary>
        /// 微信公众平台AppSecret
        /// </summary>
        [ProtoMember(2)]
        [DataMember]
        public string AppSecret { get; set; }
        /// <summary>
        /// 微信公众平台验证服务器的Token
        /// </summary>
        [ProtoMember(3)]
        [DataMember]
        public string AppToken { get; set; }
        /// <summary>
        /// 消息加密秘钥
        /// </summary>
        [ProtoMember(4)]
        [DataMember]
        public string EncodingAESKey { get; set; }
        /// <summary>
        /// 消息加密方式
        /// </summary>
        [ProtoMember(5)]
        [DataMember]
        public WeiXinMessageMode WeiXinMessageMode { get; set; }

        [ProtoMember(6)]
        [DataMember]
        public string Value1 { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public string Value2 { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public string Value3 { get; set; }

        public override string GetConfigObjectXml()
        {
            var doc = GetDocumentRoot();
            SerializeToXml(doc.Root);
            return doc.ToString(SaveOptions.DisableFormatting);
        }

        protected new XElement SerializeToXml(XElement element)
        {
            base.SerializeToXml(element);
            element.Add(new XElement("AppId", this.AppId));
            element.Add(new XElement("AppSecret", this.AppSecret));
            element.Add(new XElement("AppToken", this.AppToken));
            element.Add(new XElement("EncodingAESKey", this.EncodingAESKey));
            element.Add(new XElement("WeiXinMessageMode", this.WeiXinMessageMode.ToString()));
            return element;
        }

        public WeixinConfig(string xml)
            : base(xml)
        {
            try
            {
                var element = base.GetRootElement(xml);
                AppId = element.Element("AppId") != null
                    ? element.Element("AppId").Value
                    : string.Empty;
                AppSecret = element.Element("AppSecret") != null
                    ? element.Element("AppSecret").Value
                    : string.Empty;
                AppToken = element.Element("AppToken") != null
                    ? element.Element("AppToken").Value
                    : string.Empty;
                EncodingAESKey = element.Element("EncodingAESKey") != null
                    ? element.Element("EncodingAESKey").Value
                    : string.Empty;
                WeiXinMessageMode = element.Element("WeiXinMessageMode") != null
                    ? (WeiXinMessageMode)Enum.Parse(typeof(WeiXinMessageMode), element.Element("WeiXinMessageMode").Value, false)
                    : WeiXinMessageMode.Normal;
            }
            catch (Exception)
            {
                throw new Exception("Invalid WeixinConfig Definition");
            }
        }
    }

    public enum WeiXinMessageMode
    {

        [Display(Name = "明文模式")]
        [Description("明文模式")]
        Normal = 0,
        [Display(Name = "安全模式")]
        [Description("安全模式")]
        Encryption = 2,
    }
}
