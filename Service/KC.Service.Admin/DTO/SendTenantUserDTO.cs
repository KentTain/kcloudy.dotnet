using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using System.ComponentModel.DataAnnotations;
using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Common;

namespace KC.Service.DTO.Admin
{
    [Serializable, DataContract(IsReference = true)]
    public class SendTenantUserDTO : EntityBaseDTO
    {
        public SendTenantUserDTO()
        {
            Version = Framework.Tenant.TenantVersion.Standard;
            CloudType = CloudType.Azure;
            OwnApplications = new List<SendApplcationDTO>();
        }
        [DataMember]
        public string UserId { get; set; }
        [DataMember]
        public string MemberId { get; set; }
        [DataMember]
        public string DisplayName { get; set; }
        [DataMember]
        public string Signature { get; set; }

        /// <summary>
        ///  租户Logo
        /// </summary>
        [DataMember]
        public string TenantLogo { get; set; }

        /// <summary>
        /// 租户Logo对象
        /// </summary>

        [DataMember]
        public BlobInfoDTO TenantLogoBlob
        {
            get
            {
                if (TenantLogo.IsNullOrEmpty())
                    return null;

                return SerializeHelper.FromJson<BlobInfoDTO>(TenantLogo);
            }
        }
        /// <summary>
        ///  租户简介
        /// </summary>
        [DataMember]
        public string TenantIntroduction { get; set; }

        [DataMember]
        public string ContactPhone { get; set; }
        [DataMember]
        public string ContactEmail { get; set; }
        [DataMember]
        public string ContactName { get; set; }
        [DataMember]
        public bool IsEnterprise { get; set; }
        [DataMember]
        public TenantVersion Version { get; set; }
        [DataMember]
        public CloudType CloudType { get; set; }
        [DataMember]
        public string OwnDomainName { get; set; }
        [DataMember]
        public DomainLevel OwnDomainLevel { get; set; }
        [DataMember]
        public string NickName { get; set; }
        /// <summary>
        /// 企业经营模式
        /// </summary>
        [DataMember]
        public BusinessModel BusinessModel { get; set; }

        [MaxLength(128)]
        [DataMember]
        public string IndustryId { get; set; }

        [MaxLength(1024)]
        [DataMember]
        public string IndustryName { get; set; }
        [DataMember]
        public List<SendApplcationDTO> OwnApplications { get; set; } 
    }

    [Serializable, DataContract(IsReference = true)]
    public class SendApplcationDTO : EntityBaseDTO
    {
        [DataMember]
        public string ApplicationId { get; set; }
        [DataMember]
        public string ApplicationDomain { get; set; }
        [DataMember]
        public string ApplicationName { get; set; }
    }
}
