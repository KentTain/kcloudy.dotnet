using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Extension;
using KC.Database.EFRepository;
using KC.Enums.App;
using KC.Framework.Tenant;
using KC.Model.Admin;

namespace KC.DataAccess.Admin.Repository
{
    public class TenantUserRepository : EFRepositoryBase<TenantUser>, ITenantUserRepository
    {
        public TenantUserRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        public bool ExistTenantName(string tenantName)
        {
            return Entities.Any(m => m.TenantName == tenantName);
        }


        public IList<TenantUser> FindAllDetailTenantUsers()
        {
            return Entities
                .Include(o => o.Applications)
                .Include(o => o.OwnStorage)
                .Include(o => o.OwnDatabase)
                .AsNoTracking()
                .Where(m => !m.IsDeleted && m.Applications.Any()).ToList();
        }
        public IList<TenantUser> FindAllDetailTenantUsersByNames(List<string> tenantNames)
        {
            var result = Entities
                .Include(o => o.Applications)
                .Include(o => o.OwnStorage)
                .Include(o => o.OwnDatabase)
                .Where(o => !o.IsDeleted && tenantNames.Contains(o.TenantName))
                .AsNoTracking().ToList();

            return result;
        }
        public IList<TenantUser> FindAllTenantUsersByTally(Guid appId, TenantType? tally)
        {
            if (tally.HasValue)
            {
                var result = Entities
                    .Include(o => o.Applications)
                    .Where(o => !o.IsDeleted
                            && ((TenantType)o.TenantType & tally.Value) != 0
                            && o.Applications.Any(c => c.ApplicationId == appId))
                    .AsNoTracking().ToList();
                //base.UnitOfWork.Context.Configuration.LazyLoadingEnabled = true;
                //base.UnitOfWork.Context.Configuration.ProxyCreationEnabled = true;
                return result;
            }

            return Entities
                .Include(o => o.Applications)
                .Where(o => !o.IsDeleted
                        && o.Applications.Any(c => c.ApplicationId == appId))
                .AsNoTracking().ToList();
        }
        public IList<TenantUser> FindOpenAppsTenantUsersByIds(List<int> ids)
        {
            var result = Entities
                .Include(o => o.Applications)
                //.Include(o => o.Applications.Select(m => m.ApplicationModules))
                .Include(o => o.OwnStorage)
                .Include(o => o.OwnDatabase)
                .Where(o => !o.IsDeleted 
                        && ids.Contains(o.TenantId) 
                        && o.Applications.Any())
                .AsNoTracking().ToList();
            //base.UnitOfWork.Context.Configuration.LazyLoadingEnabled = true;
            //base.UnitOfWork.Context.Configuration.ProxyCreationEnabled = true;
            return result;
        }
        public IList<TenantUser> FindOpenAppTenantsByNames(Guid appId, List<string> tenantNames)
        {
            var result = Entities
                .Include(o => o.Applications)
                .Where(
                    o =>
                        !o.IsDeleted && tenantNames.Contains(o.TenantName) &&
                        o.Applications.Any(c => c.ApplicationId == appId))
                .AsNoTracking().ToList();
            //base.UnitOfWork.Context.Configuration.LazyLoadingEnabled = true;
            //base.UnitOfWork.Context.Configuration.ProxyCreationEnabled = true;
            return result;
        }
        public IList<TenantUser> FindAllOpenAppTenantsWithoutTestTenant(Guid appId, string currentTenantName, TenantType? tally = null)
        {
            var exTenants = new List<string>()
            {
                TenantConstant.DbaTenantName,
                TenantConstant.TestTenantName,
                TenantConstant.BuyTenantName,
                TenantConstant.SaleTenantName
            };
            Expression<Func<TenantUser, bool>> predicate = o => !o.IsDeleted;
            predicate = predicate.And(o => !exTenants.Contains(o.TenantName.ToLower()));
            predicate = predicate.And(o => o.Applications.Any(c => c.ApplicationId == appId));

            if (!currentTenantName.IsNullOrEmpty())
            {
                predicate = predicate.And(o => !(o.TenantName.ToLower() == currentTenantName.ToLower()));
            }

            if (tally.HasValue)
            {
                predicate = predicate.And(o => (o.TenantType & tally.Value) != 0);
            }

            return Entities
                .Include(o => o.Applications)
                .Where(predicate)
                .AsNoTracking().ToList();
        }
        public IList<TenantUser> FindAllTenantsWithoutTests(Guid appId, string exceptTenantName)
        {
            var exTenants = new List<string>()
            {
                TenantConstant.DbaTenantName,
                TenantConstant.TestTenantName,
                TenantConstant.BuyTenantName,
                TenantConstant.SaleTenantName
            };
            Expression<Func<TenantUser, bool>> predicate = o => !o.IsDeleted;
            predicate = predicate.And(o => !exTenants.Contains(o.TenantName.ToLower()));
            predicate = predicate.And(o => o.Applications.Any(c => c.ApplicationId == appId));

            predicate = predicate.And(o => o.TenantName != exceptTenantName);
            return Entities
                .Include(o => o.Applications)
                .Where(predicate)
                .AsNoTracking().ToList();
        }


