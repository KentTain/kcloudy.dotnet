using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Model.Customer.Constants;

namespace KC.Model.Customer
{
    [Table(Tables.CustomerContact)]
    public class CustomerContact : Entity
    {
        [Key]
        public int Id { get; set; }

        public BusinessType BusinessType { get; set; }

        /// <summary>
        /// 联系人姓名
        /// </summary>
        [MaxLength(50)]
        [DataMember]
        public string ContactName { get; set; }

        /// <summary>
        /// 联系人QQ
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string ContactQQ { get; set; }

        /// <summary>
        /// 联系人微信
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string ContactWeixin { get; set; }

        /// <summary>
        /// 联系人邮箱
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string ContactEmail { get; set; }

        /// <summary>
        /// 联系人手机
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string ContactPhoneNumber { get; set; }

        /// <summary>
        /// 联系人电话
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string ContactTelephone { get; set; }

        /// <summary>
        /// 联系人职位
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string PositionName { get; set; }

        [DataMember]
        public bool IsDefault { get; set; }

        /// <summary>
        /// 客户内部引用Id
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
        [DataMember]
        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public CustomerInfo CustomerInfo { get; set; }
    }
}
