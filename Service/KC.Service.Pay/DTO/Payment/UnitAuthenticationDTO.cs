using KC.Enums.Pay;
using KC.Framework.Extension;
using KC.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO.Pay
{
    public class UnitAuthenticationDTO : EntityDTO
    {
        [DataMember]
        public string MemberId { get; set; }
        [DataMember]
        public string CompanyName { get; set; }
        [DataMember]
        public string CompanyEmail { get; set; }
        [DataMember]
        public string CompanyAddress { get; set; }
        [DataMember]
        public string ZipCode { get; set; }
        [DataMember]
        public string BusinessLicenseNumber { get; set; }
        [DataMember]
        public int? Province { get; set; }
        [DataMember]
        public int? CityId { get; set; }
        [DataMember]
        public DateTime? BulidDate { get; set; }
        [DataMember]
        public int? BusinessDateLimit { get; set; }
        [DataMember]
        public bool? IsLongTerm { get; set; }
        [DataMember]
        public string ScopeOfBusiness { get; set; }
        [DataMember]
        public string BusinessLicenseScannPhoto { get; set; }
        [DataMember]
        public decimal RegisteredCapital { get; set; }
        [DataMember]
        public string OrganizationCode { get; set; }
        [DataMember]
        public string OrganizationCodeScannPhoto { get; set; }
        [DataMember]
        public string LegalPerson { get; set; }
        [DataMember]
        public string LegalPersonIdentityCardNumber { get; set; }
        [DataMember]
        public string LegalPersonIdentityCardScannPhoto { get; set; }
        [DataMember]
        public string LegalPersonIdentityCardPhoto { get; set; }
        [DataMember]
        public string LegalPersonIdentityCardPhotoOtherSide { get; set; }
        [DataMember]
        public string ManagersPerson { get; set; }
        [DataMember]
        public string ManagersPersonIdentityCardNumber { get; set; }
        //public string ManagersPersonIdentityCardPhoto { get; set; }
        [DataMember]
        public int ManagersCertificateType { get; set; }
        [DataMember]
        public string ContactJob { get; set; }
        [DataMember]
        public string ContactPhone { get; set; }
        [DataMember]
        public string ContactName { get; set; }
        [DataMember]
        public int? MembershipLevel { get; set; }
        [DataMember]
        public string CreditRating { get; set; }
        [DataMember]
        public string CustomerManager { get; set; }
        [DataMember]
        public AuditStatus AuditStatus { get; set; }
        [DataMember]
        public string AuditStatusString
        {
            get
            {
                return AuditStatus.ToDescription();
            }
        }
        [DataMember]
        public AuditStatus? ReviewStatus { get; set; }
        [DataMember]
        public string ReviewStatusString
        {
            get
            {
                if (ReviewStatus != null)
                    return ReviewStatus.Value.ToDescription();
                return string.Empty;
            }
        }
        [DataMember]
        public string UnifiedSocialCreditCode { get; set; }
        [DataMember]
        public int? CurrencyCode { get; set; }
        [DataMember]
        public int? CertificateType { get; set; }
        [DataMember]
        public string CompanyPhone { get; set; }
        [DataMember]
        public string LegalPersonMobliePhone { get; set; }
        [DataMember]
        public string ManagerIdentityCardPhoto { get; set; }
        [DataMember]
        public string ManagerIdentityCardPhotoOtherSide { get; set; }
        [DataMember]
        public DateTime? LegalPersonCertificateExpiryDate { get; set; }
        [DataMember]
        public string NickName { get; set; }
    }
}
