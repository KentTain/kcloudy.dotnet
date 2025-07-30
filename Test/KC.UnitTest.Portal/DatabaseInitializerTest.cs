using KC.DataAccess.Portal;
using KC.Framework.Tenant;
using Xunit;
using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace KC.UnitTest.Portal
{
    public class DatabaseInitializerTest : PortalTestBase
    {
        private static ILogger _logger;
        public DatabaseInitializerTest(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(DatabaseInitializerTest));
        }


        [Xunit.Fact]
        public void ComPortalDatabaseInitializer_Test()
        {
            //DbLoginUserUtil.CreateTenantDbLoginUser(DbaTenant, TenantConstant.DefaultDatabaseConnectionString);

            var tenants = new List<Tenant>() { DbaTenant, TestTenant, BuyTenant, SaleTenant };
            var dataInitializer = new ComPortalDatabaseInitializer();
            dataInitializer.Initialize(tenants);

        }

        [Xunit.Fact]
        public void ComPortalDatabaseInitializer_RollBack_Test()
        {
            var dataInitializer = new ComPortalDatabaseInitializer();

            //_logger.LogDebug("--begin to Initialize the tenant.");
            //dataInitializer.Initialize(DbaTenant);
            dataInitializer.RollBackByMigration(new List<Tenant>() { DbaTenant, TestTenant, BuyTenant, SaleTenant }, "0");
            //_logger.LogDebug("--end to Initialize the tenant.");
        }

    }
}
