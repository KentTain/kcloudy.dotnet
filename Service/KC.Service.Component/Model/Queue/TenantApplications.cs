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
    public class TenantApplications : QueueEntity
    {
        public TenantApplications()
        {
            AppIds = new List<int>();
        }
        [DataMember]
        public int TenantId { get; set; }
        [DataMember]
        public string TenantName { get; set; }
        [DataMember]
        public string TenantDisplayName { get; set; }
        [DataMember]
        public List<int> AppIds { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string EmailTitle { get; set; }
        [DataMember]
        public string EmailContent { get; set; }
        [DataMember]
        public string AdminNewPassword { get; set; }
        [DataMember]
        public string AdminEmail { get; set; }
        [DataMember]
        public string AdminPhone { get; set; }

        public override string ErrorMessage
        {
            get
            {
                var sbError = new StringBuilder();
                sbError.AppendLine(string.Format("租户（Id：{0}; Code：{1}; Name：{2}）开通应用失败：", TenantId, Tenant, TenantDisplayName));
                if (AppIds.Any())
                    sbError.AppendLine("应用Id:" + string.Join(",",AppIds) + ". ");
                sbError.AppendLine("通知人邮件：" + Email + ". ");
                sbError.AppendLine("通知人邮件标题：" + EmailTitle + ". ");
                sbError.AppendLine("通知人邮件内容：" + EmailContent + ". ");
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
            element.Add(new XElement("TenantId", this.TenantId));
            element.Add(new XElement("TenantName", this.TenantName));
            element.Add(new XElement("Email", this.Email));
            element.Add(new XElement("EmailTitle", this.EmailTitle));
            element.Add(new XElement("EmailContent", this.EmailContent));
            element.Add(new XElement("AdminNewPassword", this.AdminNewPassword));
            element.Add(new XElement("AdminEmail", this.AdminEmail));
            element.Add(new XElement("AdminPhone", this.AdminPhone));
            var pXelement = new XElement("AppIds");
            foreach (var p in AppIds)
            {
                pXelement.Add(new XElement("Phone", p));
            }
            element.Add(pXelement);

            base.SerializeToXml(element);
            return element;
        }

        public TenantApplications(string xml)
            : base(xml)
        {
            try
            {
                XDocument formXmlDocument = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
                var element = formXmlDocument.Root;
                if (element == null || element.Name != "QueueEntity")
                    throw new ArgumentException("Invalid QueueEntity Definition");

                TenantId = element.Element("TenantId") != null
                    ? int.Parse(element.Element("TenantId").Value) 
                    : 0;
                TenantName = element.Element("TenantName") != null
                    ? element.Element("TenantName").Value
                    : string.Empty;
                TenantDisplayName = element.Element("TenantDisplayName") != null
                    ? element.Element("TenantDisplayName").Value
                    : string.Empty;
                Email = element.Element("Email") != null
                    ? element.Element("Email").Value
                    : string.Empty;
                EmailTitle = element.Element("EmailTitle") != null
                    ? element.Element("EmailTitle").Value
                    : string.Empty;
                EmailContent = element.Element("EmailContent") != null
                    ? element.Element("EmailContent").Value
                    : string.Empty;
                AdminNewPassword = element.Element("AdminNewPassword") != null
                    ? element.Element("AdminNewPassword").Value
                    : string.Empty;
                AdminEmail = element.Element("AdminEmail") != null
                    ? element.Element("AdminEmail").Value
                    : string.Empty;
                AdminPhone = element.Element("AdminPhone") != null
                    ? element.Element("AdminPhone").Value
                    : string.Empty;
                var AppIds = element.Element("AppIds");
                if (AppIds != null)
                {
                    foreach (var phone in AppIds.Elements("Phone"))
                    {
                        AppIds.Add(phone != null ? long.Parse(phone.Value) : 0);
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
