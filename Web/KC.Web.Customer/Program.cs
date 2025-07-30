using System;
using System.Collections.Generic;
using KC.Framework.Tenant;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace KC.Web.Customer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var host = CreateHostBuilder(args).Build();

                Web.Util.StaticFactoryUtil.InitWeb();

                using (var scope = host.Services.CreateScope())
                {
                    //var context = scope.ServiceProvider.GetRequiredService<DataAccess.Account.ComAccountContext>();
                    //context.Database.EnsureCreated();
                    //context.Database.EnsureDeleted();
                    //context.Database.Migrate();

                    #region 初始化数据库及数据：KC.DataAccess.Customer

                    var tenants = new List<Tenant>() {
                        //TenantConstant.DbaTenantApiAccessInfo,
                        TenantConstant.TestTenantApiAccessInfo,
                        TenantConstant.BuyTenantApiAccessInfo,
                        TenantConstant.SaleTenantApiAccessInfo,
                    };
                    var dataInitializer = new DataAccess.Customer.ComCustomerDatabaseInitializer();

                    Console.WriteLine("--begin to Initialize the tenant--dba.");
                    dataInitializer.Initialize(tenants);
                    Console.WriteLine("--end to Initialize the tenant--dba.");

                    //Console.WriteLine("--begin to Initialize the tenant--dba.");
                    //dataInitializer.SeedData(tenants);
                    //Console.WriteLine("--end to Initialize the tenant--dba.");
                    #endregion
                }

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
                    .UseUrls("http://*:3001"/*, "https://*:3011"*/)
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
