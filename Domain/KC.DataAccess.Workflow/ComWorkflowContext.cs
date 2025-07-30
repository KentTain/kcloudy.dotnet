using KC.Framework.Util;
using KC.Database.EFRepository;
using KC.Framework.Tenant;
using System;
using Microsoft.EntityFrameworkCore;
using KC.Model.Workflow.Constants;
using KC.Model.Workflow;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using KC.Framework.Extension;

namespace KC.DataAccess.Workflow
{
    public class ComWorkflowContext : MultiTenantDataContext
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
        public ComWorkflowContext(
            DbContextOptions<ComWorkflowContext> options,
            Tenant tenant,
            IMemoryCache cache = null,
            //ICoreConventionSetBuilder builder = null,
            ILogger<ComWorkflowContext> logger = null)
            : base(options, tenant, cache, logger)
        {
        }

        public DbSet<ModelDefinition> ModelDefinitions { get; set; }
        public DbSet<ModelDefField> ModelDefFields { get; set; }
        public DbSet<ModelDefLog> ModelDefLogs { get; set; }
        
        public DbSet<WorkflowCategory> WorkflowCategories { get; set; }
        public DbSet<WorkflowDefinition> WorkflowDefinitions { get; set; }
        public DbSet<WorkflowDefField> WorkflowDefFields { get; set; }
        public DbSet<WorkflowDefNode> WorkflowDefNodes { get; set; }
        public DbSet<WorkflowDefNodeRule> WorkflowDefNodeRules { get; set; }
        public DbSet<WorkflowDefLog> WorkflowDefLogs { get; set; }

        public DbSet<WorkflowVerDefinition> WorkflowVerDefinitions { get; set; }
        public DbSet<WorkflowVerDefField> WorkflowVerDefFields { get; set; }
        public DbSet<WorkflowVerDefNode> WorkflowVerDefNodes { get; set; }
        public DbSet<WorkflowVerDefNodeRule> WorkflowVerDefNodeRules { get; set; }


        public DbSet<WorkflowProcess> WorkflowProcesses { get; set; }
        public DbSet<WorkflowProField> WorkflowProFields { get; set; }
        public DbSet<WorkflowProTask> WorkflowProTasks { get; set; }
        public DbSet<WorkflowProTaskRule> WorkflowProTaskRules { get; set; }
        public DbSet<WorkflowProTaskExecute> WorkflowProTaskExecutes { get; set; }
        public DbSet<WorkflowProRequestLog> WorkflowProRequestLogs { get; set; }

        
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
            
            modelBuilder.Entity<ModelDefinition>().ToTable(Tables.ModelDefinition, this.TenantName);
            modelBuilder.Entity<ModelDefField>().ToTable(Tables.ModelDefField, this.TenantName);
            modelBuilder.Entity<ModelDefLog>().ToTable(Tables.ModelDefLog, this.TenantName);


            modelBuilder.Entity<WorkflowCategory>().ToTable(Tables.WorkflowCategory, this.TenantName);
            modelBuilder.Entity<WorkflowDefinition>().ToTable(Tables.WorkflowDefinition, this.TenantName);
            modelBuilder.Entity<WorkflowDefField>().ToTable(Tables.WorkflowDefField, this.TenantName);
            modelBuilder.Entity<WorkflowDefNode>().ToTable(Tables.WorkflowDefNode, this.TenantName);
            modelBuilder.Entity<WorkflowDefNodeRule>().ToTable(Tables.WorkflowDefNodeRule, this.TenantName);
            modelBuilder.Entity<WorkflowDefLog>().ToTable(Tables.WorkflowDefLog, this.TenantName);

            modelBuilder.Entity<WorkflowVerDefinition>().ToTable(Tables.WorkflowVerDefinition, this.TenantName);
            modelBuilder.Entity<WorkflowVerDefField>().ToTable(Tables.WorkflowVerDefField, this.TenantName);
            modelBuilder.Entity<WorkflowVerDefNode>().ToTable(Tables.WorkflowVerDefNode, this.TenantName);
            modelBuilder.Entity<WorkflowVerDefNodeRule>().ToTable(Tables.WorkflowVerDefNodeRule, this.TenantName);

            modelBuilder.Entity<WorkflowProcess>().ToTable(Tables.WorkflowProcess, this.TenantName);
            modelBuilder.Entity<WorkflowProField>().ToTable(Tables.WorkflowProField, this.TenantName);
            modelBuilder.Entity<WorkflowProTask>().ToTable(Tables.WorkflowProTask, this.TenantName);
            modelBuilder.Entity<WorkflowProTaskRule>().ToTable(Tables.WorkflowProTaskRule, this.TenantName);
            modelBuilder.Entity<WorkflowProTaskExecute>().ToTable(Tables.WorkflowProTaskExecute, this.TenantName);
            modelBuilder.Entity<WorkflowProRequestLog>().ToTable(Tables.WorkflowProRequestLog, this.TenantName);

            

