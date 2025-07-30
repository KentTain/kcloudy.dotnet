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
using ProtoBuf;
using KC.Framework.Tenant;

namespace KC.Model.Message
{
    [Table(Tables.MessageClass)]
    public class MessageClass : Entity
    {
        public MessageClass()
        {
            MessageTemplates = new List<MessageTemplate>();
        }

        [Key]
        public int Id { get; set; }

        public BusinessType Type { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }

        [MaxLength(20)]
        public string Code { get; set; }

        [MaxLength(1000)]
        public string ReplaceParametersString { get; set; }

        public int Index { get; set; }

        [MaxLength(4000)]
        public string Desc { get; set; }

        public Guid ApplicationId { get; set; }
        public string ApplicationName { get; set; }

        [DataMember]
        public int? MessageCategoryId { get; set; }

        [DataMember]
        [ForeignKey("MessageCategoryId")]
        public  MessageCategory MessageCategory { get; set; }
        public virtual ICollection<MessageTemplate> MessageTemplates { get; set; }
    }
}
