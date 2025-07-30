using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using KC.Enums.Portal;

namespace KC.Service.DTO.Portal
{
    /// <summary>
    /// 页面项目
    /// </summary>
    [Serializable, DataContract(IsReference = true)]
    public class WebSiteTemplateItemDTO : EntityDTO
    {
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 项目标题
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
        [DataMember]
        [MaxLength(4000)]
        public string Description { get; set; }

        /// <summary>
        /// 自定义内容
        /// </summary>
        [DataMember]
        public string CustomizeContent { get; set; }

        [Required]
        [DataMember]
        public Guid WebSiteTemplateColumnId { get; set; }
        [DataMember]
        public string WebSiteTemplateColumnName { get; set; }
    }
}
