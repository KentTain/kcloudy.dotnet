using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO.Pay
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class BankAuthenticationApplicationDTO : PayBaseParamDTO
    {
        /// <summary>
        /// 银行表
        /// </summary>
        [DataMember]
        public BankAccountDTO BankAccount { get; set; }

        /// <summary>
        /// 支付流水
        /// </summary>
        [DataMember]
        public PaymentTradeRecordDTO PaymentTradeRecord { get; set; }

        /// <summary>
        /// 银行卡Id
        /// </summary>
        [DataMember]
        [Required]

        public int BankAccountId { get; set; }
    }
}
