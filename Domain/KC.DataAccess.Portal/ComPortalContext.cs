using KC.Framework.Util;
using KC.Database.EFRepository;
using KC.Framework.Tenant;
using System;
using Microsoft.EntityFrameworkCore;
using KC.Model.Portal.Constants;
using KC.Model.Portal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.Extensions.Logging;

namespace KC.DataAccess.Portal
{
    public class ComPortalContext : MultiTenantDataContext
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
        public ComPortalContext(
            DbContextOptions<ComPortalContext> options,
            Tenant tenant,
            IMemoryCache cache = null,
            //ICoreConventionSetBuilder builder = null,
            ILogger<ComPortalContext> logger = null)
            : base(options, tenant, cache, logger)
        {
        }

        #region 站点信息
        public DbSet<WebSiteInfo> WebSiteInfos { get; set; }
        public DbSet<WebSiteLink> WebSiteLinks { get; set; }
        public DbSet<FavoriteInfo> FavoriteInfos { get; set; }

        public DbSet<WebSiteTemplateColumn> WebSiteTemplateColumns { get; set; }
        public DbSet<WebSiteTemplateItem> WebSiteTemplateItems { get; set; }

        public DbSet<WebSitePage> WebSitePages { get; set; }
        public DbSet<WebSiteColumn> WebSiteColumns { get; set; }
        public DbSet<WebSiteItem> WebSiteItems { get; set; }
        public DbSet<WebSitePageLog> WebSitePageLogs { get; set; }
        #endregion

        #region 推荐信息
        public DbSet<RecommendCategory> RecommendCategories { get; set; }

        public DbSet<RecommendOffering> RecommendOfferings { get; set; }
        public DbSet<RecommendOfferingLog> RecommendOfferingLogs { get; set; }

        public DbSet<RecommendCustomer> RecommendCustomers { get; set; }
        public DbSet<RecommendCustomerLog> RecommendCustomerLogs { get; set; }

        public DbSet<RecommendMaterial> RecommendMaterials { get; set; }
        public DbSet<RequirementForMaterial> RequirementForMaterials { get; set; }
        public DbSet<RecommendRequirement> RecommendRequirements { get; set; }
        public DbSet<RecommendRequirementLog> RecommendRequirementLogs { get; set; }

        #endregion

        #region 企业信息
        public DbSet<CompanyInfo> CompanyInfos { get; set; }
        public DbSet<CompanyAccount> CompanyAccounts { get; set; }
        public DbSet<CompanyContact> CompanyContacts { get; set; }
        public DbSet<CompanyAddress> CompanyAddresses { get; set; }
        public DbSet<CompanyProcessLog> CompanyProcessLogs { get; set; }

        public DbSet<CompanyAuthentication> CompanyAuthentications { get; set; }
        public DbSet<CompanyAuthenticationFailedRecord> CompanyAuthenticationFailedRecords { get; set; }

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

            #region 站点信息
            modelBuilder.Entity<WebSiteInfo>().ToTable(Tables.WebSiteInfo, this.TenantName);
            modelBuilder.Entity<WebSiteLink>().ToTable(Tables.WebSiteLink, this.TenantName); 
            modelBuilder.Entity<FavoriteInfo>().ToTable(Tables.FavoriteInfo, this.TenantName);

            modelBuilder.Entity<WebSiteTemplateColumn>().ToTable(Tables.WebSiteTemplateColumn, this.TenantName);
            modelBuilder.Entity<WebSiteTemplateItem>().ToTable(Tables.WebSiteTemplateItem, this.TenantName);

            modelBuilder.Entity<WebSitePage>().ToTable(Tables.WebSitePage, this.TenantName);
            modelBuilder.Entity<WebSiteColumn>().ToTable(Tables.WebSiteColumn, this.TenantName);
            modelBuilder.Entity<WebSiteItem>().ToTable(Tables.WebSiteItem, this.TenantName);
            modelBuilder.Entity<WebSitePageLog>().ToTable(Tables.WebSitePageLog, this.TenantName);
            #endregion

            #region Recommend
            modelBuilder.Entity<RecommendCategory>().ToTable(Tables.RecommendCategory, this.TenantName);
            modelBuilder.Entity<RecommendOffering>().ToTable(Tables.RecommendOffering, this.TenantName);
            modelBuilder.Entity<RecommendOfferingLog>().ToTable(Tables.RecommendOfferingLog, this.TenantName);
           
            modelBuilder.Entity<RecommendCustomer>().ToTable(Tables.RecommendCustomer, this.TenantName);
            modelBuilder.Entity<RecommendCustomerLog>().ToTable(Tables.RecommendCustomerLog, this.TenantName);

            modelBuilder.Entity<RecommendMaterial>().ToTable(Tables.RecommendMaterial, this.TenantName);
            modelBuilder.Entity<RecommendRequirement>().ToTable(Tables.RecommendRequirement, this.TenantName);
            modelBuilder.Entity<RecommendRequirementLog>().ToTable(Tables.RecommendRequirementLog, this.TenantName);

            modelBuilder.Entity<RequirementForMaterial>(setting =>
            {
                setting.ToTable(Tables.RequirementForMaterial, TenantName);

                setting.HasKey(t => new { t.RequirementId, t.MaterialId });

                setting.HasOne(m => m.RecommendRequirement)
                    .WithMany(m => m.RequirementForMaterials)
                    .HasForeignKey(m => m.RequirementId)
                    .OnDelete(DeleteBehavior.Restrict);
                setting.HasOne(m => m.RecommendMaterial)
                    .WithMany(m => m.RequirementForMaterials)
                    .HasForeignKey(m => m.MaterialId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion

            #region Company
            modelBuilder.Entity<CompanyInfo>().ToTable(Tables.CompanyInfo, this.TenantName);
            modelBuilder.Entity<CompanyAccount>().ToTable(Tables.CompanyAccount, this.TenantName);
            modelBuilder.Entity<CompanyContact>().ToTable(Tables.CompanyContact, this.TenantName);
            modelBuilder.Entity<CompanyAddress>().ToTable(Tables.CompanyAddress, this.TenantName);
            modelBuilder.Entity<CompanyProcessLog>().ToTable(Tables.CompanyProcessLog, this.TenantName);

            modelBuilder.Entity<CompanyAuthentication>().ToTable(Tables.CompanyAuthentication, TenantName);
            modelBuilder.Entity<CompanyAuthenticationFailedRecord>().ToTable(Tables.CompanyAuthenticationFailedRecord, TenantName);

            modelBuilder.Entity<CompanyInfo>()
                .HasOne(b => b.AuthenticationInfo)
                .WithOne(i => i.CompanyInfo)
                .HasForeignKey<CompanyAuthentication>(b => b.CompanyCode);
            #endregion

            
        }
    }
}
