using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using KC.Framework.Util;
using KC.Framework.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace KC.Database.EFRepository
{
    public abstract class MultiTenantSqlServerDatabaseInitializer<T> : Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<T> where T : MultiTenantDataContext
    {
        protected readonly IServiceProvider _serviceProvider;
        protected readonly IMemoryCache _cache;
        protected MultiTenantSqlServerDatabaseInitializer()
        {
            var services = new ServiceCollection()
                .AddMemoryCache()
                .AddEntityFrameworkSqlServer()
                //.AddScoped<IModelSource, TenantModelSource>()
                //.AddScoped<IHistoryRepository, TenantHistoryRepository>()
                //.AddScoped<IModelCacheKeyFactory, MultiTenantModelCacheKeyFactory>()
                .AddScoped<IMigrationsSqlGenerator, SqlServerSchemaAwareMigrationSqlGenerator>();
            services.Replace(ServiceDescriptor.Singleton<IModelCacheKeyFactory, MultiTenantModelCacheKeyFactory>());

            _serviceProvider = services.BuildServiceProvider();
            _cache = _serviceProvider.GetService<IMemoryCache>();
        }

        public T CreateDbContext(string[] args)
        {
            ////For Debug
            //if (System.Diagnostics.Debugger.IsAttached == false)
            //    System.Diagnostics.Debugger.Launch();

            var tenant = TenantConstant.TestTenantApiAccessInfo;
            if (args.Length != 0 
                && args.FirstOrDefault().Equals(TenantConstant.DbaTenantName, StringComparison.OrdinalIgnoreCase))
            {
                tenant = TenantConstant.DbaTenantApiAccessInfo;
            }
            return Create(tenant);
        }

        #region DbContextOptions
        protected DbContextOptions<T> GetCachedDbContextOptions(string tenantName, string connection, DatabaseType databaseType)
        {
            var assebmlyName = this.GetType().Assembly.GetName().Name;
            var dynamicCacheKey = assebmlyName + "-DbContextOptionsCacheKey-TenantName:" + tenantName;
#if DEBUG
            LogUtil.LogDebug("Initialize the DbContextOptions: " + tenantName + ", dynamicCacheKey: " + dynamicCacheKey + ", connection: " + connection);
#endif
            var cachedOptions = _cache.GetOrCreate(
                dynamicCacheKey,
                t =>
                {
                    var optionsBuilder = new DbContextOptionsBuilder<T>();
                    // Create the options
                    switch (databaseType)
                    {
                        case DatabaseType.MySql:
                            var serverVersion = new MySqlServerVersion(new Version(8, 0));
                            optionsBuilder.UseMySql(connection, serverVersion, opts =>
                            {
                                var tableName = assebmlyName.Split('.').LastOrDefault();
                                opts.MigrationsAssembly(assebmlyName);
                                opts.MigrationsHistoryTable(string.Format("__{0}MigrationsHistory", tableName), tenantName);
                            });
                            break;
                        default:
                            optionsBuilder.UseSqlServer(connection, opts =>
                            {
                                var tableName = assebmlyName.Split('.').LastOrDefault();
                                opts.MigrationsAssembly(assebmlyName);
                                opts.MigrationsHistoryTable(string.Format("__{0}MigrationsHistory", tableName), tenantName);
                            });
                            break;
                    }

                    optionsBuilder.UseInternalServiceProvider(_serviceProvider);
#if DEBUG
                    optionsBuilder.EnableSensitiveDataLogging(true);
#endif

                    ////Error: Cannot create a DbSet for 'TenantUser' because this type is not included in the model for the context
                    //var conventions = Microsoft.EntityFrameworkCore.Metadata.Conventions.SqlServerConventionSetBuilder.Build();
                    //var modelBuilder = new ModelBuilder(conventions);

                    //var builder = _serviceProvider.GetService<ICoreConventionSetBuilder>();
                    //var modelBuilder = new ModelBuilder(builder.CreateConventionSet());

                    //MultiTenantDataContext.BuildModel(modelBuilder, tenantName);
                    //optionsBuilder.UseModel(modelBuilder.Model);

                    var options = optionsBuilder.Options;

                    // Ensure the schema is up to date
                    //using (var context = new MultiTenantDataContext(tenant, options))
                    //{
                    //    context.Database.Migrate();
                    //}

                    return options;
                });

            return cachedOptions;
        }

        #endregion

        #region 数据库初始化

        /// <summary>
        /// 单个租户的数据库初始化
        /// </summary>
        /// <param name="tenant">当个租户Tenant</param>
        public void Initialize(Tenant tenant)
        {
            try
            {
                using (var context = Create(tenant))
                {
                    //var migrator = context.GetInfrastructure().GetRequiredService<IMigrator>();
                    //migrator.Migrate();
                    // Ensure the schema is up to date
                    context.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
            }
        }
        /// <summary>
        /// 多个租户的数据库初始化
        /// </summary>
        /// <param name="tenants">租户列表</param>
        public void Initialize(List<Tenant> tenants)
        {
            //TenantSet.Clear();
            //InitialTenant();

            foreach (var tenant in tenants.ToList())
            {
                Initialize(tenant);
            }
        }
        #endregion

        #region 回滚数据库
        /// <summary>
        /// 单个租户的回滚数据库
        /// </summary>
        /// <param name="tenant">当个租户Tenant</param>
        public void RollBack(Tenant tenant)
        {
            var tenantName = tenant.TenantName;
            //LogUtil.LogDebug(string.Format("---Tenant({0})回滚数据库（{1}）--连接字符串（{2}）。", tenantName, tenantName + "." + typeof(T).Namespace, tenant.ConnectionString));
            using (var context = Create(tenant))
            {
                var migrator = context.GetInfrastructure().GetRequiredService<IMigrator>();
                var targetMigration = GetTargetMigration();
                migrator.Migrate(targetMigration);
            }
        }
        /// <summary>
        /// 多个租户的回滚数据库
        /// </summary>
        /// <param name="tenants">租户列表</param>
        public void RollBack(List<Tenant> tenants)
        {
            foreach (var tenant in tenants.ToList())
            {
                RollBack(tenant);
            }
        }
        /// <summary>
        /// 多个租户的回滚数据库至目标数据库迁移
        /// </summary>
        /// <param name="tenants">租户列表</param>
        /// <param name="targetMigration">目标数据库迁移</param>
        public void RollBackByMigration(List<Tenant> tenants, string targetMigration)
        {
            if (string.IsNullOrWhiteSpace(targetMigration)) return;
            
            foreach (var tenant in tenants.ToList())
            {
                var tenantName = tenant.TenantName;
                //LogUtil.LogDebug(string.Format("---Tenant({0})回滚数据库（{1}）--连接字符串（{2}）。", tenantName, tenantName + "." + typeof(T).Namespace, tenant.ConnectionString));
                using (var context = Create(tenant))
                {
                    var migrator = context.GetInfrastructure().GetRequiredService<IMigrator>();
                    migrator.Migrate(targetMigration);
                }
            }
        }
        #endregion

        protected abstract string GetTargetMigration();
        public abstract T Create(Tenant tenant);
        //protected abstract T Create(string tenantName, string connectionString);
    }
}
