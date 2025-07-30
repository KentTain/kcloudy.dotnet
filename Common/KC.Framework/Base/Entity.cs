using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Linq;
using ProtoBuf;

namespace KC.Framework.Base
{
    /// <summary>
    /// 实体模型基类
    /// </summary>
    [Serializable, DataContract(IsReference = true), ProtoContract]
    public abstract class Entity : EntityBase, ICloneable
    {
        #region 构造函数

        /// <summary>
        ///     数据实体基类
        /// </summary>
        protected Entity()
        {
            IsDeleted = false;
            CreatedDate = DateTime.UtcNow;
            ModifiedDate = DateTime.UtcNow;
        }

        #endregion

        #region 属性

        /// <summary>
        ///     获取或设置 获取或设置是否禁用，逻辑上的删除，非物理删除
        /// </summary>
        [DefaultValue(false)]
        [DataMember]
        [ProtoMember(1)]
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 创建人Id
        /// </summary>
        [Display(Name = "创建人Id")]
        [DataMember]
        [ProtoMember(2)]
        [MaxLength(128)]
        public string CreatedBy { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [Display(Name = "创建人")]
        [DataMember]
        [ProtoMember(3)]
        [MaxLength(128)]
        public string CreatedName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        [DataMember]
        [ProtoMember(4)]
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// 修改人Id
        /// </summary>
        [Display(Name = "修改人Id")]
        [DataMember]
        [ProtoMember(5)]
        [MaxLength(128)]
        public string ModifiedBy { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        [Display(Name = "修改人")]
        [DataMember]
        [ProtoMember(6)]
        [MaxLength(128)]
        public string ModifiedName { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        [Display(Name = "修改时间")]
        [DataMember]
        [ProtoMember(7)]
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        ///     获取或设置 版本控制标识，用于处理并发
        /// </summary>
        //[ConcurrencyCheck]
        //[Timestamp]
        //public byte[] Timestamp { get; set; }

        #endregion

        protected XDocument GetDocumentRoot()
        {
            var doc = new XDocument();
            var root = new XElement("Entity");
            doc.Add(root);

            return doc;
        }

        protected XElement GetRootElement(string xml)
        {
            XDocument formXmlDocument = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
            var element = formXmlDocument.Root;
            if (element == null || element.Name != "Entity")
                throw new ArgumentException("Invalid Entity Definition");

            return element;
        }

        protected virtual string GetEnityObjectXml()
        {
            var doc = GetDocumentRoot();

            SerializeToXml(doc.Root);
            return doc.ToString(SaveOptions.DisableFormatting);
        }

        protected virtual XElement SerializeToXml(XElement element)
        {
            element.Add(new XElement("IsDeleted", this.IsDeleted));
            element.Add(new XElement("CreatedBy", this.CreatedBy));
            element.Add(new XElement("CreatedName", this.CreatedName));
            element.Add(new XElement("CreatedDate", this.CreatedDate));
            element.Add(new XElement("ModifiedBy", this.ModifiedBy));
            element.Add(new XElement("ModifiedName", this.ModifiedName));
            element.Add(new XElement("ModifiedDate", this.ModifiedDate));

            return element;
        }

        protected Entity(string xml)
        {
            try
            {
                var element = GetRootElement(xml);

                IsDeleted = element.Element("IsDeleted") != null
                    ? bool.Parse(element.Element("IsDeleted").Value)
                    : false;
                CreatedBy = element.Element("CreatedBy") != null
                    ? element.Element("CreatedBy").Value
                    : string.Empty;
                CreatedName = element.Element("CreatedName") != null
                    ? element.Element("CreatedName").Value
                    : string.Empty;
                CreatedDate = element.Element("CreatedDate") != null
                    ? DateTime.Parse(element.Element("CreatedDate").Value)
                    : DateTime.UtcNow;
                ModifiedBy = element.Element("ModifiedBy") != null
                    ? element.Element("ModifiedBy").Value
                    : string.Empty;
                ModifiedName = element.Element("ModifiedName") != null
                    ? element.Element("ModifiedName").Value
                    : string.Empty;
                ModifiedDate = element.Element("ModifiedDate") != null
                    ? DateTime.Parse(element.Element("ModifiedDate").Value)
                    : DateTime.UtcNow;

            }
            catch (Exception)
            {
                throw new Exception("Invalid Entity Definition");
            }
        }

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
