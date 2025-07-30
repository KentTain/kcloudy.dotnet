using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using KC.Framework.Base;
using KC.Component.Base;

namespace KC.Model.Component.Queue
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class SmsInfo : QueueEntity
    {
        public SmsInfo()
        {
            Phone = new List<long>();
        }
        [DataMember]
        public SmsType Type { get; set; }
        [DataMember]
        public List<long> Phone { get; set; }
        [DataMember]
        public string SmsCode { get; set; }
        [DataMember]
        public string SmsContent { get; set; }

        public override string ErrorMessage
        {
            get
            {
                var sbError = new StringBuilder();
                sbError.AppendLine("发送短信出错，请重新手动发送，短信信息如下：");
                if (Phone.Any())
                    sbError.AppendLine("手机号码:" + string.Join(",",Phone) + ". ");
                sbError.AppendLine("短信内容:" + SmsContent + ". ");
                return sbError.ToString();
            }
        }

        public override string GetQueueObjectXml()
        {
            var doc = GetDocumentRoot();

            SerializeToXml(doc.Root);
            return doc.ToString(SaveOptions.DisableFormatting);
        }

        protected override XElement SerializeToXml(XElement element)
        {
            element.Add(new XElement("SmsContent", this.SmsContent));
            element.Add(new XElement("ErrorMessage", this.ErrorMessage));
            var pXelement = new XElement("Phones");
            foreach (var p in Phone)
            {
                pXelement.Add(new XElement("Phone", p));
            }
            element.Add(pXelement);

            base.SerializeToXml(element);
            return element;
        }

        public SmsInfo(string xml)
            : base(xml)
        {
            try
            {
                XDocument formXmlDocument = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
                var element = formXmlDocument.Root;
                if (element == null || element.Name != "QueueEntity")
                    throw new ArgumentException("Invalid QueueEntity Definition");

                SmsContent = element.Element("SmsContent") != null
                    ? element.Element("SmsContent").Value
                    : string.Empty;

                var phones = element.Element("Phones");
                if (phones != null)
                {
                    foreach (var phone in phones.Elements("Phone"))
                    {
                        Phone.Add(phone != null ? long.Parse(phone.Value) : 0);
                    }
                }
                
                //ErrorMessage = element.Element("ErrorMessage") != null
                //    ? element.Element("ErrorMessage").Value
                //    : string.Empty;
            }
            catch (Exception)
            {
                throw new Exception("Invalid QueueEntity Definition");
            }
        }
    }
}
