using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Framework.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace KC.Database.EFRepository
{
    public abstract class MultiTenantDataContext : DbContext
    {
        public static readonly DatabaseType DefaultDatabaseType = TenantConstant.DefaultDatabaseType;
        /// <summary>
        /// 默认的链接字符串：Server=;Database=;User ID=;Password=;MultipleActiveResultSets=true;
        /// </summary>
        public static readonly string DefaultConnectionString = TenantConstant.DefaultDatabaseConnectionString;
        /// <summary>
        /// 默认的租户：cTest
        /// </summary>
        public static readonly string DefaultTenantName = TenantConstant.TestTenantName;
        /// <summary>
        /// Migration表名格式：__{0}MigrationsHistory
        /// </summary>
        public static readonly string DefaultMigrationsHistoryTable = "__{0}MigrationsHistory";

        public string TenantName { get; private set; }
        public string ConnectionString { get; private set; }
        public DatabaseType DatabaseType { get; private set; }

        protected readonly IMemoryCache _cache;
        //protected readonly IConventionSetBuilder _builder;
        protected readonly ILogger _logger;

        #region Construction

        //public MultiTenantDataContext(DbContextOptions options)
        //    : base(options)
        //{
        //    ////For Debug
        //    //if (System.Diagnostics.Debugger.IsAttached == false)
        //    //    System.Diagnostics.Debugger.Launch();

        //    TenantName = DefaultTenantName;
        //    ConnectionString = DefaultConnectionString;
        //    DatabaseType = DefaultDatabaseType;
        //}

        public MultiTenantDataContext(
            DbContextOptions options,
            Tenant tenant = null,
            IMemoryCache cache = null,
            //IConventionSetBuilder builder = null,
            ILogger logger = null)
            : base(options)
        {
            //Database.SetInitializer<ComAccountContext>(null);
            TenantName = tenant == null ? MultiTenantDataContext.DefaultTenantName : tenant.TenantName;
            ConnectionString = tenant == null ? MultiTenantDataContext.DefaultConnectionString : tenant.ConnectionString;
            DatabaseType = tenant == null ? MultiTenantDataContext.DefaultDatabaseType : tenant.DatabaseType;

            //_builder = builder;
            _cache = cache;
            _logger = logger;

            //LogUtil.LogDebug("----MultiTenantDataContext with Tenant: " + this.TenantName + ", Connection: " + ConnectionString);
        }

        #endregion

        #region Overrides

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Assembly assembly = this.GetType().Assembly;
            var assebmlyName = assembly.GetName().Name;
            var tableName = assebmlyName.Split('.').LastOrDefault();
            //解决跟踪同一个ID问题
            //optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            switch (DatabaseType)
            {
                case DatabaseType.MySql:
                    var serverVersion = new MySqlServerVersion(new Version(8, 0));
                    optionsBuilder.UseMySql(ConnectionString, serverVersion, opts =>
                    {
                        opts.MigrationsAssembly(assebmlyName);
                        opts.MigrationsHistoryTable(
                            string.Format(DefaultMigrationsHistoryTable, tableName),
                            TenantName);
                    });

                    if (_cache != null)
                    {
                        //LogUtil.LogDebug(assebmlyName + "----OnConfiguring with Tenant: " + this.TenantName + ", Connection: " + ConnectionString);

                        var dynamicCacheKey = assebmlyName + "-DynamicModelCacheKey-TenantName:" + TenantName;
                        //直接使用缓存中的model，如果不存在再build
                        var model = _cache.GetOrCreate(dynamicCacheKey,
                            entry =>
                            {
                                var conventionSet = MySqlConventionSetBuilder.Build();
                                //var conventionSet = _builder.CreateConventionSet();
                                var modelBuilder = new ModelBuilder(conventionSet);
                                OnModelCreating(modelBuilder);

                                // fixed a bug: System.ArgumentNullException: Value cannot be null.  Parameter name: relationalTypeMapping
                                // https://github.com/aspnet/EntityFrameworkCore/issues/11738
                                modelBuilder.ApplyConfigurationsFromAssembly(assembly);
                                modelBuilder.FinalizeModel();

                                return modelBuilder.Model;
                            });

                        //动态模型注册，替代OnModelCreating
                        optionsBuilder.UseModel(model);
                    }
                    break;
                default:
                    optionsBuilder.UseSqlServer(ConnectionString, opts =>
                    {
                        opts.MigrationsAssembly(assebmlyName);
                        opts.MigrationsHistoryTable(
                            string.Format(DefaultMigrationsHistoryTable, tableName),
                            TenantName);
                    });

                    if (_cache != null)
                    {
                        //LogUtil.LogDebug(assebmlyName + "----OnConfiguring with Tenant: " + this.TenantName + ", Connection: " + ConnectionString);

                        var dynamicCacheKey = assebmlyName + "-DynamicModelCacheKey-TenantName:" + TenantName;
                        //直接使用缓存中的model，如果不存在再build
                        var model = _cache.GetOrCreate(dynamicCacheKey,
                            entry =>
                            {
                                var conventionSet = SqlServerConventionSetBuilder.Build();
                                //var conventionSet = _builder.CreateConventionSet();
                                var modelBuilder = new ModelBuilder(conventionSet);
                                OnModelCreating(modelBuilder);

                                // fixed a bug: System.ArgumentNullException: Value cannot be null.  Parameter name: relationalTypeMapping
                                // https://github.com/aspnet/EntityFrameworkCore/issues/11738
                                modelBuilder.ApplyConfigurationsFromAssembly(assembly);
                                modelBuilder.FinalizeModel();

                                return modelBuilder.Model;
                            });

                        //动态模型注册，替代OnModelCreating
                        optionsBuilder.UseModel(model);
                    }
                    break;
            }

            base.OnConfiguring(optionsBuilder);
#if DEBUG
            optionsBuilder.EnableSensitiveDataLogging(true);
#endif
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException("modelBuilder");
            }

            //移除一对多的级联删除约定，想要级联删除可以在 EntityTypeConfiguration<TEntity>的实现类中进行控制
            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            //多对多启用级联删除约定，不想级联删除可以在删除前判断关联的数据进行拦截
            //modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            //modelBuilder.Properties<decimal>().Configure(config => config.HasPrecision(18, 4));

            //foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            //{
            //    relationship.DeleteBehavior = DeleteBehavior.Restrict;
            //}

            //LogUtil.LogDebug("----OnModelCreating with Tenant: " + this.TenantName + ", Connection: " + ConnectionString);

            if (!string.IsNullOrEmpty(this.TenantName))
            {
                modelBuilder.HasDefaultSchema(this.TenantName);
            }

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            //ChangeTracker.DetectChanges();
            var entries = ChangeTracker.Entries().Where(entry => entry.Entity is Entity);
            foreach (var entry in entries)
            {
                var entity = (Entity)entry.Entity;
                if (entry.State == EntityState.Added)
                {
                    entity.CreatedBy = 
                        string.IsNullOrWhiteSpace(entity.CreatedBy) && Thread.CurrentPrincipal != null 
                        ? Thread.CurrentPrincipal.Identity.Name 
                        : entity.CreatedBy;
                    entity.CreatedDate = entity.CreatedDate == DateTime.MinValue || entity.CreatedDate == null
                        ? DateTime.UtcNow 
                        : entity.CreatedDate;
                    //entity.CreatedDate = DateTime.UtcNow;
                    entity.ModifiedBy = 
                        string.IsNullOrWhiteSpace(entity.CreatedBy) && Thread.CurrentPrincipal != null 
                        ? Thread.CurrentPrincipal.Identity.Name 
                        : entity.CreatedBy;
                    entity.ModifiedDate = entity.ModifiedDate == DateTime.MinValue || entity.ModifiedDate == null
                        ? DateTime.UtcNow
                        : entity.ModifiedDate;
                }

                if (entry.State == EntityState.Modified)
                {
                    entity.ModifiedBy = 
                        string.IsNullOrWhiteSpace(entity.CreatedBy) && Thread.CurrentPrincipal != null 
                        ? Thread.CurrentPrincipal.Identity.Name 
                        : entity.CreatedBy;
                    entity.ModifiedDate = DateTime.UtcNow;
                }
            }

            return base.SaveChanges();
        }

        #endregion

        //public void MergeContextByAssembly(ModelBuilder modelBuilder, List<string> modelAssembly, List<string> dataAccessAssembly)
        //{
        //    string path = AppDomain.CurrentDomain.BaseDirectory + @"/bin/";
        //    foreach (var assemblyName in dataAccessAssembly)
        //    {
        //        Assembly assembly = Assembly.LoadFrom(path + assemblyName + ".dll");
        //        modelBuilder.Configurations.AddFromAssembly(assembly);
        //    }
        //    var entityType = typeof(EntityBase);
        //    foreach (var assemblyName in modelAssembly)
        //    {
        //        Assembly assembly = Assembly.LoadFrom(path + assemblyName + ".dll");
        //        Type[] ts = assembly.GetTypes();
        //        foreach (var type in ts)
        //        {
        //            if (type.IsSubclassOf(entityType) && type.GetCustomAttributes(typeof(NotMappedAttribute), true).Length == 0)
        //            {
        //                modelBuilder.RegisterEntityType(type);
        //            }
        //        }
        //    }
        //}
    }
}
