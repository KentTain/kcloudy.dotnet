using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using KC.Service.Account;
using KC.Service;
using KC.Service.Constants;
using KC.Service.DTO.Account;
using KC.Service.DTO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using KC.Service.DTO.Search;

namespace KC.Web.Account.Controllers
{
    public abstract class AccountBaseController : Web.Controllers.TenantWebBaseController
    {
        protected ISysManageService SysManageService => ServiceProvider.GetService<ISysManageService>();
        protected IAccountService AccountService => ServiceProvider.GetService<IAccountService>();
        protected IRoleService RoleService => ServiceProvider.GetService<IRoleService>();
        public AccountBaseController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        #region 选小图标控件: Shared/_SelectUserPartial.cshtml
        [HttpGet]
        public PartialViewResult _SelectIconPartial()
        {
            return PartialView();
        }

        #endregion 选小图标控件: Shared/_SelectUserPartial.cshtml

        #region 选人控件: Shared/_SelectUserPartial.cshtml
        /// <summary>
        /// 获取所有的部门信息及下属员工
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetRootOrganizationsWithUsers(List<string> roleIds)
        {
            //var roleIdList = new List<string>();
            //if (!string.IsNullOrEmpty(roleIds))
            //{
            //    roleIdList = roleIds.Split(',').ToList();
            //}
            var res = SysManageService.FindOrgTreesWithUsersByRoleIds(roleIds);
            var firstOrg = res.FirstOrDefault();
            if (firstOrg != null)
                firstOrg.@checked = true;
            return this.Json(new { orgInfos = res });
        }
        [HttpGet]
        public JsonResult GetRootOrganizationsWithUsers()
        {
            var res = SysManageService.FindOrgTreesWithUsers();
            return Json(res);
        }

        /// <summary>
        /// 获取相关部门以及角色信息及下属员工
        /// </summary>
        /// <param name="roleIds"></param>
        /// <param name="depIds"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetOrgsWithUsersByRoleIdsAndOrgids(Service.DTO.Search.OrgTreesAndRolesWithUsersSearchDTO searchModel)
        {
            var orgs = SysManageService.FindOrgTreesWithUsersByOrgIds(searchModel.OrgIds, searchModel.ExceptOrgIds);
            var firstOrg = orgs.FirstOrDefault();
            if (firstOrg != null)
                firstOrg.@checked = true;
            var roles = RoleService.FindRolesWithUsersByRoleIds(searchModel.RoleIds, searchModel.ExceptRoleIds);
            return this.Json(new { orgInfos = orgs, roleInfos = roles });
        }

        #endregion 选人控件: Shared/_SelectUserPartial.cshtml

        #region Private
        /// <summary>
        /// 获取归属的角色数据：</br>
        ///     当前用户为系统管理员或人事经理时，返回所有角色；</br>
        ///     否则，返回其归属的角色及创建角色
        /// </summary>
        /// <returns></returns>
        protected async Task<IList<RoleSimpleDTO>> GetOwnSimpleRoles()
        {
            var role = await RoleService.FindAllSimpleRolesAsync();
            var data = new List<RoleSimpleDTO>();
            // 系统管理员及人事经理的角色
            if (IsSystemAdmin || CurrentUserRoleIds.Any(m => m == RoleConstants.HrManagerRoleId))
            {
                return role;
            }
            else if (role != null && role.Any())
            {
                var roleDate = new List<RoleSimpleDTO>();
                //查询当前登录用户具有哪些角色(不是系统管理员)  
                var rolesIdlist = CurrentUserRoleIds ?? new List<string>();
                var datalist = role.Where(c => c.RoleId != null && rolesIdlist.Contains(c.RoleId));
                data.AddRange(datalist);
                //查询出自己创建的角色
                var datalist2 = role.Where(c => c.CreatedBy != null && c.CreatedBy.Equals(CurrentUserId));
                data.AddRange(datalist2);
            }

            return data.Distinct().ToList();
        }

        /// <summary>
        /// 当前菜单角色
        /// </summary>
        /// <param name="menulist"></param>
        /// <param name="rolelist"></param>
        /// <returns></returns>
        protected IList<RoleSimpleDTO> GetCheckRoleList(IEnumerable<string> checkList, IList<RoleSimpleDTO> rolelist)
        {
            foreach (var dto in rolelist)
            {
                if (checkList.Contains(dto.RoleId))
                {
                    dto.@checked = true;
                }
            }

            return rolelist;
        }

        /// <summary>
        /// 清除某角色下所有用户的缓存
        /// </summary>
        /// <param name="roleIds">角色Id集合</param>
        protected void ClearUserCacheByRoles(List<string> roleIds)
        {
            var users = AccountService.FindUsersByRoleIds(roleIds);
            foreach (var user in users)
            {
                //清除用户缓存
                RemoveCachedCurrentUser(CurrentUserTenantName, user.UserId);
            }
        }

        /// <summary>
        /// 清除某用户的缓存
        /// </summary>
        /// <param name="userId"></param>
        protected void ClearUserCache(string userId)
        {
            var cacheKey = TenantName.ToLower() + "-" + CacheKeyConstants.Prefix.CurrentUserId + userId;
            CacheUtil.RemoveCache(cacheKey);
        }
        #endregion
    }
}