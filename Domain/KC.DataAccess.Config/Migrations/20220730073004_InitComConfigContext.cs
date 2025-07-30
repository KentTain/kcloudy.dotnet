using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KC.DataAccess.Config.Migrations
{
    public partial class InitComConfigContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            #region Create Table
            migrationBuilder.EnsureSchema(
                name: "cTest");

            migrationBuilder.CreateTable(
                name: "cfg_ConfigEntity",
                schema: "cTest",
                columns: table => new
                {
                    ConfigId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConfigType = table.Column<int>(type: "int", nullable: false),
                    ConfigSign = table.Column<int>(type: "int", nullable: false),
                    ConfigName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ConfigDescription = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ConfigXml = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfigImgUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    ConfigCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
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
                    table.PrimaryKey("PK_cfg_ConfigEntity", x => x.ConfigId);
                });

            migrationBuilder.CreateTable(
                name: "cfg_ConfigLog",
                schema: "cTest",
                columns: table => new
                {
                    ProcessLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConfigId = table.Column<int>(type: "int", nullable: false),
                    ConfigName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    OperatorId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Operator = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OperateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cfg_ConfigLog", x => x.ProcessLogId);
                });

            migrationBuilder.CreateTable(
                name: "cfg_SeedEntity",
                schema: "cTest",
                columns: table => new
                {
                    SeedType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SeedValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeedMin = table.Column<int>(type: "int", nullable: false),
                    SeedMax = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cfg_SeedEntity", x => x.SeedType);
                });

            migrationBuilder.CreateTable(
                name: "cfg_SysSequence",
                schema: "cTest",
                columns: table => new
                {
                    SequenceName = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    CurrentValue = table.Column<int>(type: "int", nullable: false),
                    InitValue = table.Column<int>(type: "int", nullable: false),
                    MaxValue = table.Column<int>(type: "int", nullable: false),
                    StepValue = table.Column<int>(type: "int", nullable: false),
                    PreFixString = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    PostFixString = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CurrDate = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cfg_SysSequence", x => x.SequenceName);
                });

            migrationBuilder.CreateTable(
                name: "cfg_ConfigAttribute",
                schema: "cTest",
                columns: table => new
                {
                    PropertyAttributeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConfigId = table.Column<int>(type: "int", nullable: false),
                    DataType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Ext1 = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Ext2 = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Ext3 = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    IsRequire = table.Column<bool>(type: "bit", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false),
                    IsFileAttr = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_cfg_ConfigAttribute", x => x.PropertyAttributeId);
                    table.ForeignKey(
                        name: "FK_cfg_ConfigAttribute_cfg_ConfigEntity_ConfigId",
                        column: x => x.ConfigId,
                        principalSchema: "cTest",
                        principalTable: "cfg_ConfigEntity",
                        principalColumn: "ConfigId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cfg_ConfigAttribute_ConfigId",
                schema: "cTest",
                table: "cfg_ConfigAttribute",
                column: "ConfigId");
            #endregion

            #region Init Database Structure with sql: KC.DataAccess.Config

            var sqlBuilder = new System.Text.StringBuilder();
            var sqlStatements = KC.DataAccess.Config.DataInitial.DBSqlInitializer.GetStructureSqlScript();
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

            #region Insert data into Database with sql: KC.DataAccess.Config

            sqlBuilder = new System.Text.StringBuilder();
            sqlStatements = KC.DataAccess.Config.DataInitial.DBSqlInitializer.GetInitialDataSqlScript();
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
                name: "cfg_ConfigAttribute",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "cfg_ConfigLog",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "cfg_SeedEntity",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "cfg_SysSequence",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "cfg_ConfigEntity",
                schema: "cTest");
        }
    }
}
