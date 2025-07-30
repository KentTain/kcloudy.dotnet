using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using KC.Model.Admin;

namespace KC.DataAccess.Admin.Repository
{
    public interface ITenantUserRepository : Database.IRepository.IDbRepository<TenantUser>
    {
        bool ExistTenantName(string tenantName);

        IList<TenantUser> FindAllDetailTenantUsers();
        IList<TenantUser> FindAllDetailTenantUsersByNames(List<string> tenantNames);
        IList<TenantUser> FindOpenAppsTenantUsersByIds(List<int> ids);
        IList<TenantUser> FindOpenAppTenantsByNames(Guid appId, List<string> tenantNames);
        IList<TenantUser> FindAllTenantUsersByTally(Guid appId, TenantType? tally);
        IList<TenantUser> FindAllOpenAppTenantsWithoutTestTenant(Guid appId, string currentTenantName, TenantType? tally = null);
        IList<TenantUser> FindAllTenantsWithoutTests(Guid appId, string exceptTenantName);
        
        Tuple<int, IList<TenantUser>> FindPagenatedTenantsByApplicationId(int pageIndex, int pageSize, Guid applicationId, string tenantDisplayName, List<string> exceptTenants = null, bool IsShowExceptTenants = false);

        TenantUser GetDetailTenantById(int id);
        TenantUser GetDetailTenantByName(string tenantName);
        TenantUser GetTenantUserByOpenAppId(Guid applicationId, string tenantName);

        
        Task<TenantUser> GetTenantByNameAsync(string tenantName);
        Task<TenantUser> GetTenantByNameOrNickNameAsync(string tenantName);
        Task<TenantUser> GetTenantEndWithDomainName(string domainName);

        string GetTenantDisplayNameByName(string tenantName);
        List<Tuple<string, string>> QueryTenantByKey(string key, int len = 10);

        bool SaveTenantApplications(int tenantId, List<Guid> applicationIds);
    }
}