using KC.Framework.Util;
using KC.Database.EFRepository;
using KC.Framework.Tenant;
using System;
using Microsoft.EntityFrameworkCore;
using KC.Model.Offering.Constants;
using KC.Model.Offering;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.Extensions.Logging;

namespace KC.DataAccess.Offering
{
    public class ComOfferingContext : MultiTenantDataContext
    {
        /// <summary>
        /// Only used by migrations With default tenantId "Dba"
        /// </summary>
        //public ComDictContext(DbContextOptions<ComDictContext> options)
        //    : base(options, TenantConstant.DbaTenantApiAccessInfo)
        //{
        //    ////For Debug
        //    //if (System.Diagnostics.Debugger.IsAttached == false)
        //    //    System.Diagnostics.Debugger.Launch();
        //}
        public ComOfferingContext(
            DbContextOptions<ComOfferingContext> options,
            Tenant tenant,
            IMemoryCache cache = null,
            //ICoreConventionSetBuilder builder = null,
            ILogger<ComOfferingContext> logger = null)
            : base(options, tenant, cache, logger)
        {
        }

        public DbSet<Category> OfferingCategories { get; set; }
        public DbSet<CategoryManager> CategoryManagers { get; set; }
        public DbSet<PropertyProvider> PropertyProviders { get; set; }
        public DbSet<PropertyProviderAttr> PropertyProviderAttrs { get; set; }
        public DbSet<CategoryOperationLog> CategoryOperationLogs { get; set; }

        public DbSet<Model.Offering.Offering> Offerings { get; set; }
        public DbSet<OfferingProperty> OfferingProperties { get; set; }
        public DbSet<OfferingOperationLog> OfferingOperationLogs { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductProperty> ProductProperties { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
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

            modelBuilder.Entity<Category>().ToTable(Tables.Category, this.TenantName);
            modelBuilder.Entity<CategoryManager>().ToTable(Tables.CategoryManager, this.TenantName);
            modelBuilder.Entity<PropertyProvider>().ToTable(Tables.PropertyProvider, this.TenantName);
            modelBuilder.Entity<PropertyProviderAttr>().ToTable(Tables.PropertyProviderAttr, this.TenantName);
            modelBuilder.Entity<CategoryOperationLog>().ToTable(Tables.CategoryOperationLog, this.TenantName);

            modelBuilder.Entity<Model.Offering.Offering>().ToTable(Tables.Offering, this.TenantName);
            modelBuilder.Entity<OfferingOperationLog>().ToTable(Tables.OfferingOperationLog, this.TenantName);
            modelBuilder.Entity<OfferingProperty>().ToTable(Tables.OfferingProperty, this.TenantName);

            modelBuilder.Entity<Product>().ToTable(Tables.Product, this.TenantName);
            modelBuilder.Entity<ProductProperty>().ToTable(Tables.ProductProperty, this.TenantName);

        }
    }
}
