using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Model.Admin.Constants;
using KC.Framework.Base;
using KC.Enums.App;
using System.Runtime.Serialization;

namespace KC.Model.Admin
{
    [Table(Tables.RequirementForMaterial)]
    [Serializable, DataContract(IsReference = true)]
    public class RequirementForMaterial : Entity
    {
        //[Key]
        //[Column(Order = 1)]
        [DataMember]
        public int RequirementId { get; set; }

        //[Key]
        //[Column(Order = 2)]
        [DataMember]
        public int MaterialId { get; set; }

        [ForeignKey("RequirementId")]
        [DataMember]
        public TenantUserRequirement Requirement { get; set; }
        [ForeignKey("MaterialId")]
        [DataMember]
        public TenantUserMaterial Material { get; set; }
    }
}
