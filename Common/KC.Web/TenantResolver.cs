using KC.Framework.Extension;
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
    public class TenantResolver : SaasKit.Multitenancy.ITenantResolver<Tenant>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITenantUserApiService _tenantApiService;
        private readonly ILogger _logger;
        public TenantResolver(
            IServiceProvider serviceProvider,
            ITenantUserApiService tenantApiService, 
            ILogger<TenantResolver> logger)
        {
            _serviceProvider = serviceProvider;
            _tenantApiService = tenantApiService;
            _logger = logger;
        }

        public static IEnumerable<Tenant> TenantsForTest = new List<Tenant>(new[]
        {
            TenantConstant.DbaTenantApiAccessInfo,
            TenantConstant.TestTenantApiAccessInfo,
            TenantConstant.BuyTenantApiAccessInfo,
            TenantConstant.SaleTenantApiAccessInfo
        });

        public async Task<SaasKit.Multitenancy.TenantContext<Tenant>> ResolveAsync(Microsoft.AspNetCore.Http.HttpContext context)
        {
            SaasKit.Multitenancy.TenantContext<Tenant> tenantContext = null;

            Tenant tenant = null;
            var requestHost = context.Request.Host.Value;
            var tenantName = requestHost.GetTenantNameByHost();
            if (string.IsNullOrEmpty(tenantName))
            {
                tenant = await _tenantApiService.GetTenantByName(TenantConstant.DbaTenantName);
            }
            else
            {
                tenant = await _tenantApiService.GetTenantByName(tenantName);
            }

            _logger.LogDebug(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|KC.Web.TenantResolver Get tenant【{0}】 from url【{1}】", tenant?.TenantName, context.Request.Scheme + "://" +  context.Request.Host.Value + context.Request.Path));

            if (tenant != null)
            {
                tenantContext = new SaasKit.Multitenancy.TenantContext<Tenant>(tenant);
            }

            return tenantContext;
        }
    }
}
