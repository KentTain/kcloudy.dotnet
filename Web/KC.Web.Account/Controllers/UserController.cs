using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using KC.Service.Account;
using KC.Service.DTO.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using KC.Common;
using KC.Framework.Base;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace KC.Web.Account.Controllers
{
    public class UserController : AccountBaseController
    {
        private IRoleService RoleService;
        public UserController(
            Tenant tenant,
            IRoleService roleService,
            IServiceProvider serviceProvider,
            ILogger<UserController> logger)
            : base(tenant, serviceProvider, logger)
        {
            RoleService = roleService;
        }

        #region 用户管理
        /// <summary>
        /// 三级级菜单：系统管理/用户管理/用户管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("用户管理", "用户管理", "/User/Index",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-user", AuthorityId = "730D5415-C702-4948-A209-A077AD20D4DA",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("用户管理", "用户管理", "/User/Index", "730D5415-C702-4948-A209-A077AD20D4DA",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult Index()
        {
            ViewBag.PositionLevels = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<PositionLevel>(null, null, "请选择岗位类型");
            ViewBag.StatusList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<WorkflowBusStatus>(null, null, "请选择状态");
            
            ViewBag.meUserId = CurrentUserId;
            return View();
        }

        //[Web.Extension.PermissionFilter("用户管理", "加载用户列表数据", "/User/LoadUserList",
        //    DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "9B47D623-2E38-4AF9-A3D4-B954D84D372D")]
        public JsonResult LoadUserList(int page = 1, int rows = 10, string email = "", string phone = "", string name = "", WorkflowBusStatus? status = null, PositionLevel? positionLevel = null, int? orgId = null)
        {
            // 系统管理员及人事经理的角色
            if (IsSystemAdmin || CurrentUserRoleIds.Any(m => m == RoleConstants.HrManagerRoleId))
            {
                var result = AccountService.FindPaginatedUsersByOrgIds(page, rows, email, phone,
                name, status, positionLevel, orgId != null && orgId != 0 ? new List<int>() { orgId.Value } : null);

                return Json(result);
            }

            //获取当前登录用户的部门ID（及其下属部门的ID），只查询出当前登录用户部门下的用户 
            //查询员工当前所属部门及其下属部门的ID
            var orgUsers = AccountService.FindPaginatedUsersByOrgIds(page, rows, email, phone,
                name, status, positionLevel, orgId != null && orgId != 0 ? new List<int>() { orgId.Value } : CurrentUserOrgIds);
            return Json(orgUsers);
        }

        public async Task<PartialViewResult> GetUserForm(string id)
        {
            var model = new UserDTO();
            if (!string.IsNullOrEmpty(id))
            {
                model = await AccountService.GetUserWithOrgsAndRolesByUserIdAsync(id);
                model.IsEditMode = true;
                ViewBag.PositionLevels = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<PositionLevel>((int)model.PositionLevel);
            }
            else
            {
                ViewBag.PositionLevels = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<PositionLevel>();
            }

            return PartialView("_userForm", model);
        }

        public JsonResult LoadRoleByIds(List<string> ids)
        {
            ViewBag.PositionLevels = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<PositionLevel>();
            var model = RoleService.FindRolesByIds(ids);

            return Json(model);
        }

        //EasyUi validatebox Remote validate use setTimeout: http://www.cnblogs.com/qiancheng509/archive/2012/11/23/2783700.html
        //https://uule.iteye.com/blog/1849690
        [AllowAnonymous]
        public async Task<JsonResult> ExistUserEmail(string name, string orginalName, bool isEditMode)
        {
            if (isEditMode && name.Equals(orginalName, StringComparison.OrdinalIgnoreCase))
            {
                return Json(false);
            }

            var result = await AccountService.ExistUserEmailAsync(name);
            return Json(result);
        }
        [AllowAnonymous]
        public async Task<JsonResult> ExistUserPhone(string name, string orginalName, bool isEditMode)
        {
            if (isEditMode && name.Equals(orginalName, StringComparison.OrdinalIgnoreCase))
            {
                return Json(false);
            }

            var result = await AccountService.ExistUserPhoneAsync(name);
            return Json(result);
        }

        [AllowAnonymous]
        public async Task<JsonResult> ExistUserName(string name, string orginalName, bool isEditMode)
        {
            if (isEditMode && name.Equals(orginalName, StringComparison.OrdinalIgnoreCase))
            {
                return Json(false);
            }

            var result = await AccountService.ExistUserNameAsync(name);
            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Web.Extension.PermissionFilter("用户管理", "保存用户", "/User/SaveUser", "55BD23BA-D892-41A0-A2D3-5E773D832E79",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult SaveUser(UserDTO model, string[] organizations)
        {
            return GetServiceJsonResult(() =>
            {
                bool success = false;
                if (!model.IsEditMode)
                {
                    var gussourl = GlobalConfig.PortalWebDomain.Replace(TenantConstant.SubDomain, TenantName);
                    var result =
                        AccountService.CreateUser(model, CurrentUserId, CurrentUserDisplayName, organizations, gussourl).Result;
                    if (result != null)
                    {
                        success = result.Succeeded;
                    }
                }
                else
                {
                    success = AccountService.UpdateUser(model, CurrentUserId, CurrentUserDisplayName, organizations);
                }

                return success;
            });
        }

        [Web.Extension.PermissionFilter("用户管理", "删除用户", "/User/RemoveUser", "E617F833-311B-4B4F-A04A-EC17214D6A1B",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult RemoveUser(string id)
        {
            return GetServiceJsonResult(() =>
            {
                var result = AccountService.RemoveUserById(id, CurrentUserId, CurrentUserDisplayName);
                return result;
            });
        }

        //冻结/激活
        [Web.Extension.PermissionFilter("用户管理", "启用/冻结用户", "/User/FreezeOrActivation", "B15DDB6E-6922-4E59-AC07-D8773BB42D0A",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 4, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult FreezeOrActivation(string id)
        {
            return GetServiceJsonResult(() =>
            {
                var result = AccountService.ReactUserById(id, CurrentUserId, CurrentUserDisplayName);
                return result;
            });
        }

        /// <summary>
        /// 三级级菜单：系统管理/用户管理/用户详情
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("用户管理", "用户详情", "/User/UserDetail",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-sitemap", AuthorityId = "D380A502-C702-4CEC-9B8C-D095AB427650",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsExtPage = true, Level = 3)]
        [Web.Extension.PermissionFilter("用户管理", "用户详情", "/User/UserDetail", "D380A502-C702-4CEC-9B8C-D095AB427650",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 5, IsPage = true, ResultType = ResultType.ActionResult)]
        public async Task<IActionResult> UserDetail(string id)
        {
            var model = await AccountService.GetSimpleUserWithOrgsAndRolesByUserIdAsync(id);

            var menus = await RoleService.FindUserMenusByRoleIdsAsync(model.UserRoleIds);
            var menuTree = new List<MenuNodeSimpleDTO>();
            foreach (var parent in menus.OrderBy(m => m.Index).Where(m => m.ParentId == null))
            {
                KC.Service.Util.TreeNodeUtil.NestSimpleTreeNodeDTO(parent, menus, null, null);
                menuTree.Add(parent);
            }
            ViewBag.Menus = SerializeHelper.ToJson(menuTree);

            var permissions = await RoleService.FindUserPermissionsByRoleIdsAsync(model.UserRoleIds);

            var permissionTree = new List<PermissionSimpleDTO>();
            foreach (var parent in permissions.OrderBy(m => m.Index).Where(m => m.ParentId == null))
            {
                KC.Service.Util.TreeNodeUtil.NestSimpleTreeNodeDTO(parent, permissions, null, null);
                permissionTree.Add(parent);
            }

            //根据应用进行分组
            var groupPermissions = permissionTree
                .GroupBy(m => m.ApplicationName)
                .Select(k => new { Name = k.Key, Permissions = k.ToList() });
            var result = new List<PermissionSimpleDTO>();
            foreach (var group in groupPermissions)
            {
                var appGuid = group.Permissions[0].ApplicationId.ToByteArray();
                var appId = BitConverter.ToInt32(appGuid);
                var appName = group.Name;
                var permission = group.Permissions;
                group.Permissions.ForEach(m => m.ParentId = appId);
                result.Add(new PermissionSimpleDTO() { Id = appId, Text = appName, Children = permission, Level = 0 });
            }

            ViewBag.Permissions = SerializeHelper.ToJson(result);

            return View(model);
        }

        #region Dowoload & upload Excel Template
        [Web.Extension.PermissionFilter("用户管理", "下载用户Excel模板", "/User/DownLoadExcelTemplate", "15E50D59-D883-4D31-9382-F7517F0A16B9",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 5, IsPage = false, ResultType = ResultType.ImageResult)]
        public async Task<IActionResult> DownLoadExcelTemplate()
        {
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var path = Path.Combine(
                     Directory.GetCurrentDirectory(),
                     "wwwroot\\excels", "UserTemplate.xlsx");

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, contentType, Path.GetFileName(path));
        }
        [Web.Extension.PermissionFilter("用户管理", "导入用户数据", "/User/UploadExcelTemplate", "41CD0D9C-52C8-48CB-86A2-CFB6A9B1F096",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 6, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult UploadExcelTemplate(IFormFile file, string uploadType, string md5)
        {
            return GetServiceJsonResult(() =>
            {
                //var buffer = new byte[file.ContentLength];
                //file.InputStream.Read(buffer, 0, buffer.Length);
                using (var reader = file.OpenReadStream())
                {
                    var result = AccountService.ImportUserDataFromExcel(reader);
                    return result;
                }
            });
        }

        #endregion

        #endregion

        #region 用户角色
        /// <summary>
        /// 三级级菜单：系统管理/用户管理/用户角色管理
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("用户管理", "用户角色管理", "/User/RoleInUser",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-sitemap", AuthorityId = "993C40B4-0981-4294-BDBF-E19CE7C9B392",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsExtPage = true, Level = 3)]
        [Web.Extension.PermissionFilter("用户管理", "用户角色管理", "/User/RoleInUser", "993C40B4-0981-4294-BDBF-E19CE7C9B392",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = true, ResultType = ResultType.ActionResult)]
        public ActionResult RoleInUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View();
            }
            ViewBag.SelectUserId = id;
            ViewBag.canEditRole = base.IsSystemAdmin;
            return View();
        }

        /// <summary>
        /// 用户管理分配到角色页面获取用户列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<JsonResult> GetRoleInUserUserList(string userId)
        {
            var user = await AccountService.GetSimpleUserByIdAsync(userId);
            return Json(new List<UserSimpleDTO> { user });
        }

        /// <summary>
        /// 用户管理分配到角色页面获取角色列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<JsonResult> GetRoleInUserRoleList(string userId, int page = 1, int rows = 10)
        {
            var user = await AccountService.GetUserWithOrgsAndRolesByUserIdAsync(userId);
            var roleList = await GetOwnSimpleRoles();
            roleList = GetCheckRoleList(user.RoleIds, roleList);
            return Json(roleList);
        }

        //用户管理分配角色
        [Web.Extension.PermissionFilter("用户角色管理", "保存用户角色数据", "/User/SubmitRoleInUser", "E95A9A8A-3368-4BCA-9D24-05369B1C7D15",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<JsonResult> SubmitRoleInUser(string userId, List<string> addList, List<bool> newlistIsSystemRole, string type, List<string> rolenamelist,
            int menuId = 0)
        {
            return await base.GetServiceJsonResultAsync(async () =>
            {
                if (addList == null)
                    addList = new List<string>();

                if (rolenamelist == null)
                {
                    rolenamelist = new List<string>();
                }

                var add = new List<string>();
                if (addList.Any())
                    add.AddRange(addList.Distinct());
                var roleByuserId = await AccountService.GetUserWithOrgsAndRolesByUserIdAsync(userId);
                var rolellist = RoleService.FindRolesByIds(roleByuserId.RoleIds);
                if (userId.Contains(RoleConstants.AdminUserId) && (addList.Count == 0))
                {
                    throw new ArgumentException("管理员用户必须选中管理员角色");
                }
                if (userId.Contains(RoleConstants.AdminUserId))
                {
                    //if(!(addList.Count <= 1 && item.ToString().Contains("管理员")))
                    //{
                    //    throw new ArgumentException("系统管理员用户不能分配给其他角色");
                    //}
                    if (!(addList.Contains(RoleConstants.AdminRoleId)))
                    {
                        throw new ArgumentException("管理员用户必须选中管理员角色");
                    }
                }
                var dbIdList = new List<string>();
                var result = false;
                //var appId = CurrentOperationApplicationId;
                switch (type)
                {
                    case "user":
                        if (string.IsNullOrWhiteSpace(userId))
                            throw new ArgumentNullException("userId", "选中的用户Id为空");

                        //用户分配角色
                        result = AccountService.UpdateRoleInUser(userId, add, CurrentUserId,
                            CurrentUserDisplayName);
                        //分配角色成功，清除用户缓存，以便重新获取角色权限
                        if (result)
                        {
                            ClearUserCacheByRoles(add);
                            ClearUserCache(userId);
                        }
                        break;
                    default:
                        throw new ArgumentException("传入参数--type：不支持的type类型。");
                        //break;
                }

                return result;
            });
        }
        #endregion

        #region 用户操作日志
        /// <summary>
        /// 三级级菜单：系统管理/用户管理/用户操作日志
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("用户管理", "用户操作日志", "/User/UserTracingLogList",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-user", AuthorityId = "A92352D4-C288-4B53-8CDE-16DF6646B47F",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 4, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("用户管理", "用户操作日志", "/User/UserTracingLogList", "A92352D4-C288-4B53-8CDE-16DF6646B47F",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsPage = true, ResultType = ResultType.ActionResult)]
        public ActionResult UserTracingLogList()
        {
            return View();
        }

        //[Web.Extension.PermissionFilter("用户日志", "加载用户日志列表数据", "/User/LoadUserTracingLogList",
        //    DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult LoadUserTracingLogList(string selectname, int page = 1, int rows = 10, string order = "desc")
        {
            string name = string.Empty;
            if (!string.IsNullOrWhiteSpace(selectname))
            {
                name = selectname;
            }

            var result = SysManageService.FindPaginatedUserTracingLogs(page, rows, name, order);

            return Json(result);
        }

        #endregion

        #region 用户登陆日志
        /// <summary>
        /// 三级级菜单：系统管理/用户管理/用户登陆日志
        /// </summary>
        /// <returns></returns>
        [Web.Extension.MenuFilter("用户管理", "用户登陆日志", "/User/UserLoginLogList",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-user", AuthorityId = "0102ED9C-15DC-4EB8-A966-F4E2C76DE6C3",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 4, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("用户管理", "用户登陆日志", "/User/UserLoginLogList", "A92352D4-0102ED9C-15DC-4EB8-A966-F4E2C76DE6C3",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsPage = true, ResultType = ResultType.ActionResult)]
        public ActionResult UserLoginLogList()
        {
            return View();
        }

        public JsonResult LoadUserLoginLogList(string selectname, int page = 1, int rows = 10)
        {
            var result = SysManageService.FindPaginatedUserLoginLogs(page, rows, selectname);

            return Json(result);
        }

        #endregion
    }
}