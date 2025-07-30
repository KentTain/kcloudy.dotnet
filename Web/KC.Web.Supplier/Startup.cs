using KC.Framework.Tenant;
using KC.Web.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace KC.Web.Supplier
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            var envFile = $"appsettings.{env.EnvironmentName}.json";
            if (env.EnvironmentName.Equals("Production", System.StringComparison.OrdinalIgnoreCase))
            {
                envFile = "appsettings.json";
            }
            //增加环境配置文件，新建项目默认有
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile(envFile, optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            Environment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region 相关服务器的配置
            //Hosting in IIS
            services.Configure<Microsoft.AspNetCore.Builder.IISOptions>(iis =>
            {
                iis.ForwardClientCertificate = false;
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            #endregion

            //方法一：Web客户端--认证及授权配置
            //StaticFactoryUtil.ConfigureIdentityServer4ClientService(
            //    services,
            //    new string[] { "openid", "profile", "accapi" }, false);

            //方法二：Web客户端--认证及授权配置
            //StaticFactoryUtil.ConfigureIdentityServer4ClientServiceWithOptionsExtension(
            //    services, Configuration,
            //    new string[] { "openid", "profile", "accapi" });

            //方法三：Web客户端--认证及授权配置
            var profiles = KC.Service.AutoMapper.AutoMapperConfiguration.GetAllProfiles()
                .Union(KC.Service.Supplier.AutoMapper.AutoMapperConfiguration.GetAllProfiles());
            StaticFactoryUtil.ConfigureIdentityServer4ClientServiceWithProviderExtension(
                services, Configuration,
                new string[] { "openid", "profile", "srmapi" }, profiles, true);


            services.AddSingleton<ITenantService, TenantService>(_ =>
              new TenantService(TenantConstant.BuyTenantApiAccessInfo));

            #region Service的注入

            //KC.Service.Account.DependencyInjectUtil.InjectService(services);
            
            //services
            //    .AddScoped<IMigrationsSqlGenerator, ComAccountMigrationsSqlGenerator>()
            //    .AddScoped<IModelCacheKeyFactory, ComAccountModelCacheKeyFactory>()
            //    .AddDbContext<ComAccountContext>(options => options.UseSqlServer(GlobalConfig.GetDecryptDatabaseConnectionString()));

            //services.AddDbContext<ComAccountContext>(options =>
            //    options.UseSqlServer(GlobalConfig.GetDecryptDatabaseConnectionString()));

            //services.AddScoped<IUserRepository, UserRepository>();
            //services.AddScoped<IRoleRepository, AspNetRoleRepository>();
            //services.AddScoped<IPermissionRepository, PermissionRepository>();
            //services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            //services.AddScoped<IMenuNodeRepository, MenuNodeRepository>();

            //services.AddTransient<IAccountService, AccountService>();
            //services.AddTransient<ISysManageService, SysManageService>();


            //services.AddHttpClient<IAccountService, AccountService>()
            //    .SetHandlerLifetime(TimeSpan.FromMinutes(TimeOutConstants.CacheShortTimeOut));
            //services.AddHttpClient<ISysManageService, SysManageService>()
            //    .SetHandlerLifetime(TimeSpan.FromMinutes(TimeOutConstants.CacheShortTimeOut));
            #endregion

            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            StaticFactoryUtil.UseWebClientService(app, true, true);
        }
    }

    public interface ITenantService
    {
        Tenant GetCurrentTenant();
    }


    public class TenantService : ITenantService
    {
        private readonly Tenant _tenant;
        public TenantService(string tenant)
        {
            //_tenant = tenant;
        }
        public TenantService(IServiceProvider tenant)
        {
            // _tenant = tenant;
        }
        public TenantService(Tenant tenant)
        {
            _tenant = tenant;
        }
        public Tenant GetCurrentTenant()
        {
            return _tenant;
        }
    }
}
