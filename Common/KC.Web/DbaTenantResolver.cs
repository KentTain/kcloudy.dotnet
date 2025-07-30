using KC.Framework.SecurityHelper;
using KC.Framework.Tenant;
using KC.Service.WebApiService.Business;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KC.Web
{
    public class DbaTenantResolver : SaasKit.Multitenancy.ITenantResolver<Tenant>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITenantUserApiService _tenantApiService;
        private readonly ILogger _logger;
        public DbaTenantResolver(
            IServiceProvider serviceProvider,
            ITenantUserApiService tenantApiService, 
            ILogger<TenantResolver> logger)
        {
            _serviceProvider = serviceProvider;
            _tenantApiService = tenantApiService;
            _logger = logger;
        }

        public async Task<SaasKit.Multitenancy.TenantContext<Tenant>> ResolveAsync(Microsoft.AspNetCore.Http.HttpContext context)
        {
            SaasKit.Multitenancy.TenantContext<Tenant> tenantContext = null;
            Tenant tenant = await _tenantApiService.GetTenantByName(TenantConstant.DbaTenantName); ; 
            if (tenant != null)
            {
                tenantContext = new SaasKit.Multitenancy.TenantContext<Tenant>(tenant);
            }
            else
            {
                tenantContext = new SaasKit.Multitenancy.TenantContext<Tenant>(TenantConstant.DbaTenantApiAccessInfo);
            }

            return tenantContext;
        }
    }
}
