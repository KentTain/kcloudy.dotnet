using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Service.Account;
using KC.Service;
using KC.Service.DTO.Account;
using KC.Service.DTO.Search;

namespace KC.WebApi.Account.Controllers
{
    [Authorize]
    //[EnableCors(Web.Util.StaticFactoryUtil.MyAllowSpecificOrigins)]
    public class AccountApiController : Web.Controllers.WebApiBaseController
    {
        private new const string ServiceName = "KC.WebApi.Account.Controllers.AccountApiController";

        private IAccountService AccountService => ServiceProvider.GetService<IAccountService>();
        private ISysManageService SysManageService => ServiceProvider.GetService<ISysManageService>();
        private IRoleService RoleService => ServiceProvider.GetService<IRoleService>();
        private IMenuService MenuService => ServiceProvider.GetService<IMenuService>();
        private IPermissionService PermissionService => ServiceProvider.GetService<IPermissionService>();

        public AccountApiController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<AccountApiController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        #region 组织架构
        /// <summary>
        /// 获取所有部门信息(非树状list类型)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("LoadAllOrganization")]
        public ServiceResult<List<OrganizationDTO>> LoadAllOrganization()
        {
            return GetServiceResult(() =>
            {
                return SysManageService.FindAllOrganization();
            });
        }

        /// <summary>
        /// 获取所有的部门信息（部门树结构：包含下属部门）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("LoadAllOrgTrees")]
        public ServiceResult<List<OrganizationDTO>> LoadAllOrgTrees()
        {
            return GetServiceResult(() =>
            {
                return SysManageService.FindAllOrgTrees();
            });
        }

        /// <summary>
        /// 获取所有的部门信息及下属员工
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("LoadOrgTreesWithUsers")]
        public ServiceResult<List<OrganizationDTO>> LoadOrgTreesWithUsers()
        {
            return GetServiceResult(() =>
            {
                return SysManageService.FindOrgTreesWithUsers();
            });
        }

        /// <summary>
        /// 根据组织Id列表，获取所有的部门信息(不包括下属员工数据) 
        ///     （部门列表，包含其下属所有的部门信息：递归查询）
        /// </summary>
        /// <param name="orgIds">组织Id列表</param>
        /// <returns></returns>
        [HttpPost]
        [Route("LoadOrgTreesByOrgIds")]
        public ServiceResult<List<OrganizationDTO>> LoadOrgTreesByOrgIds([FromBody] List<int> orgIds)
        {
            return GetServiceResult(() =>
            {
                return SysManageService.FindOrgTreesByIds(orgIds, null);
            });
        }

