using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Service.DTO;

namespace KC.Service.DTO.Customer
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class CustomerManagerDTO : EntityBaseDTO
    {
        [DataMember]
        public int Id { get; set; }

        [MaxLength(128)]
        [DataMember]
        public string CustomerManagerId { get; set; }

        [MaxLength(512)]
        [DataMember]
        public string CustomerManagerName { get; set; }

        [DataMember]
        public int OrganizationId { get; set; }

        [MaxLength(256)]
        [DataMember]
        public string OrganizationName { get; set; }

        [DataMember]
        public int CustomerId { get; set; }

        /// <summary>
        /// 是否被移入了公海
        /// </summary>
        [DataMember]
        public bool? IsInSeas { get; set; }

    }
}