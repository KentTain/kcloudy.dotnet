using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using KC.Common;
using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Framework.Util;
using KC.Service.Training;
using KC.Service.WebApiService.Business;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KC.UnitTest.Training
{
    public class TeacherServiceTest : ConfigTestBase
    {
        private ILogger _logger;
        public TeacherServiceTest(CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(TeacherServiceTest));
        }

        [Xunit.Fact]
        public void SeedService_GenerateId_Test()
        {
            InjectTenant(TestTenant);
            // Service的注入
            KC.Service.Training.DependencyInjectUtil.InjectService(Services);

            //var icount = 100;
            //var disc = new ConcurrentDictionary<int, string>();
            //Parallel.For(0, icount, i =>
            //{
            //    var testSeedService = Services.BuildServiceProvider().GetService<ITeacherService>();
            //    var result = testSeedService.GenerateIdByName("Member");
            //    disc.TryAdd(i, result.SeedValue);
            //});

            //_logger.LogInformation("SeedService_GenerateId_Test: " + disc.Values.ToCommaSeparatedString());
            //Xunit.Assert.Equal(icount, disc.Count);
        }

        
    }
}
