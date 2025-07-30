using KC.Framework.Util;
using KC.Database.EFRepository;
using KC.Framework.Tenant;
using System;
using Microsoft.EntityFrameworkCore;
using KC.Model.Contract.Constants;
using KC.Model.Contract;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections.Generic;

namespace KC.DataAccess.Contract
{
    public class ComContractContext : MultiTenantDataContext
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
        public ComContractContext(
            DbContextOptions<ComContractContext> options,
            Tenant tenant,
            IMemoryCache cache = null,
            //ICoreConventionSetBuilder builder = null,
            ILogger<ComContractContext> logger = null)
            : base(options, tenant, cache, logger)
        {
        }

        public DbSet<ContractGroup> ContractGroups { get; set; }
        public DbSet<ContractGroupOperationLog> ContractGroupOperationLogs { get; set; }
        public DbSet<ContractTemplate> ContractTemplets { get; set; }
        public DbSet<UserContract> UserContracts { get; set; }

        public DbSet<ElectronicPerson> ElectronicPersons { get; set; }
        public DbSet<ElectronicOrganization> ElectronicOrganizations { get; set; }


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

            //value conversions: https://docs.microsoft.com/en-us/ef/core/modeling/value-conversions
            var splitStringConverter = new ValueConverter<IEnumerable<string>, string>(v => string.Join(",", v), v => v.Split(new[] { ',' }));

            modelBuilder.Entity<ContractGroup>().ToTable(Tables.ContractGroup, TenantName);
            modelBuilder.Entity<ContractGroupOperationLog>().ToTable(Tables.ContractGroupOperationLog, TenantName);
            modelBuilder.Entity<ContractTemplate>().ToTable(Tables.ContractTemplate, TenantName);
            modelBuilder.Entity<UserContract>().ToTable(Tables.UserContract, TenantName);

            modelBuilder.Entity<ElectronicPerson>().ToTable(Tables.ElectronicPerson, TenantName);
            modelBuilder.Entity<ElectronicOrganization>().ToTable(Tables.ElectronicOrganization, TenantName);
        }
    }
}
