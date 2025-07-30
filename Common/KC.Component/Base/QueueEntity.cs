using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Linq;
using KC.Framework.Base;
using KC.Framework.Tenant;

namespace KC.Component.Base
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class QueueEntity : EntityBase
    {
        protected QueueEntity()
        {
            Id = Guid.NewGuid().ToString();
            ErrorCount = 0;
            MaxProcessErrorCount = 3;
            CreatedDate = DateTime.UtcNow;
            IsManuallyDelete = false;
        }
        [DataMember]
        public string Id { get; set; }
        /// <summary>
        /// 执行队列产生的错误次数
        /// </summary>
        [DataMember]
        public int ErrorCount { get; set; }
        /// <summary>
        /// 最大可以处理的错误次数
        /// </summary>
        [DataMember]
        public int MaxProcessErrorCount { get; set; }
        [DataMember]
        public string QueueName { get; set; }
        [DataMember]
        public QueueType QueueType { get; set; }
        [DataMember]
        public DateTime CreatedDate { get; set; }
        [DataMember]
        public virtual string ErrorMessage { get; set; }
        /// <summary>
        /// 租户编码（TenantName）
        /// </summary>
        [DataMember]
        public string Tenant { get; set; }
        /// <summary>
        /// 队列消息成功执行后，是否要删除的字段（NotDeletedWhenSuccess），
        /// 以控制队列的是否一致存在，直到调用者进行删除，默认为：false
        /// </summary>
        [DataMember]
        public bool IsManuallyDelete { get; set; }


        protected XDocument GetDocumentRoot()
        {
            var doc = new XDocument();
            var root = new XElement("QueueEntity");
            doc.Add(root);

            return doc;
        }

        protected XElement GetRootElement(string xml)
        {
            XDocument formXmlDocument = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
            var element = formXmlDocument.Root;
            if (element == null || element.Name != "QueueEntity")
                throw new ArgumentException("Invalid QueueEntity Definition");

            return element;
        }

        public virtual string GetQueueObjectXml()
        {
            var doc = GetDocumentRoot();

            SerializeToXml(doc.Root);
            return doc.ToString(SaveOptions.DisableFormatting);
        }

        protected virtual XElement SerializeToXml(XElement element)
        {
            element.Add(new XElement("Id", this.Id));
            element.Add(new XElement("Tenant", this.Tenant));
            element.Add(new XElement("ErrorCount", this.ErrorCount));
            element.Add(new XElement("MaxProcessErrorCount", this.MaxProcessErrorCount));
            element.Add(new XElement("QueueType", this.QueueType.ToString()));
            element.Add(new XElement("QueueName", this.QueueName));
            element.Add(new XElement("CreatedDate", this.CreatedDate));
            element.Add(new XElement("IsManuallyDelete", this.IsManuallyDelete));

            return element;
        }

        protected QueueEntity(string xml)
        {
            try
            {
                var element = GetRootElement(xml);

                Id = element.Element("Id") != null
                    ? element.Element("Id").Value
                    : Guid.NewGuid().ToString();
                ErrorCount = element.Element("ErrorCount") != null
                    ? int.Parse(element.Element("ErrorCount").Value)
                    : 0;
                MaxProcessErrorCount = element.Element("MaxProcessErrorCount") != null
                    ? int.Parse(element.Element("MaxProcessErrorCount").Value)
                    : 0;
                QueueType = element.Element("QueueType") != null
                    ? (QueueType)Enum.Parse(typeof(QueueType), element.Element("QueueType").Value)
                    : QueueType.UNKNOWN;
                QueueName = element.Element("QueueName") != null
                    ? element.Element("QueueName").Value
                    : string.Empty;
                Tenant = element.Element("Tenant") != null
                    ? element.Element("Tenant").Value
                    : string.Empty;
                CreatedDate = element.Element("CreatedDate") != null
                              && !string.IsNullOrWhiteSpace(element.Element("CreatedDate").Value)
                    ? DateTime.Parse(element.Element("CreatedDate").Value)
                    : DateTime.UtcNow;
                IsManuallyDelete = element.Element("IsManuallyDelete") != null
                                        && !string.IsNullOrWhiteSpace(element.Element("IsManuallyDelete").Value)
                    ? bool.Parse(element.Element("IsManuallyDelete").Value)
                    : false;
            }
            catch (Exception)
            {
                throw new Exception("Invalid QueueEntity Definition");
            }
        }
    }
}
