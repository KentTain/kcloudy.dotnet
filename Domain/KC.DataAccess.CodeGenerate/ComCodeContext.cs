using KC.Framework.Util;
using KC.Database.EFRepository;
using KC.Framework.Tenant;
using System;
using Microsoft.EntityFrameworkCore;
using KC.Model.CodeGenerate.Constants;
using KC.Model.CodeGenerate;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace KC.DataAccess.CodeGenerate
{
    public class ComCodeContext : MultiTenantDataContext
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
        public ComCodeContext(
            DbContextOptions options,
            Tenant tenant,
            ////ICoreConventionSetBuilder builder = null,
            ILogger<ComCodeContext> logger = null,
            IMemoryCache cache = null)
            : base(options, tenant, cache, logger)
        {
        }

        public DbSet<ModelCategory> ModelCategories { get; set; }
        public DbSet<ModelChangeLog> ModelChangeLogs { get; set; }

        public DbSet<ModelDefinition> ModelDefinitions { get; set; }
        public DbSet<ModelDefField> ModelDefFields { get; set; }

        public DbSet<RelationDefinition> RelationDefinitions { get; set; }
        public DbSet<RelationDefDetail> RelationDefDetails { get; set; }


        public DbSet<ApiDefinition> ApiDefinitions { get; set; }
        public DbSet<ApiInputParam> ApiInputParams { get; set; }
        public DbSet<ApiOutParam> ApiOutParams { get; set; }


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

            modelBuilder.Entity<ModelCategory>().ToTable(Tables.ModelCategory, TenantName);
            modelBuilder.Entity<ModelChangeLog>().ToTable(Tables.ModelChangeLog, TenantName);

            modelBuilder.Entity<ModelDefinition>().ToTable(Tables.ModelDefinition, TenantName);
            modelBuilder.Entity<ModelDefField>().ToTable(Tables.ModelDefField, TenantName);

            modelBuilder.Entity<RelationDefinition>().ToTable(Tables.RelationDefinition, TenantName);
            modelBuilder.Entity<RelationDefDetail>().ToTable(Tables.RelationDefDetail, TenantName);


            modelBuilder.Entity<ApiDefinition>().ToTable(Tables.ApiDefinition, TenantName);
            modelBuilder.Entity<ApiInputParam>().ToTable(Tables.ApiInputParam, TenantName);
            modelBuilder.Entity<ApiOutParam>().ToTable(Tables.ApiOutParam, TenantName);

        }
    }
}
