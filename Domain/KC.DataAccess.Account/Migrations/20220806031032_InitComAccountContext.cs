using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KC.DataAccess.Account.Migrations
{
    public partial class InitComAccountContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            #region Create Table
            migrationBuilder.EnsureSchema(
                name: "cTest");

            migrationBuilder.CreateTable(
                name: "sys_MenuNode",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    AuthorityId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    TreeCode = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    AreaName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ControllerName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ActionName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Parameters = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    TenantType = table.Column<int>(type: "int", nullable: true),
                    IsExtPage = table.Column<bool>(type: "bit", nullable: false),
                    SmallIcon = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    BigIcon = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Leaf = table.Column<bool>(type: "bit", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
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
                    table.PrimaryKey("PK_sys_MenuNode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sys_MenuNode_sys_MenuNode_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "cTest",
                        principalTable: "sys_MenuNode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sys_Organization",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    OrganizationCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    OrganizationType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    TreeCode = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    BusinessType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Leaf = table.Column<bool>(type: "bit", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false),
                    ReferenceId1 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReferenceId2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReferenceId3 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_sys_Organization", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sys_Organization_sys_Organization_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "cTest",
                        principalTable: "sys_Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sys_Permission",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthorityId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    TreeCode = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    AreaName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ControllerName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ActionName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Parameters = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    ResultType = table.Column<int>(type: "int", nullable: false),
                    Leaf = table.Column<bool>(type: "bit", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
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
                    table.PrimaryKey("PK_sys_Permission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sys_Permission_sys_Permission_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "cTest",
                        principalTable: "sys_Permission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sys_Role",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsSystemRole = table.Column<bool>(type: "bit", nullable: false),
                    BusinessType = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_sys_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_SystemSetting",
                schema: "cTest",
                columns: table => new
                {
                    PropertyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    IsRequire = table.Column<bool>(type: "bit", nullable: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApplicationName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
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
                    table.PrimaryKey("PK_sys_SystemSetting", x => x.PropertyId);
                });

            migrationBuilder.CreateTable(
                name: "sys_User",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PositionLevel = table.Column<int>(type: "int", nullable: false),
                    MemberId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    EmailConfirmedExpired = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmailConfirmedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    IsDefaultMobile = table.Column<bool>(type: "bit", nullable: false),
                    Recommended = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ContactQQ = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Telephone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    OpenId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsModifyPassword = table.Column<bool>(type: "bit", nullable: false),
                    IsClient = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReferenceId1 = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ReferenceId2 = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ReferenceId3 = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_UserLoginLog",
                schema: "cTest",
                columns: table => new
                {
                    ProcessLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IPAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BrowserInfo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    OperatorId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Operator = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OperateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_UserLoginLog", x => x.ProcessLogId);
                });

            migrationBuilder.CreateTable(
                name: "sys_UserTracingLog",
                schema: "cTest",
                columns: table => new
                {
                    ProcessLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    OperatorId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Operator = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OperateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_UserTracingLog", x => x.ProcessLogId);
                });

            migrationBuilder.CreateTable(
                name: "sys_MenuNodesInRoles",
                schema: "cTest",
                columns: table => new
                {
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MenuNodeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_MenuNodesInRoles", x => new { x.RoleId, x.MenuNodeId });
                    table.ForeignKey(
                        name: "FK_sys_MenuNodesInRoles_sys_MenuNode_MenuNodeId",
                        column: x => x.MenuNodeId,
                        principalSchema: "cTest",
                        principalTable: "sys_MenuNode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sys_MenuNodesInRoles_sys_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "cTest",
                        principalTable: "sys_Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sys_PermissionsInRoles",
                schema: "cTest",
                columns: table => new
                {
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_PermissionsInRoles", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_sys_PermissionsInRoles_sys_Permission_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "cTest",
                        principalTable: "sys_Permission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sys_PermissionsInRoles_sys_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "cTest",
                        principalTable: "sys_Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sys_RoleClaim",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_RoleClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sys_RoleClaim_sys_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "cTest",
                        principalTable: "sys_Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sys_SystemSettingProperty",
                schema: "cTest",
                columns: table => new
                {
                    PropertyAttributeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SystemSettingId = table.Column<int>(type: "int", maxLength: 128, nullable: false),
                    SystemSettingCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DataType = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_sys_SystemSettingProperty", x => x.PropertyAttributeId);
                    table.ForeignKey(
                        name: "FK_sys_SystemSettingProperty_sys_SystemSetting_SystemSettingId",
                        column: x => x.SystemSettingId,
                        principalSchema: "cTest",
                        principalTable: "sys_SystemSetting",
                        principalColumn: "PropertyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sys_UserClaim",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_UserClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sys_UserClaim_sys_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "cTest",
                        principalTable: "sys_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sys_UserLogin",
                schema: "cTest",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_UserLogin", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_sys_UserLogin_sys_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "cTest",
                        principalTable: "sys_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sys_UserSetting",
                schema: "cTest",
                columns: table => new
                {
                    PropertyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SystemSettingId = table.Column<int>(type: "int", nullable: true),
                    SystemSettingCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    IsSystemSetting = table.Column<bool>(type: "bit", nullable: false),
                    IsRequire = table.Column<bool>(type: "bit", nullable: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApplicationName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
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
                    table.PrimaryKey("PK_sys_UserSetting", x => x.PropertyId);
                    table.ForeignKey(
                        name: "FK_sys_UserSetting_sys_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "cTest",
                        principalTable: "sys_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sys_UsersInOrganizations",
                schema: "cTest",
                columns: table => new
                {
                    OrganizationId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_UsersInOrganizations", x => new { x.OrganizationId, x.UserId });
                    table.ForeignKey(
                        name: "FK_sys_UsersInOrganizations_sys_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "cTest",
                        principalTable: "sys_Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sys_UsersInOrganizations_sys_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "cTest",
                        principalTable: "sys_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sys_UsersInRoles",
                schema: "cTest",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_UsersInRoles", x => new { x.RoleId, x.UserId });
                    table.ForeignKey(
                        name: "FK_sys_UsersInRoles_sys_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "cTest",
                        principalTable: "sys_Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sys_UsersInRoles_sys_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "cTest",
                        principalTable: "sys_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sys_UserToken",
                schema: "cTest",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_UserToken", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_sys_UserToken_sys_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "cTest",
                        principalTable: "sys_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sys_UserSettingProperty",
                schema: "cTest",
                columns: table => new
                {
                    PropertyAttributeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserSettingId = table.Column<int>(type: "int", nullable: false),
                    UserSettingCode = table.Column<int>(type: "int", maxLength: 20, nullable: false),
                    DataType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRequire = table.Column<bool>(type: "bit", nullable: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    SystemSettingPropertyAttrId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_sys_UserSettingProperty", x => x.PropertyAttributeId);
                    table.ForeignKey(
                        name: "FK_sys_UserSettingProperty_sys_UserSetting_UserSettingId",
                        column: x => x.UserSettingId,
                        principalSchema: "cTest",
                        principalTable: "sys_UserSetting",
                        principalColumn: "PropertyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sys_MenuNode_ParentId",
                schema: "cTest",
                table: "sys_MenuNode",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_sys_MenuNodesInRoles_MenuNodeId",
                schema: "cTest",
                table: "sys_MenuNodesInRoles",
                column: "MenuNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_sys_Organization_ParentId",
                schema: "cTest",
                table: "sys_Organization",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_sys_Permission_ParentId",
                schema: "cTest",
                table: "sys_Permission",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_sys_PermissionsInRoles_PermissionId",
                schema: "cTest",
                table: "sys_PermissionsInRoles",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_sys_RoleClaim_RoleId",
                schema: "cTest",
                table: "sys_RoleClaim",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_sys_SystemSettingProperty_SystemSettingId",
                schema: "cTest",
                table: "sys_SystemSettingProperty",
                column: "SystemSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_sys_UserClaim_UserId",
                schema: "cTest",
                table: "sys_UserClaim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_sys_UserLogin_UserId",
                schema: "cTest",
                table: "sys_UserLogin",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_sys_UserSetting_UserId",
                schema: "cTest",
                table: "sys_UserSetting",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_sys_UserSettingProperty_UserSettingId",
                schema: "cTest",
                table: "sys_UserSettingProperty",
                column: "UserSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_sys_UsersInOrganizations_UserId",
                schema: "cTest",
                table: "sys_UsersInOrganizations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_sys_UsersInRoles_UserId",
                schema: "cTest",
                table: "sys_UsersInRoles",
                column: "UserId");

            #endregion

            #region Init Database Structure with sql: KC.DataAccess.Account

            var sqlBuilder = new System.Text.StringBuilder();
            var sqlStatements = KC.DataAccess.Account.DataInitial.DBSqlInitializer.GetStructureSqlScript();
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

            #region Insert data into Database with sql: KC.DataAccess.Account

            sqlBuilder = new System.Text.StringBuilder();
            sqlStatements = KC.DataAccess.Account.DataInitial.DBSqlInitializer.GetInitialDataSqlScript();
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
                name: "sys_MenuNodesInRoles",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "sys_PermissionsInRoles",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "sys_RoleClaim",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "sys_SystemSettingProperty",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "sys_UserClaim",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "sys_UserLogin",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "sys_UserLoginLog",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "sys_UserSettingProperty",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "sys_UsersInOrganizations",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "sys_UsersInRoles",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "sys_UserToken",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "sys_UserTracingLog",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "sys_MenuNode",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "sys_Permission",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "sys_SystemSetting",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "sys_UserSetting",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "sys_Organization",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "sys_Role",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "sys_User",
                schema: "cTest");
        }
    }
}
