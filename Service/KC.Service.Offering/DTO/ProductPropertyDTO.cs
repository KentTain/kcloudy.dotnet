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
using KC.Service.DTO;
using KC.Service.DTO.Offering;

namespace KC.Model.Offering
{
    public class ProductPropertyDTO : EntityBaseDTO
    {
        public ProductPropertyDTO()
        {
            ProductPropertyType = ProductPropertyType.Specification;
        }

        public int Id { get; set; }

        public ProductPropertyType ProductPropertyType { get; set; }

        [MaxLength(256)]
        public string Name { get; set; }
        [MaxLength(4000)]
        public string Value { get; set; }
        [MaxLength(4000)]
        public string Value1 { get; set; }
        [MaxLength(4000)]
        public string Value2 { get; set; }

        public int? RefProviderId { get; set; }

        public int? RefProviderAttrId { get; set; }

        public bool CanEdit { get; set; }

        public bool IsRequire { get; set; }

        public bool IsProvider { get; set; }

        [MaxLength(4000)]
        public string Description { get; set; }

        public int Index { get; set; }

        public int ProductId { get; set; }
        public virtual ProductDTO Product { get; set; }
    }
}
