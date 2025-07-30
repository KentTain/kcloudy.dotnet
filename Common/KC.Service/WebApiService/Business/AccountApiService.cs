using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KC.Common;
using KC.Framework.Tenant;
using KC.Service.DTO.Account;
using KC.Service.DTO.Search;
using KC.Framework.Base;

namespace KC.Service.WebApiService.Business
{
    public class AccountApiService : IdSrvOAuth2ClientRequestBase, IAccountApiService
    {
        private const string _serviceName = "KC.Service.WebApiService.Business.AccountApiService";
        public AccountApiService(
            Tenant tenant, 
            System.Net.Http.IHttpClientFactory httpClient,
            Microsoft.Extensions.Logging.ILogger<AccountApiService> logger)
            : base(tenant, httpClient, logger)
        {
        }

        /// <summary>
        /// 租户账号接口地址：http://[tenantName].acc.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:2001/api/
        /// </summary>
        private string _accoutServerUrl
        {
            get
            {
                if (string.IsNullOrEmpty(GlobalConfig.AccWebDomain))
                    return null;

                return GlobalConfig.AccWebDomain.Replace(TenantConstant.SubDomain, Tenant.TenantName) + "api/";
            }
        }

        #region 登录（包括：微信）
        /// <summary>
        /// 获取用户登录信息
        /// </summary>
        /// <param name="userName">用户名/用户邮箱/用户手机</param>
        /// <param name="password">用户密码</param>
        /// <returns></returns>
        public async Task<ServiceResult<UserDTO>> TenantUserLogin(string userName, string password)
        {
            ServiceResult<UserDTO> result = null;
            await WebSendGetAsync<ServiceResult<UserDTO>>(
                _serviceName + ".TenantUserLogin",
                _accoutServerUrl + "AccountApi/TenantUserLogin?userName=" + userName + "&password=" + password,
                ApplicationConstant.AccScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<UserDTO>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            return result;
        }
        #endregion

        #region 组织架构

        /// <summary>
        /// 获取所有的部门信息(非树状list类型)
        /// </summary>
        /// <returns></returns>
        public async Task<List<OrganizationSimpleDTO>> LoadAllOrganization()
        {
            //var cacheKey = "WebApiService.AccountApiService-LoadAllOrganization";
            //var cache = Service.CacheUtil.GetCache<List<OrganizationSimpleDTO>>(cacheKey);
            //if (cache != null)
            //    return cache;

            ServiceResult<List<OrganizationSimpleDTO>> result = null;
            await WebSendGetAsync<ServiceResult<List<OrganizationSimpleDTO>>>(
                _serviceName + ".LoadAllOrganization",
                _accoutServerUrl + "AccountApi/LoadAllOrganization",
                ApplicationConstant.AccScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<OrganizationSimpleDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                //Service.CacheUtil.SetCache(cacheKey, result.Result, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));

                return result.Result;
            }
            return new List<OrganizationSimpleDTO>();
        }
        /// <summary>
        /// 根据用户Id，获取其所属部门及下属员工(非树状list类型)
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public async Task<List<OrganizationSimpleDTO>> LoadOrganizationsWithUsersByUserId(string userId)
        {
            //var cacheKey = "WebApiService.AccountApiService-LoadOrganizationsWithUsersByUserId-" + userId;
            //var cache = Service.CacheUtil.GetCache<List<OrganizationSimpleDTO>>(cacheKey);
            //if (cache != null)
            //    return cache;

            ServiceResult<List<OrganizationSimpleDTO>> result = null;
            await WebSendGetAsync<ServiceResult<List<OrganizationSimpleDTO>>>(
                _serviceName + ".LoadOrganizationsWithUsersByUserId",
                _accoutServerUrl + "AccountApi/LoadOrganizationsWithUsersByUserId?userId=" + userId,
                ApplicationConstant.AccScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<OrganizationSimpleDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                //Service.CacheUtil.SetCache(cacheKey, result.Result, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));

                return result.Result;
            }

            return new List<OrganizationSimpleDTO>();
        }
        /// <summary>
        /// 根据用户Id，获取其所属上级部门及下属员工(非树状list类型)
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public async Task<List<OrganizationSimpleDTO>> LoadHigherOrganizationsWithUsersByUserId(string userId)
        {
            //var cacheKey = "WebApiService.AccountApiService-LoadHigherOrganizationsWithUsersByUserId-" + userId;
            //var cache = Service.CacheUtil.GetCache<List<OrganizationSimpleDTO>>(cacheKey);
            //if (cache != null)
            //    return cache;

            ServiceResult<List<OrganizationSimpleDTO>> result = null;
            await WebSendGetAsync<ServiceResult<List<OrganizationSimpleDTO>>>(
                _serviceName + ".LoadHigherOrganizationsWithUsersByUserId",
                _accoutServerUrl + "AccountApi/LoadHigherOrganizationsWithUsersByUserId?userId=" + userId,
                ApplicationConstant.AccScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<OrganizationSimpleDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                //Service.CacheUtil.SetCache(cacheKey, result.Result, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));

                return result.Result;
            }

            return new List<OrganizationSimpleDTO>();
        }


        /// <summary>
        /// 获取所有的部门信息
        /// </summary>
        /// <param name="appid">应用Id</param>
        /// <returns></returns>
        public async Task<List<OrganizationSimpleDTO>> LoadAllOrgTrees()
        {
            ServiceResult<List<OrganizationSimpleDTO>> result = null;
            await WebSendGetAsync<ServiceResult<List<OrganizationSimpleDTO>>>(
                _serviceName + ".LoadAllOrgTrees",
                _accoutServerUrl + "AccountApi/LoadAllOrgTrees",
                ApplicationConstant.AccScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<OrganizationSimpleDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
                return result.Result;

            return new List<OrganizationSimpleDTO>();
        }
        /// <summary>
        /// 获取所有的部门信息及下属员工
        /// </summary>
        /// <param name="appid">应用Id</param>
        /// <returns></returns>
        public async Task<List<OrganizationSimpleDTO>> LoadOrgTreesWithUsers()
        {
            //var cacheKey = "WebApiService.AccountApiService-LoadOrgTreesWithUsers";
            //var cache = Service.CacheUtil.GetCache<List<OrganizationSimpleDTO>>(cacheKey);
            //if (cache != null)
            //    return cache;

            ServiceResult<List<OrganizationSimpleDTO>> result = null;
            await WebSendGetAsync<ServiceResult<List<OrganizationSimpleDTO>>>(
                _serviceName + ".LoadOrgTreesWithUsers",
                _accoutServerUrl + "AccountApi/LoadOrgTreesWithUsers",
                ApplicationConstant.AccScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<OrganizationSimpleDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                //Service.CacheUtil.SetCache(cacheKey, result.Result, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));

                return result.Result;
            }

            return new List<OrganizationSimpleDTO>();
        }
        /// <summary>
        /// 根据组织Id列表，获取所有的部门信息 </br>
        ///     （部门列表，包含其下属所有的部门信息：递归查询）
        /// </summary>
        /// <param name="ids">组织Id列表</param>
        /// <returns></returns>
        public async Task<List<OrganizationSimpleDTO>> LoadOrgTreesByOrgIds(List<int> orgIds)
        {
            var jsonData = SerializeHelper.ToJson(orgIds);
            ServiceResult<List<OrganizationSimpleDTO>> result = null;
            await WebSendPostAsync<ServiceResult<List<OrganizationSimpleDTO>>>(
                _serviceName + ".LoadOrgTreesByOrgIds",
                _accoutServerUrl + "AccountApi/LoadOrgTreesByOrgIds",
                ApplicationConstant.AccScope,
                jsonData,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<OrganizationSimpleDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
                return result.Result;

            return new List<OrganizationSimpleDTO>();
        }
        
        /// <summary>
        /// 获取部门下的所有部门及其下属员工，筛选顺序如下： </br>
        ///     1. DepIds为空或无值时，显示所有部门树列表 </br>
        ///     2. DepIds有值的话，只显示其设置后的部门树列表 </br>
        ///     3. ExceptOrgIds为需要排除的部门Id列表 </br>
        /// </summary>
        /// <param name="searchModel">
        ///     DepIds、ExceptOrgIds：部门Id列表
        ///     RoleIds、ExceptRoleIds：可为空，不参与计算
        /// </param>
        /// <returns>部门下的所有部门及其下属员工(树结构)</returns>
        public async Task<List<OrganizationSimpleDTO>> LoadOrgTreesWithUsersByOrgIds(OrgTreesAndRolesWithUsersSearchDTO searchModel)
        {
            var jsonData = SerializeHelper.ToJson(searchModel);
            ServiceResult<List<OrganizationSimpleDTO>> result = null;
            await WebSendPostAsync<ServiceResult<List<OrganizationSimpleDTO>>>(
                _serviceName + ".LoadOrgTreesWithUsersByOrgIds",
                _accoutServerUrl + "AccountApi/LoadOrgTreesWithUsersByOrgIds",
                ApplicationConstant.AccScope,
                jsonData,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<OrganizationSimpleDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
                return result.Result;

            return new List<OrganizationSimpleDTO>();
        }

        #endregion

        #region 角色信息
        public async Task<List<RoleSimpleDTO>> LoadAllRoles()
        {
            //var key = "WebApiService.AccountApiService-LoadAllRoles";
            //var allPermissions = KC.Service.Util.LocalWebCacheUtil.GetCache<List<RoleSimpleDTO>>(key);
            //if (allPermissions != null) return allPermissions;

            ServiceResult<List<RoleSimpleDTO>> result = null;
            await WebSendGetAsync<ServiceResult<List<RoleSimpleDTO>>>(
                _serviceName + ".LoadAllRoles",
                _accoutServerUrl + "AccountApi/LoadAllRoles",
                ApplicationConstant.AccScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<RoleSimpleDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                //KC.Service.Util.LocalWebCacheUtil.SetCache(key, result.Result, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));
                return result.Result;
            }

            return null;
        }

        /// <summary>
        /// 根据用户UserId，获取角色列表
        /// </summary>
        /// <param name="userId">用户UserId</param>
        /// <returns></returns>
        public async Task<List<RoleSimpleDTO>> LoadRolesWithUsersByUserId(string userId)
        {
            //var cacheKey = "WebApiService.AccountApiService-LoadRolesWithUsersByUserId-" + userId;
            //var cache = Service.CacheUtil.GetCache<List<RoleSimpleDTO>>(cacheKey);
            //if (cache != null)
            //    return cache;

            ServiceResult<List<RoleSimpleDTO>> result = null;
            await WebSendGetAsync<ServiceResult<List<RoleSimpleDTO>>>(
                _serviceName + ".FindRolesWithUsersByUserId",
                _accoutServerUrl + "AccountApi/FindRolesWithUsersByUserId?userId=" + userId,
                ApplicationConstant.AccScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<RoleSimpleDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                //Service.CacheUtil.SetCache(cacheKey, result.Result, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));

                return result.Result;
            }

            return null;
        }
        
        /// <summary>
        /// 获取所有角色及其下属员工，筛选顺序如下： </br>
        ///     1. RoleIds为空或无值时，显示所有角色列表 </br>
        ///     2. RoleIds有值时，为需要包含用户的角色Id列表 </br>
        ///     3. ExceptRoleIds为需要排除的角色Id列表
        /// </summary>
        /// <param name="searchModel">
        ///     RoleIds、ExceptRoleIds：角色Id列表
        ///     DeptIds、ExceptDeptIds：可为空，不参与计算
        /// </param>
        /// <returns></returns>
        public async Task<List<RoleSimpleDTO>> LoadRolesWithUsersByRoleIds(OrgTreesAndRolesWithUsersSearchDTO searchModel)
        {
            var jsonData = SerializeHelper.ToJson(searchModel);
            ServiceResult<List<RoleSimpleDTO>> result = null;
            await WebSendPostAsync<ServiceResult<List<RoleSimpleDTO>>>(
                _serviceName + ".LoadRolesWithUsersByRoleIds",
                _accoutServerUrl + "AccountApi/LoadRolesWithUsersByRoleIds",
                ApplicationConstant.AccScope,
                jsonData,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<RoleSimpleDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
                return result.Result;

            return new List<RoleSimpleDTO>();
        }

        /// <summary>
        /// 根据角色名称，获取角色信息（包含所有角色的用户）
        /// </summary>
        /// <param name="roleId">角色名称</param>
        /// <returns></returns>
        public async Task<RoleSimpleDTO> GetRoleWithUsersByRoleId(string roleId)
        {
            //var cacheKey = "WebApiService.AccountApiService-GetRoleWithUsersByRoleId-" + roleId;
            //var cache = Service.CacheUtil.GetCache<RoleSimpleDTO>(cacheKey);
            //if (cache != null)
            //    return cache;

            ServiceResult<RoleSimpleDTO> result = null;
            await WebSendGetAsync<ServiceResult<RoleSimpleDTO>>(
                _serviceName + ".GetRoleWithUsersByRoleId",
                _accoutServerUrl + "AccountApi/GetRoleWithUsersByRoleId?roleId=" + roleId,
                ApplicationConstant.AccScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<RoleSimpleDTO>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                //Service.CacheUtil.SetCache(cacheKey, result.Result, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));

                return result.Result;
            }

            return null;
        }

        /// <summary>
        /// 获取角色Id列表下的所有权限
        /// </summary>
        /// <param name="roleIds">角色Id列表</param>
        /// <returns></returns>
        public async Task<List<MenuNodeSimpleDTO>> LoadUserMenusByRoleIds(List<string> roleIds)
        {
            var jsonData = SerializeHelper.ToJson(roleIds);
            ServiceResult<List<MenuNodeSimpleDTO>> result = null;
            await WebSendPostAsync<ServiceResult<List<MenuNodeSimpleDTO>>>(
                _serviceName + ".LoadUserMenusByRoleIds",
                _accoutServerUrl + "AccountApi/LoadUserMenusByRoleIds",
                ApplicationConstant.AccScope,
                jsonData,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<MenuNodeSimpleDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                return result.Result;
            }

            return null;
        }
        /// <summary>
        /// 获取角色Id列表下的所有权限
        /// </summary>
        /// <param name="roleIds">角色Id列表</param>
        /// <returns></returns>
        public async Task<List<PermissionSimpleDTO>> LoadUserPermissionsByRoleIds(List<string> roleIds)
        {
            var jsonData = SerializeHelper.ToJson(roleIds);
            ServiceResult<List<PermissionSimpleDTO>> result = null;
            await WebSendPostAsync<ServiceResult<List<PermissionSimpleDTO>>>(
                _serviceName + ".LoadUserPermissionsByRoleIds",
                _accoutServerUrl + "AccountApi/LoadUserPermissionsByRoleIds",
                ApplicationConstant.AccScope,
                jsonData,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<PermissionSimpleDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                return result.Result;
            }

            return null;
        }

        /// <summary>
        /// 保存应用的权限数据
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public async Task<bool> SavePermissionsAsync(List<PermissionDTO> permissions, Guid appGuid)
        {
            var jsonData = SerializeHelper.ToJson(permissions);
            ServiceResult<bool> result = null;
            await WebSendPostAsync<ServiceResult<bool>>(
                _serviceName + ".SavePermissions",
                _accoutServerUrl + "AccountApi/SavePermissions?appId=" + appGuid,
                ApplicationConstant.AccScope,
                jsonData,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<bool>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success)
            {
                return result.Result;
            }

            return false;
        }

        /// <summary>
        /// 保存应用的权限数据
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public async Task<bool> SaveMenusAsync(List<MenuNodeDTO> menus, Guid appGuid)
        {
            var jsonData = SerializeHelper.ToJson(menus);
            ServiceResult<bool> result = null;
            await WebSendPostAsync<ServiceResult<bool>>(
                _serviceName + ".SaveMenusAsync",
                _accoutServerUrl + "AccountApi/SaveMenus?appId=" + appGuid,
                ApplicationConstant.AccScope,
                jsonData,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<bool>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success)
            {
                return result.Result;
            }

            return false;
        }
        #endregion

        #region 租户内部的员工信息
        #region 获取租户员工信息
        /// <summary>
        /// 根据用户Id列表，获取主管信息
        /// </summary>
        /// <param name="userId">用户Id列表</param>
        /// <returns></returns>
        public async Task<List<UserSimpleDTO>> LoadUserManagersByUserId(string userId)
        {
            //var cacheKey = "WebApiService.AccountApiService-LoadUserManagersByUserId-" + userId;
            //var cache = Service.CacheUtil.GetCache<List<UserSimpleDTO>>(cacheKey);
            //if (cache != null)
            //    return cache;

            ServiceResult<List<UserSimpleDTO>> result = null;
            await WebSendGetAsync<ServiceResult<List<UserSimpleDTO>>>(
                _serviceName + ".LoadUserManagersByUserId",
                _accoutServerUrl + "AccountApi/LoadUserManagersByUserId?userId=" + userId,
                ApplicationConstant.AccScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<UserSimpleDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                //Service.CacheUtil.SetCache(cacheKey, result.Result, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));

                return result.Result;
            }

            return null;
        }
        /// <summary>
        /// 根据用户Id列表，获取上级主管信息
        /// </summary>
        /// <param name="userId">用户Id列表</param>
        /// <returns></returns>
        public async Task<List<UserSimpleDTO>> LoadUserSupervisorsByUserId(string userId)
        {
            //var cacheKey = "WebApiService.AccountApiService-LoadUserSupervisorsByUserId-" + userId;
            //var cache = Service.CacheUtil.GetCache<List<UserSimpleDTO>>(cacheKey);
            //if (cache != null)
            //    return cache;

            ServiceResult<List<UserSimpleDTO>> result = null;
            await WebSendGetAsync<ServiceResult<List<UserSimpleDTO>>>(
                _serviceName + ".LoadUserSupervisorsByUserId",
                _accoutServerUrl + "AccountApi/LoadUserSupervisorsByUserId?userId=" + userId,
                ApplicationConstant.AccScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<UserSimpleDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                //Service.CacheUtil.SetCache(cacheKey, result.Result, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));

                return result.Result;
            }

            return null;
        }
        /// <summary>
        /// 获取该组织下的所有员工
        /// </summary>
        /// <param name="orgId">组织Id</param>
        /// <returns></returns>
        public async Task<List<UserSimpleDTO>> LoadUsersByOrgId(int orgId)
        {
            //var cacheKey = "WebApiService.AccountApiService-LoadUsersByOrgId-" + orgId;
            //var cache = Service.CacheUtil.GetCache<List<UserSimpleDTO>>(cacheKey);
            //if (cache != null)
            //    return cache;

            ServiceResult<List<UserSimpleDTO>> result = null;
            await WebSendGetAsync<ServiceResult<List<UserSimpleDTO>>>(
                _serviceName + ".LoadUsersByOrgId",
                _accoutServerUrl + "AccountApi/LoadUsersByOrgId?orgId=" + orgId,
                ApplicationConstant.AccScope, 
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<UserSimpleDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                //Service.CacheUtil.SetCache(cacheKey, result.Result, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));

                return result.Result;
            }

            return null;
        }
        /// <summary>
        /// 根据用户Id列表，获取所有员工
        /// </summary>
        /// <param name="userIds">用户Id列表</param>
        /// <returns></returns>
        public async Task<List<UserSimpleDTO>> LoadUsersByIds(List<string> userIds)
        {
            var jsonData = SerializeHelper.ToJson(userIds);
            ServiceResult<List<UserSimpleDTO>> result = null;
            await WebSendPostAsync<ServiceResult<List<UserSimpleDTO>>>(
                _serviceName + ".LoadUsersByIds",
                _accoutServerUrl + "AccountApi/LoadUsersByIds",
                ApplicationConstant.AccScope, 
                jsonData,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<UserSimpleDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                return result.Result;
            }

            return null;
        }
        /// <summary>
        /// 获取所有组织的下的所有员工
        /// </summary>
        /// <param name="orgIds">所有组织的Id列表</param>
        /// <returns></returns>
        public async Task<List<UserSimpleDTO>> LoadUsersByOrgIds(List<int> orgIds)
        {
            var jsonData = SerializeHelper.ToJson(orgIds);
            ServiceResult<List<UserSimpleDTO>> result = null;
            await WebSendPostAsync<ServiceResult<List<UserSimpleDTO>>>(
                _serviceName + ".LoadUsersByOrgIds",
                _accoutServerUrl + "AccountApi/LoadUsersByOrgIds",
                ApplicationConstant.AccScope,
                jsonData,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<UserSimpleDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                return result.Result;
            }

            return null;
        }
        /// <summary>
        /// 获取所有角色的下的所有员工
        /// </summary>
        /// <param name="roleIds">所有角色的Id列表</param>
        /// <returns></returns>
        public async Task<List<UserSimpleDTO>> LoadUsersByRoleIds(List<string> roleIds)
        {
            var jsonData = SerializeHelper.ToJson(roleIds);
            ServiceResult<List<UserSimpleDTO>> result = null;
            await WebSendPostAsync<ServiceResult<List<UserSimpleDTO>>>(
                _serviceName + ".LoadUsersByRoleIds",
                _accoutServerUrl + "AccountApi/LoadUsersByRoleIds",
                ApplicationConstant.AccScope, 
                jsonData,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<UserSimpleDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                return result.Result;
            }

            return null;
        }
        /// <summary>
        /// 根据用户Id列表、角色Id列表、部门Id列表，获取所有的所有员工
        /// </summary>
        /// <param name="searchModel">用户Id列表、角色Id列表、部门Id列表</param>
        /// <returns></returns>
        public async Task<List<UserSimpleDTO>> LoadUsersByIdsAndRoleIdsAndOrgCodes(DataPermissionSearchDTO searchModel)
        {
            var jsonData = SerializeHelper.ToJson(searchModel);
            ServiceResult<List<UserSimpleDTO>> result = null;
            await WebSendPostAsync<ServiceResult<List<UserSimpleDTO>>>(
                _serviceName + ".LoadUsersByIdsAndRoleIdsAndOrgCodes",
                _accoutServerUrl + "AccountApi/LoadUsersByIdsAndRoleIdsAndOrgCodes",
                ApplicationConstant.AccScope,
                jsonData,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<UserSimpleDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                return result.Result;
            }

            return null;
        }

        /// <summary>
        /// 获取用户的简单数据（包含：所属的组织架构列表，不包含：系统用参数--邮箱是否确定、加密密码等等）
        /// </summary>
        /// <param name="userId">用户Guid</param>
        /// <returns></returns>
        public async Task<UserSimpleDTO> GetUserWithOrgsAndRolesByUserId(string userId)
        {
            //var cacheKey = "WebApiService.AccountApiService-GetUserWithOrgsAndRolesByUserId-" + userId;
            //var cache = Service.CacheUtil.GetCache<UserSimpleDTO>(cacheKey);
            //if (cache != null)
            //    return cache;

            ServiceResult<UserSimpleDTO> result = null;
            await WebSendGetAsync<ServiceResult<UserSimpleDTO>>(
                _serviceName + ".GetUserWithOrgsAndRolesByUserId",
                _accoutServerUrl + "AccountApi/GetUserWithOrgsAndRolesByUserId?userId=" + userId,
                ApplicationConstant.AccScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<UserSimpleDTO>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                //Service.CacheUtil.SetCache(cacheKey, result.Result, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));

                return result.Result;
            }

            return null;
        }
        /// <summary>
        /// 获取用户的简单数据（包含：所属的组织架构列表，不包含：系统用参数--邮箱是否确定、加密密码等等）
        /// </summary>
        /// <param name="userId">用户Guid</param>
        /// <returns></returns>
        public async Task<UserContactInfoDTO> GetUserContactInfoByUserId(string userId)
        {
            //var cacheKey = "WebApiService.AccountApiService-GetUserContactInfoByUserId-" + userId;
            //var cache = Service.CacheUtil.GetCache<UserContactInfoDTO>(cacheKey);
            //if (cache != null)
            //    return cache;

            ServiceResult<UserContactInfoDTO> result = null;
            await WebSendGetAsync<ServiceResult<UserContactInfoDTO>>(
                _serviceName + ".GetUserContactInfoByUserId",
                _accoutServerUrl + "AccountApi/GetUserContactInfoByUserId?userId=" + userId,
                ApplicationConstant.AccScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<UserContactInfoDTO>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                //Service.CacheUtil.SetCache(cacheKey, result.Result, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));

                return result.Result;
            }

            return null;
        }
        #endregion

        #region 更改用户信息
        /// <summary>
        /// 修改租户的系统管理员的默认密码：123456
        /// </summary>
        /// <param name="password"></param>
        /// <param name="adminEmail"></param>
        /// <param name="adminPhone"></param>
        /// <returns></returns>
        public async Task<ServiceResult<bool>> ChangeAdminRawInfo(string password, string adminEmail, string adminPhone)
        {
            ServiceResult<bool> result = null;
            await WebSendGetAsync<ServiceResult<bool>>(
                _serviceName + ".ChangeAdminRawInfo",
                _accoutServerUrl + "AccountApi/ChangeAdminRawInfoAsync?password=" + password + "&adminEmail=" + adminEmail + "&adminPhone=" + adminPhone,
                ApplicationConstant.AccScope, 
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<bool>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            return result;
        }

        /// <summary>
        /// 修改登录用户的密码
        /// </summary>
        /// <param name="userId">登录用户的Id</param>
        /// <param name="currentPassword">登录用户的原密码</param>
        /// <param name="newPassword">登录用户的新密码</param>
        /// <returns></returns>
        public async Task<ServiceResult<bool>> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            ServiceResult<bool> result = null;
            await WebSendGetAsync<ServiceResult<bool>>(
                _serviceName + ".ChangePasswordAsync",
                _accoutServerUrl + "AccountApi/ChangePasswordAsync?userId=" + userId + "&currentPassword=" + currentPassword + "&newPassword=" + newPassword,
                ApplicationConstant.AccScope,
                DefaultContentType,
                null,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<bool>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            return result;
        }
        /// <summary>
        /// 修改登录用户的邮箱及密码
        /// </summary>
        /// <param name="userId">登录用户的Id</param>
        /// <param name="email">登录用户的邮箱</param>
        /// <param name="phone">登录用户的密码</param>
        /// <returns></returns>
        public async Task<ServiceResult<bool>> ChangeMailPhoneAsync(string userId, string email, string phone)
        {
            ServiceResult<bool> result = null;
            await WebSendGetAsync<ServiceResult<bool>>(
                _serviceName + ".ChangeMailPhoneAsync",
                _accoutServerUrl + "AccountApi/ChangeMailPhoneAsync?userId=" + userId + "&email=" + email + "&phone=" + phone,
                ApplicationConstant.AccScope,
                DefaultContentType,
                null,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<bool>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            return result;
        }
        #endregion
        #endregion


        /// <summary>
        /// 获取企业的UKey认证数据
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public ServiceResult<UkeyAuthenticationDTO> GetUkeyAuthenticationByMemberId(string memberId)
        {
            ServiceResult<UkeyAuthenticationDTO> result = null;
            WebSendGet<ServiceResult<UkeyAuthenticationDTO>>(
                _serviceName + ".GetUkeyAuthenticationByMemberId",
                _accoutServerUrl + "Account/GetUkeyAuthenticationByMemberId?memberId=" + memberId,
                ApplicationConstant.AccScope,
                callback =>
                {
                    result = callback;
                },
                 (httpStatusCode, errorMessage) =>
                 {
                     result = new ServiceResult<UkeyAuthenticationDTO>(ServiceResultType.Error, errorMessage);
                 },
                true);
            return result;
        }
    }
}
