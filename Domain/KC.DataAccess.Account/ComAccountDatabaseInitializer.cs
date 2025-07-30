using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
using System.Linq;

using KC.Database.EFRepository;
using KC.Framework.Tenant;
using KC.Framework.Util;
using KC.Model.Account;

namespace KC.DataAccess.Account
{
    public class ComAccountDatabaseInitializer: Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<ComAccountContext>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMemoryCache _cache;

        public ComAccountDatabaseInitializer()
        {
            var services = new ServiceCollection()
                .AddMemoryCache()
                .AddEntityFrameworkSqlServer()
                //.AddScoped<IHistoryRepository, TenantHistoryRepository>()
                //.AddScoped<IModelSource, TenantModelSource>()
                .AddScoped<IPasswordHasher<User>, PasswordHasher<User>>()
                .AddScoped<IMigrationsSqlGenerator, ComAccountMigrationsSqlGenerator>()
                .AddScoped<IModelCacheKeyFactory, ComAccountModelCacheKeyFactory>();

            _serviceProvider = services.BuildServiceProvider();
            _cache = _serviceProvider.GetService<IMemoryCache>();
        }

        #region 创建DbContext
        public ComAccountContext CreateDbContext(string[] args)
        {
            ////For Debug
            //if (System.Diagnostics.Debugger.IsAttached == false)
            //    System.Diagnostics.Debugger.Launch();

            var tenant = TenantConstant.TestTenantApiAccessInfo;
            return Create(tenant);
        }

        public ComAccountContext Create(Tenant tenant)
        {
            if (tenant == null)
                throw new System.ArgumentNullException("Tenant is null", "tenant");
            if (string.IsNullOrEmpty(tenant.TenantName))
                throw new System.ArgumentException("tenantName is null or empty", "tenantName");
            if (string.IsNullOrEmpty(tenant.ConnectionString))
                throw new System.ArgumentException("connectionString is null or empty", "connectionString");

            //LogUtil.LogDebug(string.Format("---{0}---ComAccountDatabaseInitializer: {1}", tenant.TenantName, tenant.ConnectionString));

            var options = GetCachedDbContextOptions(tenant.TenantName, tenant.ConnectionString, tenant.DatabaseType);
            return new ComAccountContext(options, tenant, null, null);
        }

