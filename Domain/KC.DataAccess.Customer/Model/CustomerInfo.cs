using System;
using System.Collections.Generic;
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
    [Table(Tables.CustomerInfo)]
    public class CustomerInfo : Entity
    {
        public CustomerInfo()
        {
            CompanyType = CompanyType.Supplier;
            CustomerContacts = new List<CustomerContact>();
            CustomerAddresses = new List<CustomerAddress>();
            CustomerAccounts = new List<CustomerAccount>();
            CustomerExtInfos = new List<CustomerExtInfo>();
            CustomerChangeLogs = new List<CustomerChangeLog>();
            CustomerTracingLogs = new List<CustomerTracingLog>();
            CustomerManagers = new List<CustomerManager>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DataMember]
        public int CustomerId { get; set; }

        /// <summary>
        /// 客户代码：TenantId
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string CustomerCode { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        [MaxLength(1024)]
        [DataMember]
        public string CustomerName { get; set; }

        /// <summary>
        /// 企业类型
        /// </summary>
        [DataMember]
        public CompanyType CompanyType { get; set; }

        /// <summary>
        /// 客户分类
        /// </summary>
        [DataMember]
        public ClientType ClientType { get; set; }

        /// <summary>
        /// 客户来源
        /// </summary>
        [DataMember]
        public CustomerSource CustomerSource { get; set; }

        /// <summary>
        /// 企业经营模式
        /// </summary>
        [DataMember]
        public BusinessModel BusinessModel { get; set; }

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
        public string Introduction { get; set; }

        /// <summary>
        /// 推送用户Id
        /// </summary>
        [MaxLength(128)]
        public string RecommandedUserId { get; set; }

        /// <summary>
        /// 推送用户名称
        /// </summary>
        [MaxLength(512)]
        public string RecommandedUserName { get; set; }


        public CustomerAuthentication AuthenticationInfo { get; set; }

        /// <summary>
        /// 联系人列表
        /// </summary>
        public List<CustomerContact> CustomerContacts { get; set; }
        /// <summary>
        /// 地址列表
        /// </summary>
        public List<CustomerAddress> CustomerAddresses { get; set; }
        /// <summary>
        /// 银行账户列表
        /// </summary>
        public List<CustomerAccount> CustomerAccounts { get; set; }
        /// <summary>
        /// 扩展属性
        /// </summary>
        public List<CustomerExtInfo> CustomerExtInfos { get; set; }
        /// <summary>
        /// 用户信息变更日志
        /// </summary>
        public List<CustomerChangeLog> CustomerChangeLogs { get; set; }
        /// <summary>
        ///用户跟踪日志
        /// </summary>
        public List<CustomerTracingLog> CustomerTracingLogs { get; set; }
        /// <summary>
        /// 客户经理
        /// </summary>
        public List<CustomerManager> CustomerManagers { get; set; }
    }
}
