using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Service.DTO.Account;
using KC.Service.WebApiService.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Util
{
    public static class DataPermitUtil
    {
        /// <summary>
        /// 获取数据权限的筛选条件
        /// </summary>
        /// <typeparam name="T">继承DataPermissionEntity对象</typeparam>
        /// <param name="userId">当前用户Id，不能为：NULL</param>
        /// <param name="roleIds">用户所属角色Id列表</param>
        /// <param name="orgCodes">用户所属组织Code列表</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetDatePermitPredicate<T>(string userId, List<string> roleIds, List<string> orgCodes) where T : DataPermitEntity
        {
            if (userId.IsNullOrEmpty())
                throw new ArgumentNullException("userId", "未传入查询数据所属的用户Id。");

            Expression<Func<T, bool>> dataPredicate = m => (
                (m.UserIds != null && m.UserIds.Contains(userId)) 
                || (m.CreatedBy != null && m.CreatedBy.Equals(userId)))
                || (m.ExceptUserIds != null && !m.ExceptUserIds.Contains(userId));
            //Or 当前用户所属角色Id在数据角色权限设置字段（RoleIds）当中
            if (roleIds != null && roleIds.Any())
            {
                foreach (var roleId in roleIds)
                {
                    dataPredicate = dataPredicate.Or(m => m.RoleIds != null && m.RoleIds.Contains(roleId));
                }
            }
            //Or 当前用户所属部门Id在数据角色权限设置字段（OrgIds）当中
            if (orgCodes != null && orgCodes.Any())
            {
                foreach (var orgId in orgCodes)
                {
                    dataPredicate = dataPredicate.Or(m => m.OrgCodes != null && m.OrgCodes.Contains(orgId));
                }
            }

            return dataPredicate;
        }

        #region Account
        /// <summary>
        /// 获取该组织下的所有员工，包括：该组织下的所有子组织
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public static async Task<List<UserSimpleDTO>> LoadSubordinateUsersByOrgId(IAccountApiService AccountApiService, int orgId)
        {
            var user = await AccountApiService.LoadUsersByOrgId(orgId);
            if (user == null)
                return new List<UserSimpleDTO>(); ;

            return user;
        }

        /// <summary>
        /// 获取与该员工相关的下属组织的所有员工：</br>
        ///     如果，该员工为：管理岗，则返回所属的下级组织的所有员工；</br>
        ///     如果，该员工为：职员，则返回该员工的数据；
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<List<UserSimpleDTO>> LoadSubordinateUsersByUserId(IAccountApiService AccountApiService, string userId)
        {
            var result = new List<UserSimpleDTO>();
            var user = await AccountApiService.GetUserWithOrgsAndRolesByUserId(userId);
            if (user == null)
                return result;

            switch (user.PositionLevel)
            {
                //如果员工为职员，则返回该员工的数据；
                case PositionLevel.Staff:
                    result.Add(user);
                    break;
                //如果员工为管理岗，则返回所属的下级组织的所有员工；
                case PositionLevel.Mananger:
                    var orgIds = user.UserOrgIds.ToList();
                    var users = await AccountApiService.LoadUsersByOrgIds(orgIds);
                    if (users != null && users.Any())
                    {
                        result.AddRange(users);
                    }
                    break;
            }

            return result;
        }

        /// <summary>
        /// 获取某操作人的部门，兄弟部门，下级部门，父级部门Ids
        /// </summary>
        /// <param name="currentUserId">当前用户Id</param>
        /// <param name="includeCurrentOrganizations">是否包括本部门</param>
        /// <param name="includeSiblingsOrganizationIds">是否包括兄弟部门</param>
        /// <param name="includeChildOrganizaitons">是否包括子部门</param>
        /// <param name="includeParentOrganizaitons">是否包括父级部门</param>
        /// <returns></returns>
        public static async Task<List<int>> LoadCurrentUserRelationOrganizationIds(IAccountApiService AccountApiService,
            string currentUserId, bool includeCurrentOrganizations = true,
            bool includeSiblingsOrganizationIds = true, bool includeChildOrganizaitons = false, bool includeParentOrganizaitons = false)
        {
            List<int> organizationIds = new List<int>();
            //当前操作人部门
            var currentUserOrganizations = await AccountApiService.LoadOrganizationsWithUsersByUserId(currentUserId);
            if (currentUserOrganizations == null || !currentUserOrganizations.Any())
                return organizationIds;

            //所有部门
            var organizations = await AccountApiService.LoadAllOrganization();
            if (organizations == null || !organizations.Any())
                return organizationIds;

            //当前操作人部门Ids
            if (includeCurrentOrganizations)
            {
                var currentUserOrganizationIds = currentUserOrganizations.Select(o => o.Id);
                organizationIds.AddRange(currentUserOrganizationIds);
            }

            //当前操作人同级（兄弟）部门Ids
            if (includeSiblingsOrganizationIds)
            {
                //当前操作人父级部门
                var currentUserParentOrganizations =
                    organizations.Where(o => currentUserOrganizations.Any(p => p.ParentId == o.Id));
                var currentUserSiblingsOrganizationIds =
                    organizations.Where(o => currentUserParentOrganizations.Any(p => p.Id == o.ParentId))
                        .Select(o => o.Id);
                organizationIds.AddRange(currentUserSiblingsOrganizationIds);
            }

            //当前操作人子部门Ids
            if (includeChildOrganizaitons)
            {
                var currentUserChildOrganizationIds =
                    organizations.Where(o => currentUserOrganizations.Any(p => p.Id == o.ParentId))
                        .Select(q => q.Id);
                organizationIds.AddRange(currentUserChildOrganizationIds);
            }

            //当前操作人父级部门Ids
            if (includeParentOrganizaitons)
            {
                var currentUserParentOrganizationIds =
                    organizations.Where(o => currentUserOrganizations.Any(p => p.ParentId == o.Id))
                        .Select(q => q.Id);
                organizationIds.AddRange(currentUserParentOrganizationIds);
            }
            return organizationIds;
        }

        #endregion
    }
}