        #region 初始化DbContextOptions
        protected DbContextOptions<ComAccountContext> GetCachedDbContextOptions(string tenantName, string connection, DatabaseType databaseType)
        {
            var assebmlyName = this.GetType().Assembly.GetName().Name;
            var dynamicCacheKey = assebmlyName + "-DbContextOptionsCacheKey-TenantName:" + tenantName;
#if DEBUG
            LogUtil.LogDebug("----Initialize the DbContextOptions: " + tenantName + ", dynamicCacheKey: " + dynamicCacheKey + ", connection: " + connection);
#endif
            var cachedOptions = _cache.GetOrCreate(
                dynamicCacheKey,
                t =>
                {
                    // Create the options
                    var optionsBuilder = new DbContextOptionsBuilder<ComAccountContext>();
                    switch (databaseType)
                    {
                        case DatabaseType.MySql:
                            var serverVersion = new MySqlServerVersion(new Version(8, 0));
                            optionsBuilder.UseMySql(connection, serverVersion, opts =>
                            {
                                var tableName = assebmlyName.Split('.').LastOrDefault();
                                opts.MigrationsAssembly(assebmlyName);
                                opts.MigrationsHistoryTable(string.Format(MultiTenantDataContext.DefaultMigrationsHistoryTable, tableName), tenantName);
                            });
                            break;
                        default:
                            optionsBuilder.UseSqlServer(connection, opts =>
                            {
                                var tableName = assebmlyName.Split('.').LastOrDefault();
                                opts.MigrationsAssembly(assebmlyName);
                                opts.MigrationsHistoryTable(string.Format(MultiTenantDataContext.DefaultMigrationsHistoryTable, tableName), tenantName);
                            });
                            break;
                    }
                    
                    optionsBuilder.UseInternalServiceProvider(_serviceProvider);
#if DEBUG
                    optionsBuilder.EnableSensitiveDataLogging(true);
#endif
                    //var conventions = SqlServerConventionSetBuilder.Build();
                    //var modelBuilder = new ModelBuilder(conventions);

                    //var builder = _serviceProvider.GetService<ICoreConventionSetBuilder>();
                    //var modelBuilder = new ModelBuilder(builder.CreateConventionSet());

                    //ComAccountContext.BuildModel(modelBuilder, tenantName);
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

        #endregion

        #region 初始化数据库
        /// <summary>
        /// 数据库初始化
        /// </summary>
        public void Initialize(Tenant tenant)
        {
            //TenantSet.Clear();
            //InitialTenant();
            try
            {
                using (var context = Create(tenant))
                {
                    // Ensure the schema is up to date
                    context.Database.Migrate();

                    //SeedData(tenant, context);
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);

            }
        }
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
        /// 回滚数据库
        /// </summary>
        public void RollBack(Tenant tenant)
        {
            //TenantSet.Clear();
            //InitialTenant();

            var tenantName = tenant.TenantName;
            //LogUtil.LogDebug(string.Format("---Tenant({0})回滚数据库（{1}）--连接字符串（{2}）。", tenantName, tenantName + "." + typeof(T).Namespace, tenant.ConnectionString));
            var targetMigration = DataInitial.DBSqlInitializer.GetPreMigrationVersion();
            using (var context = Create(tenant))
            {
                var migrator = context.GetInfrastructure().GetRequiredService<IMigrator>();
                migrator.Migrate(targetMigration);
            }
        }
        public void RollBack(List<Tenant> tenants)
        {
            //TenantSet.Clear();
            //InitialTenant();

            foreach (var tenant in tenants.ToList())
            {
                RollBack(tenant);
            }
        }
        public void RollBackByMigration(List<Tenant> tenants, string targetMigration)
        {
            if (string.IsNullOrWhiteSpace(targetMigration)) return;

            //TenantSet.Clear();
            //InitialTenant();

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

        //#region 初始化数据
        //public void SeedData(List<Tenant> tenants)
        //{
        //    //TenantSet.Clear();
        //    //InitialTenant();

        //    foreach (var tenant in tenants.ToList())
        //    {
        //        SeedData(tenant);
        //    }
        //}
        //public void SeedData(Tenant tenant)
        //{
        //    #region 初始化Admin用户及相关的角色
        //    using (var scope = _serviceProvider.CreateScope())
        //    {
        //        var PasswordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        //        var role = new Role()
        //        {
        //            Id = RoleConstants.AdminRoleId,
        //            Name = "Admin",
        //            NormalizedName = "admin",
        //            IsSystemRole = true,
        //            DisplayName = "系统管理员",
        //        };

        //        var user = new User
        //        {
        //            Id = RoleConstants.AdminUserId,
        //            UserName = "admin",
        //            Email = "tianchangjun@outlook.com",
        //            MemberId = "UID00001",
        //            DisplayName = "系统管理员",
        //            PhoneNumber = "",
        //            Status = Enums.Core.WorkFlowStatus.Approved,
        //        };

        //        var userInRole = new UsersInRoles()
        //        {
        //            //Role = role,
        //            //User = user,
        //            RoleId = RoleConstants.AdminRoleId,
        //            UserId = RoleConstants.AdminUserId,
        //        };

        //        var hash = PasswordHasher.HashPassword(user, "123456");
        //        var stamp = NewSecurityStamp();
        //        user.PasswordHash = hash;
        //        user.SecurityStamp = stamp;

        //        using (var context = Create(tenant))
        //        {
        //            using (var transaction = context.Database.BeginTransaction())
        //            {
        //                try
        //                {
        //                    //context.Roles.Add(role);
        //                    //context.Users.Add(user);
        //                    context.AddOrUpdate(role);
        //                    context.AddOrUpdate(user);
        //                    context.SaveChanges();

        //                    //context.UserRoles.Add(userInRole);
        //                    context.AddOrUpdate(userInRole);
        //                    context.SaveChanges();

        //                    transaction.Commit();
        //                }
        //                catch (Exception ex)
        //                {
        //                    transaction.Rollback();
        //                    LogUtil.LogFatal(
        //                        string.Format("-------To initialize the admin user is failed. \r\nMessage: {0}. \r\nStackTrace: {1}",
        //                        ex.InnerException != null ? ex.InnerException.Message : ex.Message,
        //                        ex.InnerException != null ? ex.InnerException.StackTrace : ex.StackTrace));
        //                }
        //            }

        //        }
        //    }
        //    #endregion
        //}

        //private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();
        //private static string NewSecurityStamp()
        //{
        //    byte[] bytes = new byte[20];
        //    _rng.GetBytes(bytes);
        //    return Base32.ToBase32(bytes);
        //}
        //internal static class Base32
        //{
        //    private static readonly string _base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        //    public static string ToBase32(byte[] input)
        //    {
        //        if (input == null)
        //        {
        //            throw new ArgumentNullException(nameof(input));
        //        }

        //        StringBuilder sb = new StringBuilder();
        //        for (int offset = 0; offset < input.Length;)
        //        {
        //            byte a, b, c, d, e, f, g, h;
        //            int numCharsToOutput = GetNextGroup(input, ref offset, out a, out b, out c, out d, out e, out f, out g, out h);

        //            sb.Append((numCharsToOutput >= 1) ? _base32Chars[a] : '=');
        //            sb.Append((numCharsToOutput >= 2) ? _base32Chars[b] : '=');
        //            sb.Append((numCharsToOutput >= 3) ? _base32Chars[c] : '=');
        //            sb.Append((numCharsToOutput >= 4) ? _base32Chars[d] : '=');
        //            sb.Append((numCharsToOutput >= 5) ? _base32Chars[e] : '=');
        //            sb.Append((numCharsToOutput >= 6) ? _base32Chars[f] : '=');
        //            sb.Append((numCharsToOutput >= 7) ? _base32Chars[g] : '=');
        //            sb.Append((numCharsToOutput >= 8) ? _base32Chars[h] : '=');
        //        }

        //        return sb.ToString();
        //    }

        //    public static byte[] FromBase32(string input)
        //    {
        //        if (input == null)
        //        {
        //            throw new ArgumentNullException(nameof(input));
        //        }
        //        input = input.TrimEnd('=').ToUpperInvariant();
        //        if (input.Length == 0)
        //        {
        //            return new byte[0];
        //        }

        //        var output = new byte[input.Length * 5 / 8];
        //        var bitIndex = 0;
        //        var inputIndex = 0;
        //        var outputBits = 0;
        //        var outputIndex = 0;
        //        while (outputIndex < output.Length)
        //        {
        //            var byteIndex = _base32Chars.IndexOf(input[inputIndex]);
        //            if (byteIndex < 0)
        //            {
        //                throw new FormatException();
        //            }

        //            var bits = Math.Min(5 - bitIndex, 8 - outputBits);
        //            output[outputIndex] <<= bits;
        //            output[outputIndex] |= (byte)(byteIndex >> (5 - (bitIndex + bits)));

        //            bitIndex += bits;
        //            if (bitIndex >= 5)
        //            {
        //                inputIndex++;
        //                bitIndex = 0;
        //            }

        //            outputBits += bits;
        //            if (outputBits >= 8)
        //            {
        //                outputIndex++;
        //                outputBits = 0;
        //            }
        //        }
        //        return output;
        //    }

        //    // returns the number of bytes that were output
        //    private static int GetNextGroup(byte[] input, ref int offset, out byte a, out byte b, out byte c, out byte d, out byte e, out byte f, out byte g, out byte h)
        //    {
        //        uint b1, b2, b3, b4, b5;

        //        int retVal;
        //        switch (offset - input.Length)
        //        {
        //            case 1: retVal = 2; break;
        //            case 2: retVal = 4; break;
        //            case 3: retVal = 5; break;
        //            case 4: retVal = 7; break;
        //            default: retVal = 8; break;
        //        }

        //        b1 = (offset < input.Length) ? input[offset++] : 0U;
        //        b2 = (offset < input.Length) ? input[offset++] : 0U;
        //        b3 = (offset < input.Length) ? input[offset++] : 0U;
        //        b4 = (offset < input.Length) ? input[offset++] : 0U;
        //        b5 = (offset < input.Length) ? input[offset++] : 0U;

        //        a = (byte)(b1 >> 3);
        //        b = (byte)(((b1 & 0x07) << 2) | (b2 >> 6));
        //        c = (byte)((b2 >> 1) & 0x1f);
        //        d = (byte)(((b2 & 0x01) << 4) | (b3 >> 4));
        //        e = (byte)(((b3 & 0x0f) << 1) | (b4 >> 7));
        //        f = (byte)((b4 >> 2) & 0x1f);
        //        g = (byte)(((b4 & 0x3) << 3) | (b5 >> 5));
        //        h = (byte)(b5 & 0x1f);

        //        return retVal;
        //    }
        //}
        //#endregion
    }
}
