using System;
using System.Runtime.Serialization;
using System.Xml.Linq;
using ProtoBuf;

namespace KC.Service.Base
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract]
    public class EmailConfig : ConfigBase
    {
        public EmailConfig()
        {
            ConfigType = Framework.Base.ConfigType.EmailConfig;
            EnableSsl = true;
            EnablePwdCheck = true;
            EnableMail = true;
        }

        /// <summary>
        /// 注册时是否需要验证邮箱
        /// </summary>
        [ProtoMember(1)]
        [DataMember]
        public bool RequireValid { get; set; }
        /// <summary>
        /// SMTP服务器
        /// </summary>
        [ProtoMember(2)]
        [DataMember]
        public string Server { get; set; }
        /// <summary>
        /// 默认端口25（设为-1让系统自动设置）
        /// </summary>
        [ProtoMember(3)]
        [DataMember]
        public int Port { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        [ProtoMember(4)]
        [DataMember]
        public string Account { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [ProtoMember(5)]
        [DataMember]
        public string Password { get; set; }
        /// <summary>
        /// 是否使用SSL连接
        /// </summary>
        [ProtoMember(6)]
        [DataMember]
        public bool EnableSsl { get; set; }
        /// <summary>
        /// 是否验证密码
        /// </summary>
        [ProtoMember(7)]
        [DataMember]
        public bool EnablePwdCheck { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(8)]
        [DataMember]
        public bool EnableMail { get; set; }
        /// <summary>
        /// 企业签名模板
        /// </summary>
        [ProtoMember(9)]
        [DataMember]
        public string CompanySign { get; set; }

        /// <summary>
        /// 电子邮件确认链接有效时间(分钟)
        /// </summary>
        [ProtoMember(10)]
        [DataMember]
        public int EffectiveMinuteCount { get; set; }

        public override string GetConfigObjectXml()
        {
            var doc = GetDocumentRoot();

            SerializeToXml(doc.Root);
            return doc.ToString(SaveOptions.DisableFormatting);
        }

        protected new XElement SerializeToXml(XElement element)
        {
            base.SerializeToXml(element);

            element.Add(new XElement("RequireValid", this.RequireValid));
            element.Add(new XElement("Server", this.Server));
            element.Add(new XElement("Port", this.Port));
            element.Add(new XElement("Account", this.Account));
            element.Add(new XElement("Password", this.Password));
            element.Add(new XElement("EnableSsl", this.EnableSsl));
            element.Add(new XElement("EnableMail", this.EnableMail));
            element.Add(new XElement("EnablePwdCheck", this.EnablePwdCheck));
            element.Add(new XElement("EffectiveMinuteCount", this.EffectiveMinuteCount));

            return element;
        }

        public EmailConfig(string xml)
            : base(xml)
        {
            try
            {
                var element = base.GetRootElement(xml);

                RequireValid = element.Element("RequireValid") != null
                    ? bool.Parse(element.Element("RequireValid").Value) 
                    : false;

                Server = element.Element("Server") != null
                    ? element.Element("Server").Value
                    : string.Empty;

                Port = element.Element("Port") != null
                    ? int.Parse(element.Element("Port").Value)
                    : 25;

                Account = element.Element("Account") != null
                    ? element.Element("Account").Value
                    : string.Empty;

                Password = element.Element("Password") != null
                    ? element.Element("Password").Value
                    : string.Empty;

                EnableSsl = element.Element("EnableSsl") != null
                    ? bool.Parse(element.Element("EnableSsl").Value)
                    : false;

                EnableMail = element.Element("EnableMail") != null
                    ? bool.Parse(element.Element("EnableMail").Value)
                    : false;

                EnablePwdCheck = element.Element("EnablePwdCheck") != null
                    ? bool.Parse(element.Element("EnablePwdCheck").Value)
                    : false;

                EffectiveMinuteCount = element.Element("EffectiveMinuteCount") != null
                    ? int.Parse(element.Element("EffectiveMinuteCount").Value)
                    : 30;
            }
            catch (Exception)
            {
                throw new Exception("Invalid MailConfig Definition");
            }
        }
    }
}
