using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using KC.Service.DTO;
using ProtoBuf;

namespace KC.Service.DTO.Message
{
    [Serializable, DataContract(IsReference = true)]
    public class MessageCategoryDTO : TreeNodeDTO<MessageCategoryDTO>
    {
        public MessageCategoryDTO()
        {
            MessageClasses = new List<MessageClassDTO>();
            ReferenceId = Guid.NewGuid().ToString();
        }

        [DataMember]
        public bool IsEditMode { get; set; }

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
        public List<MessageClassDTO> MessageClasses { get; set; }

    }
}
