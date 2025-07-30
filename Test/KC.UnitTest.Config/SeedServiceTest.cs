using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using KC.Common;
using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Framework.Util;
using KC.Service.Config;
using KC.Service.WebApiService.Business;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KC.UnitTest.Config
{
    public class SeedServiceTest : ConfigTestBase
    {
        private ILogger _logger;
        public SeedServiceTest(CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(SeedServiceTest));
        }

        [Xunit.Fact]
        public void SeedService_GenerateId_Test()
        {
            InjectTenant(TestTenant);
            // Service的注入
            KC.Service.Config.DependencyInjectUtil.InjectService(Services);

            var testSeedService = Services.BuildServiceProvider().GetService<ISeedService>();
            var result = testSeedService.GetSeedEntityByName("Member");

            _logger.LogInformation("SeedService_GenerateId_Test: " + result.SeedValue);
        }

        [Xunit.Fact]
        public void SeedService_GenerateId_Parallel_Test()
        {
            InjectTenant(TestTenant);
            // Service的注入
            KC.Service.Config.DependencyInjectUtil.InjectService(Services);

            var icount = 100;
            var disc = new ConcurrentDictionary<int, string>();
            Parallel.For(0, icount, i =>
            {
                var testSeedService = Services.BuildServiceProvider().GetService<ISeedService>();
                var result = testSeedService.GetSeedEntityByName("Member");
                disc.TryAdd(i, result.SeedValue);
            });

            _logger.LogInformation("SeedService_GenerateId_Test: " + disc.Values.ToCommaSeparatedString());
            Xunit.Assert.Equal(icount, disc.Count);
        }

        [Xunit.Fact]
        public void SeedService_GenerateIdWithStep_Test()
        {
            InjectTenant(TestTenant);
            // Service的注入
            KC.Service.Config.DependencyInjectUtil.InjectService(Services);
            var testSeedService = Services.BuildServiceProvider().GetService<ISeedService>();

            var rows = 10;
            var count = rows - 1;
            var seed = testSeedService.GetSeedEntityByName("Customer", count);
            var min = seed.SeedMin;
            var codePrefix = seed.SeedValue.Substring(0, seed.SeedValue.Length - 5);

            var disc = new ConcurrentBag<string>();
            for (var i = 0; i <= rows; i++)
            {
                disc.Add(codePrefix + (min + i - 1).ToString().PadLeft(5, '0'));
            }

            _logger.LogInformation("SeedService_GenerateIdWithStep_Test: " + disc.ToCommaSeparatedString());
        }

        [Xunit.Fact]
        public void SeedService_GetPaginatedSysSequencesByName_Test()
        {
            InjectTenant(TestTenant);
            // Service的注入
            KC.Service.Config.DependencyInjectUtil.InjectService(Services);

            var testSeedService = Services.BuildServiceProvider().GetService<ISeedService>();
            var result = testSeedService.FindPaginatedSysSequencesByName(1, 10, null);

            Xunit.Assert.True(result.rows.Count > 0);
        }
    }
}
