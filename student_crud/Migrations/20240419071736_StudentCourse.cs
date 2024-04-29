using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace student_crud.Migrations
{
    /// <inheritdoc />
    public partial class StudentCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassSections_Classes_ClassId",
                table: "ClassSections");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassSubjects_Classes_ClassId",
                table: "ClassSubjects");

            migrationBuilder.DropIndex(
                name: "IX_ClassSubjects_ClassId",
                table: "ClassSubjects");

            migrationBuilder.DropIndex(
                name: "IX_ClassSections_ClassId",
                table: "ClassSections");

            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SectionId",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "Students");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSubjects_ClassId",
                table: "ClassSubjects",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSections_ClassId",
                table: "ClassSections",
                column: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassSections_Classes_ClassId",
                table: "ClassSections",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassSubjects_Classes_ClassId",
                table: "ClassSubjects",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
