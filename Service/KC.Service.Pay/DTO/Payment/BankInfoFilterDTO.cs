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
    public class BankInfoFilterDTO : PayBaseParamDTO
    {
        [DataMember]
        [Required]
        public string BankId { get; set; }

        [DataMember]
        public string QryFlag { get; set; }
        [DataMember]
        public string OpenBankCode { get; set; }

        [DataMember]
        public string OpenBankName { get; set; }

        [DataMember]
        public string CityCode { get; set; }

        [DataMember]
        public int QueryNum { get; set; }
    }
}
