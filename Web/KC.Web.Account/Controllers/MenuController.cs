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
using KC.Service.Extension;
using KC.Service;
using KC.Framework.Extension;
using KC.Framework.Base;
using AutoMapper;
using KC.Service.DTO;
using KC.Service.Util;
using Microsoft.AspNetCore.Authorization;

namespace KC.Web.Account.Controllers
{
    public class MenuController : AccountBaseController
    {
        private IMenuService MenuService;
        public MenuController(
            Tenant tenant,
            IMenuService menuService,
            IServiceProvider serviceProvider,
            ILogger<RoleController> logger)
            : base(tenant, serviceProvider, logger)
        {
            MenuService = menuService;
        }

        /// <summary>
        /// 三级级菜单：系统管理/菜单权限管理/菜单管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("菜单权限管理", "菜单管理", "/Menu/Index",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-sitemap", AuthorityId = "08D986E5-CB98-4F2B-B08A-040B4C935224",
        DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("菜单管理", "菜单管理", "/Menu/Index", "08D986E5-CB98-4F2B-B08A-040B4C935224",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = true, ResultType = ResultType.ActionResult)]
        public ActionResult Index()
        {
            return View();
        }

        #region 菜单管理

        public async Task<JsonResult> LoadMenuTree(string name, int excludeId, int selectedId, bool hasAll = false, bool hasRoot = true, int maxLevel = 3)
        {
            var result = new List<MenuNodeSimpleDTO>();
            if (hasAll)
            {
                result.Add(new MenuNodeSimpleDTO()
                {
                    Id = 0,
                    //TenantType = TenantType.CommercialFactoring,
                    Text = "所有文档",
                    Children = null,
                    Level = 1
                });
                result.Add(new MenuNodeSimpleDTO()
                {
                    Id = -1,
                    //TenantType = TenantType.CommercialFactoring,
                    Text = "未分类文档",
                    Children = null,
                    Level = 1
                });
            }

            if (IsSystemAdmin)
            {
                var data = await MenuService.FindMenuTreesByNameAsync(name);
                result.AddRange(data);
            }
            else
            {
                Func<MenuNodeSimpleDTO, bool> predict = m => true;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    predict = m => (m.Text.Contains(name));
                }
                var data = await GetCurrentUserMenuTree(predict);
                result.AddRange(data);
            }

            if (hasRoot)
            {
                var rootMenu = new MenuNodeSimpleDTO() { Text = "顶级分类", Children = result, Level = 0 };
                var org = TreeNodeUtil.GetNeedLevelSimpleTreeNodeDTO(rootMenu, maxLevel, new List<int>() { excludeId }, new List<int>() { selectedId });
                return Json(new List<MenuNodeSimpleDTO> { org });
            }

