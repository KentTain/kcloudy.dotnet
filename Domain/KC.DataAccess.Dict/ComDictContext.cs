using KC.Framework.Util;
using KC.Database.EFRepository;
using KC.Framework.Tenant;
using System;
using Microsoft.EntityFrameworkCore;
using KC.Model.Dict.Constants;
using KC.Model.Dict;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.Extensions.Logging;

namespace KC.DataAccess.Dict
{
    public class ComDictContext : MultiTenantDataContext
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
        public ComDictContext(
            DbContextOptions<ComDictContext> options,
            Tenant tenant,
            IMemoryCache cache = null,
            //ICoreConventionSetBuilder builder = null,
            ILogger<ComDictContext> logger = null)
            : base(options, tenant, cache, logger)
        {
        }

        public DbSet<DictType> DictTypes { get; set; }
        public DbSet<DictValue> DictValues { get; set; }

        public DbSet<Province> Provinces { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<MobileLocation> MobileLocations { get; set; }
        public DbSet<IndustryClassfication> IndustryClassfications { get; set; }

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

            modelBuilder.Entity<DictType>().ToTable(Tables.DictType, this.TenantName);
            modelBuilder.Entity<DictValue>().ToTable(Tables.DictValue, this.TenantName);
            modelBuilder.Entity<City>().ToTable(Tables.City, this.TenantName);
            modelBuilder.Entity<Province>().ToTable(Tables.Province, this.TenantName);
            modelBuilder.Entity<IndustryClassfication>().ToTable(Tables.IndustryClassfication, this.TenantName);
            modelBuilder.Entity<MobileLocation>().ToTable(Tables.MobileLocation, this.TenantName);

        }
    }
}
