using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Service.DTO;
using KC.Service.Enums.Message;

namespace KC.Service.DTO.Message
{
    [Serializable, DataContract(IsReference = true)]
    public class MessageLogDTO : EntityDTO
    {
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// 接收人邮箱
        /// </summary>
        [DataMember]
        [MaxLength(1000)]
        public string ToAddresses { get; set; }

        /// <summary>
        /// 抄送人邮箱
        /// </summary>
        [DataMember]
        [MaxLength(1000)]
        public string CCAddresses { get; set; }

        /// <summary>
        /// 邮件主题
        /// </summary>
        [DataMember]
        [MaxLength(1000)]
        public string Subject { get; set; }

        /// <summary>
        /// 邮件内容
        /// </summary>
        [DataMember]
        public string Content { get; set; }

        /// <summary>
        /// 邮件模版类型
        /// </summary>
        [DataMember]
        public MessageTemplateType Type { get; set; }

        [DataMember]
        public bool IsSuccess { get; set; }

        [DataMember]
        public string ExceptionMessage { get; set; }
    }
}
