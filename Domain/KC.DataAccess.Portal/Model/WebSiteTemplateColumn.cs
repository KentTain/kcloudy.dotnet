using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.Portal;
using KC.Framework.Base;
using KC.Model.Portal.Constants;

namespace KC.Model.Portal
{
    /// <summary>
    /// 页面项目组
    /// </summary>
    [Table(Tables.WebSiteTemplateColumn)]
    [Serializable, DataContract(IsReference = true)]
    public class WebSiteTemplateColumn : Entity
    {
        public WebSiteTemplateColumn()
        {
            WebSiteItems = new List<WebSiteTemplateItem>();
        }

        [Key]
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 页面标题
        /// </summary>
        [DataMember]
        [Required]
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
        /// <summary>
        /// 各个项目的类型  </br>
        ///     （LeftRight、LeftCenterRight、TopBottom、TopCenterBottom）、Product： </br>
        /// </summary>
        [DataMember]
        public WebSiteItemType ItemType { get; set; }

        /// <summary>
        /// 行数，设置为：1~6
        /// </summary>
        [Range(1, 6)]
        [DataMember]
        public int RowCount { get; set; }

        /// <summary>
        /// 列数，设置为：1、2、3、4、6
        /// </summary>
        [Range(2, 6)]
        [DataMember]
        public int ColumnCount { get; set; }


        /// <summary>
        /// 是否为图片
        /// </summary>
        [DataMember]
        public bool IsImage { get; set; }

        /// <summary>
        /// 图片地址或是IConCls
        ///     (fontawesome的字体样式，如：fa fa-camera-retro fa-3x)
        /// </summary>
        [DataMember]
        [MaxLength(4000)]
        public string ImageOrIConCls { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        [DataMember]
        [MaxLength(512)]
        public string Link { get; set; }

        /// <summary>
        /// 链接是否打开新页面
        /// </summary>
        [DataMember]
        public bool LinkIsOpenNewPage { get; set; }
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

        public virtual ICollection<WebSiteTemplateItem> WebSiteItems { get; set; }
    }
}
