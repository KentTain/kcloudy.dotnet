using KC.Service.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using KC.Framework.Extension;

namespace KC.Service.DTO.Blog
{
    [Serializable, DataContract(IsReference = true)]
    public class BlogDTO : EntityDTO
    {
        public BlogDTO()
        {
            Comments = new List<CommentDTO>();
        }

        [DataMember]
        public bool IsEditMode { get; set; }
        [DataMember]
        public int Id { get; set; }
        [MaxLength(512)]
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public bool IsTop { get; set; }
        [DataMember]
        public string Content { get; set; }
        [MaxLength(2000)]
        [DataMember]
        public string Summary { get; set; }
        [MaxLength(512)]
        [DataMember]
        public string Tags { get; set; }
        [DataMember]
        public string[] TagList
        {
            get
            {
                return Tags.ArrayFromCommaDelimitedStringsBySplitChar(',');
            }
        }

        [MaxLength(512)]
        [DataMember]
        public string SourceUrl { get; set; }
        [MaxLength(128)]
        [DataMember]
        public string UserId { get; set; }
        [MaxLength(128)]
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public DateTime CreateTime { set; get; }
        [DataMember]
        public int CategoryId { get; set; }
        [DataMember]
        public string CategoryName { get; set; }
        [DataMember]
        public virtual List<CommentDTO> Comments { get; set; }
    }
}
