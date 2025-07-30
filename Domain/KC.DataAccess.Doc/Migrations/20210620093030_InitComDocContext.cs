using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KC.DataAccess.Doc.Migrations
{
    public partial class InitComDocContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cTest");

            migrationBuilder.CreateTable(
                name: "doc_DocCategory",
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
                    Type = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(maxLength: 256, nullable: true),
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
                    table.PrimaryKey("PK_doc_DocCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_doc_DocCategory_doc_DocCategory_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "cTest",
                        principalTable: "doc_DocCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "doc_DocTemplate",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 1000, nullable: true),
                    Level = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    AttachmentBlob = table.Column<string>(maxLength: 1000, nullable: true),
                    OrgCodes = table.Column<string>(nullable: true),
                    OrgNames = table.Column<string>(nullable: true),
                    RoleIds = table.Column<string>(maxLength: 4000, nullable: true),
                    RoleNames = table.Column<string>(nullable: true),
                    UserIds = table.Column<string>(maxLength: 4000, nullable: true),
                    UserNames = table.Column<string>(nullable: true),
                    ExceptUserIds = table.Column<string>(nullable: true),
                    ExceptUserNames = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(maxLength: 1000, nullable: true),
                    UploadedTime = table.Column<DateTime>(nullable: true),
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
                    table.PrimaryKey("PK_doc_DocTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "doc_DocumentInfo",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocCategoryId = table.Column<int>(nullable: true),
                    DocCode = table.Column<string>(maxLength: 20, nullable: true),
                    Name = table.Column<string>(maxLength: 1000, nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Level = table.Column<int>(nullable: false),
                    AttachmentBlob = table.Column<string>(maxLength: 1000, nullable: true),
                    Ext = table.Column<string>(maxLength: 50, nullable: true),
                    FileType = table.Column<string>(nullable: true),
                    FileFormat = table.Column<string>(nullable: true),
                    Size = table.Column<long>(nullable: false),
                    HasTemplates = table.Column<bool>(nullable: false),
                    TemplateBlob = table.Column<string>(maxLength: 1000, nullable: true),
                    OrgCodes = table.Column<string>(nullable: true),
                    OrgNames = table.Column<string>(nullable: true),
                    RoleIds = table.Column<string>(maxLength: 4000, nullable: true),
                    RoleNames = table.Column<string>(nullable: true),
                    UserIds = table.Column<string>(maxLength: 4000, nullable: true),
                    UserNames = table.Column<string>(nullable: true),
                    ExceptUserIds = table.Column<string>(nullable: true),
                    ExceptUserNames = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(maxLength: 1000, nullable: true),
                    UploadedTime = table.Column<DateTime>(nullable: true),
                    IsValid = table.Column<bool>(nullable: false),
                    IsPublic = table.Column<bool>(nullable: false),
                    CanEdit = table.Column<bool>(nullable: false),
                    CanSend = table.Column<bool>(nullable: false),
                    TenantName = table.Column<string>(nullable: true),
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
                    table.PrimaryKey("PK_doc_DocumentInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_doc_DocumentInfo_doc_DocCategory_DocCategoryId",
                        column: x => x.DocCategoryId,
                        principalSchema: "cTest",
                        principalTable: "doc_DocCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "doc_DocTemplateLog",
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
                    DocOperType = table.Column<int>(nullable: false),
                    DocTemplateId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doc_DocTemplateLog", x => x.ProcessLogId);
                    table.ForeignKey(
                        name: "FK_doc_DocTemplateLog_doc_DocTemplate_DocTemplateId",
                        column: x => x.DocTemplateId,
                        principalSchema: "cTest",
                        principalTable: "doc_DocTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "doc_DocBackup",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 1000, nullable: true),
                    Level = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(maxLength: 1000, nullable: true),
                    CreatedDateTime = table.Column<DateTime>(nullable: true),
                    ExpiredDateTime = table.Column<DateTime>(nullable: true),
                    AttachmentBlob = table.Column<string>(maxLength: 1000, nullable: true),
                    DocumentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doc_DocBackup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_doc_DocBackup_doc_DocumentInfo_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "cTest",
                        principalTable: "doc_DocumentInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "doc_DocumentLog",
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
                    DocOperType = table.Column<int>(nullable: false),
                    DocumentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doc_DocumentLog", x => x.ProcessLogId);
                    table.ForeignKey(
                        name: "FK_doc_DocumentLog_doc_DocumentInfo_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "cTest",
                        principalTable: "doc_DocumentInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_doc_DocBackup_DocumentId",
                schema: "cTest",
                table: "doc_DocBackup",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_doc_DocCategory_ParentId",
                schema: "cTest",
                table: "doc_DocCategory",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_doc_DocTemplateLog_DocTemplateId",
                schema: "cTest",
                table: "doc_DocTemplateLog",
                column: "DocTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_doc_DocumentInfo_DocCategoryId",
                schema: "cTest",
                table: "doc_DocumentInfo",
                column: "DocCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_doc_DocumentLog_DocumentId",
                schema: "cTest",
                table: "doc_DocumentLog",
                column: "DocumentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "doc_DocBackup",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "doc_DocTemplateLog",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "doc_DocumentLog",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "doc_DocTemplate",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "doc_DocumentInfo",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "doc_DocCategory",
                schema: "cTest");
        }
    }
}
