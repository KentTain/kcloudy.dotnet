using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using KC.Enums.Portal;
using System.Runtime.Serialization;
using KC.Common;
using KC.Framework.Extension;

namespace KC.Service.DTO.Portal
{

    [Serializable, DataContract(IsReference = true)]
    public class RecommendRequirementDTO : EntityDTO
    {
        public RecommendRequirementDTO()
        {
            RecommendMaterials = new List<RecommendMaterialDTO>();
        }

        [DataMember]
        public bool IsEditMode { get; set; }

        #region  需求基本信息
        [DataMember]
        public int RecommendId { get; set; }
        /// <summary>
        /// 是否内部推送，不能修改
        ///     需要设置内部引用产品Id：RecommendRefCode
        /// </summary>
        [DataMember]
        public bool IsInnerPush { get; set; }
        /// <summary>
        /// 引用采购需求编码
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string RecommendRefCode { get; set; }
        /// <summary>
        /// 采购需求编码
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string RecommendCode { get; set; }
        /// <summary>
        /// 采购名称
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

        #region 需求信息
        [DataMember]
        public RequirementType RequirementType { get; set; }

        [DataMember]
        public string RequirementTypeString { get { return RequirementType.ToDescription(); } }
        /// <summary>
        /// 省份Id
        /// </summary>
        [DataMember]
        public int ProvinceId { get; set; }
        /// <summary>
        /// 省份代码
        /// </summary>
        [DataMember]
        public string ProvinceCode { get; set; }
        /// <summary>
        /// 省份名
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string ProvinceName { get; set; }
        /// <summary>
        /// 城市Id
        /// </summary>
        [DataMember]
        public int CityId { get; set; }
        /// <summary>
        /// 城市代码
        /// </summary>
        [DataMember]
        public string CityCode { get; set; }
        /// <summary>
        /// 城市名
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string CityName { get; set; }

        [MaxLength(2000)]
        [DataMember]
        public string RequirementImage { get; set; }
        [DataMember]
        public BlobInfoDTO RequirementImageBlob
        {
            get
            {
                if (string.IsNullOrEmpty(RequirementImage))
                    return null;

                return SerializeHelper.FromJson<BlobInfoDTO>(RequirementImage);
            }
        }
        [MaxLength(2000)]
        [DataMember]
        public string RequirementFile { get; set; }

        [DataMember]
        public BlobInfoDTO RequirementFileBlob
        {
            get
            {
                if (string.IsNullOrEmpty(RequirementFile))
                    return null;

                return SerializeHelper.FromJson<BlobInfoDTO>(RequirementFile);
            }
        }
        /// <summary>
        /// 采购截止日期（UTC时间）
        /// </summary>
        [DataMember]
        public System.DateTime? ExpiredDate { get; set; }

        /// <summary>
        /// 是否公示
        /// </summary>
        [DataMember]
        public bool IsPublish { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(4000)]
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Content { get; set; }

        [MaxLength(512)]
        [DataMember]
        public string Barcode { get; set; }

        [DataMember]
        public int Index { get; set; }
        #endregion

        #region 当客户为集团用户时，可以展示其供应商/客户的相关信息
        /// <summary>
        /// 是否为企业的客户
        ///     为true时，CompanyCode设置为内部企业编码
        ///     为false时，CompanyCode设置为自己的租户编码
        /// </summary>
        [DataMember]
        public bool IsCustomer { get; set; }
        /// <summary>
        /// 企业编码
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string CompanyCode { get; set; }
        /// <summary>
        /// 企业名称
        /// </summary>
        [MaxLength(512)]
        [DataMember]
        public string CompanyName { get; set; }

        /// <summary>
        /// 联系人UserId
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string ContactId { get; set; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        [MaxLength(50)]
        [DataMember]
        public string ContactName { get; set; }
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

        [DataMember]
        public List<RecommendMaterialDTO> RecommendMaterials { get; set; }
    }
}
