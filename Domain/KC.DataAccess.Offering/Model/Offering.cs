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
    [Table(Tables.Offering)]
    public class Offering : Entity
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OfferingId { get; set; }

        [MaxLength(16)]
        public string OfferingCode { get; set; }

        [MaxLength(256)]
        public string OfferingName { get; set; }

        [MaxLength(128)]
        public string OfferingTypeCode { get; set; }
        [MaxLength(512)]
        public string OfferingTypeName { get; set; }

        public OfferingStatus Status { get; set; }

        public bool IsEnabled { get; set; }

        [MaxLength(20)]
        public string OfferingUnit { get; set; }

        [MaxLength(1000)]
        public string OfferingImage { get; set; }

        [MaxLength(1000)]
        public string OfferingFile { get; set; }

        public decimal? OfferingPrice { get; set; }

        public decimal? OfferingDiscount { get; set; }

        public decimal? OfferingRate { get; set; }

        [MaxLength(4000)]
        public string OfferingAddress { get; set; }

        [MaxLength(4000)]
        public string Description { get; set; }

        [MaxLength(512)]
        public string Barcode { get; set; }

        public int Index { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public virtual ICollection<OfferingProperty> OfferingProperties { get; set; }

        public virtual ICollection<OfferingOperationLog> OfferingOperationLogs { get; set; }
    }
}
