using KC.Framework.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Model.Pay
{
    [Serializable, DataContract(IsReference = true)]
    public abstract class PaymentFlow : Entity
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

    }
}
