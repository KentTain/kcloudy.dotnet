using KC.Enums.Pay;
using KC.Framework.Base;
using KC.Model.Pay.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Model.Pay
{
    [Table(Tables.EntrustedPaymentRecord)]
    public class EntrustedPaymentRecord : Entity
    {
        [Key]
        [DataMember]
        public int Id { get; set; }

        [MaxLength(128)]
        [DataMember]
        public string OrderId { get; set; }

        [MaxLength(128)]
        [DataMember]
        public string PayableNumber { get; set; }

        [DataMember]
        public decimal PaymentAmount { get; set; }

        /// <summary>
        /// 卖方
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        public string SellerTenantName { get; set; }

        /// <summary>
        /// 卖方
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        public string Seller { get; set; }

        /// <summary>
        /// 授信方tenantName
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        public string GuaranteeTenantName { get; set; }
        /// <summary>
        /// 授信方名称
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        public string Guarantee { get; set; }

        [DataMember]
        public EntrustedPaymentStatus EntrustedPaymentStatus { get; set; }

        public string Remark { get; set; }
    }
}
