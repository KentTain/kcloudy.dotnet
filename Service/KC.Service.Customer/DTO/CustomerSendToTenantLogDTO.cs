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
    public class CustomerSendToTenantLogDTO : ProcessLogBaseDTO
    {
        [DataMember]
        public string TenantName { get; set; }
        [DataMember]
        public string TenantDisplayName { get; set; }
        [DataMember]
        public string CustomerIdList { get; set; }
    }
}
