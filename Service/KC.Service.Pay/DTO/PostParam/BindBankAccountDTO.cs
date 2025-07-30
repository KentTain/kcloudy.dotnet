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
    public class BindBankAccountDTO : PayBaseParamDTO
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
        /// 第三方支付账户信息
        /// </summary>
        [DataMember]
        public PaymentBankAccountDTO PaymentBankAccount { get; set; }

        [DataMember]
        [Required]
        public int BankId { get; set; }

        /// <summary>
        /// 1绑定，3取消绑定
        /// </summary>
        [DataMember]
        public int BindState { get; set; }
    }
}
