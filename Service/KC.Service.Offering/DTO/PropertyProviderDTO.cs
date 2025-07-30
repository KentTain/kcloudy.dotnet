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

namespace KC.Service.DTO.Offering
{
    public class PropertyProviderDTO : EntityBaseDTO
    {
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
        public virtual CategoryDTO Category { get; set; }

        public virtual ICollection<PropertyProviderAttrDTO> ServiceProviderAttrs { get; set; }
    }
}
