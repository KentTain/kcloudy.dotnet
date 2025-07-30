using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.IdentityServer4.Models;
using KC.IdentityServer4.Stores;
using KC.Framework.Tenant;
using KC.Service.Admin;
using Microsoft.Extensions.Logging;
using KC.Framework.Extension;
using Microsoft.AspNetCore.Http;

namespace KC.Web.SSO.Services
{
    /// <summary>
    /// https://damienbod.com/2017/12/30/using-an-ef-core-database-for-the-KC.IdentityServer4-configuration-data/
    /// </summary>
    public class CustomResourceStore : IResourceStore
    {
        private readonly ITenantUserService _tenantUserService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;

        public CustomResourceStore(
            ITenantUserService tenantService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CustomResourceStore> logger)
        {
            _tenantUserService = tenantService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<ApiResource> FindApiResourceAsync(string clientId)
        {
            _logger.LogInformation(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|CustomResourceStore FindApiResourceAsync: " + clientId);

            var tenantName = TenantConstant.GetDecodeClientId(clientId);
            var tenant = await _tenantUserService.GetTenantByNameOrNickNameAsync(tenantName);
            var apiResource = tenant.Scopes.FirstOrDefault();
            return await Task.FromResult(new ApiResource(apiResource.Key, apiResource.Value));
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames == null) throw new ArgumentNullException(nameof(scopeNames));

            //_logger.LogDebug(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|CustomResourceStore FindApiResourcesByScopeAsync: " + scopeNames.ToCommaSeparatedString());

            //var apiResources = await GetCurrentTenantApiResource();
            var apiResources = GetApiResources();

            return await Task.FromResult(apiResources.AsEnumerable());
        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames == null) throw new ArgumentNullException(nameof(scopeNames));

            //_logger.LogDebug("------CustomResourceStore FindIdentityResourcesByScopeAsync: " + scopeNames.ToCommaSeparatedString());

            var identityResources = new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResources.Phone()
            };

            return Task.FromResult(identityResources.AsEnumerable());
        }

        public async Task<Resources> GetAllResourcesAsync()
        {
            var identityResources = new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResources.Phone()
            };

            //var apiResources = await GetCurrentTenantApiResource();
            var apiResources = GetApiResources();

            var result = new Resources(identityResources, apiResources);
            return await Task.FromResult(result);
        }

        private async Task<List<ApiResource>> GetCurrentTenantApiResource()
        {
            SaasKit.Multitenancy.TenantContext<Tenant> tenantContext = _httpContextAccessor.HttpContext.GetTenantContext<Tenant>();
            if (tenantContext == null)
            {
                _logger.LogError("No tenant context found in CustomResourceStore");
                throw new ArgumentNullException(nameof(tenantContext));
            }

            var tenantName = tenantContext.Tenant.TenantName;
            var tenant = await _tenantUserService.GetTenantByNameOrNickNameAsync(tenantName);
            var apiResources = new List<ApiResource>();
            foreach (var apiResource in tenant.Scopes)
            {
                apiResources.Add(new ApiResource(apiResource.Key, apiResource.Value));
            }

            return apiResources;
        }

        private  IEnumerable<ApiResource> GetApiResources()
        {
            // TODO: 需要改成从数据库中读取数据
            return new List<ApiResource>
            {
                new ApiResource(ApplicationConstant.SsoScope, ApplicationConstant.SsoAppName),
                new ApiResource(ApplicationConstant.AdminScope, ApplicationConstant.AdminAppName),
                new ApiResource(ApplicationConstant.BlogScope, ApplicationConstant.BlogAppName),

                new ApiResource(ApplicationConstant.AppScope, ApplicationConstant.AppAppName),
                new ApiResource(ApplicationConstant.CfgScope, ApplicationConstant.ConfigAppName),
                new ApiResource(ApplicationConstant.DicScope, ApplicationConstant.DictAppName),
                new ApiResource(ApplicationConstant.MsgScope, ApplicationConstant.MsgAppName),

                new ApiResource(ApplicationConstant.AccScope, ApplicationConstant.AccAppName),
                new ApiResource(ApplicationConstant.EconScope, ApplicationConstant.EconAppName),
                new ApiResource(ApplicationConstant.DocScope, ApplicationConstant.DocAppName),
                new ApiResource(ApplicationConstant.HrScope, ApplicationConstant.HrAppName),

                new ApiResource(ApplicationConstant.CrmScope, ApplicationConstant.CrmAppName),
                new ApiResource(ApplicationConstant.SrmScope, ApplicationConstant.SrmAppName),
                new ApiResource(ApplicationConstant.PrdScope, ApplicationConstant.PrdAppName),
                new ApiResource(ApplicationConstant.PmcScope, ApplicationConstant.PmcAppName),

                new ApiResource(ApplicationConstant.PortalScope, ApplicationConstant.PortalAppName),
                new ApiResource(ApplicationConstant.SomScope, ApplicationConstant.SomAppName),
                new ApiResource(ApplicationConstant.PomScope, ApplicationConstant.PomAppName),
                new ApiResource(ApplicationConstant.WmsScope, ApplicationConstant.WmsAppName),

                new ApiResource(ApplicationConstant.JrScope, ApplicationConstant.JrAppName),
                new ApiResource(ApplicationConstant.TrainScope, ApplicationConstant.TrainAppName),

                new ApiResource(ApplicationConstant.WorkflowScope, ApplicationConstant.WorkflowAppName),
                new ApiResource(ApplicationConstant.PayScope, ApplicationConstant.PayAppName),
                new ApiResource(ApplicationConstant.WXScope, ApplicationConstant.WXAppName),

            };
        }
    }
}
