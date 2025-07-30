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
    public class TenantUserOpenAppErrorLogRepository : EFRepositoryBase<TenantUserOpenAppErrorLog>, ITenantUserOpenAppErrorLogRepository
    {
        public TenantUserOpenAppErrorLogRepository(EFUnitOfWorkContextBase unitOfWork) 
            : base(unitOfWork)
        {
        }

        public List<TenantUserOpenAppErrorLog> GetTenantApplicationsByFilter<K>(
            Expression<Func<TenantUserOpenAppErrorLog, bool>> predicate,
            Expression<Func<TenantUserOpenAppErrorLog, K>> keySelector, bool ascending = true)
        {
            return @ascending
                ? Entities.Include(m => m.TenantUser).Where(predicate).AsNoTracking().OrderBy(keySelector).ToList()
                : Entities.Where(predicate).AsNoTracking().OrderByDescending(keySelector).ToList();
        }

        public override Tuple<int, IList<TenantUserOpenAppErrorLog>> FindPagenatedListWithCount<K>(int pageIndex, int pageSize,
            Expression<Func<TenantUserOpenAppErrorLog, bool>> predicate,
            Expression<Func<TenantUserOpenAppErrorLog, K>> keySelector, bool ascending = true)
        {
            var databaseItems = EFContext.Set<TenantUserOpenAppErrorLog>().Include(m => m.TenantUser).Where(predicate).AsNoTracking();
            int recordCount = databaseItems.Count();

            return @ascending
                ? new Tuple<int, IList<TenantUserOpenAppErrorLog>>(recordCount, databaseItems.OrderBy(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList())
                : new Tuple<int, IList<TenantUserOpenAppErrorLog>>(recordCount, databaseItems.OrderByDescending(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList());
        }

        public TenantUserOpenAppErrorLog GetTenantApplicationByFilter(Expression<Func<TenantUserOpenAppErrorLog, bool>> predicate)
        {
            return Entities.Include(m => m.TenantUser)
                .AsNoTracking()
                .FirstOrDefault(predicate);
        }
    }
}
