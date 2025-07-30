using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using KC.Framework.Extension;
using KC.Service.DTO.Message;
using KC.Service.DTO;
using ProtoBuf;
using KC.Framework.Tenant;

namespace KC.Service.DTO.Message
{
    [Serializable, DataContract(IsReference = true)]
    public class MessageClassDTO : EntityDTO
    {
        public MessageClassDTO()
        {
            MessageTemplates = new List<MessageTemplateDTO>();
        
        }
        [DataMember]
        public bool IsEditMode { get; set; }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public BusinessType Type { get; set; }
        [DataMember]
        public string TypeStr
        {
            get { return Type.ToDescription(); }
        }

        [DataMember]
        [MaxLength(128)]
        public string Name { get; set; }

        [DataMember]
        [MaxLength(20)]
        public string Code { get; set; }
        [DataMember]
        [MaxLength(1000)]
        public string ReplaceParametersString { get; set; }
        [DataMember]
        public int Index { get; set; }
        [DataMember]
        public string Desc { get; set; }
        [DataMember]
        public Guid ApplicationId { get; set; }
        [DataMember]
        public string ApplicationName { get; set; }
        [DataMember]
        public int? MessageCategoryId { get; set; }
        [DataMember]
        public  string MessageCategoryName { get; set; }

        [DataMember]
        public bool HasSmsTemplate
        {
            get { return MessageTemplates.Any(m => m.TemplateType == Enums.Message.MessageTemplateType.SmsMessage); }
        }
        [DataMember]
        public bool HasCommomTemplate
        {
            get { return MessageTemplates.Count(m => m.TemplateType == Enums.Message.MessageTemplateType.EmailMessage || m.TemplateType == Enums.Message.MessageTemplateType.InsideMessage) >= 2; }
        }

        [DataMember]
        public List<MessageTemplateDTO> MessageTemplates { get; set; }

  
    }
}
