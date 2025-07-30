using KC.DataAccess.Admin;
using KC.Database.Extension;
using KC.Database.Util;
using KC.Framework.SecurityHelper;
using KC.Framework.Tenant;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace KC.UnitTest.Admin
{
    
    public class DatabaseInitializerTest : AdminTestBase
    {
        private ILogger _logger;
        public DatabaseInitializerTest(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(DatabaseInitializerTest));
        }

        [Xunit.Fact]
        public void ComAdminDatabaseInitializer_Test()
        {
            //DbLoginUserUtil.CreateTenantDbLoginUser(DbaTenant, TenantConstant.DefaultDatabaseConnectionString);

            var dataInitializer = new ComAdminDatabaseInitializer();

            _logger.LogDebug("--begin to Initialize the tenant.");
            dataInitializer.Initialize(DbaTenant);
            _logger.LogDebug("--end to Initialize the tenant.");

            _logger.LogDebug("--begin to Seed the tenant.");
            dataInitializer.SeedData(DbaTenant);
            _logger.LogDebug("--end to Seed the tenant.");
        }

        [Xunit.Fact]
        public void ComAdminDatabaseInitializer_RollBack_Test()
        {
            var dataInitializer = new ComAdminDatabaseInitializer();

            //_logger.LogDebug("--begin to Initialize the tenant.");
            //dataInitializer.Initialize(DbaTenant);
            dataInitializer.RollBackByMigration(new List<Tenant>() { DbaTenant }, "0");
            //_logger.LogDebug("--end to Initialize the tenant.");
        }

        [Xunit.Fact]
        public void DatabaseInitializer_DbLoginUser_Test()
        {
            DbLoginUserUtil.CreateTenantDbLoginUser(DbaTenant, TenantConstant.DefaultDatabaseConnectionString);
            DbLoginUserUtil.CreateTenantDbLoginUser(TestTenant, TenantConstant.DefaultDatabaseConnectionString);
            DbLoginUserUtil.CreateTenantDbLoginUser(BuyTenant, TenantConstant.DefaultDatabaseConnectionString);
            DbLoginUserUtil.CreateTenantDbLoginUser(SaleTenant, TenantConstant.DefaultDatabaseConnectionString);
        }

        [Xunit.Fact]
        public void DbContextExtensions_AddOrUpdateTest()
        {
            var dataInitializer = new ComAdminDatabaseInitializer();
            var connItem = KC.Framework.Tenant.TenantConstant.GetDatabaseConnectionItems();
            var server = connItem.Item1;
            var database = connItem.Item2;
            var user = connItem.Item3;
            var pwd = connItem.Item4;
            var hashPwd = EncryptPasswordUtil.EncryptPassword(pwd);

            var databasePool = new Model.Admin.DatabasePool
            {
                CloudType = CloudType.Azure,
                DatabasePoolId = 2,
                Server = server,
                Database = database,
                UserName = user,
                TenantCount = new Random().Next(),
                CanEdit = false,
                UserPasswordHash = hashPwd,
            };

            using (var context = dataInitializer.Create(TenantConstant.DbaTenantApiAccessInfo))
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    context.Database.ExecuteSqlInterpolated($"SET IDENTITY_INSERT [cDba].[tenant_DatabasePool] ON ");
                    context.AddOrUpdate(databasePool);
                    context.Database.ExecuteSqlInterpolated($"SET IDENTITY_INSERT [cDba].[tenant_DatabasePool] OFF ");
                    transaction.Commit();
                }
            }

            using (var context = dataInitializer.Create(TenantConstant.DbaTenantApiAccessInfo))
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    context.Database.ExecuteSqlInterpolated($"SET IDENTITY_INSERT [cDba].[tenant_DatabasePool] ON ");
                    databasePool.TenantCount += 1;
                    context.AddOrUpdate(databasePool);
                    context.Database.ExecuteSqlInterpolated($"SET IDENTITY_INSERT [cDba].[tenant_DatabasePool] OFF ");
                    transaction.Commit();
                }
            }
        }
    }
}
