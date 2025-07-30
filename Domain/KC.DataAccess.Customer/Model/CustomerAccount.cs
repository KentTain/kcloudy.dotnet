using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.Customer;
using KC.Framework.Base;
using KC.Model.Customer.Constants;

namespace KC.Model.Customer
{
    [Table(Tables.CustomerAccount)]
    public class CustomerAccount : Entity
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        [DataMember]
        public int Id { get; set; }
        /// <summary>
        /// 银行账户类型
        /// </summary>
        public BankAccountType BankType { get; set; }

        /// <summary>
        /// 开户名
        /// </summary>
        [MaxLength(1024)]
        public string AccountName { get; set; }
        /// <summary>
        /// 开户银行
        /// </summary>
        [MaxLength(512)]
        public string BankName { get; set; }
        /// <summary>
        /// 开户银行帐号
        /// </summary>
        [MaxLength(128)]
        public string BankNumber { get; set; }

        /// <summary>
        /// 省份Id
        /// </summary>
        [DataMember]
        public int ProvinceId { get; set; }
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
        public int CityId { get; set; }
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
        public int DistrictId { get; set; }
        /// <summary>
        /// 区域名
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string DistrictName { get; set; }
        /// <summary>
        /// 街道Id
        /// </summary>
        [DataMember]
        public int StreetId { get; set; }
        /// <summary>
        /// 街道名
        /// </summary>
        [MaxLength(256)]
        [DataMember]
        public string StreetName { get; set; }

        /// <summary>
        /// 开户行地址
        /// </summary>
        [MaxLength(1024)]
        public string BankAddress { get; set; }

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
        /// 客户内部引用Id
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        /// 引用Id2
        /// </summary>
        public string ReferenceId2 { get; set; }

        /// <summary>
        /// 引用Id3
        /// </summary>
        public string ReferenceId3 { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(512)]
        public string Remark { get; set; }

        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public CustomerInfo CustomerInfo { get; set; }
    }
}
