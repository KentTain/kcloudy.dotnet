using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using KC.Component.Base;

namespace KC.Model.Component.Queue
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class UncallEntity : QueueEntity
    {
        public UncallEntity()
        {
        }

        [DataMember]
        public string SessionId { get; set; }
        [DataMember]
        public string Caller { get; set; }
        [DataMember]
        public string CallerName { get; set; }
        [DataMember]
        public string CallerExt { get; set; }
        [DataMember]
        public string Becaller { get; set; }
        [DataMember]
        public string BecallerName { get; set; }

        public override string ErrorMessage
        {
            get
            {
                var sbError = new StringBuilder();
                sbError.AppendLine(string.Format("解析呼叫中心服务提供商（长鑫盛通）产生的Job出错，Job内容如下："));
                sbError.AppendLine("SessionId: " + SessionId + ". ");
                sbError.AppendLine("主叫人: " + CallerName + ". ");
                sbError.AppendLine("主叫电话: " + Caller + ". ");
                sbError.AppendLine("主叫电话的分机号: " + CallerExt + ". ");
                sbError.AppendLine("被叫人: " + BecallerName + ". ");
                sbError.AppendLine("被叫电话: " + Becaller + ". ");
                

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
            element.Add(new XElement("SessionId", this.SessionId));
            element.Add(new XElement("Caller", this.Caller));
            element.Add(new XElement("CallerName", this.CallerName));
            element.Add(new XElement("CallerExt", this.CallerExt));
            element.Add(new XElement("Becaller", this.Becaller));
            element.Add(new XElement("BecallerName", this.BecallerName));
            element.Add(new XElement("ErrorMessage", this.ErrorMessage));

            base.SerializeToXml(element);
            return element;
        }

        public UncallEntity(string xml)
            : base(xml)
        {
            try
            {
                XDocument formXmlDocument = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
                var element = formXmlDocument.Root;
                if (element == null || element.Name != "QueueEntity")
                    throw new ArgumentException("Invalid QueueEntity Definition");

                SessionId = element.Element("SessionId") != null
                    ? element.Element("SessionId").Value
                    : string.Empty;
                Caller = element.Element("Caller") != null
                    ? element.Element("Caller").Value
                    : string.Empty;
                CallerName = element.Element("CallerName") != null
                    ? element.Element("CallerName").Value
                    : string.Empty;
                CallerExt = element.Element("CallerExt") != null
                    ? element.Element("CallerExt").Value
                    : string.Empty;
                Becaller = element.Element("Becaller") != null
                    ? element.Element("Becaller").Value
                    : string.Empty;
                BecallerName = element.Element("BecallerName") != null
                    ? element.Element("BecallerName").Value
                    : string.Empty;
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