        /// <summary>
        /// 根据用户Id，获取其所属部门及部门下属员工(非树状list类型)
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("LoadOrganizationsWithUsersByUserId")]
        public ServiceResult<List<OrganizationDTO>> LoadOrganizationsWithUsersByUserId(string userId)
        {
            return GetServiceResult(() =>
            {
                return SysManageService.FindOrganizationsWithUsersByUserId(userId);
            });
        }

        /// <summary>
        /// 根据用户Id，获取其所属上级部门及部门下属员工(非树状list类型)
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("LoadHigherOrganizationsWithUsersByUserId")]
        public ServiceResult<List<OrganizationDTO>> LoadHigherOrganizationsWithUsersByUserId(string userId)
        {
            return GetServiceResult(() =>
            {
                return SysManageService.FindHigherOrganizationsWithUsersByUserId(userId);
            });
        }

        /// <summary>
        /// 获取角色及部门下的所有部门及其下属员工，筛选顺序如下： 
        ///     1. DepIds为空或无值时，显示所有部门树列表 
        ///     2. DepIds有值的话，只显示其设置后的部门树列表 
        ///     3. ExceptOrgIds为需要排除的部门Id列表
        /// </summary>
        /// <param name="searchModel">
        ///     RoleIds、ExceptRoleIds：角色Id列表
        ///     DepIds、ExceptOrgIds：部门Id列表
        /// </param>
        /// <returns></returns>
        [HttpPost]
        [Route("LoadOrgTreesWithUsersByOrgIds")]
        public ServiceResult<List<OrganizationDTO>> LoadOrgTreesWithUsersByOrgIds([FromBody] OrgTreesWithUsersSearchDTO searchModel)
        {
            return GetServiceResult(() =>
            {
                return SysManageService.FindOrgTreesWithUsersByOrgIds(searchModel.OrgIds, searchModel.ExceptOrgIds);
            });
        }


        #endregion

        #region 角色信息

        /// <summary>
        /// 获取角色Id列表下的所有菜单
        /// </summary>
        /// <param name="roleIds">角色Id列表</param>
        /// <returns></returns>
        [HttpPost]
        [Route("LoadUserMenusByRoleIds")]
        public async Task<ServiceResult<List<MenuNodeSimpleDTO>>> LoadUserMenusByRoleIds([FromBody]List<string> roleIds)
        {
            return await GetServiceResultAsync(async () =>
            {
                return await RoleService.FindUserMenusByRoleIdsAsync(roleIds);
            });
        }
        /// <summary>
        /// 获取角色Id列表下的所有权限
        /// </summary>
        /// <param name="roleIds">角色Id列表</param>
        /// <returns></returns>
        [HttpPost]
        [Route("LoadUserPermissionsByRoleIds")]
        public async Task<ServiceResult<List<PermissionSimpleDTO>>> LoadUserPermissionsByRoleIds([FromBody]List<string> roleIds)
        {
            return await GetServiceResultAsync(async () =>
            {
                return await RoleService.FindUserPermissionsByRoleIdsAsync(roleIds);
            });
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("LoadAllRoles")]
        public async Task<ServiceResult<List<RoleSimpleDTO>>> LoadAllRoles()
        {
            return await GetServiceResultAsync(async () =>
            {
                return await RoleService.FindAllSimpleRolesAsync();
            });
        }

        /// <summary>
        /// 根据用户UserId，获取角色列表
        /// </summary>
        /// <param name="userId">用户UserId</param>
        /// <returns></returns>
        [HttpGet("FindRolesWithUsersByUserId")]
        public ServiceResult<List<RoleDTO>> FindRolesWithUsersByUserId(string userId)
        {
            return GetServiceResult(() =>
            {
                return RoleService.FindRolesWithUsersByUserId(userId);
            });
        }

        /// <summary>
        /// 获取所有角色及其下属员工，筛选顺序如下：
        ///     1. RoleIds为空或无值时，显示所有角色列表 
        ///     2. RoleIds有值时，为需要包含用户的角色Id列表 
        ///     3. ExceptRoleIds为需要排除的角色Id列表
        /// </summary>
        /// <param name="searchModel">
        ///     RoleIds、ExceptRoleIds：角色Id列表
        /// </param>
        /// <returns></returns>
        [HttpPost]
        [Route("LoadRolesWithUsersByRoleIds")]
        public async Task<ServiceResult<List<RoleDTO>>> LoadRolesWithUsersByRoleIds([FromBody] RolesWithUsersSearchDTO searchModel)
        {
            return await GetServiceResultAsync(async () =>
            {
                return await RoleService.FindRolesWithUsersByRoleIds(searchModel.RoleIds, searchModel.ExceptRoleIds);
            });
        }

        /// <summary>
        /// 根据角色Id，获取角色信息（包含所有角色的用户）
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        [HttpGet("GetRoleWithUsersByRoleId")]
        public ServiceResult<RoleDTO> GetRoleWithUsersByRoleId(string roleId)
        {
            return GetServiceResult(() =>
            {
                return RoleService.GetRoleWithUsersByRoleId(roleId);
            });
        }

        /// <summary>
        /// 保存应用的权限数据
        /// </summary>
        /// <param name="permissions"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpPost("SavePermissions")]
        public async Task<ServiceResult<bool>> SavePermissionsAsync([FromBody]List<PermissionDTO> permissions, Guid appId)
        {
            return await GetServiceResultAsync(async () =>
            {
                return await PermissionService.SavePermissionsAsync(permissions, appId);
            });
        }

        /// <summary>
        /// 保存应用的菜单数据
        /// </summary>
        /// <param name="menus"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpPost("SaveMenus")]
        public async Task<ServiceResult<bool>> SaveMenusAsync([FromBody]List<MenuNodeDTO> menus, Guid appId)
        {
            return await GetServiceResultAsync(async () =>
            {
                return await MenuService.SaveMenusAsync(menus, appId);
            });
        }
        #endregion

        #region 租户内部的员工信息

        /// <summary>
        /// 根据用户Id列表，获取主管信息
        /// </summary>
        /// <param name="userId">用户Id列表</param>
        /// <returns></returns>
        [HttpGet("LoadUserManagersByUserId")]
        public ServiceResult<List<UserSimpleDTO>> LoadUserManagersByUserId(string userId)
        {
            return GetServiceResult(() =>
            {
                return AccountService.FindUserManagersByUserId(userId);
            });
        }
        /// <summary>
        /// 根据用户Id列表，获取上级主管信息
        /// </summary>
        /// <param name="userId">用户Id列表</param>
        /// <returns></returns>
        [HttpGet("LoadUserSupervisorsByUserId")]
        public ServiceResult<List<UserSimpleDTO>> LoadUserSupervisorsByUserId(string userId)
        {
            return GetServiceResult(() =>
            {
                return AccountService.FindUserSupervisorsByUserId(userId);
            });
        }
        /// <summary>
        /// 根据用户Id列表，获取所有员工
        /// </summary>
        /// <param name="userIds">用户Id列表</param>
        /// <returns></returns>
        [HttpPost("LoadUsersByIds")]
        public ServiceResult<List<UserSimpleDTO>> LoadUsersByIds([FromBody]List<string> userIds)
        {
            return GetServiceResult(() =>
            {
                return AccountService.FindUsersByUserIds(userIds);
            });
        }
        /// <summary>
        /// 根据组织架构Id，获取该组织下的所有员工
        /// </summary>
        /// <param name="orgId">组织Id</param>
        /// <returns></returns>
        [HttpGet("LoadUsersByOrgId")]
        public ServiceResult<List<UserSimpleDTO>> LoadUsersByOrgId(int orgId)
        {
            return GetServiceResult(() =>
            {
                return AccountService.FindUsersByOrgIds(new List<int>() { orgId });
            });
        }
        /// <summary>
        /// 根据组织架构Id列表，获取所有组织的下的所有员工
        /// </summary>
        /// <param name="orgIds">所有组织的Id列表</param>
        /// <returns></returns>
        [HttpPost("LoadUsersByOrgIds")]
        public ServiceResult<List<UserSimpleDTO>> LoadUsersByOrgIds([FromBody]List<int> orgIds)
        {
            return GetServiceResult(() =>
            {
                return AccountService.FindUsersByOrgIds(orgIds);
            });
        }
        /// <summary>
        /// 获取所有组织的下的所有员工
        /// </summary>
        /// <param name="roleIds">所有组织的Id列表</param>
        /// <returns></returns>
        [HttpPost("LoadUsersByRoleIds")]
        public ServiceResult<List<UserSimpleDTO>> LoadUsersByRoleIds([FromBody]List<string> roleIds)
        {
            return GetServiceResult(() =>
            {
                return AccountService.FindUsersByRoleIds(roleIds);
            });
        }
        /// <summary>
        /// 根据用户Id列表、角色Id列表、部门编码列表，获取所有的所有员工
        /// </summary>
        /// <param name="searchModel">用户Id列表、角色Id列表、部门编码列表</param>
        /// <returns></returns>
        [HttpPost("LoadUsersByIdsAndRoleIdsAndOrgCodes")]
        public async Task<ServiceResult<List<UserSimpleDTO>>> LoadUsersByIdsAndRoleIdsAndOrgCodes([FromBody] DataPermissionSearchDTO searchModel)
        {
            return await GetServiceResultAsync(async () =>
            {
                return await AccountService.FindUsersByDataPermissionFilterAsync(searchModel);
            });
        }

        /// <summary>
        /// 获取用户的简单数据 <br/>
        /// （包含：所属的组织架构列表，不包含：系统用参数--邮箱是否确定、加密密码等等）
        /// </summary>
        /// <param name="userId">用户Guid</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUserWithOrgsAndRolesByUserId")]
        public async Task<ServiceResult<UserSimpleDTO>> GetUserWithOrgsAndRolesByUserId(string userId)
        {
            return await GetServiceResultAsync(async () =>
            {
                return await AccountService.GetSimpleUserWithOrgsAndRolesByUserIdAsync(userId);
            });
        }

        /// <summary>
        /// 获取企业内部员工的联系方式
        /// </summary>
        /// <param name="userId">用户UserId</param>
        /// <returns></returns>
        [HttpGet("GetUserContactInfoByUserId")]
        public async Task<ServiceResult<UserContactInfoDTO>> GetUserContactInfoByUserId(string userId)
        {
            return await GetServiceResultAsync(async () =>
            {
                return await AccountService.GetUserContactInfoByIdAsync(userId);
            });
        }
        #endregion

        #region 修改用户信息

        [HttpGet]
        [Route("ApproveUser")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ServiceResult<bool>> ApproveUserAsync(string userId)
        {
            try
            {
                var result = await AccountService.AuditUserStatus(userId, Framework.Base.WorkflowBusStatus.Approved);
                return result.Succeeded
                    ? new ServiceResult<bool>(ServiceResultType.Success, string.Empty, true)
                    : new ServiceResult<bool>(ServiceResultType.Error, result.Errors.ToCommaSeparatedStringByFilter(m => m.Description), false);
            }
            catch (Exception ex)
            {
                var message = string.Format("调用服务({0})的方法({1})操作{2}。",
                    ServiceName, "ApproveUser", "失败，错误消息为：" + ex.Message);
                Logger.LogError(message);
                return new ServiceResult<bool>(ServiceResultType.Error, message);
            }
        }

        [HttpGet]
        [Route("DisagreeUser")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ServiceResult<bool>> DisagreeUserAsync(string userId)
        {
            try
            {
                var result = await AccountService.AuditUserStatus(userId, Framework.Base.WorkflowBusStatus.Disagree);
                return result.Succeeded
                    ? new ServiceResult<bool>(ServiceResultType.Success, string.Empty, true)
                    : new ServiceResult<bool>(ServiceResultType.Error, result.Errors.ToCommaSeparatedStringByFilter(m => m.Description), false);
            }
            catch (Exception ex)
            {
                var message = string.Format("调用服务({0})的方法({1})操作{2}。",
                    ServiceName, "DisagreeUser", "失败，错误消息为：" + ex.Message);
                Logger.LogError(message);
                return new ServiceResult<bool>(ServiceResultType.Error, message);
            }
        }

        /// <summary>
        /// 修改租户的系统管理员的默认密码：123456
        /// </summary>
        /// <param name="password"></param>
        /// <param name="adminEmail"></param>
        /// <param name="adminPhone"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ChangeAdminRawInfoAsync")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ServiceResult<bool>> ChangeAdminRawInfoAsync(string password, string adminEmail, string adminPhone)
        {
            try
            {
                var result = await AccountService.ChangeAdminRawInfoAsync(password, adminEmail, adminPhone);
                return result.Succeeded
                    ? new ServiceResult<bool>(ServiceResultType.Success, string.Empty, true)
                    : new ServiceResult<bool>(ServiceResultType.Error, result.Errors.ToCommaSeparatedStringByFilter(m => m.Description), false);
            }
            catch (Exception ex)
            {
                var message = string.Format("调用服务({0})的方法({1})操作{2}。",
                    ServiceName, "ChangeAdminRawInfo", "失败，错误消息为：" + ex.Message);
                Logger.LogError(message);
                return new ServiceResult<bool>(ServiceResultType.Error, message);
            }
        }

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="userId">用户UserId</param>
        /// <param name="currentPassword">用户当前密码</param>
        /// <param name="newPassword">用户新密码</param>
        /// <returns></returns>
        [HttpGet]
        [Route("ChangePasswordAsync")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ServiceResult<bool>> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            try
            {
                var result = await AccountService.ChangePasswordAsync(userId, currentPassword, newPassword);
                return result.Succeeded
                    ? new ServiceResult<bool>(ServiceResultType.Success, string.Empty, true)
                    : new ServiceResult<bool>(ServiceResultType.Error, result.Errors.ToCommaSeparatedStringByFilter(m => m.Description), false);
            }
            catch (Exception ex)
            {
                var message = string.Format("调用服务({0})的方法({1})操作{2}。",
                    ServiceName, "ChangePasswordAsync", "失败，错误消息为：" + ex.Message);
                Logger.LogError(message);
                return new ServiceResult<bool>(ServiceResultType.Error, message);
            }
        }

        /// <summary>
        /// 修改用户邮箱、手机
        /// </summary>
        /// <param name="userId">用户UserId</param>
        /// <param name="email">用户邮箱</param>
        /// <param name="phone">用户手机</param>
        /// <returns></returns>
        [HttpGet]
        [Route("ChangeMailPhoneAsync")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ServiceResult<bool>> ChangeMailPhoneAsync(string userId, string email, string phone)
        {
            try
            {
                var result = await AccountService.ChangeMailPhoneAsync(userId, email, phone);
                return result.Succeeded
                    ? new ServiceResult<bool>(ServiceResultType.Success, string.Empty, true)
                    : new ServiceResult<bool>(ServiceResultType.Error, result.Errors.ToCommaSeparatedStringByFilter(m => m.Description), false);
            }
            catch (Exception ex)
            {
                var message = string.Format("调用服务({0})的方法({1})操作{2}。",
                    ServiceName, "ChangePasswordAsync", "失败，错误消息为：" + ex.Message);
                Logger.LogError(message);
                return new ServiceResult<bool>(ServiceResultType.Error, message);
            }
        }
        #endregion
    }
}