        public Tuple<int, IList<TenantUser>> FindPagenatedTenantsByApplicationId(int pageIndex, int pageSize,
            Guid applicationId, string tenantDisplayName, List<string> exceptTenants = null,
            bool IsShowExceptTenants = false)
        {
            if (!string.IsNullOrWhiteSpace(tenantDisplayName))
            {
                if (exceptTenants != null)
                {
                    var query =
                        applicationId != Guid.Empty
                            ? from tenant in EFContext.Set<TenantUser>()
                              join app in EFContext.Set<TenantUserApplication>() on tenant.TenantId equals app.TenantId
                              where
                                  app.ApplicationId == applicationId
                                  && app.AppStatus == ApplicationStatus.OpenSuccess
                                  && !exceptTenants.Contains(tenant.TenantName)
                                  && tenant.TenantDisplayName.Contains(tenantDisplayName)
                                  && !tenant.IsDeleted
                              select tenant
                            : from tenant in EFContext.Set<TenantUser>()
                              join app in EFContext.Set<TenantUserApplication>() on tenant.TenantId equals app.TenantId
                              where
                                  app.AppStatus == ApplicationStatus.OpenSuccess
                                  && !exceptTenants.Contains(tenant.TenantName)
                                  && tenant.TenantDisplayName.Contains(tenantDisplayName)
                                  && !tenant.IsDeleted
                              select tenant;

                    int recordCount = query.Count();
                    var data = query.OrderBy(m => m.TenantName).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                    return new Tuple<int, IList<TenantUser>>(recordCount, data);
                }
                else
                {
                    var query =
                        applicationId != Guid.Empty
                            ? from tenant in EFContext.Set<TenantUser>()
                              join app in EFContext.Set<TenantUserApplication>() on tenant.TenantId equals app.TenantId
                              where
                                  app.ApplicationId == applicationId
                                  && app.AppStatus == ApplicationStatus.OpenSuccess
                                  && tenant.TenantDisplayName.Contains(tenantDisplayName)
                                  && !tenant.IsDeleted
                              select tenant
                            : from tenant in EFContext.Set<TenantUser>()
                              join app in EFContext.Set<TenantUserApplication>() on tenant.TenantId equals app.TenantId
                              where
                                  app.AppStatus == ApplicationStatus.OpenSuccess
                                  && tenant.TenantDisplayName.Contains(tenantDisplayName)
                                  && !tenant.IsDeleted
                              select tenant;

                    int recordCount = query.Count();
                    var data = query.OrderBy(m => m.TenantName).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                    return new Tuple<int, IList<TenantUser>>(recordCount, data);
                }
            }
            else
            {
                if (exceptTenants != null)
                {

                    if (IsShowExceptTenants)
                    {
                        var query =
                            applicationId != Guid.Empty
                                ? EFContext.Set<TenantUser>()
                                    .Join(EFContext.Set<TenantUserApplication>(), tenant => tenant.TenantId,
                                        app => app.TenantId,
                                        (tenant, app) => new { tenant, app })
                                    .Where(@t => @t.app.ApplicationId == applicationId
                                                 && @t.app.AppStatus == ApplicationStatus.OpenSuccess
                                                 && exceptTenants.Contains(@t.tenant.TenantName))
                                    .Select(@t => @t.tenant)
                                : EFContext.Set<TenantUser>()
                                    .Join(EFContext.Set<TenantUserApplication>(), tenant => tenant.TenantId,
                                        app => app.TenantId,
                                        (tenant, app) => new { tenant, app })
                                    .Where(@t => @t.app.AppStatus == ApplicationStatus.OpenSuccess
                                                 && exceptTenants.Contains(@t.tenant.TenantName))
                                    .Select(@t => @t.tenant);
                        int recordCount = query.Count();
                        var data =
                            query.OrderBy(m => m.TenantName).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        return new Tuple<int, IList<TenantUser>>(recordCount, data);
                    }
                    else
                    {
                        var query =
                            applicationId != Guid.Empty
                                ? EFContext.Set<TenantUser>()
                                    .Join(EFContext.Set<TenantUserApplication>(), tenant => tenant.TenantId,
                                        app => app.TenantId,
                                        (tenant, app) => new { tenant, app })
                                    .Where(@t => @t.app.ApplicationId == applicationId
                                                 && @t.app.AppStatus == ApplicationStatus.OpenSuccess
                                                 && !exceptTenants.Contains(@t.tenant.TenantName))
                                    .Select(@t => @t.tenant)
                                : EFContext.Set<TenantUser>()
                                    .Join(EFContext.Set<TenantUserApplication>(), tenant => tenant.TenantId,
                                        app => app.TenantId,
                                        (tenant, app) => new { tenant, app })
                                    .Where(@t => @t.app.AppStatus == ApplicationStatus.OpenSuccess
                                                 && !exceptTenants.Contains(@t.tenant.TenantName))
                                    .Select(@t => @t.tenant);
                        int recordCount = query.Count();
                        var data =
                            query.OrderBy(m => m.TenantName).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        return new Tuple<int, IList<TenantUser>>(recordCount, data);
                    }
                }
                else
                {
                    var query =
                        applicationId != Guid.Empty
                            ? EFContext.Set<TenantUser>()
                                .Join(EFContext.Set<TenantUserApplication>(), tenant => tenant.TenantId,
                                    app => app.TenantId,
                                    (tenant, app) => new { tenant, app })
                                .Where(@t => @t.app.ApplicationId == applicationId
                                             && @t.app.AppStatus == ApplicationStatus.OpenSuccess)
                                .Select(@t => @t.tenant)
                            : EFContext.Set<TenantUser>()
                                .Join(EFContext.Set<TenantUserApplication>(), tenant => tenant.TenantId,
                                    app => app.TenantId,
                                    (tenant, app) => new { tenant, app })
                                .Where(@t => @t.app.AppStatus == ApplicationStatus.OpenSuccess)
                                .Select(@t => @t.tenant);

                    int recordCount = query.Count();
                    var data = query.OrderBy(m => m.TenantName).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                    return new Tuple<int, IList<TenantUser>>(recordCount, data);
                }
            }
        }

