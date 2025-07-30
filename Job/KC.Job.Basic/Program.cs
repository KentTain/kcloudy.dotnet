using System;
using System.Collections.Generic;
using KC.DataAccess.Job;
using KC.Framework.Tenant;
using KC.Job.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace KC.Job.Basic
{
    public class Program
    {
        private static bool runServices = true;
        public static IServiceProvider Services { get; set; }

        public static void Main(string[] args)
        {
            try
            {
                var host = CreateHostBuilder(args).Build();

                Web.Util.StaticFactoryUtil.InitWeb();

                #region 初始化数据库及数据：KC.DataAccess.Job
                using (var scope = host.Services.CreateScope())
                {
                    var tenants = new List<Tenant>() {
                        TenantConstant.DbaTenantApiAccessInfo,
                        TenantConstant.TestTenantApiAccessInfo,
                    };
                    var dataInitializer = new ComJobDatabaseInitializer();

                    Console.WriteLine("--begin to Initialize the tenant--dba.");
                    dataInitializer.Initialize(tenants);
                    Console.WriteLine("--end to Initialize the tenant--dba.");

                    //Console.WriteLine("--begin to Initialize the tenant--dba.");
                    //dataInitializer.SeedData(tenants);
                    //Console.WriteLine("--end to Initialize the tenant--dba.");
                    
                }
                #endregion

                #region 初始化Job
                Console.WriteLine("Start on the KC.Job.Basic project..........................");
                var jobManager = AbstractJobInitializer.Initialize(new JobInitializer(host.Services), out runServices);
                if (runServices)
                {
                    jobManager.OnStart();
                }
                Console.WriteLine("End on the KC.Job.Basic project..........................");
                #endregion

                #region 运行Job
                Console.WriteLine("Entered KC.Job.Basic Run Method");
                if (runServices)
                {
                    System.Console.WriteLine("KC.Job.Basic Services Launched");
                    jobManager.Run();
                }
                else
                {
                    Console.WriteLine("Please do not run job in development environment against NuPitch production environment");
                }
                #endregion

                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Stopped program because of exception, Error Message: {0}, Error Stack Trace: {1}",
                    ex.Message, ex.StackTrace));
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                //正常的获得DbContext
                .UseDefaultServiceProvider(options =>
                {
                    options.ValidateScopes = false;
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        // https://github.com/aspnet/KestrelHttpServer/issues/475
                        options.Limits.MaxRequestHeaderCount = 200;
                        options.Limits.MaxRequestHeadersTotalSize = 65536; //64KB
                    })
                    .UseUrls("http://*:9009"/*, "https://*:9019"*/)
                    .UseIISIntegration()
                    .UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                // Replace the default IOC container with aspectcore
                .UseServiceProviderFactory(new AspectCore.Extensions.DependencyInjection.DynamicProxyServiceProviderFactory())
                // NLog: setup NLog for Dependency injection;
                .UseNLog();
        }
    }
}
