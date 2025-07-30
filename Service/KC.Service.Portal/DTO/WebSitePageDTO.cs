using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using KC.Common;
using KC.Enums.Portal;
using KC.Framework.Base;

namespace KC.Service.DTO.Portal
{
    [Serializable, DataContract(IsReference = true)]
    public class WebSitePageDTO : EntityDTO
    {
        public WebSitePageDTO()
        {
            Id = Guid.NewGuid();
            WebSiteColumns = new List<WebSiteColumnDTO>();
        }

        [DataMember]
        public bool IsEditMode { get; set; }

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
        /// 图片对象
        /// </summary>
        [DataMember]
        public List<BlobInfoDTO> MainSlideBlobs
        {
            get
            {
                if (!UseMainSlide || string.IsNullOrEmpty(MainSlide))
                    return null;

                return SerializeHelper.FromJson<List<BlobInfoDTO>>(MainSlide);
            }
        }
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

        [DataMember]
        public ICollection<WebSiteColumnDTO> WebSiteColumns { get; set; }
    }
}
