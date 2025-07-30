using KC.Framework.Tenant;
using KC.Web.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using KC.Framework.Base;

namespace KC.Web.Portal
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            //增加环境配置文件，新建项目默认有
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
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
                .Union(KC.Service.Portal.AutoMapper.AutoMapperConfiguration.GetAllProfiles());
            StaticFactoryUtil.ConfigureIdentityServer4ClientServiceWithProviderExtension(
                services, Configuration,
                new string[] { ApplicationConstant.OpenIdScope, ApplicationConstant.ProfileScope, ApplicationConstant.PortalScope }, profiles, true);

            KC.Service.Portal.DependencyInjectUtil.InjectService(services);

            var message = string.Format("Current DbConnect: {0}; Storage Connect: {1}; Queue Connect: {2}; ServiceBus Connect: {3}; NoSql Connect: {4}",
GlobalConfig.DatabaseConnectionString, GlobalConfig.StorageConnectionString, GlobalConfig.QueueConnectionString, GlobalConfig.ServiceBusConnectionString, GlobalConfig.NoSqlConnectionString);
            Service.Extension.ConsoleExtensions.ConsoleYellow(message);

            IMvcBuilder builder = services.AddRazorPages();
#if DEBUG
            builder.AddRazorRuntimeCompilation();
#endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            StaticFactoryUtil.UseWebClientService(app, false, true);

            app.UseEndpoints(routes =>
            {
                routes.MapControllers();
                routes.MapRazorPages();
                routes.MapControllerRoute(
                  name: "areas",
                  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
                routes.MapDefaultControllerRoute();
            });
        }
    }
}
