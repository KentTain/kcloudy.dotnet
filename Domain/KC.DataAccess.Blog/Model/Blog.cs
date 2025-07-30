using KC.Framework.Base;
using KC.Model.Blog.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KC.Model.Blog
{
    [Table(Tables.Blog)]
    public class Blog : Entity
    {
        public Blog()
        {
            Comments = new List<Comment>();
        }

        [Key]
        public int Id { get; set; }
        [MaxLength(512)]
        public string Title { get; set; }

        public bool IsTop { get; set; }

        public string Content { get; set; }
        [MaxLength(2046)]
        public string Summary { get; set; }
        [MaxLength(512)]
        public string Tags { get; set; }
        [MaxLength(512)]
        public string SourceUrl { get; set; }
        [MaxLength(128)]
        public string UserId { get; set; }
        [MaxLength(128)]
        public string UserName { get; set; }

        public DateTime CreateTime { set; get; }

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}
