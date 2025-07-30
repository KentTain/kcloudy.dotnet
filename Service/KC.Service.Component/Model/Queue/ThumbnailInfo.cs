using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using KC.Component.Base;

namespace KC.Model.Component.Queue
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class ThumbnailInfo : QueueEntity
    {
        public ThumbnailInfo ()
        {
            With = 16;
            Height = 16;
        }
        [DataMember]
        public string UserId { get; set; }
        [DataMember]
        public string ContainerName { get; set; }
        [DataMember]
        public string BlobId { get; set; }
        [DataMember]
        public int With { get; set; }
        [DataMember]
        public int Height { get; set; }

        public override string ErrorMessage
        {
            get
            {
                var sbError = new StringBuilder();
                sbError.AppendLine(string.Format("解析用户：{0}上传的财务报表出错，上传报表内容如下：", UserId));
                sbError.AppendLine("文件夹名称:" + ContainerName + ". ");
                sbError.AppendLine("报表文件BlobId:" + BlobId + ". ");
                sbError.AppendLine("缩略图宽:" + With + ". ");
                sbError.AppendLine("缩略图高:" + Height + ". ");
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
            element.Add(new XElement("ContainerName", this.ContainerName));
            element.Add(new XElement("BlobId", this.BlobId));
            element.Add(new XElement("With", this.With));
            element.Add(new XElement("Height", this.Height));
            element.Add(new XElement("ErrorMessage", this.ErrorMessage));

            base.SerializeToXml(element);
            return element;
        }

        public ThumbnailInfo(string xml)
            : base(xml)
        {
            try
            {
                XDocument formXmlDocument = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
                var element = formXmlDocument.Root;
                if (element == null || element.Name != "QueueEntity")
                    throw new ArgumentException("Invalid QueueEntity Definition");

                UserId = element.Element("UserId") != null
                    ? element.Element("UserId").Value
                    : string.Empty;
                ContainerName = element.Element("ContainerName") != null
                    ? element.Element("ContainerName").Value
                    : string.Empty;
                BlobId = element.Element("BlobId") != null
                    ? element.Element("BlobId").Value
                    : string.Empty;
                With = element.Element("With") != null
                   ? int.Parse(element.Element("With").Value)
                   : 16;
                Height = element.Element("Height") != null
                   ? int.Parse(element.Element("Height").Value)
                   : 16;
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
