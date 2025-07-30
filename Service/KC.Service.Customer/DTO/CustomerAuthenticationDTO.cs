using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using KC.Common;
using KC.Framework.Base;
using KC.Framework.Extension;

namespace KC.Service.DTO.Customer
{
    public class CustomerAuthenticationDTO : Entity
    {
        public CustomerAuthenticationDTO()
        {
        }
        [DataMember]
        public bool IsEditMode { get; set; }

        /// <summary>
        /// 客户代码（SequenceName--Organization：ORG2018120100001）
        /// </summary>
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
        /// 是否三证合一
        /// </summary>
        [DataMember]
        public bool IsUnified { get; set; }

        /// <summary>
        /// 工商营业执照（15位）
        /// </summary>
        [DataMember]
        [MaxLength(15)]
        [DisplayName("工商营业执照")]
        public string BusinessCode { get; set; }
        /// <summary>
        /// 工商营业执照图片Json字符串
        /// </summary>
        [DataMember]
        [MaxLength(500)]
        public string BusinessCodeScannPhoto { get; set; }
        /// <summary>
        /// 工商营业执照图片对象
        /// </summary>
        public BlobInfoDTO BusinessCodeScannPhotoBlob
        {
            get
            {
                if (BusinessCodeScannPhoto.IsNullOrEmpty())
                    return null;

                return SerializeHelper.FromJson<BlobInfoDTO>(BusinessCodeScannPhoto);
            }
        }
        /// <summary>
        /// 组织机构代码证（9位）
        /// </summary>
        [DataMember]
        [MaxLength(9)]
        [DisplayName("组织机构代码证")]
        public string OrganizationCode { get; set; }
        /// <summary>
        /// 组织机构代码证图片Json字符串
        /// </summary>
        [DataMember]
        [MaxLength(500)]
        public string OrganizationCodeScannPhoto { get; set; }
        /// <summary>
        /// 组织机构代码证图片对象
        /// </summary>
        public BlobInfoDTO OrganizationCodeScannPhotoBlob
        {
            get
            {
                if (OrganizationCodeScannPhoto.IsNullOrEmpty())
                    return null;

                return SerializeHelper.FromJson<BlobInfoDTO>(OrganizationCodeScannPhoto);
            }
        }
        /// <summary>
        /// 税务登记证（18位）
        /// </summary>
        [DataMember]
        [MaxLength(18)]
        [DisplayName("税务登记证")]
        public string TaxpayerCode { get; set; }
        /// <summary>
        /// 税务登记证图片Json字符串
        /// </summary>
        [DataMember]
        [MaxLength(500)]
        public string TaxpayerCodeScannPhoto { get; set; }
        /// <summary>
        /// 税务登记证图片对象
        /// </summary>
        public BlobInfoDTO TaxpayerCodeScannPhotoBlob
        {
            get
            {
                if (TaxpayerCodeScannPhoto.IsNullOrEmpty())
                    return null;

                return SerializeHelper.FromJson<BlobInfoDTO>(TaxpayerCodeScannPhoto);
            }
        }
        /// <summary>
        /// 统一社会信用证（18位）
        /// </summary>
        [DataMember]
        [MaxLength(18)]
        [DisplayName("统一社会信用证")]
        public string UnifiedSocialCreditCode { get; set; }
        /// <summary>
        /// 统一社会信用证图片Json字符串
        /// </summary>
        [DataMember]
        [MaxLength(500)]
        public string UnifiedSocialCreditCodeScannPhoto { get; set; }
        /// <summary>
        /// 统一社会信用证图片对象
        /// </summary>
        public BlobInfoDTO UnifiedSocialCreditCodeScannPhotoBlob
        {
            get
            {
                if (UnifiedSocialCreditCodeScannPhoto.IsNullOrEmpty())
                    return null;

                return SerializeHelper.FromJson<BlobInfoDTO>(UnifiedSocialCreditCodeScannPhoto);
            }
        }

        /// <summary>
        /// 企业授权书图片
        /// </summary>
        [DataMember]
        [MaxLength(500)]
        public string LetterOfAuthorityScannPhoto { get; set; }

        /// <summary>
        /// 企业授权书图片对象
        /// </summary>
        [DataMember]
        public BlobInfoDTO LetterOfAuthorityScannPhotoBlob
        {
            get
            {
                if (LetterOfAuthorityScannPhoto.IsNullOrEmpty())
                    return null;

                return SerializeHelper.FromJson<BlobInfoDTO>(LetterOfAuthorityScannPhoto);
            }
        }

        /// <summary>
        /// 法人姓名
        /// </summary>
        [DataMember]
        [MaxLength(50)]
        [DisplayName("法人姓名")]
        public string LegalPerson { get; set; }
        /// <summary>
        /// 法人身份证图片对象（法人手持身份证）Json字符串
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
        /// 法人身份证图片（正面）Json字符串
        /// </summary>
        [DataMember]
        [MaxLength(500)]
        public string LegalPersonIdentityCardPhoto { get; set; }
        /// <summary>
        /// 法人身份证图片（正面）对象
        /// </summary>
        public BlobInfoDTO LegalPersonIdentityCardPhotoBlob
        {
            get
            {
                if (LegalPersonIdentityCardPhoto.IsNullOrEmpty())
                    return null;

                return SerializeHelper.FromJson<BlobInfoDTO>(LegalPersonIdentityCardPhoto);
            }
        }
        /// <summary>
        /// 法人身份证图片（背面）Json字符串
        /// </summary>
        [DataMember]
        [MaxLength(500)]
        public string LegalPersonIdentityCardPhotoOtherSide { get; set; }
        /// <summary>
        /// 法人身份证图片（正面）对象
        /// </summary>
        public BlobInfoDTO LegalPersonIdentityCardPhotoOtherSideBlob
        {
            get
            {
                if (LegalPersonIdentityCardPhotoOtherSide.IsNullOrEmpty())
                    return null;

                return SerializeHelper.FromJson<BlobInfoDTO>(LegalPersonIdentityCardPhotoOtherSide);
            }
        }
        /// <summary>
        /// 法人身份证过期时间
        /// </summary>
        [DataMember]
        [DisplayName("法人身份证过期时间")]
        public DateTime? LegalPersonCertificateExpiryDate { get; set; }

        public int CustomerId { get; set; }
        public CustomerInfoDTO CustomerInfo { get; set; }
    }
}
