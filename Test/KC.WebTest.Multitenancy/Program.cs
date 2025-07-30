using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Com.WebTest.Multitenancy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "AspNetStructureMapSample";
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://localhost:60000", "http://localhost:60001", "http://localhost:60002", "http://localhost:60003")
                // This enables the request lifetime scope to be properly spawned from
                // the container rather than be a child of the default tenant scope.
                .UseAutofacMultitenantRequestServices(() => Startup.ApplicationContainer)
                .UseStartup<Startup>();
    }
}
