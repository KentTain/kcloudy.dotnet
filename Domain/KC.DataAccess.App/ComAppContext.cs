using KC.Framework.Util;
using KC.Database.EFRepository;
using KC.Framework.Tenant;
using System;
using Microsoft.EntityFrameworkCore;
using KC.Model.App.Constants;
using KC.Model.App;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace KC.DataAccess.App
{
    public class ComAppContext : MultiTenantDataContext
    {
        /// <summary>
        /// With default tenantId "Dba"
        /// </summary>
        //public ComAppContext(DbContextOptions<ComAppContext> options)
        //    : base(options, TenantConstant.DbaTenantApiAccessInfo)
        //{
        //    ////For Debug
        //    //if (System.Diagnostics.Debugger.IsAttached == false)
        //    //    System.Diagnostics.Debugger.Launch();
        //}
        public ComAppContext(
            DbContextOptions options,
            Tenant tenant,
            ////ICoreConventionSetBuilder builder = null,
            ILogger<ComAppContext> logger = null,
            IMemoryCache cache = null)
            : base(options, tenant, cache, logger)
        {
        }

        public DbSet<Application> Applications { get; set; }
        public DbSet<ApplicationLog> ApplicationLogs { get; set; }


        public DbSet<AppSetting> AppSettings { get; set; }
        public DbSet<AppSettingProperty> AppSettingProperties { get; set; }

        public DbSet<AppGit> AppGits { get; set; }
        public DbSet<AppGitBranch> AppGitBranches { get; set; }
        public DbSet<AppGitUser> AppGitUsers { get; set; }
        public DbSet<DevTemplate> DevTemplates { get; set; }

        //public DbSet<AppApiPushLog> AppApiPushLogs { get; set; }

        //public DbSet<AppTargetApiSetting> AppTargetApiSettings { get; set; }
        //public DbSet<PushMapping> PushMappings { get; set; }
        //public DbSet<SecurityParameter> SecurityParameters { get; set; }

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

            modelBuilder.Entity<Application>().ToTable(Tables.Application, TenantName);
            modelBuilder.Entity<ApplicationLog>().ToTable(Tables.ApplicationLog, TenantName);

            modelBuilder.Entity<AppSetting>().ToTable(Tables.AppSetting, TenantName);
            modelBuilder.Entity<AppSettingProperty>().ToTable(Tables.AppSettingProperty, TenantName);

            modelBuilder.Entity<AppGit>().ToTable(Tables.AppGit, TenantName);
            modelBuilder.Entity<AppGitBranch>().ToTable(Tables.AppGitBranch, TenantName);
            modelBuilder.Entity<AppGitUser>().ToTable(Tables.AppGitUser, TenantName);
            modelBuilder.Entity<DevTemplate>().ToTable(Tables.DevTemplate, TenantName);

            //modelBuilder.Entity<Application>()
            //    .HasOne<AppTemplate>(b => b.AppTemplate)
            //    .WithOne(s => s.Application)
            //    .HasForeignKey<AppTemplate>(s => s.ApplicationId);

            //modelBuilder.Entity<AppApiPushSetting>().ToTable(Tables.AppApiPushSetting, TenantName);
            //modelBuilder.Entity<AppApiPushLog>().ToTable(Tables.AppApiPushLog, TenantName);
            //modelBuilder.Entity<AppTargetApiSetting>().ToTable(Tables.AppTargetApiSetting, TenantName);
            //modelBuilder.Entity<PushMapping>().ToTable(Tables.PushMapping, TenantName);
            //modelBuilder.Entity<SecurityParameter>().ToTable(Tables.SecurityParameter, TenantName);

            //modelBuilder.Entity<ApplicationBusiness>()
            //    .HasOne<AppApiPushSetting>(b => b.PushSetting)
            //    .WithOne(s => s.Business)
            //    .HasForeignKey<AppApiPushSetting>(s => s.BusinessId);

        }
    }
}
