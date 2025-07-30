using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KC.Database.EFRepository;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Model.Account;


namespace KC.DataAccess.Account.Repository
{
    public class MenuNodeRepository : EFTreeRepositoryBase<MenuNode>, IMenuNodeRepository
    {
        public MenuNodeRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        public IList<MenuNode> GetMenuNodesByIds(List<int> menuIds)
        {
            return Entities
                .Where(m => menuIds.Contains(m.Id) && !m.IsDeleted)
                .AsNoTracking()
                .OrderBy(m => m.Index)
                .ToList();
        }
        public IList<MenuNode> GetMenuNodesByRoleIds(List<string> roleIds)
        {
            return Entities
                .Where(m => m.MenuRoles.Any(r => roleIds.Contains(r.RoleId)) && !m.IsDeleted)
                .AsNoTracking()
                .OrderBy(m => m.Index)
                .ToList();
        }

        public MenuNode GetMenuById(int id)
        {
            return Entities
                .AsNoTracking()
                .FirstOrDefault(m => m.Id == id);
        }

        public MenuNode GetDetailMenuById(int id)
        {
            return Entities
                .Include(m => m.MenuRoles)
                .ThenInclude(r => r.Role)
                .Include(m => m.ChildNodes)
                .AsNoTracking()
                .FirstOrDefault(m => m.Id == id);
        }

        public MenuNode GetMenuByFilter(Expression<Func<MenuNode, bool>> predicate)
        {
            return Entities.AsNoTracking().FirstOrDefault(predicate);
        }

