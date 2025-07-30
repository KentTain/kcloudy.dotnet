using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using KC.Component.Base;

namespace KC.Model.Component.Queue
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class EmailInfo : QueueEntity
    {
        public EmailInfo()
        {
            IsBodyHtml = true;
            SendTo = new List<string>();
            CcTo = new List<string>();
        }
        [DataMember]
        public string UserId { get; set; }
        [DataMember]
        public List<string> SendTo { get; set; }
        [DataMember]
        public List<string> CcTo { get; set; }
        [DataMember]
        public string EmailTitle { get; set; }
        [DataMember]
        public string EmailBody { get; set; }
        [DataMember]
        public string SendFrom { get; set; }
        [DataMember]
        public bool IsBodyHtml { get; set; }

        public override string ErrorMessage
        {
            get
            {
                var sbError = new StringBuilder();
                sbError.AppendLine("发送邮箱出错，请重新手动发送，邮件信息如下：");
                if (SendTo.Any())
                    sbError.AppendLine("收件人:" + string.Join(",", SendTo) + ". ");
                if (CcTo.Any())
                {
                    sbError.AppendLine("抄送人:" + string.Join(",", CcTo) + ". ");
                    sbError.AppendLine("邮件标题:" + EmailTitle + ". ");
                    sbError.AppendLine("邮件内容:" + EmailBody + ". ");
                }
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
            element.Add(new XElement("UserId", this.UserId));
            element.Add(new XElement("EmailTitle", this.EmailTitle));
            element.Add(new XElement("EmailBody", this.EmailBody));
            element.Add(new XElement("IsBodyHtml", this.IsBodyHtml));
            element.Add(new XElement("ErrorMessage", this.ErrorMessage));

            var sendList = new XElement("SendList");
            foreach (var send in this.SendTo)
            {
                sendList.Add(new XElement("SendTo", send));
            }
            element.Add(sendList);

            var ccList = new XElement("CcList");
            foreach (var cc in this.CcTo)
            {
                ccList.Add(new XElement("CcTo", cc));
            }
            element.Add(ccList);

            base.SerializeToXml(element);
            return element;
        }

        public EmailInfo(string xml)
            : base(xml)
        {
            try
            {
                IsBodyHtml = true;
                SendTo = new List<string>();
                CcTo = new List<string>();

                XDocument formXmlDocument = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
                var element = formXmlDocument.Root;
                if (element == null || element.Name != "QueueEntity")
                    throw new ArgumentException("Invalid QueueEntity Definition");

                UserId = element.Element("UserId") != null
                    ? element.Element("UserId").Value
                    : string.Empty;
                EmailTitle = element.Element("EmailTitle") != null
                    ? element.Element("EmailTitle").Value
                    : string.Empty;
                EmailBody = element.Element("EmailBody") != null
                    ? element.Element("EmailBody").Value
                    : string.Empty;
                IsBodyHtml = element.Element("IsBodyHtml") != null
                    ? bool.Parse(element.Element("IsBodyHtml").Value)
                    : true;

                if (element.Element("SendList") != null)
                {
                    var sendList = element.Element("SendList").Elements("SendTo");
                    foreach (var send in sendList)
                    {
                        SendTo.Add(send.Value);
                    }
                }

                if (element.Element("CcList") != null)
                {
                    var ccList = element.Element("CcList").Elements("CcTo");
                    foreach (var cc in ccList)
                    {
                        SendTo.Add(cc.Value);
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
