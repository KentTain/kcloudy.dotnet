using KC.Framework.Tenant;
using KC.Web.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace KC.Web.Dict
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
            //    new string[] { ApplicationConstant.OpenIdScope, ApplicationConstant.ProfileScope, ApplicationConstant.AccScope }, false);

            //方法二：Web客户端--认证及授权配置
            //StaticFactoryUtil.ConfigureIdentityServer4ClientServiceWithOptionsExtension(
            //    services, Configuration,
            //    new string[] { ApplicationConstant.OpenIdScope, ApplicationConstant.ProfileScope, ApplicationConstant.AccScope });

            //方法三：Web客户端--认证及授权配置
            var profiles = KC.Service.AutoMapper.AutoMapperConfiguration.GetAllProfiles()
                .Union(KC.Service.Dict.AutoMapper.AutoMapperConfiguration.GetAllProfiles());
            StaticFactoryUtil.ConfigureIdentityServer4ClientServiceWithProviderExtension(
                services, Configuration,
                new string[] { ApplicationConstant.OpenIdScope, ApplicationConstant.ProfileScope, ApplicationConstant.CfgScope }, profiles, true);

            // Service的注入
            KC.Service.Dict.DependencyInjectUtil.InjectService(services);

            //services.AddHttpClient<IAccountService, AccountService>()
            //    .SetHandlerLifetime(TimeSpan.FromMinutes(TimeOutConstants.CacheShortTimeOut));
            //services.AddHttpClient<ISysManageService, SysManageService>()
            //    .SetHandlerLifetime(TimeSpan.FromMinutes(TimeOutConstants.CacheShortTimeOut));

            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            IMvcBuilder builder = services.AddRazorPages();
#if DEBUG
            builder.AddRazorRuntimeCompilation();
#endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            StaticFactoryUtil.UseWebClientService(app, true, true);
        }
    }
}
