using KC.Enums.Portal;
using KC.Framework.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

namespace KC.Service.DTO.Portal
{
    /// <summary>
    /// 页面项目组
    /// </summary>
    [Serializable, DataContract(IsReference = true)]
    public class WebSiteColumnDTO : EntityDTO
    {
        public WebSiteColumnDTO()
        {
            Id = Guid.NewGuid();
            WebSiteItems = new List<WebSiteItemDTO>();
        }

        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 页面标题
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        public string Title { get; set; }

        /// <summary>
        /// 项目子标题
        /// </summary>
        [DataMember]
        [MaxLength(4000)]
        public string SubTitle { get; set; }

        /// <summary>
        /// 元素组类型：图片式、卡片式、列表式、自定义 </br>
        ///     图片式，需要设置（ImageFile/IConCls）中其中之一的值 </br>
        ///     卡片式，还需要选择卡片类型（CardType） </br>
        ///     自定义，需要设置自定义内容（CustomizeContent）
        /// </summary>
        [DataMember]
        public WebSiteColumnType Type { get; set; }
        [DataMember]
        public string TypeString { get { return Type.ToDescription(); } }
        /// <summary>
        /// 各个项目的类型  </br>
        ///     （LeftRight、LeftCenterRight、TopBottom、TopCenterBottom）、Product： </br>
        /// </summary>
        [DataMember]
        public WebSiteItemType ItemType { get; set; }
        [DataMember]
        public string ItemTypeString { get { return ItemType.ToDescription(); } }

        /// <summary>
        /// 行数，设置为：1~6
        /// </summary>
        [Range(1, 6)]
        [DataMember]
        public int RowCount { get; set; }

        /// <summary>
        /// 列数，设置为：1、2、3、4、6
        /// </summary>
        [Range(1, 6)]
        [DataMember]
        public int ColumnCount { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        [DataMember]
        public bool IsShow { get; set; }

        /// <summary>
        /// 是否能编辑
        /// </summary>
        [DataMember]
        public bool CanEdit { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [DataMember]
        public int Index { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(4000)]
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// 自定义内容
        /// </summary>
        [DataMember]
        public string CustomizeContent { get; set; }

        [Required]
        [DataMember]
        public Guid WebSitePageId { get; set; }

        [DataMember]
        public string WebSitePageName { get; set; }

        [DataMember]
        public List<WebSiteItemDTO> WebSiteItems { get; set; }
    }
}