            var orgList = TreeNodeUtil.LoadNeedLevelSimpleTreeNodeDTO(result, maxLevel, new List<int>() { excludeId }, new List<int>() { selectedId });
            return Json(orgList);  
        }

        public PartialViewResult GetMenuForm(int id = 0)
        {
            var model = new MenuNodeDTO();
            if (id != 0)
            {
                model = MenuService.GetMenuById(id);
                model.IsEditMode = true;
                if (model.TenantType.HasValue)
                {
                    ViewBag.TenantTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<TenantType>((int)model.TenantType);
                    ViewBag.SelectedTenantType = (int)model.TenantType;
                }

                var dropItems = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum((int)model.Version,
                    new List<TenantVersion>()
                    {
                        TenantVersion.Customized
                    });
                ViewBag.SystemVersionList = dropItems;
                ViewBag.SelectedVersionIds = dropItems.Select(m => int.Parse(m.Value))
                    .Where(m => (model.Version & (TenantVersion)m) != 0)
                    .ToCommaSeparatedInt();
            }
            else
            {
                model.IsEditMode = false;
                ViewBag.TenantTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<TenantType>();
                ViewBag.SelectedTenantType = (int)TenantType.Enterprise;
                ViewBag.SystemVersionList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum(null, new List<TenantVersion>()
                {
                    TenantVersion.Customized
                });
                ViewBag.SelectedVersionIds = (int)TenantVersion.Standard;
            }

            return PartialView("_menuForm", model);
        }

        [AllowAnonymous]
        public async Task<JsonResult> ExistMenuName(int id, string name, string orginalName, bool isEditMode)
        {
            if (isEditMode && name.Equals(orginalName, StringComparison.OrdinalIgnoreCase))
                return Json(false);

            return Json(await MenuService.ExistMenuNameAsync(id, name));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Web.Extension.PermissionFilter("菜单管理", "保存菜单数据", "/Menu/SaveMenu", "9DFC51A6-E5F9-480C-BB99-D619F9E5E690",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult SaveMenu(MenuNodeDTO model)
        {
            if (model.ParentId == 0)
                model.ParentId = null;
            //model.ApplicationId = CurrentOperationApplicationId;
            ModelState.Remove("IsDeleted");
            ModelState.Remove("CreatedDate");
            ModelState.Remove("ModifiedDate");
            if (!ModelState.IsValid)
            {
                var errors = GetAllModelErrorMessages();
                return base.ThrowErrorJsonMessage(false, "数据有误，错误消息为：" + errors);
            }
                

            return GetServiceJsonResult(() =>
            {
                model.Level = model.ParentId > 0 ? 2 : 1;
                if (model.ParentId.HasValue && model.ParentId > 0)
                {
                    var parentMenu = MenuService.GetMenuById(model.ParentId.Value);
                    if (null == parentMenu)
                    {
                        throw new ArgumentException(string.Format("父级ID:{0} 未找到！", model.ParentId));
                    }
                    if (parentMenu.Level > 2)
                    {
                        throw new ArgumentException(string.Format("父级ID:{0} 不能作为父级，菜单已超过三级！", model.ParentId));
                    }
                    if (model.Id == model.ParentId)
                    {
                        throw new ArgumentException(string.Format("父级ID:{0} 不能作为父级！", model.ParentId));
                    }
                } 

                if (!model.IsEditMode)
                {
                    model.CreatedName = CurrentUserDisplayName;
                    model.CreatedBy = CurrentUserId;
                    model.CreatedDate = DateTime.UtcNow;
                }
                model.ModifiedName = CurrentUserDisplayName;
                model.ModifiedBy = CurrentUserId;
                model.ModifiedDate = DateTime.UtcNow;


                var result = MenuService.SaveMenu(model, CurrentUserId, CurrentUserDisplayName);
                if (result)
                {
                    //清除该用户菜单缓存
                    var cacheKey = TenantName.ToLower() + "-CurrentUserMenus-" + CurrentUserId;
                    CacheUtil.RemoveCache(cacheKey);
                }
                return result;
            });
        }

        [Web.Extension.PermissionFilter("菜单管理", "删除菜单数据", "/Menu/RemoveMenu", "E3A5DD3D-A848-42DE-BE53-C46EE1D0D582",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult RemoveMenu(int id)
        {
            return GetServiceJsonResult(() =>
            {
                var menu = MenuService.GetMenuById(id);
                if (null == menu)
                {
                    throw new ArgumentException(string.Format("未找到菜单ID:{0}", id));
                }
                if (menu.Children.Any())
                {
                    throw new ArgumentException(string.Format("菜单:【{0}】存在子菜单，无法删除，请先删除子菜单！", menu.Text));
                }

                var result = MenuService.RemoveMenuById(id, CurrentUserId, CurrentUserDisplayName);
                if (result)
                {
                    //清除该用户菜单缓存
                    var cacheKey = TenantName.ToLower() + "-CurrentUserMenus-" + CurrentUserId;
                    CacheUtil.RemoveCache(cacheKey);
                }
                return result;
            });

        }

        #endregion

        #region 菜单角色管理
        /// <summary>
        /// 三级级菜单：系统管理/菜单权限管理/菜单角色管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("菜单权限管理", "菜单角色管理", "/Menu/RoleInMenu",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-sitemap", AuthorityId = "29FC3DD2-E8C7-4EA7-A5CC-AF7F57B106D9",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsExtPage = true, Level = 3)]
        [Web.Extension.PermissionFilter("菜单角色管理", "菜单角色管理", "/Menu/RoleInMenu", "29FC3DD2-E8C7-4EA7-A5CC-AF7F57B106D9",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = true, ResultType = ResultType.ActionResult)]
        public ActionResult RoleInMenu(int id = 0)
        {
            ViewBag.menuId = id;
            ViewBag.canEditRole = base.IsSystemAdmin;
            return View("RoleInMenu");
        }

        public async Task<JsonResult> GetRoleInMenu(int id = 0)
        {
            //返回所有菜单所有角色选中角色
            var menu = MenuService.GetDetailMenuById(id);
            var roleList = await GetOwnSimpleRoles();
            if (menu == null)
            {
                return Json(roleList);
            }
            
            if (menu.Role != null && menu.Role.Any())
            {
                var checkIds = menu.Role.Select(m => m.RoleId);
                roleList = GetCheckRoleList(checkIds, roleList);
            }

            return Json(roleList);
        }

        [Web.Extension.PermissionFilter("菜单角色管理", "保存菜单角色数据", "/Menu/SubmitRoleInMenu", "6862116B-CA67-44DA-9E6A-D5C05DA7F964",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult SubmitRoleInMenu(string userId, List<string> addList, string type, int menuId = 0)
        {
            return base.GetServiceJsonResult(() =>
            {
                if (addList == null)
                    addList = new List<string>();
                var add = new List<string>();
                if (addList.Any())
                    add.AddRange(addList.Distinct());
                var result = false;
                if (menuId == 0)
                    throw new ArgumentNullException("menuId", "选中的菜单Id为空");

                //修改菜单角色
                result = MenuService.UpdateRoleInMenu(menuId, add, CurrentUserId,
                    CurrentUserDisplayName);
                //分配角色成功，清除用户缓存，以便重新获取角色权限
                if (result)
                    ClearUserCacheByRoles(add);

                return result;
            });
        }
        #endregion
    }
}