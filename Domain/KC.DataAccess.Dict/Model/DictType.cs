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
    [Table(Tables.DictType)]
    public class DictType : Entity
    {
        public DictType()
        {
            DictValues = new List<DictValue>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(128)]
        public string Code { get; set; }

        [MaxLength(512)]
        public string Name { get; set; }

        public bool IsSys { get; set; }

        public virtual ICollection<DictValue> DictValues { get; set; }
    }
}
