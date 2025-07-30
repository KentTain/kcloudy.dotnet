using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using KC.Model.Account;

namespace KC.DataAccess.Account.Repository
{
    public interface IOrganizationRepository : Database.IRepository.IDbTreeRepository<Organization>
    {
        IList<Organization> GetAllOrgsWithNestParentAndChildAndUsers(Expression<Func<Organization, bool>> predicate, bool isIncludUserRoles = false);

        IList<Organization> GetAllOrganizationsWithUsers(Expression<Func<Organization, bool>> predicate, bool isIncludUserRoles = false);

        Organization GetOrganizationsWithUsersByOrgId(int orgId);
    }
}