        public TenantUser GetDetailTenantById(int id)
        {
            return Entities
                .Include(o => o.Applications)
                //.Include(o => o.Applications.Select(m => m.ApplicationModules))
                .Include(o => o.OwnStorage)
                .Include(o => o.OwnDatabase)
                .AsNoTracking()
                .FirstOrDefault(m => m.TenantId == id);
        }
        public TenantUser GetDetailTenantByName(string tenantName)
        {
            //base.UnitOfWork.Context.Configuration.LazyLoadingEnabled = false;
            //base.UnitOfWork.Context.Configuration.ProxyCreationEnabled = false;
            var result = Entities
                .Include(o => o.OwnDatabase)
                .Include(o => o.Applications)
                .AsNoTracking()
                .FirstOrDefault(o => o.TenantName == tenantName);
            //base.UnitOfWork.Context.Configuration.LazyLoadingEnabled = true;
            //base.UnitOfWork.Context.Configuration.ProxyCreationEnabled = true;
            return result;
        }
        public TenantUser GetTenantUserByOpenAppId(Guid applicationId, string tenantName)
        {
            var query = from tenant in EFContext.Set<TenantUser>()
                        join app in EFContext.Set<TenantUserApplication>() on tenant.TenantId equals app.TenantId
                        where
                            app.ApplicationId == applicationId
                            && app.AppStatus == ApplicationStatus.OpenSuccess
                            && tenant.TenantName == tenantName
                        select tenant;

            return query.FirstOrDefault();
        }


