using KC.IdentityServer4.Services;
using KC.Framework.Tenant;
using KC.Service.Admin;
using KC.Service.WebApiService.Business;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Extension;

namespace KC.Web.SSO
{
    public class TenantResolver : SaasKit.Multitenancy.ITenantResolver<Tenant>
    {
        private readonly IApplicationApiService _appApiService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly ILogger<TenantResolver> _logger;
        public TenantResolver(
            IApplicationApiService appApiService,
            IServiceProvider serviceProvider,
            IIdentityServerInteractionService interaction,
            ILogger<TenantResolver> logger)
        {
            _appApiService = appApiService;
            _serviceProvider = serviceProvider;
            _interaction = interaction;
            _logger = logger;
        }

        public async Task<SaasKit.Multitenancy.TenantContext<Tenant>> ResolveAsync(Microsoft.AspNetCore.Http.HttpContext context)
        {
            SaasKit.Multitenancy.TenantContext<Tenant> tenantContext = null;

            Tenant tenant = null;
            var query = context.Request.Query;
            if (query.ContainsKey(TenantConstant.ClaimTypes_TenantName))
            {
                var tenantName = query[TenantConstant.ClaimTypes_TenantName];
                tenant = await GetTenantWithHosts(tenantName);

                _logger.LogDebug(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|1. KC.Web.SSO.TenantResolver Get tenant: 【{0}-{1}】 from query tenantName: {2} with url: 【{3}】",
                    tenant?.TenantName, tenant?.TenantDisplayName, tenantName, context.Request.Scheme + "://" + context.Request.Host.Value + context.Request.Path));

                if (tenant != null)
                {
                    tenantContext = new SaasKit.Multitenancy.TenantContext<Tenant>(tenant);
                }
            }
            else if (query.ContainsKey("returnUrl"))
            {
                var returnUrl = query["returnUrl"];
                var tenantName = await GetTenantNameByReturnUrl(returnUrl);
                if (!string.IsNullOrEmpty(tenantName))
                {
                    tenant = await GetTenantWithHosts(tenantName);

                    _logger.LogDebug(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|2. KC.Web.SSO.TenantResolver Get tenant: 【{0}-{1}】 from returnUrl: 【{2}】 with url: 【{3}】",
                        tenant?.TenantName, tenant?.TenantDisplayName, returnUrl, context.Request.Scheme + "://" + context.Request.Host.Value + context.Request.Path));

                    if (tenant != null)
                    {
                        tenantContext = new SaasKit.Multitenancy.TenantContext<Tenant>(tenant);
                    }
                }
            }

            if (tenant == null)
            {
                var requestHost = context.Request.Host.Value;
                var tenantName = requestHost.GetTenantNameByHost();
                if (string.IsNullOrEmpty(tenantName))
                {
                    tenant = await GetTenantWithHosts(TenantConstant.DbaTenantName);
                }
                else
                {
                    tenant = await GetTenantWithHosts(tenantName);
                }

                _logger.LogDebug(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|3. KC.Web.SSO.TenantResolver Get default tenant: 【{0}-{1}】 from Url: 【{2}】",
                    tenant?.TenantName, tenant?.TenantDisplayName, context.Request.Scheme + "://" + context.Request.Host.Value + context.Request.Path));

                if (tenant != null)
                {
                    tenantContext = new SaasKit.Multitenancy.TenantContext<Tenant>(tenant);
                }
            }

            return tenantContext;
        }

        private async Task<Tenant> GetTenantWithHosts(string tenantName)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                try {
                    var _tenantService = scope.ServiceProvider.GetRequiredService<ITenantUserService>();
                    var tenant = await _tenantService.GetTenantByNameOrNickNameAsync(tenantName);
                    return tenant;
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                    return null;
                }
            }
        }

        private async Task<string> GetTenantNameByReturnUrl(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
                return string.Empty;

            try
            {
                var authUrl = returnUrl;
                var tenantName = string.Empty;
                if(returnUrl.StartsWith("http://") || returnUrl.StartsWith("https://"))
                {
                    var uri = new Uri(returnUrl);
                    if (uri != null && !string.IsNullOrEmpty(uri.PathAndQuery))
                        authUrl = uri.PathAndQuery;
                }

                var context = await _interaction.GetAuthorizationContextAsync(authUrl);
                tenantName = context?.Tenant;
                if (string.IsNullOrEmpty(tenantName))
                    tenantName = context?.Parameters[TenantConstant.ClaimTypes_TenantName];

                _logger.LogDebug(string.Format(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|Parser returnUrl【{0}】 and get tenantName: {1}", returnUrl, tenantName));

                return tenantName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "解析ReturnUrl出错：" + returnUrl);
                return string.Empty;
            }
        }
    }
}
