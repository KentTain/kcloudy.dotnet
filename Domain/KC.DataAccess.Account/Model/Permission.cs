using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Model.Account.Constants;
using KC.Framework.Base;

namespace KC.Model.Account
{
    [Table(Tables.Permission)]
    public class Permission : TreeNode<Permission>, ICloneable
    {
        public Permission()
        {
            PermissionRoles = new List<PermissionsInRoles>();
            ApplicationId = GlobalConfig.ApplicationGuid;
            Id = 0;
            ParentId = null;
        }

        [MaxLength(128)]
        public string AreaName { get; set; }

        [MaxLength(128)]
        public string ControllerName { get; set; }

        [MaxLength(128)]
        public string ActionName { get; set; }

        [MaxLength(1024)]
        public string Parameters { get; set; }

        public ResultType ResultType { get; set; }
        
        [MaxLength(512)]
        public string Description { get; set; }

        public Guid ApplicationId { get; set; }

        [MaxLength(128)]
        public string ApplicationName { get; set; }

        [NotMapped]
        public string DefaultRoleId { get; set; }

        /// <summary>
        /// 菜单的权限控制Id
        /// </summary>
        public string AuthorityId { get; set; }

        public virtual ICollection<PermissionsInRoles> PermissionRoles { get; set; }
        [NotMapped]
        public IEnumerable<Role> Roles => PermissionRoles.Select(e => e.Role);

        public override object Clone()
        {
            base.MemberwiseClone();
            return this.MemberwiseClone();
        }

        public string GetAuthorityUrl()
        {
            return string.Format("/{0}/{1}/{2}/{3}",
                string.IsNullOrEmpty(AreaName) ? string.Empty : AreaName.Trim().ToLower(),
                string.IsNullOrEmpty(ControllerName) ? string.Empty : ControllerName.Trim().ToLower(),
                string.IsNullOrEmpty(ActionName) ? string.Empty : ActionName.Trim().ToLower(),
                ApplicationId == null ? string.Empty : ApplicationId.ToString().Trim().ToLower()
                );
        }
    }

    public class PermissionEquality : EqualityComparer<Permission>
    {
        public override bool Equals(Permission x, Permission y)
        {
            var xcode = string.Format("/{0}/{1}/{2}/{3}",
                string.IsNullOrEmpty(x.AreaName) ? string.Empty : x.AreaName.Trim().ToLower(),
                string.IsNullOrEmpty(x.ControllerName) ? string.Empty : x.ControllerName.Trim().ToLower(),
                string.IsNullOrEmpty(x.ActionName) ? string.Empty : x.ActionName.Trim().ToLower(),
                x.ApplicationId == null ? string.Empty : x.ApplicationId.ToString().Trim().ToLower()
                );
            var ycode = string.Format("/{0}/{1}/{2}/{3}",
                string.IsNullOrEmpty(y.AreaName) ? string.Empty : y.AreaName.Trim().ToLower(),
                string.IsNullOrEmpty(y.ControllerName) ? string.Empty : y.ControllerName.Trim().ToLower(),
                string.IsNullOrEmpty(y.ActionName) ? string.Empty : y.ActionName.Trim().ToLower(),
                y.ApplicationId == null ? string.Empty : y.ApplicationId.ToString().Trim().ToLower()
                );
            
            return xcode.Equals(ycode, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode(Permission obj)
        {
            var hashCode = string.Format("/{0}/{1}/{2}/{3}",
                string.IsNullOrEmpty(obj.AreaName) ? string.Empty : obj.AreaName.Trim().ToLower(),
                string.IsNullOrEmpty(obj.ControllerName) ? string.Empty : obj.ControllerName.Trim().ToLower(),
                string.IsNullOrEmpty(obj.ActionName) ? string.Empty : obj.ActionName.Trim().ToLower(),
                obj.ApplicationId == null ? string.Empty : obj.ApplicationId.ToString().Trim().ToLower()
                ).GetHashCode();

            return hashCode;
        }
    }
}
