using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KC.DataAccess.Admin.Migrations
{
    public partial class InitComAdminContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            #region Create Table
            migrationBuilder.EnsureSchema(
                name: "cDba");

            migrationBuilder.CreateTable(
                name: "tenant_DatabasePool",
                schema: "cDba",
                columns: table => new
                {
                    DatabasePoolId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CloudType = table.Column<int>(type: "int", nullable: false),
                    DatabaseType = table.Column<int>(type: "int", nullable: false),
                    Server = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Database = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserPasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordExpiredTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantCount = table.Column<int>(type: "int", nullable: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_tenant_DatabasePool", x => x.DatabasePoolId);
                });

            migrationBuilder.CreateTable(
                name: "tenant_NoSqlPool",
                schema: "cDba",
                columns: table => new
                {
                    NoSqlPoolId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CloudType = table.Column<int>(type: "int", nullable: false),
                    NoSqlType = table.Column<int>(type: "int", nullable: false),
                    Endpoint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccessName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccessKeyPasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordExpiredTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantCount = table.Column<int>(type: "int", nullable: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    Extend1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extend2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extend3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_tenant_NoSqlPool", x => x.NoSqlPoolId);
                });

            migrationBuilder.CreateTable(
                name: "tenant_QueuePool",
                schema: "cDba",
                columns: table => new
                {
                    QueuePoolId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CloudType = table.Column<int>(type: "int", nullable: false),
                    QueueType = table.Column<int>(type: "int", nullable: false),
                    Endpoint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccessName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccessKeyPasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordExpiredTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantCount = table.Column<int>(type: "int", nullable: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    Extend1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extend2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extend3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_tenant_QueuePool", x => x.QueuePoolId);
                });

            migrationBuilder.CreateTable(
                name: "tenant_ServiceBusPool",
                schema: "cDba",
                columns: table => new
                {
                    ServiceBusPoolId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CloudType = table.Column<int>(type: "int", nullable: false),
                    ServiceBusType = table.Column<int>(type: "int", nullable: false),
                    Endpoint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccessName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccessKeyPasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordExpiredTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantCount = table.Column<int>(type: "int", nullable: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    Extend1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extend2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extend3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_tenant_ServiceBusPool", x => x.ServiceBusPoolId);
                });

            migrationBuilder.CreateTable(
                name: "tenant_StoragePool",
                schema: "cDba",
                columns: table => new
                {
                    StoragePoolId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CloudType = table.Column<int>(type: "int", nullable: false),
                    StorageType = table.Column<int>(type: "int", nullable: false),
                    Endpoint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccessName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccessKeyPasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordExpiredTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantCount = table.Column<int>(type: "int", nullable: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    Extend1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extend2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extend3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_tenant_StoragePool", x => x.StoragePoolId);
                });

            migrationBuilder.CreateTable(
                name: "tenant_TenantUserChargeSms",
                schema: "cDba",
                columns: table => new
                {
                    ChargeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TenantDisplayName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    RefCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ChargeNumber = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    ErrorLog = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_tenant_TenantUserChargeSms", x => x.ChargeId);
                });

            migrationBuilder.CreateTable(
                name: "tenant_TenantUserChargeStorage",
                schema: "cDba",
                columns: table => new
                {
                    ChargeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TenantDisplayName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    FileId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FileType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FileFormat = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    Ext = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    ErrorLog = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_tenant_TenantUserChargeStorage", x => x.ChargeId);
                });

            migrationBuilder.CreateTable(
                name: "tenant_TenantUserLoopTask",
                schema: "cDba",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    TenantName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
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
                    table.PrimaryKey("PK_tenant_TenantUserLoopTask", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tenant_TenantUserMaterial",
                schema: "cDba",
                columns: table => new
                {
                    MaterialId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TenantDisplayName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    IsCustomer = table.Column<bool>(type: "bit", nullable: false),
                    CompanyCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    ContactId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsInnerPush = table.Column<bool>(type: "bit", nullable: false),
                    MaterialRefCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MaterialCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MaterialName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    MaterialTypeCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    MaterialTypeName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    MaterialUnit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MaterialImageBlob = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MaterialFileBlob = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MaterialPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaterialDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaterialRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaterialAddress = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Barcode = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    ReferenceId1 = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ReferenceId2 = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ReferenceId3 = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
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
                    table.PrimaryKey("PK_tenant_TenantUserMaterial", x => x.MaterialId);
                });

            migrationBuilder.CreateTable(
                name: "tenant_TenantUserOffering",
                schema: "cDba",
                columns: table => new
                {
                    RecommendId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TenantDisplayName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    IsCustomer = table.Column<bool>(type: "bit", nullable: false),
                    CompanyCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    ContactId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsInnerPush = table.Column<bool>(type: "bit", nullable: false),
                    RecommendRefCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RecommendCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RecommendName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    OfferingTypeCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    OfferingTypeName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    OfferingUnit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    OfferingImageBlob = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OfferingFileBlob = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OfferingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OfferingDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OfferingRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OfferingAddress = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Barcode = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    ReferenceId1 = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ReferenceId2 = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ReferenceId3 = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
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
                    table.PrimaryKey("PK_tenant_TenantUserOffering", x => x.RecommendId);
                });

            migrationBuilder.CreateTable(
                name: "tenant_TenantUserRequirement",
                schema: "cDba",
                columns: table => new
                {
                    RecommendId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TenantDisplayName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    IsCustomer = table.Column<bool>(type: "bit", nullable: false),
                    CompanyCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    ContactId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsInnerPush = table.Column<bool>(type: "bit", nullable: false),
                    RecommendRefCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RecommendCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RecommendName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequirementTypeCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    RequirementTypeName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    RecommendAddress = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ExpiredDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsPublish = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Barcode = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    ReferenceId1 = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ReferenceId2 = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ReferenceId3 = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
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
                    table.PrimaryKey("PK_tenant_TenantUserRequirement", x => x.RecommendId);
                });

            migrationBuilder.CreateTable(
                name: "tenant_TenantUserServiceApplication",
                schema: "cDba",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    TenantName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceType = table.Column<int>(type: "int", nullable: false),
                    AppStatus = table.Column<int>(type: "int", nullable: false),
                    WorkFlowStatus = table.Column<int>(type: "int", nullable: false),
                    OperatorId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperatorDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_tenant_TenantUserServiceApplication", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tenant_VodPool",
                schema: "cDba",
                columns: table => new
                {
                    VodPoolId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CloudType = table.Column<int>(type: "int", nullable: false),
                    VodType = table.Column<int>(type: "int", nullable: false),
                    Endpoint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccessName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccessKeyPasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordExpiredTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantCount = table.Column<int>(type: "int", nullable: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    Extend1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extend2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extend3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_tenant_VodPool", x => x.VodPoolId);
                });

            migrationBuilder.CreateTable(
                name: "tenant_RequirementForMaterial",
                schema: "cDba",
                columns: table => new
                {
                    RequirementId = table.Column<int>(type: "int", nullable: false),
                    MaterialId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_tenant_RequirementForMaterial", x => new { x.RequirementId, x.MaterialId });
                    table.ForeignKey(
                        name: "FK_tenant_RequirementForMaterial_tenant_TenantUserMaterial_MaterialId",
                        column: x => x.MaterialId,
                        principalSchema: "cDba",
                        principalTable: "tenant_TenantUserMaterial",
                        principalColumn: "MaterialId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tenant_RequirementForMaterial_tenant_TenantUserRequirement_RequirementId",
                        column: x => x.RequirementId,
                        principalSchema: "cDba",
                        principalTable: "tenant_TenantUserRequirement",
                        principalColumn: "RecommendId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tenant_TenantUser",
                schema: "cDba",
                columns: table => new
                {
                    TenantId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessModel = table.Column<int>(type: "int", nullable: false),
                    TenantName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TenantDisplayName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    TenantSignature = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TenantType = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CloudType = table.Column<int>(type: "int", nullable: false),
                    TenantLogo = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TenantIntroduction = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    PrivateEncryptKey = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NickName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NickNameLastModifyDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordExpiredTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DatabaseType = table.Column<int>(type: "int", nullable: false),
                    DatabasePoolId = table.Column<int>(type: "int", nullable: false),
                    Server = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Database = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DatabasePasswordHash = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    StorageType = table.Column<int>(type: "int", nullable: false),
                    StoragePoolId = table.Column<int>(type: "int", nullable: true),
                    StorageEndpoint = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    StorageAccessName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    StorageAccessKeyPasswordHash = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    QueueType = table.Column<int>(type: "int", nullable: false),
                    QueuePoolId = table.Column<int>(type: "int", nullable: true),
                    QueueEndpoint = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    QueueAccessName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    QueueAccessKeyPasswordHash = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    NoSqlType = table.Column<int>(type: "int", nullable: false),
                    NoSqlPoolId = table.Column<int>(type: "int", nullable: true),
                    NoSqlEndpoint = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    NoSqlAccessName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    NoSqlAccessKeyPasswordHash = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ServiceBusType = table.Column<int>(type: "int", nullable: false),
                    ServiceBusPoolId = table.Column<int>(type: "int", nullable: true),
                    ServiceBusEndpoint = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ServiceBusAccessName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ServiceBusAccessKeyPasswordHash = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    VodType = table.Column<int>(type: "int", nullable: false),
                    VodPoolId = table.Column<int>(type: "int", nullable: true),
                    VodEndpoint = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    VodAccessName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    VodAccessKeyPasswordHash = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IndustryId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IndustryName = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    ReferenceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_tenant_TenantUser", x => x.TenantId);
                    table.ForeignKey(
                        name: "FK_tenant_TenantUser_tenant_DatabasePool_DatabasePoolId",
                        column: x => x.DatabasePoolId,
                        principalSchema: "cDba",
                        principalTable: "tenant_DatabasePool",
                        principalColumn: "DatabasePoolId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tenant_TenantUser_tenant_NoSqlPool_NoSqlPoolId",
                        column: x => x.NoSqlPoolId,
                        principalSchema: "cDba",
                        principalTable: "tenant_NoSqlPool",
                        principalColumn: "NoSqlPoolId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tenant_TenantUser_tenant_QueuePool_QueuePoolId",
                        column: x => x.QueuePoolId,
                        principalSchema: "cDba",
                        principalTable: "tenant_QueuePool",
                        principalColumn: "QueuePoolId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tenant_TenantUser_tenant_ServiceBusPool_ServiceBusPoolId",
                        column: x => x.ServiceBusPoolId,
                        principalSchema: "cDba",
                        principalTable: "tenant_ServiceBusPool",
                        principalColumn: "ServiceBusPoolId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tenant_TenantUser_tenant_StoragePool_StoragePoolId",
                        column: x => x.StoragePoolId,
                        principalSchema: "cDba",
                        principalTable: "tenant_StoragePool",
                        principalColumn: "StoragePoolId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tenant_TenantUser_tenant_VodPool_VodPoolId",
                        column: x => x.VodPoolId,
                        principalSchema: "cDba",
                        principalTable: "tenant_VodPool",
                        principalColumn: "VodPoolId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tenant_TenantUserApplication",
                schema: "cDba",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DomainName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppStatus = table.Column<int>(type: "int", nullable: false),
                    WebSiteName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    AssemblyName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_TenantUserApplication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tenant_TenantUserApplication_tenant_TenantUser_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "cDba",
                        principalTable: "tenant_TenantUser",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tenant_TenantUserAuthentication",
                schema: "cDba",
                columns: table => new
                {
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    ProvinceId = table.Column<int>(type: "int", nullable: true),
                    ProvinceName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: true),
                    CityName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DistrictId = table.Column<int>(type: "int", nullable: true),
                    DistrictName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CompanyAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BulidDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BusinessDateLimit = table.Column<int>(type: "int", nullable: true),
                    IsLongTerm = table.Column<bool>(type: "bit", nullable: true),
                    RegisteredCapital = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScopeOfBusiness = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UnifiedSocialCreditCode = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: true),
                    UnifiedSocialCreditCodeScannPhoto = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LegalPerson = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LegalPersonScannPhoto = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LegalPersonIdentityCardNumber = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: true),
                    LegalPersonIdentityCardPhoto = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LegalPersonIdentityCardPhotoOtherSide = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LegalPersonCertificateExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LetterOfAuthorityScannPhoto = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EnterpriseQualificationPhoto = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AuditComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_tenant_TenantUserAuthentication", x => x.TenantId);
                    table.ForeignKey(
                        name: "FK_tenant_TenantUserAuthentication_tenant_TenantUser_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "cDba",
                        principalTable: "tenant_TenantUser",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tenant_TenantUserOpenAppErrorLog",
                schema: "cDba",
                columns: table => new
                {
                    ProcessLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpenServerType = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    OperatorId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Operator = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OperateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_TenantUserOpenAppErrorLog", x => x.ProcessLogId);
                    table.ForeignKey(
                        name: "FK_tenant_TenantUserOpenAppErrorLog_tenant_TenantUser_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "cDba",
                        principalTable: "tenant_TenantUser",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tenant_TenantUserSetting",
                schema: "cDba",
                columns: table => new
                {
                    PropertyAttributeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    DataType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRequire = table.Column<bool>(type: "bit", nullable: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
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
                    table.PrimaryKey("PK_tenant_TenantUserSetting", x => x.PropertyAttributeId);
                    table.ForeignKey(
                        name: "FK_tenant_TenantUserSetting_tenant_TenantUser_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "cDba",
                        principalTable: "tenant_TenantUser",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tenant_TenantUserOperationLog",
                schema: "cDba",
                columns: table => new
                {
                    ProcessLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DomainName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    AppStatus = table.Column<int>(type: "int", nullable: false),
                    TenantName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogType = table.Column<int>(type: "int", nullable: false),
                    AdditionalInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantUserApplicationId = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    OperatorId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Operator = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OperateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_TenantUserOperationLog", x => x.ProcessLogId);
                    table.ForeignKey(
                        name: "FK_tenant_TenantUserOperationLog_tenant_TenantUserApplication_TenantUserApplicationId",
                        column: x => x.TenantUserApplicationId,
                        principalSchema: "cDba",
                        principalTable: "tenant_TenantUserApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tenant_RequirementForMaterial_MaterialId",
                schema: "cDba",
                table: "tenant_RequirementForMaterial",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_TenantUser_DatabasePoolId",
                schema: "cDba",
                table: "tenant_TenantUser",
                column: "DatabasePoolId");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_TenantUser_NoSqlPoolId",
                schema: "cDba",
                table: "tenant_TenantUser",
                column: "NoSqlPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_TenantUser_QueuePoolId",
                schema: "cDba",
                table: "tenant_TenantUser",
                column: "QueuePoolId");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_TenantUser_ServiceBusPoolId",
                schema: "cDba",
                table: "tenant_TenantUser",
                column: "ServiceBusPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_TenantUser_StoragePoolId",
                schema: "cDba",
                table: "tenant_TenantUser",
                column: "StoragePoolId");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_TenantUser_VodPoolId",
                schema: "cDba",
                table: "tenant_TenantUser",
                column: "VodPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_TenantUserApplication_TenantId",
                schema: "cDba",
                table: "tenant_TenantUserApplication",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_TenantUserOpenAppErrorLog_TenantId",
                schema: "cDba",
                table: "tenant_TenantUserOpenAppErrorLog",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_TenantUserOperationLog_TenantUserApplicationId",
                schema: "cDba",
                table: "tenant_TenantUserOperationLog",
                column: "TenantUserApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_TenantUserSetting_TenantId",
                schema: "cDba",
                table: "tenant_TenantUserSetting",
                column: "TenantId");
            #endregion

            #region Init Database Structure with sql: KC.DataAccess.Admin

            var sqlBuilder = new System.Text.StringBuilder();
            var sqlStatements = KC.DataAccess.Admin.DataInitial.DBSqlInitializer.GetStructureSqlScript();
            foreach (var line in sqlStatements)
            {
                string sqlLine = line;
                if (line.Trim().ToUpper() == "GO")
                {
                    string sql = sqlBuilder.ToString();
                    if (!string.IsNullOrWhiteSpace(sql))
                        migrationBuilder.Sql(sql);
                    sqlBuilder.Clear();
                }
                else
                {
                    sqlBuilder.AppendLine(sqlLine);
                }
            }

            #endregion

            #region Insert data into Database with sql: KC.DataAccess.Admin

            sqlBuilder = new System.Text.StringBuilder();
            sqlStatements = KC.DataAccess.Admin.DataInitial.DBSqlInitializer.GetInitialDataSqlScript();
            foreach (var line in sqlStatements)
            {
                string sqlLine = line;
                if (line.Trim().ToUpper() == "GO")
                {
                    string sql = sqlBuilder.ToString();
                    if (!string.IsNullOrWhiteSpace(sql))
                        migrationBuilder.Sql(sql);
                    sqlBuilder.Clear();
                }
                else
                {
                    sqlBuilder.AppendLine(sqlLine);
                }
            }

            #endregion
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tenant_RequirementForMaterial",
                schema: "cDba");

            migrationBuilder.DropTable(
                name: "tenant_TenantUserAuthentication",
                schema: "cDba");

            migrationBuilder.DropTable(
                name: "tenant_TenantUserChargeSms",
                schema: "cDba");

            migrationBuilder.DropTable(
                name: "tenant_TenantUserChargeStorage",
                schema: "cDba");

            migrationBuilder.DropTable(
                name: "tenant_TenantUserLoopTask",
                schema: "cDba");

            migrationBuilder.DropTable(
                name: "tenant_TenantUserOffering",
                schema: "cDba");

            migrationBuilder.DropTable(
                name: "tenant_TenantUserOpenAppErrorLog",
                schema: "cDba");

            migrationBuilder.DropTable(
                name: "tenant_TenantUserOperationLog",
                schema: "cDba");

            migrationBuilder.DropTable(
                name: "tenant_TenantUserServiceApplication",
                schema: "cDba");

            migrationBuilder.DropTable(
                name: "tenant_TenantUserSetting",
                schema: "cDba");

            migrationBuilder.DropTable(
                name: "tenant_TenantUserMaterial",
                schema: "cDba");

            migrationBuilder.DropTable(
                name: "tenant_TenantUserRequirement",
                schema: "cDba");

            migrationBuilder.DropTable(
                name: "tenant_TenantUserApplication",
                schema: "cDba");

            migrationBuilder.DropTable(
                name: "tenant_TenantUser",
                schema: "cDba");

            migrationBuilder.DropTable(
                name: "tenant_DatabasePool",
                schema: "cDba");

            migrationBuilder.DropTable(
                name: "tenant_NoSqlPool",
                schema: "cDba");

            migrationBuilder.DropTable(
                name: "tenant_QueuePool",
                schema: "cDba");

            migrationBuilder.DropTable(
                name: "tenant_ServiceBusPool",
                schema: "cDba");

            migrationBuilder.DropTable(
                name: "tenant_StoragePool",
                schema: "cDba");

            migrationBuilder.DropTable(
                name: "tenant_VodPool",
                schema: "cDba");
        }
    }
}
