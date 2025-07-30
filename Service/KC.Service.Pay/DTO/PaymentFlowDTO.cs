using KC.Service.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.DTO.Pay
{
    [Serializable, DataContract(IsReference = true)]
    public class PaymentFlowDTO : EntityDTO
    {
        /// <summary>
        /// 付款客户Id
        /// </summary>
        [MaxLength(128), DataMember]
        public string PaymentCustomerId { get; set; }

        /// <summary>
        /// 付款客户名称
        /// </summary>
        [MaxLength(128), DataMember]

        public string PaymentCustomerDisplayName { get; set; }

        /// <summary>
        /// 收款客户id
        /// </summary>
        [MaxLength(128), DataMember]
        public string PayeeCustomerId { get; set; }

        /// <summary>
        /// 收款客户名称
        /// </summary>
        [MaxLength(128), DataMember]
        public string PayeeCustomerDisplayName { get; set; }

        /// <summary>
        /// 产品id
        /// </summary>
        public int OfferingId { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        [MaxLength(256)]
        public string OfferingName { get; set; }

        /// <summary>
        /// 商城id
        /// </summary>
        public Guid ApplicationId { get; set; }
    }
}
