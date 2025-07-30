using KC.DataAccess.Workflow;
using KC.Framework.Tenant;
using Xunit;
using System;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace KC.UnitTest.Workflow
{
    public class DatabaseInitializerTest : WorkflowTestBase
    {
        private static ILogger _logger;
        public DatabaseInitializerTest(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(DatabaseInitializerTest));
        }


        [Xunit.Fact]
        public void ComWorkflowDatabaseInitializer_Test()
        {
            var tenants = new List<Tenant>() { DbaTenant, TestTenant, SaleTenant, BuyTenant };
            var dataInitializer = new ComWorkflowDatabaseInitializer();
            dataInitializer.Initialize(tenants);

            _logger.LogDebug("--begin to Seed the tenant.");
            dataInitializer.SeedData(DbaTenant);
            _logger.LogDebug("--end to Seed the tenant.");

        }

        [Xunit.Fact]
        public void ComWorkflowDatabaseInitializer_RollBack_Test()
        {
            var dataInitializer = new ComWorkflowDatabaseInitializer();

            //_logger.LogDebug("--begin to Initialize the tenant.");
            //dataInitializer.Initialize(DbaTenant);  
            dataInitializer.RollBackByMigration(new List<Tenant>() { DbaTenant, TestTenant, SaleTenant, BuyTenant }, "0");
            //_logger.LogDebug("--end to Initialize the tenant.");
        }

    }
}
