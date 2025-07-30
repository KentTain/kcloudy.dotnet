using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KC.Model.Account;

namespace KC.DataAccess.Account.Repository
{
    public interface IRoleRepository: Microsoft.AspNetCore.Identity.IRoleStore<Role>
    {
        List<Role> FindByFilter(Expression<Func<Role, bool>> predicate);
        Task<List<Role>> FindByFilterAsync(Expression<Func<Role, bool>> predicate);
        List<Role> FindDetailRoles();
        List<Role> FindDetailRolesByNames(List<string> names);
        Tuple<int, List<Role>> FindPagenatedRoles<K>(int pageIndex, int pageSize, Expression<Func<Role, bool>> predicate, Expression<Func<Role, K>> keySelector, bool paging = true, bool ascending = true);
        List<Role> FindRolesByNames(List<string> names);
        Task<IEnumerable<Role>> FindRolesMenusByIdsAsync(List<string> roleIds);
        Task<List<Role>> FindRolesPermissionsByIdsAsync(List<string> roleIds);
        IList<Role> FindRoleWithUsersByFilter<K>(Expression<Func<Role, bool>> predicate, Expression<Func<Role, K>> keySelector, bool ascending = true);
        Task<IList<Role>> FindRoleWithUsersByFilterAsync<K>(Expression<Func<Role, bool>> predicate,
            Expression<Func<Role, K>> keySelector, bool ascending = true);

        Role GetDetailRoleByRoleId(string roleId);
        void GetRolesByParentRoleIds(List<string> roleIds, ref List<Role> roles);
        bool ExistsRole(Expression<Func<Role, bool>> predicate);
        bool CreateRole(Role model);
        bool RomoveRole(string roleId, string operatorId, string operatorName);
        bool SaveMenuInRole(string roleId, List<int> newIds, string operatorId, string operatorName);
        bool SavePermissionInRole(string roleId, List<int> newIds, string operatorId, string operatorName);
        bool SaveUserInRole(string roleId, List<string> addList, List<string> delList, string operatorId, string operatorName);
        bool UpdateRole(Role model);
    }
}