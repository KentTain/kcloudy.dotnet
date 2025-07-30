using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KC.Framework.Extension;
using KC.Database.EFRepository;
using KC.Model.Account;
using KC.Framework.Tenant;


namespace KC.DataAccess.Account.Repository
{
    public class PermissionRepository : EFTreeRepositoryBase<Permission>, IPermissionRepository
    {
        public PermissionRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        //public PermissionRepository(Tenant tenant)
        //    : base(new ComAccountUnitOfWorkContext(tenant))
        //{
        //}

        public IList<Permission> LoadAllDetailPermissions()
        {
            return
                Entities
                    .AsNoTracking()
                    .Where(m => !m.IsDeleted)
                    .OrderBy(m => m.Index)
                    .ToList();
        }

        public IList<Permission> LoadAllDetailPermissionsByApplication(Guid applicationId)
        {
            return
                Entities
                    .AsNoTracking()
                    .Where(m => !m.IsDeleted && m.ApplicationId == applicationId)
                    .OrderBy(m => m.Index)
                    .ToList();
        }

        public IList<Permission> LoadDetailPermissionsByFilter(Expression<Func<Permission, bool>> predicate)
        {
            List<Permission> data =
                Entities
                    .Where(predicate)
                    .AsNoTracking()
                    .OrderBy(m => m.Index)
                    .ToList();
            RomveDeletedPermission(data);
            return data;
        }

        public IList<Permission> LoadPermissionByIndex(Expression<Func<Permission, bool>> predicate, string sort, bool ascending = true)
        {
            var q = Entities
                    .Where(predicate)
                    .AsNoTracking()
                    .OrderBy(m => m.Index)
                    .ToList();

            var results = q.SingleOrderBy(sort, ascending).ToList();
            RomveDeletedPermission(results);
            return results;
        }

        /// <summary>
        /// 去掉权限中被删除的子权限
        /// </summary>
        /// <returns></returns>
        private void RomveDeletedPermission(List<Permission> permissions)
        {
            var nodes = new List<Permission>();
            foreach (var o in permissions)
            {
                nodes = new List<Permission>();
            }
        }

        public IList<Permission> GetPermissionsByRoleIds(List<string> roleIds)
        {
            return
                Entities
                    //.Include(m => m.ChildNodes.Select(c => c.ChildNodes))
                    .Where(m => !m.IsDeleted && m.PermissionRoles.Any(r => roleIds.Contains(r.RoleId)))
                    .AsNoTracking()
                    .OrderBy(m => m.ControllerName)
                    .ToList();
        }

        public IList<Permission> GetPermissionsByRoleNames(List<string> roleNames)
        {
            return
                Entities
                    .Where(m => !m.IsDeleted && m.PermissionRoles.Select(r => r.Role).Any(r => roleNames.Contains(r.Name)))
                    .AsNoTracking()
                    .OrderBy(m => m.ControllerName)
                    .ToList();
        }

        public override Tuple<int, IList<Permission>> FindPagenatedListWithCount<K>(int pageIndex, int pageSize,
            Expression<Func<Permission, bool>> predicate, string sortProperty, bool ascending = true)
        {
            var q = Entities
                .Where(predicate).AsNoTracking();
            int recordCount = q.Count();

            return new Tuple<int, IList<Permission>>(recordCount,
                q.SingleOrderBy(sortProperty, ascending)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToList());
        }



        public Permission GetDetailPermissionById(params object[] keyValues)
        {
            if (keyValues.Any())
            {
                int id = int.Parse(keyValues[0].ToString());
                return Entities
                    .Include(m => m.PermissionRoles)
                    .ThenInclude(m => m.Role)
                    .AsNoTracking()
                    .FirstOrDefault(m => m.Id == id);
            }

            return null;
        }

        public Permission GetPermissionByAppIDAndName(Guid? appId, string controllerName, string actionName)
        {
            Expression<Func<Permission, bool>> predicate = m => !m.IsDeleted;
            if (appId != null && appId != Guid.Empty)
            {
                predicate = predicate.And(m => m.ApplicationId == appId);
            }
            if (!string.IsNullOrWhiteSpace(controllerName))
            {
                predicate = predicate.And(m => m.ControllerName == controllerName);
            }
            if (!string.IsNullOrWhiteSpace(actionName))
            {
                predicate = predicate.And(m => m.ActionName == actionName);
            }
            return Entities
                .AsNoTracking()
                .FirstOrDefault(predicate);
        }

