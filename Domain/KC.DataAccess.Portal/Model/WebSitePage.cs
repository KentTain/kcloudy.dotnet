using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using KC.Enums.Portal;
using KC.Framework.Base;
using KC.Model.Portal.Constants;

namespace KC.Model.Portal
{
    [Table(Tables.WebSitePage)]
    [Serializable, DataContract(IsReference = true)]
    public class WebSitePage : Entity
    {
        public WebSitePage()
        {
            WebSiteColumns = new List<WebSiteColumn>();
        }

        [Key]
        [DataMember]
        public Guid Id { get; set; }

        [MaxLength(20)]
        [DataMember]
        public string SkinCode { get; set; }

        [MaxLength(200)]
        [DataMember]
        public string SkinName { get; set; }

        /// <summary>
        /// 栏目名称
        /// </summary>
        [DataMember]
        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        /// <summary>
        /// 页面类型
        /// </summary>
        [DataMember]
        public WebSitePageType Type { get; set; }

        /// <summary>
        /// 发布状态
        /// </summary>
        [DataMember]
        public WorkflowBusStatus Status { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [DataMember]
        public bool IsEnable { get; set; }

        /// <summary>
        /// 展示页面链接地址
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
        /// 栏目主色调
        /// </summary>
        [DataMember]
        [MaxLength(50)]
        public string MainColor { get; set; }

        /// <summary>
        /// 栏目辅色调
        /// </summary>
        [DataMember]
        [MaxLength(50)]
        public string SecondaryColor { get; set; }

        /// <summary>
        /// 是否使用首页轮播图
        /// </summary>
        [DataMember]
        public bool UseMainSlide { get; set; }

        [DataMember]
        [MaxLength(4000)]
        public string MainSlide { get; set; }

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

        public virtual ICollection<WebSiteColumn> WebSiteColumns { get; set; }
    }
}
