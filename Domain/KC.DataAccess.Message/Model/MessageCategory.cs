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
    [Table(Tables.MessageCategory)]
    public class MessageCategory : TreeNode<MessageCategory>
    {
        public MessageCategory()
        {
            MessageClasses = new List<MessageClass>();
        }
        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        [MaxLength(4000)]
        public string Description { get; set; }

        /// <summary>
        /// 所引用的菜单Id
        /// </summary>
        [DataMember]
        [MaxLength(50)]
        public string ReferenceId { get; set; }
        [DataMember]
        public List<MessageClass> MessageClasses { get; set; }

    }
}





