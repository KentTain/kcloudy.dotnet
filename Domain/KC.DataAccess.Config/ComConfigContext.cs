using System;
using KC.Framework.Tenant;
using KC.Model.Config;
using KC.Model.Config.Constants;
using KC.Framework.Base;
using KC.Database.EFRepository;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace KC.DataAccess.Config
{
    public class ComConfigContext : MultiTenantDataContext
    {
        /// <summary>
        /// Only used by migrations With default tenantId "Dba"
        /// </summary>
        //public ComConfigContext(DbContextOptions<ComConfigContext> options)
        //    : base(options, TenantConstant.DbaTenantApiAccessInfo)
        //{
        //    ////For Debug
        //    //if (System.Diagnostics.Debugger.IsAttached == false)
        //    //    System.Diagnostics.Debugger.Launch();
        //}
        public ComConfigContext(
            DbContextOptions<ComConfigContext> options,
            Tenant tenant,
            IMemoryCache cache = null,
            ////ICoreConventionSetBuilder builder = null,
            ILogger<ComConfigContext> logger = null)
            : base(options, tenant, cache, logger)
        {
        }

        public DbSet<SeedEntity> SeedEntities { get; set; }
        public DbSet<SysSequence> SysSequences { get; set; }
        public DbSet<ConfigEntity> ConfigEntities { get; set; }
        public DbSet<ConfigAttribute> ConfigAttributes { get; set; }
        public DbSet<ConfigLog> ConfigLogs { get; set; }

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

            modelBuilder.Entity<SeedEntity>().ToTable(Tables.SeedEntity, TenantName); ;
            modelBuilder.Entity<SysSequence>().ToTable(Tables.SysSequence, TenantName);
            
            modelBuilder.Entity<ConfigEntity>().ToTable(Tables.ConfigEntity, TenantName);
            modelBuilder.Entity<ConfigAttribute>().ToTable(Tables.ConfigAttribute, TenantName);
            modelBuilder.Entity<ConfigLog>().ToTable(Tables.ConfigLog, TenantName);
            
        }
    }
}
