using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KC.DataAccess.Offering.Migrations
{
    public partial class initComOfferingContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cTest");

            migrationBuilder.CreateTable(
                name: "prd_Category",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryImage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CategoryFile = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OfferingPropertyType = table.Column<int>(type: "int", nullable: false),
                    OfferingPriceType = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_prd_Category", x => x.Id);
                    table.ForeignKey(
                        name: "FK_prd_Category_prd_Category_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "cTest",
                        principalTable: "prd_Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "prd_CategoryManager",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    MemberId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactQQ = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Telephone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    OpenId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsValid = table.Column<bool>(type: "bit", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_prd_CategoryManager", x => x.Id);
                    table.ForeignKey(
                        name: "FK_prd_CategoryManager_prd_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "cTest",
                        principalTable: "prd_Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "prd_CategoryOperationLog",
                schema: "cTest",
                columns: table => new
                {
                    ProcessLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    OperatorId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Operator = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OperateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prd_CategoryOperationLog", x => x.ProcessLogId);
                    table.ForeignKey(
                        name: "FK_prd_CategoryOperationLog_prd_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "cTest",
                        principalTable: "prd_Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "prd_Offering",
                schema: "cTest",
                columns: table => new
                {
                    OfferingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfferingCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    OfferingName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    OfferingTypeCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    OfferingTypeName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    OfferingUnit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    OfferingImage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OfferingFile = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OfferingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OfferingDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OfferingRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OfferingAddress = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Barcode = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_prd_Offering", x => x.OfferingId);
                    table.ForeignKey(
                        name: "FK_prd_Offering_prd_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "cTest",
                        principalTable: "prd_Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "prd_PropertyProvider",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceDataType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    IsRequire = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prd_PropertyProvider", x => x.Id);
                    table.ForeignKey(
                        name: "FK_prd_PropertyProvider_prd_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "cTest",
                        principalTable: "prd_Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "prd_OfferingOperationLog",
                schema: "cTest",
                columns: table => new
                {
                    ProcessLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfferingCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    OfferingName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    OfferingId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    OperatorId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Operator = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OperateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prd_OfferingOperationLog", x => x.ProcessLogId);
                    table.ForeignKey(
                        name: "FK_prd_OfferingOperationLog_prd_Offering_OfferingId",
                        column: x => x.OfferingId,
                        principalSchema: "cTest",
                        principalTable: "prd_Offering",
                        principalColumn: "OfferingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "prd_OfferingProperty",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfferingPropertyType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    IsRequire = table.Column<bool>(type: "bit", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false),
                    OfferingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prd_OfferingProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_prd_OfferingProperty_prd_Offering_OfferingId",
                        column: x => x.OfferingId,
                        principalSchema: "cTest",
                        principalTable: "prd_Offering",
                        principalColumn: "OfferingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "prd_Product",
                schema: "cTest",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    ProductUnit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ProductImage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ProductFile = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ProductPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Barcode = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    OfferingId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_prd_Product", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_prd_Product_prd_Offering_OfferingId",
                        column: x => x.OfferingId,
                        principalSchema: "cTest",
                        principalTable: "prd_Offering",
                        principalColumn: "OfferingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "prd_PropertyProviderAttr",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceAttrDataType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Value1 = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Value2 = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    ServiceProviderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prd_PropertyProviderAttr", x => x.Id);
                    table.ForeignKey(
                        name: "FK_prd_PropertyProviderAttr_prd_PropertyProvider_ServiceProviderId",
                        column: x => x.ServiceProviderId,
                        principalSchema: "cTest",
                        principalTable: "prd_PropertyProvider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "prd_ProductProperty",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductPropertyType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Value1 = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Value2 = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RefProviderId = table.Column<int>(type: "int", nullable: true),
                    RefProviderAttrId = table.Column<int>(type: "int", nullable: true),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    IsRequire = table.Column<bool>(type: "bit", nullable: false),
                    IsProvider = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prd_ProductProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_prd_ProductProperty_prd_Product_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "cTest",
                        principalTable: "prd_Product",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_prd_Category_ParentId",
                schema: "cTest",
                table: "prd_Category",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_prd_CategoryManager_CategoryId",
                schema: "cTest",
                table: "prd_CategoryManager",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_prd_CategoryOperationLog_CategoryId",
                schema: "cTest",
                table: "prd_CategoryOperationLog",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_prd_Offering_CategoryId",
                schema: "cTest",
                table: "prd_Offering",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_prd_OfferingOperationLog_OfferingId",
                schema: "cTest",
                table: "prd_OfferingOperationLog",
                column: "OfferingId");

            migrationBuilder.CreateIndex(
                name: "IX_prd_OfferingProperty_OfferingId",
                schema: "cTest",
                table: "prd_OfferingProperty",
                column: "OfferingId");

            migrationBuilder.CreateIndex(
                name: "IX_prd_Product_OfferingId",
                schema: "cTest",
                table: "prd_Product",
                column: "OfferingId");

            migrationBuilder.CreateIndex(
                name: "IX_prd_ProductProperty_ProductId",
                schema: "cTest",
                table: "prd_ProductProperty",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_prd_PropertyProvider_CategoryId",
                schema: "cTest",
                table: "prd_PropertyProvider",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_prd_PropertyProviderAttr_ServiceProviderId",
                schema: "cTest",
                table: "prd_PropertyProviderAttr",
                column: "ServiceProviderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "prd_CategoryManager",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "prd_CategoryOperationLog",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "prd_OfferingOperationLog",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "prd_OfferingProperty",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "prd_ProductProperty",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "prd_PropertyProviderAttr",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "prd_Product",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "prd_PropertyProvider",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "prd_Offering",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "prd_Category",
                schema: "cTest");
        }
    }
}
