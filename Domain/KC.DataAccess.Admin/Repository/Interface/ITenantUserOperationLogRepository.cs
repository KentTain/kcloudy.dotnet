using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using KC.Model.Admin;

namespace KC.DataAccess.Admin.Repository
{
    public interface ITenantUserOperationLogRepository : Database.IRepository.IDbRepository<TenantUserOperationLog>
    {
        //Tuple<int, IList<TenantUserOperationLog>> FindPagenatedListWithCount<K>(int pageIndex, int pageSize, Expression<Func<TenantUserOperationLog, bool>> predicate, Expression<Func<TenantUserOperationLog, K>> keySelector, bool ascending = true);
    }
}