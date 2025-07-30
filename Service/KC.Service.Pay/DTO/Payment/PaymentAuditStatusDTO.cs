using KC.Enums.Pay;
using KC.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO.Pay
{
    public class PaymentAuditStatusDTO : EntityDTO
    {
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [DataMember]

        public string MemberId { get; set; }

        [DataMember]//交易号
        public string PaymentOrderId { get; set; }

        [DataMember] //订单号
        public string OrderNo { get; set; }

        [DataMember]   //订单金额,单位分
        public decimal OrderAmount { get; set; }

        [DataMember] //支付金额
        public decimal PaymentAmount { get; set; }

        /// <summary>
        /// 支付订单类型
        /// </summary>
        [DataMember]
        public PaymentOrderType PaymentOrderType { get; set; }

        /// <summary>
        /// 支付状态 1 提交申请 2 审核中 3 审核通过 4 审核不通过 5 支付失败 6 支付成功
        /// </summary>
        [DataMember]
        public PaymentState State { get; set; }

    }
}
