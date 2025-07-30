using KC.Framework.Base;
using KC.Model.Blog.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KC.Model.Blog
{
    [Table(Tables.Category)]
    public class Category : EntityBase
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(64)]
        public string Name { get; set; }
        [MaxLength(2046)]
        public string Remark { get; set; }

        public int OrderBy { get; set; }
    }
}