        public bool ExistsPermission(Guid appId, string controllerName, string actionName)
        {
            Expression<Func<Permission, bool>> predicate = m => !m.IsDeleted;
            if (appId != null && appId != Guid.Empty)
            {
                predicate = predicate.And(m => m.ApplicationId == appId);
            }
            if (!string.IsNullOrWhiteSpace(controllerName))
            {
                predicate = predicate.And(m => m.ControllerName == controllerName);
            }
            if (!string.IsNullOrWhiteSpace(actionName))
            {
                predicate = predicate.And(m => m.ActionName == actionName);
            }
            return Entities.Any(predicate);
        }

        public bool SaveRoleInPremission(int id, List<string> newIds, string operatorId, string operatorName)
        {
            var permission = Entities
                .Include(m => m.PermissionRoles)
                .ThenInclude(r => r.Role)
                .FirstOrDefault(m => m.Id == id);
            if (permission == null)
                throw new ArgumentException("id", string.Format("系统不存在权限Id={0}的权限。", id));

            if (newIds == null)
                throw new ArgumentNullException("newIds", "传入参数--newIds：权限Id列表为空。");

            var dbIdList = new List<string>();
            dbIdList.AddRange(permission.PermissionRoles.Select(roleDto => roleDto.RoleId));

            var delList = dbIdList.Except(newIds);
            var addList = newIds.Except(dbIdList);

            var addr = base.EFContext.Context.Set<Role>()
                .Where(m => addList.Contains(m.Id))
                .ToList();
            foreach (var role in addr)
            {
                permission.PermissionRoles.Add(new PermissionsInRoles() { RoleId = role.Id, PermissionId = id });
            }

            var delr = new List<PermissionsInRoles>();
            foreach (var role in permission.PermissionRoles)
            {
                if (delList.Contains(role.RoleId))
                    delr.Add(role);
            }
            foreach (var role in delr)
            {
                permission.PermissionRoles.Remove(role);
                //存在子级权限，同时取消子级权限角色
                //var childrenPermissions = base.EFContext.Context.Set<Permission>().Include(m => m.PermissionRoles).Where(n => n.ParentId == permission.Id);
                //foreach (var childPermission in childrenPermissions)
                //{
                //    childPermission.PermissionRoles.Remove(role);
                //}

                //回收下级角色分配的权限
                //var relatedRoles = new List<AspNetRole>();
                //GetRolesByParentRoleIds(new List<string> { role.Id }, ref relatedRoles);
                //foreach (var subRole in relatedRoles)
                //{
                //    permission.Roles.Remove(subRole);
                //    //存在子级权限，同时取消子级权限角色
                //    foreach (var childPermission in childrenPermissions)
                //    {
                //        childPermission.Roles.Remove(subRole);
                //    }
                //}
            }

            if (!addr.Any() && !delr.Any())
                return true;

            const string logMessage = "{0}在{1}进行了以下操作：{2}！";
            string operMessage = string.Format("更新了权限（{0}）角色--添加了角色({1})，删除了角色({2})！",
                permission.Id + ": " + permission.Name,
                addr.Select(m => m.Id + ": " + m.Name).ToCommaSeparatedString(),
                delr.Select(m => m.RoleId + ": " + m.Role.Name).ToCommaSeparatedString());
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
            base.EFContext.Context.Set<UserTracingLog>().Add(log);

            return base.EFContext.Context.SaveChanges() > 0;
        }


