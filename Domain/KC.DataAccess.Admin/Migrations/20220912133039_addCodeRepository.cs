using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KC.DataAccess.Admin.Migrations
{
    public partial class addCodeRepository : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodeAccessKeyPasswordHash",
                schema: "cDba",
                table: "tenant_TenantUser",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodeAccessName",
                schema: "cDba",
                table: "tenant_TenantUser",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodeEndpoint",
                schema: "cDba",
                table: "tenant_TenantUser",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CodePoolId",
                schema: "cDba",
                table: "tenant_TenantUser",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CodeType",
                schema: "cDba",
                table: "tenant_TenantUser",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "tenant_CodeRepositoryPool",
                schema: "cDba",
                columns: table => new
                {
                    CodePoolId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Endpoint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccessName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccessKeyPasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordExpiredTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantCount = table.Column<int>(type: "int", nullable: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    CloudType = table.Column<int>(type: "int", nullable: false),
                    CodeType = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_tenant_CodeRepositoryPool", x => x.CodePoolId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tenant_TenantUser_CodePoolId",
                schema: "cDba",
                table: "tenant_TenantUser",
                column: "CodePoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_tenant_TenantUser_tenant_CodeRepositoryPool_CodePoolId",
                schema: "cDba",
                table: "tenant_TenantUser",
                column: "CodePoolId",
                principalSchema: "cDba",
                principalTable: "tenant_CodeRepositoryPool",
                principalColumn: "CodePoolId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tenant_TenantUser_tenant_CodeRepositoryPool_CodePoolId",
                schema: "cDba",
                table: "tenant_TenantUser");

            migrationBuilder.DropTable(
                name: "tenant_CodeRepositoryPool",
                schema: "cDba");

            migrationBuilder.DropIndex(
                name: "IX_tenant_TenantUser_CodePoolId",
                schema: "cDba",
                table: "tenant_TenantUser");

            migrationBuilder.DropColumn(
                name: "CodeAccessKeyPasswordHash",
                schema: "cDba",
                table: "tenant_TenantUser");

            migrationBuilder.DropColumn(
                name: "CodeAccessName",
                schema: "cDba",
                table: "tenant_TenantUser");

            migrationBuilder.DropColumn(
                name: "CodeEndpoint",
                schema: "cDba",
                table: "tenant_TenantUser");

            migrationBuilder.DropColumn(
                name: "CodePoolId",
                schema: "cDba",
                table: "tenant_TenantUser");

            migrationBuilder.DropColumn(
                name: "CodeType",
                schema: "cDba",
                table: "tenant_TenantUser");
        }
    }
}
