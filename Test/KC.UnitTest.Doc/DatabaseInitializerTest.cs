using KC.DataAccess.Doc;
using KC.Framework.Tenant;
using Xunit;
using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace KC.UnitTest.Doc
{
    public class DatabaseInitializerTest : DocTestBase
    {
        private static ILogger _logger;
        public DatabaseInitializerTest(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(DatabaseInitializerTest));
        }

        [Xunit.Fact]
        public void ComDocDatabaseInitializer_Test()
        {
            //DbLoginUserUtil.CreateTenantDbLoginUser(DbaTenant, TenantConstant.DefaultDatabaseConnectionString);

            var tenants = new List<Tenant>() { TestTenant, BuyTenant, SaleTenant };
            var dataInitializer = new ComDocDatabaseInitializer();
            dataInitializer.Initialize(tenants);

            //dataInitializer.SeedData(tenants);

        }

        [Xunit.Fact]
        public void ComDocDatabaseInitializer_RollBack_Test()
        {
            var dataInitializer = new ComDocDatabaseInitializer();

            //_logger.LogDebug("--begin to Initialize the tenant.");
            //dataInitializer.Initialize(DbaTenant);
            dataInitializer.RollBackByMigration(new List<Tenant>() { TestTenant, BuyTenant, SaleTenant }, "0");
            //_logger.LogDebug("--end to Initialize the tenant.");
        }
    }
}
