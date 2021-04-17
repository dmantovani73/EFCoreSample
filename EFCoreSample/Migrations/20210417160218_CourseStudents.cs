using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EFCoreSample.Migrations
{
    public partial class CourseStudents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseStudent_Courses_CoursesId",
                table: "CourseStudent");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseStudent_Students_StudentsId",
                table: "CourseStudent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseStudent",
                table: "CourseStudent");

            migrationBuilder.RenameTable(
                name: "CourseStudent",
                newName: "CourseStudents");

            migrationBuilder.RenameColumn(
                name: "StudentsId",
                table: "CourseStudents",
                newName: "StudentId");

            migrationBuilder.RenameColumn(
                name: "CoursesId",
                table: "CourseStudents",
                newName: "CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseStudent_StudentsId",
                table: "CourseStudents",
                newName: "IX_CourseStudents_StudentId");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfRegistration",
                table: "CourseStudents",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseStudents",
                table: "CourseStudents",
                columns: new[] { "CourseId", "StudentId" });

            migrationBuilder.CreateTable(
                name: "CourseStudent1",
                columns: table => new
                {
                    CoursesId = table.Column<int>(type: "INTEGER", nullable: false),
                    StudentsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseStudent1", x => new { x.CoursesId, x.StudentsId });
                    table.ForeignKey(
                        name: "FK_CourseStudent1_Courses_CoursesId",
                        column: x => x.CoursesId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseStudent1_Students_StudentsId",
                        column: x => x.StudentsId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseStudent1_StudentsId",
                table: "CourseStudent1",
                column: "StudentsId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseStudents_Courses_CourseId",
                table: "CourseStudents",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseStudents_Students_StudentId",
                table: "CourseStudents",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseStudents_Courses_CourseId",
                table: "CourseStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseStudents_Students_StudentId",
                table: "CourseStudents");

            migrationBuilder.DropTable(
                name: "CourseStudent1");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseStudents",
                table: "CourseStudents");

            migrationBuilder.DropColumn(
                name: "DateOfRegistration",
                table: "CourseStudents");

            migrationBuilder.RenameTable(
                name: "CourseStudents",
                newName: "CourseStudent");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "CourseStudent",
                newName: "StudentsId");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "CourseStudent",
                newName: "CoursesId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseStudents_StudentId",
                table: "CourseStudent",
                newName: "IX_CourseStudent_StudentsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseStudent",
                table: "CourseStudent",
                columns: new[] { "CoursesId", "StudentsId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CourseStudent_Courses_CoursesId",
                table: "CourseStudent",
                column: "CoursesId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseStudent_Students_StudentsId",
                table: "CourseStudent",
                column: "StudentsId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
