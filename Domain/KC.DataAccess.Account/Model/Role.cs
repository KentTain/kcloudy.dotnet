using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using KC.Model.Account.Constants;
using Microsoft.AspNetCore.Identity;

namespace KC.Model.Account
{
    [Table(Tables.Role)]
    public class Role : IdentityRole
    {
        public Role()
        {
            IsDeleted = false;
            BusinessType = BusinessType.None;
            RoleMenuNodes = new List<MenuNodesInRoles>();
            RolePermissions = new List<PermissionsInRoles>();
            RoleUsers = new List<UsersInRoles>();
        }
        /// <summary>
        /// 角色显示名
        /// </summary>
        [Required]
        [MaxLength(256)]
        [Display(Name = "角色显示名")]
        public string DisplayName { get; set; }
        /// <summary>
        /// 角色描述
        /// </summary>
        [MaxLength(256)]
        [Display(Name = "角色描述")]
        public string Description { get; set; }
        /// <summary>
        /// 是否为系统角色
        /// </summary>
        [Display(Name = "是否为系统角色")]
        public bool IsSystemRole { get; set; }
        /// <summary>
        /// 业务类型
        /// </summary>
        [Display(Name = "业务类型")]
        public BusinessType BusinessType { get; set; }
        /// <summary>
        /// 获取或设置 获取或设置是否禁用，逻辑上的删除，非物理删除
        /// </summary>
        [Display(Name = "是否删除")]
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 创建人Id
        /// </summary>
        [Display(Name = "创建人Id")]
        [MaxLength(128)]
        public string CreatedBy { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [Display(Name = "创建人")]
        [MaxLength(128)]
        public string CreatedName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// 修改人Id
        /// </summary>
        [Display(Name = "修改人Id")]
        [MaxLength(128)]
        public string ModifiedBy { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        [Display(Name = "修改人")]
        [MaxLength(128)]
        public string ModifiedName { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        [Display(Name = "修改时间")]
        public DateTime ModifiedDate { get; set; }

        public ICollection<MenuNodesInRoles> RoleMenuNodes { get; set; }
        [NotMapped]
        public IEnumerable<MenuNode> MenuNodes => RoleMenuNodes.Select(e => e.MenuNode);

        public ICollection<PermissionsInRoles> RolePermissions { get; set; }
        [NotMapped]
        public IEnumerable<Permission> Permissions => RolePermissions.Select(e => e.Permission);

        public ICollection<UsersInRoles> RoleUsers { get; set; }
        [NotMapped]
        public IEnumerable<User> Users => RoleUsers.Select(e => e.User);
    }
}
