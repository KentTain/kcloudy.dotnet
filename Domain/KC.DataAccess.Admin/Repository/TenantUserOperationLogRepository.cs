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
    public class TenantUserOperationLogRepository : EFRepositoryBase<TenantUserOperationLog>, ITenantUserOperationLogRepository
    {
        public TenantUserOperationLogRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {

        }
        public override Tuple<int, IList<TenantUserOperationLog>> FindPagenatedListWithCount<K>(int pageIndex, int pageSize,
            Expression<Func<TenantUserOperationLog, bool>> predicate,
            Expression<Func<TenantUserOperationLog, K>> keySelector, bool ascending = true)
        {
            var databaseItems = EFContext.Set<TenantUserOperationLog>().Include(m=>m.TenantUserApplication).Include(n=>n.TenantUserApplication.TenantUser)
                .Where(predicate);
            int recordCount = databaseItems.Count();

            return @ascending
                ? new Tuple<int, IList<TenantUserOperationLog>>(recordCount, databaseItems.OrderBy(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize)
                .ToList())
                : new Tuple<int, IList<TenantUserOperationLog>>(recordCount, databaseItems.OrderByDescending(keySelector).Skip((pageIndex - 1) * pageSize)
                .Take(pageSize).ToList());
        }


    }
}
