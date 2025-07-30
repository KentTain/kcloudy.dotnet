using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KC.Framework.Base;
using KC.Enums.Offering;

namespace KC.Service.DTO.Offering
{
    public class PropertyProviderAttrDTO : EntityBaseDTO
    {
        public int Id { get; set; }

        public ServiceAttrDataType ServiceAttrDataType { get; set; }

        [MaxLength(256)]
        public string Name { get; set; }
        [MaxLength(4000)]
        public string Value { get; set; }
        [MaxLength(4000)]
        public string Value1 { get; set; }
        [MaxLength(4000)]
        public string Value2 { get; set; }

        public bool CanEdit { get; set; }

        [MaxLength(4000)]
        public string Description { get; set; }

        public int Index { get; set; }

        public int ServiceProviderId { get; set; }
        [ForeignKey("ServiceProviderId")]
        public virtual PropertyProviderDTO ServiceProvider { get; set; }
    }
}
