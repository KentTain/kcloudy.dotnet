using KC.DataAccess.Customer;
using KC.Database.Util;
using KC.Framework.Tenant;
using Xunit;
using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace KC.UnitTest.Customer
{
    
    public class DatabaseInitializerTest : CustomerTestBase
    {
        private ILogger _logger;
        public DatabaseInitializerTest(CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(DatabaseInitializerTest));
        }

        [Xunit.Fact]
        public void ComCustomerDatabaseInitializer_Initialize_Test()
        {
            //DbLoginUserUtil.CreateTenantDbLoginUser(DbaTenant, TenantConstant.DefaultDatabaseConnectionString);

            var dataInitializer = new ComCustomerDatabaseInitializer();

            //_logger.LogDebug("--begin to Initialize the tenant.");
            //dataInitializer.Initialize(DbaTenant);
            dataInitializer.Initialize(new List<Tenant>() { TestTenant, BuyTenant, SaleTenant });
            //_logger.LogDebug("--end to Initialize the tenant.");
        }

        [Xunit.Fact]
        public void ComCustomerDatabaseInitializer_RollBack_Test()
        {
            var dataInitializer = new ComCustomerDatabaseInitializer();

            //_logger.LogDebug("--begin to Initialize the tenant.");
            //dataInitializer.Initialize(DbaTenant);
            dataInitializer.RollBackByMigration(new List<Tenant>() { TestTenant, BuyTenant, SaleTenant }, "0");
            //_logger.LogDebug("--end to Initialize the tenant.");
        }
    }
}
