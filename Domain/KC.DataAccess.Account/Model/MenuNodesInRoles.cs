using KC.Framework.Base;
using KC.Model.Account.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KC.Model.Account
{
    [Table(Tables.MenuNodesInRoles)]
    public class MenuNodesInRoles : EntityBase
    {
        //[Key]
        //[Column(Order = 1)]
        public string RoleId { get; set; }
        [ForeignKey("RoleId")]
        public Role Role { get; set; }

        //[Key]
        //[Column(Order = 2)]
        public int MenuNodeId { get; set; }
        [ForeignKey("MenuNodeId")]
        public MenuNode MenuNode { get; set; }
    }

    public class MenuNodesInRolesEquality : EqualityComparer<MenuNodesInRoles>
    {
        public override bool Equals(MenuNodesInRoles x, MenuNodesInRoles y)
        {
            var xcode = string.Format("/{0}/{1}",
                    string.IsNullOrEmpty(x.RoleId) ? string.Empty : x.RoleId.Trim().ToLower(),
                    x.MenuNodeId.ToString());
            var ycode = string.Format("/{0}/{1}",
                    string.IsNullOrEmpty(y.RoleId) ? string.Empty : y.RoleId.Trim().ToLower(),
                    y.MenuNodeId.ToString());

            return xcode.Equals(ycode, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode(MenuNodesInRoles x)
        {
            var hashCode = string.Format("/{0}/{1}",
                    string.IsNullOrEmpty(x.RoleId) ? string.Empty : x.RoleId.Trim().ToLower(),
                    x.MenuNodeId.ToString()).GetHashCode();

            return hashCode;
        }
    }
}
