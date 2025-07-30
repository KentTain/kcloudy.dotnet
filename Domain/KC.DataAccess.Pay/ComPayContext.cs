using KC.Framework.Util;
using KC.Database.EFRepository;
using KC.Framework.Tenant;
using System;
using Microsoft.EntityFrameworkCore;
using KC.Model.Pay.Constants;
using KC.Model.Pay;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections.Generic;

namespace KC.DataAccess.Pay
{
    public class ComPayContext : MultiTenantDataContext
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
        public ComPayContext(
            DbContextOptions<ComPayContext> options,
            Tenant tenant,
            IMemoryCache cache = null,
            //ICoreConventionSetBuilder builder = null,
            ILogger<ComPayContext> logger = null)
            : base(options, tenant, cache, logger)
        {
        }

        
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<PaymentBankAccount> PaymentBankAccounts { get; set; }

        public DbSet<OnlinePaymentRecord> OnlinePaymentRecords { get; set; }
        public DbSet<PaymentTradeRecord> PaymentTradeRecords { get; set; }

        public DbSet<PaymentInfo> PaymentInfos { get; set; }
        public DbSet<PaymentOperationLog> PaymentOperationLogs { get; set; }

        public DbSet<CashUsageDetail> CashUsageDetails { get; set; }

        #region  应收应付

        public DbSet<Payable> Payables { get; set; }
        public DbSet<Receivable> Receivables { get; set; }

        public DbSet<PaymentRecord> PaymentRecords { get; set; }
        public DbSet<PayableAndReceivableRecord> PayableAndReceivableRecords { get; set; }

        public DbSet<OfflineUsageBill> OfflineUsageBills { get; set; }
        public DbSet<OfflinePayment> OfflinePayments { get; set; }

        public DbSet<PaymentAttachment> PaymentAttachments { get; set; }

        public DbSet<EntrustedPaymentRecord> EntrustedPaymentRecords { get; set; }
        public DbSet<VoucherPaymentRecord> VoucherPaymentRecords { get; set; }

        public DbSet<CautionMoneyLog> CautionMoneyLogs { get; set; }
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

            modelBuilder.Entity<BankAccount>().ToTable(Tables.BankAccount, TenantName);
            modelBuilder.Entity<PaymentBankAccount>().ToTable(Tables.PaymentBankAccount, TenantName);
            modelBuilder.Entity<OnlinePaymentRecord>().ToTable(Tables.OnlinePaymentRecord, TenantName);
            modelBuilder.Entity<PaymentTradeRecord>().ToTable(Tables.PaymentTradeRecord, TenantName);
            modelBuilder.Entity<PaymentInfo>().ToTable(Tables.PaymentInfo, TenantName);
            modelBuilder.Entity<PaymentOperationLog>().ToTable(Tables.PaymentOperationLog, TenantName);
            modelBuilder.Entity<CashUsageDetail>().ToTable(Tables.CashUsageDetail, TenantName);

            modelBuilder.Entity<Payable>().ToTable(Tables.Payable, TenantName);
            modelBuilder.Entity<Receivable>().ToTable(Tables.Receivable, TenantName);
            modelBuilder.Entity<PaymentRecord>().ToTable(Tables.PaymentRecord, TenantName);
            modelBuilder.Entity<PayableAndReceivableRecord>().ToTable(Tables.PayableAndReceivableRecord, TenantName);
            modelBuilder.Entity<OfflineUsageBill>().ToTable(Tables.OfflineUsageBill, TenantName);
            modelBuilder.Entity<OfflinePayment>().ToTable(Tables.OfflinePayment, TenantName);
            modelBuilder.Entity<PaymentAttachment>().ToTable(Tables.PaymentAttachment, TenantName);
            modelBuilder.Entity<EntrustedPaymentRecord>().ToTable(Tables.EntrustedPaymentRecord, TenantName);
            modelBuilder.Entity<VoucherPaymentRecord>().ToTable(Tables.VoucherPaymentRecord, TenantName);
            
            modelBuilder.Entity<CautionMoneyLog>().ToTable(Tables.CautionMoneyLog, TenantName);
        }
    }
}
