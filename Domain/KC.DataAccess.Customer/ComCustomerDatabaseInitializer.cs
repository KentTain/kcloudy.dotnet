using System.Collections.Generic;
using KC.Database.EFRepository;
using KC.Framework.Tenant;

namespace KC.DataAccess.Customer
{
    public class ComCustomerDatabaseInitializer : MultiTenantSqlServerDatabaseInitializer<ComCustomerContext>
    {
        public ComCustomerDatabaseInitializer()
            : base()
        { }

        public override ComCustomerContext Create(Tenant tenant)
        {
            if (tenant == null)
                throw new System.ArgumentNullException("Tenant is null", "tenant");
            if (string.IsNullOrEmpty(tenant.TenantName))
                throw new System.ArgumentException("tenantName is null or empty", "tenantName");
            if (string.IsNullOrEmpty(tenant.ConnectionString))
                throw new System.ArgumentException("connectionString is null or empty", "connectionString");

            var options = GetCachedDbContextOptions(tenant.TenantName, tenant.ConnectionString, tenant.DatabaseType);
            return new ComCustomerContext(options, tenant);
        }

        protected override string GetTargetMigration()
        {
            return KC.DataAccess.Customer.DataInitial.DBSqlInitializer.GetPreMigrationVersion();
        }
    }
}
