using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Model.Portal.Constants;
using KC.Framework.Base;
using System.Runtime.Serialization;

namespace KC.Model.Portal
{
    [Table(Tables.RecommendMaterial)]
    public class RecommendMaterial : Entity
    {
        #region 采购物料的基本信息
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DataMember]
        public int MaterialId { get; set; }

        /// <summary>
        /// 是否内部推送，不能修改
        ///     需要设置内部引用配件/原材料编码：MaterialRefCode
        /// </summary>
        [DataMember]
        public bool IsInnerPush { get; set; }
        /// <summary>
        /// 引用配件/原材料编码
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string MaterialRefCode { get; set; }
        /// <summary>
        /// 配件/原材料编码
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string MaterialCode { get; set; }
        /// <summary>
        /// 配件/原材料名称
        /// </summary>
        [MaxLength(256)]
        [DataMember]
        public string MaterialName { get; set; }
        #endregion

        #region 配件/原材料信息
        [MaxLength(128)]
        [DataMember]
        public string MaterialTypeCode { get; set; }
        [MaxLength(512)]
        [DataMember]
        public string MaterialTypeName { get; set; }

        [MaxLength(20)]
        [DataMember]
        public string MaterialUnit { get; set; }

        [MaxLength(2000)]
        [DataMember]
        public string MaterialImage { get; set; }

        [MaxLength(2000)]
        [DataMember]
        public string MaterialFile { get; set; }

        [DataMember]
        public decimal? MaterialPrice { get; set; }

        [DataMember]
        public decimal? MaterialDiscount { get; set; }

        [DataMember]
        public decimal? MaterialRate { get; set; }

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

        public int? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual RecommendCategory Category { get; set; }

        public ICollection<RequirementForMaterial> RequirementForMaterials { get; set; }

        [NotMapped]
        public IEnumerable<RecommendRequirement> RecommendRequirements => RequirementForMaterials.Select(e => e.RecommendRequirement);

    }
}
