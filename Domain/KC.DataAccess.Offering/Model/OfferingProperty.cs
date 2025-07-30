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
    [Table(Tables.OfferingProperty)]
    public class OfferingProperty : EntityBase
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public OfferingPropertyType OfferingPropertyType { get; set; }

        [MaxLength(256)]
        public string Name { get; set; }

        public string Value { get; set; }

        public string Value1 { get; set; }

        public string Value2 { get; set; }

        public bool CanEdit { get; set; }

        public bool IsRequire { get; set; }

        public int Index { get; set; }

        public int OfferingId { get; set; }
        [ForeignKey("OfferingId")]
        public virtual Offering Offering { get; set; }

    }
}
