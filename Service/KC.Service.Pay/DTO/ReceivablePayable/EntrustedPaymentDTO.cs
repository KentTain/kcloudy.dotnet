
using KC.Framework.Extension;
using KC.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.Pay;

namespace KC.Service.DTO.Pay
{
    public class EntrustedPaymentDTO : EntityDTO
    {
        public int Id { get; set; }

        public string OrderId { get; set; }

        public string PayableNumber { get; set; }

        public decimal PaymentAmount { get; set; }

        /// <summary>
        /// 卖方
        /// </summary>
        public string SellerTenantName { get; set; }

        /// <summary>
        /// 卖方
        /// </summary>
        public string Seller { get; set; }

        /// <summary>
        /// 授信方tenantName
        /// </summary>
        public string GuaranteeTenantName { get; set; }
        /// <summary>
        /// 授信方名称
        /// </summary>

        public string Guarantee { get; set; }

        public EntrustedPaymentStatus EntrustedPaymentStatus { get; set; }

        public string Status { get { return EntrustedPaymentStatus.ToDescription(); } }

        public string Remark { get; set; }
    }
}
