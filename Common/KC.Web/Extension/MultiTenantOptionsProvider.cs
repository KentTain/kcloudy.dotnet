using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Web.Util;
using KC.Service.WebApiService.Business;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Extension;

namespace KC.Web.Extension
{
    public abstract class MultiTenantOptionsProvider<TOptions> : IOptionsMonitor<TOptions> where TOptions : class
    {
        protected readonly ConcurrentDictionary<(string name, string tenant), Lazy<TOptions>> _cache;
        protected readonly IDataProtectionProvider _dataProtectionProvider;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IEnumerable<IConfigureOptions<TOptions>> _setups;
        protected readonly IEnumerable<IPostConfigureOptions<TOptions>> _postConfigures;
        protected readonly ITenantUserApiService _tenantApiService;
        protected readonly ILogger _logger;

        public MultiTenantOptionsProvider(
            IDataProtectionProvider dataProtectionProvider,
            IHttpContextAccessor httpContextAccessor,
            IEnumerable<IConfigureOptions<TOptions>> setups,
            IEnumerable<IPostConfigureOptions<TOptions>> postConfigures,
            ITenantUserApiService tenantApiService,
            ILogger logger)
        {
            _cache = new ConcurrentDictionary<(string name, string tenant), Lazy<TOptions>>();
            _dataProtectionProvider = dataProtectionProvider;
            _httpContextAccessor = httpContextAccessor;
            _setups = setups;
            _postConfigures = postConfigures;
            _tenantApiService = tenantApiService;
            _logger = logger;
        }

        public virtual TOptions CurrentValue => Get(Options.DefaultName);

        public virtual TOptions Get(string name)
        {
            // This sample uses the path base as the tenant.
            // You can replace that by your own logic.
            var tenantContext = GetTenantContext();
            if (tenantContext == null)
                throw new Exception("No tenant context found");

            string tenant = tenantContext.Tenant.TenantName;

            return _cache.GetOrAdd((name, tenant), _ => new Lazy<TOptions>(() =>
            {
                var options = Activator.CreateInstance<TOptions>();
                if (options is CookieAuthenticationOptions)
                {
                    var cookieOptions = options as CookieAuthenticationOptions;
                    if (!tenant.IsNullOrEmpty())
                    {
                        cookieOptions.Cookie.Name = $"{tenant}.AspNet.Cookies";
                    }
                    else
                    {
                        cookieOptions.Cookie.Name = "AspNet.Cookies";
                    }
                }
                return Create(options, name, tenant);
            })).Value;
        }

        protected virtual TOptions Create(TOptions options, string name, string tenant)
        {
            // Run the other initializers (which is normally done by OptionsFactory when
            // using the default OptionsMonitor implementation provided by ASP.NET Core).
            foreach (var setup in _setups)
            {
                if (setup is IConfigureNamedOptions<TOptions> namedSetup)
                {
                    namedSetup.Configure(name, options);
                }
                else if (name == Options.DefaultName)
                {
                    setup.Configure(options);
                }
            }
            foreach (var post in _postConfigures)
            {
                post.PostConfigure(name, options);
            }

            return options;
        }

        protected SaasKit.Multitenancy.TenantContext<Tenant> GetTenantContext()
        {
            return _httpContextAccessor.HttpContext.GetTenantContext<Tenant>();
        }

        public IDisposable OnChange(Action<TOptions, string> listener) => null;
    }

    public class MultiTenantCookieOptionsProvider : MultiTenantOptionsProvider<CookieAuthenticationOptions>
    {
        private readonly HttpContext _httpContext;
        public MultiTenantCookieOptionsProvider(
            ITenantUserApiService tenantApiService,
            IDataProtectionProvider dataProtectionProvider,
            IHttpContextAccessor contextAccessor,
            IEnumerable<IConfigureOptions<CookieAuthenticationOptions>> setups,
            IEnumerable<IPostConfigureOptions<CookieAuthenticationOptions>> postConfigures,
            ILogger<MultiTenantCookieOptionsProvider> logger)
            : base(dataProtectionProvider, contextAccessor, setups, postConfigures, tenantApiService, logger)
        {
            _httpContext = contextAccessor.HttpContext;
        }

