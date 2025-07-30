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
    public class PenaltyInterestInfo : QueueEntity
    {
        public PenaltyInterestInfo()
        {
            IsLastCaculate = true;
        }
        [DataMember]
        public int TenantId { get; set; }
        [DataMember]
        public string TenantDisplayName { get; set; }
        [DataMember]
        public int OrderType { get; set; }
        [DataMember]
        public int RepaymentRemindId { get; set; }
        [DataMember]
        public bool IsLastCaculate { get; set; }

        public override string ErrorMessage
        {
            get
            {
                var sbError = new StringBuilder();
                sbError.AppendLine("计算罚息出错，请重新手动计算，计算信息如下：");
                sbError.AppendLine(string.Format("租户Id：{0}", TenantId));
                sbError.AppendLine(string.Format("租户代码：{0}", Tenant));
                sbError.AppendLine(string.Format("租户名称：{0}", TenantDisplayName));
                sbError.AppendLine(string.Format("罚息计算Id：{0}", RepaymentRemindId));
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
            element.Add(new XElement("TenantId", this.TenantId));
            element.Add(new XElement("TenantDisplayName", this.TenantDisplayName));
            element.Add(new XElement("OrderType", this.OrderType));
            element.Add(new XElement("RepaymentRemindId", this.RepaymentRemindId));
            element.Add(new XElement("IsLastCaculate", this.IsLastCaculate));
            element.Add(new XElement("ErrorMessage", this.ErrorMessage));

            
            base.SerializeToXml(element);
            return element;
        }

        public PenaltyInterestInfo(string xml)
            : base(xml)
        {
            try
            {
                IsLastCaculate = true;

                XDocument formXmlDocument = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
                var element = formXmlDocument.Root;
                if (element == null || element.Name != "QueueEntity")
                    throw new ArgumentException("Invalid QueueEntity Definition");

                TenantId = element.Element("TenantId") != null
                           && !string.IsNullOrWhiteSpace(element.Element("TenantId").Value)
                    ? int.Parse(element.Element("TenantId").Value)
                    : 0;
                TenantDisplayName = element.Element("TenantDisplayName") != null
                    ? element.Element("TenantDisplayName").Value
                    : string.Empty;
                OrderType = element.Element("OrderType") != null
                            && !string.IsNullOrWhiteSpace(element.Element("OrderType").Value)
                    ? int.Parse(element.Element("OrderType").Value)
                    : 0;
                RepaymentRemindId = element.Element("RepaymentRemindId") != null
                           && !string.IsNullOrWhiteSpace(element.Element("RepaymentRemindId").Value)
                    ? int.Parse(element.Element("RepaymentRemindId").Value)
                    : 0;
                IsLastCaculate = element.Element("IsLastCaculate") != null
                                 && !string.IsNullOrWhiteSpace(element.Element("IsLastCaculate").Value)
                    ? bool.Parse(element.Element("IsLastCaculate").Value)
                    : true;

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
