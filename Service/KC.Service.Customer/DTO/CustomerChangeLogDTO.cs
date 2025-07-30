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
    public class CustomerChangeLogDTO : EntityBaseDTO
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string AttributeName { get; set; }
        [DataMember]
        public string OldValue { get; set; }
        [DataMember]
        public string NewValue { get; set; }
        [DataMember]
        [MaxLength(128)]
        public string OperatorId { get; set; }
        [MaxLength(50)]
        [DataMember]
        public string Operator { get; set; }
        [DataMember]
        public System.DateTime OperateDate { get; set; }
        [DataMember]
        public int CustomerId { get; set; }
        [MaxLength(50)]
        [DataMember]
        public string CustomerName { get; set; }
    }
}
