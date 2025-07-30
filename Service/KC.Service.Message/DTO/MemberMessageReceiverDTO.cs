using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Extension;
using KC.Enums.Message;
using KC.Service.DTO;


namespace KC.Service.DTO.Message
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class MemberMessageReceiverDTO : EntityBaseDTO
    {
        [Display(Name = "Id")]
        [DataMember]
        public int Id { get; set; }
        [Display(Name = "状态")]
        [DataMember]
        public MessageStatus Status { get; set; }
        [Display(Name = "阅读时间")]
        [DataMember]
        public DateTime? ReadDate { get; set; }
        [Display(Name = "UserId")]
        [DataMember]
        [MaxLength(128)]
        public string UserId { get; set; }
        [Display(Name = "姓名")]
        [DataMember]
        [MaxLength(50)]
        public string UserName { get; set; }
        [Display(Name = "消息ID")]
        [DataMember]
        public int MessageId { get; set; }
        [Display(Name = "标题")]
        [DataMember]
        [MaxLength(200)]
        public string Title { get; set; }
        [Display(Name = "类型名称")]
        [DataMember]
        [MaxLength(50)]
        public string TypeName { get; set; }
        [Display(Name = "消息")]
        [DataMember]
        public string MemberRemindMessage { get; set; }

        public Guid ApplicationId { get; set; }
        public string ApplicationName { get; set; }
    }
}
