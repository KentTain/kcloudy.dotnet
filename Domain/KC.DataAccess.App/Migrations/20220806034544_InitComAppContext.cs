using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KC.DataAccess.App.Migrations
{
    public partial class InitComAppContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            #region Create Table
            migrationBuilder.EnsureSchema(
                name: "cTest");

            migrationBuilder.CreateTable(
                name: "app_ApplicationLog",
                schema: "cTest",
                columns: table => new
                {
                    ProcessLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    appLogType = table.Column<int>(type: "int", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    OperatorId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Operator = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OperateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_ApplicationLog", x => x.ProcessLogId);
                });

            migrationBuilder.CreateTable(
                name: "app_DevTemplate",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Version = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    GitAddress = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    IsUseToken = table.Column<bool>(type: "bit", nullable: false),
                    GitAccount = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    GitPassword = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    GitToken = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    GitTagId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    GitShaId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsPlatform = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_app_DevTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "app_Application",
                schema: "cTest",
                columns: table => new
                {
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    ApplicationCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ApplicationName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DomainName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    WebSiteName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    AssemblyName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    SmallIcon = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    BigIcon = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsEnabledWorkFlow = table.Column<bool>(type: "bit", nullable: false),
                    AppTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
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
                    table.PrimaryKey("PK_app_Application", x => x.ApplicationId);
                    table.ForeignKey(
                        name: "FK_app_Application_app_DevTemplate_AppTemplateId",
                        column: x => x.AppTemplateId,
                        principalSchema: "cTest",
                        principalTable: "app_DevTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "app_AppGit",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GitAddress = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    GitMainBranch = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    GitToken = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    IsActived = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_app_AppGit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_app_AppGit_app_Application_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "cTest",
                        principalTable: "app_Application",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "app_AppSetting",
                schema: "cTest",
                columns: table => new
                {
                    PropertyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    IsRequire = table.Column<bool>(type: "bit", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_app_AppSetting", x => x.PropertyId);
                    table.ForeignKey(
                        name: "FK_app_AppSetting_app_Application_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "cTest",
                        principalTable: "app_Application",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "app_AppGitBranch",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    AppGitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_app_AppGitBranch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_app_AppGitBranch_app_AppGit_AppGitId",
                        column: x => x.AppGitId,
                        principalSchema: "cTest",
                        principalTable: "app_AppGit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "app_AppGitUser",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    IsUseToken = table.Column<bool>(type: "bit", nullable: false),
                    UserAccount = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UserPassword = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UserToken = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false),
                    AppGitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_app_AppGitUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_app_AppGitUser_app_AppGit_AppGitId",
                        column: x => x.AppGitId,
                        principalSchema: "cTest",
                        principalTable: "app_AppGit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "app_AppSettingProperty",
                schema: "cTest",
                columns: table => new
                {
                    PropertyAttributeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppSettingId = table.Column<int>(type: "int", nullable: false),
                    AppSettingCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DataType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRequire = table.Column<bool>(type: "bit", nullable: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
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
                    table.PrimaryKey("PK_app_AppSettingProperty", x => x.PropertyAttributeId);
                    table.ForeignKey(
                        name: "FK_app_AppSettingProperty_app_AppSetting_AppSettingId",
                        column: x => x.AppSettingId,
                        principalSchema: "cTest",
                        principalTable: "app_AppSetting",
                        principalColumn: "PropertyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_app_AppGit_ApplicationId",
                schema: "cTest",
                table: "app_AppGit",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_app_AppGitBranch_AppGitId",
                schema: "cTest",
                table: "app_AppGitBranch",
                column: "AppGitId");

            migrationBuilder.CreateIndex(
                name: "IX_app_AppGitUser_AppGitId",
                schema: "cTest",
                table: "app_AppGitUser",
                column: "AppGitId");

            migrationBuilder.CreateIndex(
                name: "IX_app_Application_AppTemplateId",
                schema: "cTest",
                table: "app_Application",
                column: "AppTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_app_AppSetting_ApplicationId",
                schema: "cTest",
                table: "app_AppSetting",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_app_AppSettingProperty_AppSettingId",
                schema: "cTest",
                table: "app_AppSettingProperty",
                column: "AppSettingId");
            #endregion

            #region Init Database Structure with sql: KC.DataAccess.App

            var sqlBuilder = new System.Text.StringBuilder();
            var sqlStatements = KC.DataAccess.App.DataInitial.DBSqlInitializer.GetStructureSqlScript();
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

            #region Insert data into Database with sql: KC.DataAccess.App

            sqlBuilder = new System.Text.StringBuilder();
            sqlStatements = KC.DataAccess.App.DataInitial.DBSqlInitializer.GetInitialDataSqlScript();
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
                name: "app_AppGitBranch",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "app_AppGitUser",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "app_ApplicationLog",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "app_AppSettingProperty",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "app_AppGit",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "app_AppSetting",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "app_Application",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "app_DevTemplate",
                schema: "cTest");
        }
    }
}
