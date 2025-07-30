using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Service.DTO;

namespace KC.Service.DTO.Customer
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class CustomerContactDTO : EntityDTO
    {
        [DataMember]
        public int Id { get; set; }
        [MaxLength(50)]
        [DataMember]
        public string ContactName { get; set; }
        [MaxLength(20)]
        [DataMember]
        public string PositionName { get; set; }
        [MaxLength(20)]
        [DataMember]
        public string ContactQQ { get; set; }
        [MaxLength(100)]
        public string ContactEmail { get; set; }
        [MaxLength(20)]
        [DataMember]
        public string ContactPhoneMumber { get; set; }
        [MaxLength(20)]
        [DataMember]
        public string ContactTelephone { get; set; }
        [DataMember]
        public bool IsDefault { get; set; }
        [DataMember]
        public int CustomerId { get; set; }
    }
}
