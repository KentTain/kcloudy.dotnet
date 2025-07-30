using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using KC.Common;
using KC.Enums.Portal;
using KC.Framework.Extension;

namespace KC.Service.DTO.Portal
{

    [Serializable, DataContract(IsReference = true)]
    public class RecommendInfoDTO : EntityDTO
    {
        public RecommendInfoDTO()
        {
        }

        [DataMember]
        public bool IsEditMode { get; set; }

        [DataMember]
        public int RecommendId { get; set; }
        /// <summary>
        /// 引用企业Id
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string RecommendRefCode { get; set; }
        /// <summary>
        /// 企业编码
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string RecommendCode { get; set; }
        /// <summary>
        /// 企业名称
        /// </summary>
        [MaxLength(256)]
        [DataMember]
        public string RecommendName { get; set; }

        /// <summary>
        /// 推荐审批状态：kc.enums.RecommendStatus
        /// </summary>
        [DataMember]
        public RecommendStatus Status { get; set; }
        [DataMember]
        public string StatusString { get { return Status.ToDescription(); } }
        
        [DataMember]
        public RecommendType Type { get; set; }
        [DataMember]
        public string TypeString { get { return Type.ToDescription(); } }
        [MaxLength(2000)]
        [DataMember]
        public string RecommendImage { get; set; }
        [DataMember]
        public BlobInfoDTO RecommendImageBlob
        {
            get
            {
                if (string.IsNullOrEmpty(RecommendImage))
                    return null;

                return SerializeHelper.FromJson<BlobInfoDTO>(RecommendImage);
            }
        }
        [MaxLength(2000)]
        [DataMember]
        public string RecommendFile { get; set; }
        [DataMember]
        public BlobInfoDTO RecommendFileBlob
        {
            get
            {
                if (string.IsNullOrEmpty(RecommendFile))
                    return null;

                return SerializeHelper.FromJson<BlobInfoDTO>(RecommendFile);
            }
        }
        /// <summary>
        /// 是否置顶
        /// </summary>
        [DataMember]
        public bool IsTop { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(4000)]
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Content { get; set; }

        [DataMember]
        public int? CategoryId { get; set; }
        [DataMember]
        public string CategoryName { get; set; }

    }

    [DataContract]
    public enum RecommendType
    {
        /// <summary>
        /// 推荐用户
        /// </summary>
        [Display(Name = "推荐用户")]
        [Description("推荐用户")]
        [EnumMember]
        Customer = 0,

        /// <summary>
        /// 推荐商品
        /// </summary>
        [Display(Name = "推荐商品")]
        [Description("推荐商品")]
        [EnumMember]
        Offering = 1,

        /// <summary>
        /// 推荐招采需求
        /// </summary>
        [Display(Name = "推荐招采需求")]
        [Description("推荐招采需求")]
        [EnumMember]
        Requirement = 2,
    }
}
