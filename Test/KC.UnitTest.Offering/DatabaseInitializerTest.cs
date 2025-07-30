using KC.DataAccess.Offering;
using KC.Framework.Tenant;
using Xunit;
using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace KC.UnitTest.Offering
{
    public class DatabaseInitializerTest : OfferingTestBase
    {
        private static ILogger _logger;
        public DatabaseInitializerTest(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(DatabaseInitializerTest));
        }

        [Xunit.Fact]
        public void ComOfferingDatabaseInitializer_Test()
        {
            //Database.Util.DbLoginUserUtil.CreateTenantDbLoginUser(DbaTenant, TenantConstant.DefaultDatabaseConnectionString);

            var dataInitializer = new ComOfferingDatabaseInitializer();

            //LogUtil.LogDebug("--begin to Initialize the tenant.");
            dataInitializer.Initialize(new List<Tenant>() { DbaTenant, TestTenant });
            //LogUtil.LogDebug("--end to Initialize the tenant.");

            //LogUtil.LogDebug("--begin to Seed the tenant.");
            //dataInitializer.SeedData(DbaTenant);
            //LogUtil.LogDebug("--end to Seed the tenant.");
        }

        [Xunit.Fact]
        public void ComOfferingDatabaseInitializer_RollBack_Test()
        {
            var dataInitializer = new ComOfferingDatabaseInitializer();

            //_logger.LogDebug("--begin to Initialize the tenant.");
            //dataInitializer.Initialize(DbaTenant);
            dataInitializer.RollBackByMigration(new List<Tenant>() { DbaTenant, TestTenant }, "0");
            //_logger.LogDebug("--end to Initialize the tenant.");
        }

    }
}
