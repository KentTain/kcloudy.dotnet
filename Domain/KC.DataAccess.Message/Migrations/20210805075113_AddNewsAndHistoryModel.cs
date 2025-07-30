using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KC.DataAccess.Message.Migrations
{
    public partial class AddNewsAndHistoryModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "msg_NewsBulletinCategory",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 512, nullable: true),
                    IsShow = table.Column<bool>(nullable: false),
                    TreeCode = table.Column<string>(maxLength: 512, nullable: true),
                    Leaf = table.Column<bool>(nullable: false),
                    Level = table.Column<int>(nullable: false),
                    Index = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
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
                    table.PrimaryKey("PK_msg_NewsBulletinCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_msg_NewsBulletinCategory_msg_NewsBulletinCategory_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "cTest",
                        principalTable: "msg_NewsBulletinCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "msg_NewsBulletinLog",
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
                    NewsBulletinId = table.Column<int>(nullable: false),
                    NewsBulletinType = table.Column<int>(nullable: false),
                    NewsBulletinTitle = table.Column<string>(maxLength: 128, nullable: false),
                    NewsBulletinAuthor = table.Column<string>(maxLength: 128, nullable: true),
                    NewsBulletinStatus = table.Column<int>(nullable: false),
                    NewsBulletinContent = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_msg_NewsBulletinLog", x => x.ProcessLogId);
                });

            migrationBuilder.CreateTable(
                name: "msg_NewsBulletin",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    IsShow = table.Column<bool>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Title = table.Column<string>(maxLength: 128, nullable: false),
                    Author = table.Column<string>(maxLength: 128, nullable: true),
                    AuthorEmail = table.Column<string>(maxLength: 128, nullable: true),
                    Keywords = table.Column<string>(maxLength: 512, nullable: true),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    ImageBlob = table.Column<string>(maxLength: 1000, nullable: true),
                    FileBlob = table.Column<string>(maxLength: 1000, nullable: true),
                    Link = table.Column<string>(maxLength: 512, nullable: true),
                    Content = table.Column<string>(nullable: false),
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
                    table.PrimaryKey("PK_msg_NewsBulletin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_msg_NewsBulletin_msg_NewsBulletinCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "cTest",
                        principalTable: "msg_NewsBulletinCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_msg_NewsBulletin_CategoryId",
                schema: "cTest",
                table: "msg_NewsBulletin",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_msg_NewsBulletinCategory_ParentId",
                schema: "cTest",
                table: "msg_NewsBulletinCategory",
                column: "ParentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "msg_NewsBulletin",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "msg_NewsBulletinLog",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "msg_NewsBulletinCategory",
                schema: "cTest");
        }
    }
}
