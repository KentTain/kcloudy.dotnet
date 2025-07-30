using KC.DataAccess.Account;
using KC.Database.EFRepository;
using KC.Database.Util;
using KC.Framework.Tenant;
using KC.Framework.Util;
using KC.Service.Util;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Collections.Generic;

namespace KC.UnitTest.Account
{
    
    public class DatabaseInitializerTest : AccountTestBase
    {
        private Microsoft.Extensions.Logging.ILogger _logger;
        public DatabaseInitializerTest(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(DatabaseInitializerTest));
        }

        [Xunit.Fact]
        public void ComAccountDatabaseInitializer_Test()
        {
            var tenants = new List<Tenant>() { TestTenant, BuyTenant, SaleTenant, DbaTenant };
            var dataInitializer = new ComAccountDatabaseInitializer();
            dataInitializer.Initialize(tenants);

            //dataInitializer.SeedData(tenants);
        }

        [Xunit.Fact]
        public void ComAccountDatabaseInitializer_RollBack_Test()
        {
            var dataInitializer = new ComAccountDatabaseInitializer();

            //_logger.LogDebug("--begin to Initialize the tenant.");
            //dataInitializer.Initialize(DbaTenant);
            dataInitializer.RollBackByMigration(new List<Tenant>() { TestTenant, BuyTenant, SaleTenant, DbaTenant }, "0");
            //_logger.LogDebug("--end to Initialize the tenant.");
        }
    }
}
