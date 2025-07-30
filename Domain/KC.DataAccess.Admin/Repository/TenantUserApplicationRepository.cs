using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using KC.Database.EFRepository;
using KC.Model.Admin;

namespace KC.DataAccess.Admin.Repository
{
    public class TenantUserApplicationRepository : EFRepositoryBase<TenantUserApplication>, ITenantUserApplicationRepository
    {
        public TenantUserApplicationRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        public List<TenantUserApplication> GetTenantApplicationsByFilter<K>(
            Expression<Func<TenantUserApplication, bool>> predicate,
            Expression<Func<TenantUserApplication, K>> keySelector, bool ascending = true)
        {
            return @ascending
                ? Entities.Where(predicate).AsNoTracking().OrderBy(keySelector).ToList()
                : Entities.Where(predicate).AsNoTracking().OrderByDescending(keySelector).ToList();
        }

        public TenantUserApplication GetTenantApplicationByFilter(Expression<Func<TenantUserApplication, bool>> predicate)
        {
            return Entities
                .AsNoTracking()
                .FirstOrDefault(predicate);
        }

        public List<TenantUserApplication> FindWithModulesByFilter(Expression<Func<TenantUserApplication, bool>> predicate)
        {
            return Entities
                //.Include(m => m.ApplicationModules)
                .Where(predicate)
                .AsNoTracking()
                .ToList();
        }

        public TenantUserApplication GetWithModulesByFilter
            (Expression<Func<TenantUserApplication, bool>> predicate)
        {
            return Entities
                //.Include(m => m.ApplicationModules)
                .AsNoTracking()
                .FirstOrDefault(predicate);
        }
    }
}
