using KC.DataAccess.Blog;
using KC.Database.Util;
using KC.Framework.Tenant;
using Xunit;
using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace KC.UnitTest.Blog
{
    
    public class DatabaseInitializerTest : BlogTestBase
    {
        private ILogger _logger;
        public DatabaseInitializerTest(CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(DatabaseInitializerTest));
        }

        [Xunit.Fact]
        public void ComAdminDatabaseInitializer_Initialize_Test()
        {
            //DbLoginUserUtil.CreateTenantDbLoginUser(DbaTenant, TenantConstant.DefaultDatabaseConnectionString);

            var dataInitializer = new ComBlogDatabaseInitializer();

            //_logger.LogDebug("--begin to Initialize the tenant.");
            //dataInitializer.Initialize(DbaTenant);
            dataInitializer.Initialize(new List<Tenant>() { DbaTenant});
            //_logger.LogDebug("--end to Initialize the tenant.");
        }

        [Xunit.Fact]
        public void ComAdminDatabaseInitializer_RollBack_Test()
        {
            var dataInitializer = new ComBlogDatabaseInitializer();

            //_logger.LogDebug("--begin to Initialize the tenant.");
            //dataInitializer.Initialize(DbaTenant);
            dataInitializer.RollBackByMigration(new List<Tenant>() { DbaTenant }, "0");
            //_logger.LogDebug("--end to Initialize the tenant.");
        }
    }
}
