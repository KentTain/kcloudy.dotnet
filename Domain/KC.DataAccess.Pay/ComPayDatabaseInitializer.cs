using KC.Database.EFRepository;
using KC.Framework.Tenant;

namespace KC.DataAccess.Pay
{
    /// <summary>
    /// 数据库初始化操作类
    /// </summary>
    public class ComPayDatabaseInitializer : MultiTenantSqlServerDatabaseInitializer<ComPayContext>
    {
        public ComPayDatabaseInitializer()
            : base()
        { }

        public override ComPayContext Create(Tenant tenant)
        {
            if (tenant == null)
                throw new System.ArgumentNullException("Tenant is null", "tenant");
            if (string.IsNullOrEmpty(tenant.TenantName))
                throw new System.ArgumentException("tenantName is null or empty", "tenantName");
            if (string.IsNullOrEmpty(tenant.ConnectionString))
                throw new System.ArgumentException("connectionString is null or empty", "connectionString");

            var options = GetCachedDbContextOptions(tenant.TenantName, tenant.ConnectionString, tenant.DatabaseType);
            return new ComPayContext(options, tenant);
        }

        protected override string GetTargetMigration()
        {
            return DataInitial.DBSqlInitializer.GetPreMigrationVersion();
        }
    }
}
