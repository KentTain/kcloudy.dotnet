using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using KC.Component.Base;

namespace KC.Model.Component.Queue
{
    /// <summary>
    /// 用于存放到Queue中的数据对象，无需持久化到数据库
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class ReportInfo : QueueEntity
    {
        public ReportInfo()
        {
            SourceType = 1;
            DurationType = 3;
        }

        /// <summary>
        /// 会员ID
        /// </summary>
        [DataMember]
        public string UserId { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string ContainerName { get; set; }
        /// <summary>
        /// 上传BlobId
        /// </summary>
        [DataMember]
        public string BlobId { get; set; }

        /// <summary>
        /// 来源 --> enum:  KC.Model.Member.DurationType
        ///     [Description("客户来源")]
        ///     ClientSource = 1,
        ///     [Description("ERP来源")]
        ///     ERPSource = 2,
        ///     [Description("公司来源")]
        ///     CompanySource = 3,
        /// </summary>
        [DataMember]
        public int SourceType { get; set; }

        /// <summary>
        /// 期间类型 --> enum:  KC.Model.Member.SourceType
        ///     [Description("月度")]
        ///     Month = 1,
        ///     [Description("季度")]
        ///     Season = 2,
        ///     [Description("年度")]
        ///     Year = 3
        /// </summary>
        [DataMember]
        public int DurationType { get; set; }

        /// <summary>
        /// 期间
        /// </summary>
        [DataMember]
        public int Duration { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        [DataMember]
        public string OrderId { get; set; }

        /// <summary>
        /// 订单资料清单Id
        /// </summary>
        public int OrderReviewInfoId { get; set; }

        public override string ErrorMessage
        {
            get
            {
                var sbError = new StringBuilder();
                sbError.AppendLine(string.Format("解析用户：{0}上传的财务报表出错，上传报表内容如下：", UserName));
                sbError.AppendLine("订单ID:" + OrderId + ". ");
                sbError.AppendLine("报表来源（1:客户来源; 2:ERP来源; 3:公司来源）:" + SourceType + ". ");
                sbError.AppendLine("报表期间类型（1:月度; 2:季度; 3:年度）:" + DurationType + ". ");
                sbError.AppendLine("报表期间:" + Duration + ". ");
                sbError.AppendLine("文件夹名称:" + ContainerName + ". ");
                sbError.AppendLine("报表文件BlobId:" + BlobId + ". ");

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
            element.Add(new XElement("UserName", this.UserName));
            element.Add(new XElement("ContainerName", this.ContainerName));
            element.Add(new XElement("BlobId", this.BlobId));
            element.Add(new XElement("SourceType", this.SourceType));
            element.Add(new XElement("DurationType", this.DurationType));
            element.Add(new XElement("Duration", this.Duration));
            element.Add(new XElement("OrderId", this.OrderId));
            element.Add(new XElement("OrderReviewInfoId", this.OrderReviewInfoId));
            element.Add(new XElement("ErrorMessage", this.ErrorMessage));

            base.SerializeToXml(element);
            return element;
        }

        public ReportInfo(string xml)
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
                UserName = element.Element("UserName") != null
                    ? element.Element("UserName").Value
                    : string.Empty;
                ContainerName = element.Element("ContainerName") != null
                    ? element.Element("ContainerName").Value
                    : string.Empty;
                BlobId = element.Element("BlobId") != null
                    ? element.Element("BlobId").Value
                    : string.Empty;
                OrderId = element.Element("OrderId") != null
                   ? element.Element("OrderId").Value
                   : string.Empty;
                OrderReviewInfoId = element.Element("OrderReviewInfoId") != null
                   ? int.Parse(element.Element("OrderReviewInfoId").Value)
                   : 0;
                SourceType = element.Element("SourceType") != null
                   ? int.Parse(element.Element("SourceType").Value)
                   : 0;
                DurationType = element.Element("DurationType") != null
                   ? int.Parse(element.Element("DurationType").Value)
                   : 0;
                Duration = element.Element("Duration") != null
                   ? int.Parse(element.Element("Duration").Value)
                   : 0;
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
