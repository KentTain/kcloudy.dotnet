using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.Message;
using KC.Service.DTO;
using ProtoBuf;

namespace KC.Service.DTO.Message
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class MessageTemplateLogDTO : ProcessLogBaseDTO
    {
        [DataMember]
        public int TemplateId { get; set; }

        [DataMember]
        public MessageTemplateType TemplateType { get; set; }

        /// <summary>
        /// 模版名称
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string TemplateName { get; set; }

        /// <summary>
        /// 模版主题
        /// </summary>
        [DataMember]
        [MaxLength(256)]
        public string TemplateSubject { get; set; }

        /// <summary>
        /// 模版内容
        /// </summary>
        [DataMember]
        public string TemplateContent { get; set; }
    }
}
