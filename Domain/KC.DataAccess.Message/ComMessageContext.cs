using KC.Framework.Util;
using KC.Database.EFRepository;
using KC.Framework.Tenant;
using System;
using Microsoft.EntityFrameworkCore;
using KC.Model.Message.Constants;
using KC.Model.Message;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.Extensions.Logging;

namespace KC.DataAccess.Message
{
    public class ComMessageContext : MultiTenantDataContext
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
        public ComMessageContext(
            DbContextOptions<ComMessageContext> options,
            Tenant tenant,
            IMemoryCache cache = null,
            //ICoreConventionSetBuilder builder = null,
            ILogger<ComMessageContext> logger = null)
            : base(options, tenant, cache, logger)
        {
        }

        #region Message

        public DbSet<MessageCategory> MessageCategories { get; set; }
        public DbSet<MessageClass> MessageClasses { get; set; }
        public DbSet<MessageTemplate> MessageTemplates { get; set; }
        public DbSet<MessageTemplateLog> MessageLogs { get; set; }
        public DbSet<MemberRemindMessage> MemberRemindMessages { get; set; }
        //public DbSet<MemberMessageReceiver> MemberMessageReceivers { get; set; }

        #endregion

        #region NewsBulletin

        public DbSet<NewsBulletinCategory> NewsBulletinCategories { get; set; }
        public DbSet<NewsBulletin> NewsBulletins { get; set; }
        public DbSet<NewsBulletinLog> NewsBulletinLogs { get; set; }

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

            modelBuilder.Entity<MessageCategory>().ToTable(Tables.MessageCategory, this.TenantName);
            modelBuilder.Entity<MessageClass>().ToTable(Tables.MessageClass, this.TenantName);
            modelBuilder.Entity<MessageTemplate>().ToTable(Tables.MessageTemplate, this.TenantName);
            modelBuilder.Entity<MessageTemplateLog>().ToTable(Tables.MessageTemplateLog, this.TenantName);
            modelBuilder.Entity<MemberRemindMessage>().ToTable(Tables.MemberRemindMessage, this.TenantName);

            modelBuilder.Entity<NewsBulletinCategory>().ToTable(Tables.NewsBulletinCategory, this.TenantName);
            modelBuilder.Entity<NewsBulletin>().ToTable(Tables.NewsBulletin, this.TenantName);
            modelBuilder.Entity<NewsBulletinLog>().ToTable(Tables.NewsBulletinLog, this.TenantName);
            
        }
    }
}
