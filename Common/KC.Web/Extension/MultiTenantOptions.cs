using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Web.Util;
using KC.Service.WebApiService.Business;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KC.Web.Extension
{
    public abstract class MultiTenantOptions<TOptions> : IConfigureNamedOptions<TOptions> where TOptions : AuthenticationSchemeOptions
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly ITenantUserApiService _tenantApiService;
        protected readonly IDataProtectionProvider _dataProtectionProvider;
        protected readonly ILogger _logger;

        public MultiTenantOptions(
            IHttpContextAccessor httpContextAccessor,
            IDataProtectionProvider dataProtectionProvider,
            ITenantUserApiService tenantApiService,
            ILogger logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _tenantApiService = tenantApiService;
            _dataProtectionProvider = dataProtectionProvider;
            _logger = logger;
        }

        public abstract void Configure(string name, TOptions options);

        public abstract void Configure(TOptions options);

        protected SaasKit.Multitenancy.TenantContext<Tenant> GetTenantContext()
        {
            return _httpContextAccessor.HttpContext.GetTenantContext<Tenant>();
        }
    }

    public class MultiTenantCookieOptions : MultiTenantOptions<CookieAuthenticationOptions>
    {
        private readonly HttpContext _httpContext;

        public MultiTenantCookieOptions(
            IHttpContextAccessor contextAccessor,
            IDataProtectionProvider dataProtectionProvider,
            ITenantUserApiService tenantApiService,
            ILogger<MultiTenantCookieOptions> logger)
            :base(contextAccessor, dataProtectionProvider, tenantApiService, logger)
        {
            _httpContext = contextAccessor.HttpContext;
        }

        public override void Configure(string name, CookieAuthenticationOptions options)
        {
            var tenantContext = GetTenantContext();
            _logger.LogDebug(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|MultiTenantCookieOptions with Tenant: " + tenantContext?.Tenant?.TenantName);
            if (tenantContext == null)
            {
                options.Cookie.Name = "AspNet.Cookies";
            }
            else
            {
                // Create a tenant-specific data protection provider to ensure cookie
                // can't be read/decrypted by the other tenants.
                options.DataProtectionProvider = _dataProtectionProvider.CreateProtector(tenantContext?.Tenant?.TenantName);

                options.Cookie.Name = $"{tenantContext.Tenant.TenantName}.AspNet.Cookies";
            }
        }

        public override void Configure(CookieAuthenticationOptions options)
            => Configure(Options.DefaultName, options);
    }
    public class MultiTenantOpenIdConnectOptions : MultiTenantOptions<OpenIdConnectOptions>
    {
        public MultiTenantOpenIdConnectOptions(
            IHttpContextAccessor contextAccessor,
            IDataProtectionProvider dataProtectionProvider,
            ITenantUserApiService tenantApiService,
            ILogger<MultiTenantOpenIdConnectOptions> logger)
            : base(contextAccessor, dataProtectionProvider, tenantApiService, logger)
        {
        }

        public override void Configure(string name, OpenIdConnectOptions options)
        {
            var tenantContext = GetTenantContext();

            _logger.LogDebug(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|MultiTenantOpenIdConnectOptions with Tenant: " + tenantContext?.Tenant?.TenantName);
            if (tenantContext != null && tenantContext.Tenant != null)
            {
                // Create a tenant-specific data protection provider to ensure cookie
                // can't be read/decrypted by the other tenants.
                options.DataProtectionProvider = _dataProtectionProvider.CreateProtector(tenantContext?.Tenant?.TenantName);

                StaticFactoryUtil.InitOpenIdConnectOptions(options, tenantContext.Tenant);
            }
        }

        public override void Configure(OpenIdConnectOptions options)
        {
            this.Configure(!string.IsNullOrEmpty(Options.DefaultName)
                    ? Options.DefaultName
                    : Web.Constants.OpenIdConnectConstants.ChallengeScheme,
                options);
        }
    }

    //public class MultiTenantGoogleOptions : IConfigureNamedOptions<GoogleOptions>
    //{
    //    private readonly HttpContext _httpContext;
    //    private readonly IList<Tenant> tenants;
    //    public GoogleOptions myCurrentOptions;

    //    public MultiTenantGoogleOptions(
    //        IHttpContextAccessor contextAccessor,
    //        IOptions<MultitenancyOptions> multitenancyOptions)
    //    {
    //        _httpContext = contextAccessor.HttpContext;
    //        tenants = multitenancyOptions.Value.Tenants;
    //    }

    //    public void Configure(string name, GoogleOptions options)
    //    {
    //        var tenantContext = _httpContext.GetTenantContext<Tenant>();

    //        if (tenantContext == null)
    //            throw new Exception("Google Auth not set.");

    //        var currentTenant = tenants.Single(t => t.Id == tenantContext.Tenant.Id);

    //        options.ClientId = currentTenant.GoogleClientId;
    //        options.ClientSecret = currentTenant.GoogleClientSecret;

    //        myCurrentOptions = options;
    //    }

    //    public void Configure(GoogleOptions options)
    //        => Configure(Options.DefaultName, options);
    //}
}
