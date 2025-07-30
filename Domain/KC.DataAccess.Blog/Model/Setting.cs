using KC.Framework.Base;
using KC.Model.Blog.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KC.Model.Blog
{
    [Table(Tables.Setting)]
    public class Setting : PropertyAttributeBase
    {
        [MaxLength(128)]
        public string UserId { get; set; }
        [MaxLength(128)]
        public string UserName { get; set; }
    }
}
