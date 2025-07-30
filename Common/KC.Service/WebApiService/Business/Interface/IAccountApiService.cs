using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using KC.Service.DTO.Account;
using KC.Service.DTO.Search;

namespace KC.Service.WebApiService.Business
{
    public interface IAccountApiService
    {
        /// <summary>
        /// 获取用户登录信息
        /// </summary>
        /// <param name="userName">用户名/用户邮箱/用户手机</param>
        /// <param name="password">用户密码</param>
        /// <returns></returns>
        Task<ServiceResult<UserDTO>> TenantUserLogin(string userName, string password);

        #region 组织架构
        /// <summary>
        /// 获取所有的组织架构(非树状list类型)
        /// </summary>
        /// <returns></returns>
        [Extension.CachingCallHandler()]
        Task<List<OrganizationSimpleDTO>> LoadAllOrganization();
        /// <summary>
        /// 根据用户Id，获取其所属部门及下属员工(非树状list类型)
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        [Extension.CachingCallHandler()]
        Task<List<OrganizationSimpleDTO>> LoadOrganizationsWithUsersByUserId(string userId);
        /// <summary>
        /// 根据用户Id，获取其所属上级部门及下属员工(非树状list类型)
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        [Extension.CachingCallHandler()]
        Task<List<OrganizationSimpleDTO>> LoadHigherOrganizationsWithUsersByUserId(string userId);

        /// <summary>
        /// 获取所有的部门信息（部门树：包含其下属部门）
        /// </summary>
        /// <returns></returns>
        [Extension.CachingCallHandler()]
        Task<List<OrganizationSimpleDTO>> LoadAllOrgTrees();

        /// <summary>
        /// 获取所有的部门信息及下属员工
        /// </summary>
        /// <returns></returns>
        [Extension.CachingCallHandler()]
        Task<List<OrganizationSimpleDTO>> LoadOrgTreesWithUsers();

        /// <summary>
        /// 根据组织Id列表，获取所有的部门信息 <br/>
        ///     （部门列表，包含其下属所有的部门信息：递归查询）
        /// </summary>
        /// <param name="orgIds">组织Id列表</param>
        /// <returns></returns>
        Task<List<OrganizationSimpleDTO>> LoadOrgTreesByOrgIds(List<int> orgIds);

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
        Task<List<OrganizationSimpleDTO>> LoadOrgTreesWithUsersByOrgIds(OrgTreesAndRolesWithUsersSearchDTO searchModel);

        #endregion

        #region 角色信息
        /// <summary>
        /// 获取所有的角色列表
        /// </summary>
        /// <returns></returns>
        [Extension.CachingCallHandler()]
        Task<List<RoleSimpleDTO>> LoadAllRoles();
        /// <summary>
        /// 根据用户UserId，获取角色列表
        /// </summary>
        /// <param name="userId">用户UserId</param>
        /// <returns></returns>
        [Extension.CachingCallHandler()]
        Task<List<RoleSimpleDTO>> LoadRolesWithUsersByUserId(string userId);
        /// <summary>
        /// 获取所有角色及其下属员工，筛选顺序如下： </br>
        ///     1. RoleIds为空或无值时，显示所有角色列表 </br>
        ///     2. RoleIds有值时，为需要包含用户的角色Id列表 </br>
        ///     3. ExceptRoleIds为需要排除的角色Id列表
        /// </summary>
        /// <param name="searchModel">
        ///     RoleIds、ExceptRoleIds：角色Id列表
        ///     DepIds、ExceptOrgIds：可为空，不参与计算
        /// </param>
        /// <returns></returns>
        Task<List<RoleSimpleDTO>> LoadRolesWithUsersByRoleIds(OrgTreesAndRolesWithUsersSearchDTO searchModel);

        /// <summary>
        /// 根据角色名称，获取角色信息（包含所有角色的用户）
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        [Extension.CachingCallHandler()]
        Task<RoleSimpleDTO> GetRoleWithUsersByRoleId(string roleId);

        /// <summary>
        /// 获取角色Id列表下的所有菜单
        /// </summary>
        /// <param name="roleIds">角色Id列表</param>
        /// <returns></returns>
        Task<List<MenuNodeSimpleDTO>> LoadUserMenusByRoleIds(List<string> roleIds);
        /// <summary>
        /// 获取角色Id列表下的所有权限
        /// </summary>
        /// <param name="roleIds">角色Id列表</param>
        /// <returns></returns>
        Task<List<PermissionSimpleDTO>> LoadUserPermissionsByRoleIds(List<string> roleIds);

