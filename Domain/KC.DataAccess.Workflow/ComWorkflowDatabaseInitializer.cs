using KC.Database.EFRepository;
using KC.Database.Extension;
using KC.Enums.Workflow;
using KC.Framework.Base;
using KC.Framework.SecurityHelper;
using KC.Framework.Tenant;
using KC.Framework.Util;
using KC.Model.Workflow;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace KC.DataAccess.Workflow
{
    /// <summary>
    /// 数据库初始化操作类
    /// </summary>
    public class ComWorkflowDatabaseInitializer : MultiTenantSqlServerDatabaseInitializer<ComWorkflowContext>
    {
        public ComWorkflowDatabaseInitializer()
            : base()
        { }

        public override ComWorkflowContext Create(Tenant tenant)
        {
            if (tenant == null)
                throw new System.ArgumentNullException("Tenant is null", "tenant");
            if (string.IsNullOrEmpty(tenant.TenantName))
                throw new System.ArgumentException("tenantName is null or empty", "tenantName");
            if (string.IsNullOrEmpty(tenant.ConnectionString))
                throw new System.ArgumentException("connectionString is null or empty", "connectionString");

            var options = GetCachedDbContextOptions(tenant.TenantName, tenant.ConnectionString, tenant.DatabaseType);
            return new ComWorkflowContext(options, tenant);
        }

        protected override string GetTargetMigration()
        {
            return DataInitial.DBSqlInitializer.GetPreMigrationVersion();
        }

        public void SeedData(Tenant tenant)
        {
            using (var context = Create(tenant))
            {
                var connString = context.Database.GetDbConnection().ConnectionString;
                var builder = new SqlConnectionStringBuilder(connString);
                var server = builder.DataSource;
                var database = builder.InitialCatalog;
                var user = builder.UserID;
                var pwd = builder.Password;
                var hashPwd = EncryptPasswordUtil.EncryptPassword(pwd);

                var oauthAddress = GlobalConfig.SSOWebDomain + "connect/token";
                var oauthKey = TenantConstant.GetClientIdByTenantName(tenant.TenantName);
                var oauthSecret = TenantConstant.GetClientSecretByTenantNameAndKey(tenant.TenantName, tenant.PrivateEncryptKey);

                #region 员工入职审批
                var wfDefaultDefId_1 = new Guid("C7F01F81-DC2F-4686-BCBA-187CCB3F9206");
                var wfDefinition_1 = new WorkflowDefinition()
                {
                    Id = wfDefaultDefId_1,
                    Code = "wfd2021010100001",
                    Version = "wfv2021010100001",
                    Name = "员工入职审批",
                    Status = WorkflowBusStatus.Approved,
                    DefDeadlineInterval = 7,
                    DefMessageTemplateCode = "mgt2012020200001",
                    Description = "员工入职审批",
                    SecurityType = SecurityType.OAuth,
                    AuthAddress = oauthAddress,
                    AuthScope = ApplicationConstant.AccScope,
                    AuthKey = oauthKey,
                    AuthSecret = oauthSecret,
                    IsDeleted = false,
                    CreatedBy = RoleConstants.AdminUserId,
                    CreatedName = RoleConstants.AdminUserName,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = RoleConstants.AdminUserId,
                    ModifiedName = RoleConstants.AdminUserName,
                    ModifiedDate = DateTime.UtcNow,
                };

                var node1_start = new WorkflowDefNode()
                {
                    Id = new Guid("D0E1AB22-B296-4B44-BD7F-FC4758DAAA44"),
                    Code = "wfn2021010100001",
                    Name = "开始",
                    NodeType = WorkflowNodeType.Start,
                    Type = WorkflowType.SingleLine,
                    LocTop = 120,
                    LocLeft = 240,
                    ExecutorSetting = ExecutorSetting.Executor,
                    OrgCodes = null,
                    OrgNames = null,
                    RoleIds = null,
                    RoleNames = null,
                    UserIds = null,
                    UserNames = null,
                    IsDeleted = false,
                    CreatedBy = RoleConstants.AdminUserId,
                    CreatedName = RoleConstants.AdminUserName,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = RoleConstants.AdminUserId,
                    ModifiedName = RoleConstants.AdminUserName,
                    ModifiedDate = DateTime.UtcNow,
                };
                var node2_task = new WorkflowDefNode()
                {
                    Id = new Guid("1B5510B5-C822-46B0-9A84-C54656F125BE"),
                    Code = "wfn2012020200002",
                    Name = "人事审核",
                    Type = WorkflowType.SingleLine,
                    NodeType = WorkflowNodeType.Task,
                    LocTop = 240,
                    LocLeft = 210,
                    ExecutorSetting = ExecutorSetting.Executor,
                    OrgCodes = OrganizationConstants.人事部_Code,
                    OrgNames = "人事部",
                    RoleIds = RoleConstants.HrManagerRoleId + ", " + RoleConstants.HrRoleId,
                    RoleNames = "人事经理, 人事助理",
                    UserIds = null,
                    UserNames = null,
                    IsDeleted = false,
                    CreatedBy = RoleConstants.AdminUserId,
                    CreatedName = RoleConstants.AdminUserName,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = RoleConstants.AdminUserId,
                    ModifiedName = RoleConstants.AdminUserName,
                    ModifiedDate = DateTime.UtcNow,
                };
                var node9_end = new WorkflowDefNode()
                {
                    Id = new Guid("099DF4F8-950B-43B5-AF0A-16DFA2375F57"),
                    Code = "wfn2012020200009",
                    Name = "结束",
                    Type = WorkflowType.SingleLine,
                    NodeType = WorkflowNodeType.End,
                    LocTop = 360,
                    LocLeft = 240,
                    ExecutorSetting = ExecutorSetting.Executor,
                    OrgCodes = null,
                    OrgNames = null,
                    RoleIds = null,
                    RoleNames = null,
                    UserIds = null,
                    UserNames = null,
                    IsDeleted = false,
                    CreatedBy = RoleConstants.AdminUserId,
                    CreatedName = RoleConstants.AdminUserName,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = RoleConstants.AdminUserId,
                    ModifiedName = RoleConstants.AdminUserName,
                    ModifiedDate = DateTime.UtcNow,
                };

                //流程图：Start:node1->Task:node2->End:node9
                node1_start.WorkflowDefId = wfDefinition_1.Id;
                node1_start.NextNodeCode = node2_task.Code;

                node2_task.WorkflowDefId = wfDefinition_1.Id;
                node2_task.PrevNodeCode = node1_start.Code;
                node2_task.NextNodeCode = node9_end.Code;

                node9_end.WorkflowDefId = wfDefinition_1.Id;
                node9_end.PrevNodeCode = node2_task.Code;

                var field1 = new WorkflowDefField()
                {
                    Id = 1,
                    WorkflowDefId = wfDefinition_1.Id,
                    ParentId = null,
                    Name = "MemberId",
                    DataType = AttributeDataType.String,
                    Value = "【测试】员工编号",
                    DisplayName = "员工编号",
                    Description = "员工编号：int类型，USR2018120100001",
                    Leaf = true,
                    Level = 1,
                    CanEdit = false,
                    IsPrimaryKey = true,
                    IsCondition = false,
                    IsExecutor = false,
                    IsDeleted = false,
                    CreatedBy = RoleConstants.AdminUserId,
                    CreatedName = RoleConstants.AdminUserName,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = RoleConstants.AdminUserId,
                    ModifiedName = RoleConstants.AdminUserName,
                    ModifiedDate = DateTime.UtcNow,
                };
                var field2 = new WorkflowDefField()
                {
                    Id = 2,
                    WorkflowDefId = wfDefinition_1.Id,
                    ParentId = null,
                    Name = "DisplayName",
                    DataType = AttributeDataType.String,
                    Value = "【测试】员工姓名",
                    DisplayName = "员工姓名",
                    Description = "员工姓名：string类型",
                    Leaf = true,
                    Level = 1,
                    CanEdit = false,
                    IsPrimaryKey = false,
                    IsCondition = false,
                    IsExecutor = true,
                    IsDeleted = false,
                    CreatedBy = RoleConstants.AdminUserId,
                    CreatedName = RoleConstants.AdminUserName,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = RoleConstants.AdminUserId,
                    ModifiedName = RoleConstants.AdminUserName,
                    ModifiedDate = DateTime.UtcNow,
                };
                var field3 = new WorkflowDefField()
                {
                    Id = 3,
                    WorkflowDefId = wfDefinition_1.Id,
                    ParentId = null,
                    Name = "PhoneNumber",
                    DataType = AttributeDataType.String,
                    Value = "【测试】员工手机号",
                    DisplayName = "员工手机号",
                    Description = "员工手机号：string类型",
                    Leaf = true,
                    Level = 1,
                    CanEdit = false,
                    IsPrimaryKey = false,
                    IsCondition = false,
                    IsExecutor = true,
                    IsDeleted = false,
                    CreatedBy = RoleConstants.AdminUserId,
                    CreatedName = RoleConstants.AdminUserName,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = RoleConstants.AdminUserId,
                    ModifiedName = RoleConstants.AdminUserName,
                    ModifiedDate = DateTime.UtcNow,
                };
                var field4 = new WorkflowDefField()
                {
                    Id = 4,
                    WorkflowDefId = wfDefinition_1.Id,
                    ParentId = null,
                    Name = "Email",
                    DataType = AttributeDataType.String,
                    Value = "【测试】员工邮箱",
                    DisplayName = "员工邮箱",
                    Description = "员工邮箱：string类型",
                    Leaf = true,
                    Level = 1,
                    CanEdit = false,
                    IsPrimaryKey = false,
                    IsCondition = false,
                    IsExecutor = true,
                    IsDeleted = false,
                    CreatedBy = RoleConstants.AdminUserId,
                    CreatedName = RoleConstants.AdminUserName,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = RoleConstants.AdminUserId,
                    ModifiedName = RoleConstants.AdminUserName,
                    ModifiedDate = DateTime.UtcNow,
                };

                wfDefinition_1.WorkflowNodes = new List<WorkflowDefNode>()
                {
                    node1_start, node2_task, node9_end
                };
                wfDefinition_1.WorkflowFields = new List<WorkflowDefField>()
                {
                    field1, field2, field3, field4
                };

                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        //context.DatabasePools.Add(databasePool);

                        //LogUtil.LogDebug(string.Format("---Database----server：{0}; Database: {1}; Password: {2}; PwshHash: {3}", server, database, pwd, hashPwd));
                        context.Database.ExecuteSqlRaw(string.Format("SET IDENTITY_INSERT [{0}].[wf_WorkflowDefField] ON ", tenant.TenantName));
                        context.AddOrUpdate(wfDefinition_1);
                        context.Database.ExecuteSqlRaw(string.Format("SET IDENTITY_INSERT [{0}].[wf_WorkflowDefField] OFF ", tenant.TenantName));
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        LogUtil.LogFatal(string.Format("-------Admin Config insert Database Pool is failed. \r\nMessage: {0}. \r\nStackTrace: {1}", ex.Message, ex.StackTrace));
                    }
                }
                #endregion
            }

        }
    }
}
