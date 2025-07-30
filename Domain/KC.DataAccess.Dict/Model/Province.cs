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
    [Table(Tables.Province)]
    public class Province : Entity
    {
        public Province()
        {
            Cities = new List<City>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProvinceId { get; set; }

        [MaxLength(512)]
        public string Name { get; set; }

        public virtual ICollection<City> Cities { get; set; }
    }
}
