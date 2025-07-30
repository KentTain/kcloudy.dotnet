using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using KC.Model.Account.Constants;
using KC.Framework.Base;

namespace KC.Model.Account
{
    [Table(Tables.MenuNode)]
    public class MenuNode : TreeNode<MenuNode>, ICloneable
    {
        public MenuNode()
        {
            this.MenuRoles = new List<MenuNodesInRoles>();
            Version = TenantVersion.Standard;
            ApplicationId = GlobalConfig.ApplicationGuid;
        }

        /// <summary>
        /// 请求地址
        /// </summary>
        [MaxLength(128)]
        public string AreaName { get; set; }
        /// <summary>
        /// 请求地址
        /// </summary>
        [MaxLength(128)]
        public string ControllerName { get; set; }
        /// <summary>
        /// 请求地址
        /// </summary>
        [MaxLength(128)]
        public string ActionName { get; set; }
        /// <summary>
        /// 菜单参数
        /// </summary>
        [MaxLength(1024)]
        public string Parameters { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(512)]
        public string Description { get; set; }

        /// <summary>
        /// 小图标
        /// </summary>
        [MaxLength(256)]
        public string SmallIcon { get; set; }

        /// <summary>
        /// 大图标
        /// </summary>
        [MaxLength(256)]
        public string BigIcon { get; set; }

        public TenantVersion Version { get; set; }
        public TenantType? TenantType { get; set; }
        /// <summary>
        /// 是否Ext页面
        /// </summary>
        public bool IsExtPage { get; set; }
        /// <summary>
        /// 访问地址
        /// </summary>
        [NotMapped]
        public string URL
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ControllerName) && string.IsNullOrWhiteSpace(ActionName))
                {
                    return string.Empty;
                }
                
                if(string.IsNullOrEmpty(AreaName))
                    return string.Format("/{0}/{1}", ControllerName?.Trim(), ActionName?.Trim());

                return string.Format("/{0}/{1}/{2}", AreaName?.Trim(), ControllerName?.Trim(), ActionName?.Trim());
            }
        }

        [NotMapped]
        public string DefaultRoleId { get; set; }

        public Guid ApplicationId { get; set; }

        [MaxLength(128)] 
        public string ApplicationName { get; set; }

        /// <summary>
        /// 菜单的权限控制Id
        /// </summary>
        public string AuthorityId { get; set; }
        /// <summary>
        /// 角色列表
        /// </summary>
        public ICollection<MenuNodesInRoles> MenuRoles { get; set; }
        [NotMapped]
        public IEnumerable<Role> Roles => MenuRoles.Select(e => e.Role);

        public override object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class MenuNodeEquality : EqualityComparer<MenuNode>
    {
        public override bool Equals(MenuNode x, MenuNode y)
        {
            var xcode = string.Format("/{0}/{1}/{2}/{3}",
                string.IsNullOrEmpty(x.AreaName) ? string.Empty : x.AreaName.Trim().ToLower(),
                string.IsNullOrEmpty(x.ControllerName) ? string.Empty : x.ControllerName.Trim().ToLower(),
                string.IsNullOrEmpty(x.ActionName) ? string.Empty : x.ActionName.Trim().ToLower(),
                x.ApplicationId == null ? string.Empty : x.ApplicationId.ToString().Trim().ToLower());
            var ycode = string.Format("/{0}/{1}/{2}/{3}",
                string.IsNullOrEmpty(y.AreaName) ? string.Empty : y.AreaName.Trim().ToLower(),
                string.IsNullOrEmpty(y.ControllerName) ? string.Empty : y.ControllerName.Trim().ToLower(),
                string.IsNullOrEmpty(y.ActionName) ? string.Empty : y.ActionName.Trim().ToLower(),
                y.ApplicationId == null ? string.Empty : y.ApplicationId.ToString().Trim().ToLower());

            return xcode.Equals(ycode, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode(MenuNode obj)
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
