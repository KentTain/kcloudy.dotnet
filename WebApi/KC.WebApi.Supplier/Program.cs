using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace KC.WebApi.Supplier
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
               
                var host = CreateHostBuilder(args).Build();

                Web.Util.StaticFactoryUtil.InitWeb();

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
                    .UseUrls("http://*:1102", "https://*:1112")
                    .UseIISIntegration()
                    .UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                // NLog: setup NLog for Dependency injection;
                .UseNLog();
        }
    }
}
