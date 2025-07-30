using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using KC.Enums.Portal;
using KC.Service.DTO;
using KC.Framework.Extension;
using KC.Framework.Base;
using KC.Common;

namespace KC.Service.DTO.Portal
{
    [Serializable, DataContract(IsReference = true)]
    public class CompanyInfoDTO : EntityDTO
    {
        public CompanyInfoDTO()
        {
            CompanyContacts = new List<CompanyContactDTO>();
            CompanyAccounts = new List<CompanyAccountDTO>();
            CompanyAddresses = new List<CompanyAddressDTO>();
        }

        [DataMember]
        public bool IsEditMode { get; set; }

        [DataMember]
        [MaxLength(20)]
        public string CompanyCode { get; set; }

        [DataMember]
        [MaxLength(512)]
        [Required(ErrorMessage = "企业名称不能为空！")]
        public string CompanyName { get; set; }

        /// <summary>
        /// 企业Logo
        /// </summary>
        [MaxLength(1000)]
        [DataMember]
        public string CompanyLogo { get; set; }

        /// <summary>
        /// 租户Logo对象
        /// </summary>
        [DataMember]
        public BlobInfoDTO CompanyLogoBlob
        {
            get
            {
                if (CompanyLogo.IsNullOrEmpty())
                    return null;

                return SerializeHelper.FromJson<BlobInfoDTO>(CompanyLogo);
            }
        }
        [DataMember]
        public KC.Enums.Portal.BusinessModel BusinessModel { get; set; }

        [DataMember]
        public string BusinessModelString { get { return BusinessModel.ToDescription(); } }

        [DataMember]
        [MaxLength(128)]
        public string IndustryId { get; set; }

        [DataMember]
        [MaxLength(512)]
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

        [DataMember]
        public string ContactPhone { get; set; }

        [DataMember]
        public string ContactEmail { get; set; }

        /// <summary>
        /// 外部编号1
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "外部编号1")]
        [DataMember]
        public string ReferenceId { get; set; }

        /// <summary>
        /// 外部编号2
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "外部编号2")]
        [DataMember]
        public string ReferenceId2 { get; set; }
        /// <summary>
        /// 外部编号3
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "外部编号3")]
        [DataMember]
        public string ReferenceId3 { get; set; }

        /// <summary>
        /// 客户简介
        /// </summary>
        [DataMember]
        public string Introduction { get; set; }

        [DataMember]
        public CompanyAuthenticationDTO AuthenticationInfo { get; set; }

        [DataMember]
        public ICollection<CompanyContactDTO> CompanyContacts { get; set; }
        [DataMember]
        public ICollection<CompanyAccountDTO> CompanyAccounts { get; set; }
        [DataMember]
        public ICollection<CompanyAddressDTO> CompanyAddresses { get; set; }
    }
}
