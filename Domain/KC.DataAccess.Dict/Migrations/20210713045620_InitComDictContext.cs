using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KC.DataAccess.Dict.Migrations
{
    public partial class InitComDictContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            #region Create Table
            migrationBuilder.EnsureSchema(
                name: "cTest");

            migrationBuilder.CreateTable(
                name: "dic_DictType",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 128, nullable: true),
                    Name = table.Column<string>(maxLength: 512, nullable: true),
                    IsSys = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedName = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(maxLength: 128, nullable: true),
                    ModifiedName = table.Column<string>(maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dic_DictType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "dic_IndustryClassfication",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(nullable: true),
                    TreeCode = table.Column<string>(maxLength: 512, nullable: true),
                    Name = table.Column<string>(maxLength: 512, nullable: true),
                    Leaf = table.Column<bool>(nullable: false),
                    Level = table.Column<int>(nullable: false),
                    Index = table.Column<int>(nullable: false),
                    IsValid = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedName = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(maxLength: 128, nullable: true),
                    ModifiedName = table.Column<string>(maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dic_IndustryClassfication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dic_IndustryClassfication_dic_IndustryClassfication_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "cTest",
                        principalTable: "dic_IndustryClassfication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "dic_MobileLocation",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Mobile = table.Column<string>(nullable: true),
                    Province = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Corp = table.Column<string>(nullable: true),
                    AreaCode = table.Column<string>(nullable: true),
                    PostCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dic_MobileLocation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "dic_Province",
                schema: "cTest",
                columns: table => new
                {
                    ProvinceId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 512, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedName = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(maxLength: 128, nullable: true),
                    ModifiedName = table.Column<string>(maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dic_Province", x => x.ProvinceId);
                });

            migrationBuilder.CreateTable(
                name: "dic_DictValue",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 128, nullable: true),
                    Name = table.Column<string>(maxLength: 512, nullable: true),
                    Description = table.Column<string>(maxLength: 4000, nullable: true),
                    DictTypeId = table.Column<int>(nullable: false),
                    DictTypeCode = table.Column<string>(maxLength: 128, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedName = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(maxLength: 128, nullable: true),
                    ModifiedName = table.Column<string>(maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dic_DictValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dic_DictValue_dic_DictType_DictTypeId",
                        column: x => x.DictTypeId,
                        principalSchema: "cTest",
                        principalTable: "dic_DictType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dic_City",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 512, nullable: true),
                    ProvinceId = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedName = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(maxLength: 128, nullable: true),
                    ModifiedName = table.Column<string>(maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dic_City", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dic_City_dic_Province_ProvinceId",
                        column: x => x.ProvinceId,
                        principalSchema: "cTest",
                        principalTable: "dic_Province",
                        principalColumn: "ProvinceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_dic_City_ProvinceId",
                schema: "cTest",
                table: "dic_City",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_dic_DictValue_DictTypeId",
                schema: "cTest",
                table: "dic_DictValue",
                column: "DictTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_dic_IndustryClassfication_ParentId",
                schema: "cTest",
                table: "dic_IndustryClassfication",
                column: "ParentId");

            #endregion


            #region Init Database Structure with sql: KC.DataAccess.Dict

            var sqlBuilder = new System.Text.StringBuilder();
            var sqlStatements = KC.DataAccess.Dict.DataInitial.DBSqlInitializer.GetStructureSqlScript();
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

            #region Insert data into Database with sql: KC.DataAccess.Dict

            sqlBuilder = new System.Text.StringBuilder();
            sqlStatements = KC.DataAccess.Dict.DataInitial.DBSqlInitializer.GetInitialDataSqlScript();
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
                name: "dic_City",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "dic_DictValue",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "dic_IndustryClassfication",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "dic_MobileLocation",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "dic_Province",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "dic_DictType",
                schema: "cTest");
        }
    }
}
