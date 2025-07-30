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
    public class DownloadVoiceInfo : QueueEntity
    {
        public DownloadVoiceInfo ()
        {
        }
        [DataMember]
        public string DownloadUrl { get; set; }
        [DataMember]
        public string SessionId { get; set; }
        [DataMember]
        public string TenantName { get; set; }
        [DataMember]
        public string BlobId { get; set; }


        public override string ErrorMessage
        {
            get
            {
                var sbError = new StringBuilder();
                sbError.AppendLine(string.Format("下载语音通话记录失败，上传报表内容如下："));
                sbError.AppendLine("下载地址:" + DownloadUrl + ". ");
                sbError.AppendLine("SessionId:" + SessionId + ". ");
                sbError.AppendLine("TenantName:" + TenantName + ". ");
                sbError.AppendLine("BlobId:" + BlobId + ". ");
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
            element.Add(new XElement("DownloadUrl", this.DownloadUrl));
            element.Add(new XElement("SessionId", this.SessionId));
            element.Add(new XElement("TenantName", this.TenantName));
            element.Add(new XElement("BlobId", this.BlobId));
            element.Add(new XElement("ErrorMessage", this.ErrorMessage));

            base.SerializeToXml(element);
            return element;
        }

        public DownloadVoiceInfo(string xml)
            : base(xml)
        {
            try
            {
                XDocument formXmlDocument = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
                var element = formXmlDocument.Root;
                if (element == null || element.Name != "QueueEntity")
                    throw new ArgumentException("Invalid QueueEntity Definition");

                DownloadUrl = element.Element("DownloadUrl") != null
                    ? element.Element("DownloadUrl").Value
                    : string.Empty;
                SessionId = element.Element("SessionId") != null
                    ? element.Element("SessionId").Value
                    : string.Empty;
                TenantName = element.Element("TenantName") != null
                    ? element.Element("TenantName").Value
                    : string.Empty;
                BlobId = element.Element("BlobId") != null
                    ? element.Element("BlobId").Value
                    : string.Empty;
                ErrorMessage = element.Element("ErrorMessage") != null
                    ? element.Element("ErrorMessage").Value
                    : string.Empty;
            }
            catch (Exception)
            {
                throw new Exception("Invalid QueueEntity Definition");
            }
        }
    }
}
