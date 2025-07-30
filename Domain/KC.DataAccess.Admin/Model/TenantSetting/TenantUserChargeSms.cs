using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KC.Model.Admin.Constants;
using KC.Framework.Base;
using System.Runtime.Serialization;

namespace KC.Model.Admin
{
    [Table(Tables.TenantUserChargeSms)]
    [Serializable, DataContract(IsReference = true)]
    public class TenantUserChargeSms : Entity
    {
        public TenantUserChargeSms()
        {
            ChargeId = Guid.NewGuid();
        }

        [Key]
        [DataMember]
        public Guid ChargeId { get; set; }

        /// <summary>
        /// 来源租户编码
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string TenantName { get; set; }
        /// <summary>
        /// 来源租户名称
        /// </summary>
        [MaxLength(512)]
        [DataMember]
        public string TenantDisplayName { get; set; }

        /// <summary>
        /// 引用Id
        /// </summary>
        [MaxLength(50)]
        [DataMember]
        public string RefCode { get; set; }

        [DataMember]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [DataMember]
        public int ChargeNumber { get; set; }

        [DataMember]
        [MaxLength(2000)]
        public string Content { get; set; }

        [DataMember]
        public bool IsSuccess { get; set; }

        [DataMember]
        public string ErrorLog { get; set; }
    }
}
