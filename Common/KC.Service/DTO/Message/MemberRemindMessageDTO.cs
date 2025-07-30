using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Extension;
using KC.Service.DTO;
using KC.Service.Enums.Message;

namespace KC.Service.DTO.Message
{
    [Serializable, DataContract(IsReference = true)]
    public class MemberRemindMessageDTO : EntityDTO
    {
        [Display(Name = "Id")]
        [DataMember]
        public int Id { get; set; }

        [Display(Name = "标题")]
        [DataMember]
        [MaxLength(200)]
        public string MessageTitle { get; set; }
        [Display(Name = "内容")]

        [DataMember]
        public string MessageContent { get; set; }
        [Display(Name = "消息模板Id")]
        [DataMember]
        public int TypeId { get; set; }
        
        [Display(Name = "消息模板名称")]
        [DataMember]
        [MaxLength(50)]
        public string TypeName { get; set; }

        [Display(Name = "阅读状态")]
        [DataMember]
        public MessageStatus Status { get; set; }
        [DataMember]
        public string StatusString { get { return Status.ToDescription(); } }

        [Display(Name = "阅读人")]
        [DataMember]
        [MaxLength(128)]
        public string UserId { get; set; }

        [Display(Name = "阅读人")]
        [DataMember]
        [MaxLength(50)]
        public string UserName { get; set; }

        [Display(Name = "阅读时间")]
        [DataMember]
        public DateTime? ReadDate { get; set; }

        public Guid ApplicationId { get; set; }
        public string ApplicationName { get; set; }
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
