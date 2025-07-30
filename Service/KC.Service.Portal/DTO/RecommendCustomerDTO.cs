using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using KC.Common;
using KC.Enums.Portal;
using KC.Framework.Extension;

namespace KC.Service.DTO.Portal
{

    [Serializable, DataContract(IsReference = true)]
    public class RecommendCustomerDTO : EntityDTO
    {
        public RecommendCustomerDTO()
        {
            CompanyType = CompanyType.Customer;
        }

        [DataMember]
        public bool IsEditMode { get; set; }

        #region 推荐企业基本信息
        [DataMember]
        public int RecommendId { get; set; }
        /// <summary>
        /// 是否内部推送，不能修改
        ///     需要设置内部引用企业CustomerCode：RecommendRefCode
        /// </summary>
        [DataMember]
        public bool IsInnerPush { get; set; }
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

        /// <summary>
        /// 是否置顶
        /// </summary>
        [DataMember]
        public bool IsTop { get; set; }
        #endregion

        #region 企业信息
        /// <summary>
        /// 企业类型
        /// </summary>
        [DataMember]
        public CompanyType CompanyType { get; set; }
        [DataMember]
        public string CompanyTypeString { get { return CompanyType.ToDescription(); } }

        /// <summary>
        /// 企业Logo
        /// </summary>
        [DataMember]
        [MaxLength(1000)]
        public string CompanyLogo { get; set; }
        /// <summary>
        /// 租户Logo对象
        /// </summary>

        [DataMember]
        public BlobInfoDTO CompanyLogoBlob
        {
            get
            {
                if (string.IsNullOrEmpty(CompanyLogo))
                    return null;

                return SerializeHelper.FromJson<BlobInfoDTO>(CompanyLogo);
            }
        }
        /// <summary>
        /// 企业经营模式
        /// </summary>
        [DataMember]
        public KC.Enums.Portal.BusinessModel BusinessModel { get; set; }

        /// <summary>
        /// 所属行业
        /// </summary>
        [MaxLength(128)]
        public string IndustryId { get; set; }

        /// <summary>
        /// 所属行业
        /// </summary>
        [MaxLength(128)]
        public string IndustryName { get; set; }

        /// <summary>
        /// 联系人UserId
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        public string ContactId { get; set; }

        /// <summary>
        /// 联系人姓名
        /// </summary>
        [MaxLength(50)]
        [DataMember]
        public string ContactName { get; set; }

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

        [DataMember]
        public string Content { get; set; }

        /// <summary>
        /// 客户网址
        /// </summary>
        [DataMember]
        public string CustomerDomain { get; set; }
        #endregion

        #region 外部引用编号
        /// <summary>
        /// 外部编号1
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        [Display(Name = "外部编号1")]
        public string ReferenceId1 { get; set; }

        /// <summary>
        /// 外部编号2
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        [Display(Name = "外部编号2")]
        public string ReferenceId2 { get; set; }
        /// <summary>
        /// 外部编号3
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        [Display(Name = "外部编号3")]
        public string ReferenceId3 { get; set; }
        #endregion

        [DataMember]
        public int? CategoryId { get; set; }
        [DataMember]
        public string CategoryName { get; set; }

    }
}