            //#region Linked Node
            //modelBuilder.Entity<WorkflowDefNode>()
            //    .HasOne(x => x.NextNode)
            //    .WithMany()
            //    .HasPrincipalKey("Id")
            //    .HasForeignKey("NextNodeId")
            //    .OnDelete(DeleteBehavior.Restrict)
            //    .IsRequired(false);
            //modelBuilder.Entity<WorkflowDefNode>()
            //    .HasOne(x => x.PrevNode)
            //    .WithMany()
            //    .HasPrincipalKey("Id")
            //    .HasForeignKey("PrevNodeId")
            //    .OnDelete(DeleteBehavior.Restrict)
            //    .IsRequired(false);
            //modelBuilder.Entity<WorkflowDefNode>()
            //    .HasOne(x => x.ReturnNode)
            //    .WithMany()
            //    .HasPrincipalKey("Id")
            //    .HasForeignKey("ReturnNodeId")
            //    .OnDelete(DeleteBehavior.Restrict)
            //    .IsRequired(false);

            //modelBuilder.Entity<WorkflowVerDefNode>()
            //    .HasOne(x => x.NextNode)
            //    .WithMany()
            //    .HasPrincipalKey("Id")
            //    .HasForeignKey("NextNodeId")
            //    .OnDelete(DeleteBehavior.Restrict)
            //    .IsRequired(false);
            //modelBuilder.Entity<WorkflowVerDefNode>()
            //    .HasOne(x => x.PrevNode)
            //    .WithMany()
            //    .HasPrincipalKey("Id")
            //    .HasForeignKey("PrevNodeId")
            //    .OnDelete(DeleteBehavior.Restrict)
            //    .IsRequired(false);
            //modelBuilder.Entity<WorkflowVerDefNode>()
            //    .HasOne(x => x.ReturnNode)
            //    .WithMany()
            //    .HasPrincipalKey("Id")
            //    .HasForeignKey("ReturnNodeId")
            //    .OnDelete(DeleteBehavior.Restrict)
            //    .IsRequired(false);

            //modelBuilder.Entity<WorkflowProTask>()
            //    .HasOne(x => x.NextNode)
            //    .WithMany()
            //    .HasPrincipalKey("Id")
            //    .HasForeignKey("NextNodeId")
            //    .OnDelete(DeleteBehavior.Restrict)
            //    .IsRequired(false);
            //modelBuilder.Entity<WorkflowProTask>()
            //    .HasOne(x => x.PrevNode)
            //    .WithMany()
            //    .HasPrincipalKey("Id")
            //    .HasForeignKey("PrevNodeId")
            //    .OnDelete(DeleteBehavior.Restrict)
            //    .IsRequired(false);
            //modelBuilder.Entity<WorkflowProTask>()
            //    .HasOne(x => x.ReturnNode)
            //    .WithMany()
            //    .HasPrincipalKey("Id")
            //    .HasForeignKey("ReturnNodeId")
            //    .OnDelete(DeleteBehavior.Restrict)
            //    .IsRequired(false);
            //#endregion

            #region Value Conversions
            //value conversions: https://docs.microsoft.com/en-us/ef/core/modeling/value-conversions
            var splitStringConverter = new ValueConverter<IEnumerable<string>, string>(v => string.Join(",", v), v => v.ArrayFromCommaDelimitedStrings());

            //modelBuilder
            //    .Entity<WorkflowDefNode>()
            //    .Property(m => m.ExecuteOrgIds)
            //    .HasConversion(splitStringConverter);
            //modelBuilder
            //     .Entity<WorkflowDefNode>()
            //     .Property(m => m.ExecuteRoleIds)
            //     .HasConversion(splitStringConverter);
            //modelBuilder
            //     .Entity<WorkflowDefNode>()
            //     .Property(m => m.ExecuteUserIds)
            //     .HasConversion(splitStringConverter);
            //modelBuilder
            //     .Entity<WorkflowDefNode>()
            //     .Property(m => m.NotifyUserIds)
            //     .HasConversion(splitStringConverter);

            //modelBuilder
            //    .Entity<WorkflowVerDefNode>()
            //    .Property(m => m.ExecuteOrgIds)
            //    .HasConversion(splitStringConverter);
            //modelBuilder
            //     .Entity<WorkflowVerDefNode>()
            //     .Property(m => m.ExecuteRoleIds)
            //     .HasConversion(splitStringConverter);
            //modelBuilder
            //     .Entity<WorkflowVerDefNode>()
            //     .Property(m => m.ExecuteUserIds)
            //     .HasConversion(splitStringConverter);
            //modelBuilder
            //     .Entity<WorkflowVerDefNode>()
            //     .Property(m => m.NotifyUserIds)
            //     .HasConversion(splitStringConverter);

            //modelBuilder
            //    .Entity<WorkflowProTask>()
            //    .Property(m => m.ExecuteOrgIds)
            //    .HasConversion(splitStringConverter);
            //modelBuilder
            //     .Entity<WorkflowProTask>()
            //     .Property(m => m.ExecuteRoleIds)
            //     .HasConversion(splitStringConverter);
            //modelBuilder
            //     .Entity<WorkflowProTask>()
            //     .Property(m => m.ExecuteUserIds)
            //     .HasConversion(splitStringConverter);
            //modelBuilder
            //     .Entity<WorkflowProTask>()
            //     .Property(m => m.NotifyUserIds)
            //     .HasConversion(splitStringConverter);
            #endregion


        }
    }
}
