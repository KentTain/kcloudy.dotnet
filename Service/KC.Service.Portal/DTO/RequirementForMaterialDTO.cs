using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Model.Portal.Constants;
using KC.Framework.Base;
using KC.Enums.Portal;
using System.Runtime.Serialization;

namespace KC.Service.DTO.Portal
{

    [Serializable, DataContract(IsReference = true)]
    public class RequirementForMaterialDTO : EntityDTO
    {
        //[Key]
        //[Column(Order = 1)]
        [DataMember]
        public int RecommendId { get; set; }

        //[Key]
        //[Column(Order = 2)]
        [DataMember]
        public int MaterialId { get; set; }

        [DataMember]
        public RecommendRequirementDTO RecommendRequirement { get; set; }

        [DataMember]
        public RecommendMaterialDTO RecommendMaterial { get; set; }
    }
}
