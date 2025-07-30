using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KC.DataAccess.Customer.Migrations
{
    public partial class InitComCustomerContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cTest");

            migrationBuilder.CreateTable(
                name: "crm_CustomerExtInfoProvider",
                schema: "cTest",
                columns: table => new
                {
                    PropertyAttributeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Ext1 = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Ext2 = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Ext3 = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
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
                    table.PrimaryKey("PK_crm_CustomerExtInfoProvider", x => x.PropertyAttributeId);
                });

            migrationBuilder.CreateTable(
                name: "crm_CustomerInfo",
                schema: "cTest",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    CompanyType = table.Column<int>(type: "int", nullable: false),
                    ClientType = table.Column<int>(type: "int", nullable: false),
                    CustomerSource = table.Column<int>(type: "int", nullable: false),
                    BusinessModel = table.Column<int>(type: "int", nullable: false),
                    IndustryId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IndustryName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ContactId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReferenceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceId2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceId3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Introduction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecommandedUserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    RecommandedUserName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
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
                    table.PrimaryKey("PK_crm_CustomerInfo", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "crm_CustomerSendToTenantLog",
                schema: "cTest",
                columns: table => new
                {
                    ProcessLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerIdList = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    OperatorId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Operator = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OperateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crm_CustomerSendToTenantLog", x => x.ProcessLogId);
                });

            migrationBuilder.CreateTable(
                name: "crm_NotificationApplication",
                schema: "cTest",
                columns: table => new
                {
                    ApplicationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicantUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicantUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicantDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SendTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CcTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    viewName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_crm_NotificationApplication", x => x.ApplicationId);
                });

            migrationBuilder.CreateTable(
                name: "crm_CustomerAccount",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankType = table.Column<int>(type: "int", nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    BankNumber = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ProvinceId = table.Column<int>(type: "int", nullable: false),
                    ProvinceName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    CityName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DistrictId = table.Column<int>(type: "int", nullable: false),
                    DistrictName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    StreetId = table.Column<int>(type: "int", nullable: false),
                    StreetName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    BankAddress = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    ContactId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReferenceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceId2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceId3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_crm_CustomerAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_crm_CustomerAccount_crm_CustomerInfo_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "cTest",
                        principalTable: "crm_CustomerInfo",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "crm_CustomerAddress",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AddressType = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    ProvinceId = table.Column<int>(type: "int", nullable: false),
                    ProvinceName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    CityName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DistrictId = table.Column<int>(type: "int", nullable: false),
                    DistrictName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    StreetId = table.Column<int>(type: "int", nullable: false),
                    StreetName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    LongitudeX = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LatitudeY = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BaiduMapUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ZipCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ContactId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    ReferenceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceId2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceId3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_crm_CustomerAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_crm_CustomerAddress_crm_CustomerInfo_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "cTest",
                        principalTable: "crm_CustomerInfo",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "crm_CustomerAuthentication",
                schema: "cTest",
                columns: table => new
                {
                    CustomerCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
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
                    LetterOfAuthorityScannPhoto = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LegalPerson = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LegalPersonScannPhoto = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LegalPersonIdentityCardNumber = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: true),
                    LegalPersonIdentityCardPhoto = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LegalPersonIdentityCardPhotoOtherSide = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LegalPersonCertificateExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_crm_CustomerAuthentication", x => x.CustomerCode);
                    table.ForeignKey(
                        name: "FK_crm_CustomerAuthentication_crm_CustomerInfo_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "cTest",
                        principalTable: "crm_CustomerInfo",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "crm_CustomerChangeLog",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttributeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperatorId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Operator = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OperateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crm_CustomerChangeLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_crm_CustomerChangeLog_crm_CustomerInfo_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "cTest",
                        principalTable: "crm_CustomerInfo",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "crm_CustomerContact",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessType = table.Column<int>(type: "int", nullable: false),
                    ContactName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContactQQ = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ContactWeixin = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ContactPhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ContactTelephone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PositionName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    ReferenceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceId2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceId3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_crm_CustomerContact", x => x.Id);
                    table.ForeignKey(
                        name: "FK_crm_CustomerContact_crm_CustomerInfo_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "cTest",
                        principalTable: "crm_CustomerInfo",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "crm_CustomerExtInfo",
                schema: "cTest",
                columns: table => new
                {
                    PropertyAttributeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CustomerExtInfoProviderId = table.Column<int>(type: "int", nullable: true),
                    DataType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Ext1 = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Ext2 = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Ext3 = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
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
                    table.PrimaryKey("PK_crm_CustomerExtInfo", x => x.PropertyAttributeId);
                    table.ForeignKey(
                        name: "FK_crm_CustomerExtInfo_crm_CustomerExtInfoProvider_CustomerExtInfoProviderId",
                        column: x => x.CustomerExtInfoProviderId,
                        principalSchema: "cTest",
                        principalTable: "crm_CustomerExtInfoProvider",
                        principalColumn: "PropertyAttributeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_crm_CustomerExtInfo_crm_CustomerInfo_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "cTest",
                        principalTable: "crm_CustomerInfo",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "crm_CustomerManager",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerManagerId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CustomerManagerName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    OrganizationName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    IsInSeas = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crm_CustomerManager", x => x.Id);
                    table.ForeignKey(
                        name: "FK_crm_CustomerManager_crm_CustomerInfo_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "cTest",
                        principalTable: "crm_CustomerInfo",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "crm_CustomerSeas",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    OperatorId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Operator = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OperateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crm_CustomerSeas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_crm_CustomerSeas_crm_CustomerInfo_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "cTest",
                        principalTable: "crm_CustomerInfo",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "crm_CustomerTracingLog",
                schema: "cTest",
                columns: table => new
                {
                    ProcessLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferenceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    CustomerContactId = table.Column<int>(type: "int", nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ActivityId = table.Column<int>(type: "int", nullable: true),
                    ActivityName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Caller = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CallerPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Callee = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CalleePhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CallTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CallState = table.Column<int>(type: "int", nullable: true),
                    RecordURL = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    From = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SendTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CcTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TracingType = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    OperatorId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Operator = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OperateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crm_CustomerTracingLog", x => x.ProcessLogId);
                    table.ForeignKey(
                        name: "FK_crm_CustomerTracingLog_crm_CustomerInfo_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "cTest",
                        principalTable: "crm_CustomerInfo",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_crm_CustomerAccount_CustomerId",
                schema: "cTest",
                table: "crm_CustomerAccount",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_crm_CustomerAddress_CustomerId",
                schema: "cTest",
                table: "crm_CustomerAddress",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_crm_CustomerAuthentication_CustomerId",
                schema: "cTest",
                table: "crm_CustomerAuthentication",
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_crm_CustomerChangeLog_CustomerId",
                schema: "cTest",
                table: "crm_CustomerChangeLog",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_crm_CustomerContact_CustomerId",
                schema: "cTest",
                table: "crm_CustomerContact",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_crm_CustomerExtInfo_CustomerExtInfoProviderId",
                schema: "cTest",
                table: "crm_CustomerExtInfo",
                column: "CustomerExtInfoProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_crm_CustomerExtInfo_CustomerId",
                schema: "cTest",
                table: "crm_CustomerExtInfo",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_crm_CustomerManager_CustomerId",
                schema: "cTest",
                table: "crm_CustomerManager",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_crm_CustomerSeas_CustomerId",
                schema: "cTest",
                table: "crm_CustomerSeas",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_crm_CustomerTracingLog_CustomerId",
                schema: "cTest",
                table: "crm_CustomerTracingLog",
                column: "CustomerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "crm_CustomerAccount",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "crm_CustomerAddress",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "crm_CustomerAuthentication",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "crm_CustomerChangeLog",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "crm_CustomerContact",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "crm_CustomerExtInfo",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "crm_CustomerManager",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "crm_CustomerSeas",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "crm_CustomerSendToTenantLog",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "crm_CustomerTracingLog",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "crm_NotificationApplication",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "crm_CustomerExtInfoProvider",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "crm_CustomerInfo",
                schema: "cTest");
        }
    }
}