        public async Task<TenantUser> GetTenantByNameAsync(string tenantName)
        {
            //base.UnitOfWork.Context.Configuration.LazyLoadingEnabled = false;
            //base.UnitOfWork.Context.Configuration.ProxyCreationEnabled = false;
            var result = await Entities
                .Include(o => o.OwnDatabase)
                .Include(o => o.Applications)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.TenantName != null && o.TenantName == tenantName);
            //base.UnitOfWork.Context.Configuration.LazyLoadingEnabled = true;
            //base.UnitOfWork.Context.Configuration.ProxyCreationEnabled = true;

            return result;
        }
        public async Task<TenantUser> GetTenantByNameOrNickNameAsync(string tenantName)
        {
            //base.UnitOfWork.Context.Configuration.LazyLoadingEnabled = false;
            //base.UnitOfWork.Context.Configuration.ProxyCreationEnabled = false;
            var result = await Entities
                .Include(o => o.OwnDatabase)
                .Include(o => o.Applications)
                .AsNoTracking()
                .SingleOrDefaultAsync(o => (o.TenantName != null && o.TenantName == tenantName)
                    || (o.NickName != null && o.NickName == tenantName));
            //base.UnitOfWork.Context.Configuration.LazyLoadingEnabled = true;
            //base.UnitOfWork.Context.Configuration.ProxyCreationEnabled = true;
            return result;
        }
        public async Task<TenantUser> GetTenantEndWithDomainName(string domainName)
        {
            //base.UnitOfWork.Context.Configuration.LazyLoadingEnabled = false;
            //base.UnitOfWork.Context.Configuration.ProxyCreationEnabled = false;
            var result = await Entities
                .Include(o => o.OwnDatabase)
                .Include(o => o.Applications)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => domainName.Contains(o.NickName));
            //base.UnitOfWork.Context.Configuration.LazyLoadingEnabled = true;
            //base.UnitOfWork.Context.Configuration.ProxyCreationEnabled = true;
            return result;
        }

        public string GetTenantDisplayNameByName(string name)
        {
            return Entities.FirstOrDefault(m => m.TenantName == name).TenantDisplayName;
        }
        public List<Tuple<string, string>> QueryTenantByKey(string key, int len = 10)
        {
            return
                Entities.AsNoTracking()
                    .Where(m => m.TenantDisplayName.Contains(key)
                                                || m.TenantName.StartsWith(key)
                                                || m.NickName.StartsWith(key))
                    .Take(len)
                    .Select(m => new { DisplayName = m.TenantDisplayName, TenantName = m.TenantName })
                    .AsEnumerable()
                    .Select(m => new Tuple<string, string>(m.DisplayName, m.TenantName))
                    .ToList();
        }

        public bool SaveTenantApplications(int tenantId, List<Guid> applicationIds)
        {
            var dbTenant = EFContext.Set<TenantUser>()
                    .Include(m => m.Applications)
                    .FirstOrDefault(m => m.TenantId == tenantId);

            if (dbTenant != null)
            {
                var dbAppIds = dbTenant.Applications.Select(m => m.ApplicationId).ToList();
                var addAppIds = applicationIds.Except(dbAppIds);
                var delAppIds = dbAppIds.Except(applicationIds);

                var addApplications = EFContext.Set<TenantUserApplication>()
                    .Where(m => addAppIds.Contains(m.ApplicationId)).ToList();

                var delApplications = dbTenant.Applications
                    .Where(m => delAppIds.Contains(m.ApplicationId)).ToList();

                addApplications.ForEach(m => dbTenant.Applications.Add(m));
                delApplications.ForEach(m => dbTenant.Applications.Remove(m));

                return UnitOfWork.Commit() > 0;
            }

            return true;
        }
    }
}
