using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Common;
using KC.Framework.Tenant;
using KC.Service.Extension;
using KC.Service.DTO.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using KC.Service.Account;
using KC.Framework.Base;
using KC.Service.DTO;
using AutoMapper;

namespace KC.Web.Account.Controllers
{
    public class RoleController : AccountBaseController
    {
        private IMenuService MenuService;
        private IPermissionService PermissionService;

        public RoleController(
            Tenant tenant,
            IMenuService menuService,
            IPermissionService permissionService,

            IServiceProvider serviceProvider,
            ILogger<RoleController> logger)
            : base(tenant, serviceProvider, logger)
        {
            MenuService = menuService;
            PermissionService = permissionService;
        }

        #region  Role
        /// <summary>
        /// 三级级菜单：系统管理/组织管理/角色管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("组织管理", "角色管理", "/Role/Index",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-sitemap", AuthorityId = "EF1E670D-BA19-4536-82F4-7D78DB779C3B",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("角色管理", "角色管理", "/Role/Index", "EF1E670D-BA19-4536-82F4-7D78DB779C3B",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult Index()
        {
            ViewBag.BusinessTypes = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<BusinessType>();
            return View();
        }

        //[Web.Extension.PermissionFilter("角色管理", "加载角色列表数据", "/Role/GetRoleListResult",
        //    DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "30961CEE-683C-45B6-94E6-F3E552CB15EE")]
        public JsonResult GetRoleListResult(int page = 1, int rows = 20, string name = null)
        {
            // 系统管理员及人事经理的角色
            if (IsSystemAdmin || CurrentUserRoleIds.Any(m => m == RoleConstants.HrManagerRoleId))
            {
                return Json(RoleService.FindPagenatedRoleList(page, rows, name, null));
            }
            else
            {
                return Json(RoleService.FindPagenatedRoleList(page, rows, name, CurrentUserId));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public PartialViewResult GetRoleForm(string id, bool bl)
        {
            var model = new RoleDTO();
            model.IsEditMode = false;
            if (bl)
            {
                model = RoleService.GetDetailRoleByRoleId(id);//修改 
                model.IsEditMode = true;
                ViewBag.BusinessTypes = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<BusinessType>((int)model.BusinessType);
            }
            else
            {
                ViewBag.BusinessTypes = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<BusinessType>();
            }
            return PartialView("_roleForm", model);
        }

        [AllowAnonymous]
        public JsonResult ExistRoleName(string roleName, string orginalRoleName, bool isEditMode)
        {
            if (isEditMode && roleName.Equals(orginalRoleName, StringComparison.OrdinalIgnoreCase))
            {
                return Json(true);
            }

            var result = RoleService.ExistsRole(roleName);
            return Json(!result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Web.Extension.PermissionFilter("角色管理", "保存角色数据", "/Role/EditRole", "D1ADCD2E-DDAE-420E-BFC8-E4EE08EAFDB5",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult EditRole(RoleDTO role)
        {
            return GetServiceJsonResult(() =>
            {
                if (role.IsEditMode)
                {
                    //修改
                    role.ModifiedBy = CurrentUserId;
                    role.ModifiedName = CurrentUserDisplayName;
                    role.ModifiedDate = DateTime.UtcNow;
                    role.RoleName = role.DisplayName;
                    return RoleService.UpdateRole(role, CurrentUserId, CurrentUserDisplayName);
                }
                else
                {
                    //新增
                    role.RoleName = role.DisplayName;
                    role.RoleId = Guid.NewGuid().ToString();
                    role.CreatedBy = CurrentUserId;
                    role.CreatedName = CurrentUserDisplayName;
                    role.CreatedDate = DateTime.UtcNow;
                    role.ModifiedBy = CurrentUserId;
                    role.ModifiedName = CurrentUserDisplayName;
                    role.ModifiedDate = DateTime.UtcNow;
                    return RoleService.CreateRole(role, CurrentUserId, CurrentUserDisplayName);
                }
            });
        }

        [Web.Extension.PermissionFilter("角色管理", "删除角色数据", "/Role/RomoveRole", "6759EA9B-4F19-4A17-ACFF-0FED6CFFEF12",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult RomoveRole(string roleId)
        {
            return GetServiceJsonResult(() =>
            {
                var role = RoleService.GetDetailRoleByRoleId(roleId);

                if (role.UserIds.Any())
                {
                    throw new ArgumentException("角色下面还分配有相关的用户，请移除关联用户后再重试！");
                }
                //先移除角色菜单之间的联系
                if (role.MenuNodes.Any())
                {
                    RoleService.UpdateMenuInRole(roleId, new List<int>(), CurrentUserId, CurrentUserDisplayName);
                }
                //在移除角色权限之间的联系
                if (role.Permissions.Any())
                {
                    RoleService.UpdatePermissionInRole(roleId, new List<int>(), CurrentUserId,
                        CurrentUserDisplayName);
                }
                return RoleService.RomoveRole(roleId, CurrentUserId, CurrentUserDisplayName);
            });
        }

        /// <summary>
        /// 三级级菜单：系统管理/组织管理/角色详情
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("组织管理", "角色详情", "/Role/RoleDetail",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-sitemap", AuthorityId = "0C53ECAA-D62D-4EAD-9419-A6F904857E85",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 6, IsExtPage = true, Level = 3)]
        [Web.Extension.PermissionFilter("角色管理", "角色详情", "/Role/RoleDetail", "0C53ECAA-D62D-4EAD-9419-A6F904857E85",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 4, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult RoleDetail(string roleId, bool isSystemRole)
        {
            var model = RoleService.GetDetailRoleByRoleId(roleId);
            ViewBag.roleId = roleId;
            ViewBag.isSystemRole = isSystemRole;
            ViewBag.canEditRole = base.IsSystemAdmin;
            ViewBag.Users = SerializeHelper.ToJson(model.Users);
            
            var result = new List<MenuNodeDTO>();
            var menus = model.MenuNodes.OrderBy(m => m.Index).ToList();
            foreach (var parent in menus.Where(m => m.ParentId == null))
            {
                KC.Service.Util.TreeNodeUtil.NestTreeNodeDTO(parent, menus, null, null);
                result.Add(parent);
            }
            ViewBag.Menus = SerializeHelper.ToJson(result);

            return View(model);
        }

        /// <summary>
        /// 获取当前应用所有权限
        /// </summary>
        /// <param name="roleId">选中的权限</param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<JsonResult> GetRoleData(string roleId)
        {
            var model = new RoleData();
            Guid roleGuid;
            var success = Guid.TryParse(roleId, out roleGuid);
            if (success)
                model.RoleId = roleGuid;

            model.Permissions = new List<PermissionDTO>();
            model.RoleList = new List<RoleData.Dictionarys>();
            var data = await GetOwnSimpleRoles();
            if (data != null && data.Any())
            {
                foreach (var itme in data)
                {
                    //角色列表
                    var roleitme = new RoleData.Dictionarys();
                    if (itme.RoleId == roleId)
                    {
                        roleitme.@checked = true;
                    }
                    roleitme.Key = itme.DisplayName;
                    roleitme.Value = itme.RoleId;
                    model.RoleList.Add(roleitme);
                }
            }
                
            return Json(model);
        }
        #endregion

        #region  Menu in Role

        /// <summary>
        /// 三级级菜单：系统管理/组织管理/角色菜单管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("组织管理", "角色菜单管理", "/Role/MenuInRole",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-sitemap", AuthorityId = "C9385B3E-4FCA-46E7-ABFA-5EE6042F8787",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsExtPage = true, Level = 3)]
        [Web.Extension.PermissionFilter("角色菜单管理", "角色菜单管理", "/Role/MenuInRole", "C9385B3E-4FCA-46E7-ABFA-5EE6042F8787",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsPage = true, ResultType = ResultType.ActionResult)]
        public ActionResult MenuInRole(string roleId, bool isSystemRole)
        {
            ViewBag.roleId = roleId;
            ViewBag.isSystemRole = isSystemRole;
            ViewBag.canEditRole = base.IsSystemAdmin;
            return View("MenuInRole");
        }
        [AllowAnonymous]
        public async Task<JsonResult> GetMenuInRole(string rid)
        {
            List<MenuNodeSimpleDTO> data;
            if (IsSystemAdmin)
            {
                data = await MenuService.FindAllSimpleMenuTreesAsync();
            }
            else
            {
                Func<MenuNodeSimpleDTO, bool> predict = m => true;
                data = await GetCurrentUserMenuTree(predict);
            }

            if (string.IsNullOrWhiteSpace(rid))
            {
                return Json(data);
            }
            
            List<MenuNodeSimpleDTO> selectedMenus;
            var menus = await RoleService.FindUserMenusByRoleIdsAsync(new List<string>() { rid });
            if (menus != null && menus.Any())
            {
                var checkIds = menus.Select(m => m.Id).ToList();
                selectedMenus = Service.Util.TreeNodeUtil.GetTreeNodeSimpleWithChildren (data, checkIds);
            }
            else
            {
                selectedMenus = data;
            }
            
            return Json(selectedMenus);
        }

        //角色管理页角色菜单
        [Web.Extension.PermissionFilter("角色菜单管理", "保存角色菜单数据", "/Role/SubmitMenuInRole", "49AB8484-621B-4518-9049-191ADF948A2C",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult SubmitMenuInRole(string roleId, List<int> addList)
        {
            return base.GetServiceJsonResult(() =>
            {
                if (string.IsNullOrWhiteSpace(roleId))
                    throw new ArgumentNullException("roleId", "角色Id为空。");

                if (addList == null)
                    addList = new List<int>();
                
                var result = false;
                var newIds = addList.Distinct().ToList();

                //修改角色菜单
                result = RoleService.UpdateMenuInRole(roleId, newIds, CurrentUserId, CurrentUserDisplayName);
                //角色分配菜单成功，清除用户缓存，以便重新获取角色权限
                if (result)
                    ClearUserCacheByRoles(new List<string> { roleId });

                return result;
            });
        }
        #endregion

        #region  Permission in Role
        /// <summary>
        /// 三级级菜单：系统管理/组织管理/角色权限管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("组织管理", "角色权限管理", "/Role/PermissionInRole",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-sitemap", AuthorityId = "0DE8B69B-75BD-42AC-B0F1-C957F52FF021",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 4, IsExtPage = true, Level = 3)]
        [Web.Extension.PermissionFilter("角色权限管理", "角色权限管理", "/Role/PermissionInRole", "0DE8B69B-75BD-42AC-B0F1-C957F52FF021",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsPage = true, ResultType = ResultType.ActionResult)]
        public ActionResult PermissionInRole(string roleId, bool isSystemRole)
        {
            ViewBag.roleId = roleId;
            ViewBag.isSystemRole = isSystemRole;
            ViewBag.canEditRole = base.IsSystemAdmin;
            return View("PermissionInRole");
        }
        [AllowAnonymous]
        public async Task<JsonResult> GetPermissionInRole(string rid)
        {
            List<PermissionSimpleDTO> data;
            if (IsSystemAdmin)
            {
                data = await PermissionService.FindAllSimplePermissionTreesAsync();
            }
            else
            {
                data = await GetCurrentPermissionTree();
            }

            if (string.IsNullOrWhiteSpace(rid))
            {
                return Json(data);
            }
            List<PermissionSimpleDTO> selectedPermissions;
            var permissions = await RoleService.FindUserPermissionsByRoleIdsAsync(new List<string>() { rid });
            if (permissions != null && permissions.Any())
            {
                var checkIds = permissions.Select(m => m.Id).ToList();
                selectedPermissions = Service.Util.TreeNodeUtil.GetTreeNodeSimpleWithChildren(data, checkIds).ToList();
            }
            else
            {
                selectedPermissions = data;
            }

            return Json(selectedPermissions);
        }

        //角色管理页角色权限
        [Web.Extension.PermissionFilter("角色权限管理", "保存角色权限数据", "/Role/SubmitPermissionInRole", "B457B18B-B680-4117-8E72-98284E9151D5",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 4, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult SubmitPermissionInRole(string roleId, List<int> addList)
        {
            return base.GetServiceJsonResult(() =>
            {
                if (string.IsNullOrWhiteSpace(roleId))
                    throw new ArgumentNullException("roleId", "角色Id为空。");
                
                if (addList == null)
                    addList = new List<int>();

                var result = false;
                var newIds = addList.Distinct().ToList();

                //修改角色权限
                result = RoleService.UpdatePermissionInRole(roleId, newIds, CurrentUserId,
                    CurrentUserDisplayName);
                if (result)
                {
                    ClearUserCacheByRoles(new List<string> { roleId });
                }
                return result;
            });
        }

        #endregion

        #region User In Role
        /// <summary>
        /// 三级级菜单：系统管理/组织管理/角色用户管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("组织管理", "角色用户管理", "/Role/UserInRole",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-sitemap", AuthorityId = "F448F3D1-47D9-4066-9A42-D405F311C309",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 5, IsExtPage = true, Level = 3)]
        [Web.Extension.PermissionFilter("角色用户管理", "角色用户管理", "/Role/UserInRole", "F448F3D1-47D9-4066-9A42-D405F311C309",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 4, IsPage = true, ResultType = ResultType.ActionResult)]
        public ActionResult UserInRole(string roleId, bool isSystemRole)
        {
            ViewBag.RoleId = roleId;
            ViewBag.isSystemRole = isSystemRole;
            ViewBag.canEditRole = base.IsSystemAdmin;
            return View("UserInRole");
        }

        //角色管理=》编辑角色用户load方法
        [AllowAnonymous]
        public JsonResult LoadUserLeftInRoseList(int page = 1, int rows = 10, string email = "", string phone = "", string name = "",  string roleId = "")
        {
            if (roleId == "")
            {
                return null;
            }

            //全部用户
            var result = AccountService.FindPaginatedUsersByOrgIds(page, rows, email, phone,
                name, null, null, null);
            var resultData = new List<UserDTO>();
            //排除已经选择该角色了的用户
            result.rows.ForEach(c =>
            {
                if (c.RoleIds.Count(a => a == roleId) == 0)
                {
                    resultData.Add(c);
                }
            });
            // 系统管理员及人事经理的角色
            if (IsSystemAdmin || CurrentUserRoleIds.Any(m => m == RoleConstants.HrManagerRoleId))
            {
                return Json(GetPaginatedUsersPagedAgain(page, rows, resultData.Distinct().ToList()));
            } 
            else
            {
                //获取当前登录用户的部门ID，只查询出当前登录用户部门下的用户 
                var allUser = result.rows.FirstOrDefault(c => c.UserId == CurrentUserId);
                var resultlist = new List<UserDTO>();
                if (allUser != null)
                {
                    var currentOrgan = CurrentUser.OrganizationIds;
                    var organIdLists = SysManageService.FindOrganizationsByIdList(currentOrgan);
                    //先根据当前登录用户部门筛选一道
                    for (int j = 0; j < resultData.Count(); j++)
                    {
                        for (int i = 0; i < organIdLists.Count(); i++)
                        {
                            if (resultData[j].OrganizationIds.Contains(organIdLists[i]))
                            {
                                resultlist.Add(result.rows[j]);
                            }
                        }
                    }
                }
                return Json(GetPaginatedUsersPagedAgain(page, rows, resultlist.Distinct().ToList()));
            }
            
        }

        //重新分页
        private PaginatedBaseDTO<UserDTO> GetPaginatedUsersPagedAgain(int pageIndex, int pageSize, List<UserDTO> list)
        {
            var total = list.Count();
            var rows = list.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            var model = new PaginatedBaseDTO<UserDTO>(pageIndex, pageSize, total, rows.ToList());
            return model;
        }

        [AllowAnonymous]
        public async Task<JsonResult> GetUserInRole(string roleId)
        {
            var result = new RoleData
            {
                Users = new List<UserSimpleDTO>(),
                RoleList = new List<RoleData.Dictionarys>()
            };

            //所有用户
            var userlist = AccountService.FindAllUsersWithRolesAndOrgs();
            foreach (var userDto in userlist)
            {
                if (userDto.UserRoleIds.Contains(roleId))
                {
                    result.Users.Add(userDto);
                }
            }

            //所有权限
            var roleList = await GetOwnSimpleRoles();
            if (roleList != null && roleList.Any())
            {
                foreach (var roledto in roleList)
                {
                    var role = new RoleData.Dictionarys
                    {
                        Key = roledto.DisplayName,
                        Value = roledto.RoleId,
                        Itms = roledto.Users.Select(m => m.UserId).ToList()
                    };
                    result.RoleList.Add(role);
                }
            }
                
            return Json(result);
        }

        //角色管理页角色用户
        [Web.Extension.PermissionFilter("角色用户管理", "保存角色用户数据", "/Role/SubmitUserInRole", "D03C4451-C709-4EA9-94EF-215845FE744C",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult SubmitUserInRole(string roleId, List<string> addList, List<string> delList)
        {
            return base.GetServiceJsonResult(() =>
            {
                if (string.IsNullOrWhiteSpace(roleId))
                    throw new ArgumentNullException("roleId", "角色Id为空");
                
                if (addList == null)
                    addList = new List<string>();
                if (delList == null)
                    delList = new List<string>();

                var add = addList.Distinct().ToList();
                var del = delList.Distinct().ToList();
                var delIds = del.Except(add).ToList();
                var addIds = addList.Except(del).ToList();
                //角色分配用户
                var result = RoleService.UpdateUserInRole(roleId, addIds, delIds, CurrentUserId, CurrentUserDisplayName);
                if (result)
                    ClearUserCacheByRoles(new List<string> { roleId });
                return result;
            });
        }
        #endregion

    }

    public class RoleData
    {
        public Guid RoleId { get; set; }

        public List<PermissionDTO> Permissions { get; set; }

        public List<MenuNodeDTO> MenuNodes { get; set; }

        public List<UserSimpleDTO> Users { get; set; }

        public List<Dictionarys> RoleList { get; set; }

        public class Dictionarys
        {
            public string Key { get; set; }
            public string Value { get; set; }

            public bool @checked { get; set; }
            public List<string> Itms { get; set; }

            public Guid AppId { get; set; }
        }
    }
}