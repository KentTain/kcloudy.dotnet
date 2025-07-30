using KC.Service.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace KC.Service.DTO.Blog
{
    [Serializable, DataContract(IsReference = true)]
    public class CommentDTO : EntityDTO
    {
        [DataMember]
        public int Id { get; set; }
        [MaxLength(64)]
        [DataMember]
        public string NickName { get; set; }
        [MaxLength(1024)]
        [DataMember]
        public string HeadUrl { get; set; }
        [DataMember]
        public string Content { get; set; }
        [DataMember]
        public DateTime CreateTime { get; set; }
        [MaxLength(128)]
        [DataMember]
        public string UserId { get; set; }
        [MaxLength(128)]
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public int BlogId { get; set; }
        [DataMember]
        public BlogDTO Blog { get; set; }
    }
}
