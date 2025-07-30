using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Service.DTO;

namespace KC.Service.DTO.Account
{
    [Serializable, DataContract(IsReference = true)]
    public class UserApplicationDTO : EntityDTO
    {
        public UserApplicationDTO()
        {
            Id = Guid.NewGuid().ToString();
            Status = Framework.Base.WorkflowBusStatus.AuditPending;
        }
        [Key]
        [DataMember]
        public string Id { get; set; }

        [Key]
        [DataMember]
        public string UserName { get; set; }
        /// <summary>
        /// 用户显示名
        /// </summary>
        [MaxLength(512)]
        [DataMember]
        public string DisplayName { get; set; }
        [DataMember]
        public string Email { get; set; }
        [MaxLength(20)]
        [DataMember]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 微信公众号
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string OpenId { get; set; }
        [DataMember]
        public Framework.Base.WorkflowBusStatus Status { get; set; }
    }
}
