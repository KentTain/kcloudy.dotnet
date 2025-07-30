using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models
{
    public class OrderPayParam
    {
        /// <summary>
        /// 租户Id
        /// </summary>
        public string MemberId { get; set; }

        /// <summary>
        /// 订单Id
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 支付订单Id
        /// </summary>
        public string PaymentOrderId { get; set; }

        /// <summary>
        /// 手续费订单Id
        /// </summary>
        public string FeeOrderId { get; set; }
    }
}
