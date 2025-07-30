using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KC.DataAccess.Message.Migrations
{
    public partial class InitComMessageContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            #region Create Table
            migrationBuilder.EnsureSchema(
                name: "cTest");

            migrationBuilder.CreateTable(
                name: "msg_MemberRemindMessage",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageTitle = table.Column<string>(maxLength: 200, nullable: true),
                    MessageContent = table.Column<string>(nullable: true),
                    TypeId = table.Column<int>(nullable: false),
                    TypeName = table.Column<string>(maxLength: 50, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(maxLength: 128, nullable: true),
                    UserName = table.Column<string>(maxLength: 50, nullable: true),
                    ReadDate = table.Column<DateTime>(nullable: true),
                    ApplicationId = table.Column<Guid>(nullable: false),
                    ApplicationName = table.Column<string>(nullable: true),
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
                    table.PrimaryKey("PK_msg_MemberRemindMessage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "msg_MessageCategory",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 512, nullable: true),
                    TreeCode = table.Column<string>(maxLength: 512, nullable: true),
                    Leaf = table.Column<bool>(nullable: false),
                    Level = table.Column<int>(nullable: false),
                    Index = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 4000, nullable: true),
                    ReferenceId = table.Column<string>(maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_msg_MessageCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_msg_MessageCategory_msg_MessageCategory_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "cTest",
                        principalTable: "msg_MessageCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "msg_MessageTemplateLog",
                schema: "cTest",
                columns: table => new
                {
                    ProcessLogId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(nullable: false),
                    OperatorId = table.Column<string>(maxLength: 128, nullable: true),
                    Operator = table.Column<string>(maxLength: 50, nullable: true),
                    OperateDate = table.Column<DateTime>(nullable: false),
                    Remark = table.Column<string>(maxLength: 4000, nullable: true),
                    TemplateId = table.Column<int>(nullable: false),
                    TemplateType = table.Column<int>(nullable: false),
                    TemplateName = table.Column<string>(maxLength: 128, nullable: true),
                    TemplateSubject = table.Column<string>(maxLength: 256, nullable: true),
                    TemplateContent = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_msg_MessageTemplateLog", x => x.ProcessLogId);
                });

            migrationBuilder.CreateTable(
                name: "msg_MessageClass",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: true),
                    Code = table.Column<string>(maxLength: 20, nullable: true),
                    ReplaceParametersString = table.Column<string>(maxLength: 1000, nullable: true),
                    Index = table.Column<int>(nullable: false),
                    Desc = table.Column<string>(maxLength: 4000, nullable: true),
                    ApplicationId = table.Column<Guid>(nullable: false),
                    ApplicationName = table.Column<string>(nullable: true),
                    MessageCategoryId = table.Column<int>(nullable: true),
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
                    table.PrimaryKey("PK_msg_MessageClass", x => x.Id);
                    table.ForeignKey(
                        name: "FK_msg_MessageClass_msg_MessageCategory_MessageCategoryId",
                        column: x => x.MessageCategoryId,
                        principalSchema: "cTest",
                        principalTable: "msg_MessageCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "msg_MessageTemplate",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateType = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: true),
                    Subject = table.Column<string>(maxLength: 256, nullable: true),
                    Content = table.Column<string>(nullable: true),
                    MessageClassId = table.Column<int>(nullable: false),
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
                    table.PrimaryKey("PK_msg_MessageTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_msg_MessageTemplate_msg_MessageClass_MessageClassId",
                        column: x => x.MessageClassId,
                        principalSchema: "cTest",
                        principalTable: "msg_MessageClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_msg_MessageCategory_ParentId",
                schema: "cTest",
                table: "msg_MessageCategory",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_msg_MessageClass_MessageCategoryId",
                schema: "cTest",
                table: "msg_MessageClass",
                column: "MessageCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_msg_MessageTemplate_MessageClassId",
                schema: "cTest",
                table: "msg_MessageTemplate",
                column: "MessageClassId");
            #endregion

            #region Init Database Structure with sql: KC.DataAccess.Message

            var sqlBuilder = new System.Text.StringBuilder();
            var sqlStatements = KC.DataAccess.Message.DataInitial.DBSqlInitializer.GetStructureSqlScript();
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

            #region Insert data into Database with sql: KC.DataAccess.Message

            sqlBuilder = new System.Text.StringBuilder();
            sqlStatements = KC.DataAccess.Message.DataInitial.DBSqlInitializer.GetInitialDataSqlScript();
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
                name: "msg_MemberRemindMessage",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "msg_MessageTemplate",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "msg_MessageTemplateLog",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "msg_MessageClass",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "msg_MessageCategory",
                schema: "cTest");
        }
    }
}
