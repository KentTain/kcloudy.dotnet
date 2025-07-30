using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using KC.Framework.Extension;
using KC.Enums.Message;
using KC.Service.DTO.Message;
using KC.Service.DTO;
using ProtoBuf;

namespace KC.Service.DTO.Message
{
    public class MessageClassDTO : EntityDTO
    {
        public MessageClassDTO()
        {
            MessageTemplates = new List<MessageTemplateDTO>();
        
        }

        [Key]
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public MessageType Type { get; set; }
        [DataMember]
        public string TypeStr
        {
            get { return Type.ToDescription(); }
        }

        [MaxLength(128)]
        [DataMember]
        public string Name { get; set; }

        [MaxLength(20)]
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public int Index { get; set; }
        [DataMember]
        public string Desc { get; set; }
        [DataMember]
        public Guid ApplicationId { get; set; }
   
        public string ApplicationName { get; set; }
        [DataMember]
        [ProtoMember(34)]
        public int? MessageCategoryId { get; set; }
        [DataMember]
        [ProtoMember(35)]
        public  MessageCategoryDTO MessageCategory { get; set; }
        [DataMember]
        public virtual List<MessageTemplateDTO> MessageTemplates { get; set; }

  
    }
}