        Task<bool> SavePermissionsAsync(List<PermissionDTO> models, Guid appGuid);

        Task<bool> SaveMenusAsync(List<MenuNodeDTO> models, Guid appGuid);
        #endregion

        #region 租户内部的员工信息
        /// <summary>
        /// 根据用户Id列表，获取主管信息
        /// </summary>
        /// <param name="userId">用户Id列表</param>
        /// <returns></returns>
        [Extension.CachingCallHandler()]
        Task<List<UserSimpleDTO>> LoadUserManagersByUserId(string userId);
        /// <summary>
        /// 根据用户Id列表，获取上级主管信息
        /// </summary>
        /// <param name="userId">用户Id列表</param>
        /// <returns></returns>
        [Extension.CachingCallHandler()]
        Task<List<UserSimpleDTO>> LoadUserSupervisorsByUserId(string userId);
        /// <summary>
        /// 获取该组织下的所有员工
        /// </summary>
        /// <param name="orgId">组织Id</param>
        /// <returns></returns>
        [Extension.CachingCallHandler()]
        Task<List<UserSimpleDTO>> LoadUsersByOrgId(int orgId);

        /// <summary>
        /// 根据用户Id列表，获取所有员工
        /// </summary>
        /// <param name="userIds">用户Id列表</param>
        /// <returns></returns>
        Task<List<UserSimpleDTO>> LoadUsersByIds(List<string> userIds);

        /// <summary>
        /// 获取所有组织的下的所有员工
        /// </summary>
        /// <param name="orgIds">所有组织的Id列表</param>
        /// <returns></returns>
        Task<List<UserSimpleDTO>> LoadUsersByOrgIds(List<int> orgIds);

        /// <summary>
        /// 获取所有角色的下的所有员工
        /// </summary>
        /// <param name="roleIds">所有角色的Id列表</param>
        /// <returns></returns>
        Task<List<UserSimpleDTO>> LoadUsersByRoleIds(List<string> roleIds);

        /// <summary>
        /// 根据用户Id列表、角色Id列表、部门编码列表，获取所有的所有员工
        /// </summary>
        /// <param name="searchModel">用户Id列表、角色Id列表、部门编码列表</param>
        /// <returns></returns>
        Task<List<UserSimpleDTO>> LoadUsersByIdsAndRoleIdsAndOrgCodes(DataPermissionSearchDTO searchModel);

        /// <summary>
        /// 获取用户的联系信息
        /// </summary>
        /// <param name="userId">用户Guid</param>
        /// <returns></returns>
        [Extension.CachingCallHandler()]
        Task<UserContactInfoDTO> GetUserContactInfoByUserId(string userId);

        /// <summary>
        /// 获取用户的简单数据 </br>
        ///     包含：所属的组织架构列表
        ///     不包含：系统用参数--邮箱是否确定、加密密码等等
        /// </summary>
        /// <param name="userId">用户Guid</param>
        /// <returns></returns>
        [Extension.CachingCallHandler()]
        Task<UserSimpleDTO> GetUserWithOrgsAndRolesByUserId(string userId);

        /// <summary>
        /// 修改租户的系统管理员的默认密码：123456
        /// </summary>
        /// <param name="password"></param>
        /// <param name="adminEmail"></param>
        /// <param name="adminPhone"></param>
        /// <returns></returns>
        Task<ServiceResult<bool>> ChangeAdminRawInfo(string password, string adminEmail, string adminPhone);

        /// <summary>
        /// 修改登录用户的密码
        /// </summary>
        /// <param name="userId">登录用户的Id</param>
        /// <param name="currentPassword">登录用户的原密码</param>
        /// <param name="newPassword">登录用户的新密码</param>
        /// <returns></returns>
        Task<ServiceResult<bool>> ChangePasswordAsync(string userId, string currentPassword, string newPassword);

        /// <summary>
        /// 修改登录用户的邮箱及手机
        /// </summary>
        /// <param name="userId">登录用户的Id</param>
        /// <param name="email">登录用户的邮箱</param>
        /// <param name="phone">登录用户的手机</param>
        /// <returns></returns>
        Task<ServiceResult<bool>> ChangeMailPhoneAsync(string userId, string email, string phone);
        #endregion

        /// <summary>
        /// 获取企业的UKey认证数据
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        ServiceResult<UkeyAuthenticationDTO> GetUkeyAuthenticationByMemberId(string memberId);
    }
}
