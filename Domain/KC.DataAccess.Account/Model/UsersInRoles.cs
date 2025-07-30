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
    [Table(Tables.UsersInRoles)]
    public class UsersInRoles : Microsoft.AspNetCore.Identity.IdentityUserRole<string>
    {
        [ForeignKey("RoleId")]
        public Role Role { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
