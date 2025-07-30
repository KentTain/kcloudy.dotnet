using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.CRM;
using KC.Framework.Base;
using KC.Model.Customer.Constants;

namespace KC.Model.Customer
{
    [Table(Tables.CustomerAuthentication)]
    public class CustomerAuthentication : Entity
    {
        public CustomerAuthentication()
        {
        }

        /// <summary>
        /// 客户代码
        /// </summary>
        [Key]
        [MaxLength(128)]
        [DataMember]
        public string CustomerCode { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        [DataMember]
        [MaxLength(10)]
        public string ZipCode { get; set; }
        /// <summary>
        /// 省份Id
        /// </summary>
        [DataMember]
        public int? ProvinceId { get; set; }
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
        public int? CityId { get; set; }
        /// <summary>
        /// 城市名
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string CityName { get; set; }
        /// <summary>
        /// 区域Id
        /// </summary>
        [DataMember]
        public int? DistrictId { get; set; }
        /// <summary>
        /// 区域名
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string DistrictName { get; set; }
        /// <summary>
        /// 详情地址
        /// </summary>
        [DataMember]
        [MaxLength(500)]
        public string CompanyAddress { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        [DataMember]
        public DateTime? BulidDate { get; set; }
        /// <summary>
        /// 有效年份
        /// </summary>
        [DataMember]
        public int? BusinessDateLimit { get; set; }
        /// <summary>
        /// 是否长期有效
        /// </summary>
        [DataMember]
        public bool? IsLongTerm { get; set; }
        /// <summary>
        /// 注册资本
        /// </summary>
        [DataMember]
        public decimal RegisteredCapital { get; set; }
        /// <summary>
        /// 经营范围
        /// </summary>
        [DataMember]
        [MaxLength(1000)]
        [DisplayName("经营范围")]
        public string ScopeOfBusiness { get; set; }

        /// <summary>
        /// 统一社会信用证或工商营业执照
        /// </summary>
        [DataMember]
        [MaxLength(18)]
        [DisplayName("统一社会信用证或工商营业执照")]
        public string UnifiedSocialCreditCode { get; set; }
        /// <summary>
        /// 统一社会信用证图片对象
        /// </summary>
        [DataMember]
        [MaxLength(500)]
        public string UnifiedSocialCreditCodeScannPhoto { get; set; }

        /// <summary>
        /// 企业授权书图片对象
        /// </summary>
        [DataMember]
        [MaxLength(500)]
        public string LetterOfAuthorityScannPhoto { get; set; }

        /// <summary>
        /// 法人姓名
        /// </summary>
        [DataMember]
        [MaxLength(50)]
        [DisplayName("法人姓名")]
        public string LegalPerson { get; set; }
        /// <summary>
        /// 法人身份证图片对象（法人手持身份证）
        /// </summary>
        [DataMember]
        [MaxLength(500)]
        public string LegalPersonScannPhoto { get; set; }
        /// <summary>
        /// 法人身份证（18位）
        /// </summary>
        [DataMember]
        [MaxLength(18)]
        [DisplayName("法人身份证")]
        public string LegalPersonIdentityCardNumber { get; set; }
        /// <summary>
        /// 法人身份证图片对象（正面）
        /// </summary>
        [DataMember]
        [MaxLength(500)]
        public string LegalPersonIdentityCardPhoto { get; set; }
        /// <summary>
        /// 法人身份证图片对象（背面）
        /// </summary>
        [DataMember]
        [MaxLength(500)]
        public string LegalPersonIdentityCardPhotoOtherSide { get; set; }
        /// <summary>
        /// 法人身份证过期时间
        /// </summary>
        [DataMember]
        [DisplayName("法人身份证过期时间")]
        public DateTime? LegalPersonCertificateExpiryDate { get; set; }

        public int CustomerId { get; set; }
        public CustomerInfo CustomerInfo { get; set; }
    }
}
