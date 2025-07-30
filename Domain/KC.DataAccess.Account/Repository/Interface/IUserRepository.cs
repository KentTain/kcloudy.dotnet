using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Model.Account;

namespace KC.DataAccess.Account.Repository
{
    public interface IUserRepository: Microsoft.AspNetCore.Identity.IUserStore<User>
    {
        IList<User> FindAllDetailUsers(bool isApproved = true);
        IList<User> FindUsersWithOrgsByIds(List<string> ids);
        IList<User> FindUsersByOrgIds(List<int> origIds);
        IList<User> FindUsersByRoleIds(List<string> roleIds);
        Task<List<User>> FindUsersByFilterAsync(Expression<Func<User, bool>> predicate);

        Tuple<int, IList<User>> FindPagenatedUsersByFilter(int pageIndex, int pageSize, Expression<Func<User, bool>> predicate, string sortProperty, bool ascending = true, bool paging = true);

        Task<User> FindByIdAsync(string userId);

        Task<User> GetUserWithOrgsAndRolesByIdAsync(string userId);

        bool AddUsers(List<User> users, bool isSave = true);
        Task<bool> ExistUserByFilterAsync(Expression<Func<User, bool>> predicate);

        bool RemoveUserById(string userId, string operatorId, string operatorName);
        bool ReactUserById(string userId, string operatorId, string operatorName);

        bool SaveRoleInUser(string id, List<string> newIds, string operatorId, string operatorName);
        bool SaveOrganizationInUser(string id, List<int> newIds, string operatorId, string operatorName);

        bool UpdateUser(User user, List<int> newOrgIds = null, bool isSave = true);
        bool UpdateUserInRoles(List<string> newIds, string userId, bool isSave = true);
        bool UpdateUserInOrganizations(List<int> newIds, string userId, bool isSave = true);
    }
}