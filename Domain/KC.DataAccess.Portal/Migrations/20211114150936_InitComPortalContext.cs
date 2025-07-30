using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KC.DataAccess.Portal.Migrations
{
    public partial class InitComPortalContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cTest");

            migrationBuilder.CreateTable(
                name: "ptl_CompanyAuthenticationFailedRecord",
                schema: "cTest",
                columns: table => new
                {
                    ProcessLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KeyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    OperatorId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Operator = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OperateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ptl_CompanyAuthenticationFailedRecord", x => x.ProcessLogId);
                });

            migrationBuilder.CreateTable(
                name: "ptl_CompanyInfo",
                schema: "cTest",
                columns: table => new
                {
                    CompanyCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    CompanyLogo = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    BusinessModel = table.Column<int>(type: "int", nullable: false),
                    IndustryId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IndustryName = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    ContactId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceId2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceId3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Introduction = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
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
                    table.PrimaryKey("PK_ptl_CompanyInfo", x => x.CompanyCode);
                });

            migrationBuilder.CreateTable(
                name: "ptl_CompanyProcessLog",
                schema: "cTest",
                columns: table => new
                {
                    ProcessLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    OperatorId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Operator = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OperateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ptl_CompanyProcessLog", x => x.ProcessLogId);
                });

            migrationBuilder.CreateTable(
                name: "ptl_FavoriteInfo",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FavoriteType = table.Column<int>(type: "int", nullable: false),
                    RelationId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    RelationName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    RelationImg = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CompanyDomain = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ConcernedUserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ConcernedUserName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
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
                    table.PrimaryKey("PK_ptl_FavoriteInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ptl_RecommendCategory",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageBlob = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    FileBlob = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsShow = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    TreeCode = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Leaf = table.Column<bool>(type: "bit", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ptl_RecommendCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ptl_RecommendCategory_ptl_RecommendCategory_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "cTest",
                        principalTable: "ptl_RecommendCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ptl_RecommendCustomerLog",
                schema: "cTest",
                columns: table => new
                {
                    ProcessLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecommendId = table.Column<int>(type: "int", nullable: false),
                    RecommendRefCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RecommendCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RecommendName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    OperatorId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Operator = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OperateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ptl_RecommendCustomerLog", x => x.ProcessLogId);
                });

            migrationBuilder.CreateTable(
                name: "ptl_RecommendOfferingLog",
                schema: "cTest",
                columns: table => new
                {
                    ProcessLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecommendId = table.Column<int>(type: "int", nullable: false),
                    RecommendRefCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RecommendCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RecommendName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    OperatorId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Operator = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OperateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ptl_RecommendOfferingLog", x => x.ProcessLogId);
                });

            migrationBuilder.CreateTable(
                name: "ptl_RecommendRequirementLog",
                schema: "cTest",
                columns: table => new
                {
                    ProcessLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecommendId = table.Column<int>(type: "int", nullable: false),
                    RecommendRefCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RecommendCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RecommendName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    OperatorId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Operator = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OperateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ptl_RecommendRequirementLog", x => x.ProcessLogId);
                });

            migrationBuilder.CreateTable(
                name: "ptl_WebSiteInfo",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ServiceDate = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ServiceTime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LogoImage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    QRCode = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    HomePageSlide = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    MainImage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ContactId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContactQQ = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ContactWeixin = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ContactPhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ContactTelephone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    KeyWord = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    KeyDescription = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SkinCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SkinName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CompanyInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_ptl_WebSiteInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ptl_WebSiteLink",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    LinkType = table.Column<int>(type: "int", nullable: false),
                    Links = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    MetaKeywords = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MetaDescription = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MetaTitle = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsEnable = table.Column<bool>(type: "bit", nullable: false),
                    IsNav = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_ptl_WebSiteLink", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ptl_WebSitePage",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkinCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SkinName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsEnable = table.Column<bool>(type: "bit", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    LinkIsOpenNewPage = table.Column<bool>(type: "bit", nullable: false),
                    MainColor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SecondaryColor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UseMainSlide = table.Column<bool>(type: "bit", nullable: false),
                    MainSlide = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CustomizeContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_ptl_WebSitePage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ptl_WebSitePageLog",
                schema: "cTest",
                columns: table => new
                {
                    ProcessLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WebSiteColumnName = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    WebSiteColumnId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    OperatorId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Operator = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OperateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ptl_WebSitePageLog", x => x.ProcessLogId);
                });

            migrationBuilder.CreateTable(
                name: "ptl_WebSiteTemplateColumn",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    SubTitle = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ItemType = table.Column<int>(type: "int", nullable: false),
                    RowCount = table.Column<int>(type: "int", nullable: false),
                    ColumnCount = table.Column<int>(type: "int", nullable: false),
                    IsImage = table.Column<bool>(type: "bit", nullable: false),
                    ImageOrIConCls = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Link = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    LinkIsOpenNewPage = table.Column<bool>(type: "bit", nullable: false),
                    IsShow = table.Column<bool>(type: "bit", nullable: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CustomizeContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_ptl_WebSiteTemplateColumn", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ptl_CompanyAccount",
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
                    ProvinceCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProvinceName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    CityCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CityName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DistrictId = table.Column<int>(type: "int", nullable: false),
                    DistrictCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DistrictName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    BankAddress = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    ContactId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsPublish = table.Column<bool>(type: "bit", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    CompanyCode = table.Column<string>(type: "nvarchar(20)", nullable: true),
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
                    table.PrimaryKey("PK_ptl_CompanyAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ptl_CompanyAccount_ptl_CompanyInfo_CompanyCode",
                        column: x => x.CompanyCode,
                        principalSchema: "cTest",
                        principalTable: "ptl_CompanyInfo",
                        principalColumn: "CompanyCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ptl_CompanyAddress",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AddressType = table.Column<int>(type: "int", nullable: false),
                    ProvinceId = table.Column<int>(type: "int", nullable: false),
                    ProvinceCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProvinceName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    CityCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CityName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DistrictId = table.Column<int>(type: "int", nullable: false),
                    DistrictCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DistrictName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    StreetId = table.Column<int>(type: "int", nullable: false),
                    StreetCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StreetName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    LongitudeX = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LatitudeY = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BaiduMapUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ZipCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ContactId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsPublish = table.Column<bool>(type: "bit", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    CompanyCode = table.Column<string>(type: "nvarchar(20)", nullable: true),
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
                    table.PrimaryKey("PK_ptl_CompanyAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ptl_CompanyAddress_ptl_CompanyInfo_CompanyCode",
                        column: x => x.CompanyCode,
                        principalSchema: "cTest",
                        principalTable: "ptl_CompanyInfo",
                        principalColumn: "CompanyCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ptl_CompanyAuthentication",
                schema: "cTest",
                columns: table => new
                {
                    CompanyCode = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    ProvinceId = table.Column<int>(type: "int", nullable: false),
                    ProvinceCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProvinceName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    CityCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CityName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DistrictId = table.Column<int>(type: "int", nullable: false),
                    DistrictCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_ptl_CompanyAuthentication", x => x.CompanyCode);
                    table.ForeignKey(
                        name: "FK_ptl_CompanyAuthentication_ptl_CompanyInfo_CompanyCode",
                        column: x => x.CompanyCode,
                        principalSchema: "cTest",
                        principalTable: "ptl_CompanyInfo",
                        principalColumn: "CompanyCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ptl_CompanyContact",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessType = table.Column<int>(type: "int", nullable: false),
                    ContactId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ContactPhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ContactTelephone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ContactQQ = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ContactWeixin = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    PositionName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsPublish = table.Column<bool>(type: "bit", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false),
                    CompanyCode = table.Column<string>(type: "nvarchar(20)", nullable: true),
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
                    table.PrimaryKey("PK_ptl_CompanyContact", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ptl_CompanyContact_ptl_CompanyInfo_CompanyCode",
                        column: x => x.CompanyCode,
                        principalSchema: "cTest",
                        principalTable: "ptl_CompanyInfo",
                        principalColumn: "CompanyCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ptl_RecommendCustomer",
                schema: "cTest",
                columns: table => new
                {
                    RecommendId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsInnerPush = table.Column<bool>(type: "bit", nullable: false),
                    RecommendRefCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RecommendCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RecommendName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsTop = table.Column<bool>(type: "bit", nullable: false),
                    CompanyType = table.Column<int>(type: "int", nullable: false),
                    CompanyLogo = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    BusinessModel = table.Column<int>(type: "int", nullable: false),
                    IndustryId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IndustryName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ContactId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerDomain = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceId1 = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ReferenceId2 = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ReferenceId3 = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_ptl_RecommendCustomer", x => x.RecommendId);
                    table.ForeignKey(
                        name: "FK_ptl_RecommendCustomer_ptl_RecommendCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "cTest",
                        principalTable: "ptl_RecommendCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ptl_RecommendMaterial",
                schema: "cTest",
                columns: table => new
                {
                    MaterialId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsInnerPush = table.Column<bool>(type: "bit", nullable: false),
                    MaterialRefCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MaterialCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MaterialName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    MaterialTypeCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    MaterialTypeName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    MaterialUnit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MaterialImage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MaterialFile = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MaterialPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaterialDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaterialRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Barcode = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    IsCustomer = table.Column<bool>(type: "bit", nullable: false),
                    CompanyCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    ContactId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReferenceId1 = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ReferenceId2 = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ReferenceId3 = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_ptl_RecommendMaterial", x => x.MaterialId);
                    table.ForeignKey(
                        name: "FK_ptl_RecommendMaterial_ptl_RecommendCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "cTest",
                        principalTable: "ptl_RecommendCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ptl_RecommendOffering",
                schema: "cTest",
                columns: table => new
                {
                    RecommendId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsInnerPush = table.Column<bool>(type: "bit", nullable: false),
                    RecommendRefCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RecommendCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RecommendName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsTop = table.Column<bool>(type: "bit", nullable: false),
                    OfferingTypeCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    OfferingTypeName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    OfferingUnit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    OfferingImage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    OfferingFile = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    OfferingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OfferingDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OfferingRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Barcode = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    IsCustomer = table.Column<bool>(type: "bit", nullable: false),
                    CompanyCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    ContactId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReferenceId1 = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ReferenceId2 = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ReferenceId3 = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_ptl_RecommendOffering", x => x.RecommendId);
                    table.ForeignKey(
                        name: "FK_ptl_RecommendOffering_ptl_RecommendCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "cTest",
                        principalTable: "ptl_RecommendCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ptl_RecommendRequirement",
                schema: "cTest",
                columns: table => new
                {
                    RecommendId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsInnerPush = table.Column<bool>(type: "bit", nullable: false),
                    RecommendRefCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RecommendCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RecommendName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsTop = table.Column<bool>(type: "bit", nullable: false),
                    RequirementType = table.Column<int>(type: "int", nullable: false),
                    ProvinceId = table.Column<int>(type: "int", nullable: false),
                    ProvinceCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProvinceName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    CityCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CityName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    RequirementImage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RequirementFile = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ExpiredDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsPublish = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Barcode = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    IsCustomer = table.Column<bool>(type: "bit", nullable: false),
                    CompanyCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    ContactId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReferenceId1 = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ReferenceId2 = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ReferenceId3 = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_ptl_RecommendRequirement", x => x.RecommendId);
                    table.ForeignKey(
                        name: "FK_ptl_RecommendRequirement_ptl_RecommendCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "cTest",
                        principalTable: "ptl_RecommendCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ptl_WebSiteColumn",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    SubTitle = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ItemType = table.Column<int>(type: "int", nullable: false),
                    RowCount = table.Column<int>(type: "int", nullable: false),
                    ColumnCount = table.Column<int>(type: "int", nullable: false),
                    IsShow = table.Column<bool>(type: "bit", nullable: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CustomizeContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebSitePageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_ptl_WebSiteColumn", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ptl_WebSiteColumn_ptl_WebSitePage_WebSitePageId",
                        column: x => x.WebSitePageId,
                        principalSchema: "cTest",
                        principalTable: "ptl_WebSitePage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ptl_WebSiteTemplateItem",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    SubTitle = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsImage = table.Column<bool>(type: "bit", nullable: false),
                    ImageOrIConCls = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Link = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    LinkIsOpenNewPage = table.Column<bool>(type: "bit", nullable: false),
                    IsShow = table.Column<bool>(type: "bit", nullable: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CustomizeContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebSiteTemplateColumnId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_ptl_WebSiteTemplateItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ptl_WebSiteTemplateItem_ptl_WebSiteTemplateColumn_WebSiteTemplateColumnId",
                        column: x => x.WebSiteTemplateColumnId,
                        principalSchema: "cTest",
                        principalTable: "ptl_WebSiteTemplateColumn",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ptl_RequirementForMaterial",
                schema: "cTest",
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
                    table.PrimaryKey("PK_ptl_RequirementForMaterial", x => new { x.RequirementId, x.MaterialId });
                    table.ForeignKey(
                        name: "FK_ptl_RequirementForMaterial_ptl_RecommendMaterial_MaterialId",
                        column: x => x.MaterialId,
                        principalSchema: "cTest",
                        principalTable: "ptl_RecommendMaterial",
                        principalColumn: "MaterialId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ptl_RequirementForMaterial_ptl_RecommendRequirement_RequirementId",
                        column: x => x.RequirementId,
                        principalSchema: "cTest",
                        principalTable: "ptl_RecommendRequirement",
                        principalColumn: "RecommendId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ptl_WebSiteItem",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    SubTitle = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsImage = table.Column<bool>(type: "bit", nullable: false),
                    ImageOrIConCls = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Link = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    LinkIsOpenNewPage = table.Column<bool>(type: "bit", nullable: false),
                    IsShow = table.Column<bool>(type: "bit", nullable: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CustomizeContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebSiteColumnId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_ptl_WebSiteItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ptl_WebSiteItem_ptl_WebSiteColumn_WebSiteColumnId",
                        column: x => x.WebSiteColumnId,
                        principalSchema: "cTest",
                        principalTable: "ptl_WebSiteColumn",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ptl_CompanyAccount_CompanyCode",
                schema: "cTest",
                table: "ptl_CompanyAccount",
                column: "CompanyCode");

            migrationBuilder.CreateIndex(
                name: "IX_ptl_CompanyAddress_CompanyCode",
                schema: "cTest",
                table: "ptl_CompanyAddress",
                column: "CompanyCode");

            migrationBuilder.CreateIndex(
                name: "IX_ptl_CompanyContact_CompanyCode",
                schema: "cTest",
                table: "ptl_CompanyContact",
                column: "CompanyCode");

            migrationBuilder.CreateIndex(
                name: "IX_ptl_RecommendCategory_ParentId",
                schema: "cTest",
                table: "ptl_RecommendCategory",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ptl_RecommendCustomer_CategoryId",
                schema: "cTest",
                table: "ptl_RecommendCustomer",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ptl_RecommendMaterial_CategoryId",
                schema: "cTest",
                table: "ptl_RecommendMaterial",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ptl_RecommendOffering_CategoryId",
                schema: "cTest",
                table: "ptl_RecommendOffering",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ptl_RecommendRequirement_CategoryId",
                schema: "cTest",
                table: "ptl_RecommendRequirement",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ptl_RequirementForMaterial_MaterialId",
                schema: "cTest",
                table: "ptl_RequirementForMaterial",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_ptl_WebSiteColumn_WebSitePageId",
                schema: "cTest",
                table: "ptl_WebSiteColumn",
                column: "WebSitePageId");

            migrationBuilder.CreateIndex(
                name: "IX_ptl_WebSiteItem_WebSiteColumnId",
                schema: "cTest",
                table: "ptl_WebSiteItem",
                column: "WebSiteColumnId");

            migrationBuilder.CreateIndex(
                name: "IX_ptl_WebSiteTemplateItem_WebSiteTemplateColumnId",
                schema: "cTest",
                table: "ptl_WebSiteTemplateItem",
                column: "WebSiteTemplateColumnId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ptl_CompanyAccount",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "ptl_CompanyAddress",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "ptl_CompanyAuthentication",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "ptl_CompanyAuthenticationFailedRecord",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "ptl_CompanyContact",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "ptl_CompanyProcessLog",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "ptl_FavoriteInfo",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "ptl_RecommendCustomer",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "ptl_RecommendCustomerLog",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "ptl_RecommendOffering",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "ptl_RecommendOfferingLog",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "ptl_RecommendRequirementLog",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "ptl_RequirementForMaterial",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "ptl_WebSiteInfo",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "ptl_WebSiteItem",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "ptl_WebSiteLink",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "ptl_WebSitePageLog",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "ptl_WebSiteTemplateItem",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "ptl_CompanyInfo",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "ptl_RecommendMaterial",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "ptl_RecommendRequirement",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "ptl_WebSiteColumn",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "ptl_WebSiteTemplateColumn",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "ptl_RecommendCategory",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "ptl_WebSitePage",
                schema: "cTest");
        }
    }
}
