using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Service.DTO;

namespace KC.Service.DTO.Message
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class MemberRemindMessageDTO : EntityDTO
    {
        [Display(Name = "Id")]
        [DataMember]
        public int Id { get; set; }

        [Display(Name = "标题")]
        [DataMember]
        public string MessageTitle { get; set; }
        [Display(Name = "内容")]
        [DataMember]
        public string MessageContent { get; set; }
        [Display(Name = "类型Id")]
        [DataMember]
        public int TypeId { get; set; }
        [Display(Name = "类型名称")]
        [DataMember]
        public string TypeName { get; set; }
        [Display(Name = "状态")]
        [DataMember]
        public List<MemberMessageStatusDTO> MemberMessageStatuses { get; set; }
        [Display(Name = "ApplicationId")]
        [DataMember]
        public Guid ApplicationId { get; set; }

    }

    [Serializable]
    [DataContract(IsReference = true)]
    public class RemindMessageApiDTO : EntityBaseDTO
    {
        public RemindMessageApiDTO()
        {
            SendUserIds = new List<string>();
        }
        [DataMember]
        public MemberRemindMessageDTO Data { get; set; }
        [DataMember]
        public List<string> SendUserIds { get; set; }
    }
}
