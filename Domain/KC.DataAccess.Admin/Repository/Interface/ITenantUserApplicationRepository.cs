using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using KC.Model.Admin;

namespace KC.DataAccess.Admin.Repository
{
    public interface ITenantUserApplicationRepository : Database.IRepository.IDbRepository<TenantUserApplication>
    {
        List<TenantUserApplication> FindWithModulesByFilter(Expression<Func<TenantUserApplication, bool>> predicate);
        TenantUserApplication GetTenantApplicationByFilter(Expression<Func<TenantUserApplication, bool>> predicate);
        List<TenantUserApplication> GetTenantApplicationsByFilter<K>(Expression<Func<TenantUserApplication, bool>> predicate, Expression<Func<TenantUserApplication, K>> keySelector, bool ascending = true);
        TenantUserApplication GetWithModulesByFilter(Expression<Func<TenantUserApplication, bool>> predicate);
    }
}