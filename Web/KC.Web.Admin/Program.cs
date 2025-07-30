using System;
using KC.Framework.Tenant;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace KC.Web.Admin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var host = CreateHostBuilder(args).Build();

                Web.Util.StaticFactoryUtil.InitWeb();

                #region 初始化数据库及数据：KC.DataAccess.Admin
                var dataInitializer = new DataAccess.Admin.ComAdminDatabaseInitializer();

                Console.WriteLine("--begin to Initialize the tenant--KC.Web.Admin.");
                dataInitializer.Initialize(TenantConstant.DbaTenantApiAccessInfo);
                Console.WriteLine("--end to Initialize the tenant--KC.Web.Admin.");

                //Console.WriteLine("--begin to Seed the tenant--KC.Web.Admin.");
                //dataInitializer.SeedData(TenantConstant.DbaTenantApiAccessInfo);
                //Console.WriteLine("--end to Seed the tenant--KC.Web.Admin.");
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
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        // https://github.com/aspnet/KestrelHttpServer/issues/475
                        options.Limits.MaxRequestHeaderCount = 200;
                        options.Limits.MaxRequestHeadersTotalSize = 65536; //64KB
                    })
                    .UseUrls("http://*:1003"/*, "https://*:1013"*/)
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
