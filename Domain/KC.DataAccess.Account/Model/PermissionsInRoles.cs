using KC.Framework.Base;
using KC.Model.Account.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KC.Model.Account
{
    [Table(Tables.PermissionsInRoles)]
    public class PermissionsInRoles : EntityBase
    {
        //[Key]
        //[Column(Order = 1)]
        public string RoleId { get; set; }
        [ForeignKey("RoleId")]
        public Role Role { get; set; }

        //[Key]
        //[Column(Order = 2)]
        public int PermissionId { get; set; }
        [ForeignKey("PermissionId")]
        public Permission Permission { get; set; }
    }

    public class PermissionsInRolesEquality : EqualityComparer<PermissionsInRoles>
    {
        public override bool Equals(PermissionsInRoles x, PermissionsInRoles y)
        {
            var xcode = string.Format("/{0}/{1}",
                    string.IsNullOrEmpty(x.RoleId) ? string.Empty : x.RoleId.Trim().ToLower(),
                    x.PermissionId.ToString());
            var ycode = string.Format("/{0}/{1}",
                    string.IsNullOrEmpty(y.RoleId) ? string.Empty : y.RoleId.Trim().ToLower(),
                    y.PermissionId.ToString());

            return xcode.Equals(ycode, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode(PermissionsInRoles x)
        {
            var hashCode = string.Format("/{0}/{1}",
                    string.IsNullOrEmpty(x.RoleId) ? string.Empty : x.RoleId.Trim().ToLower(),
                    x.PermissionId.ToString()).GetHashCode();

            return hashCode;
        }
    }
}
