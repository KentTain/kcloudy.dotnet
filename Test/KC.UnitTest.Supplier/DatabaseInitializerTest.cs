using KC.DataAccess.Supplier;
using KC.Framework.Tenant;
using Xunit;
using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace KC.UnitTest.Supplier
{
    public class DatabaseInitializerTest : BakTestBase
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
            Database.Util.DbLoginUserUtil.CreateTenantDbLoginUser(DbaTenant, TenantConstant.DefaultDatabaseConnectionString);

            var dataInitializer = new ComSupplierDatabaseInitializer();

            //LogUtil.LogDebug("--begin to Initialize the tenant.");
            dataInitializer.Initialize(DbaTenant);
            //LogUtil.LogDebug("--end to Initialize the tenant.");

            //LogUtil.LogDebug("--begin to Seed the tenant.");
            //dataInitializer.SeedData(DbaTenant);
            //LogUtil.LogDebug("--end to Seed the tenant.");
        }

    }
}
