using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO.Pay
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class OpenAccountDTO : CPCNBaseDTO
    {
        /// <summary>
        /// 中金支付资金账户表
        /// </summary>
        [DataMember]
        public PaymentBankAccountDTO CPCNBankAccount { get; set; }

        /// <summary>
        /// 支付信息表
        /// </summary>
        [DataMember]
        public PaymentInfoDTO PaymentInfo { get; set; }
    }
}
