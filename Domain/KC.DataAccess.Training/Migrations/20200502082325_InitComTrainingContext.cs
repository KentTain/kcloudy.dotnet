using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KC.DataAccess.Training.Migrations
{
    public partial class InitComTrainingContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cTest");

            migrationBuilder.CreateTable(
                name: "trn_Book",
                schema: "cTest",
                columns: table => new
                {
                    BookId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trn_Book", x => x.BookId);
                });

            migrationBuilder.CreateTable(
                name: "trn_ClassRoom",
                schema: "cTest",
                columns: table => new
                {
                    ClassRoomId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trn_ClassRoom", x => x.ClassRoomId);
                });

            migrationBuilder.CreateTable(
                name: "trn_CourseRecord",
                schema: "cTest",
                columns: table => new
                {
                    CourseRecordId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurriculumId = table.Column<int>(nullable: false),
                    ClassRoomId = table.Column<int>(nullable: false),
                    ClassRoom = table.Column<string>(nullable: true),
                    CourseId = table.Column<int>(nullable: false),
                    Course = table.Column<string>(nullable: true),
                    TeacherId = table.Column<int>(nullable: false),
                    TeacherName = table.Column<string>(nullable: true),
                    StudentId = table.Column<int>(nullable: false),
                    StudentName = table.Column<string>(nullable: true),
                    CreatedDateTime = table.Column<DateTime>(nullable: false),
                    ConfirmDateTime = table.Column<DateTime>(nullable: false),
                    CourseDuration = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Remark = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trn_CourseRecord", x => x.CourseRecordId);
                });

            migrationBuilder.CreateTable(
                name: "trn_Student",
                schema: "cTest",
                columns: table => new
                {
                    StudentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Sex = table.Column<string>(nullable: true),
                    Mobile = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    IdentityCard = table.Column<string>(maxLength: 12, nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trn_Student", x => x.StudentId);
                });

            migrationBuilder.CreateTable(
                name: "trn_Teacher",
                schema: "cTest",
                columns: table => new
                {
                    TeacherId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Sex = table.Column<string>(nullable: true),
                    Mobile = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    IdentityCard = table.Column<string>(maxLength: 12, nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trn_Teacher", x => x.TeacherId);
                });

            migrationBuilder.CreateTable(
                name: "trn_Course",
                schema: "cTest",
                columns: table => new
                {
                    CourseId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    StudentLimit = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    TeacherId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trn_Course", x => x.CourseId);
                    table.ForeignKey(
                        name: "FK_trn_Course_trn_Teacher_TeacherId",
                        column: x => x.TeacherId,
                        principalSchema: "cTest",
                        principalTable: "trn_Teacher",
                        principalColumn: "TeacherId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trn_BooksInCourses",
                schema: "cTest",
                columns: table => new
                {
                    BookId = table.Column<int>(nullable: false),
                    CourseId = table.Column<int>(nullable: false),
                    BookId2 = table.Column<int>(nullable: false),
                    CourseId2 = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trn_BooksInCourses", x => new { x.BookId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_trn_BooksInCourses_trn_Book_BookId",
                        column: x => x.BookId,
                        principalSchema: "cTest",
                        principalTable: "trn_Book",
                        principalColumn: "BookId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trn_BooksInCourses_trn_Course_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "cTest",
                        principalTable: "trn_Course",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "trn_Curriculum",
                schema: "cTest",
                columns: table => new
                {
                    CurriculumId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourceDate = table.Column<DateTime>(nullable: false),
                    CourceDuration = table.Column<int>(nullable: false),
                    QrCode = table.Column<string>(nullable: true),
                    ClassRoomId = table.Column<int>(nullable: false),
                    ClassRoomId1 = table.Column<int>(nullable: true),
                    CourseId = table.Column<int>(nullable: false),
                    CourseId1 = table.Column<int>(nullable: true),
                    ClassRoomId2 = table.Column<int>(nullable: false),
                    CourseId2 = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trn_Curriculum", x => x.CurriculumId);
                    table.ForeignKey(
                        name: "FK_trn_Curriculum_trn_ClassRoom_ClassRoomId",
                        column: x => x.ClassRoomId,
                        principalSchema: "cTest",
                        principalTable: "trn_ClassRoom",
                        principalColumn: "ClassRoomId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trn_Curriculum_trn_ClassRoom_ClassRoomId1",
                        column: x => x.ClassRoomId1,
                        principalSchema: "cTest",
                        principalTable: "trn_ClassRoom",
                        principalColumn: "ClassRoomId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trn_Curriculum_trn_Course_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "cTest",
                        principalTable: "trn_Course",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trn_Curriculum_trn_Course_CourseId1",
                        column: x => x.CourseId1,
                        principalSchema: "cTest",
                        principalTable: "trn_Course",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "trn_StudentsInCourses",
                schema: "cTest",
                columns: table => new
                {
                    StudentId = table.Column<int>(nullable: false),
                    CourseId = table.Column<int>(nullable: false),
                    CourseId2 = table.Column<int>(nullable: false),
                    StudentId2 = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trn_StudentsInCourses", x => new { x.StudentId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_trn_StudentsInCourses_trn_Course_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "cTest",
                        principalTable: "trn_Course",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trn_StudentsInCourses_trn_Student_StudentId",
                        column: x => x.StudentId,
                        principalSchema: "cTest",
                        principalTable: "trn_Student",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "trn_TeachersInCourses",
                schema: "cTest",
                columns: table => new
                {
                    TeacherId = table.Column<int>(nullable: false),
                    CourseId = table.Column<int>(nullable: false),
                    CourseId2 = table.Column<int>(nullable: false),
                    TeacherId2 = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trn_TeachersInCourses", x => new { x.TeacherId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_trn_TeachersInCourses_trn_Course_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "cTest",
                        principalTable: "trn_Course",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trn_TeachersInCourses_trn_Teacher_TeacherId",
                        column: x => x.TeacherId,
                        principalSchema: "cTest",
                        principalTable: "trn_Teacher",
                        principalColumn: "TeacherId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_trn_BooksInCourses_CourseId",
                schema: "cTest",
                table: "trn_BooksInCourses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_trn_Course_TeacherId",
                schema: "cTest",
                table: "trn_Course",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_trn_Curriculum_ClassRoomId",
                schema: "cTest",
                table: "trn_Curriculum",
                column: "ClassRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_trn_Curriculum_ClassRoomId1",
                schema: "cTest",
                table: "trn_Curriculum",
                column: "ClassRoomId1");

            migrationBuilder.CreateIndex(
                name: "IX_trn_Curriculum_CourseId",
                schema: "cTest",
                table: "trn_Curriculum",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_trn_Curriculum_CourseId1",
                schema: "cTest",
                table: "trn_Curriculum",
                column: "CourseId1");

            migrationBuilder.CreateIndex(
                name: "IX_trn_StudentsInCourses_CourseId",
                schema: "cTest",
                table: "trn_StudentsInCourses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_trn_TeachersInCourses_CourseId",
                schema: "cTest",
                table: "trn_TeachersInCourses",
                column: "CourseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trn_BooksInCourses",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "trn_CourseRecord",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "trn_Curriculum",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "trn_StudentsInCourses",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "trn_TeachersInCourses",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "trn_Book",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "trn_ClassRoom",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "trn_Student",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "trn_Course",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "trn_Teacher",
                schema: "cTest");
        }
    }
}
