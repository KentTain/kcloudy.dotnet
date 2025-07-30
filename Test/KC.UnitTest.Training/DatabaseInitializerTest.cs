using KC.DataAccess.Training;
using KC.Database.Util;
using KC.Framework.Tenant;
using Xunit;
using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace KC.UnitTest.Training
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
        public void ComTrainingDatabaseInitializer_Initialize_Test()
        {
            //DbLoginUserUtil.CreateTenantDbLoginUser(DbaTenant, TenantConstant.DefaultDatabaseConnectionString);

            var dataInitializer = new ComTrainingDatabaseInitializer();

            //_logger.LogDebug("--begin to Initialize the tenant.");
            //dataInitializer.Initialize(DbaTenant);
            dataInitializer.Initialize(new List<Tenant>() { DbaTenant, TestTenant, BuyTenant, SaleTenant });
            //_logger.LogDebug("--end to Initialize the tenant.");
        }

        [Xunit.Fact]
        public void ComTrainingDatabaseInitializer_RollBack_Test()
        {
            var dataInitializer = new ComTrainingDatabaseInitializer();

            //_logger.LogDebug("--begin to Initialize the tenant.");
            //dataInitializer.Initialize(DbaTenant);
            dataInitializer.RollBackByMigration(new List<Tenant>() { DbaTenant, TestTenant, BuyTenant, SaleTenant }, "0");
            //_logger.LogDebug("--end to Initialize the tenant.");
        }
    }
}
