
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Reflection;

using KC.Framework.Base;

namespace KC.Web.Resource
{
    public class Startup
    {
        public const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
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

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
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

            #region 配置文件的读取及静态类GlobalConfig的设置
            services.Configure<GlobalConfig>(Configuration.GetSection("AppSettings"));
            services.Configure<UploadConfig>(Configuration.GetSection("UploadConfig"));

            GlobalConfig.InitGlobalConfig(Configuration);
            #endregion

            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                    builder => builder
                        //.WithOrigins("")
                        //.AllowCredentials()
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                );
            });

            services.AddControllersWithViews();
            IMvcBuilder builder = services.AddRazorPages();
//#if DEBUG
//            builder.AddRazorRuntimeCompilation();
//#endif
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            //使用Nginx反向代理时，添加下面的代码
            var forwardOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                RequireHeaderSymmetry = false
            };
            forwardOptions.KnownNetworks.Clear();
            forwardOptions.KnownProxies.Clear();
            app.UseForwardedHeaders(forwardOptions);
            app.UseDeveloperExceptionPage();

            //app.UseHttpsRedirection();
            //app.UseHsts();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    // using Microsoft.AspNetCore.Http;
                    ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
                }
            });
            app.UseCookiePolicy();
            app.UseRouting();

            //跨域访问
            app.UseCors(MyAllowSpecificOrigins);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapRazorPages();
                endpoints.MapDefaultControllerRoute();
                //endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

    }
}
