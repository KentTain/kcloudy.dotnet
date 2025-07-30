using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Extension;
using KC.Database.EFRepository;
using KC.Model.Account;

namespace KC.DataAccess.Account.Repository
{
    public class OrganizationRepository : EFTreeRepositoryBase<Organization>, IOrganizationRepository
    {
        public OrganizationRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        //public OrganizationRepository(Tenant tenant)
        //    : base(new ComAccountUnitOfWorkContext(tenant))
        //{
        //}
        public IList<Organization> GetAllOrgsWithNestParentAndChildAndUsers(Expression<Func<Organization, bool>> predicate, bool isIncludUserRoles = false)
        {
            var nodes = EFContext.Set<Organization>().Where(predicate).AsNoTracking().ToList();
            if (nodes == null || !nodes.Any())
                return new List<Organization>();

            var allIds = new HashSet<int>();
            var lastIds = new HashSet<int>();
            nodes.ForEach(m => {
                var newIds = m.TreeCode.ArrayFromCommaDelimitedIntegersBySplitChar('-').ToList();
                lastIds.Add(newIds.LastOrDefault());
                newIds.ForEach(n => allIds.Add(n));
            });

            var entities = isIncludUserRoles
                ? EFContext.Context.Set<Organization>()
                    .Include(m => m.OrganizationUsers)
                        .ThenInclude(r => r.User)
                            .ThenInclude(u => u.UserRoles)
                                .ThenInclude(u => u.Role)
                    .Where(m => !m.IsDeleted && allIds.Contains(m.Id))
                    .AsNoTracking()
                    .ToList()
                : EFContext.Context.Set<Organization>()
                    .Include(m => m.OrganizationUsers)
                        .ThenInclude(r => r.User)
                    .Where(m => !m.IsDeleted && allIds.Contains(m.Id))
                    .AsNoTracking()
                    .ToList();

            return entities;
        }

        public IList<Organization> GetAllOrganizationsWithUsers(Expression<Func<Organization, bool>> predicate, bool isIncludUserRoles = false)
        {
            Expression<Func<User, bool>> sqlWhere = c => true;
  
            var orgs = isIncludUserRoles
                ? EFContext.Set<Organization>()
                    .Include(m => m.OrganizationUsers)
                        .ThenInclude(r => r.User)
                            .ThenInclude(u => u.UserRoles)
                                .ThenInclude(u => u.Role)
                    .Where(predicate)
                    .AsNoTracking()
                    .OrderBy(m => m.Index)
                    .ToList()
                : EFContext.Set<Organization>()
                    .Include(m => m.OrganizationUsers)
                        .ThenInclude(r => r.User)
                    .Where(predicate)
                    .AsNoTracking()
                    .OrderBy(m => m.Index)
                    .ToList();
            orgs.ForEach(k =>
            {
                k.OrganizationUsers = GetValidUsers(k.OrganizationUsers, m => true).ToList();
            });
            return orgs;
        }

        public Organization GetOrganizationsWithUsersByOrgId(int orgId)
        {
            var orgs = EFContext.Set<Organization>()
                .Include(m => m.ChildNodes)
                    .ThenInclude(m => m.ChildNodes)
                        .ThenInclude(m => m.ChildNodes)
                .Include(m => m.OrganizationUsers)
                    .ThenInclude(r => r.User)
                .AsNoTracking()
                 .FirstOrDefault(m => m.Id == orgId);

            orgs.OrganizationUsers = GetValidUsers(orgs.OrganizationUsers, m => true).ToList();

            return orgs;
        }

        private void GetNestedUsers(Organization parentOrg, ref List<User> result)
        {
            if (parentOrg == null)
                return;

            result.AddRange(parentOrg.OrganizationUsers.Select(u => u.User));
            foreach (var childOrg in parentOrg.ChildNodes)
            {
                GetNestedUsers(childOrg, ref result);
            }
        }

        private IEnumerable<UsersInOrganizations> GetValidUsers(IEnumerable<UsersInOrganizations> usersInOrgs, Func<User, bool> predicate)
        {
            var users = usersInOrgs.Select(u => u.User).Where(predicate).Select(m => m.Id);
            return usersInOrgs.Where(m => users.Contains(m.UserId));
        }

    }
}
