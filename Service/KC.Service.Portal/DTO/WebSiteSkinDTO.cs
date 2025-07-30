using KC.Service.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Portal.DTO
{
    [Serializable, DataContract(IsReference = true)]
    public class WebSiteSkinDTO : EntityBaseDTO
    {
        [MaxLength(20)]
        [DataMember]
        public string SkinCode { get; set; }

        [MaxLength(200)]
        [DataMember]
        public string SkinName { get; set; }

        [MaxLength(2000)]
        [DataMember]
        public string SkinImageUrl { get; set; }

        [MaxLength(2000)]
        [DataMember]
        public string SkinDescription { get; set; }

        [DataMember]
        public decimal? SkinPrice { get; set; }

        [DataMember]
        public bool IsSelected { get; set; }
    }
}
