using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Model.Account.Constants;

namespace KC.Model.Account
{
    [Table(Tables.UsersInOrganizations)]
    public class UsersInOrganizations : EntityBase
    {
        //[Key]
        //[Column(Order = 1)]
        public int OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public Organization Organization { get; set; }

        //[Key]
        //[Column(Order = 2)]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
