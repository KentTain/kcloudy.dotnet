using System;
using KC.Framework.Tenant;
using KC.Model.Blog;
using KC.Model.Blog.Constants;
using KC.Database.EFRepository;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace KC.DataAccess.Blog
{
    public class ComBlogContext : MultiTenantDataContext
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
        public ComBlogContext(
            DbContextOptions<ComBlogContext> options,
            //Tenant tenant,
            IMemoryCache cache = null,
            ////ICoreConventionSetBuilder builder = null,
            ILogger<ComBlogContext> logger = null)
            : base(options, TenantConstant.DbaTenantApiAccessInfo, cache, logger)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Model.Blog.Blog> Blogs { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Setting> Setting { get; set; }

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

            modelBuilder.Entity<Category>().ToTable(Tables.Category, TenantName); ;
            modelBuilder.Entity<Model.Blog.Blog>().ToTable(Tables.Blog, TenantName);
            modelBuilder.Entity<Comment>().ToTable(Tables.Comment, TenantName);
            modelBuilder.Entity<Setting>().ToTable(Tables.Setting, TenantName);
        }
    }
}
