using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Model.Offering.Constants;
using KC.Framework.Base;

namespace KC.Model.Offering
{
    [Table(Tables.Product)]
    public class Product : Entity
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }

        [MaxLength(16)]
        public string ProductCode { get; set; }

        [MaxLength(256)]
        public string ProductName { get; set; }

        public bool IsEnabled { get; set; }

        [MaxLength(20)]
        public string ProductUnit { get; set; }

        [MaxLength(1000)]
        public string ProductImage { get; set; }

        [MaxLength(1000)]
        public string ProductFile { get; set; }

        public decimal? ProductPrice { get; set; }

        public decimal? ProductDiscount { get; set; }

        public decimal? ProductRate { get; set; }

        [MaxLength(512)]
        public string Barcode { get; set; }

        public int Index { get; set; }

        public int OfferingId { get; set; }
        [ForeignKey("OfferingId")]
        public virtual Offering Offering { get; set; }

        public virtual ICollection<ProductProperty> ProductProperties { get; set; }
    }
}
