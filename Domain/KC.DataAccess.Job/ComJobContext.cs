using KC.Framework.Util;
using KC.Database.EFRepository;
using KC.Framework.Tenant;
using System;
using Microsoft.EntityFrameworkCore;
using KC.Model.Job.Constants;
using KC.Model.Job;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.Extensions.Logging;
using KC.Model.Job.Table;

namespace KC.DataAccess.Job
{
    public class ComJobContext : MultiTenantDataContext
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
        public ComJobContext(
            DbContextOptions<ComJobContext> options,
            Tenant tenant,
            IMemoryCache cache = null,
            //ICoreConventionSetBuilder builder = null,
            ILogger<ComJobContext> logger = null)
            : base(options, tenant ?? TenantConstant.DbaTenantApiAccessInfo, cache, logger)
        {
        }

        public DbSet<ThreadConfigInfo> ThreadConfigInfos { get; set; }
        public DbSet<ThreadStatusInfo> ThreadStatusInfos { get; set; }

        public DbSet<DatabaseVersionInfo> DatabaseVersionInfos { get; set; }
        public DbSet<QueueErrorMessage> QueueErrorMessages { get; set; }

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

            modelBuilder.Entity<ThreadConfigInfo>().ToTable(Tables.ThreadConfigInfo, this.TenantName);
            modelBuilder.Entity<ThreadStatusInfo>().ToTable(Tables.ThreadStatusInfo, this.TenantName);
            modelBuilder.Entity<DatabaseVersionInfo>().ToTable(Tables.DatabaseVersionInfo, this.TenantName);
            modelBuilder.Entity<QueueErrorMessage>().ToTable(Tables.QueueErrorMessage, this.TenantName);

        }
    }
}
