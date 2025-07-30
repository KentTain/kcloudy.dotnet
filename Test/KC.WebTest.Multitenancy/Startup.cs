using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Autofac.Multitenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sandbox;

namespace Com.WebTest.Multitenancy
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public static MultitenantContainer ApplicationContainer { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMultitenancy<AppTenant, AppTenantResolver>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            
            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterType<MessageService>()
                .As<IMessageService>()
                .WithProperty("Id", "base")
                .InstancePerLifetimeScope();
            var container = builder.Build();
            var strategy = new QueryStringTenantIdentificationStrategy(container.Resolve<IHttpContextAccessor>(), container.Resolve<ILogger<QueryStringTenantIdentificationStrategy>>());
            var mtc = new MultitenantContainer(strategy, container);
            mtc.ConfigureTenant(
                "Tenant 1",
                cb => cb
                        .RegisterType<MessageService>()
                        .As<IMessageService>()
                        .WithProperty("Id", "Tenant 1:aaaaaaaaaaaaaaaaaaaaaaaa")
                        .InstancePerLifetimeScope());
            mtc.ConfigureTenant(
                "Tenant 2",
                cb => cb
                        .RegisterType<MessageService>()
                        .As<IMessageService>()
                        .WithProperty("Id", "Tenant 2:bbbbbbbbbbbbbbbbbbbbbbb")
                        .InstancePerLifetimeScope());
            Startup.ApplicationContainer = mtc;
            return new AutofacServiceProvider(mtc);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Debug);

            app.UseMultitenancy<AppTenant>();
            //app.UseTenantContainers<AppTenant>();

            app.UseMiddleware<LogTenantMiddleware>();

            app.UseMvc();
        }
    }

    public class LogTenantMiddleware
    {
        RequestDelegate next;
        private readonly ILogger log;

        public LogTenantMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            this.next = next;
            this.log = loggerFactory.CreateLogger<LogTenantMiddleware>();
        }

        public async Task Invoke(HttpContext context, IMessageService messageService)
        {
            var tenant = context.GetTenant<AppTenant>();

            if (tenant != null)
            {
                await context.Response.WriteAsync(
                    messageService.Format($"Tenant \"{tenant.Name}\"."));
            }
            else
            {
                await context.Response.WriteAsync("No matching tenant found.");
            }
        }
    }
}
