using KC.Database.EFRepository;
using KC.Database.Extension;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Framework.Util;
using KC.Model.CodeGenerate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KC.DataAccess.CodeGenerate
{
    /// <summary>
    /// 数据库初始化操作类
    /// </summary>
    public class ComCodeDatabaseInitializer : MultiTenantSqlServerDatabaseInitializer<ComCodeContext>
    {
        public ComCodeDatabaseInitializer()
            : base()
        { }

        public override ComCodeContext Create(Tenant tenant)
        {
            if (tenant == null)
                throw new System.ArgumentNullException("Tenant is null", "tenant");
            if (string.IsNullOrEmpty(tenant.TenantName))
                throw new System.ArgumentException("tenantName is null or empty", "tenantName");
            if (string.IsNullOrEmpty(tenant.ConnectionString))
                throw new System.ArgumentException("connectionString is null or empty", "connectionString");

            var options = GetCachedDbContextOptions(tenant.TenantName, tenant.ConnectionString, tenant.DatabaseType);
            return new ComCodeContext(options, tenant);
        }

        protected override string GetTargetMigration()
        {
            return DataInitial.DBSqlInitializer.GetPreMigrationVersion();
        }

        public void SeedData(Tenant tenant)
        {

        }
    }
}