        protected override CookieAuthenticationOptions Create(CookieAuthenticationOptions options, string name, string tenant)
        {
            // Create a tenant-specific data protection provider to ensure cookie
            // can't be read/decrypted by the other tenants.
            options.DataProtectionProvider = _dataProtectionProvider.CreateProtector(tenant);

            var tenantContext = GetTenantContext();

            _logger.LogDebug(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|MultiTenantCookieOptionsProvider with Tenant: " + tenantContext?.Tenant?.TenantName);

            return base.Create(options, name, tenant);
        }
    }

    public class MultiTenantOpenIdConnectOptionsProvider : MultiTenantOptionsProvider<OpenIdConnectOptions>
    {
        private readonly HttpContext _httpContext;
        private readonly GlobalConfig _config;
        public MultiTenantOpenIdConnectOptionsProvider(
            ITenantUserApiService tenantApiService,
            IDataProtectionProvider dataProtectionProvider,
            IHttpContextAccessor contextAccessor,
            IEnumerable<IConfigureOptions<OpenIdConnectOptions>> setups,
            IEnumerable<IPostConfigureOptions<OpenIdConnectOptions>> postConfigures,
            ILogger<MultiTenantCookieOptionsProvider> logger,
            IOptions<GlobalConfig> options)
            : base(dataProtectionProvider, contextAccessor, setups, postConfigures, tenantApiService, logger)
        {
            _config = options.Value;
            _httpContext = contextAccessor.HttpContext;
        }

        protected override OpenIdConnectOptions Create(OpenIdConnectOptions options, string name, string tenant)
        {
            var tenantContext = GetTenantContext();

            var defaultTenant = TenantConstant.TestTenantApiAccessInfo;
            _logger.LogDebug(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|MultiTenantOpenIdConnectOptionsProvider with Tenant: " + tenantContext?.Tenant?.TenantName);
            if (tenantContext != null && tenantContext.Tenant != null)
            {
                defaultTenant = tenantContext.Tenant;
            }
            else
            {
                // You can populate the "options" instance using data
                // associated with the tenant (e.g stored in a database).
                defaultTenant = _tenantApiService.GetTenantByName(tenant).Result;
            }

            // Create a tenant-specific data protection provider to ensure cookie
            // can't be read/decrypted by the other tenants.
            options.DataProtectionProvider = _dataProtectionProvider.CreateProtector(tenant);

            StaticFactoryUtil.InitOpenIdConnectOptions(options, tenantContext.Tenant);

            return base.Create(options, name, tenant);
        }
    }

    //public class MultiTenantGoogleOptionsProvider : MultiTenantOptionsProvider<GoogleOptions>
    //{
    //    public MultiTenantGoogleOptionsProvider(
    //        ITenantUserApiService tenantApiService,
    //        IDataProtectionProvider dataProtectionProvider,
    //        IHttpContextAccessor httpContextAccessor,
    //        IEnumerable<IConfigureOptions<GoogleOptions>> setups,
    //        IEnumerable<IPostConfigureOptions<GoogleOptions>> postConfigures,
    //        ILogger<MultiTenantCookieOptionsProvider> logger) 
    //        : base(dataProtectionProvider, httpContextAccessor, setups, postConfigures, tenantApiService, logger)
    //    {
    //    }

    //    protected override GoogleOptions Create(GoogleOptions options, string name, string tenant)
    //    {
    //        // Create a tenant-specific data protection provider to ensure authorization codes,
    //        // access tokens and refresh tokens can't be read/decrypted by the other tenants.
    //        options.DataProtectionProvider = _dataProtectionProvider.CreateProtector(tenant);

    //        // You can populate the "options" instance using data
    //        // associated with the tenant (e.g stored in a database).
    //        var currentTenant = _tenantApiService.GetTenantByName(tenant).Result;

    //        options.ClientId = currentTenant.TenantName;
    //        options.ClientSecret = currentTenant.TenantSignature;

    //        return base.Create(options, name, tenant);
    //    }
    //}
}
