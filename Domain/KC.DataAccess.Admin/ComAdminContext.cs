using KC.Framework.Util;
using KC.Database.EFRepository;
using KC.Model.Admin;
using KC.Framework.Tenant;
using System;
using Microsoft.EntityFrameworkCore;
using KC.Model.Admin.Constants;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace KC.DataAccess.Admin
{
    public class ComAdminContext : MultiTenantDataContext
    {
        /// <summary>
        /// Only used by migrations With default tenantId "Dba"
        /// </summary>
        //public ComAdminContext(DbContextOptions<ComAdminContext> options)
        //    : this(options, TenantConstant.DbaTenantApiAccessInfo)
        //{
        //    ////For Debug
        //    //if (System.Diagnostics.Debugger.IsAttached == false)
        //    //    System.Diagnostics.Debugger.Launch();
        //}
        public ComAdminContext(
            DbContextOptions<ComAdminContext> options,
            //Tenant tenant,
            IMemoryCache cache = null,
            //IProviderConventionSetBuilder builder = null,
            ILogger<ComAdminContext> logger = null)
            : base(options, TenantConstant.DbaTenantApiAccessInfo, cache, logger)
        {
        }

        #region 资源
        public DbSet<DatabasePool> DatabasePools { get; set; }
        public DbSet<StoragePool> StoragePools { get; set; }
        public DbSet<QueuePool> QueuePools { get; set; }
        public DbSet<NoSqlPool> NoSqlPools { get; set; }
        public DbSet<ServiceBusPool> ServiceBusPools { get; set; } 
        public DbSet<VodPool> VodPools { get; set; }
        public DbSet<CodeRepositoryPool> CodeRepositoryPools { get; set; }
        #endregion

        #region 租户
        public DbSet<TenantUser> TenantUsers { get; set; }
        
        public DbSet<TenantUserSetting> TenantSettings { get; set; }
        public DbSet<TenantUserApplication> TenantUserApplications { get; set; }
        //public DbSet<TenantUserAppModule> TenantUserAppModules { get; set; }

        public DbSet<TenantUserOperationLog> TenantUserOperationLogs { get; set; }
        public DbSet<TenantUserOpenAppErrorLog> TenantUserOpenAppErrorLogs { get; set; }
        public DbSet<TenantUserServiceApplication> TenantUserServiceApplications { get; set; } 
        public DbSet<TenantUserLoopTask> TenantUserLoopTasks { get; set; }
        #endregion

        #region 租户信息
        public DbSet<TenantUserAuthentication> TenantUserAuthentications { get; set; }

        public DbSet<TenantUserOffering> TenantUserOfferings { get; set; }
        public DbSet<TenantUserMaterial> TenantUserMaterials { get; set; }
        public DbSet<TenantUserRequirement> TenantUserRequirements { get; set; }
        public DbSet<RequirementForMaterial> RequirementForMaterials { get; set; }
        #endregion

        #region  租户计费
        public DbSet<TenantUserChargeSms> TenantUserChargeSmses { get; set; }
        public DbSet<TenantUserChargeStorage> TenantUserChargeStorages { get; set; }
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

            #region 资源
            modelBuilder.Entity<DatabasePool>().ToTable(Tables.DatabasePool, TenantName);
            modelBuilder.Entity<NoSqlPool>().ToTable(Tables.NoSqlPool, TenantName);
            modelBuilder.Entity<QueuePool>().ToTable(Tables.QueuePool, TenantName);
            modelBuilder.Entity<ServiceBusPool>().ToTable(Tables.ServiceBusPool, TenantName);
            modelBuilder.Entity<StoragePool>().ToTable(Tables.StoragePool, TenantName);
            modelBuilder.Entity<VodPool>().ToTable(Tables.VodPool, TenantName);
            modelBuilder.Entity<CodeRepositoryPool>().ToTable(Tables.CodeRepositoryPool, TenantName);
            #endregion

            #region 租户
            modelBuilder.Entity<TenantUserSetting>(setting =>
            {
                setting.ToTable(Tables.TenantSetting, TenantName);

                setting.HasOne(s => s.TenantUser)
                    .WithMany(u => u.TenantSettings)
                    .HasForeignKey(u => u.TenantId)
                    .IsRequired();
            });
            modelBuilder.Entity<TenantUserApplication>(app =>
            {
                app.ToTable(Tables.TenantUserApplication, TenantName);

                app.HasOne(s => s.TenantUser)
                    .WithMany(u => u.Applications)
                    .HasForeignKey(u => u.TenantId)
                    .IsRequired();
            });

            //modelBuilder.Entity<TenantUserAppModule>().ToTable(Tables.TenantUserAppModule, TenantName);
            modelBuilder.Entity<TenantUserLoopTask>().ToTable(Tables.TenantUserLoopTask, TenantName);
            modelBuilder.Entity<TenantUserServiceApplication>().ToTable(Tables.TenantUserServiceApplication, TenantName);

            modelBuilder.Entity<TenantUserOpenAppErrorLog>().ToTable(Tables.TenantUserOpenAppErrorLog, TenantName);
            modelBuilder.Entity<TenantUserOperationLog>().ToTable(Tables.TenantUserOperationLog, TenantName);

            modelBuilder.Entity<TenantUser>().ToTable(Tables.TenantUser, TenantName).HasKey(m => m.TenantId);

            modelBuilder.Entity<TenantUser>()
                .HasOne(b => b.AuthenticationInfo)
                .WithOne(i => i.TenantUser)
                .HasForeignKey<TenantUserAuthentication>(b => b.TenantId);
            #endregion

            #region 租户信息

            modelBuilder.Entity<TenantUserAuthentication>().ToTable(Tables.TenantUserAuthentication, TenantName).HasKey(m => m.TenantId);

            modelBuilder.Entity<TenantUserOffering>().ToTable(Tables.TenantUserOffering, TenantName);
            modelBuilder.Entity<TenantUserMaterial>().ToTable(Tables.TenantUserMaterial, TenantName);
            modelBuilder.Entity<TenantUserRequirement>().ToTable(Tables.TenantUserRequirement, TenantName);
            modelBuilder.Entity<RequirementForMaterial>(setting =>
            {
                setting.ToTable(Tables.RequirementForMaterial, TenantName);

                setting.HasKey(t => new { t.RequirementId, t.MaterialId });
                
                setting.HasOne(m => m.Requirement)
                    .WithMany(m => m.RequirementForMaterials)
                    .HasForeignKey(m => m.RequirementId)
                    .OnDelete(DeleteBehavior.Restrict);
                setting.HasOne(m => m.Material)
                    .WithMany(m => m.RequirementForMaterials)
                    .HasForeignKey(m => m.MaterialId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion

            #region  租户计费

            modelBuilder.Entity<TenantUserChargeSms>().ToTable(Tables.TenantUserChargeSms, TenantName);

            modelBuilder.Entity<TenantUserChargeStorage>().ToTable(Tables.TenantUserChargeStorage, TenantName);
            #endregion

        }
    }
}
