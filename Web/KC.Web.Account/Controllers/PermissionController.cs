using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using KC.Service.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using KC.Service.DTO.Account;
using KC.Framework.Extension;
using Microsoft.AspNetCore.Mvc.Rendering;
using KC.Service.Extension;
using KC.Framework.Base;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace KC.Web.Account.Controllers
{
    public class PermissionController : AccountBaseController
    {
        private readonly IMapper Mapper;

        private IPermissionService PermissionService;
        public PermissionController(
            Tenant tenant,
            IRoleService roleService,
            IPermissionService permissionService,
            IMapper mapper,
            IServiceProvider serviceProvider,
            ILogger<RoleController> logger)
            : base(tenant, serviceProvider, logger)
        {
            PermissionService = permissionService;
            Mapper = mapper;
        }

        /// <summary>
        /// 三级级菜单：系统管理/菜单权限管理/权限管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("菜单权限管理", "权限管理", "/Permission/Index",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-sitemap", AuthorityId = "8F11A258-26DA-4415-BE93-5D2DA0B6BBED",
        DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("权限管理", "权限管理", "/Permission/Index", "8F11A258-26DA-4415-BE93-5D2DA0B6BBED",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult Index()
        {
            return View();
        }

        #region 权限管理

        /// <summary>
        /// 树形权限列表
        /// </summary>
        /// <param name="searchKey"></param>
        /// <returns></returns>
        public async Task<JsonResult> LoadPermissionList(string searchValue)
        {
            if (IsSystemAdmin)
            {
                var result = await PermissionService.FindRootPermissionsByNameAsync(searchValue);
                return Json(result);
            }
            var menuTress = await GetCurrentPermissionTree(searchValue);

            return Json(menuTress);
        }
        public async Task<JsonResult> LoadPermissionTrees()
        {
            var tree = await PermissionService.FindAllSimplePermissionTreesAsync();
            
            var rootMenu = new PermissionSimpleDTO() { Text = "顶级菜单", Children = tree };
            return Json(new List<PermissionSimpleDTO> { rootMenu });
        }

        public PartialViewResult PermissionForm(int id = 0)
        {
            var model = id != 0 ? PermissionService.GetDetailPermissionById(id) : new PermissionDTO();

            //返回类型
            var list = EnumExtensions.GetEnumDictionary<ResultType>();
            ViewBag.ResultTypes =
                list.Select(item => new SelectListItem { Text = item.Value, Value = item.Key.ToString() }).ToList();

            return PartialView("_permissionForm", model);
        }

        [AllowAnonymous]
        public async Task<JsonResult> ExistPermissionName(int id, string name, string orginalName, bool isEditMode)
        {
            if (isEditMode && name.Equals(orginalName, StringComparison.OrdinalIgnoreCase))
                return Json(false);

            return Json(await PermissionService.ExistPermissionNameAsync(id, name));
        }
        /// <summary>
        /// 编辑权限
        /// </summary>
        /// <param name="model"></param>
        /// <param name="permissionGroupIds"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Web.Extension.PermissionFilter("权限管理", "保存权限数据", "/Permission/SavePermission", "58679DF4-30DB-42E3-A344-07D4169F932D",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult SavePermission(PermissionDTO model, string permissionGroupIds)
        {
            return GetServiceJsonResult(() =>
            {
                if (model.ParentId.IsNullOrEmpty())
                {
                    model.Level = 1;
                    model.ParentId = null;
                }
                else
                {
                    model.Level = 2;
                }

                if (model.ParentId == model.Id && model.Id != 0)
                    throw new ArgumentException("不可用当前节点作父节点");
                
                if (model.IsEditMode)
                {
                    var permission = PermissionService.GetPermissionByAppIDAndName(model.ApplicationId,
                        model.ControllerName, model.ActionName);
                    var flag = permission != null && permission.Id != model.Id;
                    if (flag)
                        throw new ArgumentException("在一个应用下的控制器中，不可存在相同的ActionName");

                    model.ModifiedName = CurrentUserDisplayName;
                    model.ModifiedBy = CurrentUserId;
                    model.ModifiedDate = DateTime.UtcNow;

                    return PermissionService.UpdatePermission(model, CurrentUserId, CurrentUserDisplayName);
                }
                else
                {
                    model.CreatedName = CurrentUserDisplayName;
                    model.CreatedBy = CurrentUserId;
                    model.CreatedDate = DateTime.UtcNow;
                    model.ModifiedName = CurrentUserDisplayName;
                    model.ModifiedBy = CurrentUserId;
                    model.ModifiedDate = DateTime.UtcNow;

                    return PermissionService.CreatePermission(model, CurrentUserId, CurrentUserDisplayName);
                }
            });
        }
        [Web.Extension.PermissionFilter("权限管理", "删除权限数据", "/Permission/RemovePermission", "353C94F6-BC38-44F4-A599-2A56BE939D8E",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult RemovePermission(int id)
        {
            return GetServiceJsonResult(() =>
            {
                var permission = PermissionService.GetDetailPermissionById(id);
                if (permission.Children.Any())
                    throw new ArgumentException("请先删除子节点");

                return PermissionService.DeletePermission(id, CurrentUserId, CurrentUserDisplayName);
            });
        }

        private PermissionDTO GetMenuTreeOneLevel(PermissionDTO tree)
        {
            if (tree.Level == 1)
                tree.Children = null;
            if (null != tree.Children)
            {
                foreach (var child in tree.Children)
                {
                    GetMenuTreeOneLevel(child);
                }
            }
            return tree;
        }
        #endregion

        #region 权限角色
        /// <summary>
        /// 三级级菜单：系统管理/菜单权限管理/权限角色管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("菜单权限管理", "权限角色管理", "/Permission/RoleInPermission",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-sitemap", AuthorityId = "3A3D7B20-7EFF-49DE-B7ED-71DF57C2C9FC",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsExtPage = true, Level = 3)]
        [Web.Extension.PermissionFilter("权限角色管理", "权限角色管理", "/Permission/RoleInPermission", "3A3D7B20-7EFF-49DE-B7ED-71DF57C2C9FC",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = true, ResultType = ResultType.ActionResult)]
        public ActionResult RoleInPermission(int id = 0, string appId = null)
        {
            ViewBag.permissionId = id;
            ViewBag.canEditRole = base.IsSystemAdmin;
            return View();
        }

        public async Task<JsonResult> GetRoleInPermission(int id = 0)
        {
            //返回所有菜单所有角色选中角色
            var permission = PermissionService.GetDetailPermissionById(id);
            var roleList = await GetOwnSimpleRoles();
            if (permission == null)
            {
                return Json(roleList);
            }

            if (permission.Role != null && permission.Role.Any())
            {
                var checkIds = permission.Role.Select(m => m.RoleId);
                roleList = GetCheckRoleList(checkIds, roleList);
            }

            return Json(roleList);
        }

        [Web.Extension.PermissionFilter("权限角色管理", "保存权限角色数据", "/Permission/SubmitRoleInPermission", "1B152A1B-231A-4576-8C66-22F142284207",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult SubmitRoleInPermission(string userId, List<string> addList, int menuId = 0)
        {
            return base.GetServiceJsonResult(() =>
            {
                if (addList == null)
                    addList = new List<string>();

                var add = new List<string>();
                if (addList.Any())
                    add.AddRange(addList.Distinct());


                var dbIdList = new List<string>();
                var result = false;
                if (menuId == 0)
                    throw new ArgumentNullException("menuId", "选中的权限Id为空");

                //修改权限角色
                result = PermissionService.UpdateRoleInPermission(menuId, add, CurrentUserId,
                    CurrentUserDisplayName);

                return result;
            });
        }
        #endregion

    }
}