using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Service.DTO;

namespace KC.Service.DTO.Customer
{
    [Serializable, DataContract(IsReference = true)]
    public class CustomerSeasDTO : EntityBaseDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DisplayName("客户ID")]
        [DataMember]
        public int CustomerId { get; set; }

        [DisplayName("操作人Id")]
        [DataMember]
        [MaxLength(128)]
        public string OperatorId { get; set; }

        [DisplayName("操作人")]
        [DataMember]
        [MaxLength(50)]
        public string Operator { get; set; }

        [DisplayName("转移时间")]
        [DataMember]
        public DateTime OperateDate { get; set; }

        [DisplayName("转移人部门")]
        [DataMember]
        public int OrganizationId { get; set; } 
    }
}
