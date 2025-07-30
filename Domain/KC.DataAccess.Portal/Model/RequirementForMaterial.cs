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

namespace KC.Model.Portal
{
    [Table(Tables.RequirementForMaterial)]
    public class RequirementForMaterial : Entity
    {
        //[Key]
        //[Column(Order = 1)]
        public int RequirementId { get; set; }

        //[Key]
        //[Column(Order = 2)]
        public int MaterialId { get; set; }

        [ForeignKey("RequirementId")]
        public RecommendRequirement RecommendRequirement { get; set; }
        [ForeignKey("MaterialId")]
        public RecommendMaterial RecommendMaterial { get; set; }
    }
}
