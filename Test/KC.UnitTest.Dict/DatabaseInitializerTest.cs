using KC.DataAccess.Dict;
using KC.Framework.Tenant;
using Xunit;
using System;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace KC.UnitTest.Dict
{
    public class DatabaseInitializerTest : DictTestBase
    {
        private static ILogger _logger;
        public DatabaseInitializerTest(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(DatabaseInitializerTest));
        }


        [Xunit.Fact]
        public void ComDictDatabaseInitializer_Test()
        {
            var tenants = new List<Tenant>() { DbaTenant, TestTenant, BuyTenant, SaleTenant, DbaTenant };
            var dataInitializer = new ComDictDatabaseInitializer();
            dataInitializer.Initialize(tenants);

            //dataInitializer.SeedData(tenants);

        }

        [Xunit.Fact]
        public void ComDictDatabaseInitializer_RollBack_Test()
        {
            var dataInitializer = new ComDictDatabaseInitializer();

            //_logger.LogDebug("--begin to Initialize the tenant.");
            //dataInitializer.Initialize(DbaTenant);
            dataInitializer.RollBackByMigration(new List<Tenant>() { DbaTenant, TestTenant, BuyTenant, SaleTenant, DbaTenant }, "0");
            //_logger.LogDebug("--end to Initialize the tenant.");
        }

    }
}
