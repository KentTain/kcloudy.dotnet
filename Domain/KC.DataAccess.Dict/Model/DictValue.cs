using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Model.Dict.Constants;
using KC.Framework.Base;

namespace KC.Model.Dict
{
    [Table(Tables.DictValue)]
    public class DictValue : Entity
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(128)]
        public string Code { get; set; }

        [MaxLength(512)]
        public string Name { get; set; }

        [MaxLength(4000)]
        public string Description { get; set; }

        public int DictTypeId { get; set; }
        [MaxLength(128)]
        public string DictTypeCode { get; set; }
        [ForeignKey("DictTypeId")]
        public virtual DictType DictType { get; set; }
    }
}
