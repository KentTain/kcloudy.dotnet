using KC.Framework.Base;
using KC.Model.Blog.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KC.Model.Blog
{
    [Table(Tables.Comment)]
    public class Comment : Entity
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(64)]
        public string NickName { get; set; }
        [MaxLength(1024)]
        public string HeadUrl { get; set; }

        public string Content { get; set; }

        public DateTime CreateTime { get; set; }
        [MaxLength(128)]
        public string UserId { get; set; }
        [MaxLength(128)]
        public string UserName { get; set; }

        public int BlogId { get; set; }
        [ForeignKey("BlogId")]
        public Blog Blog { get; set; }
    }
}
