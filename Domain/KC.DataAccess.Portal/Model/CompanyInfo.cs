using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using KC.Framework.Base;
using KC.Model.Portal.Constants;

namespace KC.Model.Portal
{
    [Table(Tables.CompanyInfo)]
    public class CompanyInfo : Entity
    {
        public CompanyInfo()
        {
            CompanyContacts = new List<CompanyContact>();
            CompanyAddresses = new List<CompanyAddress>();
            CompanyAccounts = new List<CompanyAccount>();
        }

        /// <summary>
        /// 企业编码：TenantId
        /// </summary>
        [Key]
        [MaxLength(20)]
        public string CompanyCode { get; set; }
        /// <summary>
        /// 企业名称
        /// </summary>
        [MaxLength(512)]
        public string CompanyName { get; set; }

        /// <summary>
        /// 企业Logo
        /// </summary>
        [MaxLength(1000)]
        public string CompanyLogo { get; set; }
        /// <summary>
        /// 企业经营模式
        /// </summary>
        public KC.Enums.Portal.BusinessModel BusinessModel { get; set; }

        [MaxLength(128)]
        public string IndustryId { get; set; }

        [MaxLength(1024)]
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
        /// 客户内部引用Id：TenantId
        /// </summary>
        [DataMember]
        public string ReferenceId { get; set; }
        /// <summary>
        /// 引用Id2
        /// </summary>
        [DataMember]
        public string ReferenceId2 { get; set; }
        /// <summary>
        /// 引用Id3
        /// </summary>
        [DataMember]
        public string ReferenceId3 { get; set; }

        /// <summary>
        /// 客户简介
        /// </summary>
        [MaxLength(4000)]
        public string Introduction { get; set; }

        public CompanyAuthentication AuthenticationInfo { get; set; }

        /// <summary>
        /// 联系人列表
        /// </summary>
        public virtual ICollection<CompanyContact> CompanyContacts { get; set; }
         /// <summary>
        /// 地址
        /// </summary>
        public virtual ICollection<CompanyAddress> CompanyAddresses { get; set; }
        /// <summary>
        /// 银行账号
        /// </summary>
        public virtual ICollection<CompanyAccount> CompanyAccounts { get; set; }
    }
}
