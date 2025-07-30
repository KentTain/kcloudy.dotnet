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
    public class BankAuthenticationDTO : BankAuthenticationApplicationDTO
    {
        [DataMember]
        [Required]
        public decimal Amount { get; set; }
    }
}