        /// <summary>
        /// 获取角色用户下创建的角色
        /// </summary>
        /// <param name="roleIds"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public void GetRolesByParentRoleIds(List<string> roleIds, ref List<Role> roles)
        {
            var usersInRole = UnitOfWork.Context.Set<Role>()
                .Include(o => o.RoleUsers)
                .Where(p => roleIds.Contains(p.Id));
            var userIds = usersInRole
                .SelectMany(o => o.RoleUsers.Select(p => p.UserId))
                .ToList();
            var targetRole =
                UnitOfWork.Context.Set<Role>()
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

        //allPermissions is tree node, that contain children's permissions
        public async Task<bool> SavePermissionsAsync(List<Permission> permissionTrees, Guid appId)
        {
            var equality = new PermissionEquality();
            //var dbPermissions = UnitOfWork.Context.Set<Permission>()
            //    .Where(m => m.ApplicationId == appId).ToList();

            #region Level 1
            //处理页面权限（parentId == null）
            var dbAllL1Permissions = await UnitOfWork.Context.Set<Permission>().Where(m => m.ParentId == null && m.ApplicationId == appId).AsNoTracking().ToListAsync();

            //需要添加的页面权限，同时授权给默认的角色（Level 1）
            var addL1List = permissionTrees.Except(dbAllL1Permissions, equality).ToList();
            addL1List.ForEach(async m =>
            {
                //不能同时添加其所属的子菜单（ChildNodes）：通过浅拷贝，将引用类型对象置为空
                var cloneM = m.Clone() as Permission;
                cloneM.ChildNodes = null;
                //授权给默认的角色
                if (!string.IsNullOrEmpty(cloneM.DefaultRoleId))
                    cloneM.PermissionRoles.Add(new PermissionsInRoles() { RoleId = cloneM.DefaultRoleId, Permission = cloneM });
                await UnitOfWork.Context.Set<Permission>().AddAsync(cloneM);
            });

            if (addL1List.Any())
                await UnitOfWork.Context.SaveChangesAsync();

            var dbPermissions = UnitOfWork.Context.Set<Permission>()
                .Where(m => m.ApplicationId == appId).ToList();

            var dbL1Permissions = dbPermissions.Where(m => m.ParentId == null).ToList();
            //需要删除的页面权限（Level 1）
            var delL1List = dbL1Permissions.Except(permissionTrees, equality).ToList();
            delL1List.ForEach(m => m.IsDeleted = true);
            //需要编辑的页面权限（Level 1）
            var editL1List = dbL1Permissions.Intersect(permissionTrees, equality).ToList();
            foreach (var dbPermission in editL1List)
            {
                var permission = permissionTrees.FirstOrDefault(m => GetPermissionHash(m).Equals(GetPermissionHash(dbPermission), StringComparison.OrdinalIgnoreCase));
                if (permission != null)
                {
                    dbPermission.AreaName = permission.AreaName;
                    dbPermission.ControllerName = permission.ControllerName;
                    dbPermission.ActionName = permission.ActionName;
                    dbPermission.Name = permission.Name;
                    dbPermission.Parameters = permission.Parameters;
                    dbPermission.ResultType = permission.ResultType;
                    dbPermission.Description = permission.Description;
                    dbPermission.Index = permission.Index;
                    dbPermission.Leaf = permission.Leaf;
                    dbPermission.Level = permission.Level;
                    dbPermission.IsDeleted = false;
                    dbPermission.AuthorityId = permission.AuthorityId;
                }
            }
            #endregion

            #region Level 2

            //处理页面的操作权限（parentId != null）
            var dbL2Permissions = dbPermissions.Where(m => m.ParentId != null).ToList();
            var childPermissions = permissionTrees.SelectMany(m => m.ChildNodes).ToList();

            //需要删除的页面权限（Level 2）
            var delL2List = dbL2Permissions.Except(childPermissions, equality).ToList();
            delL2List.ForEach(m => m.IsDeleted = true);
            //需要添加的操作权限，同时授权给默认的角色（Level 2）
            var addL2List = childPermissions.Except(dbL2Permissions, equality).ToList();
            addL2List.ForEach(async m =>
            {
                var addParentNode = permissionTrees.FirstOrDefault(p => p.ChildNodes.Contains(m));
                if (addParentNode != null)
                {
                    var dbParentNode = dbPermissions.FirstOrDefault(p => GetPermissionHash(p).Equals(GetPermissionHash(addParentNode), StringComparison.OrdinalIgnoreCase));
                    if (dbParentNode != null)
                    {
                        m.Id = 0;
                        m.ParentId = dbParentNode.Id;
                    }

                        //不能同时添加其所属的子菜单（ChildNodes）：通过浅拷贝，将引用类型对象置为空
                        var cloneM = m.Clone() as Permission;
                    cloneM.ChildNodes = null;
                        //授权给默认的角色
                        if (!string.IsNullOrEmpty(cloneM.DefaultRoleId))
                        cloneM.PermissionRoles.Add(new PermissionsInRoles() { RoleId = cloneM.DefaultRoleId, Permission = cloneM });
                    await UnitOfWork.Context.Set<Permission>().AddAsync(cloneM);
                }
            });
            //需要编辑的操作权限（Level 2）
            var editL2List = dbL2Permissions.Intersect(childPermissions, equality).ToList();
            foreach (var dbPermission in editL2List)
            {
                var permission = childPermissions.FirstOrDefault(m => GetPermissionHash(m).Equals(GetPermissionHash(dbPermission), StringComparison.OrdinalIgnoreCase));
                if (permission != null)
                {
                    dbPermission.AreaName = permission.AreaName;
                    dbPermission.ControllerName = permission.ControllerName;
                    dbPermission.ActionName = permission.ActionName;
                    dbPermission.Name = permission.Name;
                    dbPermission.Parameters = permission.Parameters;
                    dbPermission.ResultType = permission.ResultType;
                    dbPermission.Description = permission.Description;
                    dbPermission.Index = permission.Index;
                    dbPermission.Leaf = permission.Leaf;
                    dbPermission.Level = permission.Level;
                    dbPermission.IsDeleted = false;
                    dbPermission.AuthorityId = permission.AuthorityId;
                }
            }

            await UnitOfWork.Context.SaveChangesAsync();
            #endregion

            await base.UpdateExtendFieldsByFilterAsync(m => m.ApplicationId == appId);

            //await SetSystemAdminRoleDefaultPermissions();

            #region 将权限授权给默认的角色

            Expression<Func<PermissionsInRoles, bool>> predicate = null;
            delL1List.ForEach(m =>
            {
                predicate = predicate == null
                            ? r => r.PermissionId == m.Id
                            : predicate.Or(r => r.PermissionId == m.Id);
            });
            delL2List.ForEach(m =>
            {
                predicate = predicate == null
                            ? r => r.PermissionId == m.Id
                            : predicate.Or(r => r.PermissionId == m.Id);
            });

            if (predicate != null)
            {
                var dbMenusInRoles = await UnitOfWork.Context.Set<PermissionsInRoles>()
                    .Where(predicate).ToListAsync();
                if (dbMenusInRoles.Any())
                {
                    UnitOfWork.Context.Set<PermissionsInRoles>().RemoveRange(dbMenusInRoles);
                    await UnitOfWork.Context.SaveChangesAsync();
                }
            }

            #endregion

            return true;

        }

        private string GetPermissionHash(Permission x)
        {
            return string.Format("/{0}/{1}/{2}/{3}",
                string.IsNullOrEmpty(x.AreaName) ? string.Empty : x.AreaName.Trim().ToLower(),
                string.IsNullOrEmpty(x.ControllerName) ? string.Empty : x.ControllerName.Trim().ToLower(),
                string.IsNullOrEmpty(x.ActionName) ? string.Empty : x.ActionName.Trim().ToLower(),
                x.ApplicationId == null ? string.Empty : x.ApplicationId.ToString().Trim().ToLower()
                );
        }

        /// <summary>
        /// 设置系统管理员角色的默认权限
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SetSystemAdminRoleDefaultPermissions()
        {
            if (!(UnitOfWork.Context is ComAccountContext))
                return true;

            var equality = new PermissionsInRolesEquality();

            Expression<Func<Permission, bool>> predicate = m => m.ApplicationId == ApplicationConstant.AccAppId;
            var tenant = (UnitOfWork.Context as ComAccountContext).TenantName;
            if (tenant.Equals(TenantConstant.DbaTenantName, StringComparison.OrdinalIgnoreCase))
            {
                predicate = predicate.Or(m => m.ApplicationId == ApplicationConstant.AdminAppId);
            }

            var allMenus = await UnitOfWork.Context.Set<Permission>()
                        .Where(predicate).ToListAsync();
            var allMenuIds = allMenus.Select(m => m.Id);
            var allMenusInRoles = allMenus.Where(m => !m.IsDeleted).Select(m =>
            {
                return new PermissionsInRoles()
                {
                    RoleId = RoleConstants.AdminRoleId,
                    PermissionId = m.Id,
                };
            });

            var dbMenusInRoles = await UnitOfWork.Context.Set<PermissionsInRoles>()
                        .Where(m => allMenuIds.Contains(m.PermissionId) && m.RoleId == RoleConstants.AdminRoleId)
                        .ToListAsync();

            //添加新增系统管理员角色的菜单
            var addList = allMenusInRoles.Except(dbMenusInRoles, equality).ToList();
            addList.ForEach(async m =>
            {
                await UnitOfWork.Context.Set<PermissionsInRoles>().AddAsync(m);
            });
            //删除系统管理员角色的菜单
            var delList = dbMenusInRoles.Except(allMenusInRoles, equality).ToList();
            UnitOfWork.Context.Set<PermissionsInRoles>().RemoveRange(delList);


            return await UnitOfWork.Context.SaveChangesAsync() > 0;
        }
    }
}
