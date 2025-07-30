using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using KC.Framework.Base;
using KC.Model.Admin.Constants;

namespace KC.Model.Admin
{
    [Table(Tables.TenantUserOffering)]
    [Serializable, DataContract(IsReference = true)]
    public class TenantUserOffering : Entity
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DataMember]
        public int RecommendId { get; set; }
        /// <summary>
        /// 来源租户编码
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string TenantName { get; set; }
        /// <summary>
        /// 来源租户名称
        /// </summary>
        [MaxLength(512)]
        [DataMember]
        public string TenantDisplayName { get; set; }

        #region 推荐商品基本信息

        /// <summary>
        /// 是否内部推送，不能修改
        ///     需要设置内部引用商品编码：RecommendRefCode
        /// </summary>
        [DataMember]
        public bool IsInnerPush { get; set; }
        /// <summary>
        /// 引用商品编码
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string RecommendRefCode { get; set; }
        /// <summary>
        /// 商品编码
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string RecommendCode { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [MaxLength(256)]
        [DataMember]
        public string RecommendName { get; set; }

        /// <summary>
        /// 推荐审批状态：kc.enums.WorkflowBusStatus
        /// </summary>
        [DataMember]
        public WorkflowBusStatus Status { get; set; }
        #endregion

        #region 商品信息
        [MaxLength(128)]
        [DataMember]
        public string OfferingTypeCode { get; set; }
        [MaxLength(512)]
        [DataMember]
        public string OfferingTypeName { get; set; }

        [MaxLength(20)]
        [DataMember]
        public string OfferingUnit { get; set; }

        [MaxLength(1000)]
        [DataMember]
        public string OfferingImageBlob { get; set; }

        [MaxLength(1000)]
        [DataMember]
        public string OfferingFileBlob { get; set; }

        [DataMember]
        public decimal? OfferingPrice { get; set; }

        [DataMember]
        public decimal? OfferingDiscount { get; set; }

        [DataMember]
        public decimal? OfferingRate { get; set; }

        [MaxLength(4000)]
        [DataMember]
        public string OfferingAddress { get; set; }

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
    }
}
