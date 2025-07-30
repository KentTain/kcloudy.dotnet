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
    public class PaymentFlowRecordsDTO : EntityDTO
    {
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [DataMember]
        public string MemberId { get; set; }


        [DataMember]
        public int PaymentAuditStatusId { get; set; }

        /// <summary>
        /// 支付状态Id
        /// </summary>
        public PaymentAuditStatusDTO PaymentAuditStatus { get; set; }

        /// <summary>
        /// 操作状态
        /// </summary>
        [DataMember]
        public PaymentState OperationState { get; set; }

    }
}
