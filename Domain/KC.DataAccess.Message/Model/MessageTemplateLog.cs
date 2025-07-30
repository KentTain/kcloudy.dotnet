using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.Message;
using KC.Model.Message.Constants;
using KC.Framework.Base;

namespace KC.Model.Message
{
    [Table(Tables.MessageTemplateLog)]
    public class MessageTemplateLog: ProcessLogBase
    {
        public int TemplateId { get; set; }

        public MessageTemplateType TemplateType { get; set; }

        /// <summary>
        /// 模版名称
        /// </summary>
        [MaxLength(128)]
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
