using KC.DataAccess.Config;
using KC.Database.Util;
using KC.Framework.Tenant;
using Xunit;
using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace KC.UnitTest.Config
{
    
    public class DatabaseInitializerTest : ConfigTestBase
    {
        private ILogger _logger;
        public DatabaseInitializerTest(CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(DatabaseInitializerTest));
        }

        [Xunit.Fact]
        public void ComConfigDatabaseInitializer_Test()
        {
            //DbLoginUserUtil.CreateTenantDbLoginUser(DbaTenant, TenantConstant.DefaultDatabaseConnectionString);

            var dataInitializer = new ComConfigDatabaseInitializer();

            //_logger.LogDebug("--begin to Initialize the tenant.");
            //dataInitializer.Initialize(DbaTenant);
            dataInitializer.Initialize(new List<Tenant>() { DbaTenant, TestTenant, BuyTenant, SaleTenant });
            //_logger.LogDebug("--end to Initialize the tenant.");
        }

        [Xunit.Fact]
        public void ComConfigDatabaseInitializer_RollBack_Test()
        {
            var dataInitializer = new ComConfigDatabaseInitializer();

            //_logger.LogDebug("--begin to Initialize the tenant.");
            //dataInitializer.Initialize(DbaTenant);
            dataInitializer.RollBackByMigration(new List<Tenant>() { DbaTenant, TestTenant, BuyTenant, SaleTenant }, "0");
            //_logger.LogDebug("--end to Initialize the tenant.");
        }
    }
}
