using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KC.DataAccess.Contract.Migrations
{
    public partial class InitComContractContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cTest");

            migrationBuilder.CreateTable(
                name: "econ_ContractGroup",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BlobId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    ContractNo = table.Column<string>(nullable: true),
                    ContractTitle = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    ContractContent = table.Column<string>(nullable: true),
                    ContractFootnote = table.Column<string>(nullable: true),
                    Statu = table.Column<int>(nullable: false),
                    GroupId = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    PId = table.Column<Guid>(nullable: true),
                    Break = table.Column<string>(nullable: true),
                    BreakStart = table.Column<bool>(nullable: false),
                    AllBreak = table.Column<string>(nullable: true),
                    AllBreakStart = table.Column<bool>(nullable: false),
                    SyncStatus = table.Column<int>(nullable: false),
                    WorkFlowStatus = table.Column<int>(nullable: false),
                    ReferenceId = table.Column<string>(nullable: true),
                    RelationData = table.Column<string>(nullable: true),
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
                    table.PrimaryKey("PK_econ_ContractGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_econ_ContractGroup_econ_ContractGroup_PId",
                        column: x => x.PId,
                        principalSchema: "cTest",
                        principalTable: "econ_ContractGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "econ_ContractTemplate",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    TransactionTypeName = table.Column<string>(nullable: true),
                    ContractValue = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
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
                    table.PrimaryKey("PK_econ_ContractTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "econ_ElectronicPerson",
                schema: "cTest",
                columns: table => new
                {
                    UserId = table.Column<string>(maxLength: 128, nullable: false),
                    UserName = table.Column<string>(maxLength: 56, nullable: true),
                    IsSync = table.Column<bool>(nullable: false),
                    Email = table.Column<string>(maxLength: 56, nullable: true),
                    Mobile = table.Column<string>(maxLength: 11, nullable: true),
                    IdentityNumber = table.Column<string>(maxLength: 18, nullable: true),
                    AccountId = table.Column<string>(maxLength: 128, nullable: true),
                    SealId = table.Column<string>(maxLength: 256, nullable: true),
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
                    table.PrimaryKey("PK_econ_ElectronicPerson", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "econ_ContractGroupOperationLog",
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
                    ContractGroupId = table.Column<Guid>(nullable: false),
                    ContractGroupProgress = table.Column<int>(nullable: false),
                    NotContractGroupUsers = table.Column<string>(nullable: true),
                    ToPlatFormContractGroup = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_econ_ContractGroupOperationLog", x => x.ProcessLogId);
                    table.ForeignKey(
                        name: "FK_econ_ContractGroupOperationLog_econ_ContractGroup_ContractGroupId",
                        column: x => x.ContractGroupId,
                        principalSchema: "cTest",
                        principalTable: "econ_ContractGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "econ_UserContract",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    StaffId = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    Key = table.Column<string>(nullable: true),
                    Statu = table.Column<int>(nullable: false),
                    BreakRemark = table.Column<string>(nullable: true),
                    CustomerType = table.Column<int>(nullable: false),
                    BlobId = table.Column<Guid>(nullable: false),
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
                    table.PrimaryKey("PK_econ_UserContract", x => x.Id);
                    table.ForeignKey(
                        name: "FK_econ_UserContract_econ_ContractGroup_BlobId",
                        column: x => x.BlobId,
                        principalSchema: "cTest",
                        principalTable: "econ_ContractGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "econ_ElectronicOrganization",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantName = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 512, nullable: true),
                    IsSync = table.Column<bool>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    blobId = table.Column<string>(nullable: true),
                    RegType = table.Column<int>(nullable: false),
                    OrgNumber = table.Column<string>(maxLength: 128, nullable: true),
                    Mobile = table.Column<string>(maxLength: 13, nullable: true),
                    QText = table.Column<string>(maxLength: 128, nullable: true),
                    Data = table.Column<string>(nullable: true),
                    OrgId = table.Column<string>(maxLength: 128, nullable: true),
                    SealId = table.Column<string>(maxLength: 256, nullable: true),
                    OrgLegalName = table.Column<string>(maxLength: 128, nullable: true),
                    OrgLegalIdNumber = table.Column<string>(maxLength: 56, nullable: true),
                    Remark = table.Column<string>(maxLength: 512, nullable: true),
                    UserId = table.Column<string>(nullable: true),
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
                    table.PrimaryKey("PK_econ_ElectronicOrganization", x => x.Id);
                    table.ForeignKey(
                        name: "FK_econ_ElectronicOrganization_econ_ElectronicPerson_UserId",
                        column: x => x.UserId,
                        principalSchema: "cTest",
                        principalTable: "econ_ElectronicPerson",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_econ_ContractGroup_PId",
                schema: "cTest",
                table: "econ_ContractGroup",
                column: "PId");

            migrationBuilder.CreateIndex(
                name: "IX_econ_ContractGroupOperationLog_ContractGroupId",
                schema: "cTest",
                table: "econ_ContractGroupOperationLog",
                column: "ContractGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_econ_ElectronicOrganization_UserId",
                schema: "cTest",
                table: "econ_ElectronicOrganization",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_econ_UserContract_BlobId",
                schema: "cTest",
                table: "econ_UserContract",
                column: "BlobId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "econ_ContractGroupOperationLog",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "econ_ContractTemplate",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "econ_ElectronicOrganization",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "econ_UserContract",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "econ_ElectronicPerson",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "econ_ContractGroup",
                schema: "cTest");
        }
    }
}
