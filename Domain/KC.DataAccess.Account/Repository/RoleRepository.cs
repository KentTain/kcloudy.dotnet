using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Extension;
using KC.Model.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace KC.DataAccess.Account.Repository
{
    public class AspNetRoleRepository : RoleStore<Role>, IRoleRepository
    {
        public AspNetRoleRepository(ComAccountContext dbContext)
            : base(dbContext)
        {
        }

        

        public List<Role> FindDetailRolesByNames(List<string> names)
        {
            return
                base.Context.Set<Role>()
                    .Include(m => m.RolePermissions)
                        .ThenInclude(r => r.Permission)
                    .Include(m => m.RoleMenuNodes)
                        .ThenInclude(r => r.MenuNode)
                    .Where(m => names.Contains(m.Name) && !m.IsDeleted)
                    .OrderByDescending(m => m.CreatedDate)
                    .AsNoTracking()
                    .ToList();
        }

        public async Task<IEnumerable<Role>> FindRolesMenusByIdsAsync(List<string> roleIds)
        {
            return await base.Context.Set<Role>()
                    .Include(m => m.RoleMenuNodes)
                        .ThenInclude(e => e.MenuNode)
                    .Where(m => roleIds.Contains(m.Id) && !m.IsDeleted)
                    .AsNoTracking()
                    .ToListAsync();
        }

        public async Task<List<Role>> FindRolesPermissionsByIdsAsync(List<string> roleIds)
        {
            return await base.Context.Set<Role>()
                    .Include(m => m.RolePermissions)
                        .ThenInclude(r => r.Permission)
                    .Where(m => roleIds.Contains(m.Id) && !m.IsDeleted)
                    .AsNoTracking()
                    .ToListAsync();
        }

        public List<Role> FindRolesByNames(List<string> names)
        {
            return
                base.Context.Set<Role>()
                    .Include(m => m.RolePermissions)
                        .ThenInclude(r => r.Permission)
                    .Include(m => m.RoleMenuNodes)
                        .ThenInclude(r => r.MenuNode)
                    .Where(m => names.Contains(m.Name))
                    .AsNoTracking()
                    .ToList();
        }

        public List<Role> FindDetailRoles()
        {
            Expression<Func<Role, bool>> predicate = m => !m.IsDeleted;
            return base.Context.Set<Role>()
                .Include(m => m.RolePermissions)
                    .ThenInclude(r => r.Permission)
                .Include(m => m.RoleMenuNodes)
                    .ThenInclude(r => r.MenuNode)
                .AsNoTracking()
                .Where(predicate)
                .OrderByDescending(m => m.CreatedDate)
                .ToList();
        }

        public IList<Role> FindRoleWithUsersByFilter<K>(Expression<Func<Role, bool>> predicate,
            Expression<Func<Role, K>> keySelector, bool ascending = true)
        {
            return @ascending
                ? Roles
                    .Include(m => m.RoleUsers)
                        .ThenInclude(r => r.User)
                    .Where(predicate)
                    .AsNoTracking()
                    .OrderBy(keySelector)
                    .ToList()
                : Roles
                    .Include(m => m.RoleUsers)
                        .ThenInclude(r => r.User)
                    .Where(predicate)
                    .AsNoTracking()
                    .OrderByDescending(keySelector)
                    .ToList();
        }

        public async Task<IList<Role>> FindRoleWithUsersByFilterAsync<K>(Expression<Func<Role, bool>> predicate,
            Expression<Func<Role, K>> keySelector, bool ascending = true)
        {
            return @ascending
                ? await Roles
                    .Include(m => m.RoleUsers)
                        .ThenInclude(r => r.User)
                    .Where(predicate)
                    .AsNoTracking()
                    .OrderBy(keySelector)
                    .ToListAsync()
                : await Roles
                    .Include(m => m.RoleUsers)
                        .ThenInclude(r => r.User)
                    .Where(predicate)
                    .AsNoTracking()
                    .OrderByDescending(keySelector)
                    .ToListAsync();
        }

        public Tuple<int, List<Role>> FindPagenatedRoles<K>(int pageIndex, int pageSize,
            Expression<Func<Role, bool>> predicate, Expression<Func<Role, K>> keySelector,
            bool paging = true, bool ascending = true)
        {
            var databaseItems =
                base.Context.Set<Role>().AsNoTracking().Where(predicate);
            int resultcount = databaseItems.Count(predicate);
            if (!paging)
            {
                var results = @ascending
                    ? databaseItems
                        .Where(predicate)
                        .AsNoTracking()
                        .OrderBy(keySelector)
                        .ToList()
                    : databaseItems
                        .Include(m => m.RoleUsers)
                            .ThenInclude(r => r.User)
                        .Where(predicate)
                        .AsNoTracking()
                        .OrderByDescending(keySelector)
                        .ToList();

                return new Tuple<int, List<Role>>(resultcount, results);
            }
            var result = @ascending
                ? databaseItems
                    .Where(predicate)
                    .AsNoTracking()
                    .OrderBy(keySelector)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToList()
                : databaseItems
                    .Include(m => m.RoleUsers)
                    .ThenInclude(r => r.User)
                    .Where(predicate)
                    .AsNoTracking()
                    .OrderByDescending(keySelector)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

            return new Tuple<int, List<Role>>(resultcount, result);
        }

        public Role GetDetailRoleByRoleId(string roleId)
        {
            //base.Context.Configuration.LazyLoadingEnabled = false;
            var result = base.Context.Set<Role>()
                .Include(m => m.RolePermissions)
                    .ThenInclude(r => r.Permission)
                .Include(m => m.RoleMenuNodes)
                    .ThenInclude(r => r.MenuNode)
                .Include(m => m.RoleUsers)
                    .ThenInclude(r => r.User)
                .AsNoTracking()
                .FirstOrDefault(m => m.Id == roleId && !m.IsDeleted);
            //base.Context.Configuration.LazyLoadingEnabled = true;

            return result;
        }

        public bool ExistsRole(Expression<Func<Role, bool>> predicate)
        {
            return base.Context.Set<Role>().Any(predicate);
        }

        public bool CreateRole(Role model)
        {

            base.Context.Set<Role>().Add(model);
            base.Context.ChangeTracker.AutoDetectChangesEnabled = false;
            int i = base.Context.SaveChanges();
            base.Context.ChangeTracker.AutoDetectChangesEnabled = true;
            return i > 0;
        }

        public bool RomoveRole(string roleId, string operatorId, string operatorName)
        {
            var role =
                base.Context.Set<Role>()
                    .FirstOrDefault(m => m.Id == roleId);

            if (role == null)
            {
                return true;
            }

            Context.Entry(role).State = EntityState.Deleted;
            //role.IsDeleted = true;
            const string logMessage = "{0}在{1}进行了以下操作：{2}！";
            var log = new UserTracingLog
            {
                OperatorId = operatorId,
                Operator = operatorName,
                OperateDate = DateTime.UtcNow,
                Type = Framework.Base.ProcessLogType.Success,
                Remark = string.Format(logMessage,
                    operatorName,
                    DateTime.UtcNow.ToLocalDateTimeStr("yyyy-MM-dd HH:mm:ss"),
                    "删除了角色（" + role.Id + ": " + role.Name + ")")
            };
            Context.Set<UserTracingLog>().Add(log);

            return base.Context.SaveChanges() > 0;
        }

        public bool UpdateRole(Role model)
        {
            if (Context.Entry(model).State == EntityState.Detached)
            {
                Context.Set<Role>().Attach(model);
            }
            Context.Entry(model).State = EntityState.Modified;
            int i = base.Context.SaveChanges();
            return i > 0;
        }

        public bool SavePermissionInRole(string roleId, List<int> newIds, string operatorId,
            string operatorName)
        {
            //获取角色及角色下所拥有的权限数据
            var role = base.Context.Set<Role>()
                .Include(m => m.RolePermissions)
                .ThenInclude(r => r.Permission)
                .FirstOrDefault(m => m.Id == roleId);
            if (role == null)
                throw new ArgumentException("roleId", string.Format("系统不存在角色Id={0}的角色。", roleId));

            if (newIds == null)
                throw new ArgumentNullException("newIds", "传入参数--newIds：权限Id列表为空");

            //默认权限组，即所有权限操作
            var permissions = role.RolePermissions.Select(m => m.Permission).Where(m => !m.IsDeleted).ToList();
            var dbIdList = permissions.Select(roleDto => roleDto.Id).ToList();
            var addIds = newIds.Except(dbIdList).ToList();
            var delIds = dbIdList.Except(newIds).ToList();

            foreach (var addId in addIds)
            {
                role.RolePermissions.Add(new PermissionsInRoles() { PermissionId = addId, RoleId = roleId });
            }

            var delp = role.RolePermissions.Where(m => delIds.Contains(m.PermissionId)).ToList();
            foreach (var permission in delp)
            {
                role.RolePermissions.Remove(permission);

                ////回收下级角色分配的权限
                //var relatedRoles = new List<AspNetRole>();
                //GetRolesByParentRoleIds(new List<string> { roleId }, ref relatedRoles);
                //foreach (var subRole in relatedRoles)
                //{
                //    subRole.Permissions.Remove(permission);
                //    //存在子级权限，同时取消角色子权限
                //    foreach (var childPermission in childrenPermissions)
                //    {
                //        subRole.Permissions.Remove(childPermission);
                //    }
                //}
            }

            if (!addIds.Any() && !delIds.Any())
                return true;

            const string logMessage = "{0}在{1}进行了以下操作：{2}！";
            string operMessage = string.Format("更新了角色（{0}）的权限-", role.DisplayName);

            if (addIds.Any())
                operMessage += string.Format("添加了权限({0})", addIds.Select(m => m.ToString()).ToCommaSeparatedString());
            if (delp.Any())
                operMessage += string.Format("删除了权限({0})", delp.Select(m => m.PermissionId + ": " + m.Permission.Name).ToCommaSeparatedString());

            var log = new UserTracingLog
            {
                OperatorId = operatorId,
                Operator = operatorName,
                OperateDate = DateTime.UtcNow,
                Type = Framework.Base.ProcessLogType.Success,
                Remark = string.Format(logMessage,
                    operatorName,
                    DateTime.UtcNow.ToLocalDateTimeStr("yyyy-MM-dd HH:mm:ss"),
                    operMessage)
            };
            base.Context.Set<UserTracingLog>().Add(log);

            return base.Context.SaveChanges() > 0;
        }

        public bool SaveMenuInRole(string roleId, List<int> newIds, string operatorId, string operatorName)
        {
            var role = base.Context.Set<Role>()
                .Include(m => m.RoleMenuNodes)
                .ThenInclude(r => r.MenuNode)
                .FirstOrDefault(m => m.Id == roleId);
            if (role == null)
                throw new ArgumentException("roleId", string.Format("系统不存在角色Id={0}的角色。", roleId));

            if (newIds == null)
                throw new ArgumentNullException("newIds", "传入参数--newIds：菜单Id列表为空");

            var dbIdList = new List<int>();
            var menuNodes = role.RoleMenuNodes.Select(m => m.MenuNode).Where(m => !m.IsDeleted).ToList();
            dbIdList.AddRange(menuNodes.Select(roleDto => roleDto.Id));

            var delIds = dbIdList.Except(newIds).ToList();
            var addIds = newIds.Except(dbIdList).ToList();

            foreach (var addId in addIds)
            {
                role.RoleMenuNodes.Add(new MenuNodesInRoles() { MenuNodeId = addId, RoleId = roleId });
            }

            var delp = role.RoleMenuNodes.Where(m => delIds.Contains(m.MenuNodeId)).ToList();
            foreach (var menu in delp)
            {
                role.RoleMenuNodes.Remove(menu);

                //回收下级角色分配的菜单
                //var relatedRoles = new List<AspNetRole>();
                //GetRolesByParentRoleIds(new List<string> { role.Id }, ref relatedRoles);
                //foreach (var subRole in relatedRoles)
                //{
                //    subRole.MenuNodes.Remove(menu);
                //}
            }

            if (!addIds.Any() && !delp.Any())
                return true;

            const string logMessage = "{0}在{1}进行了以下操作：{2}！";
            string operMessage = string.Format("更新了角色（{0}）的菜单--添加了菜单({1})，删除了菜单({2})",
                 role.Name,
                addIds.Select(m => m.ToString()).ToCommaSeparatedString(),
                delp.Select(m => m.MenuNodeId + ": " + m.MenuNode.Name).ToCommaSeparatedString());
            var log = new UserTracingLog
            {
                OperatorId = operatorId,
                Operator = operatorName,
                OperateDate = DateTime.UtcNow,
                Type = Framework.Base.ProcessLogType.Success,
                Remark = string.Format(logMessage,
                    operatorName,
                    DateTime.UtcNow.ToLocalDateTimeStr("yyyy-MM-dd HH:mm:ss"),
                    operMessage)
            };
            base.Context.Set<UserTracingLog>().Add(log);

            return base.Context.SaveChanges() > 0;
        }

        public bool SaveUserInRole(string roleId, List<string> addList, List<string> delList, string operatorId,
            string operatorName)
        {
            var role = base.Context.Set<Role>()
                .Include(m => m.RoleUsers)
                .ThenInclude(r => r.User)
                .FirstOrDefault(m => m.Id == roleId);
            if (role == null)
                throw new ArgumentException("roleId", string.Format("系统不存在角色Id={0}的角色。", roleId));

            if (addList == null && delList == null)
                throw new ArgumentNullException("addList", "传入参数--addList和delList：用户Id列表为空");

            if (addList == null)
                addList = new List<string>();

            foreach (var addUserId in addList)
            {
                var userInRole = new UsersInRoles() { UserId = addUserId, RoleId = roleId };
                role.RoleUsers.Add(userInRole);
            }

            if (delList == null)
                delList = new List<string>();

            var delUsers = role.RoleUsers.Where(m => delList.Contains(m.UserId.ToString())).ToList();
            foreach (var delUser in delUsers)
            {
                role.RoleUsers.Remove(delUser);
            }

            if (!addList.Any() && !delUsers.Any())
                return true;

            //var addp = base.Context.Set<AspNetUser>().Where(m => addList.Contains(m.Id.ToString())).ToList();
            //var delp = base.Context.Set<AspNetUser>().Where(m => delList.Contains(m.Id.ToString())).ToList();
            //foreach (var menu in addp)
            //{
            //    var userInRole = new IdentityUserRole() { UserId = menu.Id, RoleId = roleId };
            //    role.Users.Add(userInRole);
            //}
            //foreach (var menu in delp)
            //{
            //    var userInRole = new IdentityUserRole() { UserId = menu.Id, RoleId = roleId };
            //    role.Users.Remove(userInRole);
            //}

            const string logMessage = "{0}在{1}进行了以下操作：{2}！";
            string operMessage = string.Format("更新了角色（{0}）的用户--添加了用户Id({1})，删除了用户Id({2})",
                 role.Name,
                addList.Select(m => m).ToCommaSeparatedString(),
                delUsers.Select(m => m.UserId).ToCommaSeparatedString());
            var log = new UserTracingLog
            {
                OperatorId = operatorId,
                Operator = operatorName,
                OperateDate = DateTime.UtcNow,
                Type = Framework.Base.ProcessLogType.Success,
                Remark = string.Format(logMessage,
                    operatorName,
                    DateTime.UtcNow.ToLocalDateTimeStr("yyyy-MM-dd HH:mm:ss"),
                    operMessage)
            };
            base.Context.Set<UserTracingLog>().Add(log);

            return base.Context.SaveChanges() > 0;
        }

        /// <summary>
        /// 获取角色用户下创建的角色
        /// </summary>
        /// <param name="roleIds"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public void GetRolesByParentRoleIds(List<string> roleIds, ref List<Role> roles)
        {
            var usersInRole = base.Context.Set<Role>()
                .Include(o => o.RoleUsers)
                .ThenInclude(r => r.User)
                .Where(p => roleIds.Contains(p.Id));
            var userIds = usersInRole.SelectMany(o => o.RoleUsers.Select(p => p.UserId)).ToList();
            var targetRole =
                base.Context.Set<Role>()
                    .Include(o => o.RolePermissions)
                    .ThenInclude(r => r.Permission)
                    .Include(o => o.RoleMenuNodes)
                    .ThenInclude(r => r.MenuNode)
                    .Where(o => userIds.Contains(o.CreatedBy));
            var resultRoleIds = roles.Select(o => o.Id).ToList();
            var resultTargetRoles = targetRole.Where(o => !resultRoleIds.Contains(o.Id));
            if (resultTargetRoles.Any())
            {
                roles.AddRange(resultTargetRoles);
                GetRolesByParentRoleIds(targetRole.Select(o => o.Id).ToList(), ref roles);
            }
        }

        private void AddLog(string roleName, List<Permission> newPermissions, List<Permission> delPermissions, string operatorId, string operatorName)
        {
            const string logMessage = "{0}在{1}进行了以下操作：{2}！";
            string operMessage = string.Format("更新了角色（{0}）的权限-", roleName);

            if (newPermissions.Any())
                operMessage += string.Format("添加了权限({0})", newPermissions.Select(m => m.Id + ": " + m.Name).ToCommaSeparatedString());
            if (delPermissions.Any())
                operMessage += string.Format("删除了权限({0})", delPermissions.Select(m => m.Id + ": " + m.Name).ToCommaSeparatedString());

            var log = new UserTracingLog
            {
                OperatorId = operatorId,
                Operator = operatorName,
                OperateDate = DateTime.UtcNow,
                Type = Framework.Base.ProcessLogType.Success,
                Remark = string.Format(logMessage,
                    operatorName,
                    DateTime.UtcNow.ToLocalDateTimeStr("yyyy-MM-dd HH:mm:ss"),
                    operMessage)
            };
            base.Context.Set<UserTracingLog>().Add(log);
        }

        public List<Role> FindByFilter(Expression<Func<Role, bool>> predicate)
        {
            return Context.Set<Role>().AsNoTracking().Where(predicate).ToList();
        }
        public async Task<List<Role>> FindByFilterAsync(Expression<Func<Role, bool>> predicate)
        {
            return await Context.Set<Role>()
                .Where(predicate)
                .OrderBy(m => m.DisplayName)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
