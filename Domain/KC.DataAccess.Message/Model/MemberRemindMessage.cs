using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Model.Message.Constants;
using KC.Framework.Base;
using KC.Enums.Message;

namespace KC.Model.Message
{
    [Table(Tables.MemberRemindMessage)]
    public class MemberRemindMessage : Entity
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(200)]
        public string MessageTitle { get; set; }
        public string MessageContent { get; set; }

        [Display(Name = "消息模板Id")]
        public int TypeId { get; set; }
        [Display(Name = "消息模板名称")]
        [MaxLength(50)]
        public string TypeName { get; set; }

        public MessageStatus Status { get; set; }

        [MaxLength(128)]
        public string UserId { get; set; }
        [MaxLength(50)]
        public string UserName { get; set; }
        public DateTime? ReadDate { get; set; }

        public Guid ApplicationId { get; set; }
        public string ApplicationName { get; set; }

    }
}
