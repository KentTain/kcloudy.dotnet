using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KC.Model.Account;

namespace KC.DataAccess.Account.Repository
{
    public interface IPermissionRepository : Database.IRepository.IDbTreeRepository<Permission>
    {
        bool ExistsPermission(Guid appId, string controllerName, string actionName);
        //Tuple<int, IList<Permission>> FindPagenatedListWithCount<K>(int pageIndex, int pageSize, Expression<Func<Permission, bool>> predicate, string sortProperty, bool ascending = true);
        Permission GetDetailPermissionById(params object[] keyValues);
        Permission GetPermissionByAppIDAndName(Guid? appId, string controllerName, string actionName);
        IList<Permission> GetPermissionsByRoleIds(List<string> roleIds);
        IList<Permission> GetPermissionsByRoleNames(List<string> roleNames);
        void GetRolesByParentRoleIds(List<string> roleIds, ref List<Role> roles);
        IList<Permission> LoadAllDetailPermissions();
        IList<Permission> LoadAllDetailPermissionsByApplication(Guid applicationId);
        IList<Permission> LoadDetailPermissionsByFilter(Expression<Func<Permission, bool>> predicate);
        IList<Permission> LoadPermissionByIndex(Expression<Func<Permission, bool>> predicate, string sort, bool ascending = true);
        Task<bool> SavePermissionsAsync(List<Permission> permissionTrees, Guid appId);
        bool SaveRoleInPremission(int id, List<string> newIds, string operatorId, string operatorName);
        Task<bool> SetSystemAdminRoleDefaultPermissions();
    }
}