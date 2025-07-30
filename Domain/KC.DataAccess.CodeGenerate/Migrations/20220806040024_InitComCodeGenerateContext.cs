using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KC.DataAccess.CodeGenerate.Migrations
{
    public partial class InitComCodeGenerateContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cTest");

            migrationBuilder.CreateTable(
                name: "code_ModelCategory",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    ModelType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    TreeCode = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Leaf = table.Column<bool>(type: "bit", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false),
                    ApplicationId = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_code_ModelCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_code_ModelCategory_code_ModelCategory_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "cTest",
                        principalTable: "code_ModelCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "code_ModelChangeLog",
                schema: "cTest",
                columns: table => new
                {
                    ProcessLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModelType = table.Column<int>(type: "int", nullable: false),
                    ReferenceId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ReferenceName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    RefObjectJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    OperatorId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Operator = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OperateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_code_ModelChangeLog", x => x.ProcessLogId);
                });

            migrationBuilder.CreateTable(
                name: "code_ApiDefinition",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    ApiStatus = table.Column<int>(type: "int", nullable: false),
                    ApiMethodType = table.Column<int>(type: "int", nullable: false),
                    ApiHttpType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Url = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    ApplicationId = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_code_ApiDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_code_ApiDefinition_code_ModelCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "cTest",
                        principalTable: "code_ModelCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "code_ModelDefinition",
                schema: "cTest",
                columns: table => new
                {
                    PropertyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TableName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ModelBaseType = table.Column<int>(type: "int", nullable: false),
                    IsUseLog = table.Column<bool>(type: "bit", nullable: false),
                    IsRequire = table.Column<bool>(type: "bit", nullable: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    ApplicationId = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_code_ModelDefinition", x => x.PropertyId);
                    table.ForeignKey(
                        name: "FK_code_ModelDefinition_code_ModelCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "cTest",
                        principalTable: "code_ModelCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "code_RelationDefinition",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MainModelDefId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApplicationId = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_code_RelationDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_code_RelationDefinition_code_ModelCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "cTest",
                        principalTable: "code_ModelCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "code_ApiInputParam",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApiDefId = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    TreeCode = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    DataType = table.Column<int>(type: "int", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsArray = table.Column<bool>(type: "bit", nullable: false),
                    IsNotNull = table.Column<bool>(type: "bit", nullable: false),
                    RequestType = table.Column<int>(type: "int", nullable: false),
                    BodyType = table.Column<int>(type: "int", nullable: false),
                    Leaf = table.Column<bool>(type: "bit", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
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
                    table.PrimaryKey("PK_code_ApiInputParam", x => x.Id);
                    table.ForeignKey(
                        name: "FK_code_ApiInputParam_code_ApiDefinition_ApiDefId",
                        column: x => x.ApiDefId,
                        principalSchema: "cTest",
                        principalTable: "code_ApiDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_code_ApiInputParam_code_ApiInputParam_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "cTest",
                        principalTable: "code_ApiInputParam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "code_ApiOutParam",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApiDefId = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    TreeCode = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    DataType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsArray = table.Column<bool>(type: "bit", nullable: false),
                    ReturnType = table.Column<int>(type: "int", nullable: false),
                    Leaf = table.Column<bool>(type: "bit", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
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
                    table.PrimaryKey("PK_code_ApiOutParam", x => x.Id);
                    table.ForeignKey(
                        name: "FK_code_ApiOutParam_code_ApiDefinition_ApiDefId",
                        column: x => x.ApiDefId,
                        principalSchema: "cTest",
                        principalTable: "code_ApiDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_code_ApiOutParam_code_ApiOutParam_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "cTest",
                        principalTable: "code_ApiOutParam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "code_ModelDefField",
                schema: "cTest",
                columns: table => new
                {
                    PropertyAttributeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModelDefId = table.Column<int>(type: "int", nullable: false),
                    DataType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsPrimaryKey = table.Column<bool>(type: "bit", nullable: false),
                    PrimaryKeyType = table.Column<int>(type: "int", nullable: true),
                    IsNotNull = table.Column<bool>(type: "bit", nullable: false),
                    IsUnique = table.Column<bool>(type: "bit", nullable: false),
                    IsExecutor = table.Column<bool>(type: "bit", nullable: false),
                    IsCondition = table.Column<bool>(type: "bit", nullable: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    IsRequire = table.Column<bool>(type: "bit", nullable: false),
                    RelateObjectId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    RelateObjFieldId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
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
                    table.PrimaryKey("PK_code_ModelDefField", x => x.PropertyAttributeId);
                    table.ForeignKey(
                        name: "FK_code_ModelDefField_code_ModelDefinition_ModelDefId",
                        column: x => x.ModelDefId,
                        principalSchema: "cTest",
                        principalTable: "code_ModelDefinition",
                        principalColumn: "PropertyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "code_RelationDefDetail",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RelationType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MainModelDefFieldId = table.Column<int>(type: "int", nullable: false),
                    SubModelDefId = table.Column<int>(type: "int", nullable: false),
                    SubModelDefFieldId = table.Column<int>(type: "int", nullable: false),
                    RelationDefId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_code_RelationDefDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_code_RelationDefDetail_code_RelationDefinition_RelationDefId",
                        column: x => x.RelationDefId,
                        principalSchema: "cTest",
                        principalTable: "code_RelationDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_code_ApiDefinition_CategoryId",
                schema: "cTest",
                table: "code_ApiDefinition",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_code_ApiInputParam_ApiDefId",
                schema: "cTest",
                table: "code_ApiInputParam",
                column: "ApiDefId");

            migrationBuilder.CreateIndex(
                name: "IX_code_ApiInputParam_ParentId",
                schema: "cTest",
                table: "code_ApiInputParam",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_code_ApiOutParam_ApiDefId",
                schema: "cTest",
                table: "code_ApiOutParam",
                column: "ApiDefId");

            migrationBuilder.CreateIndex(
                name: "IX_code_ApiOutParam_ParentId",
                schema: "cTest",
                table: "code_ApiOutParam",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_code_ModelCategory_ParentId",
                schema: "cTest",
                table: "code_ModelCategory",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_code_ModelDefField_ModelDefId",
                schema: "cTest",
                table: "code_ModelDefField",
                column: "ModelDefId");

            migrationBuilder.CreateIndex(
                name: "IX_code_ModelDefinition_CategoryId",
                schema: "cTest",
                table: "code_ModelDefinition",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_code_RelationDefDetail_RelationDefId",
                schema: "cTest",
                table: "code_RelationDefDetail",
                column: "RelationDefId");

            migrationBuilder.CreateIndex(
                name: "IX_code_RelationDefinition_CategoryId",
                schema: "cTest",
                table: "code_RelationDefinition",
                column: "CategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "code_ApiInputParam",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "code_ApiOutParam",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "code_ModelChangeLog",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "code_ModelDefField",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "code_RelationDefDetail",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "code_ApiDefinition",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "code_ModelDefinition",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "code_RelationDefinition",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "code_ModelCategory",
                schema: "cTest");
        }
    }
}
