using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Model.Offering.Constants;
using KC.Framework.Base;
using KC.Enums.Offering;

namespace KC.Model.Offering
{
    [Table(Tables.PropertyProvider)]
    public class PropertyProvider : EntityBase
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public ServiceDataType ServiceDataType { get; set; }

        [MaxLength(256)]
        public string Name { get; set; }

        public bool CanEdit { get; set; }

        public bool IsRequire { get; set; }

        [MaxLength(4000)]
        public string Description { get; set; }

        public int Index { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        public virtual ICollection<PropertyProviderAttr> ServiceProviderAttrs { get; set; }
    }
}