        public bool SaveRoleInMenu(int id, List<string> newIds, string operatorId, string operatorName)
        {
            var menu = Entities
                .Include(m => m.MenuRoles)
                //.ThenInclude(r => r.Role)
                .FirstOrDefault(m => m.Id == id);
            if (menu == null)
                throw new ArgumentException("id", string.Format("系统不存在菜单Id={0}的菜单。", id));

            if (newIds == null)
                throw new ArgumentNullException("newIds", "传入参数--newIds：菜单Id列表为空。");

            var dbIdList = new List<string>();
            dbIdList.AddRange(menu.MenuRoles.Select(roleDto => roleDto.RoleId));

            var delList = dbIdList.Except(newIds);
            var addList = newIds.Except(dbIdList);

            //https://github.com/dotnet/efcore/issues/17342
            var addr = this.EFContext.Context.Set<Role>()
                .AsNoTracking()
                .Where(m => addList.Contains(m.Id))
                .ToList();
            foreach (var role in addr)
            {
                menu.MenuRoles.Add(new MenuNodesInRoles() { RoleId = role.Id, MenuNodeId = id });
            }

            var delr = new List<MenuNodesInRoles>();
            foreach (var role in menu.MenuRoles)
            {
                if (delList.Contains(role.RoleId))
                    delr.Add(role);
            }
            foreach (var role in delr)
            {
                menu.MenuRoles.Remove(role);
            }

            if (!addr.Any() && !delr.Any())
                return true;

            const string logMessage = "{0}在{1}进行了以下操作：{2}！";
            string operMessage = string.Format("更新了菜单（{0}）角色--添加了角色({1})，删除了角色({2})",
                menu.Id + ": " + menu.Name,
                addr.Select(m => m.Id + ": " + m.Name).ToCommaSeparatedString(),
                delr.Select(m => m.RoleId + ": " + m.Role?.Name).ToCommaSeparatedString());
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

        public async Task<bool> SaveMenusAsync(List<MenuNode> menuTrees, Guid appId)
        {
            var equality = new MenuNodeEquality();

            #region 根据传入数据更新数据库已经存在的菜单的Level层级

            var hasChange = false;
            var dbAllMenus = await UnitOfWork.Context.Set<MenuNode>().Where(m => m.ApplicationId == appId).ToListAsync();
            if (dbAllMenus.Any())
            {
                menuTrees.ForEach(l1 =>
                {
                    var dbL1Menu = dbAllMenus.FirstOrDefault(m => GetMenuHash(m).Equals(GetMenuHash(l1)));
                    if (dbL1Menu != null)
                    {
                        hasChange = true;
                        dbL1Menu.Level = l1.Level;
                        dbL1Menu.IsDeleted = false;
                    }

                    l1.ChildNodes.ForEach(l2 =>
                    {
                        var dbL2Menu = dbAllMenus.FirstOrDefault(m => GetMenuHash(m).Equals(GetMenuHash(l2)));
                        if (dbL2Menu != null)
                        {
                            hasChange = true;
                            dbL2Menu.Level = l2.Level;
                            dbL2Menu.IsDeleted = false;
                        }

                        l2.ChildNodes.ForEach(l3 =>
                        {
                            var dbL3Menu = dbAllMenus.FirstOrDefault(m => GetMenuHash(m).Equals(GetMenuHash(l3)));
                            if (dbL3Menu != null)
                            {
                                hasChange = true;
                                dbL3Menu.Level = l3.Level;
                                dbL3Menu.IsDeleted = false;
                            }
                        });
                    });
                });

                if (hasChange)
                    await UnitOfWork.Context.SaveChangesAsync();
            }

            #endregion

            #region Level 1
            //处理页面权限（parentId == null）
            //var dbL1menus = UnitOfWork.Context.Set<MenuNode>().Where(m => m.ApplicationId == appId && m.Level == 1).AsNoTracking().ToList();
            var dbL1menus = dbAllMenus.Where(m => m.Level == 1).ToList();

            //需要添加的页面权限，同时授权给默认的角色（Level 1）
            var addL1List = menuTrees.Except(dbL1menus, equality).ToList();
            addL1List.ForEach(async m =>
            {
                //不能同时添加其所属的子菜单（ChildNodes）：通过浅拷贝，将引用类型对象置为空
                var cloneM = m.Clone() as MenuNode;
                cloneM.ChildNodes = null;
                cloneM.MenuRoles.Clear();
                //授权给默认的角色
                if (!string.IsNullOrEmpty(cloneM.DefaultRoleId))
                    cloneM.MenuRoles.Add(new MenuNodesInRoles() { RoleId = cloneM.DefaultRoleId, MenuNode = cloneM });
                await UnitOfWork.Context.Set<MenuNode>().AddAsync(cloneM);
            });

            if (addL1List.Any())
                await UnitOfWork.Context.SaveChangesAsync();

            //需要删除的页面权限（Level 1）
            var delL1List = dbL1menus.Except(menuTrees, equality).ToList();
            delL1List.ForEach(m => m.IsDeleted = true);

            //需要编辑的页面权限（Level 1）
            var editL1List = dbL1menus.Intersect(menuTrees, equality).ToList();
            foreach (var dbmenu in editL1List)
            {
                var menu = menuTrees.FirstOrDefault(m => GetMenuHash(m).Equals(GetMenuHash(dbmenu), StringComparison.OrdinalIgnoreCase));
                if (menu != null)
                {
                    dbmenu.AreaName = menu.AreaName;
                    dbmenu.ControllerName = menu.ControllerName;
                    dbmenu.ActionName = menu.ActionName;
                    dbmenu.Name = menu.Name;
                    //dbmenu.Parameters = menu.Parameters;
                    dbmenu.Description = menu.Description;
                    dbmenu.IsExtPage = menu.IsExtPage;
                    dbmenu.Index = menu.Index;
                    dbmenu.Leaf = menu.Leaf;
                    dbmenu.Level = menu.Level;
                    dbmenu.TenantType = menu.TenantType;
                    dbmenu.Version = menu.Version;
                    dbmenu.SmallIcon = menu.SmallIcon;
                    dbmenu.AuthorityId = menu.AuthorityId;
                }
            }

            #endregion

            #region Level 2

            var dbL12menus = await UnitOfWork.Context.Set<MenuNode>().Where(m => m.ApplicationId == appId && (m.Level == 1 || m.Level == 2)).ToListAsync();
            //var dbL12menus = dbAllMenus.Where(m => m.Level == 1 || m.Level == 2).ToList();

            //处理页面的操作权限（parentId != null）
            var dbL2menus = dbL12menus.Where(m => m.Level == 2).ToList();
            var childL2menus = menuTrees.SelectMany(m => m.ChildNodes).ToList();

            //需要删除的页面权限（Level 2）
            var delL2List = dbL2menus.Except(childL2menus, equality).ToList();
            delL2List.ForEach(m => m.IsDeleted = true);
            //需要添加的操作权限，同时授权给默认的角色（Level 2）
            var addL2List = childL2menus.Except(dbL2menus, equality).ToList();
            addL2List.ForEach(async m =>
            {
                var addParentNode = menuTrees.FirstOrDefault(p => p.ChildNodes.Contains(m));
                if (addParentNode != null)
                {
                    var dbParentNode = dbL12menus.FirstOrDefault(p => GetMenuHash(p).Equals(GetMenuHash(addParentNode), StringComparison.OrdinalIgnoreCase));
                    if (dbParentNode != null)
                    {
                        m.Id = 0;
                        m.ParentId = dbParentNode.Id;
                    }
                    //不能同时添加其所属的子菜单（ChildNodes）：通过浅拷贝，将引用类型对象置为空
                    var cloneM = m.Clone() as MenuNode;
                    cloneM.ChildNodes = null;
                    cloneM.MenuRoles.Clear();
                    //授权给默认的角色
                    if (!string.IsNullOrEmpty(cloneM.DefaultRoleId))
                    cloneM.MenuRoles.Add(new MenuNodesInRoles() { RoleId = cloneM.DefaultRoleId, MenuNode = cloneM });
                    await UnitOfWork.Context.Set<MenuNode>().AddAsync(cloneM);
                }
            });
            //需要编辑的页面权限（Level 2）
            var editL2List = dbL2menus.Intersect(childL2menus, equality).ToList();
            foreach (var dbmenu in editL2List)
            {
                var menu = childL2menus.FirstOrDefault(m => GetMenuHash(m).Equals(GetMenuHash(dbmenu), StringComparison.OrdinalIgnoreCase));
                if (menu != null)
                {
                    dbmenu.AreaName = menu.AreaName;
                    dbmenu.ControllerName = menu.ControllerName;
                    dbmenu.ActionName = menu.ActionName;
                    dbmenu.Name = menu.Name;
                    //dbmenu.Parameters = menu.Parameters;
                    dbmenu.Description = menu.Description;
                    dbmenu.IsExtPage = menu.IsExtPage;
                    dbmenu.Index = menu.Index;
                    dbmenu.Leaf = menu.Leaf;
                    dbmenu.Level = menu.Level;
                    dbmenu.TenantType = menu.TenantType;
                    dbmenu.Version = menu.Version;
                    dbmenu.SmallIcon = menu.SmallIcon;
                    dbmenu.AuthorityId = menu.AuthorityId;

                    var editParentNode = menuTrees.FirstOrDefault(p => p.ChildNodes.Contains(menu));
                    if (editParentNode != null)
                    {
                        var editDbParentNode = dbL12menus.FirstOrDefault(m => GetMenuHash(m).Equals(GetMenuHash(editParentNode), StringComparison.OrdinalIgnoreCase));
                        if (editDbParentNode != null)
                            dbmenu.ParentId = editDbParentNode.Id;
                    }
                }
            }

            await UnitOfWork.Context.SaveChangesAsync();
            #endregion

            #region Level 3
            var dbL23menus = await UnitOfWork.Context.Set<MenuNode>().Where(m => m.ApplicationId == appId && (m.Level == 2 || m.Level == 3)).ToListAsync();
            //var dbL23menus = dbAllMenus.Where(m => m.Level == 2 || m.Level == 3).ToList();

            //处理页面的操作权限（parentId != null）
            var dbL3menus = dbL23menus.Where(m => m.Level == 3).ToList();
            var childL3menus = menuTrees.SelectMany(m => m.ChildNodes).SelectMany(m => m.ChildNodes).ToList();

            //需要删除的页面权限（Level 3）
            var delL3List = dbL3menus.Except(childL3menus, equality).ToList();
            delL3List.ForEach(m => m.IsDeleted = true);
            //需要添加的操作权限，同时授权给默认的角色（Level 3）
            var addL3List = childL3menus.Except(dbL3menus, equality).ToList();
            addL3List.ForEach(async m =>
            {
                var addParentNode = menuTrees.SelectMany(p => p.ChildNodes).FirstOrDefault(p => p.ChildNodes.Contains(m));
                if (addParentNode != null)
                {
                    var dbParentNode = dbL23menus.FirstOrDefault(p => GetMenuHash(p).Equals(GetMenuHash(addParentNode), StringComparison.OrdinalIgnoreCase));
                    if (dbParentNode != null)
                    {
                        m.Id = 0;
                        m.ParentId = dbParentNode.Id;
                        //m.ParentNode = dbParentNode;
                    }
                    m.MenuRoles.Clear();
                    //授权给默认的角色
                    if (!string.IsNullOrEmpty(m.DefaultRoleId))
                        m.MenuRoles.Add(new MenuNodesInRoles() { RoleId = m.DefaultRoleId, MenuNode = m });
                    await UnitOfWork.Context.Set<MenuNode>().AddAsync(m);
                }
            });
            //需要编辑的页面权限（Level 3）
            var editL3List = dbL3menus.Intersect(childL3menus, equality).ToList();
            foreach (var dbmenu in editL3List)
            {
                var menu = childL3menus.FirstOrDefault(m => GetMenuHash(m).Equals(GetMenuHash(dbmenu), StringComparison.OrdinalIgnoreCase));
                if (menu != null)
                {
                    dbmenu.AreaName = menu.AreaName;
                    dbmenu.ControllerName = menu.ControllerName;
                    dbmenu.ActionName = menu.ActionName;
                    dbmenu.Name = menu.Name;
                    //dbmenu.Parameters = menu.Parameters;
                    dbmenu.Description = menu.Description;
                    dbmenu.IsExtPage = menu.IsExtPage;
                    dbmenu.Index = menu.Index;
                    dbmenu.Leaf = menu.Leaf;
                    dbmenu.Level = menu.Level;
                    dbmenu.TenantType = menu.TenantType;
                    dbmenu.Version = menu.Version;
                    dbmenu.SmallIcon = menu.SmallIcon;
                    dbmenu.AuthorityId = menu.AuthorityId;

                    var editParentNode = menuTrees.SelectMany(m => m.ChildNodes).FirstOrDefault(p => p.ChildNodes.Contains(menu));
                    if (editParentNode != null)
                    {
                        var editDbParentNode = dbL23menus.FirstOrDefault(m => GetMenuHash(m).Equals(GetMenuHash(editParentNode), StringComparison.OrdinalIgnoreCase));
                        if (editDbParentNode != null)
                            dbmenu.ParentId = editDbParentNode.Id;
                    }
                }
            }

            await UnitOfWork.Context.SaveChangesAsync();
            #endregion

            await base.UpdateExtendFieldsByFilterAsync(m => m.ApplicationId == appId);

            //await SetSystemAdminRoleDefaultMenus();

            #region 将菜单授权给默认的角色

            Expression<Func<MenuNodesInRoles, bool>> predicate = null;
            delL1List.ForEach(m =>
            {
                predicate = predicate == null
                            ? r => r.MenuNodeId == m.Id
                            : predicate.Or(r => r.MenuNodeId == m.Id);
            });
            delL2List.ForEach(m =>
            {
                predicate = predicate == null
                            ? r => r.MenuNodeId == m.Id
                            : predicate.Or(r => r.MenuNodeId == m.Id);
            });
            delL3List.ForEach(m =>
            {
                predicate = predicate == null
                            ? r => r.MenuNodeId == m.Id
                            : predicate.Or(r => r.MenuNodeId == m.Id);
            });

            if (predicate != null)
            {
                var dbMenusInRoles = await UnitOfWork.Context.Set<MenuNodesInRoles>()
                    .Where(predicate).ToListAsync();
                if (dbMenusInRoles.Any())
                {
                    UnitOfWork.Context.Set<MenuNodesInRoles>().RemoveRange(dbMenusInRoles);
                    await UnitOfWork.Context.SaveChangesAsync();
                }
            }

            #endregion

            return true;
        }

        /// <summary>
        /// 设置系统管理员角色的默认菜单
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SetSystemAdminRoleDefaultMenus()
        {
            if (!(UnitOfWork.Context is ComAccountContext))
                return true;

            var equality = new MenuNodesInRolesEquality();

            Expression<Func<MenuNode, bool>> predicate = m => m.ApplicationId == ApplicationConstant.AccAppId;
            var tenant = (UnitOfWork.Context as ComAccountContext).TenantName;
            if (tenant.Equals(TenantConstant.DbaTenantName, StringComparison.OrdinalIgnoreCase))
            {
                predicate = predicate.Or(m => m.ApplicationId == ApplicationConstant.AdminAppId);
            }

            var allMenus = await UnitOfWork.Context.Set<MenuNode>()
                        .Where(predicate).ToListAsync();
            var allMenuIds = allMenus.Select(m => m.Id);
            var allMenusInRoles = allMenus.Where(m => !m.IsDeleted).Select(m =>
            {
                return new MenuNodesInRoles()
                {
                    RoleId = RoleConstants.AdminRoleId,
                    MenuNodeId = m.Id,
                };
            });

            var dbMenusInRoles = await UnitOfWork.Context.Set<MenuNodesInRoles>()
                        .Where(m => allMenuIds.Contains(m.MenuNodeId) && m.RoleId == RoleConstants.AdminRoleId)
                        .ToListAsync();

            //添加新增系统管理员角色的菜单
            var addList = allMenusInRoles.Except(dbMenusInRoles, equality).ToList();
            addList.ForEach(async m =>
            {
                await UnitOfWork.Context.Set<MenuNodesInRoles>().AddAsync(m);
            });
            //删除系统管理员角色的菜单
            var delList = dbMenusInRoles.Except(allMenusInRoles, equality).ToList();
            UnitOfWork.Context.Set<MenuNodesInRoles>().RemoveRange(delList);

            return await UnitOfWork.Context.SaveChangesAsync() > 0;
        }

        private string GetMenuHash(MenuNode x)
        {
            return string.Format("/{0}/{1}/{2}/{3}",
                string.IsNullOrEmpty(x.AreaName) ? string.Empty : x.AreaName.Trim().ToLower(),
                string.IsNullOrEmpty(x.ControllerName) ? string.Empty : x.ControllerName.Trim().ToLower(),
                string.IsNullOrEmpty(x.ActionName) ? string.Empty : x.ActionName.Trim().ToLower(),
                x.ApplicationId == null ? string.Empty : x.ApplicationId.ToString().Trim().ToLower()
                );
        }
    }
}
