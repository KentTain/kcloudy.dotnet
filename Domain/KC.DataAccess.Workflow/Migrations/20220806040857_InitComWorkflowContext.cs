using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KC.DataAccess.Workflow.Migrations
{
    public partial class InitComWorkflowContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cTest");

            migrationBuilder.CreateTable(
                name: "wf_ModelDefinition",
                schema: "cTest",
                columns: table => new
                {
                    PropertyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    IsRequire = table.Column<bool>(type: "bit", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wf_ModelDefinition", x => x.PropertyId);
                });

            migrationBuilder.CreateTable(
                name: "wf_ModelDefLog",
                schema: "cTest",
                columns: table => new
                {
                    ProcessLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModelDefId = table.Column<int>(type: "int", nullable: false),
                    ModelDefName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    OperatorId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Operator = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OperateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wf_ModelDefLog", x => x.ProcessLogId);
                });

            migrationBuilder.CreateTable(
                name: "wf_WorkflowCategory",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    TreeCode = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Leaf = table.Column<bool>(type: "bit", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wf_WorkflowCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_wf_WorkflowCategory_wf_WorkflowCategory_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "cTest",
                        principalTable: "wf_WorkflowCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "wf_WorkflowDefLog",
                schema: "cTest",
                columns: table => new
                {
                    ProcessLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowDefId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Version = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    OperatorId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Operator = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OperateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wf_WorkflowDefLog", x => x.ProcessLogId);
                });

            migrationBuilder.CreateTable(
                name: "wf_WorkflowProRequestLog",
                schema: "cTest",
                columns: table => new
                {
                    ProcessLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProcessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProcessCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ProcessName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    RequestType = table.Column<int>(type: "int", nullable: false),
                    RequestUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RequestPostData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestResultData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestErrorData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    OperatorId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Operator = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OperateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wf_WorkflowProRequestLog", x => x.ProcessLogId);
                });

            migrationBuilder.CreateTable(
                name: "wf_ModelDefField",
                schema: "cTest",
                columns: table => new
                {
                    PropertyAttributeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModelDefId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DataType = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPrimaryKey = table.Column<bool>(type: "bit", nullable: false),
                    IsExecutor = table.Column<bool>(type: "bit", nullable: false),
                    IsCondition = table.Column<bool>(type: "bit", nullable: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    IsRequire = table.Column<bool>(type: "bit", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false),
                    Ext1 = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Ext2 = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Ext3 = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wf_ModelDefField", x => x.PropertyAttributeId);
                    table.ForeignKey(
                        name: "FK_wf_ModelDefField_wf_ModelDefinition_ModelDefId",
                        column: x => x.ModelDefId,
                        principalSchema: "cTest",
                        principalTable: "wf_ModelDefinition",
                        principalColumn: "PropertyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "wf_WorkflowDefinition",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Version = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DefDeadlineInterval = table.Column<int>(type: "int", nullable: true),
                    DefMessageTemplateCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SecurityType = table.Column<int>(type: "int", nullable: false),
                    AuthAddress = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AuthAddressParams = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AuthScope = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AuthKey = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AuthSecret = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    WorkflowFormType = table.Column<int>(type: "int", nullable: false),
                    AppFormDetailApiUrl = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AppFormDetailQueryString = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AppAuditSuccessApiUrl = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AppAuditReturnApiUrl = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AppAuditQueryString = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wf_WorkflowDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_wf_WorkflowDefinition_wf_WorkflowCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "cTest",
                        principalTable: "wf_WorkflowCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "wf_WorkflowProcess",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    WorkflowDefId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowDefCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    WorkflowDefName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Version = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DefDeadlineInterval = table.Column<int>(type: "int", nullable: true),
                    DefMessageTemplateCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SecurityType = table.Column<int>(type: "int", nullable: false),
                    AuthAddress = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AuthAddressParams = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AuthScope = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AuthKey = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AuthSecret = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    WorkflowFormType = table.Column<int>(type: "int", nullable: false),
                    AppFormDetailApiUrl = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AppFormDetailQueryString = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AppAuditSuccessApiUrl = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AppAuditReturnApiUrl = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AppAuditQueryString = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SubmitUserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    SubmitUserName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ProcessStatus = table.Column<int>(type: "int", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeadlineDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrentTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wf_WorkflowProcess", x => x.Id);
                    table.ForeignKey(
                        name: "FK_wf_WorkflowProcess_wf_WorkflowCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "cTest",
                        principalTable: "wf_WorkflowCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "wf_WorkflowDefField",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowDefId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    TreeCode = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataType = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Value1 = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Value2 = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    IsPrimaryKey = table.Column<bool>(type: "bit", nullable: false),
                    IsExecutor = table.Column<bool>(type: "bit", nullable: false),
                    IsCondition = table.Column<bool>(type: "bit", nullable: false),
                    Leaf = table.Column<bool>(type: "bit", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wf_WorkflowDefField", x => x.Id);
                    table.ForeignKey(
                        name: "FK_wf_WorkflowDefField_wf_WorkflowDefField_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "cTest",
                        principalTable: "wf_WorkflowDefField",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_wf_WorkflowDefField_wf_WorkflowDefinition_WorkflowDefId",
                        column: x => x.WorkflowDefId,
                        principalSchema: "cTest",
                        principalTable: "wf_WorkflowDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "wf_WorkflowDefNode",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowDefId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrgCodes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrgNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExceptUserIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExceptUserNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    NodeType = table.Column<int>(type: "int", nullable: false),
                    DeadlineInterval = table.Column<int>(type: "int", nullable: true),
                    MessageTemplateCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LocTop = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LocLeft = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrevNodeCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    NextNodeCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ReturnNodeCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    SubDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubDefinitionCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SubDefinitionVersion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SubDefinitionName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    WeightValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExecutorSetting = table.Column<int>(type: "int", nullable: false),
                    ExecutorFormFieldName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExecutorFormFieldDisplayName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    NotifyUserIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotifyUserNames = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wf_WorkflowDefNode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_wf_WorkflowDefNode_wf_WorkflowDefinition_WorkflowDefId",
                        column: x => x.WorkflowDefId,
                        principalSchema: "cTest",
                        principalTable: "wf_WorkflowDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "wf_WorkflowVerDefinition",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    WorkflowVerDefId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Version = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DefDeadlineInterval = table.Column<int>(type: "int", nullable: true),
                    DefMessageTemplateCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SecurityType = table.Column<int>(type: "int", nullable: false),
                    AuthAddress = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AuthAddressParams = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AuthScope = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AuthKey = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AuthSecret = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    WorkflowFormType = table.Column<int>(type: "int", nullable: false),
                    AppFormDetailApiUrl = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AppFormDetailQueryString = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AppAuditSuccessApiUrl = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AppAuditReturnApiUrl = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AppAuditQueryString = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wf_WorkflowVerDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_wf_WorkflowVerDefinition_wf_WorkflowCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "cTest",
                        principalTable: "wf_WorkflowCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_wf_WorkflowVerDefinition_wf_WorkflowDefinition_WorkflowVerDefId",
                        column: x => x.WorkflowVerDefId,
                        principalSchema: "cTest",
                        principalTable: "wf_WorkflowDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "wf_WorkflowProField",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProcessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    TreeCode = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Leaf = table.Column<bool>(type: "bit", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataType = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Value1 = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Value2 = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    IsPrimaryKey = table.Column<bool>(type: "bit", nullable: false),
                    IsExecutor = table.Column<bool>(type: "bit", nullable: false),
                    IsCondition = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wf_WorkflowProField", x => x.Id);
                    table.ForeignKey(
                        name: "FK_wf_WorkflowProField_wf_WorkflowProcess_ProcessId",
                        column: x => x.ProcessId,
                        principalSchema: "cTest",
                        principalTable: "wf_WorkflowProcess",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_wf_WorkflowProField_wf_WorkflowProField_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "cTest",
                        principalTable: "wf_WorkflowProField",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "wf_WorkflowProTask",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskStatus = table.Column<int>(type: "int", nullable: false),
                    ProcessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrgCodes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrgNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExceptUserIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExceptUserNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    NodeType = table.Column<int>(type: "int", nullable: false),
                    DeadlineInterval = table.Column<int>(type: "int", nullable: true),
                    MessageTemplateCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LocTop = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LocLeft = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrevNodeCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    NextNodeCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ReturnNodeCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    SubDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubDefinitionCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SubDefinitionVersion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SubDefinitionName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    WeightValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExecutorSetting = table.Column<int>(type: "int", nullable: false),
                    ExecutorFormFieldName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExecutorFormFieldDisplayName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    NotifyUserIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotifyUserNames = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeadlineDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AgreeUserIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AgreeUserNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisagreeUserIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisagreeUserNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnProcessUserIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnProcessUserNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllUserIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllUserNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wf_WorkflowProTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_wf_WorkflowProTask_wf_WorkflowProcess_ProcessId",
                        column: x => x.ProcessId,
                        principalSchema: "cTest",
                        principalTable: "wf_WorkflowProcess",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "wf_WorkflowDefNodeRule",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowNodeCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    WorkflowNodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RuleType = table.Column<int>(type: "int", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    FieldDisplayName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    OperatorType = table.Column<int>(type: "int", nullable: false),
                    FieldValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wf_WorkflowDefNodeRule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_wf_WorkflowDefNodeRule_wf_WorkflowDefNode_WorkflowNodeId",
                        column: x => x.WorkflowNodeId,
                        principalSchema: "cTest",
                        principalTable: "wf_WorkflowDefNode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "wf_WorkflowVerDefField",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowVerDefId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    TreeCode = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataType = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Value1 = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Value2 = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    IsPrimaryKey = table.Column<bool>(type: "bit", nullable: false),
                    IsExecutor = table.Column<bool>(type: "bit", nullable: false),
                    IsCondition = table.Column<bool>(type: "bit", nullable: false),
                    Leaf = table.Column<bool>(type: "bit", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wf_WorkflowVerDefField", x => x.Id);
                    table.ForeignKey(
                        name: "FK_wf_WorkflowVerDefField_wf_WorkflowVerDefField_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "cTest",
                        principalTable: "wf_WorkflowVerDefField",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_wf_WorkflowVerDefField_wf_WorkflowVerDefinition_WorkflowVerDefId",
                        column: x => x.WorkflowVerDefId,
                        principalSchema: "cTest",
                        principalTable: "wf_WorkflowVerDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "wf_WorkflowVerDefNode",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowVerDefId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    NodeType = table.Column<int>(type: "int", nullable: false),
                    DeadlineInterval = table.Column<int>(type: "int", nullable: true),
                    MessageTemplateCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LocTop = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LocLeft = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrevNodeCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    NextNodeCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ReturnNodeCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    SubDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubDefinitionCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SubDefinitionVersion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SubDefinitionName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    WeightValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExecutorSetting = table.Column<int>(type: "int", nullable: false),
                    ExecutorFormFieldName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExecutorFormFieldDisplayName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    NotifyUserIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotifyUserNames = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    OrgCodes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrgNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExceptUserIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExceptUserNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wf_WorkflowVerDefNode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_wf_WorkflowVerDefNode_wf_WorkflowVerDefinition_WorkflowVerDefId",
                        column: x => x.WorkflowVerDefId,
                        principalSchema: "cTest",
                        principalTable: "wf_WorkflowVerDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "wf_WorkflowProTaskExecute",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExecuteUserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ExecuteUserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ExecuteDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NextAuditorUserIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextAuditorUserNames = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ExecuteStatus = table.Column<int>(type: "int", nullable: false),
                    ExecuteFileBlob = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ExecuteRemark = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wf_WorkflowProTaskExecute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_wf_WorkflowProTaskExecute_wf_WorkflowProTask_TaskId",
                        column: x => x.TaskId,
                        principalSchema: "cTest",
                        principalTable: "wf_WorkflowProTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "wf_WorkflowProTaskRule",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RuleType = table.Column<int>(type: "int", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    FieldDisplayName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    OperatorType = table.Column<int>(type: "int", nullable: false),
                    FieldValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wf_WorkflowProTaskRule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_wf_WorkflowProTaskRule_wf_WorkflowProTask_TaskId",
                        column: x => x.TaskId,
                        principalSchema: "cTest",
                        principalTable: "wf_WorkflowProTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "wf_WorkflowVerDefNodeRule",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowVerNodeCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    WorkflowVerNodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RuleType = table.Column<int>(type: "int", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    FieldDisplayName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    OperatorType = table.Column<int>(type: "int", nullable: false),
                    FieldValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wf_WorkflowVerDefNodeRule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_wf_WorkflowVerDefNodeRule_wf_WorkflowVerDefNode_WorkflowVerNodeId",
                        column: x => x.WorkflowVerNodeId,
                        principalSchema: "cTest",
                        principalTable: "wf_WorkflowVerDefNode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_wf_ModelDefField_ModelDefId",
                schema: "cTest",
                table: "wf_ModelDefField",
                column: "ModelDefId");

            migrationBuilder.CreateIndex(
                name: "IX_wf_WorkflowCategory_ParentId",
                schema: "cTest",
                table: "wf_WorkflowCategory",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_wf_WorkflowDefField_ParentId",
                schema: "cTest",
                table: "wf_WorkflowDefField",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_wf_WorkflowDefField_WorkflowDefId",
                schema: "cTest",
                table: "wf_WorkflowDefField",
                column: "WorkflowDefId");

            migrationBuilder.CreateIndex(
                name: "IX_wf_WorkflowDefinition_CategoryId",
                schema: "cTest",
                table: "wf_WorkflowDefinition",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_wf_WorkflowDefNode_WorkflowDefId",
                schema: "cTest",
                table: "wf_WorkflowDefNode",
                column: "WorkflowDefId");

            migrationBuilder.CreateIndex(
                name: "IX_wf_WorkflowDefNodeRule_WorkflowNodeId",
                schema: "cTest",
                table: "wf_WorkflowDefNodeRule",
                column: "WorkflowNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_wf_WorkflowProcess_CategoryId",
                schema: "cTest",
                table: "wf_WorkflowProcess",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_wf_WorkflowProField_ParentId",
                schema: "cTest",
                table: "wf_WorkflowProField",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_wf_WorkflowProField_ProcessId",
                schema: "cTest",
                table: "wf_WorkflowProField",
                column: "ProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_wf_WorkflowProTask_ProcessId",
                schema: "cTest",
                table: "wf_WorkflowProTask",
                column: "ProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_wf_WorkflowProTaskExecute_TaskId",
                schema: "cTest",
                table: "wf_WorkflowProTaskExecute",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_wf_WorkflowProTaskRule_TaskId",
                schema: "cTest",
                table: "wf_WorkflowProTaskRule",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_wf_WorkflowVerDefField_ParentId",
                schema: "cTest",
                table: "wf_WorkflowVerDefField",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_wf_WorkflowVerDefField_WorkflowVerDefId",
                schema: "cTest",
                table: "wf_WorkflowVerDefField",
                column: "WorkflowVerDefId");

            migrationBuilder.CreateIndex(
                name: "IX_wf_WorkflowVerDefinition_CategoryId",
                schema: "cTest",
                table: "wf_WorkflowVerDefinition",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_wf_WorkflowVerDefinition_WorkflowVerDefId",
                schema: "cTest",
                table: "wf_WorkflowVerDefinition",
                column: "WorkflowVerDefId");

            migrationBuilder.CreateIndex(
                name: "IX_wf_WorkflowVerDefNode_WorkflowVerDefId",
                schema: "cTest",
                table: "wf_WorkflowVerDefNode",
                column: "WorkflowVerDefId");

            migrationBuilder.CreateIndex(
                name: "IX_wf_WorkflowVerDefNodeRule_WorkflowVerNodeId",
                schema: "cTest",
                table: "wf_WorkflowVerDefNodeRule",
                column: "WorkflowVerNodeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "wf_ModelDefField",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "wf_ModelDefLog",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "wf_WorkflowDefField",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "wf_WorkflowDefLog",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "wf_WorkflowDefNodeRule",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "wf_WorkflowProField",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "wf_WorkflowProRequestLog",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "wf_WorkflowProTaskExecute",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "wf_WorkflowProTaskRule",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "wf_WorkflowVerDefField",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "wf_WorkflowVerDefNodeRule",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "wf_ModelDefinition",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "wf_WorkflowDefNode",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "wf_WorkflowProTask",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "wf_WorkflowVerDefNode",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "wf_WorkflowProcess",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "wf_WorkflowVerDefinition",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "wf_WorkflowDefinition",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "wf_WorkflowCategory",
                schema: "cTest");
        }
    }
}
