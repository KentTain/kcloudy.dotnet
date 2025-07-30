using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Web.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace KC.WebApi.Doc
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

            #endregion

            //WebApi客户端：GlobalConfig对象的设置、认证及授权配置
            var profiles = KC.Service.AutoMapper.AutoMapperConfiguration.GetAllProfiles()
                .Union(KC.Service.Doc.AutoMapper.AutoMapperConfiguration.GetAllProfiles());
            StaticFactoryUtil.ConfigureApiClientService(services, Configuration,  ApplicationConstant.DocScope, profiles);

            // Service的注入
            KC.Service.Doc.DependencyInjectUtil.InjectService(services);

            var message = string.Format("Current DbConnect: {0}; Storage Connect: {1}; Queue Connect: {2}; ServiceBus Connect: {3}; NoSql Connect: {4}",
GlobalConfig.DatabaseConnectionString, GlobalConfig.StorageConnectionString, GlobalConfig.QueueConnectionString, GlobalConfig.ServiceBusConnectionString, GlobalConfig.NoSqlConnectionString);
            Service.Extension.ConsoleExtensions.ConsoleYellow(message);

            //services.AddTransient<ITenantUserService, TenantUserService>();
            //services.AddHttpClient<ITenantUserService, TenantUserService>()
            //    .SetHandlerLifetime(TimeSpan.FromMinutes(TimeOutConstants.CacheShortTimeOut));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            StaticFactoryUtil.UseApiClientService(app);
        }
    }
}
