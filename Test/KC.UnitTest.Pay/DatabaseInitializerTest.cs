using KC.DataAccess.Pay;
using KC.Framework.Tenant;
using Xunit;
using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace KC.UnitTest.Pay
{
    public class DatabaseInitializerTest : PayTestBase
    {
        private static ILogger _logger;
        public DatabaseInitializerTest(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(DatabaseInitializerTest));
        }

        [Xunit.Fact]
        public void ComPayDatabaseInitializer_Test()
        {
            //DbLoginUserUtil.CreateTenantDbLoginUser(DbaTenant, TenantConstant.DefaultDatabaseConnectionString);

            var tenants = new List<Tenant>() { TestTenant, BuyTenant, SaleTenant };
            var dataInitializer = new ComPayDatabaseInitializer();
            dataInitializer.Initialize(tenants);

            //dataInitializer.SeedData(tenants);

        }

        [Xunit.Fact]
        public void ComPayDatabaseInitializer_RollBack_Test()
        {
            var dataInitializer = new ComPayDatabaseInitializer();

            //_logger.LogDebug("--begin to Initialize the tenant.");
            //dataInitializer.Initialize(DbaTenant);
            dataInitializer.RollBackByMigration(new List<Tenant>() { TestTenant, BuyTenant, SaleTenant }, "0");
            //_logger.LogDebug("--end to Initialize the tenant.");
        }
    }
}
