using System;
using KC.Database.EFRepository;
using KC.Framework.Tenant;
using KC.Model.Customer;
using KC.Model.Customer.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace KC.DataAccess.Customer
{
    public class ComCustomerContext : MultiTenantDataContext
    {
        /// <summary>
        /// Only used by migrations With default tenantId "cTest"
        /// </summary>
        //public ComCustomerContext(DbContextOptions<ComCustomerContext> options)
        //    : base(options, TenantConstant.TestTenantApiAccessInfo)
        //{
        //    ////For Debug
        //    //if (System.Diagnostics.Debugger.IsAttached == false)
        //    //    System.Diagnostics.Debugger.Launch();
        //}
        public ComCustomerContext(
            DbContextOptions<ComCustomerContext> options,
            Tenant tenant,
            IMemoryCache cache = null,
            //ICoreConventionSetBuilder builder = null,
            ILogger<ComCustomerContext> logger = null)
            : base(options, tenant, cache, logger)
        {
        }

        #region DbSet

        public DbSet<CustomerInfo> CustomerInfos { get; set; }
        public DbSet<CustomerAuthentication> CustomerAuthentications { get; set; }
        public DbSet<CustomerExtInfoProvider> CustomerExtInfoProviders { get; set; }
        public DbSet<CustomerExtInfo> CustomerExtInfos { get; set; }
        public DbSet<CustomerContact> CustomerContacts { get; set; }
        public DbSet<CustomerManager> CustomerManagers { get; set; }
        public DbSet<CustomerSeas> CustomerSeases { get; set; }

        public DbSet<CustomerTracingLog> CustomerTracingInfos { get; set; }
        public DbSet<CustomerChangeLog> CustomerChangeLogs { get; set; }

        
        public DbSet<CustomerSendToTenantLog> CustomerSendToTenantLogs { get; set; }

        public DbSet<NotificationApplication> NotificationApplications { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException("modelBuilder");
            }

            base.OnModelCreating(modelBuilder);

            //移除一对多的级联删除约定，想要级联删除可以在 EntityTypeConfiguration<TEntity>的实现类中进行控制
            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            //多对多启用级联删除约定，不想级联删除可以在删除前判断关联的数据进行拦截
            //modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            //modelBuilder.Properties<decimal>().Configure(config => config.HasPrecision(18, 4));

            modelBuilder.Entity<CustomerInfo>().ToTable(Tables.CustomerInfo, TenantName);
            modelBuilder.Entity<CustomerAuthentication>().ToTable(Tables.CustomerAuthentication, TenantName);
            modelBuilder.Entity<CustomerExtInfoProvider>().ToTable(Tables.CustomerExtInfoProvider, TenantName);
            modelBuilder.Entity<CustomerExtInfo>().ToTable(Tables.CustomerExtInfo, TenantName);

            modelBuilder.Entity<CustomerContact>().ToTable(Tables.CustomerContact, TenantName);
            modelBuilder.Entity<CustomerManager>().ToTable(Tables.CustomerManager, TenantName);
            modelBuilder.Entity<CustomerSeas>().ToTable(Tables.CustomerSeas, TenantName);
            modelBuilder.Entity<CustomerTracingLog>().ToTable(Tables.CustomerTracingLog, TenantName);
            modelBuilder.Entity<CustomerChangeLog>().ToTable(Tables.CustomerChangeLog, TenantName);
            modelBuilder.Entity<CustomerSendToTenantLog>().ToTable(Tables.CustomerSendToTenantLog, TenantName);

            modelBuilder.Entity<NotificationApplication>().ToTable(Tables.NotificationApplication, TenantName);

            //One by one
            modelBuilder.Entity<CustomerInfo>()
                .HasOne(b => b.AuthenticationInfo)
                .WithOne(i => i.CustomerInfo)
                .HasForeignKey<CustomerAuthentication>(b => b.CustomerId);

        }
    }
}
