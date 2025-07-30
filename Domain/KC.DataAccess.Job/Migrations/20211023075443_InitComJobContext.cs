using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KC.DataAccess.Job.Migrations
{
    public partial class InitComJobContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cTest");

            migrationBuilder.CreateTable(
                name: "job_DatabaseVersionInfo",
                schema: "cTest",
                columns: table => new
                {
                    RowKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DatabaseName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VersionNumber = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PartitionKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Tenant = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_DatabaseVersionInfo", x => x.RowKey);
                });

            migrationBuilder.CreateTable(
                name: "job_QueueErrorMessage",
                schema: "cTest",
                columns: table => new
                {
                    RowKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Tenant = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QueueType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QueueName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QueueMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceFrom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorFrom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PartitionKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_QueueErrorMessage", x => x.RowKey);
                });

            migrationBuilder.CreateTable(
                name: "job_ThreadConfigInfo",
                schema: "cTest",
                columns: table => new
                {
                    RowKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WorkerRoleId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HostName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    EnableLiveBackup = table.Column<bool>(type: "bit", nullable: false),
                    EnableEmailService = table.Column<bool>(type: "bit", nullable: false),
                    EnableNotificationService = table.Column<bool>(type: "bit", nullable: false),
                    EnableConversionService = table.Column<bool>(type: "bit", nullable: false),
                    EnableUploadProcessing = table.Column<bool>(type: "bit", nullable: false),
                    EnableConversionMonitor = table.Column<bool>(type: "bit", nullable: false),
                    EnableTenantSandboxService = table.Column<bool>(type: "bit", nullable: false),
                    EnableTenantArchiveService = table.Column<bool>(type: "bit", nullable: false),
                    EnableTenantRestoreService = table.Column<bool>(type: "bit", nullable: false),
                    EnableMetricDatabaseSync = table.Column<bool>(type: "bit", nullable: false),
                    EnableFullTextSearchIndexer = table.Column<bool>(type: "bit", nullable: false),
                    FullTextSearchCheckingIntervalInMinutes = table.Column<int>(type: "int", nullable: false),
                    ConversionTimeoutInMinutes = table.Column<int>(type: "int", nullable: false),
                    MaxRetryTimeForConversion = table.Column<int>(type: "int", nullable: false),
                    ConversionCheckInterval = table.Column<int>(type: "int", nullable: false),
                    SelectedPdfLibrary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KeepThisConfig = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifyTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastAccessTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PartitionKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Tenant = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_ThreadConfigInfo", x => x.RowKey);
                });

            migrationBuilder.CreateTable(
                name: "job_ThreadStatusInfo",
                schema: "cTest",
                columns: table => new
                {
                    RowKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThreadName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkerRoleId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Successes = table.Column<int>(type: "int", nullable: false),
                    Failures = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PartitionKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Tenant = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_ThreadStatusInfo", x => x.RowKey);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "job_DatabaseVersionInfo",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "job_QueueErrorMessage",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "job_ThreadConfigInfo",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "job_ThreadStatusInfo",
                schema: "cTest");
        }
    }
}
