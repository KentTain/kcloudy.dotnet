using KC.Database.EFRepository;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Framework.Util;
using KC.Model.Account;
using KC.Model.Account.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace KC.DataAccess.Account
{
    public class ComAccountContext
        : IdentityDbContext<User, Role, string, IdentityUserClaim<string>, UsersInRoles, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public string ConnectionString { get; private set; }
        public string TenantName { get; private set; }
        public DatabaseType DatabaseType { get; private set; }

        private readonly IMemoryCache _cache;
        //private readonly IConventionSetBuilder _builder;
        private readonly ILogger<ComAccountContext> _logger;

        /// <summary>
        /// Only used by migrations
        /// </summary>
        /// <param name="options"></param>
        //public ComAccountContext(DbContextOptions<ComAccountContext> options)
        //    : base(options)
        //{
        //    ////For Debug
        //    //if (System.Diagnostics.Debugger.IsAttached == false)
        //    //    System.Diagnostics.Debugger.Launch();

        //    TenantName = MultiTenantDataContext.DefaultTenantName;
        //    ConnectionString = MultiTenantDataContext.DefaultConnectionString;
        //    DatabaseType = MultiTenantDataContext.DefaultDatabaseType;
        //}

        public ComAccountContext(
            DbContextOptions<ComAccountContext> options,
            Tenant tenant,
            IMemoryCache cache,
            //IConventionSetBuilder builder,
            ILogger<ComAccountContext> logger)
            : base(options)
        {
            ////For Debug
            //if (System.Diagnostics.Debugger.IsAttached == false)
            //    System.Diagnostics.Debugger.Launch();

            //Database.SetInitializer<ComAccountContext>(null);
            TenantName = tenant == null ? MultiTenantDataContext.DefaultTenantName : tenant.TenantName;
            ConnectionString = tenant == null ? MultiTenantDataContext.DefaultConnectionString : tenant.ConnectionString;
            DatabaseType = tenant == null ? MultiTenantDataContext.DefaultDatabaseType : tenant.DatabaseType;

            //_builder = builder;
            _cache = cache;
            _logger = logger;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Assembly assembly = this.GetType().Assembly;
            var assebmlyName = assembly.GetName().Name;
            var tableName = assebmlyName.Split('.').LastOrDefault();
#if DEBUG
            LogUtil.LogDebug(assebmlyName + "----OnConfiguring with Tenant: " + this.TenantName + ", Connection: " + ConnectionString);
#endif
            switch (DatabaseType)
            {
                case DatabaseType.MySql:
                    var serverVersion = new MySqlServerVersion(new Version(8, 0));
                    optionsBuilder
                        .UseMySql(ConnectionString, serverVersion, opts =>
                        {
                            opts.MigrationsAssembly(assebmlyName);
                            opts.MigrationsHistoryTable(
                                string.Format(MultiTenantDataContext.DefaultMigrationsHistoryTable, tableName),
                                TenantName);
                        });
                    if (_cache != null)
                    {
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
                    optionsBuilder
                        .UseSqlServer(ConnectionString, opts =>
                        {
                            opts.MigrationsAssembly(assebmlyName);
                            opts.MigrationsHistoryTable(
                                string.Format(MultiTenantDataContext.DefaultMigrationsHistoryTable, tableName),
                                TenantName);
                        });
                    if (_cache != null)
                    {
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
        }

        #region Account
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<MenuNode> MenuNodes { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserTracingLog> UserTracingLogs { get; set; }

        public DbSet<UserLoginLog> UserLoginLogs { get; set; }

        public DbSet<UsersInOrganizations> UsersInOrganizations { get; set; }
        public DbSet<MenuNodesInRoles> MenuNodesInRoles { get; set; }
        public DbSet<PermissionsInRoles> PermissionsInRoles { get; set; }
        #endregion

        #region  System Setting 
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<SystemSettingProperty> SystemSettingProperties { get; set; }

        #endregion

        #region  User Setting 
        public DbSet<UserSetting> UserSettings { get; set; }
        public DbSet<UserSettingProperty> UserSettingProperties { get; set; }

        #endregion

        //public DbSet<SeedEntity> SeedEntities { set; get; }

        //初始化时，只执行一次，如果需要使用动态模型，使用OnConfiguring中的optionsBuilder.UseModel(model)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException("modelBuilder");
            }

            //base.OnModelCreating(modelBuilder);

            //_logger.LogDebug("----OnModelCreating: " + this.TenantName);

            //移除一对多的级联删除约定，想要级联删除可以在 EntityTypeConfiguration<TEntity>的实现类中进行控制
            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            //多对多启用级联删除约定，不想级联删除可以在删除前判断关联的数据进行拦截
            //modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            //modelBuilder.Properties<decimal>().Configure(config => config.HasPrecision(18, 4));

            //foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            //{
            //    relationship.DeleteBehavior = DeleteBehavior.Restrict;
            //}

            if (!string.IsNullOrEmpty(TenantName))
            {
                modelBuilder.HasDefaultSchema(TenantName);
            }

            modelBuilder.Entity<Organization>().ToTable(Tables.Organization, TenantName);
            modelBuilder.Entity<MenuNode>().ToTable(Tables.MenuNode, TenantName);
            modelBuilder.Entity<Permission>().ToTable(Tables.Permission, TenantName);
            modelBuilder.Entity<UsersInOrganizations>().ToTable(Tables.UsersInOrganizations, TenantName);
            modelBuilder.Entity<MenuNodesInRoles>().ToTable(Tables.MenuNodesInRoles, TenantName);
            modelBuilder.Entity<PermissionsInRoles>().ToTable(Tables.PermissionsInRoles, TenantName);
            modelBuilder.Entity<UserTracingLog>().ToTable(Tables.UserTracingLog, TenantName);
            modelBuilder.Entity<UserLoginLog>().ToTable(Tables.UserLoginLog, TenantName);

            modelBuilder.Entity<SystemSetting>().ToTable(Tables.SystemSetting, TenantName);
            modelBuilder.Entity<SystemSettingProperty>().ToTable(Tables.SystemSettingProperty, TenantName);

            modelBuilder.Entity<UserSetting>().ToTable(Tables.UserSetting, TenantName);
            modelBuilder.Entity<UserSettingProperty>().ToTable(Tables.UserSettingProperty, TenantName);

            modelBuilder.Entity<User>()
                .ToTable(Tables.User, TenantName)
                .HasMany<IdentityUserClaim<string>>()
                .WithOne()
                .HasForeignKey(uc => uc.UserId)
                .IsRequired();
            modelBuilder.Entity<User>()
                .HasMany<IdentityUserLogin<string>>()
                .WithOne()
                .HasForeignKey(ul => ul.UserId)
                .IsRequired();
            modelBuilder.Entity<User>()
                .HasMany<IdentityUserToken<string>>()
                .WithOne()
                .HasForeignKey(ut => ut.UserId)
                .IsRequired();

            modelBuilder.Entity<Role>()
                .ToTable(Tables.Role, TenantName)
                .HasMany<IdentityRoleClaim<string>>()
                .WithOne()
                .HasForeignKey(rc => rc.RoleId)
                .IsRequired();
            
            modelBuilder.Entity<IdentityRoleClaim<string>>()
                .ToTable(Tables.RoleClaim, TenantName)
                .HasKey(m => m.Id);

            modelBuilder.Entity<IdentityUserLogin<string>>()
                .ToTable(Tables.UserLogin, TenantName)
                .HasKey(l => new { l.LoginProvider, l.ProviderKey });

            modelBuilder.Entity<IdentityUserClaim<string>>()
                .ToTable(Tables.UserClaim, TenantName)
                .HasKey(m => m.Id);

            modelBuilder.Entity<IdentityUserToken<string>>()
                .ToTable(Tables.UserToken, TenantName)
                .HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

            modelBuilder.Entity<User>()
                .ToTable(Tables.User, TenantName)
                .HasMany(m => m.UserRoles)
                .WithOne()
                .HasForeignKey(m => m.UserId);
            modelBuilder.Entity<Role>()
                .ToTable(Tables.Role, TenantName)
                .HasMany(m => m.RoleUsers)
                .WithOne()
                .HasForeignKey(m => m.RoleId);

            modelBuilder.Entity<UsersInRoles>()
                .ToTable(Tables.UsersInRoles, TenantName)
                .HasKey(t => new { t.RoleId, t.UserId });
            modelBuilder.Entity<UsersInRoles>()
                .ToTable(Tables.UsersInRoles, TenantName)
                .HasOne(m => m.User)
                .WithMany(m => m.UserRoles)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<UsersInRoles>()
                .ToTable(Tables.UsersInRoles, TenantName)
                .HasOne(m => m.Role)
                .WithMany(m => m.RoleUsers)
                .HasForeignKey(m => m.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UsersInOrganizations>()
                .ToTable(Tables.UsersInOrganizations, TenantName)
                .HasKey(t => new { t.OrganizationId, t.UserId });
            modelBuilder.Entity<UsersInOrganizations>()
                .ToTable(Tables.UsersInOrganizations, TenantName)
                .HasOne(m => m.User)
                .WithMany(m => m.UserOrganizations)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<UsersInOrganizations>()
                .ToTable(Tables.UsersInOrganizations, TenantName)
                .HasOne(m => m.Organization)
                .WithMany(m => m.OrganizationUsers)
                .HasForeignKey(m => m.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MenuNodesInRoles>()
                .ToTable(Tables.MenuNodesInRoles, TenantName)
                .HasKey(t => new { t.RoleId, t.MenuNodeId });
            modelBuilder.Entity<MenuNodesInRoles>()
                .ToTable(Tables.MenuNodesInRoles, TenantName)
                .HasOne(m => m.MenuNode)
                .WithMany(m => m.MenuRoles)
                .HasForeignKey(m => m.MenuNodeId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MenuNodesInRoles>()
                .ToTable(Tables.MenuNodesInRoles, TenantName)
                .HasOne(m => m.Role)
                .WithMany(m => m.RoleMenuNodes)
                .HasForeignKey(m => m.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PermissionsInRoles>()
                .ToTable(Tables.PermissionsInRoles, TenantName)
                .HasKey(t => new { t.RoleId, t.PermissionId });
            modelBuilder.Entity<PermissionsInRoles>()
                .ToTable(Tables.PermissionsInRoles, TenantName)
                .HasOne(m => m.Permission)
                .WithMany(m => m.PermissionRoles)
                .HasForeignKey(m => m.PermissionId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PermissionsInRoles>()
                .ToTable(Tables.PermissionsInRoles, TenantName)
                .HasOne(m => m.Role)
                .WithMany(m => m.RolePermissions)
                .HasForeignKey(m => m.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (!(entry.Entity is Entity))
                {
                    continue;
                }

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
    }

    public class ComAccountMigrationsSqlGenerator : SqlServerSchemaAwareMigrationSqlGenerator
    {
        private readonly ILogger<ComAccountMigrationsSqlGenerator> _logger;
        public ComAccountMigrationsSqlGenerator(
            MigrationsSqlGeneratorDependencies dependencies,
            IRelationalAnnotationProvider migrationsAnnotations,
            ICurrentDbContext context,
            ILogger<ComAccountMigrationsSqlGenerator> logger = null)
        : base(dependencies, migrationsAnnotations, context)
        {
            _schema = ((ComAccountContext)context.Context)?.TenantName;

            //_logger.LogDebug("----ComAccountMigrationsSqlGenerator: " + this._schema);

        }
    }

    public class ComAccountModelCacheKeyFactory : MultiTenantModelCacheKeyFactory
    {
        private readonly ILogger<ComAccountModelCacheKeyFactory> _logger;
        public ComAccountModelCacheKeyFactory(
            ModelCacheKeyFactoryDependencies dependencies,
            ILogger<ComAccountModelCacheKeyFactory> logger = null)
            : base(dependencies)
        {
        }
        public override object Create(DbContext context)
        {
            this._schemaName = (context as ComAccountContext)?.TenantName;

            //_logger.LogDebug("----ComAccountModelCacheKeyFactory: " + this._schemaName);

            return new ComAccountModelCacheKey(context);
        }
    }

    public class ComAccountModelCacheKey : MultiTenantModelCacheKey
    {
        public ComAccountModelCacheKey(DbContext context)
            : base(context)
        {
        }
    }
}
