using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Extension;
using KC.Enums.CRM;
using KC.Service.DTO;
using KC.Framework.Base;

namespace KC.Service.DTO.Customer
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class CustomerInfoDTO : EntityDTO
    {
        public CustomerInfoDTO()
        {
            CompanyType = CompanyType.Supplier;
            CustomerAddresses = new List<CustomerAddressDTO>();
            CustomerAccounts = new List<CustomerAccountDTO>();
            CustomerContacts = new List<CustomerContactDTO>();
            CustomerExtInfos = new List<CustomerExtInfoDTO>();
            CustomerChangeLogs = new List<CustomerChangeLogDTO>();
            CustomerTracingLogs = new List<CustomerTracingLogDTO>();
            CustomerManagers = new List<CustomerManagerDTO>();
        }

        [DataMember]
        public int CustomerId { get; set; }

        [MaxLength(128)]
        [DataMember]
        [DisplayName("企业代码")]
        public string CustomerCode { get; set; }

        [MaxLength(500)]
        [DataMember]
        [DisplayName("企业名称")]
       
        public string CustomerName { get; set; }

        [DataMember]
        [DisplayName("企业类型")]
        public CompanyType CompanyType { get; set; }

        public string CompanyTypeStr
        {
            get { return CompanyType.ToDescription(); }
        }

        [DataMember]
        [DisplayName("企业分类")]
        public ClientType ClientType { get; set; }

        public string ClientTypeStr
        {
            get { return ClientType.ToDescription(); }
        }

        [DataMember]
        [DisplayName("企业来源")]
        public CustomerSource CustomerSource { get; set; }

        public string CustomerSourceStr
        {
            get { return CustomerSource.ToDescription(); }
        }

        /// <summary>
        /// 企业经营模式
        /// </summary>
        [DataMember]
        public Framework.Base.BusinessModel BusinessModel { get; set; }

        [DataMember]
        public string BusinessModelStr
        {
            get { return BusinessModel.ToDescription(); }
        }

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
        /// 企业内部引用Id：TenantId
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
        /// 企业简介
        /// </summary>
        [DataMember]
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

        /// <summary>
        /// 认证数据
        /// </summary>
        public CustomerAuthenticationDTO AuthenticationInfo { get; set; }

        /// <summary>
        /// 联系人列表
        /// </summary>
        [DataMember]
        public List<CustomerContactDTO> CustomerContacts { get; set; }
        /// <summary>
        /// 地址列表
        /// </summary>
        [DataMember]
        public List<CustomerAddressDTO> CustomerAddresses { get; set; }
        /// <summary>
        /// 银行账户列表
        /// </summary>
        [DataMember]
        public List<CustomerAccountDTO> CustomerAccounts { get; set; }
        /// <summary>
        /// 扩展属性
        /// </summary>
        [DataMember]
        public List<CustomerExtInfoDTO> CustomerExtInfos { get; set; }
        /// <summary>
        /// 用户信息变更日志
        /// </summary>
        [DataMember]
        public List<CustomerChangeLogDTO> CustomerChangeLogs { get; set; }
        /// <summary>
        /// 用户跟踪日志
        /// </summary>
        [DataMember]
        public List<CustomerTracingLogDTO> CustomerTracingLogs { get; set; }
        /// <summary>
        /// 企业经理
        /// </summary>
        [DataMember]
        public List<CustomerManagerDTO> CustomerManagers { get; set; }
    }
}
