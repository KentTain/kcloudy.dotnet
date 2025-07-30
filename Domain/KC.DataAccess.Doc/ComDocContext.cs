using KC.Framework.Util;
using KC.Database.EFRepository;
using KC.Framework.Tenant;
using KC.Framework.Extension;
using System;
using Microsoft.EntityFrameworkCore;
using KC.Model.Doc.Constants;
using KC.Model.Doc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections.Generic;

namespace KC.DataAccess.Doc
{
    public class ComDocContext : MultiTenantDataContext
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
        public ComDocContext(
            DbContextOptions<ComDocContext> options,
            Tenant tenant,
            IMemoryCache cache = null,
            //ICoreConventionSetBuilder builder = null,
            ILogger<ComDocContext> logger = null)
            : base(options, tenant, cache, logger)
        {
        }

        #region Doc
        public DbSet<DocTemplate> DocTemplates { get; set; }
        public DbSet<DocTemplateLog> DocTemplateLogs { get; set; }
        public DbSet<DocCategory> DocCategories { get; set; }
        public DbSet<DocumentInfo> DocumentInfos { get; set; }
        public DbSet<DocBackup> DocBackups { get; set; }
        public DbSet<DocumentLog> DocumentLogs { get; set; }
        #endregion

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
            
            modelBuilder.Entity<DocTemplate>().ToTable(Tables.DocTemplate, TenantName);
            modelBuilder.Entity<DocTemplateLog>().ToTable(Tables.DocTemplateLog, TenantName);
            modelBuilder.Entity<DocCategory>().ToTable(Tables.DocCategory, TenantName);
            modelBuilder.Entity<DocumentInfo>().ToTable(Tables.DocumentInfo, TenantName);
            modelBuilder.Entity<DocBackup>().ToTable(Tables.DocBackup, TenantName);
            modelBuilder.Entity<DocumentLog>().ToTable(Tables.DocumentLog, TenantName);

            //value conversions: https://docs.microsoft.com/en-us/ef/core/modeling/value-conversions
            var splitStringConverter = new ValueConverter<IEnumerable<string>, string>(v => string.Join(",", v), v => v.ArrayFromCommaDelimitedStrings());
            //modelBuilder
            //    .Entity<DocTemplate>()
            //    .Property(m => m.OrgIds)
            //    .HasConversion(splitStringConverter);
            //modelBuilder
            //     .Entity<DocTemplate>()
            //     .Property(m => m.RoleIds)
            //     .HasConversion(splitStringConverter);
            //modelBuilder
            //     .Entity<DocTemplate>()
            //     .Property(m => m.UserIds)
            //     .HasConversion(splitStringConverter);

            //modelBuilder
            //    .Entity<DocumentInfo>()
            //    .Property(m => m.OrgIds)
            //    .HasConversion(splitStringConverter);
            //modelBuilder
            //     .Entity<DocumentInfo>()
            //     .Property(m => m.RoleIds)
            //     .HasConversion(splitStringConverter);
            //modelBuilder
            //     .Entity<DocumentInfo>()
            //     .Property(m => m.UserIds)
            //     .HasConversion(splitStringConverter);

        }
    }
}
