using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using KC.Model.Admin;

namespace KC.DataAccess.Admin.Repository
{
    public interface ITenantUserOpenAppErrorLogRepository : Database.IRepository.IDbRepository<TenantUserOpenAppErrorLog>
    {
        //Tuple<int, IList<TenantUserOpenAppErrorLog>> FindPagenatedListWithCount<K>(int pageIndex, int pageSize, Expression<Func<TenantUserOpenAppErrorLog, bool>> predicate, Expression<Func<TenantUserOpenAppErrorLog, K>> keySelector, bool ascending = true);
        TenantUserOpenAppErrorLog GetTenantApplicationByFilter(Expression<Func<TenantUserOpenAppErrorLog, bool>> predicate);
        List<TenantUserOpenAppErrorLog> GetTenantApplicationsByFilter<K>(Expression<Func<TenantUserOpenAppErrorLog, bool>> predicate, Expression<Func<TenantUserOpenAppErrorLog, K>> keySelector, bool ascending = true);
    }
}