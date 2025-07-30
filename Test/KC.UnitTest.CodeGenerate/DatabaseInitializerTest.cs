using KC.DataAccess.CodeGenerate;
using KC.Database.Util;
using KC.Framework.Tenant;
using Xunit;
using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace KC.UnitTest.CodeGenerate
{
    
    public class DatabaseInitializerTest : CodeGenerateTestBase
    {
        private static ILogger _logger;
        public DatabaseInitializerTest(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(DatabaseInitializerTest));
        }

        [Xunit.Fact]
        public void ComCodeDatabaseInitializer_Test()
        {
            //DbLoginUserUtil.CreateTenantDbLoginUser(DbaTenant, TenantConstant.DefaultDatabaseConnectionString);

            var tenants = new List<Tenant>() { DbaTenant, TestTenant, BuyTenant, SaleTenant };
            var dataInitializer = new ComCodeDatabaseInitializer();
            dataInitializer.Initialize(tenants);

            _logger.LogDebug("--begin to Seed the tenant.");
            dataInitializer.SeedData(DbaTenant);
            _logger.LogDebug("--end to Seed the tenant.");
        }

        [Xunit.Fact]
        public void ComCodeDatabaseInitializer_RollBack_Test()
        {
            var dataInitializer = new ComCodeDatabaseInitializer();

            //_logger.LogDebug("--begin to Initialize the tenant.");
            //dataInitializer.Initialize(DbaTenant);
            dataInitializer.RollBackByMigration(new List<Tenant>() { DbaTenant, TestTenant, BuyTenant, SaleTenant }, "0");
            //_logger.LogDebug("--end to Initialize the tenant.");
        }
    }
}
