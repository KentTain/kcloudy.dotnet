using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Web.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace KC.Web.Account
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
            services.Configure<IISOptions>(iis =>
            {
                iis.ForwardClientCertificate = false;
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);

            #endregion

            //KC.IdentityServer4 OIDC-Client authorize redirect loop: https://stackoverflow.com/questions/47100607/infinite-authentication-loop-when-using-KC.IdentityServer4-in-asp-net-core-2-0
            //var serviceDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(Service.Account.IAccountService));
            //services.Remove(serviceDescriptor);

            //方法一：Web客户端--认证及授权配置
            //StaticFactoryUtil.ConfigureIdentityServer4ClientService(
            //    services,
            //    new string[] { ApplicationConstant.OpenIdScope, ApplicationConstant.ProfileScope, ApplicationConstant.AccScope }, false);

            //方法二：Web客户端--认证及授权配置
            //StaticFactoryUtil.ConfigureIdentityServer4ClientServiceWithOptionsExtension(
            //    services, Configuration,
            //    new string[] { ApplicationConstant.OpenIdScope, ApplicationConstant.ProfileScope, ApplicationConstant.AccScope });

            //方法三：Web客户端--认证及授权配置，备注：参数scopes最后一个为Web项目的ApiScope（如：AccScope）
            var profiles = KC.Service.AutoMapper.AutoMapperConfiguration.GetAllProfiles()
                .Union(KC.Service.Account.AutoMapper.AutoMapperConfiguration.GetAllProfiles());
            StaticFactoryUtil.ConfigureIdentityServer4ClientServiceWithProviderExtension(
                services, Configuration,
                new string[] { ApplicationConstant.OpenIdScope, ApplicationConstant.ProfileScope, ApplicationConstant.AccScope },
                profiles, true);

            //Servicer & Repository 注入
            KC.Service.Account.DependencyInjectUtil.InjectService(services);

            var message = string.Format("Current DbConnect: {0}; Storage Connect: {1}; Queue Connect: {2}; ServiceBus Connect: {3}; NoSql Connect: {4}",
GlobalConfig.DatabaseConnectionString, GlobalConfig.StorageConnectionString, GlobalConfig.QueueConnectionString, GlobalConfig.ServiceBusConnectionString, GlobalConfig.NoSqlConnectionString);
            Service.Extension.ConsoleExtensions.ConsoleYellow(message);

            IMvcBuilder builder = services.AddRazorPages();
#if DEBUG
            builder.AddRazorRuntimeCompilation();
#endif
            //services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            StaticFactoryUtil.UseWebClientService(app, true, true);
        }
    }
}
