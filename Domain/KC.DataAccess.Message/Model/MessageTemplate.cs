using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.Message;
using KC.Model.Message.Constants;
using KC.Framework.Base;

namespace KC.Model.Message
{
    [Table(Tables.MessageTemplate)]
    public class MessageTemplate : Entity
    {
        [Key]
        public int Id { get; set; }

        public MessageTemplateType TemplateType { get; set; }

        /// <summary>
        /// 模版名称
        /// </summary>
        [MaxLength(128)]
        public string Name { get; set; }

        /// <summary>
        /// 模版主题
        /// </summary>
        [MaxLength(256)]
        public string Subject { get; set; }

        /// <summary>
        /// 模版内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 消息类别
        /// </summary>
        public int MessageClassId { get; set; }
        [ForeignKey("MessageClassId")]
        public virtual MessageClass MessageClass { get; set; }
    }
}
